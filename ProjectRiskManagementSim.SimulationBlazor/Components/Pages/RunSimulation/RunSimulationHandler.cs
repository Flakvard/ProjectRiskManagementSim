using ProjectRiskManagementSim.ProjectSimulation.Interfaces;
using ProjectRiskManagementSim.SimulationBlazor.Lib;

namespace ProjectRiskManagementSim.SimulationBlazor.Components.Pages.RunSimulation
{
    public class RunSimulationHandler
    {
        public IMonteCarloSimulation MCS { get; set; } = SimulationManager.GetFirstSimulation();
    }
}
