using ProjectRiskManagementSim.DataAccess;

namespace ProjectRiskManagementSim.SimulationBlazor.Components.Pages.Simulations;

public class ListSimulationsHandler
{
    public List<ViewProject> _projects { get; set; } = new List<ViewProject>();
    public List<ViewSimulation> _simulations { get; set; } = new List<ViewSimulation>();
    public bool projectSelected { get; set; } = false;
    public bool simulationSelected { get; set; } = false;

    OxygenSimulationContext _context;
    public ListSimulationsHandler(OxygenSimulationContext context)
    {
        _context = context;
    }
    public async Task InitializeSimulationsAsync()
    {
        var projects = await _context.GetProjectsAsync();
        _projects = projects.Select(p => new ViewProject
        {
            Id = p.Id,
            JiraId = p.JiraId,
            Name = p.Name,
            ProjectSelected = false
        }).ToList();
        var simulations = await _context.GetProjectSimulationsAsync();
        _simulations = simulations.Select(s => new ViewSimulation
        {
            Name = s.Name!,
            CreatedAt = s.CreatedAt,
            UpdatedAt = s.UpdatedAt,
            SimulationSelected = false
        }).ToList();
    }
    public async Task HandleProjectSelection(int projectId)
    {
        if (projectId != 0)
        {
            var project = _projects.FirstOrDefault(p => p.Id == projectId);
            if (project != null)
            {
                // Check if other projects are selected and unselect them
                foreach (var p in _projects)
                {
                    if (p.ProjectSelected && p.Id != projectId)
                    {
                        p.ProjectSelected = false;
                    }
                }
                project.ProjectSelected = !project.ProjectSelected;
            }
        }
        var simulations = await _context.GetProjectSimulationsAsync(projectId);
        if (simulations != null)
        {
            _simulations = simulations.Select(s => new ViewSimulation
            {
                Name = s.Name!,
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt,
                SimulationSelected = false
            }).ToList();
        }
    }
}
public class ViewProject
{
    public required int Id { get; set; }
    public required string JiraId { get; set; }
    public required string Name { get; set; }
    public bool ProjectSelected { get; set; }
}
public class ViewSimulation
{
    public required string Name { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required DateTime UpdatedAt { get; set; }
    public bool SimulationSelected { get; set; }
}

