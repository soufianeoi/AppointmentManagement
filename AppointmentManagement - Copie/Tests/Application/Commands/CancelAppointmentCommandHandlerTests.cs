using AppointmentManagement.Application.Appointments.Commands.CancelAppointment;
using AppointmentManagement.Domain.Abstractions;
using AppointmentManagement.Domain.Entities;
using AppointmentManagement.Domain.Enums;
using AppointmentManagement.Domain.ValueObjects;

namespace Tests.Application.Commands;

[TestClass]
public class CancelAppointmentCommandHandlerTests
{
    private Mock<IAppointmentRepository> _mockRepository;
    private Mock<IUnitOfWork> _mockUnitOfWork;
    private CancelAppointmentCommandHandler _handler;
    private Appointment _existingAppointment;
    private AppointmentId _appointmentId;

    [TestInitialize]
    public void Setup()
    {
        _mockRepository = new Mock<IAppointmentRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _handler = new CancelAppointmentCommandHandler(_mockRepository.Object, _mockUnitOfWork.Object);

        // Create a valid appointment for testing
        _appointmentId = AppointmentId.New();
        var dateRange = DateRange.Create(DateTime.Now.AddDays(1), DateTime.Now.AddDays(1).AddHours(1));
        _existingAppointment = Appointment.Create(
            "Medical Consultation",
            "Annual checkup",
            dateRange,
            "John Doe",
            "john.doe@email.com",
            "+1234567890",
            "Dr. Smith");
    }

    [TestMethod]
    public async Task Handle_WithValidCommand_ShouldCancelAppointmentAndReturnSuccess()
    {
        // Arrange
        var command = new CancelAppointmentCommand(_appointmentId.Value);

        _mockRepository.Setup(x => x.GetByIdAsync(_appointmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_existingAppointment);

        _mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _existingAppointment.Status.Should().Be(AppointmentStatus.Cancelled);

        _mockRepository.Verify(x => x.GetByIdAsync(_appointmentId, It.IsAny<CancellationToken>()), Times.Once);
        _mockRepository.Verify(x => x.Update(_existingAppointment), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [TestMethod]
    public async Task Handle_WithNonExistentAppointment_ShouldReturnFailure()
    {
        // Arrange
        var command = new CancelAppointmentCommand(Guid.NewGuid());

        _mockRepository.Setup(x => x.GetByIdAsync(It.IsAny<AppointmentId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Appointment?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Appointment not found");

        _mockRepository.Verify(x => x.Update(It.IsAny<Appointment>()), Times.Never);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [TestMethod]
    public async Task Handle_WithCompletedAppointment_ShouldReturnFailure()
    {
        // Arrange
        _existingAppointment.Confirm();
        _existingAppointment.MarkInProgress();
        _existingAppointment.Complete(); // Complete the appointment

        var command = new CancelAppointmentCommand(_appointmentId.Value);

        _mockRepository.Setup(x => x.GetByIdAsync(_appointmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_existingAppointment);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Cannot cancel a completed or already cancelled appointment");

        _mockRepository.Verify(x => x.Update(It.IsAny<Appointment>()), Times.Never);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [TestMethod]
    public async Task Handle_WithAlreadyCancelledAppointment_ShouldReturnFailure()
    {
        // Arrange
        _existingAppointment.Cancel(); // Cancel the appointment first

        var command = new CancelAppointmentCommand(_appointmentId.Value);

        _mockRepository.Setup(x => x.GetByIdAsync(_appointmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_existingAppointment);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Cannot cancel a completed or already cancelled appointment");

        _mockRepository.Verify(x => x.Update(It.IsAny<Appointment>()), Times.Never);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [TestMethod]
    public async Task Handle_WhenRepositoryThrowsException_ShouldReturnFailure()
    {
        // Arrange
        var command = new CancelAppointmentCommand(_appointmentId.Value);

        _mockRepository.Setup(x => x.GetByIdAsync(_appointmentId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("An error occurred while cancelling the appointment");
    }
}