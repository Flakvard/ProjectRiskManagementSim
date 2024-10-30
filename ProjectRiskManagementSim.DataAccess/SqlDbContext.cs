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
    public async Task<List<Project>> GetProjectsAsync()
    {
        return await Projects.ToListAsync();
    }
}

public class OxygenSimulationContext : DbContext
{
    public OxygenSimulationContext(DbContextOptions<OxygenSimulationContext> options) : base(options) { }

    // Define DbSets for simulation data, e.g.
    // public DbSet<SimulationResult> SimulationResults { get; set; }
}
