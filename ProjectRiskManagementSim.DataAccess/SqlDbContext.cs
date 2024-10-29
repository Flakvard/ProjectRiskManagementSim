using Microsoft.EntityFrameworkCore;

namespace ProjectRiskManagementSim.DataAccess
{
    public class OxygenAnalyticsContext : DbContext
    {
        public OxygenAnalyticsContext(DbContextOptions<OxygenAnalyticsContext> options) : base(options) { }

        // Define DbSets for project data, e.g.
        public DbSet<ProjectModel> ProjectModels { get; set; }
    }

    public class OxygenSimulationContext : DbContext
    {
        public OxygenSimulationContext(DbContextOptions<OxygenSimulationContext> options) : base(options) { }

        // Define DbSets for simulation data, e.g.
        public DbSet<SimulationResult> SimulationResults { get; set; }
    }
}
