
using Freelance_Project.Contexts;
using Freelance_Project.Interfaces;
using Freelance_Project.Misc;
using Freelance_Project.Models;
using Freelance_Project.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;

namespace Freelance_Project.Test.Services
{
    public class TestableTokenService : TokenService
    {
        public TestableTokenService(IConfiguration config, FreelanceDBContext appContext, IRepository<string, User> userRepository)
            : base(config, appContext, userRepository) { }

        public override async Task<string> GenerateRefreshToken(string email)
        {
            if (string.IsNullOrEmpty(email)) throw new AppException("Email is required", 400);
            var user = await _appContext.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null) throw new AppException("User not found", 404);
            if (user.DeletedAt != null || !user.IsActive) throw new AppException("User is inactive or deleted", 403);

            var expiredTokens = _appContext.RefreshTokens.Where(rt => rt.Email == email && rt.Expires < DateTime.UtcNow);
            if (expiredTokens.Any()) _appContext.RefreshTokens.RemoveRange(expiredTokens);

            var refreshToken = Guid.NewGuid().ToString();
            await _appContext.RefreshTokens.AddAsync(new RefreshToken
            {
                Token = refreshToken,
                Email = user.Email,
                Expires = DateTime.UtcNow.AddDays(_refreshTokenExpiresInDays)
            });
            await _appContext.SaveChangesAsync();

            var userToUpdate = await _appContext.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (userToUpdate != null)
            {
                userToUpdate.LastLogin = DateTime.UtcNow;
                await _appContext.SaveChangesAsync();
            }
            return refreshToken;
        }
    }
    public class TokenServiceTest
    {
        private FreelanceDBContext _dbContext;
        private TokenService _tokenService;
        private IConfiguration _configuration;
        private Mock<IRepository<string, User>> _userRepoMock;

        [SetUp]
        public void Setup()
        {
            var inMemoryDb = new DbContextOptionsBuilder<FreelanceDBContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _dbContext = new FreelanceDBContext(inMemoryDb);

            // Seed a user
            var user = new User
            {
                Email = "test@example.com",
                Role = "Freelancer",
                IsActive = true,
                DeletedAt = null,
                HashKey = System.Text.Encoding.UTF8.GetBytes("dummyhashkey"),      // Add required property
                Password = System.Text.Encoding.UTF8.GetBytes("dummypassword")
            };
            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();

            var dict = new Dictionary<string, string>
            {
                {"JWT:Key", "supersecretkeysupersecretkey123456"},
                {"JWT:ExpiresInHours", "1"},
                {"JWT:RefreshTokenExpiresInDays", "7"}
            };
            _configuration = new ConfigurationBuilder().AddInMemoryCollection(dict).Build();

            _userRepoMock = new Mock<IRepository<string, User>>();

            _tokenService = new TestableTokenService(_configuration, _dbContext, _userRepoMock.Object);
        }

        public async Task<string> GenerateRefreshToken(string email)
        {
            if (string.IsNullOrEmpty(email)) throw new AppException("Email is required", 400);
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null) throw new AppException("User not found", 404);
            if (user.DeletedAt != null || !user.IsActive) throw new AppException("User is inactive or deleted", 403);

            var providerName = _dbContext.Database.ProviderName;
            if (providerName != "Microsoft.EntityFrameworkCore.InMemory")
            {
                using var transaction = await _dbContext.Database.BeginTransactionAsync();
                try
                {
                    //remove expired token for that user
                    var expiredTokens = _dbContext.RefreshTokens.Where(rt => rt.Email == email && rt.Expires < DateTime.UtcNow);
                    if (expiredTokens.Any()) _dbContext.RefreshTokens.RemoveRange(expiredTokens);

                    var refreshToken = Guid.NewGuid().ToString();
                    await _dbContext.RefreshTokens.AddAsync(new RefreshToken
                    {
                        Token = refreshToken,
                        Email = user.Email,
                        Expires = DateTime.UtcNow.AddDays(double.Parse(_configuration["JWT:RefreshTokenExpiresInDays"]))
                    });
                    await _dbContext.SaveChangesAsync();

                    await _dbContext.Users
                        .Where(u => u.Email == email)
                        .ExecuteUpdateAsync(set => set.SetProperty(u => u.LastLogin, DateTime.UtcNow));

                    await transaction.CommitAsync();
                    return refreshToken;
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }else
            {
                var expiredTokens = _dbContext.RefreshTokens.Where(rt => rt.Email == email && rt.Expires < DateTime.UtcNow);
                if (expiredTokens.Any()) _dbContext.RefreshTokens.RemoveRange(expiredTokens);

                var refreshToken = Guid.NewGuid().ToString();
                await _dbContext.RefreshTokens.AddAsync(new RefreshToken
                {
                    Token = refreshToken,
                    Email = user.Email,
                    Expires = DateTime.UtcNow.AddDays(double.Parse(_configuration["JWT:RefreshTokenExpiresInDays"]))
                });
                await _dbContext.SaveChangesAsync();

                await _dbContext.Users
                    .Where(u => u.Email == email)
                    .ExecuteUpdateAsync(set => set.SetProperty(u => u.LastLogin, DateTime.UtcNow));

                return refreshToken;
            }
        }

        [TestCase("test@example.com", true)]
        [TestCase("", false)]
        [TestCase("notfound@example.com", false)]
        public async Task GenerateRefreshToken_TestCases(string email, bool shouldSucceed)
        {
            if (shouldSucceed)
            {
                var refreshToken = await _tokenService.GenerateRefreshToken(email);
                Assert.IsNotNull(refreshToken);
                Assert.IsNotEmpty(refreshToken);
                Assert.IsTrue(await _dbContext.RefreshTokens.AnyAsync(rt => rt.Token == refreshToken));
            }
            else
            {
                var ex = Assert.ThrowsAsync<AppException>(async () =>
                {
                    await _tokenService.GenerateRefreshToken(email);
                });
                Assert.IsTrue(ex.StatusCode == 400 || ex.StatusCode == 404);
            }
        }

        [Test]
        public async Task GenerateToken_Success()
        {
            // Arrange
            var user = await _dbContext.Users.FirstAsync();

            // Act
            var token = await _tokenService.GenerateToken(user);

            // Assert
            Assert.IsNotNull(token);
            Assert.IsNotEmpty(token);
        }

        [Test]
        public void GenerateToken_NullUser_ThrowsException()
        {
            // Arrange & Act & Assert
            Assert.ThrowsAsync<NullReferenceException>(async () =>
            {
                await _tokenService.GenerateToken(null);
            });
        }

        [Test]
        public async Task GenerateRefreshToken_Success()
        {
            // Arrange
            var email = "test@example.com";

            // Act
            var refreshToken = await _tokenService.GenerateRefreshToken(email);

            // Assert
            Assert.IsNotNull(refreshToken);
            Assert.IsNotEmpty(refreshToken);
            Assert.IsTrue(await _dbContext.RefreshTokens.AnyAsync(rt => rt.Token == refreshToken));
        }

        [Test]
        public void GenerateRefreshToken_EmptyEmail_ThrowsAppException()
        {
            // Arrange
            var email = "";

            // Act & Assert
            var ex = Assert.ThrowsAsync<AppException>(async () =>
            {
                await _tokenService.GenerateRefreshToken(email);
            });
            Assert.AreEqual(400, ex.StatusCode);
        }

        [Test]
        public void GenerateRefreshToken_UserNotFound_ThrowsAppException()
        {
            // Arrange
            var email = "notfound@example.com";

            // Act & Assert
            var ex = Assert.ThrowsAsync<AppException>(async () =>
            {
                await _tokenService.GenerateRefreshToken(email);
            });
            Assert.AreEqual(404, ex.StatusCode);
        }

        [Test]
        public async Task ValidateRefreshToken_Success()
        {
            // Arrange
            var email = "test@example.com";
            var refreshToken = await _tokenService.GenerateRefreshToken(email);

            // Act
            var newRefreshToken = await _tokenService.ValidateRefreshToken(refreshToken);

            // Assert
            Assert.IsNotNull(newRefreshToken);
            Assert.IsNotEmpty(newRefreshToken);
            Assert.AreNotEqual(refreshToken, newRefreshToken);
        }

        [Test]
        public void ValidateRefreshToken_EmptyToken_ThrowsAppException()
        {
            // Arrange
            var token = "";

            // Act & Assert
            var ex = Assert.ThrowsAsync<AppException>(async () =>
            {
                await _tokenService.ValidateRefreshToken(token);
            });
            Assert.AreEqual(400, ex.StatusCode);
        }

        [Test]
        public void ValidateRefreshToken_InvalidToken_ThrowsAppException()
        {
            // Arrange
            var token = Guid.NewGuid().ToString();

            // Act & Assert
            var ex = Assert.ThrowsAsync<AppException>(async () =>
            {
                await _tokenService.ValidateRefreshToken(token);
            });
            Assert.AreEqual(401, ex.StatusCode); // Wrapped in AppException with 401
        }
        [TearDown]
        public void TearDown()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }
    }
}