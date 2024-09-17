// See https://aka.ms/new-console-template for more information
using ProjectRiskManagementSim.ProjectSimulation;

Console.WriteLine("Hello, World!");

var projectSimModel = new ProjectSimulationModel
{
    Staff = new List<StaffModel>
    {
      new StaffModel { Name = "John Doe", Role = Role.ProjectManager, Cost = 1000, Days = 20 },
      new StaffModel { Name = "Jane Doe", Role = Role.FrontendDeveloper, Cost = 800, Days = 20 },
      new StaffModel { Name = "Jack Doe", Role = Role.BackendDeveloper, Cost = 800, Days = 20 },
      new StaffModel { Name = "Jill Doe", Role = Role.SoftwareTester, Cost = 800, Days = 20 },
      new StaffModel { Name = "Jim Doe", Role = Role.UXUIDesigner, Cost = 800, Days = 20 }
    },
    StartDate = new DateTime(2024, 1, 1),
    TargetDate = new DateTime(2024, 8, 31),
    Revenue = new RevenueModel { Amount = 1000 }
};

var MCS = new MonteCarloSimulation(projectSimModel);

MCS.RunSimulation();

