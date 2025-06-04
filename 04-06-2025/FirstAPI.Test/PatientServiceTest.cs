using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using Moq;
using FirstAPI.Interfaces;
using FirstAPI.Models;
using FirstAPI.Misc;
using FirstAPI.Contexts;
using FirstAPI.Models.DTOs.DoctorSpecialities;
using FirstAPI.Services;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public class PatientServiceTests
{
    private ClinicContext _context;
    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<ClinicContext>()
                            .UseInMemoryDatabase("TestDb")
                            .Options;
        _context = new ClinicContext(options);
    }

    [Test]
    public async Task AddPatient_ShouldReturnPatient_WhenPatientIsAdded()
    {
        // Arrange
        var patientRepoMock = new Mock<IRepository<int, Patient>>();
        var userRepoMock = new Mock<IRepository<string, User>>();
        var otherContextMock = new Mock<IOtherContextFunctionalities>();
        var mapperMock = new Mock<IMapper>();
        var encryptionMock = new Mock<IEncryptionService>();
        var loggerMock = new Mock<ILogger<PatientService>>();

        var patientDto = new PatientAddRequestDto
        {
            Name = "Test Patient",
            Age = 30,
            Email = "test@pat.com",
            Phone = "1234567890",
            Password = "pass"
        };

        var user = new User { Username = "test@pat.com", Role = "Patient" };
        var patient = new Patient { Id = 1, Name = "Test Patient", Age = 30, Email = "test@pat.com", Phone = "1234567890" };

        mapperMock.Setup(m => m.Map<PatientAddRequestDto, User>(patientDto)).Returns(user);
        encryptionMock.Setup(e => e.EncryptData(It.IsAny<EncryptModel>()))
                .ReturnsAsync(new EncryptModel { EncryptedData = new byte[] { 1, 2, 3 }, HashKey = new byte[] { 4, 5, 6 } });
        userRepoMock.Setup(r => r.Add(It.IsAny<User>())).ReturnsAsync(user);
        mapperMock.Setup(m => m.Map<PatientAddRequestDto, Patient>(patientDto)).Returns(patient);
        patientRepoMock.Setup(r => r.GetAll()).ReturnsAsync(new List<Patient>());
        patientRepoMock.Setup(r => r.Add(It.IsAny<Patient>())).ReturnsAsync(patient);

        var service = new PatientService(
            patientRepoMock.Object,
            userRepoMock.Object,
            otherContextMock.Object,
            mapperMock.Object,
            encryptionMock.Object,
            loggerMock.Object
        );

        // Act
        var result = await service.AddPatient(patientDto);

        // Assert
        Assert.NotNull(result);
        Assert.AreEqual("Test Patient", result.Name);
        Assert.AreEqual(30, result.Age);
        Assert.AreEqual("test@pat.com", result.Email);
        Assert.AreEqual("1234567890", result.Phone);
    }

    [Test]
    public async Task GetPatientByName_ShouldReturnPatient_WhenPatientExists()
    {
        // Arrange
        var patientRepoMock = new Mock<IRepository<int, Patient>>();
        var userRepoMock = new Mock<IRepository<string, User>>();
        var otherContextMock = new Mock<IOtherContextFunctionalities>();
        var mapperMock = new Mock<IMapper>();
        var encryptionMock = new Mock<IEncryptionService>();
        var loggerMock = new Mock<ILogger<PatientService>>();

        var patientList = new List<Patient>
        {
            new Patient { Id = 1, Name = "Test Patient", Age = 30, Email = "test@pat.com", Phone = "1234567890" }
        };

        patientRepoMock.Setup(r => r.GetAll()).ReturnsAsync(patientList);

        var service = new PatientService(
            patientRepoMock.Object,
            userRepoMock.Object,
            otherContextMock.Object,
            mapperMock.Object,
            encryptionMock.Object,
            loggerMock.Object
        );

        // Act
        var result = await service.GetPatientByName("Test Patient");

        // Assert
        Assert.NotNull(result);
        Assert.That(result.Name, Is.EqualTo("Test Patient"));
    }

    [Test]
    public void GetPatientByName_ShouldThrowException_WhenPatientDoesNotExist()
    {
        // Arrange
        var patientRepoMock = new Mock<IRepository<int, Patient>>();
        var userRepoMock = new Mock<IRepository<string, User>>();
        var otherContextMock = new Mock<IOtherContextFunctionalities>();
        var mapperMock = new Mock<IMapper>();
        var encryptionMock = new Mock<IEncryptionService>();
        var loggerMock = new Mock<ILogger<PatientService>>();

        patientRepoMock.Setup(r => r.GetAll()).ReturnsAsync(new List<Patient>());

        var service = new PatientService(
            patientRepoMock.Object,
            userRepoMock.Object,
            otherContextMock.Object,
            mapperMock.Object,
            encryptionMock.Object,
            loggerMock.Object
        );

        // Act & Assert
        var ex = Assert.ThrowsAsync<System.Exception>(async () =>
            await service.GetPatientByName("Nonexistent Patient"));
        Assert.That(ex.Message, Does.Contain("Patient not found"));
    }
    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
    }
}