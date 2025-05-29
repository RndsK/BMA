using bma.Application.Transfer.Finacial.Dtos;
using FluentValidation;

namespace bma.Application.Transfer.Finacial.Validators;

/// <summary>
/// Validator for CreateFinancialRequestDto.
/// </summary>
public class CreateFinancialRequestDtoValidator : AbstractValidator<CreateFinancialRequestDto>
{
    public CreateFinancialRequestDtoValidator()
    {
        RuleFor(x => x.Type)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithState(x => new { Key = "Errors.Type", Message = "Transfer type is required." })
            .MaximumLength(50)
            .WithState(x => new { Key = "Errors.Type", Message = "Transfer type must not exceed 50 characters." });

        RuleFor(x => x.RecurrenceType)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithState(x => new { Key = "Errors.RecurrenceType", Message = "Recurrence type is required." })
            .MaximumLength(50)
            .WithState(x => new { Key = "Errors.RecurrenceType", Message = "Recurrence type must not exceed 50 characters." });

        RuleFor(x => x.Currency)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithState(x => new { Key = "Errors.Currency", Message = "Currency is required." })
            .MaximumLength(10)
            .WithState(x => new { Key = "Errors.Currency", Message = "Currency must not exceed 10 characters." });

        RuleFor(x => x.Amount)
            .Cascade(CascadeMode.Stop)
            .GreaterThan(0)
            .WithState(x => new { Key = "Errors.Amount", Message = "Amount should be greater than 0." });

        RuleFor(x => x.TransferFrom)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithState(x => new { Key = "Errors.TransferFrom", Message = "Source account is required." })
            .MaximumLength(100)
            .WithState(x => new { Key = "Errors.TransferFrom", Message = "Source account must not exceed 100 characters." });

        RuleFor(x => x.TransferTo)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithState(x => new { Key = "Errors.TransferTo", Message = "Destination account is required." })
            .MaximumLength(100)
            .WithState(x => new { Key = "Errors.TransferTo", Message = "Destination account must not exceed 100 characters." });

        RuleFor(x => x.Description)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithState(x => new { Key = "Errors.Purpose", Message = "The description is required." })
            .MaximumLength(500)
            .WithState(x => new { Key = "Errors.Purpose", Message = "The description must not exceed 500 characters." });

        RuleForEach(x => x.SignOffParticipants)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithState(x => new { Key = "Errors.SignOffParticipants", Message = "Each sign-off participant email must be provided." })
            .EmailAddress()
            .WithState(x => new { Key = "Errors.SignOffParticipants", Message = "Each sign-off participant must have a valid email address." });
    }
}
