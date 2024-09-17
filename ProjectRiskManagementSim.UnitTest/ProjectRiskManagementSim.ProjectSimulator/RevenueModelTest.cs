using ProjectRiskManagementSim.ProjectSimulation;

namespace ProjectRiskManagementSim.UnitTest;

public class RevenueModelTest
{
    [Fact]
    public void RevenueModel()
    {
        // Arrange
        var revenue = new RevenueModel
        {
            Amount = 1000,
            Date = new DateTime(2024, 1, 1),
        };

        // Act
        var amount = revenue.Amount;
        var date = revenue.Date;

        // Assert
        Assert.Equal(1000, amount);
        Assert.Equal(new DateTime(2024, 1, 1), date);
    }
}

