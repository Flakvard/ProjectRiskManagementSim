using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectRiskManagementSim.DataAccess.Models;
public class ProjectModel
{
    [Key]
    public int Id { get; set; }

    public required string Name { get; set; }
    public required string JiraId { get; set; }
    public required string JiraProjectId { get; set; }

    // Navigation property for one-to-many relationship
    public ICollection<ProjectSimulationModel> ProjectSimulationModels { get; set; } = null!;
}
