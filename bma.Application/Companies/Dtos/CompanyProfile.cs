using bma.Domain.Entities;

namespace bma.Application.Companies.Dtos;

public static class CompanyProfile
{
    /// <summary>
    /// Maps the company entity to the dto
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public static CompanyDto ToCompanyDto(Company entity)
    {
        return new CompanyDto
        {
            Id = entity.Id,
            Name = entity.Name,
            OwnerName = entity.Owner.Name,
            Description = entity.Description,
            Country = entity.Country,
            Industry = entity.Industry,
            Logo = entity.Logo

        };
    }

    /// <summary>
    /// Maps the company dto to the company entity
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    public static Company ToCompany(CompanyDto dto)
    {
        return new Company
        {
            Name = dto.Name,
            Description = dto.Description,
            Country = dto.Country,
            Industry = dto.Industry,
            Logo = dto.Logo
        };
    }

    /// <summary>
    /// Maps the createcompanyDto to the company entity
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    public static Company ToCompany(CreateCompanyDto dto)
    {
        return new Company
        {
            Name = dto.Name,
            Description = dto.Description,
            Country = dto.Country,
            Industry = dto.Industry,
            Logo = dto.Logo
        };
    }
}
