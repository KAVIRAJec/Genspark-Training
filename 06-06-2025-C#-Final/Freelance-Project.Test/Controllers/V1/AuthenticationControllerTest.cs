using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Freelance_Project.Controllers.V1;
using Freelance_Project.Interfaces;
using Freelance_Project.Models.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;

namespace Freelance_Project.Test.Controllers.V1
{
    [TestFixture]
    public class AuthenticationControllerTest
    {
        private Mock<IAuthenticationService> _authServiceMock;
        private AuthenticationController _controller;

        [SetUp]
        public void SetUp()
        {
            _authServiceMock = new Mock<IAuthenticationService>();
            _controller = new AuthenticationController(_authServiceMock.Object);
        }

        [Test]
        public async Task Login_ReturnsSuccess_WhenValid()
        {
            var dto = new LoginRequestDTO { Email = "test@example.com", Password = "Test1234" };
            var response = new LoginResponseDTO { Token = "token", RefreshToken = "refresh" };
            _authServiceMock.Setup(s => s.Login(dto)).ReturnsAsync(response);

            var result = await _controller.Login(dto);

            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public async Task Login_ReturnsBadRequest_WhenNull()
        {
            var result = await _controller.Login(null);

            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        [Test]
        public async Task Login_ReturnsBadRequest_WhenLoginFails()
        {
            var dto = new LoginRequestDTO { Email = "fail@example.com", Password = "fail" };
            _authServiceMock.Setup(s => s.Login(dto)).ReturnsAsync((LoginResponseDTO)null);

            var result = await _controller.Login(dto);

            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        [Test]
        public async Task Logout_ReturnsSuccess_WhenValid()
        {
            var email = "test@example.com";
            _authServiceMock.Setup(s => s.Logout(email)).ReturnsAsync(true);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Email, email)
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            var result = await _controller.Logout();

            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public async Task Logout_ReturnsBadRequest_WhenNoEmail()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { }, "mock"));
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            var result = await _controller.Logout();

            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        [Test]
        public async Task Logout_ReturnsBadRequest_WhenLogoutFails()
        {
            var email = "test@example.com";
            _authServiceMock.Setup(s => s.Logout(email)).ReturnsAsync(false);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Email, email)
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            var result = await _controller.Logout();

            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        [Test]
        public async Task RefreshToken_ReturnsSuccess_WhenValid()
        {
            var refreshToken = "refresh";
            var response = new LoginResponseDTO { Token = "token", RefreshToken = refreshToken };
            _authServiceMock.Setup(s => s.RefreshToken(refreshToken)).ReturnsAsync(response);

            var result = await _controller.RefreshToken(refreshToken);

            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public async Task RefreshToken_ReturnsBadRequest_WhenNull()
        {
            var result = await _controller.RefreshToken(null);

            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        [Test]
        public async Task RefreshToken_ReturnsBadRequest_WhenRefreshFails()
        {
            var refreshToken = "refresh";
            _authServiceMock.Setup(s => s.RefreshToken(refreshToken)).ReturnsAsync((LoginResponseDTO)null);

            var result = await _controller.RefreshToken(refreshToken);

            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        [Test]
        public async Task GetDetails_ReturnsFreelancer_WhenRoleFreelancer()
        {
            var email = "freelancer@example.com";
            var role = "Freelancer";
            var response = new FreelancerResponseDTO { Email = email, Username = "freelancer" };
            _authServiceMock.Setup(s => s.GetDetails<FreelancerResponseDTO>(email)).ReturnsAsync(response);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, role)
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            var result = await _controller.GetDetails();

            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public async Task GetDetails_ReturnsClient_WhenRoleClient()
        {
            var email = "client@example.com";
            var role = "Client";
            var response = new ClientResponseDTO { Email = email, Username = "client" };
            _authServiceMock.Setup(s => s.GetDetails<ClientResponseDTO>(email)).ReturnsAsync(response);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, role)
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            var result = await _controller.GetDetails();

            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public async Task GetDetails_ReturnsBadRequest_WhenNoEmail()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Role, "Freelancer")
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            var result = await _controller.GetDetails();

            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        [Test]
        public async Task GetDetails_ReturnsBadRequest_WhenInvalidRole()
        {
            var email = "unknown@example.com";
            var role = "Unknown";
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, role)
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            var result = await _controller.GetDetails();

            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }
    }
}