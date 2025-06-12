using System;
using System.Threading.Tasks;
using Freelance_Project.Controllers.V1;
using Freelance_Project.Interfaces;
using Freelance_Project.Misc;
using Freelance_Project.Models.DTO;
using Freelance_Project.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Moq;
using NUnit.Framework;

namespace Freelance_Project.Test.Controllers.V1
{
    [TestFixture]
    public class ProposalControllerTest
    {
        private Mock<IFreelancerProposalService> _serviceMock;
        private Mock<IHubContext<NotificationHub>> _hubContextMock;
        private Mock<IClientProxy> _clientProxyMock;
        private ProposalController _controller;

        [SetUp]
        public void Setup()
        {
            _serviceMock = new Mock<IFreelancerProposalService>();
            _hubContextMock = new Mock<IHubContext<NotificationHub>>();
            _clientProxyMock = new Mock<IClientProxy>();

            var clientsMock = new Mock<IHubClients>();
            clientsMock.Setup(c => c.User(It.IsAny<string>())).Returns(_clientProxyMock.Object);
            _hubContextMock.SetupGet(h => h.Clients).Returns(clientsMock.Object);

            _controller = new ProposalController(_serviceMock.Object, _hubContextMock.Object);
        }
        private void SetUser(ProposalController controller, Guid clientId)
        {
            var claims = new List<Claim>
            {
                new Claim("Id", clientId.ToString())
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var principal = new ClaimsPrincipal(identity);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };
        }

        [Test]
        public async Task CreateProposal_ValidRequest_ReturnsSuccess()
        {
            var freelancerId = Guid.NewGuid();
            var clientId = Guid.NewGuid();
            var dto = new CreateProposalDTO { FreelancerId = freelancerId };
            var response = new ProposalResponseDTO
            {
                Project = new ProjectSummaryDTO { ClientId = clientId }
            };
            _serviceMock.Setup(s => s.CreateProposal(dto)).ReturnsAsync(response);
            SetUser(_controller, freelancerId); // Set user claim to FreelancerId

            var result = await _controller.CreateProposal(dto) as OkObjectResult;

            Assert.IsNotNull(result);
            _clientProxyMock.Verify(
                c => c.SendCoreAsync("ClientNotification", It.IsAny<object[]>(), default), Times.Once);
        }

        [Test]
        public async Task CreateProposal_NullDto_ReturnsBadRequest()
        {
            var clientId = Guid.NewGuid();
            SetUser(_controller, clientId);
            var result = await _controller.CreateProposal(null) as BadRequestObjectResult;
            Assert.IsNotNull(result);
        }

        [Test]
        public async Task GetProposalByProposalId_EmptyId_ReturnsBadRequest()
        {
            var result = await _controller.GetProposalByProposalId(Guid.Empty) as BadRequestObjectResult;
            Assert.IsNotNull(result);
        }

        [Test]
        public async Task GetProposalByProposalId_NotFound_ReturnsNotFound()
        {
            _serviceMock.Setup(s => s.GetProposalById(It.IsAny<Guid>())).ReturnsAsync((ProposalResponseDTO)null);
            var result = await _controller.GetProposalByProposalId(Guid.NewGuid()) as NotFoundObjectResult;
            Assert.IsNotNull(result);
        }

        [Test]
        public async Task GetProposalsByFreelancerId_EmptyId_ReturnsBadRequest()
        {
            var result = await _controller.GetProposalsByFreelancerId(Guid.Empty, null) as BadRequestObjectResult;
            Assert.IsNotNull(result);
        }

        [Test]
        public async Task GetAllProposals_NotFound_ReturnsNotFound()
        {
            _serviceMock.Setup(s => s.GetAllProposalsPaged(It.IsAny<PaginationParams>())).ReturnsAsync((PagedResponse<ProposalResponseDTO>)null);
            var result = await _controller.GetAllProposals(null) as NotFoundObjectResult;
            Assert.IsNotNull(result);
        }

        [Test]
        public async Task UpdateProposal_EmptyId_ReturnsBadRequest()
        {
            var result = await _controller.UpdateProposal(Guid.Empty, new UpdateProposalDTO()) as BadRequestObjectResult;
            Assert.IsNotNull(result);
        }

        [Test]
        public async Task UpdateProposal_NullDto_ReturnsBadRequest()
        {
            var result = await _controller.UpdateProposal(Guid.NewGuid(), null) as BadRequestObjectResult;
            Assert.IsNotNull(result);
        }

        [Test]
        public async Task DeleteProposal_EmptyId_ReturnsBadRequest()
        {
            SetUser(_controller, Guid.NewGuid());
            _serviceMock.Setup(s => s.GetProposalById(It.IsAny<Guid>())).ReturnsAsync((ProposalResponseDTO)null);
            var result = await _controller.DeleteProposal(Guid.Empty) as BadRequestObjectResult;
            Assert.IsNotNull(result);
        }
    }
}