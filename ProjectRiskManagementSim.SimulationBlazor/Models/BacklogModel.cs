namespace ProjectRiskManagementSim.SimulationBlazor.Models;

public class BacklogModel
{
    public required List<DeliverableModel> Deliverables { get; set; }
    public required double PercentageLowBound { get; set; }
    public required double PercentageHighBound { get; set; }

}
