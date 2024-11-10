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
    public DateTime? SimEndDate { get; set; }

    public double? ActualRevenue { get; set; }

    public double? BudgetCosts { get; set; }
    public double? ActualCosts { get; set; }
    public double? SimulationCosts { get; set; }

    public double? CostPrDay { get; set; }

    public double FrontendDevs { get; set; }
    public double BackendDevs { get; set; }
    public double Testers { get; set; }

    public double? ActualDays { get; set; }
    public double TargetDays { get; set; }
    public double? SimulationDays { get; set; }
    public double? SimulationDaysOfDelay { get; set; }

    public double? ActualHours { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation property for one-to-many relationship
    public ICollection<ColumnModel> Columns { get; set; }

    // Navigation property for one-to-many relationship
    public ICollection<ForecastModel> Forecasts { get; set; }

    // Navigation property for one-to-many relationship
    public ICollection<SensitivityModel> Sensitivities { get; set; }

    // Navigation property for one-to-many relationship
    public ICollection<StaffAnalysisModel> StaffAnalyses { get; set; }

    // Deliverables info
    public double DeliverablesCount { get; set; }
    public double IssueCount { get; set; }
    public double PercentageLowBound { get; set; }
    public double PercentageHighBound { get; set; }
    public double IssueDoneCount { get; set; }
    public double BugCount { get; set; }
    public double BugPercentage { get; set; }
}
