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
        StaffAnalysisList.Add(new StaffAnalysis { Priority = 1, StaffName = "None", Days = 0, EndDate = "None" });
        StaffAnalysisList.Add(new StaffAnalysis { Priority = 2, StaffName = "None", Days = 0, EndDate = "None" });
        StaffAnalysisList.Add(new StaffAnalysis { Priority = 3, StaffName = "None", Days = 0, EndDate = "None" });
        StaffAnalysisList.Add(new StaffAnalysis { Priority = 4, StaffName = "None", Days = 0, EndDate = "None" });
        StaffAnalysisList.Add(new StaffAnalysis { Priority = 5, StaffName = "None", Days = 0, EndDate = "None" });
    }
    public ListStaffAnalysis(ProjectSimulationModel? simulation, IMonteCarloSimulation monteCarloSimulation)
    {
        if (simulation != null && simulation!.StaffAnalyses.Count > 0)
        {
            StaffAnalysisList = simulation.StaffAnalyses.Select(s => new StaffAnalysis
            {
                Priority = s.Priority,
                StaffName = s.StaffName,
                Days = s.Days,
                EndDate = s.EndDate
            }).ToList();
        }
        else
        {
            StaffAnalysisList = new List<StaffAnalysis>();
            LoadStaffAnalysis();
        }
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
        // Add or Update the StaffAnalyses in database
        if (Simulation != null)
        {
            Simulation.StaffAnalyses = StaffAnalysisList.Select(s => new StaffAnalysisModel
            {
                Priority = s.Priority,
                StaffName = s.StaffName,
                Days = s.Days,
                EndDate = s.EndDate,

            }).ToList();
            await context.UpdateSimulationAsync(Simulation);
        }
        else
        {
            var staffAnalyses = StaffAnalysisList.Select(s => new StaffAnalysisModel
            {
                Priority = s.Priority,
                StaffName = s.StaffName,
                Days = s.Days,
                EndDate = s.EndDate,
                ProjectSimulationModelId = Simulation!.Id
            }).ToList();
            context.StaffAnalysisModel.AddRange(staffAnalyses);
        }

    }
    public void UpdateStaffAnalysis()
    {
        StaffAnalysisList.Clear();
        for (int i = 1; i <= 5; i++)
        {
            if (WipAnalysis == null || !WipAnalysis!.ContainsKey(i)) continue;
            var staffName = WipAnalysis![i].Item1;
            var days = WipAnalysis[i].Item2;
            var staffPriority = i;
            DateTime listOfEndDays = listOfEndDays = Simulation!.StartDate.AddDays(days);
            var endDate = listOfEndDays.ToString("dd MMM yyyy");
            StaffAnalysisList.Add(new StaffAnalysis { Priority = staffPriority, StaffName = staffName, Days = days, EndDate = endDate });
        }
    }
}
public class StaffAnalysis
{
    public int Priority { get; set; }
    public string StaffName { get; set; } = null!;
    public double Days { get; set; }
    public string EndDate { get; set; } = null!;
}
