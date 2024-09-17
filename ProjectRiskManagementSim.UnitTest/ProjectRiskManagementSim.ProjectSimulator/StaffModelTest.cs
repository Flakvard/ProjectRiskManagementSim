using ProjectRiskManagementSim.ProjectSimulation;

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
            Cost = 1000
        };

        // Act
        var name = staff.Name;
        var role = staff.Role;
        var cost = staff.Cost;

        // Assert
        Assert.Equal("John Doe", name);
        Assert.Equal(Role.ProjectManager, role);
        Assert.Equal(1000, cost);
    }
}

