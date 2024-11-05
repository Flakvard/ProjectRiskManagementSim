using ProjectRiskManagementSim.SimulationBlazor.Models;
namespace ProjectRiskManagementSim.SimulationBlazor.Components.Pages.Simulations;

public class SimulationResponse
{
    public ViewProjectSimulationModel projectData { get; set; } = new();
    public Guid SimulationId { get; set; }
}
