using bma.Application.Holidays.Dtos;
using bma.Application.Overtime.Dtos;
using FluentValidation;

namespace bma.Application.Overtime.Validators;

public class CreateOvertimeRequestDtoValidator : AbstractValidator<CreateOvertimeRequestDto>
{

    public CreateOvertimeRequestDtoValidator()
    {
        RuleFor(x => x.StartDate)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithState(x => new { Key = "Errors.StartDateEmpty", Message = "Start date is required." });

        RuleFor(x => x.Length)
            .Cascade(CascadeMode.Stop)
            .InclusiveBetween(1, 7)
            .WithState(x => new { Key = "Errors.OvertimeLengthInvalid", Message = "Overtime length can only be between 1 and 7." });

        RuleFor(x => x.Description)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithState(x => new { Key = "Errors.ReasonEmpty", Message = "The description is required." })
            .MaximumLength(250)
            .WithState(x => new { Key = "Errors.Reason250", Message = "The description must not exceed 250 characters." })
            .Matches(@"^[a-zA-Z0-9\s,.!?-]*$")
            .WithState(x => new { Key = "Errors.ReasonInvalid", Message = "The description contains invalid characters." });
    }

}