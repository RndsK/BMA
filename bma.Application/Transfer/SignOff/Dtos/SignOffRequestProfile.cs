using bma.Domain.Entities.RequestEntities;

namespace bma.Application.Transfer.SignOff.Dtos;
public static class SignOffRequestProfile
{
    /// <summary>
    /// Maps a CreateOvertimeRequestDto to an OvertimeRequest entity.
    /// </summary>
    public static SignOffRequest ToSignOffRequest(CreateSignOffRequestDto dto)
    {
        return new SignOffRequest
        {
            SignOffParticipants = dto.SignOffParticipants,
            Description = dto.Description
        };
    }
    /// <summary>
    /// Maps a OvertimeRequest entity to a OvertimeRequestDto.
    /// </summary>
    public static SignOffRequestResponseDto ToSignOffRequestResponseDto(SignOffRequest entity)
    {
        return new SignOffRequestResponseDto
        {
            Id = entity.Id,
            Status = entity.Status,
            RequestType = "SignOffRequest",
            UserName = entity.User.Name,
            SupportingDocument = entity.SupportingDocument,
            SignOffParticipants = entity.SignOffParticipants,
            Description = entity.Description
        };
    }
}