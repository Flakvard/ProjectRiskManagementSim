using ProjectRiskManagementSim.ProjectSimulation.Interfaces;
using ProjectRiskManagementSim.SimulationBlazor.Data;
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
    public static List<IMonteCarloSimulation> InitSimulationsFromDb(IMonteCarloSimulation MCS)
    {
        var projectData = Database.ProjectModelInit();
        var simulationId = Guid.NewGuid(); // Create a unique ID for this simulation session
        var mappedProjectData = ModelMapper.MapToProjectSimulationModel(projectData);

        // Use the injected IMonteCarloSimulation instance to get a new simulation instance
        var simulation = MCS.GetSimulationInstance();
        simulation.InitiateAndRunSimulation(mappedProjectData, 100, simulationId);

        // Store the simulation instance with the simulationId for later retrieval
        SimulationManager.StartSimulation(simulationId, simulation);
        return SimulationManager.GetAllSimulations();
    }
}
