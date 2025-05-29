namespace bma.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    ICompanyRepository Companies { get; }
    IApplicationUserRepository Users { get; }
    IApplicationUserRoleRepository Roles { get; }
    IRequestRepository Requests { get; }
    IJoinRequestRepository JoinRequests { get; }
    IHolidayRequestRepository Holidays { get; }
    IOvertimeRequestRepository OvertimeRequests { get; }
    IExpensesRequestRepository ExpensesRequests { get; }
    ISignOffRequestRepository SignOffRequests { get; }
    IFinancialRequestRepository FinancialRequests { get; }
    IRoleInCompanyRepository RolesInCompanies { get; }
    IApprovalRepository Approvals { get;  }
    Task SaveChangesAsync();
}
