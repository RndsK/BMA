using bma.Application.Expenses.Dtos;
using FluentValidation;

namespace bma.Application.Expenses.Validators;

/// <summary>
/// Validator for CreateExpenseRequestDto.
/// </summary>
public class CreateExpenseRequestDtoValidator : AbstractValidator<CreateExpenseRequestDto>
{
    public CreateExpenseRequestDtoValidator()
    {
        RuleFor(x => x.Amount)
            .Cascade(CascadeMode.Stop)
            .GreaterThan(0)
            .WithState(x => new { Key = "Errors.Amount", Message = "Amount Should be greater than 0." });
        RuleFor(x => x.Currency)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithState(x => new { Key = "Errors.Currency", Message = "Currency is required." });
        RuleFor(x => x.ExpenseType)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithState(x => new { Key = "Errors.ExpenseType", Message = "Expense type is required." });
        RuleFor(x => x.ProjectName)
            .Cascade(CascadeMode.Stop)
            .MaximumLength(100)
            .WithState(x => new { Key = "Errors.ProjectName", Message = "Project name must not exceed 100 characters." });
        RuleFor(x => x.Description)
            .Cascade(CascadeMode.Stop)
            .MaximumLength(500)
            .WithState(x => new { Key = "Errors.ExpenseType", Message = "Description must not exceed 500 characters." });
    }
}
