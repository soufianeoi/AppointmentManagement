using AppointmentManagement.Application.Common;

namespace Tests.Application.Common;

[TestClass]
public class ResultTests
{
    [TestMethod]
    public void Success_ShouldCreateSuccessfulResult()
    {
        // Act
        var result = Result.Success();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Error.Should().BeEmpty();
    }

    [TestMethod]
    public void Failure_ShouldCreateFailedResult()
    {
        // Arrange
        const string errorMessage = "Something went wrong";

        // Act
        var result = Result.Failure(errorMessage);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(errorMessage);
    }

    [TestMethod]
    public void Success_WithValue_ShouldCreateSuccessfulResultWithValue()
    {
        // Arrange
        const string value = "Test Value";

        // Act
        var result = Result.Success(value);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Error.Should().BeEmpty();
        result.Value.Should().Be(value);
    }

    [TestMethod]
    public void Failure_WithValue_ShouldCreateFailedResultWithDefaultValue()
    {
        // Arrange
        const string errorMessage = "Something went wrong";

        // Act
        var result = Result.Failure<string>(errorMessage);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(errorMessage);
        result.Value.Should().BeNull();
    }

    [TestMethod]
    public void Constructor_WithSuccessAndError_ShouldThrowInvalidOperationException()
    {
        // Act & Assert
        var action = () => new TestableResult(true, "Error message");
        action.Should().Throw<InvalidOperationException>();
    }

    [TestMethod]
    public void Constructor_WithFailureAndNoError_ShouldThrowInvalidOperationException()
    {
        // Act & Assert
        var action = () => new TestableResult(false, string.Empty);
        action.Should().Throw<InvalidOperationException>();
    }

    [TestMethod]
    public void Success_WithComplexObject_ShouldPreserveObject()
    {
        // Arrange
        var complexObject = new { Id = 1, Name = "Test", Date = DateTime.Now };

        // Act
        var result = Result.Success(complexObject);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(complexObject);
    }

    [TestMethod]
    public void Success_WithGuid_ShouldReturnCorrectGuid()
    {
        // Arrange
        var guid = Guid.NewGuid();

        // Act
        var result = Result.Success(guid);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(guid);
    }

    // Helper class to test protected constructor
    private class TestableResult : Result
    {
        public TestableResult(bool isSuccess, string error) : base(isSuccess, error)
        {
        }
    }
}