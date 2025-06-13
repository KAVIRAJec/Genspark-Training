using System;
using System.Threading.Tasks;
using Freelance_Project.Contexts;
using Freelance_Project.Interfaces;
using Freelance_Project.Misc;
using Freelance_Project.Models;
using Freelance_Project.Models.DTO;
using Freelance_Project.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace Freelance_Project.Test.Services
{
    public class TestableClientService : ClientService
    {
        private readonly IRepository<string, User> _userRepository;
        private readonly IRepository<Guid, Client> _clientRepository;
        private readonly FreelanceDBContext _appContext;
        private readonly IImageUploadService _imageUploadService;

        public TestableClientService(
            IRepository<Guid, Client> clientRepository,
            IRepository<string, User> userRepository,
            FreelanceDBContext appContext,
            IImageUploadService imageUploadService)
            : base(clientRepository, userRepository, appContext, imageUploadService)
        {
            _userRepository = userRepository;
            _clientRepository = clientRepository;
            _appContext = appContext;
            _imageUploadService = imageUploadService;
        }

        public override async Task<ClientResponseDTO> CreateClient(CreateClientDTO createClientDTO)
        {
            var user = await UserMapper.CreateUserFromCreateClientDTO(createClientDTO);
            var newUser = await _userRepository.Add(user);
            if (newUser == null) throw new AppException("Unable to create user.", 500);

            var client = ClientMapper.CreateClientFromCreateDTO(createClientDTO);
            client.Email = newUser.Email;

            var response = await _clientRepository.Add(client);
            if (response == null) throw new AppException("Unable to create client.", 500);

            return ClientMapper.ToResponseDTO(response);
        }

        public override async Task<ClientResponseDTO> DeleteClient(Guid clientId)
        {
            if (clientId == Guid.Empty) throw new AppException("Client ID is required.", 400);
            var client = await _clientRepository.Get(clientId);
            if (client == null || client.IsActive == false) throw new AppException("Client not found/ inactive.", 404);
            if (client.ProfileUrl != null) await _imageUploadService.DeleteImage(client.ProfileUrl);
            var response = await _clientRepository.Delete(clientId);
            if (response == null) throw new AppException("Unable to delete client.", 500);

            var user = await _userRepository.Get(client.Email);
            if (user == null || user.IsActive == false) throw new AppException("User not found/ inactive.", 404);
            var userResponse = await _userRepository.Delete(user.Email);
            if (userResponse == null) throw new AppException("Unable to delete user.", 500);

            return ClientMapper.ToResponseDTO(response);
        }
    }

    [TestFixture]
    public class ClientServiceTest
    {
        private Mock<IRepository<Guid, Client>> _clientRepoMock;
        private Mock<IRepository<string, User>> _userRepoMock;
        private Mock<IImageUploadService> _imageUploadMock;
        private FreelanceDBContext _dbContext;
        private ClientService _service;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<FreelanceDBContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _dbContext = new FreelanceDBContext(options);

            _clientRepoMock = new Mock<IRepository<Guid, Client>>();
            _userRepoMock = new Mock<IRepository<string, User>>();
            _imageUploadMock = new Mock<IImageUploadService>();

            _service = new TestableClientService(
                _clientRepoMock.Object,
                _userRepoMock.Object,
                _dbContext,
                _imageUploadMock.Object
            );
            _dbContext.Database.EnsureCreated();
        }

        [Test]
        public void CreateClient_UserAddFails_ThrowsAppException()
        {
            _userRepoMock.Setup(r => r.Add(It.IsAny<User>())).ReturnsAsync((User)null);
            var dto = new CreateClientDTO
            {
                Email = "test@example.com",
                Password = "password123",
            };
            var ex = Assert.ThrowsAsync<AppException>(async () => await _service.CreateClient(dto));
            Assert.That(ex.Message, Does.Contain("Unable to create user"));
        }

        [Test]
        public void CreateClient_ClientAddFails_ThrowsAppException()
        {
            _userRepoMock.Setup(r => r.Add(It.IsAny<User>())).ReturnsAsync(new User { Email = "test@example.com" });
            _clientRepoMock.Setup(r => r.Add(It.IsAny<Client>())).ReturnsAsync((Client)null);
            var dto = new CreateClientDTO
            {
                Email = "test@example.com",
                Password = "password123",
            };
            var ex = Assert.ThrowsAsync<AppException>(async () => await _service.CreateClient(dto));
            Assert.That(ex.Message, Does.Contain("Unable to create client"));
        }

        [Test]
        public async Task CreateClient_Success_ReturnsClientResponseDTO()
        {
            _userRepoMock.Setup(r => r.Add(It.IsAny<User>())).ReturnsAsync(new User { Email = "test@example.com" });
            _clientRepoMock.Setup(r => r.Add(It.IsAny<Client>())).ReturnsAsync(new Client { Id = Guid.NewGuid(), Email = "test@example.com", IsActive = true });
            var dto = new CreateClientDTO
            {
                Email = "test@example.com",
                Password = "password123",
            };
            var result = await _service.CreateClient(dto);
            Assert.IsNotNull(result);
            Assert.AreEqual("test@example.com", result.Email);
        }

        [Test]
        public void DeleteClient_EmptyId_ThrowsAppException()
        {
            var ex = Assert.ThrowsAsync<AppException>(async () => await _service.DeleteClient(Guid.Empty));
            Assert.That(ex.Message, Does.Contain("Client ID is required"));
        }

        [Test]
        public void DeleteClient_ClientNotFound_ThrowsAppException()
        {
            _clientRepoMock.Setup(r => r.Get(It.IsAny<Guid>())).ReturnsAsync((Client)null);
            var ex = Assert.ThrowsAsync<AppException>(async () => await _service.DeleteClient(Guid.NewGuid()));
            Assert.That(ex.Message, Does.Contain("Client not found"));
        }

        [Test]
        public void DeleteClient_UserNotFound_ThrowsAppException()
        {
            var client = new Client { Id = Guid.NewGuid(), Email = "test@example.com", IsActive = true };
            _clientRepoMock.Setup(r => r.Get(client.Id)).ReturnsAsync(client);
            _clientRepoMock.Setup(r => r.Delete(client.Id)).ReturnsAsync(client);
            _userRepoMock.Setup(r => r.Get(client.Email)).ReturnsAsync((User)null);

            var ex = Assert.ThrowsAsync<AppException>(async () => await _service.DeleteClient(client.Id));
            Assert.That(ex.Message, Does.Contain("User not found"));
        }

        [Test]
        public async Task DeleteClient_Success_ReturnsClientResponseDTO()
        {
            var client = new Client { Id = Guid.NewGuid(), Email = "test@example.com", IsActive = true };
            var user = new User { Email = "test@example.com", IsActive = true };
            _clientRepoMock.Setup(r => r.Get(client.Id)).ReturnsAsync(client);
            _clientRepoMock.Setup(r => r.Delete(client.Id)).ReturnsAsync(client);
            _userRepoMock.Setup(r => r.Get(client.Email)).ReturnsAsync(user);
            _userRepoMock.Setup(r => r.Delete(user.Email)).ReturnsAsync(user);

            var result = await _service.DeleteClient(client.Id);
            Assert.IsNotNull(result);
            Assert.AreEqual(client.Id, result.Id);
        }

        [Test]
        public async Task GetAllClientsPaged_ReturnsPagedResponse()
        {
            _dbContext.Clients.Add(new Client { Id = Guid.NewGuid(), Email = "a@a.com", IsActive = true, CreatedAt = DateTime.UtcNow });
            _dbContext.Clients.Add(new Client { Id = Guid.NewGuid(), Email = "b@b.com", IsActive = true, CreatedAt = DateTime.UtcNow.AddMinutes(-1) });
            _dbContext.SaveChanges();

            var result = await _service.GetAllClientsPaged(new PaginationParams { Page = 1, PageSize = 10 });
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Data.Count() > 0);
        }

        [Test]
        public void GetClientById_EmptyId_ThrowsAppException()
        {
            var ex = Assert.ThrowsAsync<AppException>(async () => await _service.GetClientById(Guid.Empty));
            Assert.That(ex.Message, Does.Contain("Client ID is required"));
        }

        [Test]
        public void GetClientById_NotFound_ThrowsAppException()
        {
            _clientRepoMock.Setup(r => r.Get(It.IsAny<Guid>())).ReturnsAsync((Client)null);
            var ex = Assert.ThrowsAsync<AppException>(async () => await _service.GetClientById(Guid.NewGuid()));
            Assert.That(ex.Message, Does.Contain("Client not found"));
        }

        [Test]
        public async Task GetClientById_Success_ReturnsClientResponseDTO()
        {
            var clientId = Guid.NewGuid();
            var client = new Client { Id = clientId, Email = "test@example.com", IsActive = true };
            _clientRepoMock.Setup(r => r.Get(clientId)).ReturnsAsync(client);

            var result = await _service.GetClientById(clientId);
            Assert.IsNotNull(result);
            Assert.AreEqual(clientId, result.Id);
        }

        [Test]
        public void UpdateClient_EmptyId_ThrowsAppException()
        {
            var ex = Assert.ThrowsAsync<AppException>(async () => await _service.UpdateClient(Guid.Empty, new UpdateClientDTO()));
            Assert.That(ex.Message, Does.Contain("Client ID is required"));
        }

        [Test]
        public void UpdateClient_NullDto_ThrowsAppException()
        {
            var ex = Assert.ThrowsAsync<AppException>(async () => await _service.UpdateClient(Guid.NewGuid(), null));
            Assert.That(ex.Message, Does.Contain("Client DTO is required"));
        }

        [Test]
        public void UpdateClient_NotFound_ThrowsAppException()
        {
            _clientRepoMock.Setup(r => r.Get(It.IsAny<Guid>())).ReturnsAsync((Client)null);
            var ex = Assert.ThrowsAsync<AppException>(async () => await _service.UpdateClient(Guid.NewGuid(), new UpdateClientDTO()));
            Assert.That(ex.Message, Does.Contain("Client not found"));
        }

        [Test]
        public async Task UpdateClient_Success_ReturnsClientResponseDTO()
        {
            var clientId = Guid.NewGuid();
            var client = new Client { Id = clientId, Email = "test@example.com", IsActive = true };
            _clientRepoMock.Setup(r => r.Get(clientId)).ReturnsAsync(client);
            _clientRepoMock.Setup(r => r.Update(clientId, It.IsAny<Client>())).ReturnsAsync(client);

            var dto = new UpdateClientDTO { Username = "UpdatedName" };
            var result = await _service.UpdateClient(clientId, dto);

            Assert.IsNotNull(result);
            Assert.AreEqual(clientId, result.Id);
            Assert.AreEqual("UpdatedName", result.Username);
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Dispose();
        }
    }
}