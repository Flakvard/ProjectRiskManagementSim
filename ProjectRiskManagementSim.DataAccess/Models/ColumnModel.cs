using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectRiskManagementSim.DataAccess.Models;
public class ColumnModel
{
    [Key]
    public int Id { get; set; }
    public string? Name { get; set; }
    public double EstimatedLowBound { get; set; }
    public double EstimatedHighBound { get; set; }
    public int WIP { get; set; }
    public int WIPMax { get; set; }
    public bool IsBuffer { get; set; }

    // Foreign key
    public int ProjectSimulationModelId { get; set; }
    public ProjectSimulationModel ProjectSimulationModel { get; set; } = null!;
}
