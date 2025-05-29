using bma.Domain.Entities.RequestEntities;

namespace bma.Application.Overtime.Dtos;

public static class OvertimeRequestProfile
{
    /// <summary>
    /// Maps a OvertimeRequest entity to a OvertimeRequestDto.
    /// </summary>
    public static OvertimeRequestResponseDto ToOvertimeRequestResponseDto(this OvertimeRequest entity)
    {
        return new OvertimeRequestResponseDto
        {
            Id = entity.Id,
            Status = entity.Status,
            RequestType = "OvertimeRequest",
            StartDate = entity.StartDate,
            Length = entity.Length,
            Description = entity.Description
        };
    }
    /// <summary>
    /// Maps a CreateOvertimeRequestDto to an OvertimeRequest entity.
    /// </summary>
    public static OvertimeRequest ToOvertimeRequest(this CreateOvertimeRequestDto dto)
    {
        return new OvertimeRequest
        {
            StartDate = dto.StartDate!.Value,
            Length = dto.Length!.Value,
            Description = dto.Description
        };
    }
}