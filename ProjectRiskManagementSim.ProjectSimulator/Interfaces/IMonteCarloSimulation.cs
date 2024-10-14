using ProjectRiskManagementSim.ProjectSimulation.Models;

namespace ProjectRiskManagementSim.ProjectSimulation.Interfaces;

public interface IMonteCarloSimulation
{
    public Task ColumnEstimateAnalysis(ProjectSimulationModel projectSimModel, int projectSimulationsCount);
    public Task WIPAnalysis(ProjectSimulationModel projectSimModel, int projectSimulationsCount);
    public Task BlockWIPAnalysis(ProjectSimulationModel projectSimModel, int projectSimulationsCount);
    public void PrintSimulationResults(ProjectSimulationModel projectSimulationModel, int simulationCount);
    public bool IsCompleted { get; }
    public void InitiateAndRunSimulation(ProjectSimulationModel projectSimulationModel);
    public IMonteCarloSimulation GetSimulationInstance();
}
