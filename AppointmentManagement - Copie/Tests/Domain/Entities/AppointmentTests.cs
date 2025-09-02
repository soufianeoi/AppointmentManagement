using AppointmentManagement.Domain.Entities;
using AppointmentManagement.Domain.Enums;
using AppointmentManagement.Domain.ValueObjects;

namespace Tests.Domain.Entities;

[TestClass]
public class AppointmentTests
{
    private DateRange _validDateRange;
    private const string ValidTitle = "Medical Consultation";
    private const string ValidDescription = "Annual checkup";
    private const string ValidPatientName = "John Doe";
    private const string ValidPatientEmail = "john.doe@email.com";
    private const string ValidPatientPhone = "+1234567890";
    private const string ValidDoctorName = "Dr. Smith";

    [TestInitialize]
    public void Setup()
    {
        var startDate = DateTime.Now.AddDays(1);
        var endDate = startDate.AddHours(1);
        _validDateRange = DateRange.Create(startDate, endDate);
    }

    [TestMethod]
    public void Create_WithValidData_ShouldCreateAppointment()
    {
        // Act
        var appointment = Appointment.Create(
            ValidTitle,
            ValidDescription,
            _validDateRange,
            ValidPatientName,
            ValidPatientEmail,
            ValidPatientPhone,
            ValidDoctorName);

        // Assert
        appointment.Should().NotBeNull();
        appointment.Title.Should().Be(ValidTitle);
        appointment.Description.Should().Be(ValidDescription);
        appointment.DateRange.Should().Be(_validDateRange);
        appointment.PatientName.Should().Be(ValidPatientName);
        appointment.PatientEmail.Should().Be(ValidPatientEmail);
        appointment.PatientPhone.Should().Be(ValidPatientPhone);
        appointment.DoctorName.Should().Be(ValidDoctorName);
        appointment.Status.Should().Be(AppointmentStatus.Scheduled);
        appointment.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        appointment.UpdatedAt.Should().BeNull();
        appointment.Id.Value.Should().NotBe(Guid.Empty);
    }

    [TestMethod]
    [DataRow("")]
    [DataRow(" ")]
    [DataRow(null)]
    public void Create_WithInvalidTitle_ShouldThrowArgumentException(string invalidTitle)
    {
        // Act & Assert
        var action = () => Appointment.Create(
            invalidTitle,
            ValidDescription,
            _validDateRange,
            ValidPatientName,
            ValidPatientEmail,
            ValidPatientPhone,
            ValidDoctorName);

        action.Should().Throw<ArgumentException>()
            .WithMessage("Title cannot be empty*");
    }

    [TestMethod]
    [DataRow("")]
    [DataRow(" ")]
    [DataRow(null)]
    public void Create_WithInvalidPatientName_ShouldThrowArgumentException(string invalidPatientName)
    {
        // Act & Assert
        var action = () => Appointment.Create(
            ValidTitle,
            ValidDescription,
            _validDateRange,
            invalidPatientName,
            ValidPatientEmail,
            ValidPatientPhone,
            ValidDoctorName);

        action.Should().Throw<ArgumentException>()
            .WithMessage("Patient name cannot be empty*");
    }

    [TestMethod]
    [DataRow("")]
    [DataRow(" ")]
    [DataRow(null)]
    public void Create_WithInvalidDoctorName_ShouldThrowArgumentException(string invalidDoctorName)
    {
        // Act & Assert
        var action = () => Appointment.Create(
            ValidTitle,
            ValidDescription,
            _validDateRange,
            ValidPatientName,
            ValidPatientEmail,
            ValidPatientPhone,
            invalidDoctorName);

        action.Should().Throw<ArgumentException>()
            .WithMessage("Doctor name cannot be empty*");
    }

    [TestMethod]
    public void Create_WithInvalidDateRange_ShouldThrowArgumentException()
    {
        // Arrange
        var invalidDateRange = new DateRange(DateTime.Now, DateTime.Now.AddHours(-1));

        // Act & Assert
        var action = () => Appointment.Create(
            ValidTitle,
            ValidDescription,
            invalidDateRange,
            ValidPatientName,
            ValidPatientEmail,
            ValidPatientPhone,
            ValidDoctorName);

        action.Should().Throw<ArgumentException>()
            .WithMessage("Date range is invalid*");
    }

    [TestMethod]
    public void UpdateDetails_WithValidData_ShouldUpdateAppointment()
    {
        // Arrange
        var appointment = CreateValidAppointment();
        var newTitle = "Updated Title";
        var newDescription = "Updated Description";
        var newDateRange = DateRange.Create(DateTime.Now.AddDays(2), DateTime.Now.AddDays(2).AddHours(2));

        // Act
        appointment.UpdateDetails(newTitle, newDescription, newDateRange);

        // Assert
        appointment.Title.Should().Be(newTitle);
        appointment.Description.Should().Be(newDescription);
        appointment.DateRange.Should().Be(newDateRange);
        appointment.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [TestMethod]
    public void UpdateDetails_WhenCancelled_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var appointment = CreateValidAppointment();
        appointment.Cancel();

        // Act & Assert
        var action = () => appointment.UpdateDetails("New Title", "New Description", _validDateRange);
        action.Should().Throw<InvalidOperationException>()
            .WithMessage("Cannot update a cancelled or completed appointment");
    }

    [TestMethod]
    public void UpdateDetails_WhenCompleted_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var appointment = CreateValidAppointment();
        appointment.Confirm();
        appointment.MarkInProgress();
        appointment.Complete();

        // Act & Assert
        var action = () => appointment.UpdateDetails("New Title", "New Description", _validDateRange);
        action.Should().Throw<InvalidOperationException>()
            .WithMessage("Cannot update a cancelled or completed appointment");
    }

    [TestMethod]
    public void Confirm_WhenScheduled_ShouldConfirmAppointment()
    {
        // Arrange
        var appointment = CreateValidAppointment();

        // Act
        appointment.Confirm();

        // Assert
        appointment.Status.Should().Be(AppointmentStatus.Confirmed);
        appointment.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [TestMethod]
    public void Confirm_WhenNotScheduled_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var appointment = CreateValidAppointment();
        appointment.Confirm();

        // Act & Assert
        var action = () => appointment.Confirm();
        action.Should().Throw<InvalidOperationException>()
            .WithMessage("Can only confirm scheduled appointments");
    }

    [TestMethod]
    public void Cancel_WhenScheduledOrConfirmed_ShouldCancelAppointment()
    {
        // Arrange
        var appointment = CreateValidAppointment();

        // Act
        appointment.Cancel();

        // Assert
        appointment.Status.Should().Be(AppointmentStatus.Cancelled);
        appointment.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [TestMethod]
    public void Cancel_WhenCompleted_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var appointment = CreateValidAppointment();
        appointment.Confirm();
        appointment.MarkInProgress();
        appointment.Complete();

        // Act & Assert
        var action = () => appointment.Cancel();
        action.Should().Throw<InvalidOperationException>()
            .WithMessage("Cannot cancel a completed or already cancelled appointment");
    }

    [TestMethod]
    public void MarkInProgress_WhenConfirmed_ShouldMarkAsInProgress()
    {
        // Arrange
        var appointment = CreateValidAppointment();
        appointment.Confirm();

        // Act
        appointment.MarkInProgress();

        // Assert
        appointment.Status.Should().Be(AppointmentStatus.InProgress);
        appointment.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [TestMethod]
    public void MarkInProgress_WhenNotConfirmed_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var appointment = CreateValidAppointment();

        // Act & Assert
        var action = () => appointment.MarkInProgress();
        action.Should().Throw<InvalidOperationException>()
            .WithMessage("Can only start confirmed appointments");
    }

    [TestMethod]
    public void Complete_WhenInProgress_ShouldCompleteAppointment()
    {
        // Arrange
        var appointment = CreateValidAppointment();
        appointment.Confirm();
        appointment.MarkInProgress();

        // Act
        appointment.Complete();

        // Assert
        appointment.Status.Should().Be(AppointmentStatus.Completed);
        appointment.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [TestMethod]
    public void Complete_WhenNotInProgress_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var appointment = CreateValidAppointment();
        appointment.Confirm();

        // Act & Assert
        var action = () => appointment.Complete();
        action.Should().Throw<InvalidOperationException>()
            .WithMessage("Can only complete appointments that are in progress");
    }

    [TestMethod]
    public void MarkNoShow_WhenConfirmed_ShouldMarkAsNoShow()
    {
        // Arrange
        var appointment = CreateValidAppointment();
        appointment.Confirm();

        // Act
        appointment.MarkNoShow();

        // Assert
        appointment.Status.Should().Be(AppointmentStatus.NoShow);
        appointment.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [TestMethod]
    public void MarkNoShow_WhenNotConfirmed_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var appointment = CreateValidAppointment();

        // Act & Assert
        var action = () => appointment.MarkNoShow();
        action.Should().Throw<InvalidOperationException>()
            .WithMessage("Can only mark confirmed appointments as no-show");
    }

    private Appointment CreateValidAppointment()
    {
        return Appointment.Create(
            ValidTitle,
            ValidDescription,
            _validDateRange,
            ValidPatientName,
            ValidPatientEmail,
            ValidPatientPhone,
            ValidDoctorName);
    }
}