using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Freelance_Project.Controllers.V1;
using Freelance_Project.Interfaces;
using Freelance_Project.Models;
using Freelance_Project.Models.DTO;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace Freelance_Project.Test.Controllers.V1
{
    [TestFixture]
    public class FreelancerControllerTest
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
        private Mock<IFreelancerService> _freelancerServiceMock;
        private FreelancerController _controller;

        [SetUp]
        public void SetUp()
        {
            _freelancerServiceMock = new Mock<IFreelancerService>();
            _controller = new FreelancerController(_freelancerServiceMock.Object);
        }

        [Test]
        public async Task CreateFreelancer_ReturnsSuccess_WhenValid()
        {
            var dto = new CreateFreelancerDTO
            {
                Username = "testuser",
                Email = "test@example.com",
                ExperienceYears = 5,
                HourlyRate = 50,
                Password = "Test1234",
                ProfileUrl = "http://example.com/profile.jpg",
                Location = "Remote"
            };
            var response = new FreelancerResponseDTO
            {
                Id = Guid.NewGuid(),
                Username = "testuser",
                Email = "test@example.com",
                ExperienceYears = 5,
                HourlyRate = 50,
                ProfileUrl = "http://example.com/profile.jpg",
                Location = "Remote"
            };
            _freelancerServiceMock.Setup(s => s.CreateFreelancer(dto)).ReturnsAsync(response);

            var result = await _controller.CreateFreelancer(dto);

            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public async Task CreateFreelancer_ReturnsBadRequest_WhenNull()
        {
            var result = await _controller.CreateFreelancer(null);

            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        [Test]
        public async Task GetFreelancerById_ReturnsSuccess_WhenFound()
        {
            var id = Guid.NewGuid();
            var response = new FreelancerResponseDTO
            {
                Id = Guid.NewGuid(),
                Username = "testuser",
                Email = "test@example.com",
                ExperienceYears = 5,
                HourlyRate = 50,
                ProfileUrl = "http://example.com/profile.jpg",
                Location = "Remote"
            };
            _freelancerServiceMock.Setup(s => s.GetFreelancerById(id)).ReturnsAsync(response);

            var result = await _controller.GetFreelancerById(id);

            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public async Task GetFreelancerById_ReturnsNotFound_WhenMissing()
        {
            var id = Guid.NewGuid();
            _freelancerServiceMock.Setup(s => s.GetFreelancerById(id)).ReturnsAsync((FreelancerResponseDTO)null);

            var result = await _controller.GetFreelancerById(id);

            Assert.IsInstanceOf<NotFoundObjectResult>(result);
        }

        [Test]
        public async Task GetAllFreelancers_ReturnsSuccess_WhenFound()
        {
            var paged = new PagedResponse<FreelancerResponseDTO>
            {
                Data = new List<FreelancerResponseDTO> { new FreelancerResponseDTO() }
            };
            _freelancerServiceMock.Setup(s => s.GetAllFreelancersPaged(It.IsAny<PaginationParams>())).ReturnsAsync(paged);

            var result = await _controller.GetAllFreelancers(new PaginationParams());

            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public async Task GetAllFreelancers_ReturnsNotFound_WhenNone()
        {
            _freelancerServiceMock.Setup(s => s.GetAllFreelancersPaged(It.IsAny<PaginationParams>())).ReturnsAsync((PagedResponse<FreelancerResponseDTO>)null);

            var result = await _controller.GetAllFreelancers(new PaginationParams());

            Assert.IsInstanceOf<NotFoundObjectResult>(result);
        }

        [Test]
        public async Task UpdateFreelancer_ReturnsSuccess_WhenUpdated()
        {
            var id = Guid.NewGuid();
            var dto = new UpdateFreelancerDTO
            {
                Username = "updatedUser",
                ExperienceYears = 6,
                HourlyRate = 60,
                ProfileUrl = "http://example.com/updated_profile.jpg",
                Location = "Remote"
            };
            var response = new FreelancerResponseDTO
            {
                Id = id,
                Username = "testuser",
                Email = "test@example.com",
                ExperienceYears = 5,
                HourlyRate = 50,
                ProfileUrl = "http://example.com/profile.jpg",
                Location = "Remote"
            };
            _freelancerServiceMock.Setup(s => s.UpdateFreelancer(id, dto)).ReturnsAsync(response);
            SetUser(_controller, id);
            var result = await _controller.UpdateFreelancer(id, dto);

            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public async Task UpdateFreelancer_ReturnsNotFound_WhenMissing()
        {
            var id = Guid.NewGuid();
            var dto = new UpdateFreelancerDTO
            {
                Username = "updatedUser",
                ExperienceYears = 6,
                HourlyRate = 60,
                ProfileUrl = "http://example.com/updated_profile.jpg",
                Location = "Remote"
            };
            _freelancerServiceMock.Setup(s => s.UpdateFreelancer(id, dto)).ReturnsAsync((FreelancerResponseDTO)null);
            SetUser(_controller, id);
            var result = await _controller.UpdateFreelancer(id, dto);

            Assert.IsInstanceOf<NotFoundObjectResult>(result);
        }

        [Test]
        public async Task DeleteFreelancer_ReturnsSuccess_WhenDeleted()
        {
            var id = Guid.NewGuid();
            var response = new FreelancerResponseDTO
            {
                Id = id,
                Username = "testuser",
                Email = "test@example.com",
                ExperienceYears = 5,
                HourlyRate = 50,
                ProfileUrl = "http://example.com/profile.jpg",
                Location = "Remote"
            };
            _freelancerServiceMock.Setup(s => s.DeleteFreelancer(id)).ReturnsAsync(response);
            SetUser(_controller, id);
            var result = await _controller.DeleteFreelancer(id);

            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public async Task DeleteFreelancer_ReturnsNotFound_WhenMissing()
        {
            var id = Guid.NewGuid();
            _freelancerServiceMock.Setup(s => s.DeleteFreelancer(id)).ReturnsAsync((FreelancerResponseDTO)null);
            SetUser(_controller, id);
            var result = await _controller.DeleteFreelancer(id);

            Assert.IsInstanceOf<NotFoundObjectResult>(result);
        }
    }
}