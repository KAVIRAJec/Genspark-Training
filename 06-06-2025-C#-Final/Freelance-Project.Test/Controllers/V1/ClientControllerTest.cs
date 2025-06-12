using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Freelance_Project.Controllers.V1;
using Freelance_Project.Interfaces;
using Freelance_Project.Models;
using Freelance_Project.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;

namespace Freelance_Project.Test.Controllers.V1
{
    [TestFixture]
    public class ClientControllerTest
    {
        private Mock<IClientService> _clientServiceMock;
        private ClientController _controller;

        [SetUp]
        public void SetUp()
        {
            _clientServiceMock = new Mock<IClientService>();
            _controller = new ClientController(_clientServiceMock.Object);
        }

        private void SetUser(ClientController controller, Guid clientId)
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
        public async Task CreateClient_ReturnsSuccess_WhenValid()
        {
            var dto = new CreateClientDTO
            {
                Username = "clientuser",
                Email = "client@example.com",
                Password = "Test1234",
                CompanyName = "Test Company",
                Location = "Remote"
            };
            var response = new ClientResponseDTO
            {
                Id = Guid.NewGuid(),
                Username = "clientuser",
                Email = "client@example.com",
                CompanyName = "Test Company",
                Location = "Remote"
            };
            _clientServiceMock.Setup(s => s.CreateClient(dto)).ReturnsAsync(response);

            var result = await _controller.CreateClient(dto);

            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public async Task CreateClient_ReturnsBadRequest_WhenNull()
        {
            var result = await _controller.CreateClient(null);

            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        [Test]
        public async Task GetClientById_ReturnsSuccess_WhenFound()
        {
            var id = Guid.NewGuid();
            var response = new ClientResponseDTO { Id = id, Username = "clientuser" };
            _clientServiceMock.Setup(s => s.GetClientById(id)).ReturnsAsync(response);

            var result = await _controller.GetClientById(id);

            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public async Task GetClientById_ReturnsNotFound_WhenMissing()
        {
            var id = Guid.NewGuid();
            _clientServiceMock.Setup(s => s.GetClientById(id)).ReturnsAsync((ClientResponseDTO)null);

            var result = await _controller.GetClientById(id);

            Assert.IsInstanceOf<NotFoundObjectResult>(result);
        }

        [Test]
        public async Task GetAllClients_ReturnsSuccess_WhenFound()
        {
            var paged = new PagedResponse<ClientResponseDTO>
            {
                Data = new List<ClientResponseDTO> { new ClientResponseDTO() }
            };
            _clientServiceMock.Setup(s => s.GetAllClientsPaged(It.IsAny<PaginationParams>())).ReturnsAsync(paged);

            var result = await _controller.GetAllClients(new PaginationParams());

            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public async Task GetAllClients_ReturnsNotFound_WhenNone()
        {
            _clientServiceMock.Setup(s => s.GetAllClientsPaged(It.IsAny<PaginationParams>())).ReturnsAsync((PagedResponse<ClientResponseDTO>)null);

            var result = await _controller.GetAllClients(new PaginationParams());

            Assert.IsInstanceOf<NotFoundObjectResult>(result);
        }

        [Test]
        public async Task UpdateClient_ReturnsSuccess_WhenUpdated()
        {
            var id = Guid.NewGuid();
            var dto = new UpdateClientDTO { CompanyName = "Updated Company" };
            var response = new ClientResponseDTO { Id = id, CompanyName = "Updated Company" };
            _clientServiceMock.Setup(s => s.UpdateClient(id, dto)).ReturnsAsync(response);

            SetUser(_controller, id);
            var result = await _controller.UpdateClient(id, dto);

            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public async Task UpdateClient_ReturnsNotFound_WhenMissing()
        {
            var id = Guid.NewGuid();
            var dto = new UpdateClientDTO { CompanyName = "Updated Company" };
            _clientServiceMock.Setup(s => s.UpdateClient(id, dto)).ReturnsAsync((ClientResponseDTO)null);

            SetUser(_controller, id);
            var result = await _controller.UpdateClient(id, dto);

            Assert.IsInstanceOf<NotFoundObjectResult>(result);
        }

        [Test]
        public async Task DeleteClient_ReturnsSuccess_WhenDeleted()
        {
            var id = Guid.NewGuid();
            var response = new ClientResponseDTO { Id = id, Username = "deleteduser" };
            _clientServiceMock.Setup(s => s.DeleteClient(id)).ReturnsAsync(response);

            SetUser(_controller, id);
            var result = await _controller.DeleteClient(id);

            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public async Task DeleteClient_ReturnsNotFound_WhenMissing()
        {
            var id = Guid.NewGuid();
            _clientServiceMock.Setup(s => s.DeleteClient(id)).ReturnsAsync((ClientResponseDTO)null);

            SetUser(_controller, id);
            var result = await _controller.DeleteClient(id);

            Assert.IsInstanceOf<NotFoundObjectResult>(result);
        }
    }
}