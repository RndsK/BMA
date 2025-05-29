using bma.Application.Expenses.Dtos;
using bma.Application.Holidays.Dtos;
using bma.Application.Overtime.Dtos;
using bma.Application.Transfer.Finacial.Dtos;
using bma.Application.Transfer.SignOff.Dtos;
using bma.Domain.Entities.RequestEntities;

namespace bma.Application.Requests.Dtos;

public static class RequestProfile
{
    /// <exception cref="ArgumentException"></exception>
    public static RequestDto MapToDto(Request request, string discriminator)
    {
        return request switch
        {
            ExpensesRequest er => new ExpenseRequestResponseDto
            {
                Id = er.Id,
                RequestType = discriminator,
                Status = er.Status,
                UserName = er.User.Name,
                Amount = er.Amount,
                Currency = er.Currency,
                ExpenseType = er.ExpenseType,
                Description = er.Description
            },
            HolidayRequest hr => new HolidayRequestResponseDto
            {
                Id = hr.Id,
                RequestType = discriminator,
                Status = hr.Status,
                UserName = hr.User.Name,
                StartDate = hr.StartDate,
                EndDate = hr.EndDate,
                Description = hr.Description
            },
            OvertimeRequest or => new OvertimeRequestResponseDto
            {
                Id = or.Id,
                RequestType = discriminator,
                Status = or.Status,
                UserName = or.User.Name,
                StartDate = or.StartDate,
                Length = or.Length,
                Description = or.Description
            },
            FinancialRequest tr => new FinancialRequestResponseDto
            {
                Id = tr.Id,
                RequestType = discriminator,
                Status = tr.Status,
                UserName = tr.User.Name,
                Type = tr.Type,
                RecurrenceType = tr.RecurrenceType,
                Currency = tr.Currency,
                Amount = tr.Amount,
                TransferFrom = tr.TransferFrom,
                TransferTo = tr.TransferTo,
                Description = tr.Description,
                SupportingDocument = tr.SupportingDocument,
                SignOffParticipants = tr.SignOffParticipants
            },
            SignOffRequest sr => new SignOffRequestResponseDto
            {
                Id = sr.Id,
                RequestType = discriminator,
                Status = sr.Status,
                UserName = sr.User.Name,
                Description = sr.Description,
                SupportingDocument = sr.SupportingDocument,
                SignOffParticipants = sr.SignOffParticipants
            },
            _ => throw new ArgumentException("Invalid request type")
        };
    }
}