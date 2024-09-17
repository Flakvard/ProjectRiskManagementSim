namespace ProjectRiskManagementSim.ProjectSimulation;

internal class ProjectSimulationModel
{
    public List<StaffModel>? Staff { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime TargetDate { get; set; }
    public RevenueModel? Revenue { get; set; }
    public CostModel Costs { get; set; }
    // public List<DeliverableModel> Deliverables { get; set; }
    // public List<PhaseModel> Phases { get; set; }
    // public List<ColumnModel> Columns { get; set; }
    // public List<DefectModel> Defects { get; set; }
    // public List<BlockingEventModel> BlockingEvents { get; set; }
}
