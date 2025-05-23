using Moq;
using Microsoft.AspNetCore.Mvc;
using Tutorial11.Controllers;
using Tutorial11.Services;
using Tutorial11.DTOs;
using Tutorial11.Exceptions;
namespace Tutorial11Tests;

public class PrescriptionsControllerTests
{
    private Mock<IDbService> _mockService;
    private PrescriptionsController _controller;
    [SetUp]
    public void Setup()
    {
        _mockService = new Mock<IDbService>();
        _controller = new PrescriptionsController(_mockService.Object);
    }
    [Test]
    public async Task AddPrescription_ReturnsCreated_WhenValid()
    {
        var request = new PrescriptionRequestDto
        {
            Date = DateTime.Today,
            DueDate = DateTime.Today.AddDays(5),
            IdDoctor = 1,
            Patient = new PatientDto
            {
                FirstName = "Alice",
                LastName = "Smith",
                BirthDate = DateTime.Parse("1985-05-20")
            },
            Medicaments = new List<PrescriptionMedicamentDto>
            {
                new() { IdMedicament = 1, Dose = 2, Description = "After meals" }
            }
        };
        _mockService.Setup(s => s.AddPrescription(request)).ReturnsAsync(100);
        var result = await _controller.AddPrescription(request);
        Assert.IsInstanceOf<CreatedAtActionResult>(result);
        var created = result as CreatedAtActionResult;
        Assert.That(created.StatusCode, Is.EqualTo(201));
        Assert.That(created.Value, Is.EqualTo(100));
    }
    [Test]
    public async Task AddPrescription_ReturnsBadRequest_WhenInvalid()
    {
        var request = new PrescriptionRequestDto();
        _mockService.Setup(s => s.AddPrescription(request))
            .ThrowsAsync(new BadRequestException("Validation failed"));
        var result = await _controller.AddPrescription(request);
        Assert.IsInstanceOf<BadRequestObjectResult>(result);
        var badRequest = result as BadRequestObjectResult;
        Assert.That(badRequest.StatusCode, Is.EqualTo(400));
    }
    [Test]
    public async Task GetPrescription_ReturnsOk_WhenFound()
    {
        var dto = new PrescriptionDto { IdPrescription = 1, IdDoctor = 1, IdPatient = 1 };
        _mockService.Setup(s => s.GetPrescriptionByIdAsync(1)).ReturnsAsync(dto);
        var result = await _controller.GetPrescription(1);
        Assert.IsInstanceOf<OkObjectResult>(result);
        var okResult = result as OkObjectResult;
        Assert.That(okResult.Value, Is.EqualTo(dto));
    }
    [Test]
    public async Task GetPrescription_ReturnsNotFound_WhenMissing()
    {
        _mockService.Setup(s => s.GetPrescriptionByIdAsync(1))
            .ThrowsAsync(new NotFoundException("Not found"));
        var result = await _controller.GetPrescription(1);
        Assert.IsInstanceOf<NotFoundObjectResult>(result);
        var notFound = result as NotFoundObjectResult;
        Assert.That(notFound.StatusCode, Is.EqualTo(404));
    }
}
