
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
    [TestFixture]
    public class FreelancerServiceTest
    {
        private Mock<IRepository<Guid, Freelancer>> _freelancerRepoMock;
        private Mock<IRepository<string, User>> _userRepoMock;
        private Mock<IGetOrCreateSkills> _skillsMock;
        private Mock<IImageUploadService> _imageUploadMock;
        private FreelanceDBContext _dbContext;
        private FreelancerService _service;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<FreelanceDBContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _dbContext = new FreelanceDBContext(options);

            _freelancerRepoMock = new Mock<IRepository<Guid, Freelancer>>();
            _userRepoMock = new Mock<IRepository<string, User>>();
            _skillsMock = new Mock<IGetOrCreateSkills>();
            _imageUploadMock = new Mock<IImageUploadService>();

            _service = new TestableFreelancerService(
                _freelancerRepoMock.Object,
                _userRepoMock.Object,
                _dbContext,
                _skillsMock.Object,
                _imageUploadMock.Object
            );
        }

        [Test]
        public void CreateFreelancer_UserAddFails_ThrowsAppException()
        {
            var dto = new CreateFreelancerDTO { Email = "test@x.com", Password = "pass" };
            _userRepoMock.Setup(r => r.Add(It.IsAny<User>())).ReturnsAsync((User)null);

            var ex = Assert.ThrowsAsync<AppException>(async () => await _service.CreateFreelancer(dto));
            Assert.That(ex.Message, Does.Contain("Unable to create user"));
        }

        [Test]
        public void CreateFreelancer_FreelancerAddFails_ThrowsAppException()
        {
            var dto = new CreateFreelancerDTO { Email = "test@x.com", Password = "pass" };
            _userRepoMock.Setup(r => r.Add(It.IsAny<User>())).ReturnsAsync(new User { Email = "test@x.com" });
            _skillsMock.Setup(s => s.GetOrCreateSkills(It.IsAny<IEnumerable<SkillDTO>>())).ReturnsAsync(new List<Skill>());
            _freelancerRepoMock.Setup(r => r.Add(It.IsAny<Freelancer>())).ReturnsAsync((Freelancer)null);

            var ex = Assert.ThrowsAsync<AppException>(async () => await _service.CreateFreelancer(dto));
            Assert.That(ex.Message, Does.Contain("Unable to create freelancer"));
        }

        [Test]
        public async Task CreateFreelancer_Success_ReturnsFreelancerResponseDTO()
        {
            var dto = new CreateFreelancerDTO { Email = "test@x.com", Password = "pass", Skills = new List<SkillDTO> { new SkillDTO { Name = "C#" } } };
            _userRepoMock.Setup(r => r.Add(It.IsAny<User>())).ReturnsAsync(new User { Email = "test@x.com" });
            _skillsMock.Setup(s => s.GetOrCreateSkills(It.IsAny<IEnumerable<SkillDTO>>())).ReturnsAsync(new List<Skill>());
            _freelancerRepoMock.Setup(r => r.Add(It.IsAny<Freelancer>())).ReturnsAsync(new Freelancer { Id = Guid.NewGuid(), Email = "test@x.com", IsActive = true });

            var result = await _service.CreateFreelancer(dto);
            Assert.IsNotNull(result);
            Assert.AreEqual("test@x.com", result.Email);
        }

        [Test]
        public void DeleteFreelancer_EmptyId_ThrowsAppException()
        {
            var ex = Assert.ThrowsAsync<AppException>(async () => await _service.DeleteFreelancer(Guid.Empty));
            Assert.That(ex.Message, Does.Contain("Freelancer ID is required"));
        }

        [Test]
        public void DeleteFreelancer_NotFound_ThrowsAppException()
        {
            _freelancerRepoMock.Setup(r => r.Get(It.IsAny<Guid>())).ReturnsAsync((Freelancer)null);
            var ex = Assert.ThrowsAsync<AppException>(async () => await _service.DeleteFreelancer(Guid.NewGuid()));
            Assert.That(ex.Message, Does.Contain("Freelancer not found"));
        }

        [Test]
        public void DeleteFreelancer_UserNotFound_ThrowsAppException()
        {
            var freelancerId = Guid.NewGuid();
            var freelancer = new Freelancer { Id = freelancerId, Email = "test@x.com", IsActive = true };
            _freelancerRepoMock.Setup(r => r.Get(freelancerId)).ReturnsAsync(freelancer);
            _freelancerRepoMock.Setup(r => r.Delete(freelancerId)).ReturnsAsync(freelancer);
            _userRepoMock.Setup(r => r.Get(freelancer.Email)).ReturnsAsync((User)null);

            var ex = Assert.ThrowsAsync<AppException>(async () => await _service.DeleteFreelancer(freelancerId));
            Assert.That(ex.Message, Does.Contain("User not found"));
        }

        [Test]
        public async Task DeleteFreelancer_Success_ReturnsFreelancerResponseDTO()
        {
            var freelancerId = Guid.NewGuid();
            var freelancer = new Freelancer { Id = freelancerId, Email = "test@x.com", IsActive = true };
            var user = new User { Email = "test@x.com", IsActive = true };
            _freelancerRepoMock.Setup(r => r.Get(freelancerId)).ReturnsAsync(freelancer);
            _freelancerRepoMock.Setup(r => r.Delete(freelancerId)).ReturnsAsync(freelancer);
            _userRepoMock.Setup(r => r.Get(freelancer.Email)).ReturnsAsync(user);
            _userRepoMock.Setup(r => r.Delete(user.Email)).ReturnsAsync(user);

            var result = await _service.DeleteFreelancer(freelancerId);
            Assert.IsNotNull(result);
            Assert.AreEqual(freelancerId, result.Id);
        }

        [Test]
        public async Task GetAllFreelancersPaged_ReturnsPagedResponse()
        {
            _dbContext.Freelancers.Add(new Freelancer { Id = Guid.NewGuid(), Email = "a@a.com", IsActive = true, CreatedAt = DateTime.UtcNow });
            _dbContext.Freelancers.Add(new Freelancer { Id = Guid.NewGuid(), Email = "b@b.com", IsActive = true, CreatedAt = DateTime.UtcNow.AddMinutes(-1) });
            _dbContext.SaveChanges();

            var result = await _service.GetAllFreelancersPaged(new PaginationParams { Page = 1, PageSize = 10 });
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Data.Count() > 0);
        }

        [Test]
        public void GetFreelancerById_EmptyId_ThrowsAppException()
        {
            var ex = Assert.ThrowsAsync<AppException>(async () => await _service.GetFreelancerById(Guid.Empty));
            Assert.That(ex.Message, Does.Contain("Freelancer ID is required"));
        }

        [Test]
        public void GetFreelancerById_NotFound_ThrowsAppException()
        {
            _freelancerRepoMock.Setup(r => r.Get(It.IsAny<Guid>())).ReturnsAsync((Freelancer)null);
            var ex = Assert.ThrowsAsync<AppException>(async () => await _service.GetFreelancerById(Guid.NewGuid()));
            Assert.That(ex.Message, Does.Contain("Freelancer not found"));
        }

        [Test]
        public async Task GetFreelancerById_Success_ReturnsFreelancerResponseDTO()
        {
            var freelancerId = Guid.NewGuid();
            var freelancer = new Freelancer { Id = freelancerId, Email = "test@x.com", IsActive = true };
            _freelancerRepoMock.Setup(r => r.Get(freelancerId)).ReturnsAsync(freelancer);

            var result = await _service.GetFreelancerById(freelancerId);
            Assert.IsNotNull(result);
            Assert.AreEqual(freelancerId, result.Id);
        }

        [Test]
        public void UpdateFreelancer_EmptyId_ThrowsAppException()
        {
            var ex = Assert.ThrowsAsync<AppException>(async () => await _service.UpdateFreelancer(Guid.Empty, new UpdateFreelancerDTO()));
            Assert.That(ex.Message, Does.Contain("Freelancer ID is required"));
        }

        [Test]
        public void UpdateFreelancer_NullDto_ThrowsAppException()
        {
            var ex = Assert.ThrowsAsync<AppException>(async () => await _service.UpdateFreelancer(Guid.NewGuid(), null));
            Assert.That(ex.Message, Does.Contain("Freelancer DTO is required"));
        }

        [Test]
        public void UpdateFreelancer_NotFound_ThrowsAppException()
        {
            _freelancerRepoMock.Setup(r => r.Get(It.IsAny<Guid>())).ReturnsAsync((Freelancer)null);
            var ex = Assert.ThrowsAsync<AppException>(async () => await _service.UpdateFreelancer(Guid.NewGuid(), new UpdateFreelancerDTO()));
            Assert.That(ex.Message, Does.Contain("Freelancer not found"));
        }

        [Test]
        public async Task UpdateFreelancer_Success_ReturnsFreelancerResponseDTO()
        {
            var freelancerId = Guid.NewGuid();
            var freelancer = new Freelancer { Id = freelancerId, Email = "test@x.com", IsActive = true };
            _freelancerRepoMock.Setup(r => r.Get(freelancerId)).ReturnsAsync(freelancer);
            _skillsMock.Setup(s => s.GetOrCreateSkills(It.IsAny<IEnumerable<SkillDTO>>())).ReturnsAsync(new List<Skill>());
            _freelancerRepoMock.Setup(r => r.Update(freelancerId, It.IsAny<Freelancer>())).ReturnsAsync(freelancer);

            var dto = new UpdateFreelancerDTO { Username = "UpdatedName" };
            var result = await _service.UpdateFreelancer(freelancerId, dto);

            Assert.IsNotNull(result);
            Assert.AreEqual(freelancerId, result.Id);
            Assert.AreEqual("UpdatedName", result.Username);
        }

        // Testable version that skips transaction logic for InMemory
        public class TestableFreelancerService : FreelancerService
        {
            private readonly IRepository<Guid, Freelancer> _freelancerRepository;
            private readonly IRepository<string, User> _userRepository;
            private readonly IGetOrCreateSkills _getOrCreateSkills;
            private readonly IImageUploadService _imageUploadService;
            public TestableFreelancerService(
                IRepository<Guid, Freelancer> freelancerRepository,
                IRepository<string, User> userRepository,
                FreelanceDBContext appContext,
                IGetOrCreateSkills getOrCreateSkills,
                IImageUploadService imageUploadService)
                : base(freelancerRepository, userRepository, appContext, getOrCreateSkills, imageUploadService)
            {
                _freelancerRepository = freelancerRepository;
                _userRepository = userRepository;
                _getOrCreateSkills = getOrCreateSkills;
                _imageUploadService = imageUploadService;
            }

            public override async Task<FreelancerResponseDTO> CreateFreelancer(CreateFreelancerDTO createDto)
            {
                // Copy logic, skip transaction
                var user = await UserMapper.CreateUserFromCreateFreelancerDTO(createDto);
                var newUser = await _userRepository.Add(user);
                if (newUser == null) throw new AppException("Unable to create user.", 500);

                var requiredSkills = new List<Skill>();
                if (createDto.Skills != null && createDto.Skills.Count() > 0)
                    requiredSkills = await _getOrCreateSkills.GetOrCreateSkills(createDto.Skills);

                var freelancer = FreelancerMapper.CreateFreelancerFromCreateDTO(createDto, requiredSkills);
                freelancer.Email = newUser.Email;

                var response = await _freelancerRepository.Add(freelancer);
                if (response == null) throw new AppException("Unable to create freelancer.", 500);

                return FreelancerMapper.ToResponseDTO(response);
            }

            public override async Task<FreelancerResponseDTO> DeleteFreelancer(Guid freelancerId)
            {
                if (freelancerId == Guid.Empty) throw new AppException("Freelancer ID is required.", 400);
                var freelancer = await _freelancerRepository.Get(freelancerId);
                if (freelancer == null || freelancer.IsActive == false) throw new AppException("Freelancer not found", 404);
                if (freelancer.ProfileUrl != null) await _imageUploadService.DeleteImage(freelancer.ProfileUrl);

                var response = await _freelancerRepository.Delete(freelancerId);
                if (response == null) throw new AppException("Unable to delete freelancer.", 500);

                var user = await _userRepository.Get(freelancer.Email);
                if (user == null || user.IsActive == false) throw new AppException("User not found", 404);
                var userResponse = await _userRepository.Delete(user.Email);
                if (userResponse == null) throw new AppException("Unable to delete user.", 500);

                return FreelancerMapper.ToResponseDTO(response);
            }
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Dispose();
        }
    }
}