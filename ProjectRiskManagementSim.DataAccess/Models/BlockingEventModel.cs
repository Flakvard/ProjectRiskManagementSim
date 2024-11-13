using System.ComponentModel.DataAnnotations;

namespace ProjectRiskManagementSim.DataAccess.Models;

public class BlockingEventModel
{
    [Key]
    public int Id { get; set; }
    public string? Name { get; set; }

    public double BlockingEventPercentage { get; set; } = 0;
    public double BlockingEventsPercentageLowBound { get; set; } = 0;
    public double BlockingEventsPercentageHighBound { get; set; } = 0;
    // Foreign key
    public int ProjectSimulationModelId { get; set; }
    public ProjectSimulationModel ProjectSimulationModel { get; set; } = null!;
}
