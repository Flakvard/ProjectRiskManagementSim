// See https://aka.ms/new-console-template for more information
using Dumpify;
using ProjectRiskManagementSim.ProjectSimulation;

var deliverableModel = new List<DeliverableModel>();
for (var i = 0; i < 10; i++)
{
    deliverableModel.Add(new DeliverableModel { Id = Guid.NewGuid(), Nr = i });
}
var backLogModel = new BacklogModel
{
    Deliverables = deliverableModel,
    PercentageLowBound = 0.5,
    PercentageHighBound = 0.8
};

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
    Backlog = backLogModel,
    Columns = new List<ColumnModel>
    {
        new ColumnModel { Name = "To Do", WIP = 10, EstimatedLowBound = 1, EstimatedHighBound = 54 },
        new ColumnModel { Name = "In Progress", WIP = 2, EstimatedLowBound = 1, EstimatedHighBound = 47 },
        new ColumnModel { Name = "Testing", WIP = 5, EstimatedLowBound = 1, EstimatedHighBound = 52 },
        new ColumnModel { Name = "Done", WIP = 3, EstimatedLowBound = 1, EstimatedHighBound = 5 },    }
};


var MCS = new MonteCarloSimulation(projectSimModel, 1000);
MCS.InitiateSimulation();
var simulationsResult = MCS.SimResult;
var simulationsList = MCS.Simulations;
// simulationsList.Dump();

Console.WriteLine("");
Console.WriteLine(simulationsResult.Average());

// foreach (var simulation in simulationsResult)
// {
//     Console.WriteLine(simulation);
// }


