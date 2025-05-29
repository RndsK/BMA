using bma.Application.Transfer.Finacial.Dtos;
using bma.Application.Transfer.Finacial.Validators;
using FluentAssertions;

namespace bma.UnitTests.ValidationTests;

public class CreateTransferRequestDtoValidatorTests
{
    private readonly CreateFinancialRequestDtoValidator _validator;

    public CreateTransferRequestDtoValidatorTests()
    {
        _validator = new CreateFinancialRequestDtoValidator();
    }

    [Fact]
    public void ValidateTransfer_TypeEmpty_ReturnsTypeError()
    {
        // Arrange
        var dto = new CreateFinancialRequestDto { Type = "" };

        // Act
        var validationResult = _validator.Validate(dto);

        // Assert
        validationResult.Errors.Should().ContainSingle(e => e.PropertyName == nameof(dto.Type));
        var error = validationResult.Errors.First(e => e.PropertyName == nameof(dto.Type));

        var key = error.CustomState?.GetType().GetProperty("Key")?.GetValue(error.CustomState)?.ToString();
        var message = error.CustomState?.GetType().GetProperty("Message")?.GetValue(error.CustomState)?.ToString();

        key.Should().Be("Errors.Type");
        message.Should().Be("Transfer type is required.");
    }

    [Fact]
    public void ValidateTransfer_TypeTooLong_ReturnsTypeError()
    {
        // Arrange
        var dto = new CreateFinancialRequestDto { Type = new string('a', 51) };

        // Act
        var validationResult = _validator.Validate(dto);

        // Assert
        validationResult.Errors.Should().ContainSingle(e => e.PropertyName == nameof(dto.Type));
        var error = validationResult.Errors.First(e => e.PropertyName == nameof(dto.Type));

        var key = error.CustomState?.GetType().GetProperty("Key")?.GetValue(error.CustomState)?.ToString();
        var message = error.CustomState?.GetType().GetProperty("Message")?.GetValue(error.CustomState)?.ToString();

        key.Should().Be("Errors.Type");
        message.Should().Be("Transfer type must not exceed 50 characters.");
    }

    [Fact]
    public void ValidateTransfer_RecurrenceTypeEmpty_ReturnsRecurrenceTypeRequiredError()
    {
        var dto = new CreateFinancialRequestDto { RecurrenceType = "" };

        var validationResult = _validator.Validate(dto);

        var error = validationResult.Errors.FirstOrDefault(e => e.PropertyName == nameof(dto.RecurrenceType));
        error.Should().NotBeNull();
        error!.CustomState.Should().NotBeNull();
        error!.CustomState!.GetType().GetProperty("Key")!.GetValue(error.CustomState)!.ToString().Should().Be("Errors.RecurrenceType");
        error!.CustomState!.GetType().GetProperty("Message")!.GetValue(error.CustomState)!.ToString().Should().Be("Recurrence type is required.");
    }

    [Fact]
    public void ValidateTransfer_RecurrenceTypeTooLong_ReturnsRecurrenceTypeMaxLengthError()
    {
        var dto = new CreateFinancialRequestDto { RecurrenceType = new string('a', 51) };

        var validationResult = _validator.Validate(dto);

        var error = validationResult.Errors.FirstOrDefault(e => e.PropertyName == nameof(dto.RecurrenceType));
        error.Should().NotBeNull();
        error!.CustomState.Should().NotBeNull();
        error!.CustomState!.GetType().GetProperty("Key")!.GetValue(error.CustomState)!.ToString().Should().Be("Errors.RecurrenceType");
        error!.CustomState!.GetType().GetProperty("Message")!.GetValue(error.CustomState)!.ToString().Should().Be("Recurrence type must not exceed 50 characters.");
    }

    [Fact]
    public void ValidateTransfer_CurrencyEmpty_ReturnsCurrencyRequiredError()
    {
        var dto = new CreateFinancialRequestDto { Currency = "" };

        var validationResult = _validator.Validate(dto);

        var error = validationResult.Errors.FirstOrDefault(e => e.PropertyName == nameof(dto.Currency));
        error.Should().NotBeNull();
        error!.CustomState.Should().NotBeNull();
        error!.CustomState!.GetType().GetProperty("Key")!.GetValue(error.CustomState)!.ToString().Should().Be("Errors.Currency");
        error!.CustomState!.GetType().GetProperty("Message")!.GetValue(error.CustomState)!.ToString().Should().Be("Currency is required.");
    }

    [Fact]
    public void ValidateTransfer_AmountInvalid_ReturnsAmountError()
    {
        var dto = new CreateFinancialRequestDto { Amount = 0 };

        var validationResult = _validator.Validate(dto);

        var error = validationResult.Errors.FirstOrDefault(e => e.PropertyName == nameof(dto.Amount));
        error.Should().NotBeNull();
        error!.CustomState.Should().NotBeNull();
        error!.CustomState!.GetType().GetProperty("Key")!.GetValue(error.CustomState)!.ToString().Should().Be("Errors.Amount");
        error!.CustomState!.GetType().GetProperty("Message")!.GetValue(error.CustomState)!.ToString().Should().Be("Amount should be greater than 0.");
    }

    [Fact]
    public void ValidateTransfer_SignOffParticipantInvalidEmail_ReturnsSignOffParticipantsError()
    {
        // Arrange
        var dto = new CreateFinancialRequestDto
        {
            SignOffParticipants = new List<string> { "invalid-email" }
        };

        // Act
        var validationResult = _validator.Validate(dto);

        // Assert
        validationResult.Errors.Should().ContainSingle(e => e.PropertyName == "SignOffParticipants[0]");
        var error = validationResult.Errors.First(e => e.PropertyName == "SignOffParticipants[0]");

        var key = error.CustomState?.GetType().GetProperty("Key")?.GetValue(error.CustomState)?.ToString();
        var message = error.CustomState?.GetType().GetProperty("Message")?.GetValue(error.CustomState)?.ToString();

        key.Should().Be("Errors.SignOffParticipants");
        message.Should().Be("Each sign-off participant must have a valid email address.");
    }
}
