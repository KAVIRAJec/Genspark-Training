using System;
using System.Text;
using System.Threading.Tasks;
using Freelance_Project.Contexts;
using Freelance_Project.Interfaces;
using Freelance_Project.Misc;
using Freelance_Project.Models;
using Freelance_Project.Models.DTO;
using Freelance_Project.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;

namespace Freelance_Project.Test.Services
{
    [TestFixture]
    public class AuthenticationServiceTest
    {
        private Mock<IRepository<string, User>> _userRepoMock;
        private Mock<ITokenService> _tokenServiceMock;
        private Mock<IHashingService> _hashingServiceMock;
        private IConfiguration _config;
        private FreelanceDBContext _dbContext;
        private AuthenticationService _authService;

        [SetUp]
        public void Setup()
        {
            var inMemoryDb = new DbContextOptionsBuilder<FreelanceDBContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _dbContext = new FreelanceDBContext(inMemoryDb);

            _userRepoMock = new Mock<IRepository<string, User>>();
            _tokenServiceMock = new Mock<ITokenService>();
            _hashingServiceMock = new Mock<IHashingService>();

            var dict = new System.Collections.Generic.Dictionary<string, string>
            {
                {"JWT:Key", "supersecretkeysupersecretkey123456"},
                {"JWT:ExpiresInHours", "1"},
                {"JWT:RefreshTokenExpiresInDays", "7"}
            };
            _config = new ConfigurationBuilder().AddInMemoryCollection(dict).Build();

            _authService = new AuthenticationService(
                _userRepoMock.Object,
                _tokenServiceMock.Object,
                _hashingServiceMock.Object,
                _config,
                _dbContext
            );
        }

        [Test]
        public void Login_NullRequest_ThrowsAppException()
        {
            var ex = Assert.ThrowsAsync<AppException>(async () => await _authService.Login(null));
            Assert.AreEqual(400, ex.StatusCode);
        }

        [Test]
        public void Login_UserNotFound_ThrowsAppException()
        {
            _userRepoMock.Setup(r => r.Get(It.IsAny<string>())).ReturnsAsync((User)null);
            var dto = new LoginRequestDTO { Email = "test@example.com", Password = "pass" };
            var ex = Assert.ThrowsAsync<AppException>(async () => await _authService.Login(dto));
            Assert.AreEqual(404, ex.StatusCode);
        }

        [Test]
        public void Login_UserInactive_ThrowsAppException()
        {
            _userRepoMock.Setup(r => r.Get(It.IsAny<string>())).ReturnsAsync(new User { IsActive = false });
            var dto = new LoginRequestDTO { Email = "test@example.com", Password = "pass" };
            var ex = Assert.ThrowsAsync<AppException>(async () => await _authService.Login(dto));
            Assert.AreEqual(404, ex.StatusCode);
        }

        [Test]
        public void Login_InvalidPassword_ThrowsAppException()
        {
            _userRepoMock.Setup(r => r.Get(It.IsAny<string>())).ReturnsAsync(new User
            {
                Email = "test@example.com",
                Password = Encoding.UTF8.GetBytes("hash"),
                HashKey = Encoding.UTF8.GetBytes("key"),
                IsActive = true
            });
            _hashingServiceMock.Setup(h => h.VerifyHash(It.IsAny<HashingModel>())).ReturnsAsync(false);

            var dto = new LoginRequestDTO { Email = "test@example.com", Password = "wrongpass" };
            var ex = Assert.ThrowsAsync<AppException>(async () => await _authService.Login(dto));
            Assert.AreEqual(401, ex.StatusCode);
        }

        [Test]
        public void Login_TokenGenerationFails_ThrowsAppException()
        {
            var user = new User
            {
                Email = "test@example.com",
                Password = Encoding.UTF8.GetBytes("hash"),
                HashKey = Encoding.UTF8.GetBytes("key"),
                IsActive = true,
                Role = "Freelancer"
            };
            _userRepoMock.Setup(r => r.Get(It.IsAny<string>())).ReturnsAsync(user);
            _hashingServiceMock.Setup(h => h.VerifyHash(It.IsAny<HashingModel>())).ReturnsAsync(true);
            _tokenServiceMock.Setup(t => t.GenerateToken(user)).ReturnsAsync((string)null);
            _tokenServiceMock.Setup(t => t.GenerateRefreshToken(user.Email)).ReturnsAsync((string)null);

            var dto = new LoginRequestDTO { Email = "test@example.com", Password = "pass" };
            var ex = Assert.ThrowsAsync<AppException>(async () => await _authService.Login(dto));
            Assert.AreEqual(500, ex.StatusCode);
        }

        [Test]
        public async Task Login_Success_ReturnsLoginResponseDTO()
        {
            var user = new User
            {
                Email = "test@example.com",
                Password = Encoding.UTF8.GetBytes("hash"),
                HashKey = Encoding.UTF8.GetBytes("key"),
                IsActive = true,
                Role = "Freelancer"
            };
            _userRepoMock.Setup(r => r.Get(It.IsAny<string>())).ReturnsAsync(user);
            _hashingServiceMock.Setup(h => h.VerifyHash(It.IsAny<HashingModel>())).ReturnsAsync(true);
            _tokenServiceMock.Setup(t => t.GenerateToken(user)).ReturnsAsync("jwt-token");
            _tokenServiceMock.Setup(t => t.GenerateRefreshToken(user.Email)).ReturnsAsync("refresh-token");

            var dto = new LoginRequestDTO { Email = "test@example.com", Password = "pass" };
            var result = await _authService.Login(dto);

            Assert.IsNotNull(result);
            Assert.AreEqual(user.Email, result.Email);
            Assert.AreEqual(user.Role, result.Role);
            Assert.AreEqual("jwt-token", result.Token);
            Assert.AreEqual("refresh-token", result.RefreshToken);
        }

        [Test]
        public void Logout_EmptyEmail_ThrowsAppException()
        {
            var ex = Assert.ThrowsAsync<AppException>(async () => await _authService.Logout(""));
            Assert.AreEqual(400, ex.StatusCode);
        }

        [Test]
        public void Logout_UserNotFound_ThrowsAppException()
        {
            _userRepoMock.Setup(r => r.Get(It.IsAny<string>())).ReturnsAsync((User)null);
            var ex = Assert.ThrowsAsync<AppException>(async () => await _authService.Logout("notfound@example.com"));
            Assert.AreEqual(404, ex.StatusCode);
        }

        [Test]
        public async Task Logout_Success_ReturnsTrue()
        {
            var user = new User { Email = "test@example.com", IsActive = true };
            _userRepoMock.Setup(r => r.Get(user.Email)).ReturnsAsync(user);
            _dbContext.RefreshTokens.Add(new RefreshToken { Email = user.Email, Token = "tok", Expires = DateTime.UtcNow.AddDays(1) });
            await _dbContext.SaveChangesAsync();

            var result = await _authService.Logout(user.Email);
            Assert.IsTrue(result);
            Assert.IsEmpty(_dbContext.RefreshTokens.Where(rt => rt.Email == user.Email));
        }

        [Test]
        public void RefreshToken_NullToken_ThrowsAppException()
        {
            var ex = Assert.ThrowsAsync<AppException>(async () => await _authService.RefreshToken(null));
            Assert.AreEqual(400, ex.StatusCode);
        }

        [Test]
        public void RefreshToken_ValidateFails_ThrowsAppException()
        {
            _tokenServiceMock.Setup(t => t.ValidateRefreshToken(It.IsAny<string>())).ReturnsAsync((string)null);
            var ex = Assert.ThrowsAsync<AppException>(async () => await _authService.RefreshToken("badtoken"));
            Assert.AreEqual(401, ex.StatusCode);
        }

        [Test]
        public void RefreshToken_EmailNotFound_ThrowsAppException()
        {
            _tokenServiceMock.Setup(t => t.ValidateRefreshToken(It.IsAny<string>())).ReturnsAsync("token");
            var ex = Assert.ThrowsAsync<AppException>(async () => await _authService.RefreshToken("sometoken"));
            Assert.AreEqual(401, ex.StatusCode);
        }

        [Test]
        public void RefreshToken_UserNotFound_ThrowsAppException()
        {
            _tokenServiceMock.Setup(t => t.ValidateRefreshToken(It.IsAny<string>())).ReturnsAsync("token");
            _dbContext.RefreshTokens.Add(new RefreshToken { Token = "token", Email = "notfound@example.com", Expires = DateTime.UtcNow.AddDays(1) });
            _dbContext.SaveChanges();
            _userRepoMock.Setup(r => r.Get(It.IsAny<string>())).ReturnsAsync((User)null);

            var ex = Assert.ThrowsAsync<AppException>(async () => await _authService.RefreshToken("token"));
            Assert.AreEqual(404, ex.StatusCode);
        }

        [Test]
        public async Task RefreshToken_Success_ReturnsLoginResponseDTO()
        {
            var user = new User
            {
                Email = "test@example.com",
                Password = Encoding.UTF8.GetBytes("hash"),
                HashKey = Encoding.UTF8.GetBytes("key"),
                IsActive = true,
                Role = "Freelancer"
            };
            _tokenServiceMock.Setup(t => t.ValidateRefreshToken(It.IsAny<string>())).ReturnsAsync("token");
            _dbContext.RefreshTokens.Add(new RefreshToken { Token = "token", Email = user.Email, Expires = DateTime.UtcNow.AddDays(1) });
            _dbContext.SaveChanges();
            _userRepoMock.Setup(r => r.Get(user.Email)).ReturnsAsync(user);
            _tokenServiceMock.Setup(t => t.GenerateToken(user)).ReturnsAsync("jwt-token");

            var result = await _authService.RefreshToken("token");
            Assert.IsNotNull(result);
            Assert.AreEqual(user.Email, result.Email);
            Assert.AreEqual(user.Role, result.Role);
            Assert.AreEqual("jwt-token", result.Token);
            Assert.AreEqual("token", result.RefreshToken);
        }

        [Test]
        public void GetDetails_EmptyEmail_ThrowsAppException()
        {
            var ex = Assert.ThrowsAsync<AppException>(async () => await _authService.GetDetails<User>(""));
            Assert.AreEqual(400, ex.StatusCode);
        }

        [Test]
        public void GetDetails_UserNotFound_ThrowsAppException()
        {
            _userRepoMock.Setup(r => r.Get(It.IsAny<string>())).ReturnsAsync((User)null);
            var ex = Assert.ThrowsAsync<AppException>(async () => await _authService.GetDetails<User>("notfound@example.com"));
            Assert.AreEqual(404, ex.StatusCode);
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext?.Dispose();
        }
    }
}