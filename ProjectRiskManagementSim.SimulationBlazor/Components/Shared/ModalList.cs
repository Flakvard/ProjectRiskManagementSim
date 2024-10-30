
using Microsoft.EntityFrameworkCore;
using ProjectRiskManagementSim.DataAccess;

public class ProjectViewModel
{
    public int Id { get; set; }
    public Guid ProjectListViewModelId { get; set; }
    public string? Name { get; set; }
    public string? ProjectCategory { get; set; }
    public string? Manager { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Status { get; set; }
    public string JiraId { get; set; }
    public string JiraProjectId { get; set; }
    public int? AccountId { get; set; }
    public bool Selected { get; set; }
}

public class ProjectListViewModel
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public List<ProjectViewModel> Projects { get; set; } = new List<ProjectViewModel>();
    public int SelectedProjectId { get; set; }
    public ProjectViewModel SelectedProject { get; set; } = new ProjectViewModel();

    public void SetProjectListViewModelId(Guid id)
    {
        Id = id;
    }

    public void SetSelectedProject(int id)
    {
        SelectedProject = Projects.FirstOrDefault(x => x.Id == id);
    }

    public async Task InitializeProjectsAsync(OxygenAnalyticsContext context)
    {
        Projects = await FetchProjectsAsync(context);
    }

    public string HandleProjectSelection(int? projectId, ProjectListViewModel plvm)
    {
        var handlerProjectListViewModel = plvm; // Assuming ProjectsView is already populated
        if (handlerProjectListViewModel.Projects.Any())
        {
            var project = handlerProjectListViewModel.Projects.FirstOrDefault(p => p.Id == projectId);
            if (project != null)
            {
                // Check if other projects are selected and unselect them
                foreach (var p in handlerProjectListViewModel.Projects)
                {
                    if (p.Selected && p.Id != projectId)
                    {
                        p.Selected = false;
                    }
                }
                project.Selected = !project.Selected;
                string html = "";
                foreach (var p in handlerProjectListViewModel.Projects)
                {
                    var activeTdClass = (p.Selected == true ? "bg-[#7e44eb] text-white border-b border-gray-300" : "border-b border-gray-300");
                    html += $@"
                <tr id='{p.Id}'
                  hx-put='/select-project/{p.Id}?projectListViewModelId={p.ProjectListViewModelId}'
                  hx-trigger='click' hx-target='#ProjectsInTable' hx-swap='outerHTML'>
                    <td class='hidden'>{p.Id}</td>
                    <td class='hidden'>{p.ProjectListViewModelId}</td>
                    <td class='{activeTdClass}'>{p.Name}</td>
                    <td class='{activeTdClass}'>{p.ProjectCategory}</td>
                    <td class='{activeTdClass}'>{p.Manager}</td>
                    <td class='{activeTdClass}'>{p.Status}</td>
                    <td class='{activeTdClass}'>{p.Selected}</td>
                </tr>";
                }
                return html;
            }
        }
        return string.Empty;
    }

    private async Task<List<ProjectViewModel>> FetchProjectsAsync(OxygenAnalyticsContext context)
    {
        // var projects = await context.Projects.ToListAsync();
        var projects = await context.GetProjectsAsync();
        return projects.Select(p => new ProjectViewModel
        {
            Id = p.Id,
            Name = p.Name,
            ProjectCategory = p.ProjectCategory,
            Selected = false,
            JiraId = p.JiraId,
            JiraProjectId = p.JiraProjectId,
            AccountId = p.AccountId,
            ProjectListViewModelId = Guid.NewGuid() // Assign a new Guid for each project
        }).ToList();
    }
}
