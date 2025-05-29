using bma.Application.Companies.Dtos;
using bma.Application.Companies.Validators;
using FluentAssertions;
using FluentValidation.TestHelper;

namespace bma.UnitTests.ValidationTests;

public class CreateCompanyDtoValidatorTests
{
    private readonly CreateCompanyDtoValidator _validator;

    public CreateCompanyDtoValidatorTests()
    {
        _validator = new CreateCompanyDtoValidator();
    }

    [Fact]
    public void ValidateCompany_EmptyName_ReturnsNameEmptyError()
    {
        // Arrange
        var validator = new CreateCompanyDtoValidator();
        var dto = new CreateCompanyDto { Name = string.Empty }; // Empty name

        // Act
        var validationResult = validator.Validate(dto);

        // Assert
        var error = validationResult.Errors.FirstOrDefault(e => e.PropertyName == nameof(dto.Name));
        error.Should().NotBeNull("Validation should fail for an empty name.");
        var customState = error?.CustomState;

        var key = customState?.GetType().GetProperty("Key")?.GetValue(customState)?.ToString();
        var message = customState?.GetType().GetProperty("Message")?.GetValue(customState)?.ToString();

        key.Should().Be("Errors.NameEmpty");
        message.Should().Be("Company name is required.");
    }


    [Fact]
    public void ValidateCompany_TooLongName_ReturnsName100Error()
    {
        // Arrange
        var validator = new CreateCompanyDtoValidator();
        var dto = new CreateCompanyDto { Name = new string('a', 101) }; // 101 characters

        // Act
        var validationResult = validator.Validate(dto);

        // Assert
        var error = validationResult.Errors.FirstOrDefault(e => e.PropertyName == nameof(dto.Name));
        error.Should().NotBeNull("Validation should fail for a name longer than 100 characters.");
        var customState = error?.CustomState;

        var key = customState?.GetType().GetProperty("Key")?.GetValue(customState)?.ToString();
        var message = customState?.GetType().GetProperty("Message")?.GetValue(customState)?.ToString();

        key.Should().Be("Errors.Name100");
        message.Should().Be("Name can't be more than 100 characters.");
    }

    [Fact]
    public void ValidateCompany_ValidName_NoErrors()
    {
        // Arrange
        var dto = new CreateCompanyDto { Name = "Valid Company Name" };

        // Act
        var validationResult = _validator.TestValidate(dto);

        // Assert
        validationResult.ShouldNotHaveValidationErrorFor(x => x.Name);
    }
}