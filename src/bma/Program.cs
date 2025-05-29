using bma.Application.Companies.Dtos;
using bma.Application.Companies.Validators;
using bma.Application.Expenses.Dtos;
using bma.Application.Expenses.Validators;
using bma.Application.Holidays.Dtos;
using bma.Application.Holidays.Validators;
using bma.Application.Overtime.Dtos;
using bma.Application.Overtime.Validators;
using bma.Application.Transfer.Finacial.Dtos;
using bma.Application.Transfer.Finacial.Validators;
using bma.Domain.Entities;
using bma.Domain.Interfaces;
using bma.Infrastructure.Configuration;
using bma.Infrastructure.Data;
using bma.Infrastructure.Repositories;
using bma.Infrastructure.Seeders;
using bma.Infrastructure.Storage;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsEnvironment("Testing"))
{
    // Use In-Memory database for testing
    builder.Services.AddDbContext<BmaDbContext>(options =>
        options.UseInMemoryDatabase("TestDb"));
}
else
{
    // Use SQL Server for non-testing environments
    builder.Services.AddDbContext<BmaDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("BmaDb")));
}

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
builder.Services.AddScoped<IApplicationUserRepository, ApplicationUserRepository>();
builder.Services.AddScoped<IApplicationUserRoleRepository, ApplicationUserRoleRepository>();
builder.Services.AddScoped<IExpensesRequestRepository, ExpensesRequestRepository>();
builder.Services.AddScoped<IJoinRequestRepository, JoinRequestRepository>();
builder.Services.AddScoped<IHolidayRequestRepository, HolidayRequestRepository>();
builder.Services.AddScoped<IOvertimeRequestRepository, OvertimeRequestRepository>();
builder.Services.AddScoped<IFinancialRequestRepository, FinancialRequestRepository>();
builder.Services.AddScoped<IJoinRequestRepository, JoinRequestRepository>();
builder.Services.AddScoped<IRoleInCompanyRepository, RoleInCompanyRepository>();
builder.Services.AddScoped<IApprovalRepository, ApprovalRepository>();

builder.Services.Configure<BlobStorageSettings>(builder.Configuration.GetSection("BlobStorage"));
builder.Services.AddScoped<IBlobStorageService, BlobStorageService>();

//Dependency injection for validation services
builder.Services.AddScoped<IValidator<CreateCompanyDto>, CreateCompanyDtoValidator>();
builder.Services.AddScoped<IValidator<CreateExpenseRequestDto>, CreateExpenseRequestDtoValidator>();
builder.Services.AddScoped<IValidator<CreateOvertimeRequestDto>, CreateOvertimeRequestDtoValidator>();
builder.Services.AddScoped<IValidator<CreateFinancialRequestDto>, CreateFinancialRequestDtoValidator>();
builder.Services.AddScoped<IValidator<CreateHolidayRequestDto>, CreateHolidayRequestDtoValidator>();


builder.Services.AddIdentityApiEndpoints<ApplicationUser>()
        .AddRoles<IdentityRole>()
        .AddEntityFrameworkStores<BmaDbContext>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policyBuilder => policyBuilder
        .WithOrigins(
            "https://localhost:5173",
            "https://localhost:5174"
        )
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials());
});

var app = builder.Build();

if (!builder.Environment.IsEnvironment("Testing"))
{
    using var scope = app.Services.CreateScope();
    var serviceProvider = scope.ServiceProvider;

    await RoleSeeder.SeedRolesAsync(serviceProvider);

    var dbContext = serviceProvider.GetRequiredService<BmaDbContext>();
    if (dbContext.Database.GetPendingMigrations().Any())
    {
        await dbContext.Database.MigrateAsync();
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowReactApp");

app.UseAuthentication();

app.MapGroup("api/account")
        .WithTags("Account")
        .MapIdentityApi<ApplicationUser>();
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }