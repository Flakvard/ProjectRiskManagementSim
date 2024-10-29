using ProjectRiskManagementSim.ProjectSimulation;
using ProjectRiskManagementSim.ProjectSimulation.Models;

namespace ProjectRiskManagementSim.UnitTest;

public class CostModelTest
{
    [Fact]
    public void CostModel()
    {
        // Arrange
        var Cost = new CostModel
        {
            Cost = 1000,
            Days = 20,
        };

        // Act
        var cost = Cost.Cost;
        var days = Cost.Days;

        // Assert
        Assert.Equal(1000, cost);
        Assert.Equal(20, days);
    }
}

