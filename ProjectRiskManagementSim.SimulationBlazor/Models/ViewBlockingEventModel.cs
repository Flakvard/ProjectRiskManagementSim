namespace ProjectRiskManagementSim.SimulationBlazor.Models;

public class ViewBlockingEventModel
{
    public Guid Id { get; set; }
    public string? Name { get; set; }

    public double BlockingEventPercentage { get; set; } = 0;
    public double BlockingEventsPercentageLowBound { get; set; } = 0;
    public double BlockingEventsPercentageHighBound { get; set; } = 0;
}
