using ProjectRiskManagementSim.DataAccess.Models;

namespace ProjectRiskManagementSim.SimulationBlazor.Components.Pages.Dashboard;

public partial class ListSimulationsResultTable
{

    public List<ProjectSimulationModel> Simulations { get; set; }
    public List<SimulationsResultTableModel> SimulationsResultTableModels;

    public ListSimulationsResultTable()
    {
        SimulationsResultTableModels = SimulationsResultTableModel.InitializeSimulationResults();
    }
}

public class SimulationsResultTableModel
{
    public string SimulationName { get; set; }
    public string SimulationDate { get; set; }
    public string TargetDate { get; set; }
    public int Days { get; set; }
    public string Budget { get; set; }
    public string ActualCost { get; set; }
    public string CostPerDay { get; set; }
    public string DelayPerDay { get; set; }
    public int Epics { get; set; }

    public static List<SimulationsResultTableModel> InitializeSimulationResults()
    {
        return new List<SimulationsResultTableModel>{
        new SimulationsResultTableModel
        {
          SimulationName = "None",
          SimulationDate = "None",
          TargetDate = "None",
          Days = 0,
          Budget = "0",
          ActualCost = "0",
          CostPerDay = "0",
          DelayPerDay = "0",
          Epics = 0
        },
        new SimulationsResultTableModel
        {
          SimulationName = "None",
          SimulationDate = "None",
          TargetDate = "None",
          Days = 0,
          Budget = "0",
          ActualCost = "0",
          CostPerDay = "0",
          DelayPerDay = "0",
          Epics = 0,
        },
        new SimulationsResultTableModel
        {
          SimulationName = "None",
          SimulationDate = "None",
          TargetDate = "None",
          Days = 0,
          Budget = "0",
          ActualCost = "0",
          CostPerDay = "0",
          DelayPerDay = "0",
          Epics = 0,
        },
        new SimulationsResultTableModel
        {
          SimulationName = "None",
          SimulationDate = "None",
          TargetDate = "None",
          Days = 0,
          Budget = "0",
          ActualCost = "0",
          CostPerDay = "0",
          DelayPerDay = "0",
          Epics = 0,
        }
      };
    }
}

