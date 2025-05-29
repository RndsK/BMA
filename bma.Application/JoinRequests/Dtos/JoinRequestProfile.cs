using bma.Domain.Entities;

namespace bma.Application.JoinRequests.Dtos;

public static class JoinRequestProfile
{
    public static JoinRequestDto ToJoinRequestDto(JoinRequest joinRequest)
    {
        return new JoinRequestDto
        {
            Id = joinRequest.Id,
            Status = joinRequest.Status,
            AcceptanceDate = joinRequest.AcceptanceDate,
            CompanyId = joinRequest.CompanyId,
            UserId = joinRequest.UserId
        };
    }
}