using ProjectRiskManagementSim.ProjectSimulation;
using ProjectRiskManagementSim.ProjectSimulation.Models;

namespace ProjectRiskManagementSim.UnitTest;

public class ProjectSimulationModelTest
{
    [Fact]
    public void ProjectSimulationModel()
    {
        // Arrange
        var deliverableModel = new List<DeliverableModel>();
        for (var i = 0; i < 25; i++)
        {
            deliverableModel.Add(new DeliverableModel { Id = Guid.NewGuid() });
        }
        var backlog = new BacklogModel
        {
            Deliverables = deliverableModel,
            PercentageLowBound = 0.5,
            PercentageHighBound = 0.8
        };
        var projectSimModel = new ProjectSimulationModel
        {
            Name = "Baseline",
            Staff = new List<StaffModel>
            {
                new StaffModel
                {
                    Name = "John Doe",
                    Role = Role.ProjectManager,
                    Cost = 1000
                }
            },
            StartDate = new DateTime(2024, 1, 1),
            TargetDate = new DateTime(2024, 12, 31),
            Revenue = new RevenueModel
            {
                Amount = 1000,
                Date = new DateTime(2024, 1, 1),
            },
            Costs = new CostModel
            {
                Cost = 1000,
                Days = 20
            },
            Columns = new List<ColumnModel>
            {
                new ColumnModel
                {
                    Name = "To Do",
                    WIP = 10,
                    EstimatedLowBound = 1,
                    EstimatedHighBound = 54
                },
                new ColumnModel
                {
                    Name = "In Progress",
                    WIP = 2,
                    EstimatedLowBound = 1,
                    EstimatedHighBound = 47
                },
                new ColumnModel
                {
                    Name = "Testing",
                    WIP = 5,
                    EstimatedLowBound = 1,
                    EstimatedHighBound = 52
                },
                new ColumnModel
                {
                    Name = "Done",
                    WIP = 3,
                    EstimatedLowBound = 1,
                    EstimatedHighBound = 5
                }
            },
            Backlog = backlog,
        };

        // Act
        var staff = projectSimModel.Staff[0].Name;
        var role = projectSimModel.Staff[0].Role;
        var cost = projectSimModel.Staff[0].Cost;
        var startDate = projectSimModel.StartDate;
        var targetDate = projectSimModel.TargetDate;
        var amount = projectSimModel.Revenue.Amount;
        var date = projectSimModel.Revenue.Date;
        var deliverables = projectSimModel.Backlog.Deliverables;


        // Assert
        Assert.Equal("John Doe", staff);
        Assert.Equal(Role.ProjectManager, role);
        Assert.Equal(1000, cost);
        Assert.Equal(new DateTime(2024, 1, 1), startDate);
        Assert.Equal(new DateTime(2024, 12, 31), targetDate);
        Assert.Equal(1000, amount);
        Assert.Equal(new DateTime(2024, 1, 1), date);
        Assert.Equal(25, deliverables.Count);
    }
}

