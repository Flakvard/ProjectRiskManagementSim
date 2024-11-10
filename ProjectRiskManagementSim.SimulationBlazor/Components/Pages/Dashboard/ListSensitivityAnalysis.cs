using ProjectRiskManagementSim.DataAccess;
using ProjectRiskManagementSim.DataAccess.Models;
using ProjectRiskManagementSim.ProjectSimulation.Interfaces;
using ProjectRiskManagementSim.SimulationBlazor.Lib;
using ProjectRiskManagementSim.SimulationBlazor.Models;

public class ListSensitivityAnalysis
{
    public List<SensitivityAnalysis> SensitivityAnalysisList { get; set; }
    public ProjectSimulationModel? Simulation { get; set; }
    public IMonteCarloSimulation? MonteCarloSimulation { get; set; }
    public Guid SimulationId { get; set; }
    public Dictionary<double, (string, double)>? ColumnAnalysis { get; set; } = new Dictionary<double, (string, double)>();

    private void LoadSensitivityAnalysis()
    {
        SensitivityAnalysisList.Add(new SensitivityAnalysis { Priority = 1, SensitivityName = "None", Days = 0, EndDate = "None" });
        SensitivityAnalysisList.Add(new SensitivityAnalysis { Priority = 2, SensitivityName = "None", Days = 0, EndDate = "None" });
        SensitivityAnalysisList.Add(new SensitivityAnalysis { Priority = 3, SensitivityName = "None", Days = 0, EndDate = "None" });
        SensitivityAnalysisList.Add(new SensitivityAnalysis { Priority = 4, SensitivityName = "None", Days = 0, EndDate = "None" });
        SensitivityAnalysisList.Add(new SensitivityAnalysis { Priority = 5, SensitivityName = "None", Days = 0, EndDate = "None" });
    }
    public ListSensitivityAnalysis(ProjectSimulationModel? simulation, IMonteCarloSimulation monteCarloSimulation)
    {
        Simulation = simulation;
        if (Simulation != null && Simulation!.Sensitivities.Count > 0)
        {
            SensitivityAnalysisList = Simulation.Sensitivities.Select(s => new SensitivityAnalysis
            {
                Priority = s.Priority,
                SensitivityName = s.SensitivityName,
                Days = s.Days,
                EndDate = s.EndDate
            }).ToList();
        }
        else
        {
            SensitivityAnalysisList = new List<SensitivityAnalysis>();
            LoadSensitivityAnalysis();
        }
        MonteCarloSimulation = monteCarloSimulation;
    }

    public ListSensitivityAnalysis()
    {
        SensitivityAnalysisList = new List<SensitivityAnalysis>();
        LoadSensitivityAnalysis();
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
        await MonteCarloSimulation.ColumnEstimateAnalysis(mappedProjectData, 1000);
        ColumnAnalysis = MonteCarloSimulation.ColumnAnalysis;

        UpdateSensitivityAnalysis();

        // Add or Update the Sensitivities in database
        if (Simulation != null)
        {
            Simulation.Sensitivities = SensitivityAnalysisList.Select(s => new SensitivityModel
            {
                Priority = s.Priority,
                SensitivityName = s.SensitivityName,
                Days = s.Days,
                EndDate = s.EndDate,
            }).ToList();
            await context.UpdateSimulationAsync(Simulation);
        }
        else
        {
            var sensitivities = SensitivityAnalysisList.Select(s => new SensitivityModel
            {
                Priority = s.Priority,
                SensitivityName = s.SensitivityName,
                Days = s.Days,
                EndDate = s.EndDate,
                ProjectSimulationModelId = Simulation!.Id
            }).ToList();
            context.SensitivityModel.AddRange(sensitivities);
        }
    }
    public void UpdateSensitivityAnalysis()
    {
        SensitivityAnalysisList.Clear();
        for (int i = 1; i <= 5; i++)
        {
            if (ColumnAnalysis == null || !ColumnAnalysis!.ContainsKey(i)) continue;
            var sensitivityName = ColumnAnalysis![i].Item1;
            var days = ColumnAnalysis[i].Item2;
            DateTime listOfEndDays = listOfEndDays = Simulation!.StartDate.AddDays(days);
            var endDate = listOfEndDays.ToString("dd MMM yyyy");
            var sensitivityPriority = i;
            SensitivityAnalysisList.Add(new SensitivityAnalysis { Priority = sensitivityPriority, SensitivityName = sensitivityName, Days = days, EndDate = endDate });
        }
    }
}
public class SensitivityAnalysis
{
    public int Priority { get; set; }
    public string SensitivityName { get; set; } = null!;
    public double Days { get; set; }
    public string EndDate { get; set; } = null!;
}
