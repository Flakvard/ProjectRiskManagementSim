using ProjectRiskManagementSim.DataAccess.Models;
using ProjectRiskManagementSim.DataAccess;

namespace ProjectRiskManagementSim.SimulationBlazor.Components.Pages.Dashboard;

public class DashboardInfoHandler
{

    public ProjectModel? _project { get; set; }
    public List<ProjectSimulationModel> _simulations { get; set; } = new List<ProjectSimulationModel>();
    public ProjectSimulationModel? _simulation { get; set; }
    public int SimulationId { get; set; }
    public DashboardInfoHandler(int projectId)
    {
        SimulationId = projectId;
    }
    public async Task InitializeProjectsAsync(OxygenSimulationContext context)
    {
        // Initialize the list of simulations
        var simulation = await context.GetSimulationByIdAsync(SimulationId);
        if (simulation != null)
        {
            _simulation = simulation;
            // Get the project for the simulation
            var project = await context.GetProjectBySimulationIdAsync(simulation.ProjectId);
            _project = project;
            // Get all simulations for the project
            if (project != null)
            {
                var simulations = await context.GetProjectSimulationsAsync(project.Id);
                if (simulations != null)
                    _simulations = simulations;
            }
        }
    }

}
