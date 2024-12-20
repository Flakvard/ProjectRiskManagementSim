using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectRiskManagementSim.DataAccess.Models;
public class IssueLeadTime
{
    public int Id { get; set; }
    [ForeignKey("IssueId")]
    public IssueModel Issue { get; set; }
    public int IssueId { get; set; }
    public string? IssueKey { get; set; }
    public double Open { get; set; }
    public double InProgress { get; set; }
    public double Reopened { get; set; }
    public double Resolved { get; set; }
    public double Closed { get; set; }
    public double Backlog { get; set; }
    public double SelectedForDevelopment { get; set; }
    public double Done { get; set; }
    public double Finished { get; set; }
    public double ReadyForTestOnStage { get; set; }
    public double AwaitingCustomer { get; set; }
    public double AwaitingThirdParty { get; set; }
    public double AwaitingTask { get; set; }
    public double TestingOnStage { get; set; }
    public double WaitingForDeploymentToProduction { get; set; }
    public double ReadyToTestOnProduction { get; set; }
    public double Answered { get; set; }
    public double FailedTest { get; set; }
    public double ReadyForTestOnDev { get; set; }
    public double TestingOnDev { get; set; }
    public double WaitingForDeploymentToStage { get; set; }
    public double ToDo { get; set; }
    public double AwaitingEstimation { get; set; }
    public double InRefinement { get; set; }
    public double AwaitingApproval { get; set; }
    public double InDesign { get; set; }
    public double AwaitingRefinement { get; set; }
    public double OnHold { get; set; }
    public double Created { get; set; }
    public string? LastUpdated { get; set; }
    public string? CreatedDate { get; set; }
    public string? DoneDate { get; set; }
    public string? OpenDate { get; set; }
    public string? ReadyToTestOnProductionDate { get; set; }
    public double? CycleTime { get; set; }
    public double? LeadTime { get; set; }
    public string? CurrentStatus { get; set; }
    public string? PreviousStatus { get; set; }
    public string? IssueType { get; set; }
    public int? Assignee { get; set; }
    public int? Creator { get; set; }
    public int? Reporter { get; set; }
    public string? Epic { get; set; }
    public string? Priority { get; set; }
    public string? Customer { get; set; }
    public string? Project { get; set; }
    public string? Summary { get; set; }
}
