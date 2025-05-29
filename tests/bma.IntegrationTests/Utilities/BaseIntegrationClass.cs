using bma.Domain.Constants;
using bma.Domain.Entities.RequestEntities;
using bma.Domain.Entities;
using bma.Infrastructure.Data;
using bma.IntegrationTests.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

public abstract class BaseIntegrationClass : IClassFixture<CustomWebApplicationFactory<Program>>
{
    protected readonly HttpClient Client;
    private readonly CustomWebApplicationFactory<Program> _factory;

    protected BaseIntegrationClass(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        Client = factory.CreateClient();
        Client.DefaultRequestHeaders.Add("X-Test-User", "TestUserId");
    }

    /// <summary>
    /// Resets the database values to their initial state and seeds test data.
    /// </summary>
    protected void ResetDatabaseAndSeed()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<BmaDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        // Drop and recreate the database
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        // Seed initial data
        SeedDatabase(context, userManager, roleManager);
    }

    /// <summary>
    /// Switches the test user by updating the authentication header.
    /// </summary>
    protected void SwitchTestUser(string testUserId)
    {
        Client.DefaultRequestHeaders.Remove("X-Test-User");
        Client.DefaultRequestHeaders.Add("X-Test-User", testUserId);
    }


    private void SeedDatabase(BmaDbContext context, UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        context.Database.EnsureCreated();

        // Seed roles
        if (!roleManager.RoleExistsAsync("Owner").Result)
        {
            roleManager.CreateAsync(new IdentityRole("Owner")).Wait();
        }

        // Seed first test user
        var user = userManager.FindByIdAsync("TestUserId").Result;
        if (user == null)
        {
            user = new ApplicationUser
            {
                Id = "TestUserId",
                UserName = "TestUser",
                Email = "testuser@example.com",
                Name = "TestOwner"
            };

            userManager.CreateAsync(user, "Password123!").Wait();
            userManager.AddToRoleAsync(user, "Owner").Wait();
        }

        // Seed second test user
        var user2 = userManager.FindByIdAsync("TestUser2Id").Result;
        if (user2 == null)
        {
            user2 = new ApplicationUser
            {
                Id = "TestUser2Id",
                UserName = "TestUser2",
                Email = "testuser2@example.com"
            };

            userManager.CreateAsync(user2, "Password123!").Wait();
        }

        // Seed test companies
        var companies = new List<Company>
        {
            new Company
            {
                Id = 1,
                Name = "Test Company",
                OwnerId = user.Id,
                Owner = user
            },
            new Company
            {
                Id = 2,
                Name = "Test Company2",
                OwnerId = user.Id,
                Owner = user
            }
        };

        foreach (var company in companies)
        {
            if (!context.Companies.Any(c => c.Id == company.Id))
            {
                context.Companies.Add(company);
            }
        }


        // Seed role in company (establishing the user's association with the company)
        var roleInCompany =
            context.RoleInCompany.FirstOrDefault(r => r.Id == 1); // Replace 2 with the specific Id to check
        if (roleInCompany == null)
        {
            context.RoleInCompany.Add(new RoleInCompany
            {
                Id = 1,
                UserId = user.Id,
                CompanyId = companies[0].Id,
                Name = "Owner" // Role of the user in the company
            });
        }

        context.SaveChanges(); // Save


        user = context.Users.Find(user.Id); // Ensure the user is tracked
        if (user == null)
        {
            throw new InvalidOperationException("Test user not seeded properly.");
        }


        // Seed join requests
        if (!context.JoinRequests.Any())
        {
            // Approved join request
            context.JoinRequests.Add(new JoinRequest
            {
                Id = 1,
                Status = StringDefinitions.RequestStatusApproved,
                CompanyId = companies[0].Id,
                Company = companies[0],
                UserId = user.Id,
                Applicant = user,
                AcceptanceDate = DateOnly.FromDateTime(DateTime.UtcNow)
            });

            // Pending join request
            context.JoinRequests.Add(new JoinRequest
            {
                Id = 2,
                Status = StringDefinitions.RequestStatusPending, // Set status to pending
                CompanyId = companies[1].Id,
                Company = companies[1],
                UserId = user.Id, // Associate with a different user
                Applicant = user,
                AcceptanceDate = null // Not accepted yet
            });
        }

        // Seed test expenses
        if (!context.ExpensesRequests.Any())
        {
            context.ExpensesRequests.AddRange(new List<ExpensesRequest>
            {
                new ExpensesRequest
                {
                    Id = 1,
                    Amount = 100,
                    Currency = "CHF",
                    ExpenseType = "Office",
                    ProjectName = "Project Alpha",
                    Description = "Office supplies purchase",
                    Attachment = "https://mockblobstorage.com/receipt1.png",
                    UserId = user.Id,
                    User = user, // Associate the user
                    CompanyId = companies[0].Id,
                    Company = companies[0] // Associate the company
                },
                new ExpensesRequest
                {
                    Id = 2,
                    Amount = 200,
                    Currency = "CHF",
                    ExpenseType = "Travel",
                    ProjectName = "Project Beta",
                    Description = "Travel expenses for client meeting",
                    Attachment = "https://mockblobstorage.com/receipt2.png",
                    UserId = user.Id,
                    User = user, // Associate the user
                    CompanyId = companies[0].Id,
                    Company = companies[0] // Associate the company
                }
            });
        }

        // Seed holiday requests
        if (!context.HolidayRequests.Any())
        {
            context.HolidayRequests.Add(new HolidayRequest
            {
                Id = 3,
                Status = StringDefinitions.RequestStatusApproved,
                CompanyId = companies[0].Id,
                Company = companies[0],
                UserId = user.Id,
                User = user,
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)),
                EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10)),
                Description = "Vacation"
            });
            context.HolidayRequests.Add(new HolidayRequest
            {
                Id = 4,
                Status = StringDefinitions.RequestStatusPending,
                CompanyId = companies[0].Id,
                Company = companies[0],
                UserId = user.Id,
                User = user,
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(15)),
                EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(20)),
                Description = "Vacation"
            });
        }


        // Seed overtime requests
        if (!context.OvertimeRequests.Any())
        {
            context.OvertimeRequests.AddRange(new List<OvertimeRequest>
            {
                new OvertimeRequest
                {
                    Id = 5,
                    Status = StringDefinitions.RequestStatusApproved,
                    CompanyId = companies[0].Id,
                    Company = companies[0],
                    UserId = user.Id,
                    User = user,
                    StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-5)),
                    Length = 3,
                    Description = "Urgent Project"
                },
                new OvertimeRequest
                {
                    Id = 6,
                    Status = StringDefinitions.RequestStatusPending,
                    CompanyId = companies[0].Id,
                    Company = companies[0],
                    UserId = user.Id,
                    User = user,
                    StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
                    Length = 2,
                    Description = "Additional Testing"
                }
            });
        }

        // Seed financial requests
        if (!context.FinancialRequests.Any())
        {
            context.FinancialRequests.AddRange(new List<FinancialRequest>
            {
                new FinancialRequest
                {
                    Id = 7,
                    Type = "Budget",
                    RecurrenceType = "One-Off",
                    Currency = "CHF",
                    Amount = 1000m,
                    TransferFrom = "Account A",
                    TransferTo = "Account B",
                    Description = "Project Funding",
                    CompanyId = companies[0].Id,
                    UserId = user.Id,
                    Status = StringDefinitions.RequestStatusPending
                },
                new FinancialRequest
                {
                    Id = 8,
                    Type = "Investment",
                    RecurrenceType = "Monthly",
                    Currency = "USD",
                    Amount = 5000m,
                    TransferFrom = "Account X",
                    TransferTo = "Account Y",
                    Description = "Long-term Investment",
                    CompanyId = companies[0].Id,
                    UserId = user.Id,
                    Status = StringDefinitions.RequestStatusApproved
                }
            });

        }
        // Seed sign-off requests
        if (!context.SignOffRequests.Any())
        {
            context.SignOffRequests.AddRange(new List<SignOffRequest>
        {
            new SignOffRequest
            {
                Id = 9,
                Description = "Approval for project",
                CompanyId = companies[0].Id,
                UserId = user.Id,
                Status = StringDefinitions.RequestStatusPending
            },
            new SignOffRequest
            {
                Id = 10,
                Description = "Team evaluation sign-off",
                CompanyId = companies[1].Id,
                UserId = user2.Id,
                Status = StringDefinitions.RequestStatusApproved
            }
        });
        }

        context.SaveChanges();
        // Seed approvals
        if (!context.Approvals.Any())
        {
            var requests = context.Requests.Take(3).ToList(); // Get two unique requests
            if (requests.Count >= 3)
            {
                context.Approvals.AddRange(new List<Approval>
                    {
                        new Approval
                        {
                            Id = 1,
                            RequestId = requests[0].Id,
                            Request = requests[0],
                            Status = StringDefinitions.RequestStatusApproved,
                            ApprovedBy = user.Email
                        },
                        new Approval
                        {
                            Id = 2,
                            RequestId = requests[1].Id,
                            Request = requests[1],
                            Status = StringDefinitions.RequestStatusRejected,
                            ApprovedBy = user.Email
                        },
                        new Approval
                        {
                            Id = 3,
                            RequestId = requests[2].Id,
                            Request = requests[2],
                            Status = StringDefinitions.RequestStatusRejected,
                            ApprovedBy = user.Email
                        }
                    });
            }
        }


        context.SaveChanges();
    }
}