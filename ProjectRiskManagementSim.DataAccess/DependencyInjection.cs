using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectRiskManagementSim.DataAccess;
    public class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Register EF Core DbContext
            services.AddDbContext<SqlDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("EfCoreSqlDbConnection"),
                sqlServerOptionsAction: sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure();
                }));

            // Ensure database migration
            using (var serviceScope = services.BuildServiceProvider().CreateScope())
            {
                // Ensure database migration
                var dbContext = serviceScope.ServiceProvider.GetRequiredService<SqlDbContext>();
                dbContext.Database.Migrate();
            }

            // Core layer services
            services.TryAddScoped<IValidator<OxygenAssociate>, EmployeeValidator>();
            services.AddScoped<IValidator<Team>, TeamValidator>();

            //  Infrastructure layer services
            services.TryAddScoped<IUnitOfWork, UnitOfWork>();
            services.TryAddScoped<IOxygenAssociateRepository, OxygenAssociateRepository>();
            services.TryAddScoped<ITeamRepository, TeamRepository>();
            services.TryAddScoped<IIssuePriorityRepository, IssuePriorityRepository>();
            services.TryAddScoped<IIssueTypeRepository, IssueTypeRepository>();
            services.TryAddScoped<IIssueStatusRepository, IssueStatusRepository>();
            services.TryAddScoped<IIssueStatusRepository, IssueStatusRepository>();
            services.TryAddScoped<IIssueLeadTimeRepository, IssueLeadTimeRepository>();
            services.TryAddScoped<IPlannerRepository, PlannerRepository>();
            return services;
        }

    }