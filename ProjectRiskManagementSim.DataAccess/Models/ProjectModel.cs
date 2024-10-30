namespace ProjectRiskManagementSim.DataAccess.Models;
public class Project
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string JiraId { get; set; }
    public string ProjectCategory { get; set; }
    public string JiraProjectId { get; set; }
    public int? AccountId { get; set; }
}
