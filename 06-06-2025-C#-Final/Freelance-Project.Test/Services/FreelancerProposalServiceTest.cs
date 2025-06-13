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
    public class FreelancerProposalServiceTest
    {
        private Mock<IRepository<Guid, Proposal>> _proposalRepoMock;
        private Mock<IRepository<Guid, Freelancer>> _freelancerRepoMock;
        private Mock<IRepository<Guid, Project>> _projectRepoMock;
        private FreelanceDBContext _dbContext;
        private FreelancerProposalService _service;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<FreelanceDBContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _dbContext = new FreelanceDBContext(options);

            _proposalRepoMock = new Mock<IRepository<Guid, Proposal>>();
            _freelancerRepoMock = new Mock<IRepository<Guid, Freelancer>>();
            _projectRepoMock = new Mock<IRepository<Guid, Project>>();

            _service = new FreelancerProposalService(
                _proposalRepoMock.Object,
                _freelancerRepoMock.Object,
                _projectRepoMock.Object,
                _dbContext
            );
        }

        [Test]
        public void CreateProposal_NullDto_ThrowsAppException()
        {
            var ex = Assert.ThrowsAsync<AppException>(async () => await _service.CreateProposal(null));
            Assert.That(ex.Message, Does.Contain("Proposal request cannot be null"));
        }

        [Test]
        public void CreateProposal_EmptyFreelancerId_ThrowsAppException()
        {
            var dto = new CreateProposalDTO { FreelancerId = Guid.Empty };
            var ex = Assert.ThrowsAsync<AppException>(async () => await _service.CreateProposal(dto));
            Assert.That(ex.Message, Does.Contain("Freelancer ID cannot be empty"));
        }

        [Test]
        public void CreateProposal_FreelancerNotFound_ThrowsAppException()
        {
            var dto = new CreateProposalDTO { FreelancerId = Guid.NewGuid(), ProjectId = Guid.NewGuid() };
            _freelancerRepoMock.Setup(r => r.Get(dto.FreelancerId)).ReturnsAsync((Freelancer)null);

            var ex = Assert.ThrowsAsync<AppException>(async () => await _service.CreateProposal(dto));
            Assert.That(ex.Message, Does.Contain("Freelancer not found"));
        }

        [Test]
        public void CreateProposal_ProjectNotFound_ThrowsAppException()
        {
            var freelancerId = Guid.NewGuid();
            var projectId = Guid.NewGuid();
            var dto = new CreateProposalDTO { FreelancerId = freelancerId, ProjectId = projectId };
            _freelancerRepoMock.Setup(r => r.Get(freelancerId)).ReturnsAsync(new Freelancer { Id = freelancerId, IsActive = true });
            _projectRepoMock.Setup(r => r.Get(projectId)).ReturnsAsync((Project)null);

            var ex = Assert.ThrowsAsync<AppException>(async () => await _service.CreateProposal(dto));
            Assert.That(ex.Message, Does.Contain("Project not found"));
        }

        [Test]
        public void CreateProposal_AlreadySubmitted_ThrowsAppException()
        {
            var freelancerId = Guid.NewGuid();
            var projectId = Guid.NewGuid();
            var dto = new CreateProposalDTO { FreelancerId = freelancerId, ProjectId = projectId };
            _freelancerRepoMock.Setup(r => r.Get(freelancerId)).ReturnsAsync(new Freelancer { Id = freelancerId, IsActive = true });
            _projectRepoMock.Setup(r => r.Get(projectId)).ReturnsAsync(new Project
            {
                Id = projectId,
                IsActive = true,
                Proposals = new List<Proposal>
                {
                    new Proposal { FreelancerId = freelancerId, IsActive = true }
                }
            });

            var ex = Assert.ThrowsAsync<AppException>(async () => await _service.CreateProposal(dto));
            Assert.That(ex.Message, Does.Contain("already submitted"));
        }

        [Test]
        public void CreateProposal_AddFails_ThrowsAppException()
        {
            var freelancerId = Guid.NewGuid();
            var projectId = Guid.NewGuid();
            var dto = new CreateProposalDTO { FreelancerId = freelancerId, ProjectId = projectId };
            _freelancerRepoMock.Setup(r => r.Get(freelancerId)).ReturnsAsync(new Freelancer { Id = freelancerId, IsActive = true });
            _projectRepoMock.Setup(r => r.Get(projectId)).ReturnsAsync(new Project { Id = projectId, IsActive = true, Proposals = new List<Proposal>() });
            _proposalRepoMock.Setup(r => r.Add(It.IsAny<Proposal>())).ReturnsAsync((Proposal)null);

            var ex = Assert.ThrowsAsync<AppException>(async () => await _service.CreateProposal(dto));
            Assert.That(ex.Message, Does.Contain("Unable to create proposal"));
        }

        [Test]
        public async Task CreateProposal_Success_ReturnsProposalResponseDTO()
        {
            // Arrange
            var freelancerId = Guid.NewGuid();
            var projectId = Guid.NewGuid();
            var dto = new CreateProposalDTO { FreelancerId = freelancerId, ProjectId = projectId, Description = "desc", ProposedAmount = 100, ProposedDuration = TimeSpan.FromDays(5) };

            _freelancerRepoMock.Setup(r => r.Get(freelancerId)).ReturnsAsync(new Freelancer { Id = freelancerId, IsActive = true });
            _projectRepoMock.Setup(r => r.Get(projectId)).ReturnsAsync(new Project { Id = projectId, IsActive = true, Proposals = new List<Proposal>() });
            _proposalRepoMock.Setup(r => r.Add(It.IsAny<Proposal>()))
                .ReturnsAsync((Proposal p) =>
                {
                    p.Freelancer = new Freelancer { Id = p.FreelancerId, Username = "Test", Email = "test@x.com", IsActive = true };
                    p.Project = new Project { Id = p.ProjectId, Title = "Test Project", Status = "Open", IsActive = true, FreelancerId = p.FreelancerId };
                    return p;
                });

            // Act
            var result = await _service.CreateProposal(dto);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(freelancerId, result.Freelancer.Id);
            Assert.AreEqual(projectId, result.Project.Id);
        }

        [Test]
        public void DeleteProposal_EmptyId_ThrowsAppException()
        {
            var ex = Assert.ThrowsAsync<AppException>(async () => await _service.DeleteProposal(Guid.Empty));
            Assert.That(ex.Message, Does.Contain("Proposal ID cannot be empty"));
        }

        [Test]
        public void DeleteProposal_NotFound_ThrowsAppException()
        {
            var proposalId = Guid.NewGuid();
            _proposalRepoMock.Setup(r => r.Get(proposalId)).ReturnsAsync((Proposal)null);

            var ex = Assert.ThrowsAsync<AppException>(async () => await _service.DeleteProposal(proposalId));
            Assert.That(ex.Message, Does.Contain("Proposal not found"));
        }

        [Test]
        public void DeleteProposal_DeleteFails_ThrowsAppException()
        {
            var proposalId = Guid.NewGuid();
            _proposalRepoMock.Setup(r => r.Get(proposalId)).ReturnsAsync(new Proposal { Id = proposalId, IsActive = true });
            _proposalRepoMock.Setup(r => r.Delete(proposalId)).ReturnsAsync((Proposal)null);

            var ex = Assert.ThrowsAsync<AppException>(async () => await _service.DeleteProposal(proposalId));
            Assert.That(ex.Message, Does.Contain("Unable to delete proposal"));
        }

        [Test]
        public async Task DeleteProposal_Success_ReturnsProposalResponseDTO()
        {
            var proposalId = Guid.NewGuid();
            _proposalRepoMock.Setup(r => r.Get(proposalId)).ReturnsAsync(new Proposal { Id = proposalId, IsActive = true });
            _proposalRepoMock.Setup(r => r.Delete(proposalId)).ReturnsAsync(new Proposal { Id = proposalId, IsActive = true });

            var result = await _service.DeleteProposal(proposalId);
            Assert.IsNotNull(result);
            Assert.AreEqual(proposalId, result.Id);
        }

        [Test]
        public async Task GetAllProposalsPaged_ReturnsPagedResponse()
        {
            _dbContext.Proposals.Add(new Proposal { Id = Guid.NewGuid(), IsActive = true });
            _dbContext.Proposals.Add(new Proposal { Id = Guid.NewGuid(), IsActive = true });
            _dbContext.SaveChanges();

            var result = await _service.GetAllProposalsPaged(new PaginationParams { Page = 1, PageSize = 10 });
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Data.Count() > 0);
        }

        [Test]
        public void GetProposalById_EmptyId_ThrowsAppException()
        {
            var ex = Assert.ThrowsAsync<AppException>(async () => await _service.GetProposalById(Guid.Empty));
            Assert.That(ex.Message, Does.Contain("Proposal ID is required"));
        }

        [Test]
        public void GetProposalById_NotFound_ThrowsAppException()
        {
            var proposalId = Guid.NewGuid();
            _proposalRepoMock.Setup(r => r.Get(proposalId)).ReturnsAsync((Proposal)null);

            var ex = Assert.ThrowsAsync<AppException>(async () => await _service.GetProposalById(proposalId));
            Assert.That(ex.Message, Does.Contain("Proposal not found"));
        }

        [Test]
        public async Task GetProposalById_Success_ReturnsProposalResponseDTO()
        {
            var proposalId = Guid.NewGuid();
            _proposalRepoMock.Setup(r => r.Get(proposalId)).ReturnsAsync(new Proposal { Id = proposalId, IsActive = true });

            var result = await _service.GetProposalById(proposalId);
            Assert.IsNotNull(result);
            Assert.AreEqual(proposalId, result.Id);
        }

        [Test]
        public void GetProposalsByFreelancerId_EmptyId_ThrowsAppException()
        {
            var ex = Assert.ThrowsAsync<AppException>(async () => await _service.GetProposalsByFreelancerId(Guid.Empty, new PaginationParams()));
            Assert.That(ex.Message, Does.Contain("Freelancer ID is required"));
        }

        [Test]
        public void GetProposalsByFreelancerId_FreelancerNotFound_ThrowsAppException()
        {
            var freelancerId = Guid.NewGuid();
            _freelancerRepoMock.Setup(r => r.Get(freelancerId)).ReturnsAsync((Freelancer)null);

            var ex = Assert.ThrowsAsync<AppException>(async () => await _service.GetProposalsByFreelancerId(freelancerId, new PaginationParams()));
            Assert.That(ex.Message, Does.Contain("Freelancer not found"));
        }

        [Test]
        public async Task GetProposalsByFreelancerId_Success_ReturnsPagedResponse()
        {
            var freelancerId = Guid.NewGuid();
            _freelancerRepoMock.Setup(r => r.Get(freelancerId)).ReturnsAsync(new Freelancer { Id = freelancerId, IsActive = true });

            _dbContext.Proposals.Add(new Proposal { Id = Guid.NewGuid(), FreelancerId = freelancerId, IsActive = true });
            _dbContext.SaveChanges();

            var result = await _service.GetProposalsByFreelancerId(freelancerId, new PaginationParams { Page = 1, PageSize = 10 });
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Data.Count() > 0);
        }

        [Test]
        public void UpdateProposal_EmptyId_ThrowsAppException()
        {
            var ex = Assert.ThrowsAsync<AppException>(async () => await _service.UpdateProposal(Guid.Empty, new UpdateProposalDTO()));
            Assert.That(ex.Message, Does.Contain("Proposal ID is required"));
        }

        [Test]
        public void UpdateProposal_NullDto_ThrowsAppException()
        {
            var ex = Assert.ThrowsAsync<AppException>(async () => await _service.UpdateProposal(Guid.NewGuid(), null));
            Assert.That(ex.Message, Does.Contain("Update data is required"));
        }

        [Test]
        public void UpdateProposal_NotFound_ThrowsAppException()
        {
            var proposalId = Guid.NewGuid();
            _proposalRepoMock.Setup(r => r.Get(proposalId)).ReturnsAsync((Proposal)null);

            var ex = Assert.ThrowsAsync<AppException>(async () => await _service.UpdateProposal(proposalId, new UpdateProposalDTO()));
            Assert.That(ex.Message, Does.Contain("Proposal not found"));
        }

        [Test]
        public void UpdateProposal_UpdateFails_ThrowsAppException()
        {
            var proposalId = Guid.NewGuid();
            _proposalRepoMock.Setup(r => r.Get(proposalId)).ReturnsAsync(new Proposal { Id = proposalId, IsActive = true });
            _proposalRepoMock.Setup(r => r.Update(proposalId, It.IsAny<Proposal>())).ReturnsAsync((Proposal)null);

            var ex = Assert.ThrowsAsync<AppException>(async () => await _service.UpdateProposal(proposalId, new UpdateProposalDTO { Description = "desc" }));
            Assert.That(ex.Message, Does.Contain("Proposal update failed"));
        }

        [Test]
        public async Task UpdateProposal_Success_ReturnsProposalResponseDTO()
        {
            var proposalId = Guid.NewGuid();
            var proposal = new Proposal { Id = proposalId, IsActive = true, Description = "old" };
            _proposalRepoMock.Setup(r => r.Get(proposalId)).ReturnsAsync(proposal);
            _proposalRepoMock.Setup(r => r.Update(proposalId, It.IsAny<Proposal>())).ReturnsAsync(proposal);

            var result = await _service.UpdateProposal(proposalId, new UpdateProposalDTO { Description = "new" });
            Assert.IsNotNull(result);
            Assert.AreEqual(proposalId, result.Id);
            Assert.AreEqual("new", result.Description);
        }
        
        [TearDown]
        public void TearDown()
        {
            _dbContext.Dispose();
        }
    }
}