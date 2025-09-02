using AppointmentManagement.Application.Appointments.Queries.GetAppointmentById;
using AppointmentManagement.Domain.Abstractions;
using AppointmentManagement.Domain.Entities;
using AppointmentManagement.Domain.Enums;
using AppointmentManagement.Domain.ValueObjects;

namespace Tests.Application.Queries;

[TestClass]
public class GetAppointmentByIdQueryHandlerTests
{
    private Mock<IAppointmentRepository> _mockRepository;
    private GetAppointmentByIdQueryHandler _handler;
    private Appointment _existingAppointment;
    private AppointmentId _appointmentId;

    [TestInitialize]
    public void Setup()
    {
        _mockRepository = new Mock<IAppointmentRepository>();
        _handler = new GetAppointmentByIdQueryHandler(_mockRepository.Object);

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
    public async Task Handle_WithExistingAppointment_ShouldReturnAppointmentDto()
    {
        // Arrange
        var query = new GetAppointmentByIdQuery(_appointmentId.Value);

        _mockRepository.Setup(x => x.GetByIdAsync(_appointmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_existingAppointment);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Id.Should().Be(_existingAppointment.Id.Value);
        result.Value.Title.Should().Be(_existingAppointment.Title);
        result.Value.Description.Should().Be(_existingAppointment.Description);
        result.Value.PatientName.Should().Be(_existingAppointment.PatientName);
        result.Value.PatientEmail.Should().Be(_existingAppointment.PatientEmail);
        result.Value.PatientPhone.Should().Be(_existingAppointment.PatientPhone);
        result.Value.DoctorName.Should().Be(_existingAppointment.DoctorName);
        result.Value.Status.Should().Be(_existingAppointment.Status);
        result.Value.StartDate.Should().Be(_existingAppointment.DateRange.StartDate);
        result.Value.EndDate.Should().Be(_existingAppointment.DateRange.EndDate);

        _mockRepository.Verify(x => x.GetByIdAsync(_appointmentId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [TestMethod]
    public async Task Handle_WithNonExistentAppointment_ShouldReturnFailure()
    {
        // Arrange
        var query = new GetAppointmentByIdQuery(Guid.NewGuid());

        _mockRepository.Setup(x => x.GetByIdAsync(It.IsAny<AppointmentId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Appointment?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Appointment not found");

        _mockRepository.Verify(x => x.GetByIdAsync(It.IsAny<AppointmentId>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [TestMethod]
    public async Task Handle_WhenRepositoryThrowsException_ShouldReturnFailure()
    {
        // Arrange
        var query = new GetAppointmentByIdQuery(_appointmentId.Value);

        _mockRepository.Setup(x => x.GetByIdAsync(_appointmentId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("An error occurred while retrieving the appointment");
    }

    [TestMethod]
    public async Task Handle_WithConfirmedAppointment_ShouldReturnCorrectStatus()
    {
        // Arrange
        _existingAppointment.Confirm(); // Change status to Confirmed
        var query = new GetAppointmentByIdQuery(_appointmentId.Value);

        _mockRepository.Setup(x => x.GetByIdAsync(_appointmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_existingAppointment);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Status.Should().Be(AppointmentStatus.Confirmed);
        result.Value.UpdatedAt.Should().NotBeNull();

        _mockRepository.Verify(x => x.GetByIdAsync(_appointmentId, It.IsAny<CancellationToken>()), Times.Once);
    }
}