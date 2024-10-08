using System.Linq;
using Dumpify;

namespace ProjectRiskManagementSim.ProjectSimulation;

internal class MonteCarloSimulation
{
    public readonly ProjectSimulationModel ProjectSimulationModel;
    private readonly Random _random = new Random();
    public List<double>? SimTotalDaysResult { get; set; }
    public List<double>? SimTotalCostsResult { get; set; }
    public List<double>? SimTotalSalesResult { get; set; }
    public List<List<DeliverableModel>>? Simulations { get; set; }
    public DateTime NewDate { get; set; }
    private readonly int _simulationCount;

    public MonteCarloSimulation(ProjectSimulationModel projectSimulationModel,
                                int simulationCount)
    {
        ProjectSimulationModel = projectSimulationModel;
        _simulationCount = simulationCount;
    }

    public void InitiateAndRunSimulation()
    {
        var staff = ProjectSimulationModel.Staff;
        var startDate = ProjectSimulationModel.StartDate;
        var targetDate = ProjectSimulationModel.TargetDate;
        var revenue = ProjectSimulationModel.Revenue;
        var cost = ProjectSimulationModel.Costs;
        var backlog = ProjectSimulationModel.Backlog;
        var columns = ProjectSimulationModel.Columns;

        ValidateProps(staff, revenue, cost, backlog);

        var totalDays = (targetDate - startDate).TotalDays;

        var totalRevenue = 0.0;
        var totalCost = 0.0;
        for (var i = 0; i < staff?.Count; i++)
        {
            var staffMember = staff[i];
            var staffDays = staffMember.Days;
            var staffRevenue = staffMember.Sale * staffDays;
            var staffCost = staffMember.Cost * staffDays;
            totalRevenue += staffRevenue;
            totalCost += staffCost;
        }
        var estimatedRevenuePerDay = revenue.Amount / totalDays;
        var estimatedCostPerDay = cost.Cost / totalDays;

        // Console.WriteLine($"Total Days: {totalDays}");
        // Console.WriteLine($"Total Revenue: {totalRevenue}");
        // Console.WriteLine($"Total Cost: {totalCost}");
        // Console.WriteLine($"Total Deliverables: {backlog.Deliverables.Count}");

        var simulationList = new List<DeliverableModel>();
        var simulationListofList = new List<List<DeliverableModel>>();
        var simulationDays = new List<double>();
        var simulationsSalesResults = new List<double>();
        var simulationsCostResults = new List<double>();
        for (int i = 0; i < _simulationCount; ++i)
        {
            simulationList = MonteCarloSimulation.RunSimulation(backlog, columns, totalDays, estimatedRevenuePerDay, estimatedCostPerDay);
            simulationListofList.Add(simulationList);
            simulationDays.Add(simulationList.Max(d => d.AccumulatedDays));
            simulationsSalesResults.Add(simulationList.Max(d => d.AccumulatedDays) * estimatedRevenuePerDay);
            simulationsCostResults.Add(simulationList.Max(d => d.AccumulatedDays) * estimatedCostPerDay);
        }
        SimTotalSalesResult = simulationsSalesResults;
        SimTotalCostsResult = simulationsCostResults;
        SimTotalDaysResult = simulationDays;
        Simulations = simulationListofList;
        var averageTotalDays = simulationDays.Average();
        NewDate = startDate.AddDays(averageTotalDays);

    }

    private static List<DeliverableModel> RunSimulation(BacklogModel backlog,
                                      List<ColumnModel> columns,
                                      double totalDays,
                                      double? revenuePerDay,
                                      double costPerDay
                                      )
    {
        // Simulation logic
        // For each deliverable, simulate the probability of 
        // completion for each column holding the contrains of 
        // the WIPs in each column

        var deliverablesCopy = backlog.Deliverables.Select(d => new DeliverableModel
        {
            Id = d.Id,
            Nr = d.Nr,
            CompletionDays = d.CompletionDays,
            AccumulatedDays = d.AccumulatedDays
        }).ToList();

        // List of deliverables in each column
        var columnDeliverables = columns.ToDictionary(c => c, c => new List<DeliverableModel>());

        // Initialize deliverables
        deliverablesCopy.Shuffle();
        foreach (var deliverable in deliverablesCopy)
        {
            var firstColumn = columns.First();
            columnDeliverables[firstColumn].Add(deliverable);
        }

        double currentDay = 0;

        // While there are deliverables in the system
        while (columnDeliverables.Any(kvp => kvp.Value.Count > 0))
        {
            // Increment the current day for each iteration
            currentDay++;

            foreach (var column in columns)
            {
                var wipQueue = columnDeliverables[column];
                var wipStack = new List<DeliverableModel>();

                // We use a new list to keep track of deliverables to move to next column
                var deliverablesToMove = new List<DeliverableModel>();

                foreach (var deliverable in wipQueue.ToList()) // iterate over a copy of the list
                {
                    if (!deliverable.IsCalculated && column.IsBuffer == false)
                    {
                        // Probability is always a random % between the low and high bounds
                        var probability = backlog.PercentageLowBound
                                          + (backlog.PercentageHighBound - backlog.PercentageLowBound)
                                          * ThreadSafeRandom.ThisThreadsRandom.NextDouble();

                        // CompletionDays is always a random day between the low and high bounds for the column
                        var completionDays = column.EstimatedLowBound
                                             + (column.EstimatedHighBound - column.EstimatedLowBound)
                                             * ThreadSafeRandom.ThisThreadsRandom.NextDouble();

                        // Random generated completionDay for deliverable
                        deliverable!.CompletionDays = currentDay + (completionDays * probability);
                        deliverable.IsCalculated = true;
                    }
                    if (column.IsBuffer == true)
                    {
                        deliverable.CompletionDays = 0.0 + currentDay;
                    }

                    // Check if deliverable is done with this column
                    if (deliverable.CompletionDays <= currentDay)
                    {
                        // Deliverable is ready to move
                        deliverablesToMove.Add(deliverable);
                    }
                    else
                    {
                        // Deliverable is still in progress, add to WIP stack if there's space
                        if (wipStack.Count < column.WIP)
                        {
                            wipStack.Add(deliverable);
                        }
                    }
                }
                // Move deliverables to the next column if possible
                foreach (var deliverable in deliverablesToMove)
                {
                    wipQueue.Remove(deliverable); // remove from current column

                    var nextColumnIndex = columns.IndexOf(column) + 1;

                    // If there's a next column
                    if (nextColumnIndex < columns.Count)
                    {
                        var nextColumn = columns[nextColumnIndex];

                        // Check if next column's WIP limit is not exceeded
                        if (columnDeliverables[nextColumn].Count < nextColumn.WIP)
                        {
                            deliverable.AccumulatedDays = currentDay; // += deliverable.CompletionDays;
                            deliverable.ColumnIndex = nextColumnIndex;
                            deliverable.IsCalculated = false;
                            columnDeliverables[nextColumn].Add(deliverable);
                            wipStack.Remove(deliverable);
                        }
                        else
                        {
                            // If next column WIP is full, hold the deliverable here
                            wipQueue.Add(deliverable);
                            // If deliverable is still in wipStack add WaitTime
                            deliverable.WaitTime += 1;
                        }
                    }
                }
            }
        }
        var orderdedByAccumulated = deliverablesCopy.OrderBy(d => d.AccumulatedDays).ToList();
        return orderdedByAccumulated;
    }

    public void PrintSimulationResults()
    {
        var staff = ProjectSimulationModel.Staff;
        var startDate = ProjectSimulationModel.StartDate;
        var targetDate = ProjectSimulationModel.TargetDate;
        var revenue = ProjectSimulationModel.Revenue;
        var cost = ProjectSimulationModel.Costs;
        var backlog = ProjectSimulationModel.Backlog;
        var columns = ProjectSimulationModel.Columns;

        ValidateProps(staff, revenue, cost, backlog);

        var totalDays = (targetDate - startDate).TotalDays;

        var totalRevenue = 0.0;
        var totalCost = 0.0;
        for (var i = 0; i < staff?.Count; i++)
        {
            var staffMember = staff[i];
            var staffDays = staffMember.Days;
            var staffRevenue = staffMember.Sale * staffDays;
            var staffCost = staffMember.Cost * staffDays;
            totalRevenue += staffRevenue;
            totalCost += staffCost;
        }
        var estimatedRevenuePerDay = revenue.Amount / totalDays;
        var estimatedCostPerDay = cost.Cost / totalDays;


        var simulationList = new List<DeliverableModel>();
        for (int i = 0; i < _simulationCount; ++i)
        {
            simulationList = MonteCarloSimulation.RunSlowSimulation(backlog, columns, totalDays, estimatedRevenuePerDay, estimatedCostPerDay);
        }
    }

    private static void PrintDeliverable(List<ColumnModel> columns, ColumnModel column, int interval, DeliverableModel moveDeliverable, int nextColumnIndex)
    {
        // Console.WriteLine($"Interval = {interval} Deliverable Nr.: {moveDeliverable.Nr}, Days {moveDeliverable.CompletionDays} - moved from {column.Name} to column {columns[nextColumnIndex].Name}");
    }

    static void ValidateProps(List<StaffModel>? staff, RevenueModel? revenue, CostModel cost, BacklogModel deliverables)
    {
        if (staff == null || revenue == null || cost == null || deliverables == null)
        {
            throw new InvalidOperationException("Staff, Revenue, Cost, and Deliverables must be set.");
        }
    }
    private static List<DeliverableModel> RunSlowSimulation(BacklogModel backlog,
                                      List<ColumnModel> columns,
                                      double totalDays,
                                      double? revenuePerDay,
                                      double costPerDay
                                      )
    {
        // Simulation logic
        // For each deliverable, simulate the probability of 
        // completion for each column holding the contrains of 
        // the WIPs in each column

        var deliverablesCopy = backlog.Deliverables.Select(d => new DeliverableModel
        {
            Id = d.Id,
            Nr = d.Nr,
            CompletionDays = d.CompletionDays,
            AccumulatedDays = d.AccumulatedDays
        }).ToList();

        // Initialize deliverables
        deliverablesCopy.Shuffle();

        // List of deliverables in each column
        var columnDeliverables = columns.ToDictionary(c => c, c => new List<DeliverableModel>());

        // Initialize deliverables
        //deliverablesCopy.Shuffle();
        foreach (var deliverable in deliverablesCopy)
        {
            var firstColumn = columns.First();
            columnDeliverables[firstColumn].Add(deliverable);
        }


        double currentDay = 0;

        // While there are deliverables in the system
        while (columnDeliverables.Any(kvp => kvp.Value.Count > 0))
        {
            // Increment the current day for each iteration
            currentDay++;

            foreach (var column in columns)
            {
                var wipQueue = columnDeliverables[column];
                var wipStack = new List<DeliverableModel>();

                // We use a new list to keep track of deliverables to move to next column
                var deliverablesToMove = new List<DeliverableModel>();

                foreach (var deliverable in wipQueue.ToList()) // iterate over a copy of the list
                {
                    if (!deliverable.IsCalculated && column.IsBuffer == false)
                    {
                        // Probability is always a random % between the low and high bounds
                        var probability = backlog.PercentageLowBound
                                          + (backlog.PercentageHighBound - backlog.PercentageLowBound)
                                          * ThreadSafeRandom.ThisThreadsRandom.NextDouble();

                        // CompletionDays is always a random day between the low and high bounds for the column
                        var completionDays = column.EstimatedLowBound
                                             + (column.EstimatedHighBound - column.EstimatedLowBound)
                                             * ThreadSafeRandom.ThisThreadsRandom.NextDouble();

                        // Random generated completionDay for deliverable
                        deliverable!.CompletionDays = currentDay + (completionDays * probability);
                        deliverable.IsCalculated = true;
                    }
                    if (column.IsBuffer == true)
                    {
                        deliverable.CompletionDays = 0.0 + currentDay;
                    }

                    // Check if deliverable is done with this column
                    if (deliverable.CompletionDays <= currentDay)
                    {
                        // Deliverable is ready to move
                        deliverablesToMove.Add(deliverable);
                    }
                    else
                    {
                        // Deliverable is still in progress, add to WIP stack if there's space
                        if (wipStack.Count < column.WIP)
                        {
                            wipStack.Add(deliverable);
                        }
                    }
                }
                // Move deliverables to the next column if possible
                foreach (var deliverable in deliverablesToMove)
                {
                    wipQueue.Remove(deliverable); // remove from current column

                    var nextColumnIndex = columns.IndexOf(column) + 1;

                    // If there's a next column
                    if (nextColumnIndex < columns.Count)
                    {
                        var nextColumn = columns[nextColumnIndex];

                        // Check if next column's WIP limit is not exceeded
                        if (columnDeliverables[nextColumn].Count < nextColumn.WIP)
                        {
                            deliverable.AccumulatedDays = currentDay; // += deliverable.CompletionDays;
                            deliverable.ColumnIndex = nextColumnIndex;
                            deliverable.IsCalculated = false;
                            columnDeliverables[nextColumn].Add(deliverable);
                            wipStack.Remove(deliverable);
                        }
                        else
                        {
                            // If next column WIP is full, hold the deliverable here
                            wipQueue.Add(deliverable);
                            // If deliverable is still in wipStack add WaitTime
                            deliverable.WaitTime += 1;
                        }
                    }
                }
            }
            PrintEachIteration(columns, deliverablesCopy, currentDay, columnDeliverables);
        }
        var orderdedByAccumulated = deliverablesCopy.OrderBy(d => d.AccumulatedDays).ToList();
        return orderdedByAccumulated;
    }

    private static void PrintEachIteration(List<ColumnModel> columns, List<DeliverableModel> deliverablesCopy, double currentDay, Dictionary<ColumnModel, List<DeliverableModel>> columnDeliverables)
    {
        Console.Clear();
        var columnsToPrint = new Dictionary<int, int>()
                {
                    { 0, 0 },
                    { 1, 20 },
                    { 2, 40 },
                    { 3, 60 },
                    { 4, 90 },
                    { 5, 100 },
                    { 6, 120 },
                    { 7, 160 }
                };

        Console.WriteLine($"Current Day: {currentDay}");
        for (var i = 0; i < columns.Count; ++i)
        {
            // Printing each header position
            Console.SetCursorPosition(columnsToPrint[i], 1);
            Console.WriteLine(columns[i].Name);

        }
        foreach (var printColumn in columns)
        {
            int index = 0;
            if (index > printColumn.WIP)
            {
                index = 0;
            }
            // Printing rows for each deliverable to each header position
            // Sort by AccumulatedDays
            // If printColumn is the first column, do not sort by AccumulatedDays otherwise sort
            foreach (var deliverableToPrint in deliverablesCopy)
            {
                // check if deliverableToPrint is in wipQueue
                if (deliverableToPrint.ColumnIndex == columns.IndexOf(printColumn) && index < 36)
                {
                    Console.SetCursorPosition(columnsToPrint[deliverableToPrint.ColumnIndex], index + 2);
                    // T=Task, D=Days, W=WaitTime
                    Console.WriteLine($"T:{deliverableToPrint.Nr} D: {double.Round(deliverableToPrint.CompletionDays)}, W: {double.Round(deliverableToPrint.WaitTime)}");
                    index++;
                }
            }
        }
        // Pause every print
        System.Threading.Thread.Sleep(TimeSpan.FromSeconds(0.3));
    }

    public static async Task ColumnEstimateAnalysis(ProjectSimulationModel projectSimModel, int projectSimulationsCount)
    {
        const double estimateMultiplier = 0.5;
        var projectWithModifiedEstimates = new List<ProjectSimulationModel> { projectSimModel };
        // Loop through each column and modify EstimatedLowBound and EstimatedHighBound
        for (int i = 0; i < projectSimModel.Columns.Count; i++)
        {
            var columnName = projectSimModel.Columns[i].Name!;
            // Clone the original model
            var newProjectSimModel = projectSimModel.CloneProjectSimModel(projectSimModel, columnName);
            // Modify the estimated bounds of the specific column
            var columnToModify = newProjectSimModel.Columns[i];
            columnToModify.EstimatedLowBound *= estimateMultiplier;
            columnToModify.EstimatedHighBound *= estimateMultiplier;
            projectWithModifiedEstimates.Add(newProjectSimModel);
        }

        int projectsCount = projectWithModifiedEstimates.Count;
        var orderedByTotalDays = await RunSimulationAsync(projectWithModifiedEstimates, projectSimulationsCount);

        var baselineProject = orderedByTotalDays.FirstOrDefault(mcs => mcs.ProjectSimulationModel.Name == projectSimModel.Name);
        var baselineTotalDays = baselineProject!.SimTotalDaysResult!.Percentile(0.9);
        foreach (var MCS in orderedByTotalDays)
        {
            var name = MCS.ProjectSimulationModel.Name;
            var totalDays = MCS.SimTotalDaysResult != null && MCS.SimTotalDaysResult.Any()
                ? MCS.SimTotalDaysResult.Percentile(0.9)
                : 0.0;
            if (totalDays < baselineTotalDays)
            {
                Console.WriteLine($"Simulation {name} is {totalDays}\t\t\t\tdays from {baselineProject.ProjectSimulationModel.Name} {baselineTotalDays}\t\t\tDifference {baselineTotalDays - totalDays}");
            }
        }
    }


    public static async Task WIPAnalysis(ProjectSimulationModel projectSimModel, int projectSimulationsCount)
    {

        // Rerun the simulation with the modified WIP and get the Results
        for (int k = 1; k <= 5; k++)
        {
            // setting constant for the estimate multiplier
            int wipMultiplier = 1;

            // getting all the projects with modified estimates
            var projectWithModifiedWIP = new List<ProjectSimulationModel> { projectSimModel };
            ProjectWIPMultiplier(projectSimModel, projectWithModifiedWIP, wipMultiplier);

            int projectsCount = projectWithModifiedWIP.Count;

            //var orderedByTotalDays = await RunSimulationAsync(projectWithModifiedEstimates, projectSimulationsCount);
            var orderedByTotalDays = await RunSimulationAsync(projectWithModifiedWIP, projectSimulationsCount);

            var baselineProject = orderedByTotalDays.FirstOrDefault(mcs => mcs.ProjectSimulationModel.Name == projectSimModel.Name);
            if (baselineProject == null)
            {
                Console.WriteLine("Baseline project not found.");
                continue;
            }
            var baselineTotalDays = baselineProject!.SimTotalDaysResult!.Percentile(0.9);
            var simulationWithLowestTotalDays = baselineTotalDays;
            var simulationWithLowestTotalDaysIndex = baselineProject;

            // Column with most impact on the project duration
            foreach (var MCS in orderedByTotalDays)
            {
                var name = MCS.ProjectSimulationModel.Name;
                var totalDays = MCS.SimTotalDaysResult != null && MCS.SimTotalDaysResult.Any()
                    ? MCS.SimTotalDaysResult.Percentile(0.9)
                    : 0.0;
                if (totalDays < simulationWithLowestTotalDays)
                {
                    simulationWithLowestTotalDays = totalDays;
                    simulationWithLowestTotalDaysIndex = MCS;
                }
            }
            // print the simulation with the lowest total Days
            Console.WriteLine($"Simulation with the lowest total days: {simulationWithLowestTotalDaysIndex.ProjectSimulationModel.Name} is {simulationWithLowestTotalDays} days from {baselineProject.ProjectSimulationModel.Name} {baselineTotalDays}");
            projectSimModel = simulationWithLowestTotalDaysIndex.ProjectSimulationModel;
            wipMultiplier += 1;

        }
    }

    private static void ProjectWIPMultiplier(ProjectSimulationModel projectSimModel, List<ProjectSimulationModel> projectWithModifiedEstimates, int multiplier)
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

    private static async Task<MonteCarloSimulation[]> RunSimulationAsync(List<ProjectSimulationModel> projectWithModifiedEstimates, int projectSimulationsCount)
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
}
public static class ThreadSafeRandom
{
    [ThreadStatic] private static Random? Local;

    public static Random ThisThreadsRandom
    {
        get { return Local ?? (Local = new Random(unchecked(Environment.TickCount * 31 + Thread.CurrentThread.ManagedThreadId))); }
    }
}

static class MyExtensions
{
    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = ThreadSafeRandom.ThisThreadsRandom.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}
