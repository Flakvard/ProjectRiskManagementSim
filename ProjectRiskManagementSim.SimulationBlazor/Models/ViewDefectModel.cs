namespace ProjectRiskManagementSim.SimulationBlazor.Models;

public class ViewDefectModel
{
    public Guid Id { get; set; }
    public string? Name { get; set; }

    public double DefectPercentage { get; set; } = 0;
    public double DefectsPercentageLowBound { get; set; } = 0;
    public double DefectsPercentageHighBound { get; set; } = 0;
}