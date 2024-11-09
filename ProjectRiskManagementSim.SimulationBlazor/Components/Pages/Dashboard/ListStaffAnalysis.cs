using ProjectRiskManagementSim.DataAccess;
using ProjectRiskManagementSim.DataAccess.Models;
using ProjectRiskManagementSim.ProjectSimulation.Interfaces;
using ProjectRiskManagementSim.SimulationBlazor.Lib;
using ProjectRiskManagementSim.SimulationBlazor.Models;

public class ListStaffAnalysis
{
    public List<StaffAnalysis> StaffAnalysisList { get; set; }
    public ProjectSimulationModel? Simulation { get; set; }
    public IMonteCarloSimulation? MonteCarloSimulation { get; set; }
    public Guid SimulationId { get; set; }
    public Dictionary<double, (string, double)>? WipAnalysis { get; set; } = new Dictionary<double, (string, double)>();
    public ListStaffAnalysis()
    {
        StaffAnalysisList = new List<StaffAnalysis>();
        LoadStaffAnalysis();
    }

    private void LoadStaffAnalysis()
    {
        StaffAnalysisList.Add(new StaffAnalysis { Priority = 1, StaffName = "None", Days = 0 });
        StaffAnalysisList.Add(new StaffAnalysis { Priority = 2, StaffName = "None", Days = 0 });
        StaffAnalysisList.Add(new StaffAnalysis { Priority = 3, StaffName = "None", Days = 0 });
        StaffAnalysisList.Add(new StaffAnalysis { Priority = 4, StaffName = "None", Days = 0 });
        StaffAnalysisList.Add(new StaffAnalysis { Priority = 5, StaffName = "None", Days = 0 });
    }
    public ListStaffAnalysis(ProjectSimulationModel? simulation, IMonteCarloSimulation monteCarloSimulation)
    {
        StaffAnalysisList = new List<StaffAnalysis>();
        LoadStaffAnalysis();
        Simulation = simulation;
        MonteCarloSimulation = monteCarloSimulation;
    }
    // Run the simulation staff analysis
    public async Task RunSimulationAnalysis(OxygenSimulationContext context, int dbSimulationId)
    {
        Simulation = await context.GetSimulationByIdAsync(dbSimulationId);
        var mappedProjectData = ModelMapper.MapDBProjectSimulationModelToProjectSimulationModel(Simulation!);
        var simulationId = Guid.NewGuid(); // Create a unique ID for this simulation session
        SimulationId = simulationId; // Store the simulationId for later  

        // Use the injected IMonteCarloSimulation instance to get a new simulation instance
        MonteCarloSimulation!.InitiateSimulation(mappedProjectData, simulationId);
        await MonteCarloSimulation.WIPAnalysis(mappedProjectData, 1000);
        WipAnalysis = MonteCarloSimulation.WipAnalysis;

        UpdateStaffAnalysis();
    }
    public void UpdateStaffAnalysis()
    {
        StaffAnalysisList.Clear();
        for (int i = 1; i <= 5; i++)
        {
            var staffName = WipAnalysis![i].Item1;
            var days = WipAnalysis[i].Item2;
            var staffPriority = i;
            StaffAnalysisList.Add(new StaffAnalysis { Priority = staffPriority, StaffName = staffName, Days = days });
        }
    }
}
public class StaffAnalysis
{
    public int Priority { get; set; }
    public string StaffName { get; set; }
    public double Days { get; set; }
}
