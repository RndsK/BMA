using bma.Domain.Entities.RequestEntities;

namespace bma.Application.Holidays.Dtos;

public static class HolidayRequestProfile
{
    /// <summary>
    /// Maps a HolidayRequest entity to a HolidayRequestDto.
    /// </summary>
    public static HolidayRequestResponseDto ToHolidayRequestResponseDto(this HolidayRequest entity)
    {
        return new HolidayRequestResponseDto
        {
            Id = entity.Id,
            StartDate = entity.StartDate,
            RequestType = "HolidayRequest",
            UserName = entity.User.Name,
            EndDate = entity.EndDate,
            Description = entity.Description,
            Status = entity.Status
        };
    }

    /// <summary>
    /// Maps a CreateHolidayRequestDto to a HolidayRequest entity.
    /// </summary>
    public static HolidayRequest ToHolidayRequest(this CreateHolidayRequestDto dto)
    {
        return new HolidayRequest
        {
            StartDate = dto.StartDate!.Value,
            EndDate = dto.EndDate!.Value,
            Description = dto.Description!
        };
    }
}