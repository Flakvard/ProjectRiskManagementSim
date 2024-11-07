namespace ProjectRiskManagementSim.SimulationBlazor.Models;
public static class ModelMapper
{
    public static ProjectRiskManagementSim.ProjectSimulation.Models.ProjectSimulationModel MapToSimProjProjectSimulationModel(ProjectRiskManagementSim.SimulationBlazor.Models.ViewProjectSimulationModel source)
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
                WIPMax = c.WIPMax,
                IsBuffer = c.IsBuffer
            }).ToList()
        };
    }

    public static ProjectRiskManagementSim.ProjectSimulation.Models.ColumnModel MapToSimProjColumnModel(ProjectRiskManagementSim.SimulationBlazor.Models.ViewColumnModel source)
    {
        return new ProjectRiskManagementSim.ProjectSimulation.Models.ColumnModel
        {
            Id = source.Id,
            Name = source.Name,
            EstimatedLowBound = source.EstimatedLowBound,
            EstimatedHighBound = source.EstimatedHighBound,
            WIP = source.WIP,
            WIPMax = source.WIPMax,
            IsBuffer = source.IsBuffer
        };
    }

    public static ProjectRiskManagementSim.ProjectSimulation.Models.DeliverableModel MapToSimProjDeliverableModel(ProjectRiskManagementSim.SimulationBlazor.Models.DeliverableModel source)
    {
        return new ProjectRiskManagementSim.ProjectSimulation.Models.DeliverableModel
        {
            Id = source.Id,
            Nr = source.Nr,
            CompletionDays = source.CompletionDays,
            AccumulatedDays = source.AccumulatedDays,
            ColumnIndex = source.ColumnIndex,
            IsCalculated = source.IsCalculated,
            WaitTime = source.WaitTime,
            StoppedWorkingTime = source.StoppedWorkingTime
        };
    }

    public static ProjectRiskManagementSim.SimulationBlazor.Models.ViewColumnModel MapToBlazorColumnModel(ProjectRiskManagementSim.ProjectSimulation.Models.ColumnModel source)
    {
        return new ProjectRiskManagementSim.SimulationBlazor.Models.ViewColumnModel
        {
            Id = source.Id,
            Name = source.Name,
            EstimatedLowBound = source.EstimatedLowBound,
            EstimatedHighBound = source.EstimatedHighBound,
            WIP = source.WIP,
            WIPMax = source.WIPMax,
            IsBuffer = source.IsBuffer
        };
    }

    public static ProjectRiskManagementSim.SimulationBlazor.Models.DeliverableModel MapToBlazorDeliverableModel(ProjectRiskManagementSim.ProjectSimulation.Models.DeliverableModel source)
    {
        return new ProjectRiskManagementSim.SimulationBlazor.Models.DeliverableModel
        {
            Id = source.Id,
            Nr = source.Nr,
            CompletionDays = source.CompletionDays,
            AccumulatedDays = source.AccumulatedDays,
            ColumnIndex = source.ColumnIndex,
            IsCalculated = source.IsCalculated,
            WaitTime = source.WaitTime,
            StoppedWorkingTime = source.StoppedWorkingTime
        };
    }

    public static ProjectRiskManagementSim.SimulationBlazor.Models.ViewProjectSimulationModel MapToBlazorProjectSimulationModel(ProjectRiskManagementSim.ProjectSimulation.Models.ProjectSimulationModel source)
    {
        return new ProjectRiskManagementSim.SimulationBlazor.Models.ViewProjectSimulationModel
        {
            Name = source.Name,
            Staff = source.Staff?.Select(s => new ProjectRiskManagementSim.SimulationBlazor.Models.StaffModel
            {
                Name = s.Name,
                Role = (ProjectRiskManagementSim.SimulationBlazor.Models.Role)s.Role,
                Sale = s.Sale,
                Cost = s.Cost,
                Days = s.Days
            }).ToList(),
            StartDate = source.StartDate,
            TargetDate = source.TargetDate,
            Revenue = new ProjectRiskManagementSim.SimulationBlazor.Models.RevenueModel { Amount = source.Revenue.Amount },
            Costs = new ProjectRiskManagementSim.SimulationBlazor.Models.CostModel { Cost = source.Costs.Cost, Days = source.Costs.Days },
            Backlog = new ProjectRiskManagementSim.SimulationBlazor.Models.BacklogModel
            {
                Deliverables = source.Backlog.Deliverables.Select(d => new ProjectRiskManagementSim.SimulationBlazor.Models.DeliverableModel
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
            Columns = source.Columns.Select(c => new ProjectRiskManagementSim.SimulationBlazor.Models.ViewColumnModel
            {
                Name = c.Name,
                EstimatedLowBound = c.EstimatedLowBound,
                EstimatedHighBound = c.EstimatedHighBound,
                WIP = c.WIP,
                WIPMax = c.WIPMax,
                IsBuffer = c.IsBuffer
            }).ToList()
        };
    }
    public static ProjectRiskManagementSim.ProjectSimulation.Models.ProjectSimulationModel MapDBProjectSimulationModelToProjectSimulationModel(ProjectRiskManagementSim.DataAccess.Models.ProjectSimulationModel source)
    {
        List<ProjectRiskManagementSim.ProjectSimulation.Models.DeliverableModel> deliverables = new();
        for (var i = 0; i < source.DeliverablesCount; i++)
        {
            var dev = new ProjectRiskManagementSim.ProjectSimulation.Models.DeliverableModel
            {
                Id = new Guid(),
                Nr = i,
            };
            deliverables.Add(dev);
        }

        var staff = new List<ProjectRiskManagementSim.ProjectSimulation.Models.StaffModel>();
        for (var i = 1; i <= source.BackendDevs; i++)
        {
            staff.Add(new ProjectRiskManagementSim.ProjectSimulation.Models.StaffModel
            {
                Name = $"Backend Dev {i}",
                Role = ProjectRiskManagementSim.ProjectSimulation.Models.Role.BackendDeveloper,
                Sale = 1000,
                Cost = 370,
                Days = 20
            });
        }
        for (var i = 1; i <= source.FrontendDevs; i++)
        {
            staff.Add(new ProjectRiskManagementSim.ProjectSimulation.Models.StaffModel
            {
                Name = $"Frontend Dev {i}",
                Role = ProjectRiskManagementSim.ProjectSimulation.Models.Role.FrontendDeveloper,
                Sale = 1000,
                Cost = 370,
                Days = 20
            });
        }
        for (var i = 1; i <= source.Testers; i++)
        {
            staff.Add(new ProjectRiskManagementSim.ProjectSimulation.Models.StaffModel
            {
                Name = $"Tester {i}",
                Role = ProjectRiskManagementSim.ProjectSimulation.Models.Role.SoftwareTester,
                Sale = 1000,
                Cost = 370,
                Days = 20
            });
        }

        return new ProjectRiskManagementSim.ProjectSimulation.Models.ProjectSimulationModel
        {
            Name = source.Name != null ? source.Name : "",
            StartDate = source.StartDate,
            TargetDate = source.TargetDate,
            Revenue = new ProjectRiskManagementSim.ProjectSimulation.Models.RevenueModel { Amount = (double)source.ActualRevenue },
            Costs = new ProjectRiskManagementSim.ProjectSimulation.Models.CostModel { Cost = (double)source.BudgetCosts, Days = source.TargetDays },
            Staff = staff,
            Backlog = new ProjectRiskManagementSim.ProjectSimulation.Models.BacklogModel
            {
                Deliverables = deliverables,
                PercentageLowBound = source.PercentageLowBound,
                PercentageHighBound = source.PercentageHighBound
            },
            Columns = source.Columns.Select(c => new ProjectRiskManagementSim.ProjectSimulation.Models.ColumnModel
            {
                Name = c.Name,
                EstimatedLowBound = c.EstimatedLowBound,
                EstimatedHighBound = c.EstimatedHighBound,
                WIP = c.WIP,
                WIPMax = c.WIPMax,
                IsBuffer = c.IsBuffer
            }).ToList()
        };
    }
    public static ProjectRiskManagementSim.SimulationBlazor.Models.ViewProjectSimulationModel MapDBProjectSimulationModelToViewProjectSimulationModel(ProjectRiskManagementSim.DataAccess.Models.ProjectSimulationModel source)
    {
        List<ProjectRiskManagementSim.SimulationBlazor.Models.DeliverableModel> deliverables = new();
        for (var i = 0; i < source.DeliverablesCount; i++)
        {
            var dev = new ProjectRiskManagementSim.SimulationBlazor.Models.DeliverableModel
            {
                Id = new Guid(),
                Nr = i,
            };
            deliverables.Add(dev);
        }

        var staff = new List<StaffModel>();
        for (var i = 1; i <= source.BackendDevs; i++)
        {
            staff.Add(new StaffModel
            {
                Name = $"Backend Dev {i}",
                Role = Role.BackendDeveloper,
                Sale = 1000,
                Cost = 370,
                Days = 20
            });
        }
        for (var i = 1; i <= source.FrontendDevs; i++)
        {
            staff.Add(new StaffModel
            {
                Name = $"Frontend Dev {i}",
                Role = Role.FrontendDeveloper,
                Sale = 1000,
                Cost = 370,
                Days = 20
            });
        }
        for (var i = 1; i <= source.Testers; i++)
        {
            staff.Add(new StaffModel
            {
                Name = $"Tester {i}",
                Role = Role.SoftwareTester,
                Sale = 1000,
                Cost = 370,
                Days = 20
            });
        }


        return new ProjectRiskManagementSim.SimulationBlazor.Models.ViewProjectSimulationModel
        {
            Name = source.Name != null ? source.Name : "",
            StartDate = source.StartDate,
            TargetDate = source.TargetDate,
            Revenue = new ProjectRiskManagementSim.SimulationBlazor.Models.RevenueModel { Amount = (double)source.ActualRevenue },
            Costs = new ProjectRiskManagementSim.SimulationBlazor.Models.CostModel { Cost = (double)source.BudgetCosts, Days = source.TargetDays },
            Staff = staff,
            Backlog = new ProjectRiskManagementSim.SimulationBlazor.Models.BacklogModel
            {
                Deliverables = deliverables,
                PercentageLowBound = source.PercentageLowBound,
                PercentageHighBound = source.PercentageHighBound
            },
            Columns = source.Columns.Select(c => new ProjectRiskManagementSim.SimulationBlazor.Models.ViewColumnModel
            {
                Name = c.Name,
                EstimatedLowBound = c.EstimatedLowBound,
                EstimatedHighBound = c.EstimatedHighBound,
                WIP = c.WIP,
                WIPMax = c.WIPMax,
                IsBuffer = c.IsBuffer
            }).ToList()
        };
    }
}

