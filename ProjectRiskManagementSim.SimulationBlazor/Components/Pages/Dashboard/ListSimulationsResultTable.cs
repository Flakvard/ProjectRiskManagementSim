namespace ProjectRiskManagementSim.SimulationBlazor.Components.Pages.Dashboard;

public class SimulationsResultTableModel
{
    public string SimulationName { get; set; }
    public string SimulationDate { get; set; }
    public string TargetDate { get; set; }
    public int Days { get; set; }
    public decimal Budget { get; set; }
    public decimal ActualCost { get; set; }
    public decimal CostPerDay { get; set; }
    public decimal DelayPerDay { get; set; }
    public int Epics { get; set; }

    public static List<SimulationsResultTableModel> InitializeSimulationResults()
    {
        return new List<SimulationsResultTableModel>{
        new SimulationsResultTableModel
        {
          SimulationName = "Phase 1",
          SimulationDate = "25.09.2024",
          TargetDate = "01-10-2024",
          Days = -6,
          Budget = 1100000,
          ActualCost = 80000,
          CostPerDay = 2846,
          DelayPerDay = 5320,
          Epics = 25
        },
        new SimulationsResultTableModel
        {
          SimulationName = "Ekstra feature on project",
          SimulationDate = "20.10.2024",
          TargetDate = "01-10-2024",
          Days = 19,
          Budget = 1200000,
          ActualCost = 110000,
          CostPerDay = 2996,
          DelayPerDay = 5320,
          Epics = 30
        },
        new SimulationsResultTableModel
        {
          SimulationName = "New Deadline",
          SimulationDate = "09.11.2024",
          TargetDate = "09-11-2024",
          Days = 0,
          Budget = 1200000,
          ActualCost = 140000,
          CostPerDay = 3154,
          DelayPerDay = 5320,
          Epics = 30
        },
        new SimulationsResultTableModel
        {
          SimulationName = "Phase II of bech bruun project",
          SimulationDate = "19.11.2024",
          TargetDate = "09-11-2024",
          Days = 10,
          Budget = 1250000,
          ActualCost = 180000,
          CostPerDay = 3154,
          DelayPerDay = 5320,
          Epics = 30
        }
      };
    }
}

