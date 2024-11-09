using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using ProjectRiskManagementSim.DataAccess.Models;

namespace ProjectRiskManagementSim.DataAccess;
public class OxygenAnalyticsContext : DbContext
{
    public OxygenAnalyticsContext(DbContextOptions<OxygenAnalyticsContext> options) : base(options)
    {
        this.ChangeTracker.LazyLoadingEnabled = false;
    }

    public DbSet<IssueLeadTime> IssueLeadTimes { get; set; }
    public DbSet<ProjectJira> Projects { get; set; }
    public DbSet<IssueModel> Issues { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<IssueLeadTime>().ToTable("IssueLeadTimes", "dbo");
        modelBuilder.Entity<ProjectJira>().ToTable("Projects", "dbo");
        modelBuilder.Entity<IssueModel>().ToTable("Issues", "dbo");


        // Optional: Define additional configurations if needed (e.g., primary keys, relationships)
    }
    // Get all projects
    public async Task<List<ProjectJira>> GetProjectsAsync()
    {
        return await Projects.ToListAsync();
    }
    // Get a specific project by Id
    public async Task<ProjectJira> GetProjectByIdAsync(int id)
    {
        return await Projects.FirstOrDefaultAsync(p => p.Id == id);
    }
    // Get all IssueLeadTimes for a specific project
    public async Task<List<IssueLeadTime>> GetIssueLeadTimesByProjectAsync(string projectName)
    {
        if (string.IsNullOrEmpty(projectName))
        {
            throw new ArgumentNullException(nameof(projectName));
        }

        // Step 1: Get the Project ID
        var project = await Projects
            .Where(p => p.Name == projectName)
            .Select(p => p.Id)
            .FirstOrDefaultAsync();

        if (project == default)
        {
            throw new ArgumentException("Project not found", nameof(projectName));
        }

        var issues = await IssueLeadTimes
                            .Include(il => il.Issue)
                            .Where(il => il.Issue.ProjectId == project)
                            .AsNoTracking()
                            .ToListAsync();
        return issues;
    }
    public async Task<IssueModel> GetIssueById(int issueId)
    {
        return await Issues.FirstOrDefaultAsync(issue => issue.Id == issueId);
    }
}

public class OxygenSimulationContext : DbContext
{
    public OxygenSimulationContext(DbContextOptions<OxygenSimulationContext> options) : base(options) { }

    // Define DbSets for simulation data, e.g.
    public DbSet<ProjectModel> ProjectModel { get; set; }
    public DbSet<ProjectSimulationModel> ProjectSimulationModel { get; set; }
    public DbSet<ColumnModel> ColumnModel { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure ProjectModel to ProjectSimulationModel (one-to-many)
        modelBuilder.Entity<ProjectModel>()
                    .HasMany(p => p.ProjectSimulationModels)
                    .WithOne(ps => ps.Project)
                    .HasForeignKey(ps => ps.ProjectId);

        // Configure ProjectSimulationModel to ColumnModel (one-to-many)
        modelBuilder.Entity<ProjectSimulationModel>()
                    .HasMany(ps => ps.Columns)
                    .WithOne(c => c.ProjectSimulationModel)
                    .HasForeignKey(c => c.ProjectSimulationModelId);
    }
    public async Task<List<ProjectModel>> GetProjectsAsync()
    {
        return await ProjectModel.ToListAsync();
    }
    public ProjectModel? GetProjectByIdAsync(string jiraId)
    {
        // Check if the project already exists in the database
        var project = ProjectModel
            .Include(p => p.ProjectSimulationModels) // Ensure related simulations are loaded
            .FirstOrDefault(p => p.JiraId == jiraId);
        return project;
    }
    public async Task<List<ProjectSimulationModel>> GetProjectSimulationsAsync(int projectId)
    {
        return await ProjectSimulationModel
            .Include(p => p.Project)
            .Where(ps => ps.ProjectId == projectId)
            .ToListAsync();
    }
    public async Task<List<ProjectSimulationModel>> GetProjectSimulationsAsync()
    {
        return await ProjectSimulationModel
            .ToListAsync();
    }
    public async Task<ProjectSimulationModel?> GetSimulationByIdAsync(int simulationProjectId)
    {
        return await ProjectSimulationModel
            .Include(p => p.Project)
            .Include(p => p.Columns)
            .FirstOrDefaultAsync(p => p.Id == simulationProjectId);
    }
    public async Task<ProjectModel?> GetProjectBySimulationIdAsync(int simulationProjectId)
    {
        return await ProjectModel
            .Include(p => p.ProjectSimulationModels)
            .FirstOrDefaultAsync(p => p.Id == simulationProjectId);
    }
    public async Task UpdateSimulationAsync(ProjectSimulationModel simulation)
    {
        ProjectSimulationModel.Update(simulation);
        await SaveChangesAsync();
    }
}


public class OxygenSimulationContextFactory : IDesignTimeDbContextFactory<OxygenSimulationContext>
{
    public OxygenSimulationContext CreateDbContext(string[] args)
    {
        IConfigurationRoot configBuilder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(@Directory.GetCurrentDirectory() + "/../ProjectRiskManagementSim.SimulationBlazor/appsettings.development.json")
            .Build();

        var useWindowsAuthSection = configBuilder.GetSection("UseWindowsAuth");
        var useWindowsAuth = bool.Parse(useWindowsAuthSection.Value!);

        var connectionStringSimulation = useWindowsAuth
            ? configBuilder.GetConnectionString("EfCoreSqlDbConnectionWindows")
            : configBuilder.GetConnectionString("EfCoreSqlDbConnection");

        var optionsBuilder = new DbContextOptionsBuilder<OxygenSimulationContext>();
        optionsBuilder.UseSqlServer(connectionStringSimulation);

        return new OxygenSimulationContext(optionsBuilder.Options);
    }
}
