using System;
using System.Collections.Generic;
using System.Linq;
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
    [TestFixture]
    public class ClientProjectServiceTest
    {
        private Mock<IRepository<Guid, Project>> _projectRepoMock;
        private Mock<IRepository<Guid, Client>> _clientRepoMock;
        private Mock<IGetOrCreateSkills> _skillsMock;
        private FreelanceDBContext _dbContext;
        private ClientProjectService _service;

        [SetUp]
        public void Setup()
        {
            var dbOptions = new DbContextOptionsBuilder<FreelanceDBContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _dbContext = new FreelanceDBContext(dbOptions);

            _projectRepoMock = new Mock<IRepository<Guid, Project>>();
            _clientRepoMock = new Mock<IRepository<Guid, Client>>();
            _skillsMock = new Mock<IGetOrCreateSkills>();

            _service = new ClientProjectService(
                _projectRepoMock.Object,
                _clientRepoMock.Object,
                _skillsMock.Object,
                _dbContext
            );
        }

        [Test]
        public async Task PostProject_Success()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var client = new Client { Id = clientId, IsActive = true };
            var createDto = new CreateProjectDTO
            {
                ClientId = clientId,
                RequiredSkills = new List<SkillDTO> { new SkillDTO { Name = "C#" } }
            };
            var skills = new List<Skill> { new Skill { Id = Guid.NewGuid(), Name = "C#" } };
            var project = new Project { Id = Guid.NewGuid(), ClientId = clientId, IsActive = true };
            var responseProject = project;

            _clientRepoMock.Setup(r => r.Get(clientId)).ReturnsAsync(client);
            _skillsMock.Setup(s => s.GetOrCreateSkills(It.IsAny<IEnumerable<SkillDTO>>())).ReturnsAsync(skills);
            _projectRepoMock.Setup(r => r.Add(It.IsAny<Project>())).ReturnsAsync(responseProject);

            // Act
            var result = await _service.PostProject(createDto);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(project.Id, result.Id);
        }

        [Test]
        public void PostProject_ClientNotFound_ThrowsAppException()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var createDto = new CreateProjectDTO { ClientId = clientId };
            _clientRepoMock.Setup(r => r.Get(clientId)).ReturnsAsync((Client)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<AppException>(async () => await _service.PostProject(createDto));
            Assert.AreEqual(404, ex.StatusCode);
        }

        [Test]
        public void PostProject_AddFails_ThrowsAppException()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var client = new Client { Id = clientId, IsActive = true };
            var createDto = new CreateProjectDTO { ClientId = clientId };
            _clientRepoMock.Setup(r => r.Get(clientId)).ReturnsAsync(client);
            _projectRepoMock.Setup(r => r.Add(It.IsAny<Project>())).ReturnsAsync((Project)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<AppException>(async () => await _service.PostProject(createDto));
            Assert.AreEqual(500, ex.StatusCode);
        }

        [Test]
        public async Task DeleteProject_Success()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var project = new Project { Id = projectId, IsActive = true };
            _projectRepoMock.Setup(r => r.Get(projectId)).ReturnsAsync(project);
            _projectRepoMock.Setup(r => r.Delete(projectId)).ReturnsAsync(project);

            // Act
            var result = await _service.DeleteProject(projectId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(projectId, result.Id);
        }

        [Test]
        public void DeleteProject_NotFound_ThrowsAppException()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            _projectRepoMock.Setup(r => r.Get(projectId)).ReturnsAsync((Project)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<AppException>(async () => await _service.DeleteProject(projectId));
            Assert.AreEqual(404, ex.StatusCode);
        }

        [Test]
        public void DeleteProject_DeleteFails_ThrowsAppException()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var project = new Project { Id = projectId, IsActive = true };
            _projectRepoMock.Setup(r => r.Get(projectId)).ReturnsAsync(project);
            _projectRepoMock.Setup(r => r.Delete(projectId)).ReturnsAsync((Project)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<AppException>(async () => await _service.DeleteProject(projectId));
            Assert.AreEqual(500, ex.StatusCode);
        }

        [Test]
        public async Task GetAllProjectsPaged_Success()
        {
            // Arrange
            var pagination = new PaginationParams { Page = 1, PageSize = 10 };
            _dbContext.Projects.Add(new Project { Id = Guid.NewGuid(), IsActive = true, CreatedAt = DateTime.UtcNow });
            _dbContext.SaveChanges();

            // Act
            var result = await _service.GetAllProjectsPaged(pagination);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Data.Any());
        }

        [Test]
        public void GetProjectById_EmptyId_ThrowsAppException()
        {
            // Arrange
            var emptyId = Guid.Empty;

            // Act & Assert
            var ex = Assert.ThrowsAsync<AppException>(async () => await _service.GetProjectById(emptyId));
            Assert.AreEqual(400, ex.StatusCode);
        }

        [Test]
        public void GetProjectById_NotFound_ThrowsAppException()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            _projectRepoMock.Setup(r => r.Get(projectId)).ReturnsAsync((Project)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<AppException>(async () => await _service.GetProjectById(projectId));
            Assert.AreEqual(404, ex.StatusCode);
        }

        [Test]
        public async Task GetProjectById_Success()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var project = new Project { Id = projectId, IsActive = true };
            _projectRepoMock.Setup(r => r.Get(projectId)).ReturnsAsync(project);

            // Act
            var result = await _service.GetProjectById(projectId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(projectId, result.Id);
        }

        [Test]
        public void GetProjectsByClientId_EmptyId_ThrowsAppException()
        {
            // Arrange
            var emptyId = Guid.Empty;
            var pagination = new PaginationParams();

            // Act & Assert
            var ex = Assert.ThrowsAsync<AppException>(async () => await _service.GetProjectsByClientId(emptyId, pagination));
            Assert.AreEqual(400, ex.StatusCode);
        }

        [Test]
        public void GetProjectsByClientId_ClientNotFound_ThrowsAppException()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var pagination = new PaginationParams();
            _clientRepoMock.Setup(r => r.Get(clientId)).ReturnsAsync((Client)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<AppException>(async () => await _service.GetProjectsByClientId(clientId, pagination));
            Assert.AreEqual(404, ex.StatusCode);
        }

        [Test]
        public async Task GetProjectsByClientId_Success()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var client = new Client { Id = clientId, IsActive = true };
            _clientRepoMock.Setup(r => r.Get(clientId)).ReturnsAsync(client);

            _dbContext.Projects.Add(new Project { Id = Guid.NewGuid(), ClientId = clientId, IsActive = true, CreatedAt = DateTime.UtcNow });
            _dbContext.SaveChanges();

            var pagination = new PaginationParams();

            // Act
            var result = await _service.GetProjectsByClientId(clientId, pagination);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Data.Any());
        }

        [Test]
        public void UpdateProject_EmptyId_ThrowsAppException()
        {
            // Arrange
            var emptyId = Guid.Empty;
            var updateDto = new UpdateProjectDTO();

            // Act & Assert
            var ex = Assert.ThrowsAsync<AppException>(async () => await _service.UpdateProject(emptyId, updateDto));
            Assert.AreEqual(400, ex.StatusCode);
        }

        [Test]
        public void UpdateProject_NotFound_ThrowsAppException()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var updateDto = new UpdateProjectDTO();
            _projectRepoMock.Setup(r => r.Get(projectId)).ReturnsAsync((Project)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<AppException>(async () => await _service.UpdateProject(projectId, updateDto));
            Assert.AreEqual(404, ex.StatusCode);
        }

        [Test]
        public void UpdateProject_UpdateFails_ThrowsAppException()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var project = new Project { Id = projectId, IsActive = true };
            var updateDto = new UpdateProjectDTO();
            _projectRepoMock.Setup(r => r.Get(projectId)).ReturnsAsync(project);
            _projectRepoMock.Setup(r => r.Update(projectId, It.IsAny<Project>())).ReturnsAsync((Project)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<AppException>(async () => await _service.UpdateProject(projectId, updateDto));
            Assert.AreEqual(500, ex.StatusCode);
        }

        [Test]
        public async Task UpdateProject_Success()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var project = new Project { Id = projectId, IsActive = true };
            var updateDto = new UpdateProjectDTO { Title = "Updated Title" };
            _projectRepoMock.Setup(r => r.Get(projectId)).ReturnsAsync(project);
            _projectRepoMock.Setup(r => r.Update(projectId, It.IsAny<Project>())).ReturnsAsync(project);

            // Act
            var result = await _service.UpdateProject(projectId, updateDto);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(projectId, result.Id);
            Assert.AreEqual("Updated Title", result.Title);
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }
    }
}