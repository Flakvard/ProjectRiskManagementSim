using ProjectRiskManagementSim.ProjectSimulation;
using ProjectRiskManagementSim.ProjectSimulation.Models;

namespace ProjectRiskManagementSim.UnitTest;

public class StaffModelTest
{
    [Fact]
    public void StaffModel()
    {
        // Arrange
        var staff = new StaffModel
        {
            Name = "John Doe",
            Role = Role.ProjectManager,
            Sale = 1000,
            Cost = 370,
            Days = 20
        };

        // Act
        var name = staff.Name;
        var role = staff.Role;
        var cost = staff.Cost;
        var sale = staff.Sale;
        var days = staff.Days;

        // Assert
        Assert.Equal("John Doe", name);
        Assert.Equal(Role.ProjectManager, role);
        Assert.Equal(1000, sale);
        Assert.Equal(370, cost);
        Assert.Equal(20, days);
    }
}

