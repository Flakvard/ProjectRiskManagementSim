using ProjectRiskManagementSim.ProjectSimulation.Models;

namespace ProjectRiskManagementSim.ProjectSimulation.Interfaces;

public interface IMonteCarloSimulation
{
    public ProjectSimulationModel ProjectSimulationModel { get; set; }
    public List<double>? SimTotalDaysResult { get; set; }
    public List<double>? SimTotalCostsResult { get; set; }
    public List<double>? SimTotalSalesResult { get; set; }
    public List<List<DeliverableModel>>? Simulations { get; set; }
    public DateTime NewDate { get; set; }
    public bool IsCompleted { get; set; }
    public Guid SimulationId { get; set; }

    public Task ColumnEstimateAnalysis(ProjectSimulationModel projectSimModel, int projectSimulationsCount);
    public Task WIPAnalysis(ProjectSimulationModel projectSimModel, int projectSimulationsCount);
    public Task BlockWIPAnalysis(ProjectSimulationModel projectSimModel, int projectSimulationsCount);
    public void PrintSimulationResults(ProjectSimulationModel projectSimulationModel, int simulationCount);
    public void InitiateAndRunSimulation(ProjectSimulationModel projectSimulationModel, int simulationCount, Guid simulationId);
    public IMonteCarloSimulation GetSimulationInstance();
    public (List<ColumnModel>, List<DeliverableModel>) RunSimulationStep(ProjectSimulationModel projectSimulationModel, int currentDay);
    public void InitiateSimulation(ProjectSimulationModel projectSimulationModel, Guid simulationId);
    public void ResetSimulation();
}
