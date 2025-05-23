using Microsoft.AspNetCore.Mvc;
using Moq;
using Tutorial11.Controllers;
using Tutorial11.Services;
using Tutorial11.DTOs;
using Tutorial11.Exceptions;
namespace Tutorial11Tests;


public class PatientsControllerTests
{
    private Mock<IDbService> _mockService;
    private PatientsController _controller;

    [SetUp]
    public void Setup()
    {
        _mockService = new Mock<IDbService>();
        _controller = new PatientsController(_mockService.Object);
    }


    [Test]
    public async Task GetPatientDetails_ReturnsOk_WhenPatientExists()
    {
        var patientDto = new PatientDetailsDto { IdPatient = 1, FirstName = "Alice", LastName = "Smith" };
        _mockService.Setup(s => s.GetPatientDetailsByIdAsync(1)).ReturnsAsync(patientDto);

        var result = await _controller.GetPatientDetails(1);

        Assert.IsInstanceOf<OkObjectResult>(result);
        var okResult = result as OkObjectResult;
        Assert.That(okResult.StatusCode, Is.EqualTo(200));
        Assert.That(okResult.Value, Is.EqualTo(patientDto));
    }

    [Test]
    public async Task GetPatientDetails_ReturnsNotFound_WhenPatientDoesNotExist()
    {
        _mockService.Setup(s => s.GetPatientDetailsByIdAsync(1))
            .ThrowsAsync(new NotFoundException("Patient not found"));

        var result = await _controller.GetPatientDetails(1);

        Assert.IsInstanceOf<NotFoundObjectResult>(result);
        var notFoundResult = result as NotFoundObjectResult;
        Assert.That(notFoundResult.StatusCode, Is.EqualTo(404));
    }
}