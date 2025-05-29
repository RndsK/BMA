using bma.Domain.Entities.RequestEntities;

namespace bma.Application.Expenses.Dtos;

public static class ExpensesRequestProfile
{
    /// <summary>
    /// Maps a CreateExpenseRequestDto to an ExpensesRequest.
    /// </summary>
    public static ExpensesRequest ToExpensesRequest(CreateExpenseRequestDto dto)
    {
        return new ExpensesRequest
        {
            Amount = dto.Amount,
            Currency = dto.Currency,
            ExpenseType = dto.ExpenseType,
            ProjectName = dto.ProjectName,
            Description = dto.Description,
        };
    }

    /// <summary>
    /// Maps an ExpensesRequest to an ExpenseRequestResponseDto.
    /// </summary>
    public static ExpenseRequestResponseDto ToExpensesRequestResponseDto(ExpensesRequest entity)
    {
        return new ExpenseRequestResponseDto
        {
            Id = entity.Id,
            Status = entity.Status,
            Amount = entity.Amount,
            Currency = entity.Currency,
            ExpenseType = entity.ExpenseType,
            ProjectName = entity.ProjectName,
            Description = entity.Description,
            Attachment = entity.Attachment,
            Email = entity.User.Email!
        };
    }
}
