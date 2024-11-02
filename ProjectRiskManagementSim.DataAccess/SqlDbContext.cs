using Microsoft.EntityFrameworkCore;
using ProjectRiskManagementSim.DataAccess.Models;


namespace ProjectRiskManagementSim.DataAccess;
public class OxygenAnalyticsContext : DbContext
{
    public OxygenAnalyticsContext(DbContextOptions<OxygenAnalyticsContext> options) : base(options) {
        this.ChangeTracker.LazyLoadingEnabled = false;
    }

    public DbSet<IssueLeadTime> IssueLeadTimes { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<IssueModel> Issues { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<IssueLeadTime>().ToTable("IssueLeadTimes", "dbo");
        modelBuilder.Entity<Project>().ToTable("Projects", "dbo");
        modelBuilder.Entity<IssueModel>().ToTable("Issues", "dbo");


        // Optional: Define additional configurations if needed (e.g., primary keys, relationships)
    }
    // Get all projects
    public async Task<List<Project>> GetProjectsAsync()
    {
        return await Projects.ToListAsync();
    }
    // Get a specific project by Id
    public async Task<Project> GetProjectByIdAsync(int id)
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
    // public DbSet<SimulationResult> SimulationResults { get; set; }
}
