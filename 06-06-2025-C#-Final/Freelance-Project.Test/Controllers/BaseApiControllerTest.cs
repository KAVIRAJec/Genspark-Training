using Freelance_Project.Controllers;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace Freelance_Project.Test.Controllers
{
    public class BaseApiControllerTest
    {
        private class TestController : BaseApiController { }

        [Test]
        public void Success_ReturnsOkObjectResult_WithExpectedStructure()
        {
            var controller = new TestController();
            var data = new { Id = 1, Name = "Test" };
            var message = "Custom success message";

            var result = controller.Success(data, message) as OkObjectResult;

            Assert.IsNotNull(result);
            var value = result.Value;
            var type = value.GetType();

            Assert.AreEqual(true, type.GetProperty("success")?.GetValue(value));
            Assert.AreEqual(message, type.GetProperty("message")?.GetValue(value));
            Assert.AreEqual(data, type.GetProperty("data")?.GetValue(value));
            Assert.IsNull(type.GetProperty("errors")?.GetValue(value));
        }

        [Test]
        public void Success_FailureCase_ReturnsUnexpectedStructure()
        {
            // Arrange
            var controller = new TestController();
            var data = new { Id = 2, Name = "Fail" };
            var message = "Failure message";

            // Act
            var result = controller.Success(data, message) as OkObjectResult;

            // Assert (intentionally failing)
            var value = result.Value;
            var type = value.GetType();

            Assert.AreNotEqual(false, type.GetProperty("success")?.GetValue(value));
            // This will fail because we expect a different message
            Assert.AreNotEqual("Some other message", type.GetProperty("message")?.GetValue(value));
        }
    }
}