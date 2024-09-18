namespace ProjectRiskManagementSim.ProjectSimulation;

internal class DeliverableModel : IComparer<DeliverableModel>
{
    public Guid Id { get; set; }
    public int Nr { get; set; }
    public double CompletionDays { get; set; }

    public int Compare(DeliverableModel x, DeliverableModel y)
    {
        return x.Nr.CompareTo(y.Nr);
    }
}
