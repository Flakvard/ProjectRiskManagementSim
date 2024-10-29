namespace ProjectRiskManagementSim.DataAccess.Models;

public enum Role
{
    ProjectManager,
    FrontendDeveloper,
    BackendDeveloper,
    SoftwareTester,
    UXUIDesigner,
}
public class StaffModel
{
    public required string Name { get; set; }
    public Role Role { get; set; }
    public double Sale { get; set; }
    public double Cost { get; set; }
    public int Days { get; set; }
}
