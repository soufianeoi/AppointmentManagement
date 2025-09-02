namespace AppointmentManagement.Domain.ValueObjects;

public record DateRange(DateTime StartDate, DateTime EndDate)
{
    public DateRange() : this(DateTime.MinValue, DateTime.MinValue) { }
    
    public TimeSpan Duration => EndDate - StartDate;
    
    public bool IsValid => StartDate < EndDate;
    
    public bool Contains(DateTime date) => date >= StartDate && date <= EndDate;
    
    public bool Overlaps(DateRange other) => 
        StartDate < other.EndDate && EndDate > other.StartDate;
    
    public static DateRange Create(DateTime startDate, DateTime endDate)
    {
        if (startDate >= endDate)
            throw new ArgumentException("Start date must be before end date");
            
        return new DateRange(startDate, endDate);
    }
}