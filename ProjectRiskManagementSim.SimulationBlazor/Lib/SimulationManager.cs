using ProjectRiskManagementSim.ProjectSimulation.Interfaces;
using ProjectRiskManagementSim.SimulationBlazor.Models;
namespace ProjectRiskManagementSim.SimulationBlazor.Lib;

public static class SimulationManager
{
    private static readonly Dictionary<Guid, IMonteCarloSimulation> Simulations = new();

    public static void StartSimulation(Guid simulationId, IMonteCarloSimulation simulation)
    {
        Simulations[simulationId] = simulation;
    }

    public static IMonteCarloSimulation? GetSimulation(Guid simulationId)
    {
        Simulations.TryGetValue(simulationId, out var simulation);
        return simulation;
    }

    public static bool IsSimulationFinished(Guid simulationId)
    {
        return !Simulations.ContainsKey(simulationId) || Simulations[simulationId].IsCompleted;
    }
    public static List<IMonteCarloSimulation> GetAllSimulations()
    {
        return Simulations.Values.ToList();
    }
}
