namespace ProjectRiskManagementSim.SimulationBlazor.Models;


public class ViewProjectSimulationModel
{
    public string? Name { get; set; }
    public List<StaffModel>? Staff { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime TargetDate { get; set; }
    public RevenueModel? Revenue { get; set; }
    public CostModel? Costs { get; set; }
    public BacklogModel? Backlog { get; set; }
    public List<ViewColumnModel?>? Columns { get; set; }
    // public List<PhaseModel> Phases { get; set; }
    public List<ViewDefectModel> Defects { get; set; }
    public List<ViewBlockingEventModel> BlockingEvents { get; set; }

    // Function to deep copy a ProjectSimulationModel
    internal ViewProjectSimulationModel CloneProjectSimModel(ViewProjectSimulationModel original, string Name)
    {
        return new ViewProjectSimulationModel
        {
            Name = Name,
            Staff = original.Staff!.Select(s => new StaffModel
            {
                Name = s.Name,
                Role = s.Role,
                Sale = s.Sale,
                Cost = s.Cost,
                Days = s.Days
            }).ToList(),
            StartDate = original.StartDate,
            TargetDate = original.TargetDate,
            Revenue = new RevenueModel { Amount = original.Revenue.Amount },
            Costs = new CostModel { Cost = original.Costs.Cost, Days = original.Costs.Days },
            Backlog = new BacklogModel
            {
                Deliverables = original.Backlog.Deliverables.Select(d => new DeliverableModel
                {
                    Id = d.Id,
                    Nr = d.Nr,
                    CompletionDays = d.CompletionDays,
                    AccumulatedDays = d.AccumulatedDays,
                    ColumnIndex = d.ColumnIndex,
                    IsCalculated = d.IsCalculated,
                    WaitTime = d.WaitTime

                }).ToList(),
                PercentageLowBound = original.Backlog.PercentageLowBound,
                PercentageHighBound = original.Backlog.PercentageHighBound
            },
            Columns = original.Columns.Select(c => new ViewColumnModel
            {
                Name = c.Name,
                EstimatedLowBound = c.EstimatedLowBound,
                EstimatedHighBound = c.EstimatedHighBound,
                WIP = c.WIP,
                IsBuffer = c.IsBuffer

            }).ToList()
        };
    }
}
