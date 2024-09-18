// See https://aka.ms/new-console-template for more information
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
    // 80 days beween start and target date
    StartDate = new DateTime(2024, 1, 1),
    TargetDate = new DateTime(2024, 3, 21),
    Revenue = new RevenueModel { Amount = 480000 },
    Costs = new CostModel { Cost = 177600, Days = 20 },
    Backlog = backLogModel,
    Columns = new List<ColumnModel>
    {
        new ColumnModel { Name = "To Do", WIP = 10, EstimatedLowBound = 1, EstimatedHighBound = 54 },
        new ColumnModel { Name = "In Progress", WIP = 2, EstimatedLowBound = 1, EstimatedHighBound = 47 },
        new ColumnModel { Name = "Testing", WIP = 5, EstimatedLowBound = 1, EstimatedHighBound = 52 },
        new ColumnModel { Name = "Done", WIP = 3, EstimatedLowBound = 1, EstimatedHighBound = 5 },    }
};

const int projectsCount = 1;
const int projectSimulationsCount = 10000;

// Create and run simulations concurrently
// Start timing the simulations
var stopwatch = Stopwatch.StartNew();

var tasks = new List<Task<(double, double, double)>>();
for (int i = 0; i < projectsCount; i++)
{
    tasks.Add(Task.Run(() =>
    {
        var MCS = new MonteCarloSimulation(projectSimModel, projectSimulationsCount);
        MCS.InitiateAndRunSimulation();

        // Get the average values, check if the lists are empty before calculating
        var totalDays = MCS.SimTotalDaysResult != null && MCS.SimTotalDaysResult.Any()
            ? MCS.SimTotalDaysResult.Average()
            : 0.0;

        var totalCosts = MCS.SimTotalCostsResult != null && MCS.SimTotalCostsResult.Any()
            ? MCS.SimTotalCostsResult.Average()
            : 0.0;

        var totalSales = MCS.SimTotalSalesResult != null && MCS.SimTotalSalesResult.Any()
            ? MCS.SimTotalSalesResult.Average()
            : 0.0;
        return (totalDays, totalCosts, totalSales);
    }));
};

// Wait for all tasks to finish and print results
var results = await Task.WhenAll(tasks);
for (int i = 0; i < results.Length; i++)
{
    // Deconstruct the tuple into individual variables
    var (totalDays, totalCosts, totalSales) = results[i];
    Console.WriteLine($"Simulation {i + 1} Results: ");
    Console.WriteLine($"  Total Days (Avg): {double.Round(totalDays)} compared to 80 days");
    Console.WriteLine($"  Total Costs (Avg): {double.Round(totalCosts)} compared to {projectSimModel.Costs.Cost}");
    Console.WriteLine($"  Total Sales (Avg): {double.Round(totalSales)} compared to {projectSimModel.Revenue.Amount}");
    Console.WriteLine();
    Console.WriteLine($"  Total Costs (Avg): {double.Round(totalCosts / totalDays)} compared to {projectSimModel.Costs.Cost / 80.0}");
    Console.WriteLine($"  Total Sales (Avg): {double.Round(totalSales / totalDays)} to {projectSimModel.Revenue.Amount / 80.0}");
}

// Stop timing
stopwatch.Stop();
// Output the elapsed time
Console.WriteLine($"Total time for all multi-threaded simulations: {stopwatch.ElapsedMilliseconds} ms");


