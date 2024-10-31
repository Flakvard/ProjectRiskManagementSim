
namespace ProjectRiskManagementSim.DataAccess.Models;
public class IssueModel
{
    public int Id { get; set; }
    public string? JiraId { get; set; }
    public string? Key { get; set; }
    public string? JiraKey { get; set; }
    public DateTime CreateAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public string? Summary { get; set; }
    public double? TimeEstimateSeconds { get; set; }
    public double? TimeSpentSeconds { get; set; }
    public string? Epic { get; set; }
    public int IssueTypeId { get; set; }
    public int StatusId { get; set; }
    public int IssuePriorityId { get; set; }
    public int? AssigneeId { get; set; }
    public int? ReporterId { get; set; }
    public int? CreatorId { get; set; }
    public int ProjectId { get; set; }
    public double TimeRemainingSeconds { get; set; }
}
