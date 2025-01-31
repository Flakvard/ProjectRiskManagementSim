namespace ProjectRiskManagementSim.ProjectSimulation.Models;

public class DeliverableModel : IComparer<DeliverableModel>
{
    public Guid Id { get; set; }
    public int Nr { get; set; }
    public double CompletionDays { get; set; }
    public double AccumulatedDays { get; set; }
    public int ColumnIndex { get; set; } = 0;
    public bool IsCalculated { get; set; } = false;
    public double WaitTime { get; set; } = 0;
    public double StoppedWorkingTime { get; set; } = 0;
    public bool IsBug { get; set; } = false;
    public bool IsBlocked { get; set; } = false;
    public bool WasBlocked { get; set; } = false;
    public int DefectIndex { get; set; } = 0;
    public int BlockingEventIndex { get; set; } = 0;

    public DeliverableModel()
    {
        Id = Guid.NewGuid();
        Nr = 0;
        CompletionDays = 0;
        AccumulatedDays = 0;
        StoppedWorkingTime = 0;
    }
    public int Compare(DeliverableModel? x, DeliverableModel? y)
    {
        if (x == null && y == null) return 0;   // Both are null, considered equal
        if (x == null) return -1;               // Null is considered "less than" a non-null value
        if (y == null) return 1;                // Non-null is considered "greater than" null

        // Perform comparison based on Nr property
        return x.Nr.CompareTo(y.Nr);
    }

}
