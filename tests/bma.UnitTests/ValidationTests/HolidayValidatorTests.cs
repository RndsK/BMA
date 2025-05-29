using bma.Application.Holidays.Dtos;
using bma.Application.Holidays.Validators;
using FluentAssertions;

namespace bma.UnitTests.ValidationTests;

public class CreateHolidayRequestDtoValidatorTests
{
    private readonly CreateHolidayRequestDtoValidator _validator;

    public CreateHolidayRequestDtoValidatorTests()
    {
        _validator = new CreateHolidayRequestDtoValidator();
    }

    [Fact]
    public void ValidateHoliday_StartDateEmpty_ReturnsStartDateEmptyError()
    {
        // Arrange
        var dto = new CreateHolidayRequestDto { StartDate = null };

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
    public void ValidateHoliday_StartDateNotAfterToday_ReturnsStartDateError()
    {
        // Arrange
        var dto = new CreateHolidayRequestDto { StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)) };

        // Act
        var validationResult = _validator.Validate(dto);

        // Assert
        var error = validationResult.Errors.FirstOrDefault(e => e.PropertyName == nameof(dto.StartDate));
        error.Should().NotBeNull("Validation should fail when StartDate is not after today.");
        var customState = error?.CustomState;

        var key = customState?.GetType().GetProperty("Key")?.GetValue(customState)?.ToString();
        var message = customState?.GetType().GetProperty("Message")?.GetValue(customState)?.ToString();

        key.Should().Be("Errors.StartDate");
        message.Should().Be("Start date must be after today.");
    }

    [Fact]
    public void ValidateHoliday_EndDateEmpty_ReturnsEndDateEmptyError()
    {
        // Arrange
        var dto = new CreateHolidayRequestDto
        {
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
            EndDate = null
        };

        // Act
        var validationResult = _validator.Validate(dto);

        // Assert
        var error = validationResult.Errors.FirstOrDefault(e => e.PropertyName == nameof(dto.EndDate));
        error.Should().NotBeNull("Validation should fail when EndDate is empty.");
        var customState = error?.CustomState;

        var key = customState?.GetType().GetProperty("Key")?.GetValue(customState)?.ToString();
        var message = customState?.GetType().GetProperty("Message")?.GetValue(customState)?.ToString();

        key.Should().Be("Errors.EndDateEmpty");
        message.Should().Be("End date is required.");
    }

    [Fact]
    public void ValidateHoliday_EndDateBeforeStartDate_ReturnsEndDateAfterStartDateError()
    {
        // Arrange
        var dto = new CreateHolidayRequestDto
        {
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
            EndDate = DateOnly.FromDateTime(DateTime.UtcNow)
        };

        // Act
        var validationResult = _validator.Validate(dto);

        // Assert
        var error = validationResult.Errors.FirstOrDefault(e => e.PropertyName == nameof(dto.EndDate));
        error.Should().NotBeNull("Validation should fail when EndDate is before StartDate.");
        var customState = error?.CustomState;

        var key = customState?.GetType().GetProperty("Key")?.GetValue(customState)?.ToString();
        var message = customState?.GetType().GetProperty("Message")?.GetValue(customState)?.ToString();

        key.Should().Be("Errors.EndDateAfterStartDate");
        message.Should().Be("End date must be after the start date.");
    }

    [Fact]
    public void ValidateHoliday_PurposeEmpty_ReturnsPurposeEmptyError()
    {
        // Arrange
        var dto = new CreateHolidayRequestDto { Description = string.Empty };

        // Act
        var validationResult = _validator.Validate(dto);

        // Assert
        var error = validationResult.Errors.FirstOrDefault(e => e.PropertyName == nameof(dto.Description));
        error.Should().NotBeNull("Validation should fail when Purpose is empty.");
        var customState = error?.CustomState;

        var key = customState?.GetType().GetProperty("Key")?.GetValue(customState)?.ToString();
        var message = customState?.GetType().GetProperty("Message")?.GetValue(customState)?.ToString();

        key.Should().Be("Errors.PurposeEmpty");
        message.Should().Be("The description is required.");
    }

    [Fact]
    public void ValidateHoliday_PurposeTooLong_ReturnsPurpose250Error()
    {
        // Arrange
        var dto = new CreateHolidayRequestDto { Description = new string('a', 251) };

        // Act
        var validationResult = _validator.Validate(dto);

        // Assert
        var error = validationResult.Errors.FirstOrDefault(e => e.PropertyName == nameof(dto.Description));
        error.Should().NotBeNull("Validation should fail when Purpose exceeds 250 characters.");
        var customState = error?.CustomState;

        var key = customState?.GetType().GetProperty("Key")?.GetValue(customState)?.ToString();
        var message = customState?.GetType().GetProperty("Message")?.GetValue(customState)?.ToString();

        key.Should().Be("Errors.Purpose250");
        message.Should().Be("The description must not exceed 250 characters.");
    }

    [Fact]
    public void ValidateHoliday_PurposeInvalidCharacters_ReturnsPurposeInvalidError()
    {
        // Arrange
        var dto = new CreateHolidayRequestDto { Description = "Invalid#Characters!" };

        // Act
        var validationResult = _validator.Validate(dto);

        // Assert
        var error = validationResult.Errors.FirstOrDefault(e => e.PropertyName == nameof(dto.Description));
        error.Should().NotBeNull("Validation should fail when Purpose contains invalid characters.");
        var customState = error?.CustomState;

        var key = customState?.GetType().GetProperty("Key")?.GetValue(customState)?.ToString();
        var message = customState?.GetType().GetProperty("Message")?.GetValue(customState)?.ToString();

        key.Should().Be("Errors.PurposeInvalid");
        message.Should().Be("The description contains invalid characters.");
    }
}
