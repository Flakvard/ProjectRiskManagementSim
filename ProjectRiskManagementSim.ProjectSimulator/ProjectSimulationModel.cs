namespace ProjectRiskManagementSim.ProjectSimulation;


internal class ProjectSimulationModel
{
    public List<StaffModel>? Staff { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime TargetDate { get; set; }
    public required RevenueModel Revenue { get; set; }
    public required CostModel Costs { get; set; }
    public required BacklogModel Backlog { get; set; }
    public required List<ColumnModel> Columns { get; set; }
    // public List<PhaseModel> Phases { get; set; }
    // public List<DefectModel> Defects { get; set; }
    // public List<BlockingEventModel> BlockingEvents { get; set; }
}
