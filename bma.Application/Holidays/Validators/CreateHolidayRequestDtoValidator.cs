using bma.Application.Companies.Dtos;
using bma.Application.Holidays.Dtos;
using bma.Domain.Entities.RequestEntities;
using FluentValidation;

namespace bma.Application.Holidays.Validators;

public class CreateHolidayRequestDtoValidator : AbstractValidator<CreateHolidayRequestDto>
{
    public CreateHolidayRequestDtoValidator()
    {
        RuleFor(x => x.StartDate)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithState(x => new { Key = "Errors.StartDateEmpty", Message = "Start date is required." })
            .Must(BeAfterToday)
            .WithState(x => new { Key = "Errors.StartDate", Message = "Start date must be after today." });

        RuleFor(x => x.EndDate)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithState(x => new { Key = "Errors.EndDateEmpty", Message = "End date is required." })
            .Must((request, endDate) => BeAfterOrEqualToStartDate(request.StartDate.Value, endDate.Value))
            .WithState(x => new { Key = "Errors.EndDateAfterStartDate", Message = "End date must be after the start date." });

        RuleFor(x => x.Description)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithState(x => new { Key = "Errors.PurposeEmpty", Message = "The description is required." })
            .MaximumLength(250)
            .WithState(x => new { Key = "Errors.Purpose250", Message = "The description must not exceed 250 characters." })
            .Matches(@"^[a-zA-Z0-9\s,.!?-]*$")
            .WithState(x => new { Key = "Errors.PurposeInvalid", Message = "The description contains invalid characters." });
    }

    /// <summary>
    /// Static function to check if the date is after today.
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    private static bool BeAfterToday(DateOnly? date)
    {
        return date > DateOnly.FromDateTime(DateTime.UtcNow);
    }

    /// <summary>
    /// Static function to check if the selected end date is after the selected start date
    /// </summary>
    /// <param name="startDate"></param>
    /// <param name="endDate"></param>
    /// <returns></returns>
    private static bool BeAfterOrEqualToStartDate(DateOnly startDate, DateOnly endDate)
    {
        return endDate >= startDate;
    }
}
