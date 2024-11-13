// See https://aka.ms/new-console-template for more information
using System.Diagnostics;
using ProjectRiskManagementSim.ProjectSimulation.Models;
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
    PercentageHighBound = 0.8,
};

var wip = 30;
var wip10 = 10;
var wip5 = 5;
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
    // Bech Bruun model
    Columns = new List<ColumnModel>
    {
        new ColumnModel(wip: backLogModel.Deliverables.Count) { Name = "Backlog", IsBuffer=true, EstimatedLowBound = 1, EstimatedHighBound = 54 },
        new ColumnModel(wip: wip, wipMax: wip) { Name = "Open", IsBuffer=true,  EstimatedLowBound = 1, EstimatedHighBound = 54 },
        new ColumnModel(wip: 5, wipMax: 5) { Name = "In Progress", EstimatedLowBound = 1, EstimatedHighBound = 47 },
        new ColumnModel(wip: wip, wipMax: wip) { Name = "Stuck", IsBuffer=true, EstimatedLowBound = 1, EstimatedHighBound = 50 },
        new ColumnModel(wip: wip, wipMax: wip) { Name = "Finish", IsBuffer=true, EstimatedLowBound = 1, EstimatedHighBound = 50 },
        new ColumnModel(wip: wip, wipMax: wip) { Name = "Ready to test on Development", IsBuffer=true, EstimatedLowBound = 1, EstimatedHighBound = 50 },
        new ColumnModel(wip: 0, wipMax: 2) { Name = "Testing on Development", EstimatedLowBound = 1, EstimatedHighBound = 11 },
        new ColumnModel(wip: wip, wipMax: wip) { Name = "Waiting Deployment on Production", IsBuffer=true, EstimatedLowBound = 1, EstimatedHighBound = 22 },
        new ColumnModel(wip: 0, wipMax: 2) { Name = "Ready to test on Production", EstimatedLowBound = 1, EstimatedHighBound = 54 },
        new ColumnModel(wip: backLogModel.Deliverables.Count) { Name = "Done", IsBuffer=true, EstimatedLowBound = 1, EstimatedHighBound = 54 }
    },
    // Columns = new List<ColumnModel>
    // {
    //     new ColumnModel(wip: backLogModel.Deliverables.Count) { Name = "Backlog", IsBuffer=true, EstimatedLowBound = 1, EstimatedHighBound = 54 },
    //     new ColumnModel(wip: wip, wipMax: wip) { Name = "Open", IsBuffer=true,  EstimatedLowBound = 1, EstimatedHighBound = 54 },
    //     new ColumnModel(wip: 5, wipMax: 5) { Name = "In Progress", EstimatedLowBound = 1, EstimatedHighBound = 47 },
    //     new ColumnModel(wip: wip, wipMax: wip) { Name = "Stuck", IsBuffer=true, EstimatedLowBound = 1, EstimatedHighBound = 24.79 },
    //     new ColumnModel(wip: wip, wipMax: wip) { Name = "Finished", IsBuffer=true, EstimatedLowBound = 1, EstimatedHighBound = 65 },
    //     new ColumnModel(wip: wip, wipMax: wip) { Name = "Rdy4Test", IsBuffer=true, EstimatedLowBound = 1, EstimatedHighBound = 50 },
    //     new ColumnModel(wip: 0, wipMax: 2) { Name = "Test Stage", EstimatedLowBound = 1, EstimatedHighBound = 11 },
    //     new ColumnModel(wip: wip, wipMax: wip) { Name = "Await Dply Prod", IsBuffer=true, EstimatedLowBound = 1, EstimatedHighBound = 22 },
    //     new ColumnModel(wip: 0, wipMax: 2) { Name = "Rdy4TestProd", EstimatedLowBound = 1, EstimatedHighBound = 54 },
    //     new ColumnModel(wip: backLogModel.Deliverables.Count) { Name = "Done", IsBuffer=true, EstimatedLowBound = 1, EstimatedHighBound = 54 }
    // },
};

// setting constant for the estimate multiplier

// getting all the projects with modified estimates

//int projectsCount = projectWithModifiedEstimates.Count;
const int projectSimulationsCount = 100;

// Start timing the simulations
var stopwatch = Stopwatch.StartNew();
// SensitiveAnalysis
var MCS = new MonteCarloSimulation();
Console.WriteLine("Column Estimate Analysis");
await MCS.ColumnEstimateAnalysis(projectSimModel, projectSimulationsCount);
Console.WriteLine();
Console.WriteLine();
Console.WriteLine("WIP Analysis");
await MCS.WIPAnalysis(projectSimModel, projectSimulationsCount);
Console.WriteLine();
Console.WriteLine();
Console.WriteLine("Block WIP Analysis");
await MCS.BlockWIPAnalysis(projectSimModel, projectSimulationsCount);
Console.WriteLine();
Console.WriteLine();
// Stop timing
stopwatch.Stop();

// Output the elapsed time
Console.WriteLine();
Console.WriteLine($"Total time for all multi-threaded simulations: {stopwatch.ElapsedMilliseconds} ms");

// var MCS = new MonteCarloSimulation(projectSimModel, projectSimulationsCount);
// MCS.PrintSimulationResults(projectSimModel, projectSimulationsCount);
//
// PrintResults(orderedByTotalDays, projectSimModel);

static void PrintResults(MonteCarloSimulation[] orderedByTotalDays, ProjectSimulationModel projectSimModel)
{
    var baseLine = orderedByTotalDays.Where(mcs => mcs.ProjectSimulationModel.Name == "Baseline").FirstOrDefault();
    for (int i = 0; i < orderedByTotalDays.Length; i++)
    {
        var MCS = orderedByTotalDays[i];
        var name = MCS.ProjectSimulationModel.Name;
        // Get the average values, check if the lists are empty before calculating
        var totalDays = MCS.SimTotalDaysResult != null && MCS.SimTotalDaysResult.Any()
            ? MCS.SimTotalDaysResult.Percentile(0.9)
            : 0.0;

        var totalCosts = MCS.SimTotalCostsResult != null && MCS.SimTotalCostsResult.Any()
            ? MCS.SimTotalCostsResult.Percentile(0.9)
            : 0.0;

        var totalSales = MCS.SimTotalSalesResult != null && MCS.SimTotalSalesResult.Any()
            ? MCS.SimTotalSalesResult.Percentile(0.9)
            : 0.0;

        var totalDaysDiff = totalDays - baseLine!.SimTotalDaysResult!.Percentile(0.9);
        if (totalDaysDiff < -1)
        {
            Console.WriteLine($"Simulation {i + 1}\t- {name} Results:");
            //Console.WriteLine($"\t\tTotal Days (Percentile 90%):\t{double.Round(totalDays)}, compared to\t{baseLine!.SimTotalDaysResult!.Percentile(0.9)}\t(+{double.Round(totalDaysDiff)})");
            Console.WriteLine($"\t\tTotal Days (Percentile 90%):\t{double.Round(totalDays)}, compared to\t{MCS.NewDate.Subtract(baseLine!.NewDate).Days}");
        }
        else
        {
        }
        // Console.WriteLine($"  Total Costs (Avg): {double.Round(totalCosts)} compared to {projectSimModel.Costs.Cost}");
        // Console.WriteLine($"  Total Sales (Avg): {double.Round(totalSales)} compared to {projectSimModel.Revenue.Amount}");
        // Console.WriteLine();
    }

}



