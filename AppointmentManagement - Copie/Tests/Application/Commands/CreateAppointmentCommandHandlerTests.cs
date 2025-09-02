using AppointmentManagement.Application.Appointments.Commands.CreateAppointment;
using AppointmentManagement.Domain.Abstractions;
using AppointmentManagement.Domain.Entities;
using AppointmentManagement.Domain.ValueObjects;

namespace Tests.Application.Commands;

[TestClass]
public class CreateAppointmentCommandHandlerTests
{
    private Mock<IAppointmentRepository> _mockRepository;
    private Mock<IUnitOfWork> _mockUnitOfWork;
    private CreateAppointmentCommandHandler _handler;

    [TestInitialize]
    public void Setup()
    {
        _mockRepository = new Mock<IAppointmentRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _handler = new CreateAppointmentCommandHandler(_mockRepository.Object, _mockUnitOfWork.Object);
    }

    [TestMethod]
    public async Task Handle_WithValidCommand_ShouldCreateAppointmentAndReturnSuccess()
    {
        // Arrange
        var command = new CreateAppointmentCommand(
            "Medical Consultation",
            "Annual checkup",
            DateTime.Now.AddDays(1),
            DateTime.Now.AddDays(1).AddHours(1),
            "John Doe",
            "john.doe@email.com",
            "+1234567890",
            "Dr. Smith");

        _mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBe(Guid.Empty);

        _mockRepository.Verify(x => x.AddAsync(It.IsAny<Appointment>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [TestMethod]
    public async Task Handle_WithEmptyTitle_ShouldReturnFailure()
    {
        // Arrange
        var command = new CreateAppointmentCommand(
            "",
            "Annual checkup",
            DateTime.Now.AddDays(1),
            DateTime.Now.AddDays(1).AddHours(1),
            "John Doe",
            "john.doe@email.com",
            "+1234567890",
            "Dr. Smith");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Title cannot be empty");

        _mockRepository.Verify(x => x.AddAsync(It.IsAny<Appointment>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [TestMethod]
    public async Task Handle_WithEmptyPatientName_ShouldReturnFailure()
    {
        // Arrange
        var command = new CreateAppointmentCommand(
            "Medical Consultation",
            "Annual checkup",
            DateTime.Now.AddDays(1),
            DateTime.Now.AddDays(1).AddHours(1),
            "",
            "john.doe@email.com",
            "+1234567890",
            "Dr. Smith");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Patient name cannot be empty");

        _mockRepository.Verify(x => x.AddAsync(It.IsAny<Appointment>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [TestMethod]
    public async Task Handle_WithEmptyDoctorName_ShouldReturnFailure()
    {
        // Arrange
        var command = new CreateAppointmentCommand(
            "Medical Consultation",
            "Annual checkup",
            DateTime.Now.AddDays(1),
            DateTime.Now.AddDays(1).AddHours(1),
            "John Doe",
            "john.doe@email.com",
            "+1234567890",
            "");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Doctor name cannot be empty");

        _mockRepository.Verify(x => x.AddAsync(It.IsAny<Appointment>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [TestMethod]
    public async Task Handle_WithInvalidDateRange_ShouldReturnFailure()
    {
        // Arrange
        var command = new CreateAppointmentCommand(
            "Medical Consultation",
            "Annual checkup",
            DateTime.Now.AddDays(1),
            DateTime.Now.AddDays(1).AddHours(-1), // End date before start date
            "John Doe",
            "john.doe@email.com",
            "+1234567890",
            "Dr. Smith");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Start date must be before end date");

        _mockRepository.Verify(x => x.AddAsync(It.IsAny<Appointment>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [TestMethod]
    public async Task Handle_WhenRepositoryThrowsException_ShouldReturnFailure()
    {
        // Arrange
        var command = new CreateAppointmentCommand(
            "Medical Consultation",
            "Annual checkup",
            DateTime.Now.AddDays(1),
            DateTime.Now.AddDays(1).AddHours(1),
            "John Doe",
            "john.doe@email.com",
            "+1234567890",
            "Dr. Smith");

        _mockRepository.Setup(x => x.AddAsync(It.IsAny<Appointment>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("An error occurred while creating the appointment");
    }
}