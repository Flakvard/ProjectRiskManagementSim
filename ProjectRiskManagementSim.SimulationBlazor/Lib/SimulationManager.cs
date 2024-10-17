using ProjectRiskManagementSim.ProjectSimulation.Interfaces;
using ProjectRiskManagementSim.SimulationBlazor.Data;
using ProjectRiskManagementSim.SimulationBlazor.Models;
namespace ProjectRiskManagementSim.SimulationBlazor.Lib;

public class SimulationManager
{
    private IMonteCarloSimulation _monteCarloSimulation { get; set; }
    private static readonly Dictionary<Guid, IMonteCarloSimulation> Simulations = new();

    public SimulationManager(IMonteCarloSimulation monteCarloSimulation)
    {
        _monteCarloSimulation = monteCarloSimulation;
    }


    public static void AddSimulationToManager(Guid simulationId, IMonteCarloSimulation simulation)
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
        var mappedProjectData = ModelMapper.MapToSimProjProjectSimulationModel(projectData);

        // Use the injected IMonteCarloSimulation instance to get a new simulation instance
        var simulation = MCS.GetSimulationInstance();
        simulation.InitiateSimulation(mappedProjectData, simulationId);

        // Store the simulation instance with the simulationId for later retrieval
        SimulationManager.AddSimulationToManager(simulationId, simulation);
        return SimulationManager.GetAllSimulations();
    }
    public IMonteCarloSimulation GetFirstSimulation(Guid simulationId)
    {
        if (!Simulations.ContainsKey(simulationId))
        {
            var projectData = Database.ProjectModelInit();
            var mappedProjectData = ModelMapper.MapToSimProjProjectSimulationModel(projectData);

            // Use the injected IMonteCarloSimulation instance to get a new simulation instance
            var simulation = _monteCarloSimulation.GetSimulationInstance();
            simulation.InitiateSimulation(mappedProjectData, simulationId);

            // Store the simulation instance with the simulationId for later retrieval
            Simulations[simulationId] = simulation;
        }

        return Simulations[simulationId];
    }
    public void ResetSimulation(Guid simulationId)
    {
        if (Simulations.ContainsKey(simulationId))
        {
            Simulations.Remove(simulationId); // Reset simulation by removing the old instance
        }
    }
}
