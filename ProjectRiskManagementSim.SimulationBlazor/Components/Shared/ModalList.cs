
public class ProjectViewModel
{
    public int Id { get; set; }
    public Guid ProjectListViewModelId { get; set; }
    public string? Name { get; set; }
    public string? Type { get; set; }
    public string? Manager { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? Status { get; set; }
    public bool Selected { get; set; }


}

public class ProjectListViewModel
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public List<ProjectViewModel> Projects { get; set; }
    public int SelectedProjectId { get; set; }

    public ProjectListViewModel()
    {
        Projects = Init();
    }

    public ProjectViewModel SelectedProject { get; set; } = new ProjectViewModel();

    public List<ProjectViewModel> Init() => new List<ProjectViewModel>
    {
        new ProjectViewModel { Id = 1, Name = "Project 1", Type = "Type 1", Manager = "Manager 1", StartDate = new DateTime(2021, 1, 1), EndDate = new DateTime(2022, 1, 1), Status = "Active" ,Selected = false, ProjectListViewModelId = Id},
        new ProjectViewModel { Id = 2, Name = "Project 2", Type = "Type 2", Manager = "Manager 2", StartDate = new DateTime(2021, 1, 1), EndDate = new DateTime(2022, 1, 1), Status = "Active" ,Selected = false, ProjectListViewModelId = Id},
        new ProjectViewModel { Id = 3, Name = "Project 3", Type = "Type 3", Manager = "Manager 3", StartDate = new DateTime(2021, 1, 1), EndDate = new DateTime(2022, 1, 1), Status = "Active" ,Selected = false, ProjectListViewModelId = Id},
        new ProjectViewModel { Id = 4, Name = "Project 4", Type = "Type 4", Manager = "Manager 4", StartDate = new DateTime(2021, 1, 1), EndDate = new DateTime(2022, 1, 1), Status = "Active" ,Selected = false, ProjectListViewModelId = Id},
        new ProjectViewModel { Id = 5, Name = "Project 5", Type = "Type 5", Manager = "Manager 5", StartDate = new DateTime(2021, 1, 1), EndDate = new DateTime(2022, 1, 1), Status = "Active" ,Selected = false, ProjectListViewModelId = Id},
        new ProjectViewModel { Id = 6, Name = "Project 6", Type = "Type 6", Manager = "Manager 6", StartDate = new DateTime(2021, 1, 1), EndDate = new DateTime(2022, 1, 1), Status = "Active" ,Selected = false, ProjectListViewModelId = Id},
        new ProjectViewModel { Id = 7, Name = "Project 7", Type = "Type 7", Manager = "Manager 7", StartDate = new DateTime(2021, 1, 1), EndDate = new DateTime(2022, 1, 1), Status = "Active" ,Selected = false, ProjectListViewModelId = Id},
        new ProjectViewModel { Id = 8, Name = "Project 8", Type = "Type 8", Manager = "Manager 8", StartDate = new DateTime(2021, 1, 1), EndDate = new DateTime(2022, 1, 1), Status = "Active" ,Selected = false, ProjectListViewModelId = Id},
        new ProjectViewModel { Id = 9, Name = "Project 9", Type = "Type 9", Manager = "Manager 9", StartDate = new DateTime(2021, 1, 1), EndDate = new DateTime(2022, 1, 1), Status = "Active" ,Selected = false, ProjectListViewModelId = Id},
    };
    public void SetProjectListViewModelId(Guid id)
    {
        Id = id;
    }
    public void SetSelectedProject(int id)
    {
        SelectedProject = Projects.FirstOrDefault(x => x.Id == id);
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
                    <td class='{activeTdClass}'>{p.Type}</td>
                    <td class='{activeTdClass}'>{p.Manager}</td>
                    <td class='{activeTdClass}'>{p.StartDate.ToString("MM/dd/yyyy")}</td>
                    <td class='{activeTdClass}'>{p.EndDate.ToString("MM/dd/yyyy")}</td>
                    <td class='{activeTdClass}'>{p.Status}</td>
                    <td class='{activeTdClass}'>{p.Selected}</td>
                </tr>";
                }
                return html;
            }
        }
        return string.Empty;
    }
}

