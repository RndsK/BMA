using bma.Application.Companies.Dtos;
using FluentValidation;

namespace bma.Application.Companies.Validators;

public class CreateCompanyDtoValidator : AbstractValidator<CreateCompanyDto>
{
    public CreateCompanyDtoValidator()
    {
        RuleFor(x => x.Name)
          .Cascade(CascadeMode.Stop)
          .NotEmpty().WithState(x => new { Key = "Errors.NameEmpty", Message = "Company name is required." })
          .MaximumLength(100).WithState(x => new { Key = "Errors.Name100", Message = "Name can't be more than 100 characters." });

        RuleFor(x => x.Country)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithState(x => new { Key = "Errors.CountryEmpty", Message = "Country is required." })
            .Length(2, 100).WithState(x => new { Key = "Errors.CountryLength", Message = "Country must be between 2 and 100 characters." });

        RuleFor(x => x.Industry)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithState(x => new { Key = "Errors.IndustryEmpty", Message = "Industry is required." })
            .MaximumLength(50).WithState(x => new { Key = "Errors.Industry50", Message = "Industry can't be more than 50 characters." });

        RuleFor(x => x.Description)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithState(x => new { Key = "Errors.DescriptionEmpty", Message = "Description is required." })
            .MaximumLength(500).WithState(x => new { Key = "Errors.Description500", Message = "Description can't be more than 500 characters." });

        RuleFor(x => x.Logo)
            .Cascade(CascadeMode.Stop)
            .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
            .When(x => !string.IsNullOrWhiteSpace(x.Logo))
            .WithState(x => new { Key = "Errors.LogoInvalid", Message = "Logo must be a valid URL." });
    }
}
