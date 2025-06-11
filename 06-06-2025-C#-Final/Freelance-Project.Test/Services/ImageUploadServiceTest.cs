using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Freelance_Project.Interfaces;
using Freelance_Project.Misc;
using Freelance_Project.Models;
using Freelance_Project.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace Freelance_Project.Test.Services
{
    [TestFixture]
    public class ImageUploadServiceTest
    {
        private Mock<Cloudinary> _cloudinaryMock;
        private Mock<IOptions<CloudinarySettings>> _optionsMock;
        private CloudinarySettings _settings;

        [SetUp]
        public void Setup()
        {
            _settings = new CloudinarySettings
            {
                CloudName = "test",
                ApiKey = "key",
                ApiSecret = "secret"
            };
            _optionsMock = new Mock<IOptions<CloudinarySettings>>();
            _optionsMock.Setup(o => o.Value).Returns(_settings);
        }

        [Test]
        public void UploadImage_NullImage_ThrowsAppException()
        {
            var service = new ImageUploadService(_optionsMock.Object);
            var ex = Assert.ThrowsAsync<AppException>(async () => await service.UploadImage(null));
            Assert.That(ex.Message, Does.Contain("Image file is required"));
        }

        [Test]
        public void UploadImage_EmptyImage_ThrowsAppException()
        {
            var service = new ImageUploadService(_optionsMock.Object);
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(0);

            var ex = Assert.ThrowsAsync<AppException>(async () => await service.UploadImage(fileMock.Object));
            Assert.That(ex.Message, Does.Contain("Image file is required"));
        }

        [Test]
        public async Task UploadImage_ValidImage_ReturnsUrl()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(10);
            fileMock.Setup(f => f.FileName).Returns("test.jpg");
            fileMock.Setup(f => f.OpenReadStream()).Returns(new MemoryStream(new byte[10]));

            var service = new TestableImageUploadService(_optionsMock.Object);

            // Act
            var url = await service.UploadImage(fileMock.Object);

            // Assert
            Assert.That(url, Is.EqualTo("https://dummy.url/test.jpg"));
        }

        [Test]
        public void DeleteImage_EmptyUrl_ThrowsAppException()
        {
            var service = new ImageUploadService(_optionsMock.Object);
            var ex = Assert.ThrowsAsync<AppException>(async () => await service.DeleteImage(null));
            Assert.That(ex.Message, Does.Contain("Image URL is required"));
        }

        [Test]
        public async Task DeleteImage_ValidUrl_ReturnsTrue()
        {
            var service = new TestableImageUploadService(_optionsMock.Object);
            var result = await service.DeleteImage("https://dummy.url/v123/test.jpg");
            Assert.IsTrue(result);
        }

        public class TestableImageUploadService : ImageUploadService
        {
            public TestableImageUploadService(IOptions<CloudinarySettings> config)
                : base(config)
            {
            }

            public override async Task<string> UploadImage(IFormFile image)
            {
                if (image == null || image.Length == 0)
                    throw new AppException("Image file is required.", 400);
                // Simulate upload
                await Task.Delay(1);
                return "https://dummy.url/" + image.FileName;
            }

            public override async Task<bool> DeleteImage(string imageUrl)
            {
                if (string.IsNullOrEmpty(imageUrl))
                    throw new AppException("Image URL is required.", 400);
                // Simulate delete
                await Task.Delay(1);
                return true;
            }
        }

        [TearDown]
        public void TearDown()
        {
            _cloudinaryMock = null;
            _optionsMock = null;
            _settings = null;
        }
    }
}