using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ProjectRiskManagementSim.DataAccess.Models;

namespace ProjectRiskManagementSim.DataAccess.Models;
public class ProjectSimulationModel
{
    [Key]
    public int Id { get; set; }

    // Foreign key
    public int ProjectId { get; set; }
    public ProjectModel Project { get; set; } = null!;

    public string? Name { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime TargetDate { get; set; }
    public double? Revenue { get; set; }
    public double? Costs { get; set; }
    public double? Hours { get; set; }

    // Navigation property for one-to-many relationship
    public ICollection<ColumnModel> Columns { get; set; }


    // Deliverables info
    public double DeliverablesCount { get; set; }
    public double PercentageLowBound { get; set; }
    public double PercentageHighBound { get; set; }
}
