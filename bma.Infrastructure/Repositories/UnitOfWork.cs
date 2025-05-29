using bma.Domain.Entities;
using bma.Domain.Interfaces;
using bma.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;

namespace bma.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly BmaDbContext _context;

    public UnitOfWork(BmaDbContext context,
        UserManager<ApplicationUser> userManager,
        HttpClient httpClient)
    {
        _context = context;
        Users = new ApplicationUserRepository(_context);
        Roles = new ApplicationUserRoleRepository(_context, userManager);
        Requests = new RequestRepository(_context);
        JoinRequests = new JoinRequestRepository(_context);
        ExpensesRequests = new ExpensesRequestRepository(_context);
        OvertimeRequests = new OvertimeRequestRepository(_context);
        FinancialRequests = new FinancialRequestRepository(_context);
        SignOffRequests = new SignOffRequestRepository(_context);
        Holidays = new HolidayRequestRepository(_context, httpClient);
        RolesInCompanies = new RoleInCompanyRepository(_context);
        Companies = new CompanyRepository(_context, Users, Roles, RolesInCompanies);
        Approvals = new ApprovalRepository(_context);
    }

    public ICompanyRepository Companies { get; }
    public IApplicationUserRepository Users { get; }
    public IApplicationUserRoleRepository Roles { get; }
    public IRequestRepository Requests { get; }
    public IJoinRequestRepository JoinRequests { get; }
    public IExpensesRequestRepository ExpensesRequests { get; }
    public IOvertimeRequestRepository OvertimeRequests { get; }
    public IHolidayRequestRepository Holidays { get; }
    public IFinancialRequestRepository FinancialRequests { get; }
    public ISignOffRequestRepository SignOffRequests {  get; }
    public IRoleInCompanyRepository RolesInCompanies { get; }
    public IApprovalRepository Approvals { get; }

    public async Task SaveChangesAsync() => await _context.SaveChangesAsync();

    public void Dispose() => _context.Dispose();
}
