namespace AppointmentManagement.Domain.ValueObjects;

public record AppointmentId(Guid Value)
{
    public static AppointmentId New() => new(Guid.NewGuid());
    public static AppointmentId Empty => new(Guid.Empty);
    
    public static implicit operator Guid(AppointmentId appointmentId) => appointmentId.Value;
    public static implicit operator AppointmentId(Guid value) => new(value);
    
    public override string ToString() => Value.ToString();
}