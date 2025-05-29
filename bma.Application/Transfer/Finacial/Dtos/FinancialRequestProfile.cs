using bma.Domain.Entities.RequestEntities;

namespace bma.Application.Transfer.Finacial.Dtos;

public static class FinancialRequestProfile
{
    /// <summary>
    /// Maps a CreateFinancialRequestDto to a FinancialRequest entity.
    /// </summary>
    public static FinancialRequest ToFinancialRequest(CreateFinancialRequestDto dto)
    {
        return new FinancialRequest
        {
            Type = dto.Type,
            RecurrenceType = dto.RecurrenceType,
            Currency = dto.Currency,
            Amount = dto.Amount,
            TransferFrom = dto.TransferFrom,
            TransferTo = dto.TransferTo,
            Description = dto.Description,
            SignOffParticipants = dto.SignOffParticipants
        };
    }

    /// <summary>
    /// Maps a FinancialRequest entity to a FinancialRequestResponseDto.
    /// </summary>
    public static FinancialRequestResponseDto ToFinancialRequestResponseDto(FinancialRequest entity)
    {
        return new FinancialRequestResponseDto
        {
            Id = entity.Id,
            Type = entity.Type,
            RecurrenceType = entity.RecurrenceType,
            RequestType = "FinancialRequest",
            UserName = entity.User.Name,
            Currency = entity.Currency,
            Amount = entity.Amount,
            TransferFrom = entity.TransferFrom,
            TransferTo = entity.TransferTo,
            Description = entity.Description,
            Status = entity.Status,
            SupportingDocument = entity.SupportingDocument,
            SignOffParticipants = entity.SignOffParticipants,
        };
    }
}
