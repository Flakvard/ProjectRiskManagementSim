namespace ProjectRiskManagementSim.ProjectSimulation;

internal class ColumnModel
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public double EstimatedLowBound { get; set; }
    public double EstimatedHighBound { get; set; }
    public int WIP { get; set; }
    // public List<CardModel> Cards { get; set; }
    // public List<PhaseModel> Phases { get; set; }
    // public List<BlockingEventModel> BlockingEvents { get; set; }
    // public List<DefectModel> Defects { get; set; }
}
