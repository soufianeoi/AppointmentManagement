using AppointmentManagement.Domain.ValueObjects;

namespace Tests.Domain.ValueObjects;

[TestClass]
public class AppointmentIdTests
{
    [TestMethod]
    public void New_ShouldCreateUniqueIds()
    {
        // Act
        var id1 = AppointmentId.New();
        var id2 = AppointmentId.New();

        // Assert
        id1.Should().NotBe(id2);
        id1.Value.Should().NotBe(Guid.Empty);
        id2.Value.Should().NotBe(Guid.Empty);
    }

    [TestMethod]
    public void Empty_ShouldReturnEmptyGuid()
    {
        // Act
        var emptyId = AppointmentId.Empty;

        // Assert
        emptyId.Value.Should().Be(Guid.Empty);
    }

    [TestMethod]
    public void ImplicitConversion_FromGuid_ShouldWork()
    {
        // Arrange
        var guid = Guid.NewGuid();

        // Act
        AppointmentId appointmentId = guid;

        // Assert
        appointmentId.Value.Should().Be(guid);
    }

    [TestMethod]
    public void ImplicitConversion_ToGuid_ShouldWork()
    {
        // Arrange
        var appointmentId = AppointmentId.New();

        // Act
        Guid guid = appointmentId;

        // Assert
        guid.Should().Be(appointmentId.Value);
    }

    [TestMethod]
    public void ToString_ShouldReturnGuidString()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var appointmentId = new AppointmentId(guid);

        // Act
        var result = appointmentId.ToString();

        // Assert
        result.Should().Be(guid.ToString());
    }

    [TestMethod]
    public void Constructor_WithGuid_ShouldSetValue()
    {
        // Arrange
        var guid = Guid.NewGuid();

        // Act
        var appointmentId = new AppointmentId(guid);

        // Assert
        appointmentId.Value.Should().Be(guid);
    }
}