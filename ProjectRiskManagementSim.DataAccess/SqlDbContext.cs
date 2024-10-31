using Microsoft.EntityFrameworkCore;
using ProjectRiskManagementSim.DataAccess.Models;


namespace ProjectRiskManagementSim.DataAccess;
public class OxygenAnalyticsContext : DbContext
{
    public OxygenAnalyticsContext(DbContextOptions<OxygenAnalyticsContext> options) : base(options) { }

    public DbSet<IssueLeadTime> IssueLeadTimes { get; set; }
    public DbSet<Project> Projects { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<IssueLeadTime>().ToTable("IssueLeadTimes", "dbo");
        modelBuilder.Entity<Project>().ToTable("Projects", "dbo");

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
        var issues = await IssueLeadTimes.Where(i => i.Project == projectName).ToListAsync();
        if (issues == null)
        {
            throw new ArgumentNullException(nameof(issues));
        }
        return issues;
    }
}

public class OxygenSimulationContext : DbContext
{
    public OxygenSimulationContext(DbContextOptions<OxygenSimulationContext> options) : base(options) { }

    // Define DbSets for simulation data, e.g.
    // public DbSet<SimulationResult> SimulationResults { get; set; }
}
