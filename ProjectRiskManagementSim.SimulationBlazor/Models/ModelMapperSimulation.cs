namespace ProjectRiskManagementSim.SimulationBlazor.Models;
public static class ModelMapper
{
    public static ProjectRiskManagementSim.ProjectSimulation.Models.ProjectSimulationModel MapToProjectSimulationModel(ProjectRiskManagementSim.SimulationBlazor.Models.ProjectSimulationModel source)
    {
        return new ProjectRiskManagementSim.ProjectSimulation.Models.ProjectSimulationModel
        {
            Name = source.Name,
            Staff = source.Staff?.Select(s => new ProjectRiskManagementSim.ProjectSimulation.Models.StaffModel
            {
                Name = s.Name,
                Role = (ProjectRiskManagementSim.ProjectSimulation.Models.Role)s.Role,
                Sale = s.Sale,
                Cost = s.Cost,
                Days = s.Days
            }).ToList(),
            StartDate = source.StartDate,
            TargetDate = source.TargetDate,
            Revenue = new ProjectRiskManagementSim.ProjectSimulation.Models.RevenueModel { Amount = source.Revenue.Amount },
            Costs = new ProjectRiskManagementSim.ProjectSimulation.Models.CostModel { Cost = source.Costs.Cost, Days = source.Costs.Days },
            Backlog = new ProjectRiskManagementSim.ProjectSimulation.Models.BacklogModel
            {
                Deliverables = source.Backlog.Deliverables.Select(d => new ProjectRiskManagementSim.ProjectSimulation.Models.DeliverableModel
                {
                    Id = d.Id,
                    Nr = d.Nr,
                    CompletionDays = d.CompletionDays,
                    AccumulatedDays = d.AccumulatedDays,
                    ColumnIndex = d.ColumnIndex,
                    IsCalculated = d.IsCalculated,
                    WaitTime = d.WaitTime
                }).ToList(),
                PercentageLowBound = source.Backlog.PercentageLowBound,
                PercentageHighBound = source.Backlog.PercentageHighBound
            },
            Columns = source.Columns.Select(c => new ProjectRiskManagementSim.ProjectSimulation.Models.ColumnModel
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
