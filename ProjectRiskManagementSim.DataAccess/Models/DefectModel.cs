using System.ComponentModel.DataAnnotations;

namespace ProjectRiskManagementSim.DataAccess.Models;

public class DefectModel
{
    [Key]
    public int Id { get; set; }
    public string? Name { get; set; }

    public double DefectPercentage { get; set; } = 0;
    public double DefectsPercentageLowBound { get; set; } = 0;
    public double DefectsPercentageHighBound { get; set; } = 0;
    // Foreign key
    public int ProjectSimulationModelId { get; set; }
    public ProjectSimulationModel ProjectSimulationModel { get; set; } = null!;
}
