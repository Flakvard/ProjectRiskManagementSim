using ProjectRiskManagementSim.ProjectSimulation;

namespace ProjectRiskManagementSim.UnitTest;

public class ProjectSimulationModelTest
{
    [Fact]
    public void ProjectSimulationModel()
    {
        // Arrange
        var projectSimModel = new ProjectSimulationModel
        {
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
            }
        };

        // Act
        var staff = projectSimModel.Staff[0].Name;
        var role = projectSimModel.Staff[0].Role;
        var cost = projectSimModel.Staff[0].Cost;
        var startDate = projectSimModel.StartDate;
        var targetDate = projectSimModel.TargetDate;
        var amount = projectSimModel.Revenue.Amount;
        var date = projectSimModel.Revenue.Date;


        // Assert
        Assert.Equal("John Doe", staff);
        Assert.Equal(Role.ProjectManager, role);
        Assert.Equal(1000, cost);
        Assert.Equal(new DateTime(2024, 1, 1), startDate);
        Assert.Equal(new DateTime(2024, 12, 31), targetDate);
        Assert.Equal(1000, amount);
        Assert.Equal(new DateTime(2024, 1, 1), date);
    }
}

