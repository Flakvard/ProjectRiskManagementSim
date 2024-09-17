namespace ProjectRiskManagementSim.ProjectSimulation;

internal enum Role
{
    ProjectManager,
    FrontendDeveloper,
    BackendDeveloper,
    SoftwareTester,
    UXUIDesigner,
}
internal class StaffModel
{
    public required string Name { get; set; }
    public Role Role { get; set; }
    public double Sale { get; set; }
    public double Cost { get; set; }
    public int Days { get; set; }
}
