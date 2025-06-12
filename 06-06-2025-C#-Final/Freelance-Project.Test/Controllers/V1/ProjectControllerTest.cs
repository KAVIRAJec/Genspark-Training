using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Freelance_Project.Controllers.V1;
using Freelance_Project.Interfaces;
using Freelance_Project.Misc;
using Freelance_Project.Models;
using Freelance_Project.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Moq;
using NUnit.Framework;

namespace Freelance_Project.Test.Controllers.V1
{
    [TestFixture]
    public class ProjectControllerTest
    {
        private void SetUser(ControllerBase controller, Guid userId)
        {
            var claims = new List<Claim>
            {
                new Claim("Id", userId.ToString())
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var principal = new ClaimsPrincipal(identity);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };
        }

        private Mock<IClientProjectService> _serviceMock;
        private Mock<IHubContext<NotificationHub>> _hubContextMock;
        private Mock<IHubClients> _hubClientsMock;
        private Mock<IClientProxy> _clientProxyMock;
        private ProjectController _controller;

        [SetUp]
        public void SetUp()
        {
            _serviceMock = new Mock<IClientProjectService>();
            _hubContextMock = new Mock<IHubContext<NotificationHub>>();
            _hubClientsMock = new Mock<IHubClients>();
            _clientProxyMock = new Mock<IClientProxy>();

            _hubClientsMock.Setup(c => c.All).Returns(_clientProxyMock.Object);
            _hubClientsMock.Setup(c => c.User(It.IsAny<string>())).Returns(_clientProxyMock.Object);
            _hubContextMock.SetupGet(h => h.Clients).Returns(_hubClientsMock.Object);

            _controller = new ProjectController(_serviceMock.Object, _hubContextMock.Object);
        }

        [Test]
        public async Task PostProject_ReturnsSuccess_WhenValid()
        {
            var clientId = Guid.NewGuid();
            var dto = new CreateProjectDTO { Title = "Test Project", ClientId = clientId };
            var response = new ProjectResponseDTO { Title = "Test Project" };
            _serviceMock.Setup(s => s.PostProject(dto)).ReturnsAsync(response);

            SetUser(_controller, clientId);
            var result = await _controller.PostProject(dto);

            Assert.IsInstanceOf<OkObjectResult>(result);
            _clientProxyMock.Verify(
                c => c.SendCoreAsync("FreelancerNotification", It.IsAny<object[]>(), default), Times.Once);
        }

        [Test]
        public async Task GetProjectByProjectId_ReturnsSuccess_WhenFound()
        {
            var projectId = Guid.NewGuid();
            var response = new ProjectResponseDTO { Id = projectId, Title = "Test" };
            _serviceMock.Setup(s => s.GetProjectById(projectId)).ReturnsAsync(response);

            var result = await _controller.GetProjectByProjectId(projectId);

            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public async Task GetProjectsByClientId_ReturnsSuccess_WhenFound()
        {
            var clientId = Guid.NewGuid();
            var paged = new PagedResponse<ProjectResponseDTO>
            {
                Data = new List<ProjectResponseDTO> { new ProjectResponseDTO() }
            };
            _serviceMock.Setup(s => s.GetProjectsByClientId(clientId, It.IsAny<PaginationParams>())).ReturnsAsync(paged);

            var result = await _controller.GetProjectsByClientId(clientId, new PaginationParams());

            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public async Task GetAllProjects_ReturnsSuccess_WhenFound()
        {
            var paged = new PagedResponse<ProjectResponseDTO>
            {
                Data = new List<ProjectResponseDTO> { new ProjectResponseDTO() }
            };
            _serviceMock.Setup(s => s.GetAllProjectsPaged(It.IsAny<PaginationParams>())).ReturnsAsync(paged);

            var result = await _controller.GetAllProjects(new PaginationParams());

            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public async Task UpdateProject_ReturnsSuccess_AndSendsNotification()
        {
            var projectId = Guid.NewGuid();
            var clientId = Guid.NewGuid();
            var dto = new UpdateProjectDTO { Title = "Updated" };
            var response = new ProjectResponseDTO { Id = projectId, Title = "Updated", ClientId = clientId, FreelancerId = Guid.NewGuid() };
            _serviceMock.Setup(s => s.GetProjectById(projectId)).ReturnsAsync(response);
            _serviceMock.Setup(s => s.UpdateProject(projectId, dto)).ReturnsAsync(response);

            SetUser(_controller, clientId);
            var result = await _controller.UpdateProject(projectId, dto);

            Assert.IsInstanceOf<OkObjectResult>(result);
            _clientProxyMock.Verify(
                c => c.SendCoreAsync("FreelancerNotification", It.IsAny<object[]>(), default), Times.Once);
        }

        [Test]
        public async Task DeleteProject_ReturnsSuccess_AndSendsNotification()
        {
            var projectId = Guid.NewGuid();
            var clientId = Guid.NewGuid();
            var dto = new UpdateProjectDTO { Title = "Deleted" };
            var response = new ProjectResponseDTO { Id = projectId, Title = "Deleted", ClientId = clientId, FreelancerId = Guid.NewGuid() };
            _serviceMock.Setup(s => s.GetProjectById(projectId)).ReturnsAsync(response);
            _serviceMock.Setup(s => s.DeleteProject(projectId)).ReturnsAsync(response);
            SetUser(_controller, clientId);
            var result = await _controller.DeleteProject(projectId);

            Assert.IsInstanceOf<OkObjectResult>(result);
            _clientProxyMock.Verify(
                c => c.SendCoreAsync("FreelancerNotification", It.IsAny<object[]>(), default), Times.Once);
        }
    }
}