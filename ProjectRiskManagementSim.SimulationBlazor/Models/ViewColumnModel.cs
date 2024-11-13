namespace ProjectRiskManagementSim.SimulationBlazor.Models;

public class ViewColumnModel
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public double EstimatedLowBound { get; set; }
    public double EstimatedHighBound { get; set; }
    public int WIP { get; set; }
    public int WIPMax { get; set; }
    public bool IsBuffer { get; set; } = false;
    public bool IsDefectsColumn { get; set; } = false;
    public bool IsBlockingEventsColumn { get; set; } = false;


    public double DefectPercentage { get; set; } = 0;
    public double BlockingEventPercentage { get; set; } = 0;

    public double BlockingEventsPercentageLowBound { get; set; } = 0;
    public double BlockingEventsPercentageHighBound { get; set; } = 0;

    public double DefectsPercentageLowBound { get; set; } = 0;
    public double DefectsPercentageHighBound { get; set; } = 0;
    // public List<CardModel> Cards { get; set; }
    // public List<PhaseModel> Phases { get; set; }
    // Constructor

    public ViewColumnModel(int? wip = null, int? wipMax = null)
    {
        WIP = wip ?? 0;
        WIPMax = wipMax ?? WIP;
    }
    public static List<ViewColumnModel> InitializeKanbanColumns()
    {
        return new List<ViewColumnModel>
        {
            new ViewColumnModel(wip: 20, wipMax: 20) { Name = "Open", EstimatedLowBound = 2, EstimatedHighBound = 50, IsBuffer = true },
            new ViewColumnModel(wip: 3, wipMax: 3) { Name = "In Progress", EstimatedLowBound = 1, EstimatedHighBound = 30, IsBuffer = false },
            new ViewColumnModel(wip: 17, wipMax: 17) { Name = "Stuck", EstimatedLowBound = 2, EstimatedHighBound = 50, IsBuffer = true },
            new ViewColumnModel(wip: 20, wipMax: 20) { Name = "Finished", EstimatedLowBound = 2, EstimatedHighBound = 40, IsBuffer = true },
            new ViewColumnModel(wip: 17, wipMax: 17) { Name = "Ready to test on Development", EstimatedLowBound = 2, EstimatedHighBound = 50, IsBuffer = true },
            new ViewColumnModel(wip: 0, wipMax: 2) { Name = "Testing on Development", EstimatedLowBound = 1, EstimatedHighBound = 52, IsBuffer = false },
            new ViewColumnModel(wip: 10, wipMax: 10) { Name = "Ready to test on Production", EstimatedLowBound = 2, EstimatedHighBound = 45, IsBuffer = true },
            new ViewColumnModel(wip: 20, wipMax: 20) { Name = "Done", EstimatedLowBound = 1, EstimatedHighBound = 30, IsBuffer = true },
            new ViewColumnModel(wip: 10, wipMax: 10) { Name = "Ready to test on Production", EstimatedLowBound = 2, EstimatedHighBound = 45, IsBuffer = true },
            new ViewColumnModel(wip: 0, wipMax: 2) { Name = "Testing on Production", EstimatedLowBound = 5, EstimatedHighBound = 70, IsBuffer = false },
            new ViewColumnModel(wip: 20, wipMax: 20) { Name = "Done", EstimatedLowBound = 1, EstimatedHighBound = 30, IsBuffer = true }
        };
    }
}
