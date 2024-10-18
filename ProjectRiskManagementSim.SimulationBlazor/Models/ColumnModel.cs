namespace ProjectRiskManagementSim.SimulationBlazor.Models;

public class ColumnModel
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public double EstimatedLowBound { get; set; }
    public double EstimatedHighBound { get; set; }
    public int WIP { get; set; }
    public int WIPMax { get; set; }
    public bool IsBuffer { get; set; } = false;
    // public List<CardModel> Cards { get; set; }
    // public List<PhaseModel> Phases { get; set; }
    // public List<BlockingEventModel> BlockingEvents { get; set; }
    // public List<DefectModel> Defects { get; set; }
    // Constructor
    public ColumnModel(int? wip = null, int? wipMax = null)
    {
        WIP = wip ?? 0;
        WIPMax = wipMax ?? WIP;
    }
    public static List<ColumnModel> InitializeKanbanColumns()
    {
        return new List<ColumnModel>
        {
            new ColumnModel(wip: 20, wipMax: 20) { Name = "Open", EstimatedLowBound = 2, EstimatedHighBound = 50, IsBuffer = true },
            new ColumnModel(wip: 3, wipMax: 3) { Name = "In Progress", EstimatedLowBound = 1, EstimatedHighBound = 30, IsBuffer = false },
            new ColumnModel(wip: 17, wipMax: 17) { Name = "Stuck", EstimatedLowBound = 2, EstimatedHighBound = 50, IsBuffer = true },
            new ColumnModel(wip: 20, wipMax: 20) { Name = "Finished", EstimatedLowBound = 2, EstimatedHighBound = 40, IsBuffer = true },
            new ColumnModel(wip: 17, wipMax: 17) { Name = "Ready to test on Development", EstimatedLowBound = 2, EstimatedHighBound = 50, IsBuffer = true },
            new ColumnModel(wip: 0, wipMax: 2) { Name = "Testing on Development", EstimatedLowBound = 1, EstimatedHighBound = 52, IsBuffer = false },
            new ColumnModel(wip: 10, wipMax: 10) { Name = "Ready to test on Production", EstimatedLowBound = 2, EstimatedHighBound = 45, IsBuffer = true },
            new ColumnModel(wip: 20, wipMax: 20) { Name = "Done", EstimatedLowBound = 1, EstimatedHighBound = 30, IsBuffer = true },
            new ColumnModel(wip: 10, wipMax: 10) { Name = "Ready to test on Production", EstimatedLowBound = 2, EstimatedHighBound = 45, IsBuffer = true },
            new ColumnModel(wip: 0, wipMax: 2) { Name = "Testing on Production", EstimatedLowBound = 5, EstimatedHighBound = 70, IsBuffer = false },
            new ColumnModel(wip: 20, wipMax: 20) { Name = "Done", EstimatedLowBound = 1, EstimatedHighBound = 30, IsBuffer = true }
        };
    }
}
