// See https://aka.ms/new-console-template for more information
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
    // Bech Bruun model
    Columns = new List<ColumnModel>
    {
        new ColumnModel { Name = "Backlog", IsBuffer=true, WIP = backLogModel.Deliverables.Count, EstimatedLowBound = 1, EstimatedHighBound = 54 },
        new ColumnModel { Name = "Open", IsBuffer=true, WIP = 7, EstimatedLowBound = 1, EstimatedHighBound = 54 },
        new ColumnModel { Name = "In Progress", WIP = 3, EstimatedLowBound = 1, EstimatedHighBound = 47 },
        new ColumnModel { Name = "Stuck", IsBuffer=true, WIP = 12, EstimatedLowBound = 1, EstimatedHighBound = 24.79 },
        new ColumnModel { Name = "Finished", IsBuffer=true, WIP = 35, EstimatedLowBound = 1, EstimatedHighBound = 65 },
        new ColumnModel { Name = "Rdy For Test on Stage", IsBuffer=true, WIP = 21, EstimatedLowBound = 1, EstimatedHighBound = 50 },
        new ColumnModel { Name = "Test on Stage", WIP = 2, EstimatedLowBound = 1, EstimatedHighBound = 11 },
        new ColumnModel { Name = "Await Dply on Prod", IsBuffer=true, WIP = 21, EstimatedLowBound = 1, EstimatedHighBound = 22 },
        new ColumnModel { Name = "Rdy For Test on Prod", WIP = 2, EstimatedLowBound = 1, EstimatedHighBound = 54 },
        new ColumnModel { Name = "Done", IsBuffer=true, WIP = backLogModel.Deliverables.Count, EstimatedLowBound = 1, EstimatedHighBound = 54 }
    },
};

// setting constant for the estimate multiplier
// const double estimateMultiplier = 0.5;

// getting all the projects with modified estimates
// var projectWithModifiedEstimates = MonteCarloSimulation.ProjectEstimateMultiplier(projectSimModel, estimateMultiplier);

// SensitiveAnalysis
//int projectsCount = projectWithModifiedEstimates.Count;
const int projectSimulationsCount = 1000;

// Start timing the simulations
var stopwatch = Stopwatch.StartNew();

// Rerun the simulation with the modified WIP and get the Results
for (int k = 1; k <= 5; k++)
{

    // setting constant for the estimate multiplier
    int wipMultiplier = k;

    // getting all the projects with modified estimates
    var projectWithModifiedWIP = new List<ProjectSimulationModel>();
    projectWithModifiedWIP.Add(projectSimModel);
    ProjectWIPMultiplier(projectSimModel, projectWithModifiedWIP, wipMultiplier);

    int projectsCount = projectWithModifiedWIP.Count;

    //var orderedByTotalDays = await RunSimulationAsync(projectWithModifiedEstimates, projectSimulationsCount);
    var orderedByTotalDays = await RunSimulationAsync(projectWithModifiedWIP, projectSimulationsCount);

    var baselineProject = orderedByTotalDays.Where(mcs => mcs.ProjectSimulationModel.Name == projectSimModel.Name).FirstOrDefault();
    var baselineTotalDays = baselineProject!.SimTotalDaysResult!.Percentile(0.9);
    var simulationWithLowestTotalDays = baselineTotalDays;
    var simulationWithLowestTotalDaysIndex = baselineProject;

    // Column with most impact on the project duration
    for (int i = 0; i < orderedByTotalDays.Count(); i++)
    {
        var MCS = orderedByTotalDays[i];
        var name = MCS.ProjectSimulationModel.Name;
        var totalDays = MCS.SimTotalDaysResult != null && MCS.SimTotalDaysResult.Any()
            ? MCS.SimTotalDaysResult.Percentile(0.9)
            : 0.0;
        if (totalDays <= baselineTotalDays && totalDays != 0.0)
        {
            simulationWithLowestTotalDays = totalDays;
            simulationWithLowestTotalDaysIndex = orderedByTotalDays[i];
        }
    }
    // print the simulation with the lowest total Days
    Console.WriteLine($"Simulation with the lowest total days: {simulationWithLowestTotalDaysIndex.ProjectSimulationModel.Name} is {simulationWithLowestTotalDays} days");
    projectSimModel = simulationWithLowestTotalDaysIndex.ProjectSimulationModel;

}
// PrintResults(orderedByTotalDays, projectSimModel);

// Stop timing
stopwatch.Stop();
// Output the elapsed time
Console.WriteLine();
Console.WriteLine($"Total time for all multi-threaded simulations: {stopwatch.ElapsedMilliseconds} ms");

// var MCS = new MonteCarloSimulation(projectSimModel, projectSimulationsCount);
// MCS.PrintSimulationResults();

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

static void ProjectWIPMultiplier(ProjectSimulationModel projectSimModel, List<ProjectSimulationModel> projectWithModifiedEstimates, int multiplier)
{
    // Loop through each column and modify WIP
    for (int i = 0; i < projectSimModel.Columns.Count; i++)
    {
        var columnName = projectSimModel.Columns[i].Name! + " WIP";
        // Clone the original model
        var newProjectSimModel = projectSimModel.CloneProjectSimModel(projectSimModel, columnName);
        // Modify the estimated bounds of the specific column
        var columnToModify = newProjectSimModel.Columns[i];
        columnToModify.WIP += multiplier;
        projectWithModifiedEstimates.Add(newProjectSimModel);
    }
}

static async Task<MonteCarloSimulation[]> RunSimulationAsync(List<ProjectSimulationModel> projectWithModifiedEstimates, int projectSimulationsCount)
{
    // Create and run simulations concurrently
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
    var orderedByTotalDays = results.OrderBy(r => r.SimTotalDaysResult!.Percentile(0.9)).ToArray();
    return orderedByTotalDays;
}
