using AppointmentManagement.Domain.ValueObjects;

namespace Tests.Domain.ValueObjects;

[TestClass]
public class DateRangeTests
{
    [TestMethod]
    public void Create_WithValidDates_ShouldCreateDateRange()
    {
        // Arrange
        var startDate = DateTime.Now;
        var endDate = startDate.AddHours(1);

        // Act
        var dateRange = DateRange.Create(startDate, endDate);

        // Assert
        dateRange.StartDate.Should().Be(startDate);
        dateRange.EndDate.Should().Be(endDate);
        dateRange.IsValid.Should().BeTrue();
    }

    [TestMethod]
    public void Create_WithInvalidDates_ShouldThrowArgumentException()
    {
        // Arrange
        var startDate = DateTime.Now;
        var endDate = startDate.AddHours(-1);

        // Act & Assert
        var action = () => DateRange.Create(startDate, endDate);
        action.Should().Throw<ArgumentException>()
            .WithMessage("Start date must be before end date");
    }

    [TestMethod]
    public void Duration_ShouldReturnCorrectTimeSpan()
    {
        // Arrange
        var startDate = DateTime.Now;
        var endDate = startDate.AddHours(2);
        var dateRange = DateRange.Create(startDate, endDate);

        // Act
        var duration = dateRange.Duration;

        // Assert
        duration.Should().Be(TimeSpan.FromHours(2));
    }

    [TestMethod]
    public void Contains_WithDateInRange_ShouldReturnTrue()
    {
        // Arrange
        var startDate = DateTime.Now;
        var endDate = startDate.AddHours(2);
        var dateRange = DateRange.Create(startDate, endDate);
        var dateInRange = startDate.AddMinutes(30);

        // Act
        var result = dateRange.Contains(dateInRange);

        // Assert
        result.Should().BeTrue();
    }

    [TestMethod]
    public void Contains_WithDateOutsideRange_ShouldReturnFalse()
    {
        // Arrange
        var startDate = DateTime.Now;
        var endDate = startDate.AddHours(2);
        var dateRange = DateRange.Create(startDate, endDate);
        var dateOutsideRange = startDate.AddHours(3);

        // Act
        var result = dateRange.Contains(dateOutsideRange);

        // Assert
        result.Should().BeFalse();
    }

    [TestMethod]
    public void Overlaps_WithOverlappingRange_ShouldReturnTrue()
    {
        // Arrange
        var startDate1 = DateTime.Now;
        var endDate1 = startDate1.AddHours(2);
        var dateRange1 = DateRange.Create(startDate1, endDate1);

        var startDate2 = startDate1.AddHours(1);
        var endDate2 = startDate2.AddHours(2);
        var dateRange2 = DateRange.Create(startDate2, endDate2);

        // Act
        var result = dateRange1.Overlaps(dateRange2);

        // Assert
        result.Should().BeTrue();
    }

    [TestMethod]
    public void Overlaps_WithNonOverlappingRange_ShouldReturnFalse()
    {
        // Arrange
        var startDate1 = DateTime.Now;
        var endDate1 = startDate1.AddHours(1);
        var dateRange1 = DateRange.Create(startDate1, endDate1);

        var startDate2 = endDate1.AddHours(1);
        var endDate2 = startDate2.AddHours(1);
        var dateRange2 = DateRange.Create(startDate2, endDate2);

        // Act
        var result = dateRange1.Overlaps(dateRange2);

        // Assert
        result.Should().BeFalse();
    }
}