using bma.Application.Expenses.Dtos;
using bma.Application.Expenses.Validators;
using FluentAssertions;

namespace bma.UnitTests.ValidationTests;

public class ExpensesValidatorTests
{
    [Fact]
    public void ValidateExpenses_AmountLessThanOrEqualToZero_ReturnsAmountError()
    {
        // Arrange
        var validator = new CreateExpenseRequestDtoValidator();
        var dto = new CreateExpenseRequestDto { Amount = 0 }; // Invalid amount

        // Act
        var validationResult = validator.Validate(dto);

        // Assert
        var error = validationResult.Errors.FirstOrDefault(e => e.PropertyName == nameof(dto.Amount));
        error.Should().NotBeNull("Validation should fail for an amount less than or equal to 0.");
        var customState = error?.CustomState;

        var key = customState?.GetType().GetProperty("Key")?.GetValue(customState)?.ToString();
        var message = customState?.GetType().GetProperty("Message")?.GetValue(customState)?.ToString();

        key.Should().Be("Errors.Amount");
        message.Should().Be("Amount Should be greater than 0.");
    }
    [Fact]
    public void ValidateExpenses_EmptyCurrency_ReturnsCurrencyError()
    {
        // Arrange
        var validator = new CreateExpenseRequestDtoValidator();
        var dto = new CreateExpenseRequestDto { Currency = string.Empty }; // Empty currency

        // Act
        var validationResult = validator.Validate(dto);

        // Assert
        var error = validationResult.Errors.FirstOrDefault(e => e.PropertyName == nameof(dto.Currency));
        error.Should().NotBeNull("Validation should fail for an empty currency.");
        var customState = error?.CustomState;

        var key = customState?.GetType().GetProperty("Key")?.GetValue(customState)?.ToString();
        var message = customState?.GetType().GetProperty("Message")?.GetValue(customState)?.ToString();

        key.Should().Be("Errors.Currency");
        message.Should().Be("Currency is required.");
    }
    [Fact]
    public void ValidateExpenses_EmptyExpenseType_ReturnsExpenseTypeError()
    {
        // Arrange
        var validator = new CreateExpenseRequestDtoValidator();
        var dto = new CreateExpenseRequestDto { ExpenseType = string.Empty }; // Empty expense type

        // Act
        var validationResult = validator.Validate(dto);

        // Assert
        var error = validationResult.Errors.FirstOrDefault(e => e.PropertyName == nameof(dto.ExpenseType));
        error.Should().NotBeNull("Validation should fail for an empty expense type.");
        var customState = error?.CustomState;

        var key = customState?.GetType().GetProperty("Key")?.GetValue(customState)?.ToString();
        var message = customState?.GetType().GetProperty("Message")?.GetValue(customState)?.ToString();

        key.Should().Be("Errors.ExpenseType");
        message.Should().Be("Expense type is required.");
    }
    [Fact]
    public void ValidateExpenses_ProjectNameTooLong_ReturnsProjectNameError()
    {
        // Arrange
        var validator = new CreateExpenseRequestDtoValidator();
        var dto = new CreateExpenseRequestDto { ProjectName = new string('a', 101) }; // Project name exceeds 100 characters

        // Act
        var validationResult = validator.Validate(dto);

        // Assert
        var error = validationResult.Errors.FirstOrDefault(e => e.PropertyName == nameof(dto.ProjectName));
        error.Should().NotBeNull("Validation should fail for a project name longer than 100 characters.");
        var customState = error?.CustomState;

        var key = customState?.GetType().GetProperty("Key")?.GetValue(customState)?.ToString();
        var message = customState?.GetType().GetProperty("Message")?.GetValue(customState)?.ToString();

        key.Should().Be("Errors.ProjectName");
        message.Should().Be("Project name must not exceed 100 characters.");
    }
    [Fact]
    public void ValidateExpenses_CommentsTooLong_ReturnsCommentsError()
    {
        // Arrange
        var validator = new CreateExpenseRequestDtoValidator();
        var dto = new CreateExpenseRequestDto { Description = new string('a', 501) }; // Comments exceed 500 characters

        // Act
        var validationResult = validator.Validate(dto);

        // Assert
        var error = validationResult.Errors.FirstOrDefault(e => e.PropertyName == nameof(dto.Description));
        error.Should().NotBeNull("Validation should fail for comments longer than 500 characters.");
        var customState = error?.CustomState;

        var key = customState?.GetType().GetProperty("Key")?.GetValue(customState)?.ToString();
        var message = customState?.GetType().GetProperty("Message")?.GetValue(customState)?.ToString();

        key.Should().Be("Errors.ExpenseType"); // Note: Reused key for comments error
        message.Should().Be("Description must not exceed 500 characters.");
    }

}