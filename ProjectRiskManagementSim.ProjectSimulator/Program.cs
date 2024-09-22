﻿// See https://aka.ms/new-console-template for more information
using System.Diagnostics;
using ProjectRiskManagementSim.ProjectSimulation;
using Dumpify;

var deliverableModel = new List<DeliverableModel>();
for (var i = 1; i < 36; i++)
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
    Name = "Baseline",
    Staff = new List<StaffModel>
    {
      new StaffModel { Name = "John Doe", Role = Role.ProjectManager, Sale = 1000, Cost = 370, Days = 20 },
      new StaffModel { Name = "Jane Doe", Role = Role.FrontendDeveloper, Sale = 1000, Cost = 370, Days = 20 },
      new StaffModel { Name = "Jack Doe", Role = Role.BackendDeveloper, Sale = 1000, Cost = 370, Days = 20 },
      new StaffModel { Name = "Jill Doe", Role = Role.SoftwareTester, Sale = 1000, Cost = 370, Days = 20 },
      new StaffModel { Name = "Jim Doe", Role = Role.UXUIDesigner, Sale = 1000, Cost = 370, Days = 20 }
    },
    // 74 days beween start and target date
    StartDate = new DateTime(2024, 1, 1),
    TargetDate = new DateTime(2024, 3, 15),
    Revenue = new RevenueModel { Amount = 480000 },
    Costs = new CostModel { Cost = 177600, Days = 20 },
    Backlog = backLogModel,
    Columns = new List<ColumnModel>
    {
        new ColumnModel { Name = "Backlog", IsBuffer=true, WIP = 20, EstimatedLowBound = 1, EstimatedHighBound = 54 },
        new ColumnModel { Name = "To Do", IsBuffer=true, WIP = 10, EstimatedLowBound = 1, EstimatedHighBound = 54 },
        new ColumnModel { Name = "In Progress", WIP = 2, EstimatedLowBound = 1, EstimatedHighBound = 23 },
        new ColumnModel { Name = "Finished", IsBuffer=true, WIP = 10, EstimatedLowBound = 1, EstimatedHighBound = 65 },
        new ColumnModel { Name = "Testing", WIP = 2, EstimatedLowBound = 1, EstimatedHighBound = 52 },
        new ColumnModel { Name = "Done", WIP = 3, EstimatedLowBound = 1, EstimatedHighBound = 5 },    }
};


const double estimateMultiplier = 0.5;

var projectWithModifiedEstimates = MonteCarloSimulation.SensitivityAnalysis(projectSimModel, estimateMultiplier);

// Loop through each column and modify WIP
// for (int i = 0; i < projectSimModel.Columns.Count; i++)
// {
//     var columnName = projectSimModel.Columns[i].Name! + " WIP";
//     // Clone the original model
//     var newProjectSimModel = projectSimModel.CloneProjectSimModel(projectSimModel, columnName);
//     // Modify the estimated bounds of the specific column
//     var columnToModify = newProjectSimModel.Columns[i];
//     columnToModify.WIP += 4;
//     projectWithModifiedEstimates.Add(newProjectSimModel);
// }

int projectsCount = projectWithModifiedEstimates.Count;
const int projectSimulationsCount = 1000;

// Create and run simulations concurrently
// Start timing the simulations
var stopwatch = Stopwatch.StartNew();

var tasks = new List<Task<MonteCarloSimulation>>();
foreach (var projectSimModels in projectWithModifiedEstimates)
{
    tasks.Add(Task.Run(() =>
    {
        var MCS = new MonteCarloSimulation(projectSimModels, projectSimulationsCount);
        MCS.InitiateAndRunSimulation();
        return MCS;
    }));
};

// Wait for all tasks to finish and print results
var results = await Task.WhenAll(tasks);
// Order the results by the average total days
var orderedByTotalDays = results.OrderBy(r => r.SimTotalDaysResult!.Average()).ToArray();

PrintResults(orderedByTotalDays, projectSimModel);

// Stop timing
stopwatch.Stop();
// Output the elapsed time
Console.WriteLine();
Console.WriteLine($"Total time for all multi-threaded simulations: {stopwatch.ElapsedMilliseconds} ms");

// var MCS = new MonteCarloSimulation(projectSimModel, projectSimulationsCount);
// MCS.PrintSimulationResults();

static void PrintResults(MonteCarloSimulation[] orderedByTotalDays, ProjectSimulationModel projectSimModel)
{
    for (int i = 0; i < orderedByTotalDays.Length; i++)
    {
        var MCS = orderedByTotalDays[i];
        var name = MCS.ProjectSimulationModel.Name;
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

        Console.Write($"Simulation {i + 1} - {name} Results: ");
        Console.WriteLine($"  Total Days (Avg): {double.Round(totalDays)}, compared to {MCS.NewDate.Subtract(projectSimModel.TargetDate).Days}");
        // Console.WriteLine($"  Total Costs (Avg): {double.Round(totalCosts)} compared to {projectSimModel.Costs.Cost}");
        // Console.WriteLine($"  Total Sales (Avg): {double.Round(totalSales)} compared to {projectSimModel.Revenue.Amount}");
        // Console.WriteLine();
    }

}
