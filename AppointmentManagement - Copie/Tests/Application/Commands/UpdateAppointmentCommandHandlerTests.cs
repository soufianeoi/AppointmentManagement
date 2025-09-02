using AppointmentManagement.Application.Appointments.Commands.UpdateAppointment;
using AppointmentManagement.Domain.Abstractions;
using AppointmentManagement.Domain.Entities;
using AppointmentManagement.Domain.Enums;
using AppointmentManagement.Domain.ValueObjects;

namespace Tests.Application.Commands;

[TestClass]
public class UpdateAppointmentCommandHandlerTests
{
    private Mock<IAppointmentRepository> _mockRepository;
    private Mock<IUnitOfWork> _mockUnitOfWork;
    private UpdateAppointmentCommandHandler _handler;
    private Appointment _existingAppointment;
    private AppointmentId _appointmentId;

    [TestInitialize]
    public void Setup()
    {
        _mockRepository = new Mock<IAppointmentRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _handler = new UpdateAppointmentCommandHandler(_mockRepository.Object, _mockUnitOfWork.Object);

        // Create a valid appointment for testing
        _appointmentId = AppointmentId.New();
        var dateRange = DateRange.Create(DateTime.Now.AddDays(1), DateTime.Now.AddDays(1).AddHours(1));
        _existingAppointment = Appointment.Create(
            "Original Title",
            "Original Description",
            dateRange,
            "John Doe",
            "john.doe@email.com",
            "+1234567890",
            "Dr. Smith");
    }

    [TestMethod]
    public async Task Handle_WithValidCommand_ShouldUpdateAppointmentAndReturnSuccess()
    {
        // Arrange
        var command = new UpdateAppointmentCommand(
            _appointmentId.Value,
            "Updated Title",
            "Updated Description",
            DateTime.Now.AddDays(2),
            DateTime.Now.AddDays(2).AddHours(2));

        _mockRepository.Setup(x => x.GetByIdAsync(_appointmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_existingAppointment);

        _mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        _existingAppointment.Title.Should().Be("Updated Title");
        _existingAppointment.Description.Should().Be("Updated Description");

        _mockRepository.Verify(x => x.GetByIdAsync(_appointmentId, It.IsAny<CancellationToken>()), Times.Once);
        _mockRepository.Verify(x => x.Update(_existingAppointment), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [TestMethod]
    public async Task Handle_WithNonExistentAppointment_ShouldReturnFailure()
    {
        // Arrange
        var command = new UpdateAppointmentCommand(
            Guid.NewGuid(),
            "Updated Title",
            "Updated Description",
            DateTime.Now.AddDays(2),
            DateTime.Now.AddDays(2).AddHours(2));

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
    public async Task Handle_WithInvalidDateRange_ShouldReturnFailure()
    {
        // Arrange
        var command = new UpdateAppointmentCommand(
            _appointmentId.Value,
            "Updated Title",
            "Updated Description",
            DateTime.Now.AddDays(2),
            DateTime.Now.AddDays(2).AddHours(-1)); // End date before start date

        _mockRepository.Setup(x => x.GetByIdAsync(_appointmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_existingAppointment);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Start date must be before end date");

        _mockRepository.Verify(x => x.Update(It.IsAny<Appointment>()), Times.Never);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [TestMethod]
    public async Task Handle_WithCancelledAppointment_ShouldReturnFailure()
    {
        // Arrange
        _existingAppointment.Cancel(); // Cancel the appointment

        var command = new UpdateAppointmentCommand(
            _appointmentId.Value,
            "Updated Title",
            "Updated Description",
            DateTime.Now.AddDays(2),
            DateTime.Now.AddDays(2).AddHours(2));

        _mockRepository.Setup(x => x.GetByIdAsync(_appointmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_existingAppointment);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Cannot update a cancelled or completed appointment");

        _mockRepository.Verify(x => x.Update(It.IsAny<Appointment>()), Times.Never);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [TestMethod]
    public async Task Handle_WithEmptyTitle_ShouldReturnFailure()
    {
        // Arrange
        var command = new UpdateAppointmentCommand(
            _appointmentId.Value,
            "", // Empty title
            "Updated Description",
            DateTime.Now.AddDays(2),
            DateTime.Now.AddDays(2).AddHours(2));

        _mockRepository.Setup(x => x.GetByIdAsync(_appointmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_existingAppointment);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Title cannot be empty");

        _mockRepository.Verify(x => x.Update(It.IsAny<Appointment>()), Times.Never);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [TestMethod]
    public async Task Handle_WhenRepositoryThrowsException_ShouldReturnFailure()
    {
        // Arrange
        var command = new UpdateAppointmentCommand(
            _appointmentId.Value,
            "Updated Title",
            "Updated Description",
            DateTime.Now.AddDays(2),
            DateTime.Now.AddDays(2).AddHours(2));

        _mockRepository.Setup(x => x.GetByIdAsync(_appointmentId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("An error occurred while updating the appointment");
    }
}