using System.Threading.Tasks;
using Freelance_Project.Controllers.V1;
using Freelance_Project.Interfaces;
using Freelance_Project.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace Freelance_Project.Test.Controllers.V1
{
    [TestFixture]
    public class ImageUploadControllerTest
    {
        private Mock<IImageUploadService> _imageUploadServiceMock;
        private ImageUploadController _controller;

        [SetUp]
        public void SetUp()
        {
            _imageUploadServiceMock = new Mock<IImageUploadService>();
            _controller = new ImageUploadController(_imageUploadServiceMock.Object);
        }

        [Test]
        public async Task UploadImage_ReturnsSuccess_WhenUploadSucceeds()
        {
            var mockFile = new Mock<IFormFile>();
            _imageUploadServiceMock.Setup(s => s.UploadImage(mockFile.Object))
                .ReturnsAsync("http://test.com/image.jpg");

            var result = await _controller.UploadImage(mockFile.Object);

            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public async Task UploadImage_ReturnsBadRequest_WhenUploadFails()
        {
            var mockFile = new Mock<IFormFile>();
            _imageUploadServiceMock.Setup(s => s.UploadImage(mockFile.Object))
                .ReturnsAsync((string)null);

            var result = await _controller.UploadImage(mockFile.Object);

            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        [Test]
        public async Task DeleteImage_ReturnsSuccess_WhenDeleteSucceeds()
        {
            _imageUploadServiceMock.Setup(s => s.DeleteImage("url"))
                .ReturnsAsync(true);

            var result = await _controller.DeleteImage("url");

            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public async Task DeleteImage_ReturnsBadRequest_WhenDeleteFails()
        {
            _imageUploadServiceMock.Setup(s => s.DeleteImage("url"))
                .ReturnsAsync(false);

            var result = await _controller.DeleteImage("url");

            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }
    }
}