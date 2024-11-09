using System.ComponentModel.DataAnnotations;

namespace ProjectRiskManagementSim.DataAccess.Models;
public class ForecastModel
{
    [Key]
    public int Id { get; set; }
    public string Percentage { get; set; } = null!;
    public string EndDate { get; set; } = null!;
    public int Days { get; set; }
    public string Cost { get; set; } = null!;
    public string CostOfDelay { get; set; } = null!;

    // Foreign key
    public int ProjectSimulationModelId { get; set; }
    public ProjectSimulationModel ProjectSimulationModel { get; set; } = null!;
}
