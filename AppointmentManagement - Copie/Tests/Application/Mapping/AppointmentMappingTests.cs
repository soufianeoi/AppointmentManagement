using AppointmentManagement.Application.Appointments.Mapping;
using AppointmentManagement.Domain.Entities;
using AppointmentManagement.Domain.Enums;
using AppointmentManagement.Domain.ValueObjects;

namespace Tests.Application.Mapping;

[TestClass]
public class AppointmentMappingTests
{
    private Appointment _appointment;

    [TestInitialize]
    public void Setup()
    {
        var dateRange = DateRange.Create(DateTime.Now.AddDays(1), DateTime.Now.AddDays(1).AddHours(1));
        _appointment = Appointment.Create(
            "Medical Consultation",
            "Annual checkup",
            dateRange,
            "John Doe",
            "john.doe@email.com",
            "+1234567890",
            "Dr. Smith");
    }

    [TestMethod]
    public void ToDto_ShouldMapAllPropertiesCorrectly()
    {
        // Act
        var dto = _appointment.ToDto();

        // Assert
        dto.Should().NotBeNull();
        dto.Id.Should().Be(_appointment.Id.Value);
        dto.Title.Should().Be(_appointment.Title);
        dto.Description.Should().Be(_appointment.Description);
        dto.StartDate.Should().Be(_appointment.DateRange.StartDate);
        dto.EndDate.Should().Be(_appointment.DateRange.EndDate);
        dto.PatientName.Should().Be(_appointment.PatientName);
        dto.PatientEmail.Should().Be(_appointment.PatientEmail);
        dto.PatientPhone.Should().Be(_appointment.PatientPhone);
        dto.DoctorName.Should().Be(_appointment.DoctorName);
        dto.Status.Should().Be(_appointment.Status);
        dto.CreatedAt.Should().Be(_appointment.CreatedAt);
        dto.UpdatedAt.Should().Be(_appointment.UpdatedAt);
    }

    [TestMethod]
    public void ToDto_WithUpdatedAppointment_ShouldMapUpdatedAtCorrectly()
    {
        // Arrange
        var newDateRange = DateRange.Create(DateTime.Now.AddDays(2), DateTime.Now.AddDays(2).AddHours(2));
        _appointment.UpdateDetails("Updated Title", "Updated Description", newDateRange);

        // Act
        var dto = _appointment.ToDto();

        // Assert
        dto.Should().NotBeNull();
        dto.Title.Should().Be("Updated Title");
        dto.Description.Should().Be("Updated Description");
        dto.UpdatedAt.Should().NotBeNull();
        dto.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [TestMethod]
    public void ToDto_WithConfirmedAppointment_ShouldMapStatusCorrectly()
    {
        // Arrange
        _appointment.Confirm();

        // Act
        var dto = _appointment.ToDto();

        // Assert
        dto.Should().NotBeNull();
        dto.Status.Should().Be(AppointmentStatus.Confirmed);
        dto.UpdatedAt.Should().NotBeNull();
    }

    [TestMethod]
    public void ToDto_WithCancelledAppointment_ShouldMapStatusCorrectly()
    {
        // Arrange
        _appointment.Cancel();

        // Act
        var dto = _appointment.ToDto();

        // Assert
        dto.Should().NotBeNull();
        dto.Status.Should().Be(AppointmentStatus.Cancelled);
        dto.UpdatedAt.Should().NotBeNull();
    }

    [TestMethod]
    public void ToDto_WithInProgressAppointment_ShouldMapStatusCorrectly()
    {
        // Arrange
        _appointment.Confirm();
        _appointment.MarkInProgress();

        // Act
        var dto = _appointment.ToDto();

        // Assert
        dto.Should().NotBeNull();
        dto.Status.Should().Be(AppointmentStatus.InProgress);
        dto.UpdatedAt.Should().NotBeNull();
    }

    [TestMethod]
    public void ToDto_WithCompletedAppointment_ShouldMapStatusCorrectly()
    {
        // Arrange
        _appointment.Confirm();
        _appointment.MarkInProgress();
        _appointment.Complete();

        // Act
        var dto = _appointment.ToDto();

        // Assert
        dto.Should().NotBeNull();
        dto.Status.Should().Be(AppointmentStatus.Completed);
        dto.UpdatedAt.Should().NotBeNull();
    }

    [TestMethod]
    public void ToDto_WithNoShowAppointment_ShouldMapStatusCorrectly()
    {
        // Arrange
        _appointment.Confirm();
        _appointment.MarkNoShow();

        // Act
        var dto = _appointment.ToDto();

        // Assert
        dto.Should().NotBeNull();
        dto.Status.Should().Be(AppointmentStatus.NoShow);
        dto.UpdatedAt.Should().NotBeNull();
    }
}