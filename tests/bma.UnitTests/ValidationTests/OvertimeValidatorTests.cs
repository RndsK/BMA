using bma.Application.Overtime.Dtos;
using bma.Application.Overtime.Validators;
using FluentAssertions;

namespace bma.UnitTests.ValidationTests;

public class CreateOvertimeRequestDtoValidatorTests
{
    private readonly CreateOvertimeRequestDtoValidator _validator;

    public CreateOvertimeRequestDtoValidatorTests()
    {
        _validator = new CreateOvertimeRequestDtoValidator();
    }

    [Fact]
    public void ValidateOvertime_StartDateEmpty_ReturnsStartDateEmptyError()
    {
        // Arrange
        var dto = new CreateOvertimeRequestDto { StartDate = null };

        // Act
        var validationResult = _validator.Validate(dto);

        // Assert
        var error = validationResult.Errors.FirstOrDefault(e => e.PropertyName == nameof(dto.StartDate));
        error.Should().NotBeNull("Validation should fail when StartDate is empty.");
        var customState = error?.CustomState;

        var key = customState?.GetType().GetProperty("Key")?.GetValue(customState)?.ToString();
        var message = customState?.GetType().GetProperty("Message")?.GetValue(customState)?.ToString();

        key.Should().Be("Errors.StartDateEmpty");
        message.Should().Be("Start date is required.");
    }

    [Fact]
    public void ValidateOvertime_LengthOutOfRange_ReturnsOvertimeLengthInvalidError()
    {
        // Arrange
        var dto = new CreateOvertimeRequestDto { Length = 10 }; // Out of range

        // Act
        var validationResult = _validator.Validate(dto);

        // Assert
        var error = validationResult.Errors.FirstOrDefault(e => e.PropertyName == nameof(dto.Length));
        error.Should().NotBeNull("Validation should fail when Length is out of range.");
        var customState = error?.CustomState;

        var key = customState?.GetType().GetProperty("Key")?.GetValue(customState)?.ToString();
        var message = customState?.GetType().GetProperty("Message")?.GetValue(customState)?.ToString();

        key.Should().Be("Errors.OvertimeLengthInvalid");
        message.Should().Be("Overtime length can only be between 1 and 7.");
    }

    [Fact]
    public void ValidateOvertime_ReasonEmpty_ReturnsReasonEmptyError()
    {
        // Arrange
        var dto = new CreateOvertimeRequestDto { Description = string.Empty };

        // Act
        var validationResult = _validator.Validate(dto);

        // Assert
        var error = validationResult.Errors.FirstOrDefault(e => e.PropertyName == nameof(dto.Description));
        error.Should().NotBeNull("Validation should fail when description is empty.");
        var customState = error?.CustomState;

        var key = customState?.GetType().GetProperty("Key")?.GetValue(customState)?.ToString();
        var message = customState?.GetType().GetProperty("Message")?.GetValue(customState)?.ToString();

        key.Should().Be("Errors.ReasonEmpty");
        message.Should().Be("The description is required.");
    }

    [Fact]
    public void ValidateOvertime_ReasonTooLong_ReturnsReason250Error()
    {
        // Arrange
        var dto = new CreateOvertimeRequestDto { Description = new string('a', 251) }; // Exceeds max length

        // Act
        var validationResult = _validator.Validate(dto);

        // Assert
        var error = validationResult.Errors.FirstOrDefault(e => e.PropertyName == nameof(dto.Description));
        error.Should().NotBeNull("Validation should fail when description exceeds 250 characters.");
        var customState = error?.CustomState;

        var key = customState?.GetType().GetProperty("Key")?.GetValue(customState)?.ToString();
        var message = customState?.GetType().GetProperty("Message")?.GetValue(customState)?.ToString();

        key.Should().Be("Errors.Reason250");
        message.Should().Be("The description must not exceed 250 characters.");
    }

    [Fact]
    public void ValidateOvertime_ReasonInvalidCharacters_ReturnsReasonInvalidError()
    {
        // Arrange
        var dto = new CreateOvertimeRequestDto { Description = "Invalid#Characters!" }; // Contains invalid characters

        // Act
        var validationResult = _validator.Validate(dto);

        // Assert
        var error = validationResult.Errors.FirstOrDefault(e => e.PropertyName == nameof(dto.Description));
        error.Should().NotBeNull("Validation should fail when description contains invalid characters.");
        var customState = error?.CustomState;

        var key = customState?.GetType().GetProperty("Key")?.GetValue(customState)?.ToString();
        var message = customState?.GetType().GetProperty("Message")?.GetValue(customState)?.ToString();

        key.Should().Be("Errors.ReasonInvalid");
        message.Should().Be("The description contains invalid characters.");
    }
}
