using System.ComponentModel.DataAnnotations;

namespace ProjectRiskManagementSim.DataAccess.Models;
public class StaffAnalysisModel
{
    [Key]
    public int Id { get; set; }
    public int Priority { get; set; }
    public string StaffName { get; set; } = null!;
    public double Days { get; set; }
    public string EndDate { get; set; } = null!;

    // Foreign key
    public int ProjectSimulationModelId { get; set; }
    public ProjectSimulationModel ProjectSimulationModel { get; set; } = null!;
}
