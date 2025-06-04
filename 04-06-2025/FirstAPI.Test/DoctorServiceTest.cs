using System.Text;
using FirstAPI.Contexts;
using FirstAPI.Models;
using FirstAPI.Repositories;
using FirstAPI.Interfaces;
using FirstAPI.Services;
using FirstAPI.Models.DTOs;
using FirstAPI.Models.DTOs.DoctorSpecialities;
using FirstAPI.Misc;
using Microsoft.EntityFrameworkCore;
using Moq;
using AutoMapper;

namespace FirstAPI.Test;

public class DoctorServiceTest
{
    private ClinicContext _context;
    private Mock<IRepository<int, Doctor>> doctorRepositoryMock;
    private Mock<IRepository<int, Speciality>> specialityRepositoryMock;
    private Mock<IRepository<int, DoctorSpeciality>> doctorSpecialityRepositoryMock;
    private Mock<IRepository<string, User>> userRepositoryMock;
    private Mock<OtherFunctionalitiesImplementation> otherFunctionalitiesImplementationMock;
    private Mock<EncryptionService> encryptionServiceMock;
    private Mock<IMapper> mapperMock;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<ClinicContext>()
                            .UseInMemoryDatabase(Guid.NewGuid().ToString()) // Unique DB per test
                            .Options;
        _context = new ClinicContext(options);

        doctorRepositoryMock = new Mock<IRepository<int, Doctor>>();
        specialityRepositoryMock = new Mock<IRepository<int, Speciality>>();
        doctorSpecialityRepositoryMock = new Mock<IRepository<int, DoctorSpeciality>>();
        userRepositoryMock = new Mock<IRepository<string, User>>();
        otherFunctionalitiesImplementationMock = new Mock<OtherFunctionalitiesImplementation>(_context);
        encryptionServiceMock = new Mock<EncryptionService>();
        mapperMock = new Mock<IMapper>();
    }

    [TestCase("General")]
    public async Task TestGetDoctorBySpeciality(string speciality)
    {
        otherFunctionalitiesImplementationMock.Setup(ocf => ocf.GetDoctorsBySpeciality(It.IsAny<string>()))
            .ReturnsAsync(new List<DoctorsBySpecialityResponseDto>
            {
                new DoctorsBySpecialityResponseDto { Dname = "test", Yoe = 2, Id = 1 }
            });

        IDoctorService doctorService = new DoctorService(doctorRepositoryMock.Object,
                                                        specialityRepositoryMock.Object,
                                                        doctorSpecialityRepositoryMock.Object,
                                                        userRepositoryMock.Object,
                                                        otherFunctionalitiesImplementationMock.Object,
                                                        mapperMock.Object,
                                                        encryptionServiceMock.Object
                                                        );


        //Assert.That(doctorService, Is.Not.Null);
        //Action
        var result = await doctorService.GetDoctorsBySpeciality(speciality);
        //Assert
        Assert.That(result.Count(), Is.EqualTo(1));
    }

    [Test]
    public async Task TestAddDoctor_Success()
    {
        var doctorDto = new DoctorAddRequestDto
        {
            Name = "Dr. Test",
            Password = "password",
            Specialities = new List<SpecialityAddRequestDto> { new SpecialityAddRequestDto { Name = "Cardiology" } }
        };

        var user = new User { Username = "Dr. Test", Role = "Doctor" };
        var encryptedModel = new EncryptModel { EncryptedData = Encoding.UTF8.GetBytes("encrypted"), HashKey = Encoding.UTF8.GetBytes("key") };
        var doctor = new Doctor { Id = 1, Name = "Dr. Test" };
        var speciality = new Speciality { Id = 1, Name = "Cardiology" };
        var doctorSpeciality = new DoctorSpeciality { SerialNumber = 1, DoctorId = 1, SpecialityId = 1 };

        mapperMock.Setup(m => m.Map<DoctorAddRequestDto, User>(It.IsAny<DoctorAddRequestDto>())).Returns(user);
        encryptionServiceMock.Setup(e => e.EncryptData(It.IsAny<EncryptModel>())).ReturnsAsync(encryptedModel);
        userRepositoryMock.Setup(r => r.Add(It.IsAny<User>())).ReturnsAsync(user);
        doctorRepositoryMock.Setup(r => r.Add(It.IsAny<Doctor>())).ReturnsAsync(doctor);
        specialityRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync(new List<Speciality>());
        specialityRepositoryMock.Setup(r => r.Add(It.IsAny<Speciality>())).ReturnsAsync(speciality);
        doctorSpecialityRepositoryMock.Setup(r => r.Add(It.IsAny<DoctorSpeciality>())).ReturnsAsync(doctorSpeciality);

        IDoctorService doctorService = new DoctorService(
            doctorRepositoryMock.Object,
            specialityRepositoryMock.Object,
            doctorSpecialityRepositoryMock.Object,
            userRepositoryMock.Object,
            otherFunctionalitiesImplementationMock.Object,
            mapperMock.Object,
            encryptionServiceMock.Object
        );

        var result = await doctorService.AddDoctor(doctorDto);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Name, Is.EqualTo("Dr. Test"));
    }

    [Test]
    public void TestGetDoctorByName_NotImplemented()
    {
        IDoctorService doctorService = new DoctorService(
            doctorRepositoryMock.Object,
            specialityRepositoryMock.Object,
            doctorSpecialityRepositoryMock.Object,
            userRepositoryMock.Object,
            otherFunctionalitiesImplementationMock.Object,
            mapperMock.Object,
            encryptionServiceMock.Object
        );

        Assert.ThrowsAsync<NotImplementedException>(async () =>
        {
            await doctorService.GetDoctorByName("Dr. Test");
        });
    }

    [TearDown]
    public void TearDown() 
    {
        _context.Dispose();
    }
}