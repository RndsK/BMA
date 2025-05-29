using bma.Application.Transfer.SignOff.Dtos;
using FluentValidation;

namespace bma.Application.Transfer.SignOff.Validators;

/// <summary>
/// Validator for CreateSignOffRequestDto.
/// </summary>
public class CreateSignOffRequestDtoValidator : AbstractValidator<CreateSignOffRequestDto>
{

    public CreateSignOffRequestDtoValidator()
    {
        RuleFor(x => x.Description)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithState(x => new { Key = "Errors.ReasonEmpty", Message = "The Description is required." })
            .MaximumLength(250)
            .WithState(x => new { Key = "Errors.Reason250", Message = "The Description must not exceed 250 characters." })
            .Matches(@"^[a-zA-Z0-9\s,.!?-]*$")
            .WithState(x => new { Key = "Errors.ReasonInvalid", Message = "Description contains invalid characters." });

        RuleForEach(x => x.SignOffParticipants)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithState(x => new { Key = "Errors.SignOffParticipants", Message = "Each sign-off participant email must be provided." })
                .EmailAddress()
                .WithState(x => new { Key = "Errors.SignOffParticipants", Message = "Each sign-off participant must have a valid email address." });
    }

}