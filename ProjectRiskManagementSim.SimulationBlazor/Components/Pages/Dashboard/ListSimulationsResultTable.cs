using ProjectRiskManagementSim.DataAccess.Models;

namespace ProjectRiskManagementSim.SimulationBlazor.Components.Pages.Dashboard;

public partial class ListSimulationsResultTable
{

    public List<ProjectSimulationModel> Simulations { get; set; }
    public List<SimulationsResultTableModel> SimulationsResultTableModels;

    public ListSimulationsResultTable()
    {
        if (Simulations == null)
            SimulationsResultTableModels = SimulationsResultTableModel.InitializeSimulationResults();
        if (Simulations != null)
        {
            SimulationsResultTableModels = MapProjectSimModelToResultTable(Simulations);
        }
    }
    public void Init(List<ProjectSimulationModel> simulationModels)
    {
        Simulations = simulationModels;
        if (Simulations == null)
            SimulationsResultTableModels = SimulationsResultTableModel.InitializeSimulationResults();
        if (Simulations != null)
        {
            SimulationsResultTableModels = MapProjectSimModelToResultTable(Simulations);
        }
    }
    private List<SimulationsResultTableModel> MapProjectSimModelToResultTable(List<ProjectSimulationModel> simulations)
    {
        List<SimulationsResultTableModel> resultTableModels = new List<SimulationsResultTableModel>();
        foreach (var simulation in simulations)
        {
            resultTableModels.Add(new SimulationsResultTableModel
            {
                SimulationName = simulation.Name ?? "N/A",
                SimulationDate = simulation.SimEndDate?.ToString("dd MMM yyyy") ?? "N/A",
                TargetDate = simulation.TargetDate.ToString("dd MMM yyyy"),
                Days = (int)(simulation.SimulationDays ?? 0),
                Budget = simulation.BudgetCosts?.ToString("N0") ?? "0",
                SimCost = simulation.SimulationCosts?.ToString("N0") ?? "0",
                ActualCost = simulation.ActualCosts?.ToString("N0") ?? "0",
                CostPerDay = simulation.CostPrDay?.ToString("N0") ?? "0",
                DelayPerDay = simulation.SimulationDaysOfDelay?.ToString() ?? "0",
                Epics = (int)simulation.DeliverablesCount,
                BugPercentage = ((simulation.BugPercentage / 100).ToString("P0")) ?? "0"
            });
        }
        return resultTableModels;
    }
}

public class SimulationsResultTableModel
{
    public string SimulationName { get; set; }
    public string SimulationDate { get; set; }
    public string TargetDate { get; set; }
    public int Days { get; set; }
    public string Budget { get; set; }
    public string SimCost { get; set; }
    public string ActualCost { get; set; }
    public string CostPerDay { get; set; }
    public string DelayPerDay { get; set; }
    public int Epics { get; set; }
    public string BugPercentage { get; set; }

    public static List<SimulationsResultTableModel> InitializeSimulationResults()
    {
        var list = new List<SimulationsResultTableModel>();
        for (int i = 0; i < 4; i++)
        {
            list.Add(new SimulationsResultTableModel
            {
                SimulationName = "None",
                SimulationDate = "None",
                TargetDate = "None",
                Days = 0,
                Budget = "0",
                ActualCost = "0",
                SimCost = "0",
                CostPerDay = "0",
                DelayPerDay = "0",
                Epics = 0,
                BugPercentage = "0"
            });
        }
        return list;
    }
}

