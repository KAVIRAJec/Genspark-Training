using System.Text;
using System.Threading.Tasks;
using Freelance_Project.Misc;
using Freelance_Project.Models;
using Freelance_Project.Services;
using NUnit.Framework;

namespace Freelance_Project.Test.Services
{
    [TestFixture]
    public class HashingServiceTest
    {
        private HashingService _hashingService;

        [SetUp]
        public void Setup()
        {
            _hashingService = new HashingService();
        }

        [Test]
        public async Task HashData_ValidInput_ReturnsHashedData()
        {
            var model = new HashingModel { Data = "password123" };
            var result = await _hashingService.HashData(model);

            Assert.IsNotNull(result.HashedData);
            Assert.IsNotNull(result.HashKey);
            Assert.IsNotEmpty(result.HashedData);
            Assert.IsNotEmpty(result.HashKey);
        }

        [Test]
        public void HashData_NullModel_ThrowsAppException()
        {
            Assert.ThrowsAsync<AppException>(async () => await _hashingService.HashData(null));
        }

        [Test]
        public void HashData_EmptyData_ThrowsAppException()
        {
            var model = new HashingModel { Data = "" };
            Assert.ThrowsAsync<AppException>(async () => await _hashingService.HashData(model));
        }

        [Test]
        public async Task VerifyHash_ValidHash_ReturnsTrue()
        {
            var model = new HashingModel { Data = "testPassword" };
            var hashed = await _hashingService.HashData(model);

            var verifyModel = new HashingModel
            {
                Data = "testPassword",
                HashKey = hashed.HashKey,
                HashedData = hashed.HashedData
            };

            var result = await _hashingService.VerifyHash(verifyModel);
            Assert.IsTrue(result);
        }

        [Test]
        public async Task VerifyHash_InvalidHash_ReturnsFalse()
        {
            var model = new HashingModel { Data = "testPassword" };
            var hashed = await _hashingService.HashData(model);

            var verifyModel = new HashingModel
            {
                Data = "wrongPassword",
                HashKey = hashed.HashKey,
                HashedData = hashed.HashedData
            };

            var result = await _hashingService.VerifyHash(verifyModel);
            Assert.IsFalse(result);
        }

        [Test]
        public void VerifyHash_NullModel_ThrowsAppException()
        {
            Assert.ThrowsAsync<AppException>(async () => await _hashingService.VerifyHash(null));
        }

        [Test]
        public void VerifyHash_MissingFields_ThrowsAppException()
        {
            var model = new HashingModel { Data = "abc" }; // Missing HashKey and HashedData
            Assert.ThrowsAsync<AppException>(async () => await _hashingService.VerifyHash(model));
        }

        [TearDown]
        public void TearDown()
        {
            _hashingService = null;
        }
    }
}