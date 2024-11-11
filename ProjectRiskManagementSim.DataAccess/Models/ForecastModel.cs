using System.ComponentModel.DataAnnotations;

namespace ProjectRiskManagementSim.DataAccess.Models;
public class ForecastModel
{
    [Key]
    public int Id { get; set; }
    public string Percentage { get; set; } = null!;
    public DateTime EndDate { get; set; }
    public int Days { get; set; }
    public double Cost { get; set; }
    public double DaysDelay { get; set; }
    public double CostOfDelay { get; set; }

    // Foreign key
    public int ProjectSimulationModelId { get; set; }
    public ProjectSimulationModel ProjectSimulationModel { get; set; } = null!;
}
