using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Freelance_Project.Controllers.V1;
using Freelance_Project.Interfaces;
using Freelance_Project.Misc;
using Freelance_Project.Models;
using Freelance_Project.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Moq;
using NUnit.Framework;

namespace Freelance_Project.Test.Controllers.V1
{
    [TestFixture]
    public class ProjectProposalControllerTest
    {
        private Mock<IProjectProposalService> _serviceMock;
        private Mock<IHubContext<NotificationHub>> _hubContextMock;
        private Mock<IClientProxy> _clientProxyMock;
        private ProjectProposalController _controller;

        [SetUp]
        public void SetUp()
        {
            _serviceMock = new Mock<IProjectProposalService>();
            _hubContextMock = new Mock<IHubContext<NotificationHub>>();
            _clientProxyMock = new Mock<IClientProxy>();

            var clientsMock = new Mock<IHubClients>();
            clientsMock.Setup(c => c.User(It.IsAny<string>())).Returns(_clientProxyMock.Object);
            _hubContextMock.SetupGet(h => h.Clients).Returns(clientsMock.Object);

            _controller = new ProjectProposalController(_serviceMock.Object, _hubContextMock.Object);
        }

        [Test]
        public async Task GetProposalsByProjectId_ReturnsSuccess_WhenProposalsExist()
        {
            var projectId = Guid.NewGuid();
            var pagedResponse = new PagedResponse<ProposalResponseDTO>
            {
                Data = new List<ProposalResponseDTO> { new ProposalResponseDTO() }
            };
            _serviceMock.Setup(s => s.GetProposalsByProjectId(projectId, It.IsAny<PaginationParams>()))
                .ReturnsAsync(pagedResponse);

            var result = await _controller.GetProposalsByProjectId(projectId, new PaginationParams());
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public async Task AcceptProposal_ReturnsSuccess_WhenValid()
        {
            var proposalId = Guid.NewGuid();
            var projectId = Guid.NewGuid();
            var dto = new ProposalRequestDTO { ProposalId = proposalId, ProjectId = projectId };
            var projectResponse = new ProjectResponseDTO { FreelancerId = Guid.NewGuid(), Title = "Test" };

            _serviceMock.Setup(s => s.AcceptProposal(proposalId, projectId)).ReturnsAsync(projectResponse);

            var result = await _controller.AcceptProposal(dto);
            Assert.IsInstanceOf<OkObjectResult>(result);
            _clientProxyMock.Verify(
                c => c.SendCoreAsync("FreelancerNotification", It.IsAny<object[]>(), default), Times.Once);
        }

        [Test]
        public async Task CancelProject_ReturnsSuccess_WhenValid()
        {
            var projectId = Guid.NewGuid();
            var projectResponse = new ProjectResponseDTO { FreelancerId = Guid.NewGuid(), Title = "Test" };

            _serviceMock.Setup(s => s.CancelProject(projectId)).ReturnsAsync(projectResponse);

            var result = await _controller.CancelProject(projectId);
            Assert.IsInstanceOf<OkObjectResult>(result);
            _clientProxyMock.Verify(
                c => c.SendCoreAsync("FreelancerNotification", It.IsAny<object[]>(), default), Times.Once);
        }

        [Test]
        public async Task RejectProposal_ReturnsSuccess_WhenValid()
        {
            var proposalId = Guid.NewGuid();
            var projectId = Guid.NewGuid();
            var dto = new ProposalRequestDTO { ProposalId = proposalId, ProjectId = projectId };
            var proposalResponse = new ProposalResponseDTO
            {
                Freelancer = new Freelance_Project.Models.DTO.FreelancerSummaryDTO { Id = Guid.NewGuid() },
                Project = new Freelance_Project.Models.DTO.ProjectSummaryDTO { Title = "Test" }
            };

            _serviceMock.Setup(s => s.RejectProposal(proposalId, projectId)).ReturnsAsync(proposalResponse);

            var result = await _controller.RejectProposal(dto);
            Assert.IsInstanceOf<OkObjectResult>(result);
            _clientProxyMock.Verify(
                c => c.SendCoreAsync("FreelancerNotification", It.IsAny<object[]>(), default), Times.Once);
        }
    }
}