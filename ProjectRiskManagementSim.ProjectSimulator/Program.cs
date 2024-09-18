// See https://aka.ms/new-console-template for more information
using Dumpify;
using System.Diagnostics;
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

// Start timing the simulations
Stopwatch stopwatch = Stopwatch.StartNew();
//
//
const int projectsCount = 10;
const int projectSimulationsCount = 10000;
// Create and run simulations sequentially
for (int i = 0; i < projectsCount; i++)
{
    var MCS = new MonteCarloSimulation(projectSimModel, projectSimulationsCount);
    MCS.InitiateAndRunSimulation();
    Console.WriteLine($"Simulation {i + 1} Average: {MCS.SimResult.Average()}");
}

// simulationsList.Dump();
Console.WriteLine("");
// Stop timing
stopwatch.Stop();
// Output the elapsed time
Console.WriteLine($"Total time for all single-threaded simulations: {stopwatch.ElapsedMilliseconds} ms");

// Start timing the simulations
stopwatch = Stopwatch.StartNew();

// Create and run simulations concurrently
var tasks = new List<Task<double>>();
for (int i = 0; i < projectsCount; i++)
{
    tasks.Add(Task.Run(() =>
    {
        var MCS = new MonteCarloSimulation(projectSimModel, projectSimulationsCount);
        MCS.InitiateAndRunSimulation();
        return MCS.SimResult.Average();
    }));
};

// Wait for all tasks to finish and print results
var results = await Task.WhenAll(tasks);
for (int i = 0; i < results.Length; i++)
{
    Console.WriteLine($"Simulation {i + 1} Average: {results[i]}");
}

// Stop timing
stopwatch.Stop();
// Output the elapsed time
Console.WriteLine($"Total time for all multi-threaded simulations: {stopwatch.ElapsedMilliseconds} ms");
// foreach (var simulation in simulationsResult)
// {
//     Console.WriteLine(simulation);
// }


