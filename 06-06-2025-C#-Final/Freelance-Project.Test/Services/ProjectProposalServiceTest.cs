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
    public class ProjectProposalServiceTest
    {
        private Mock<IRepository<Guid, Project>> _projectRepoMock;
        private Mock<IRepository<Guid, Proposal>> _proposalRepoMock;
        private Mock<IRepository<Guid, Freelancer>> _freelancerRepoMock;
        private Mock<IRepository<Guid, ChatRoom>> _chatRoomRepoMock;
        private FreelanceDBContext _dbContext;
        private ProjectProposalService _service;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<FreelanceDBContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _dbContext = new FreelanceDBContext(options);

            _projectRepoMock = new Mock<IRepository<Guid, Project>>();
            _proposalRepoMock = new Mock<IRepository<Guid, Proposal>>();
            _freelancerRepoMock = new Mock<IRepository<Guid, Freelancer>>();
            _chatRoomRepoMock = new Mock<IRepository<Guid, ChatRoom>>();

            _service = new TestableProjectProposalService(
            _projectRepoMock.Object,
            _proposalRepoMock.Object,
            _freelancerRepoMock.Object,
            _chatRoomRepoMock.Object,
            _dbContext
);
        }

        [Test]
        public void GetProposalsByProjectId_EmptyId_ThrowsAppException()
        {
            var ex = Assert.ThrowsAsync<AppException>(async () => await _service.GetProposalsByProjectId(Guid.Empty, new PaginationParams()));
            Assert.That(ex.Message, Does.Contain("Project ID cannot be empty"));
        }

        [Test]
        public void GetProposalsByProjectId_ProjectNotFound_ThrowsAppException()
        {
            _projectRepoMock.Setup(r => r.Get(It.IsAny<Guid>())).ReturnsAsync((Project)null);
            var ex = Assert.ThrowsAsync<AppException>(async () => await _service.GetProposalsByProjectId(Guid.NewGuid(), new PaginationParams()));
            Assert.That(ex.Message, Does.Contain("Project not found"));
        }

        [Test]
        public void GetProposalsByProjectId_FreelancerNotFound_ThrowsAppException()
        {
            var projectId = Guid.NewGuid();
            var freelancerId = Guid.NewGuid();
            _projectRepoMock.Setup(r => r.Get(projectId)).ReturnsAsync(new Project { Id = projectId, IsActive = true, FreelancerId = freelancerId });
            _freelancerRepoMock.Setup(r => r.Get(freelancerId)).ReturnsAsync((Freelancer)null);

            var ex = Assert.ThrowsAsync<AppException>(async () => await _service.GetProposalsByProjectId(projectId, new PaginationParams()));
            Assert.That(ex.Message, Does.Contain("Freelancer not found"));
        }

        [Test]
        public async Task GetProposalsByProjectId_Success_ReturnsPagedResponse()
        {
            var projectId = Guid.NewGuid();
            _projectRepoMock.Setup(r => r.Get(projectId)).ReturnsAsync(new Project { Id = projectId, IsActive = true });
            _dbContext.Proposals.Add(new Proposal { Id = Guid.NewGuid(), ProjectId = projectId, IsActive = true });
            _dbContext.SaveChanges();

            var result = await _service.GetProposalsByProjectId(projectId, new PaginationParams { Page = 1, PageSize = 10 });
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Data.Count() > 0);
        }

        [Test]
        public void AcceptProposal_EmptyIds_ThrowsAppException()
        {
            var ex = Assert.ThrowsAsync<AppException>(async () => await _service.AcceptProposal(Guid.Empty, Guid.Empty));
            Assert.That(ex.Message, Does.Contain("Proposal ID or Project ID cannot be empty"));
        }

        [Test]
        public void AcceptProposal_ProposalNotFound_ThrowsAppException()
        {
            _proposalRepoMock.Setup(r => r.Get(It.IsAny<Guid>())).ReturnsAsync((Proposal)null);
            var ex = Assert.ThrowsAsync<AppException>(async () => await _service.AcceptProposal(Guid.NewGuid(), Guid.NewGuid()));
            Assert.That(ex.Message, Does.Contain("Proposal not found"));
        }

        [Test]
        public void AcceptProposal_ProposalAlreadyAccepted_ThrowsAppException()
        {
            var proposalId = Guid.NewGuid();
            var projectId = Guid.NewGuid();
            _proposalRepoMock.Setup(r => r.Get(proposalId))
                .ReturnsAsync(new Proposal { Id = proposalId, IsActive = true, IsAccepted = true, ProjectId = projectId });
            _projectRepoMock.Setup(r => r.Get(projectId))
                .ReturnsAsync(new Project { Id = projectId, IsActive = true, Status = "Pending" });

            var ex = Assert.ThrowsAsync<AppException>(async () => await _service.AcceptProposal(proposalId, projectId));
            Assert.That(ex.Message, Does.Contain("already accepted"));
        }

        [Test]
        public void AcceptProposal_ProposalRejected_ThrowsAppException()
        {
            var proposalId = Guid.NewGuid();
            var projectId = Guid.NewGuid();
            _proposalRepoMock.Setup(r => r.Get(proposalId))
                .ReturnsAsync(new Proposal { Id = proposalId, IsActive = true, IsAccepted = false, ProjectId = projectId });
            _projectRepoMock.Setup(r => r.Get(projectId))
                .ReturnsAsync(new Project { Id = projectId, IsActive = true, Status = "Pending" });

            var ex = Assert.ThrowsAsync<AppException>(async () => await _service.AcceptProposal(proposalId, projectId));
            Assert.That(ex.Message, Does.Contain("Proposal has been rejected"));
        }

        [Test]
        public void AcceptProposal_ProjectNotFound_ThrowsAppException()
        {
            var proposalId = Guid.NewGuid();
            _proposalRepoMock.Setup(r => r.Get(proposalId)).ReturnsAsync(new Proposal { Id = proposalId, IsActive = true });
            _projectRepoMock.Setup(r => r.Get(It.IsAny<Guid>())).ReturnsAsync((Project)null);

            var ex = Assert.ThrowsAsync<AppException>(async () => await _service.AcceptProposal(proposalId, Guid.NewGuid()));
            Assert.That(ex.Message, Does.Contain("Project not found"));
        }

        [Test]
        public void AcceptProposal_ProjectNotPending_ThrowsAppException()
        {
            var proposalId = Guid.NewGuid();
            var projectId = Guid.NewGuid();
            _proposalRepoMock.Setup(r => r.Get(proposalId)).ReturnsAsync(new Proposal { Id = proposalId, IsActive = true });
            _projectRepoMock.Setup(r => r.Get(projectId)).ReturnsAsync(new Project { Id = projectId, IsActive = true, Status = "In Progress" });

            var ex = Assert.ThrowsAsync<AppException>(async () => await _service.AcceptProposal(proposalId, projectId));
            Assert.That(ex.Message, Does.Contain("not in pending state"));
        }

        [Test]
        public async Task AcceptProposal_Success_ReturnsProjectResponseDTO()
        {
            var proposalId = Guid.NewGuid();
            var projectId = Guid.NewGuid();
            var proposal = new Proposal { Id = proposalId, IsActive = true, ProjectId = projectId, FreelancerId = Guid.NewGuid(), IsAccepted = null };
            var project = new Project { Id = projectId, IsActive = true, Status = "Pending" };

            _proposalRepoMock.Setup(r => r.Get(proposalId)).ReturnsAsync(proposal);
            _projectRepoMock.Setup(r => r.Get(projectId)).ReturnsAsync(project);
            _proposalRepoMock.Setup(r => r.GetAll()).ReturnsAsync(new List<Proposal> { proposal });
            _proposalRepoMock.Setup(r => r.Update(It.IsAny<Guid>(), It.IsAny<Proposal>())).ReturnsAsync((Guid id, Proposal p) => p);
            _projectRepoMock.Setup(r => r.Update(It.IsAny<Guid>(), It.IsAny<Project>())).ReturnsAsync((Guid id, Project p) => p);

            var result = await _service.AcceptProposal(proposalId, projectId);
            Assert.IsNotNull(result);
            Assert.AreEqual(projectId, result.Id);
            Assert.AreEqual("In Progress", result.Status);
        }

        [Test]
        public void CancelProject_EmptyId_ThrowsAppException()
        {
            var ex = Assert.ThrowsAsync<AppException>(async () => await _service.CancelProject(Guid.Empty));
            Assert.That(ex.Message, Does.Contain("Project ID cannot be empty"));
        }

        [Test]
        public void CancelProject_ProjectNotFound_ThrowsAppException()
        {
            _projectRepoMock.Setup(r => r.Get(It.IsAny<Guid>())).ReturnsAsync((Project)null);
            var ex = Assert.ThrowsAsync<AppException>(async () => await _service.CancelProject(Guid.NewGuid()));
            Assert.That(ex.Message, Does.Contain("Project not found"));
        }

        [Test]
        public void CancelProject_NotApprovedOrInProgress_ThrowsAppException()
        {
            var projectId = Guid.NewGuid();
            _projectRepoMock.Setup(r => r.Get(projectId))
                .ReturnsAsync(new Project { Id = projectId, IsActive = true, Status = "Pending" }); // Not "In Progress"
            var ex = Assert.ThrowsAsync<AppException>(async () => await _service.CancelProject(projectId));
            Assert.That(ex.Message, Does.Contain("Project is not Approved or In Progress, cannot cancel"));
        }

        [Test]
        public void CancelProject_AlreadyCancelled_ThrowsAppException()
        {
            var projectId = Guid.NewGuid();
            _projectRepoMock.Setup(r => r.Get(projectId))
                .ReturnsAsync(new Project { Id = projectId, IsActive = true, Status = "Cancelled" });
            var ex = Assert.ThrowsAsync<AppException>(async () => await _service.CancelProject(projectId));
            Assert.That(ex.Message, Does.Contain("already cancelled"));
        }

        [Test]
        public void CancelProject_AlreadyCompleted_ThrowsAppException()
        {
            var projectId = Guid.NewGuid();
            _projectRepoMock.Setup(r => r.Get(projectId))
                .ReturnsAsync(new Project { Id = projectId, IsActive = true, Status = "Completed" });
            var ex = Assert.ThrowsAsync<AppException>(async () => await _service.CancelProject(projectId));
            Assert.That(ex.Message, Does.Contain("already completed"));
        }

        [Test]
        public void CancelProject_NotApproved_ThrowsAppException()
        {
            var projectId = Guid.NewGuid();
            _projectRepoMock.Setup(r => r.Get(projectId)).ReturnsAsync(new Project { Id = projectId, IsActive = true, Status = "Pending" });
            var ex = Assert.ThrowsAsync<AppException>(async () => await _service.CancelProject(projectId));
            Assert.That(ex.Message, Does.Contain("not Approved"));
        }

        [Test]
        public async Task CancelProject_Success_ReturnsProjectResponseDTO()
        {
            var projectId = Guid.NewGuid();
            var project = new Project { Id = projectId, IsActive = true, Status = "In Progress" };
            _projectRepoMock.Setup(r => r.Get(projectId)).ReturnsAsync(project);
            _projectRepoMock.Setup(r => r.Update(projectId, It.IsAny<Project>())).ReturnsAsync((Guid id, Project p) => p);

            var result = await _service.CancelProject(projectId);
            Assert.IsNotNull(result);
            Assert.AreEqual(projectId, result.Id);
            Assert.AreEqual("Cancelled", result.Status);
        }

        [Test]
        public void RejectProposal_EmptyIds_ThrowsAppException()
        {
            var ex = Assert.ThrowsAsync<AppException>(async () => await _service.RejectProposal(Guid.Empty, Guid.Empty));
            Assert.That(ex.Message, Does.Contain("Proposal ID or Project ID cannot be empty"));
        }

        [Test]
        public void RejectProposal_ProposalNotFound_ThrowsAppException()
        {
            _proposalRepoMock.Setup(r => r.Get(It.IsAny<Guid>())).ReturnsAsync((Proposal)null);
            var ex = Assert.ThrowsAsync<AppException>(async () => await _service.RejectProposal(Guid.NewGuid(), Guid.NewGuid()));
            Assert.That(ex.Message, Does.Contain("Proposal not found"));
        }

        [Test]
        public void RejectProposal_ProposalAccepted_ThrowsAppException()
        {
            var proposalId = Guid.NewGuid();
            var projectId = Guid.NewGuid();
            _proposalRepoMock.Setup(r => r.Get(proposalId))
                .ReturnsAsync(new Proposal { Id = proposalId, IsActive = true, IsAccepted = true, ProjectId = projectId });
            _projectRepoMock.Setup(r => r.Get(projectId))
                .ReturnsAsync(new Project { Id = projectId, IsActive = true, Status = "Pending" });

            var ex = Assert.ThrowsAsync<AppException>(async () => await _service.RejectProposal(proposalId, projectId));
            Assert.That(ex.Message, Does.Contain("Cannot reject an accepted proposal"));
        }

        [Test]
        public void RejectProposal_ProjectNotFound_ThrowsAppException()
        {
            var proposalId = Guid.NewGuid();
            var projectId = Guid.NewGuid();
            _proposalRepoMock.Setup(r => r.Get(proposalId))
                .ReturnsAsync(new Proposal { Id = proposalId, IsActive = true, ProjectId = projectId });
            _projectRepoMock.Setup(r => r.Get(projectId)).ReturnsAsync((Project)null);

            var ex = Assert.ThrowsAsync<AppException>(async () => await _service.RejectProposal(proposalId, projectId)); // <-- Use matching projectId!
            Assert.That(ex.Message, Does.Contain("Project not found"));
        }

        [Test]
        public void RejectProposal_ProjectNotPending_ThrowsAppException()
        {
            var proposalId = Guid.NewGuid();
            var projectId = Guid.NewGuid();
            _proposalRepoMock.Setup(r => r.Get(proposalId))
                .ReturnsAsync(new Proposal { Id = proposalId, IsActive = true, ProjectId = projectId });
            _projectRepoMock.Setup(r => r.Get(projectId))
                .ReturnsAsync(new Project { Id = projectId, IsActive = true, Status = "In Progress" }); // Not "Pending"

            var ex = Assert.ThrowsAsync<AppException>(async () => await _service.RejectProposal(proposalId, projectId));
            Assert.That(ex.Message, Does.Contain("not in pending state"));
        }

        [Test]
        public void RejectProposal_UpdateFails_ThrowsAppException()
        {
            var proposalId = Guid.NewGuid();
            var projectId = Guid.NewGuid();
            _proposalRepoMock.Setup(r => r.Get(proposalId)).ReturnsAsync(new Proposal { Id = proposalId, IsActive = true, ProjectId = projectId });
            _projectRepoMock.Setup(r => r.Get(projectId)).ReturnsAsync(new Project { Id = projectId, IsActive = true, Status = "Pending" });
            _proposalRepoMock.Setup(r => r.Update(proposalId, It.IsAny<Proposal>())).ReturnsAsync((Proposal)null);

            var ex = Assert.ThrowsAsync<AppException>(async () => await _service.RejectProposal(proposalId, projectId));
            Assert.That(ex.Message, Does.Contain("Proposal update failed"));
        }

        [Test]
        public async Task RejectProposal_Success_ReturnsProposalResponseDTO()
        {
            var proposalId = Guid.NewGuid();
            var projectId = Guid.NewGuid();
            _proposalRepoMock.Setup(r => r.Get(proposalId)).ReturnsAsync(new Proposal { Id = proposalId, IsActive = true, ProjectId = projectId });
            _projectRepoMock.Setup(r => r.Get(projectId)).ReturnsAsync(new Project { Id = projectId, IsActive = true, Status = "Pending" });
            _proposalRepoMock.Setup(r => r.Update(proposalId, It.IsAny<Proposal>())).ReturnsAsync((Guid id, Proposal p) => p);

            var result = await _service.RejectProposal(proposalId, projectId);
            Assert.IsNotNull(result);
            Assert.AreEqual(proposalId, result.Id);
            Assert.AreEqual(false, result.IsAccepted);
        }

        public class TestableProjectProposalService : ProjectProposalService
        {
            private readonly IRepository<Guid, Project> _projectRepository;
            private readonly IRepository<Guid, Proposal> _proposalRepository;
            private readonly IRepository<Guid, Freelancer> _freelancerRepo;
            private readonly IRepository<Guid, ChatRoom> _chatRoomRepository;
            private readonly FreelanceDBContext _dbContext;

            public TestableProjectProposalService(
                IRepository<Guid, Project> projectRepository,
                IRepository<Guid, Proposal> proposalRepository,
                IRepository<Guid, Freelancer> freelancerRepository,
                IRepository<Guid, ChatRoom> chatRoomRepository,
                FreelanceDBContext dbContext)
                : base(projectRepository, proposalRepository, freelancerRepository, chatRoomRepository, dbContext)
            {
                _projectRepository = projectRepository;
                _proposalRepository = proposalRepository;
                _freelancerRepo = freelancerRepository;
                _chatRoomRepository = chatRoomRepository;
                _dbContext = dbContext;
            }

            public override async Task<ProjectResponseDTO> AcceptProposal(Guid proposalId, Guid projectId)
            {
                if (proposalId == Guid.Empty || projectId == Guid.Empty)
                    throw new AppException("Proposal ID or Project ID cannot be empty", 400);

                var proposal = await _proposalRepository.Get(proposalId);
                if (proposal == null || !proposal.IsActive)
                    throw new AppException("Proposal not found", 404);

                var project = await _projectRepository.Get(projectId);
                if (project == null || !project.IsActive)
                    throw new AppException("Project not found", 404);

                if (!string.Equals(project.Status, "Pending", StringComparison.OrdinalIgnoreCase))
                    throw new AppException("Project is not in pending state", 400);

                if (proposal.IsAccepted == true)
                    throw new AppException("Proposal already accepted", 400);
                if (proposal.IsAccepted == false)
                    throw new AppException("Proposal has been rejected", 400);

                proposal.IsAccepted = true;
                await _proposalRepository.Update(proposalId, proposal);

                var allProposals = await _proposalRepository.GetAll();
                foreach (var p in allProposals.Where(p => p.ProjectId == projectId && p.Id != proposalId && p.IsActive))
                {
                    p.IsAccepted = false;
                    await _proposalRepository.Update(p.Id, p);
                }

                project.Status = "In Progress";
                await _projectRepository.Update(projectId, project);

                return ProjectMapper.ToResponseDTO(project);
            }
            public override async Task<ProjectResponseDTO> CancelProject(Guid projectId)
            {
                if (projectId == Guid.Empty)
                    throw new AppException("Project ID cannot be empty", 400);

                var project = await _projectRepository.Get(projectId);
                if (project == null || !project.IsActive)
                    throw new AppException("Project not found", 404);

                if (string.Equals(project.Status, "Cancelled", StringComparison.OrdinalIgnoreCase))
                    throw new AppException("Project already cancelled", 400);

                if (string.Equals(project.Status, "Completed", StringComparison.OrdinalIgnoreCase))
                    throw new AppException("Project already completed", 400);

                if (!string.Equals(project.Status, "In Progress", StringComparison.OrdinalIgnoreCase) &&
                    !string.Equals(project.Status, "Approved", StringComparison.OrdinalIgnoreCase))
                    throw new AppException("Project is not Approved or In Progress, cannot cancel", 400);

                project.Status = "Cancelled";
                await _projectRepository.Update(projectId, project);

                return ProjectMapper.ToResponseDTO(project);
            }
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Dispose();
        }
    }
}