using ProjectRiskManagementSim.DataAccess;

public class CreateNewProjectHandler
{
    public int ProjectId { get; set; }
    public string? Name { get; set; }
    public string? ProjectCategory { get; set; }
    public string? Manager { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string JiraId { get; set; }
    public string JiraProjectId { get; set; }

    public async Task InitializeProjectsAsync(OxygenAnalyticsContext context)
    {
        await FetchProjectInfo(context);
    }
    private async Task FetchProjectInfo(OxygenAnalyticsContext context)
    {
        // var projects = await context.Projects.ToListAsync();
        var project = await context.GetProjectByIdAsync(ProjectId);
        Name = project.Name;
        ProjectCategory = project.ProjectCategory;
        JiraId = project.JiraId;
        JiraProjectId = project.JiraProjectId;
    }
}
