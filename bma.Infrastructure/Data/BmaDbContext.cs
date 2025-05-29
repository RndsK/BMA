using bma.Domain.Entities;
using bma.Domain.Entities.RequestEntities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace bma.Infrastructure.Data;

/// <summary>
/// Database context for the application with Identity integration.
/// </summary>
public class BmaDbContext : IdentityDbContext<ApplicationUser>
{
    public BmaDbContext(DbContextOptions<BmaDbContext> options) : base(options)
    {
    }

    public DbSet<Company> Companies { get; set; }
    public DbSet<RoleInCompany> RoleInCompany { get; set; }
    public DbSet<JoinRequest> JoinRequests { get; set; }
    public DbSet<Request> Requests { get; set; }
    public DbSet<Approval> Approvals { get; set; }
    public DbSet<ExpensesRequest> ExpensesRequests { get; set; }
    public DbSet<OvertimeRequest> OvertimeRequests { get; set; }
    public DbSet<HolidayRequest> HolidayRequests { get; set; }
    public DbSet<FinancialRequest> FinancialRequests { get; set; }
    public DbSet<SignOffRequest> SignOffRequests { get; set; }
    public DbSet<TransferRequestSignOffParticipant> TransferRequestSignOffParticipants { get; set; }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Company>(entity =>
        {
            entity.HasKey(c => c.Id);

            entity.Property(c => c.Name);

            entity.HasOne(c => c.Owner)
                  .WithMany()
                  .HasForeignKey(c => c.OwnerId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
        builder.Entity<Approval>()
            .HasOne(a => a.Request)
            .WithMany()
            .HasForeignKey(a => a.RequestId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Approval>()
            .HasOne(a => a.JoinRequest)
            .WithMany()
            .HasForeignKey(a => a.JoinRequestId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<RoleInCompany>()
            .HasKey(uc => uc.Id);

        builder.Entity<RoleInCompany>()
            .HasOne(uc => uc.User)
            .WithMany() // No navigation property in ApplicationUser
            .HasForeignKey(uc => uc.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<RoleInCompany>()
            .HasOne(uc => uc.Company)
            .WithMany() // No navigation property in Company
            .HasForeignKey(uc => uc.CompanyId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<TransferRequestSignOffParticipant>()
           .HasKey(uc => uc.Id);

        builder.Entity<TransferRequestSignOffParticipant>()
            .HasOne(uc => uc.User)
            .WithMany() // No navigation property in ApplicationUser
            .HasForeignKey(uc => uc.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<TransferRequestSignOffParticipant>()
            .HasOne(p => p.Request)
            .WithMany()
            .HasForeignKey(p => p.RequestId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<JoinRequest>(entity =>
        {
            entity.HasKey(c => c.Id);

            entity.Property(c => c.Status);

            entity.HasOne(c => c.Applicant)
                  .WithMany()
                  .HasForeignKey(c => c.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(c => c.Company)
                  .WithMany()
                  .HasForeignKey(c => c.CompanyId)
                  .OnDelete(DeleteBehavior.NoAction);
        });

        builder.Entity<Request>(entity =>
        {
            entity.HasKey(c => c.Id);

            entity.Property(c => c.Status);

            entity.HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(c => c.Company)
                .WithMany()
                .HasForeignKey(c => c.CompanyId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        builder.Entity<ExpensesRequest>().HasBaseType<Request>();
        builder.Entity<OvertimeRequest>().HasBaseType<Request>();
        builder.Entity<HolidayRequest>().HasBaseType<Request>();
        builder.Entity<FinancialRequest>().HasBaseType<Request>();
        builder.Entity<SignOffRequest>().HasBaseType<Request>();
    }
}
