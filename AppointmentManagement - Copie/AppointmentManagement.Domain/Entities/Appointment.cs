using AppointmentManagement.Domain.Abstractions;
using AppointmentManagement.Domain.Enums;
using AppointmentManagement.Domain.ValueObjects;

namespace AppointmentManagement.Domain.Entities;

public class Appointment : IAggregateRoot
{
    public AppointmentId Id { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public DateRange DateRange { get; private set; }
    public string PatientName { get; private set; }
    public string PatientEmail { get; private set; }
    public string PatientPhone { get; private set; }
    public string DoctorName { get; private set; }
    public AppointmentStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private Appointment() { } // For EF Core

    private Appointment(
        AppointmentId id,
        string title,
        string description,
        DateRange dateRange,
        string patientName,
        string patientEmail,
        string patientPhone,
        string doctorName)
    {
        Id = id;
        Title = title;
        Description = description;
        DateRange = dateRange;
        PatientName = patientName;
        PatientEmail = patientEmail;
        PatientPhone = patientPhone;
        DoctorName = doctorName;
        Status = AppointmentStatus.Scheduled;
        CreatedAt = DateTime.UtcNow;
    }

    public static Appointment Create(
        string title,
        string description,
        DateRange dateRange,
        string patientName,
        string patientEmail,
        string patientPhone,
        string doctorName)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty", nameof(title));
            
        if (string.IsNullOrWhiteSpace(patientName))
            throw new ArgumentException("Patient name cannot be empty", nameof(patientName));
            
        if (string.IsNullOrWhiteSpace(doctorName))
            throw new ArgumentException("Doctor name cannot be empty", nameof(doctorName));
            
        if (!dateRange.IsValid)
            throw new ArgumentException("Date range is invalid", nameof(dateRange));

        return new Appointment(
            AppointmentId.New(),
            title,
            description,
            dateRange,
            patientName,
            patientEmail,
            patientPhone,
            doctorName);
    }

    public void UpdateDetails(string title, string description, DateRange dateRange)
    {
        if (Status == AppointmentStatus.Cancelled || Status == AppointmentStatus.Completed)
            throw new InvalidOperationException("Cannot update a cancelled or completed appointment");
            
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty", nameof(title));
            
        if (!dateRange.IsValid)
            throw new ArgumentException("Date range is invalid", nameof(dateRange));

        Title = title;
        Description = description;
        DateRange = dateRange;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Confirm()
    {
        if (Status != AppointmentStatus.Scheduled)
            throw new InvalidOperationException("Can only confirm scheduled appointments");
            
        Status = AppointmentStatus.Confirmed;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        if (Status == AppointmentStatus.Completed || Status == AppointmentStatus.Cancelled)
            throw new InvalidOperationException("Cannot cancel a completed or already cancelled appointment");
            
        Status = AppointmentStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkInProgress()
    {
        if (Status != AppointmentStatus.Confirmed)
            throw new InvalidOperationException("Can only start confirmed appointments");
            
        Status = AppointmentStatus.InProgress;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Complete()
    {
        if (Status != AppointmentStatus.InProgress)
            throw new InvalidOperationException("Can only complete appointments that are in progress");
            
        Status = AppointmentStatus.Completed;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkNoShow()
    {
        if (Status != AppointmentStatus.Confirmed)
            throw new InvalidOperationException("Can only mark confirmed appointments as no-show");
            
        Status = AppointmentStatus.NoShow;
        UpdatedAt = DateTime.UtcNow;
    }
}