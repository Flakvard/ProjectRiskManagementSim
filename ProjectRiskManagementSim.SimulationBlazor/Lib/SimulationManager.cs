using ProjectRiskManagementSim.ProjectSimulation;
namespace ProjectRiskManagementSim.SimulationBlazor.Lib;

public static class SimulationManager
{
    private static readonly Dictionary<Guid, MonteCarloSimulation> Simulations = new();

    public static void StartSimulation(Guid simulationId, MonteCarloSimulation simulation)
    {
        Simulations[simulationId] = simulation;
    }

    public static MonteCarloSimulation? GetSimulation(Guid simulationId)
    {
        Simulations.TryGetValue(simulationId, out var simulation);
        return simulation;
    }

    public static bool IsSimulationFinished(Guid simulationId)
    {
        return !Simulations.ContainsKey(simulationId) || Simulations[simulationId].IsCompleted;
    }
}
