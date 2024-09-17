// See https://aka.ms/new-console-template for more information
using ProjectRiskManagementSim.ProjectSimulation;

var deliverableModel = new List<DeliverableModel>();
for (var i = 0; i < 25; i++)
{
    deliverableModel.Add(new DeliverableModel { Id = Guid.NewGuid() });
}

var projectSimModel = new ProjectSimulationModel
{
    Staff = new List<StaffModel>
    {
      new StaffModel { Name = "John Doe", Role = Role.ProjectManager, Sale = 1000, Cost = 370, Days = 20 },
      new StaffModel { Name = "Jane Doe", Role = Role.FrontendDeveloper, Sale = 1000, Cost = 370, Days = 20 },
      new StaffModel { Name = "Jack Doe", Role = Role.BackendDeveloper, Sale = 1000, Cost = 370, Days = 20 },
      new StaffModel { Name = "Jill Doe", Role = Role.SoftwareTester, Sale = 1000, Cost = 370, Days = 20 },
      new StaffModel { Name = "Jim Doe", Role = Role.UXUIDesigner, Sale = 1000, Cost = 370, Days = 20 }
    },
    StartDate = new DateTime(2024, 1, 1),
    TargetDate = new DateTime(2024, 8, 31),
    Revenue = new RevenueModel { Amount = 1000 },
    Costs = new CostModel { Cost = 370, Days = 20 },
    Deliverables = deliverableModel
};

var MCS = new MonteCarloSimulation(projectSimModel);

MCS.InitiateSimulation();

