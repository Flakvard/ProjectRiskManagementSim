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

        var columnDeliverables = columns.ToDictionary(c => c, c => new List<DeliverableModel>());

        // Initialize deliverables
        deliverablesCopy.Shuffle();
        foreach (var deliverable in deliverablesCopy)
        {
            //Console.Write(deliverable.Nr);
            var firstColumn = columns.First();
            columnDeliverables[firstColumn].Add(deliverable);
            // columnDeliverables.Dump();
        }

        foreach (var column in columns)
        {
            // Console.WriteLine($"Loop column: {column.Name}");
            var wipQueue = columnDeliverables[column];
            var wipStack = new List<DeliverableModel>();
            var interval = 0;

            while (wipQueue.Count > 0)
            {
                var deliverable = wipQueue.First();
                // Probability is always a random % between the low and high bounds
                var probability = backlog.PercentageLowBound + (backlog.PercentageHighBound - backlog.PercentageLowBound) * ThreadSafeRandom.ThisThreadsRandom.NextDouble();
                // CompletionDays is always a random day between the low and high bounds for the column
                var completionDays = column.EstimatedLowBound + (column.EstimatedHighBound - column.EstimatedLowBound) * ThreadSafeRandom.ThisThreadsRandom.NextDouble();
                // Random generated completionDay for deliverable
                deliverable!.CompletionDays = completionDays * probability;

                // If the WIP stack is full, move deliverable to the next column
                if (wipStack.Count >= column.WIP)
                {
                    var moveDeliverable = wipStack.OrderBy(d => d.CompletionDays).First();
                    wipStack.Remove(moveDeliverable);
                    wipQueue.Remove(moveDeliverable);

                    // Move deliverable to the next column
                    var nextColumnIndex = columns.IndexOf(column) + 1;
                    if (nextColumnIndex < columns.Count)
                    {
                        moveDeliverable.AccumulatedDays += moveDeliverable.CompletionDays;
                        ++interval;
                        PrintDeliverable(columns, column, interval, moveDeliverable, nextColumnIndex);
                        columnDeliverables[columns[nextColumnIndex]].Add(moveDeliverable);
                    }
                }
                // When the queue is empty, move deliverables from the stack to the next column
                else if (wipStack.Count <= column.WIP && wipQueue.Count == 1)
                {
                    while (wipStack.Count > 0)
                    {

                        var moveDeliverable = wipStack.OrderBy(d => d.CompletionDays).First();
                        wipStack.Remove(moveDeliverable);

                        // Move deliverable to the next column
                        var nextColumnIndex = columns.IndexOf(column) + 1;
                        if (nextColumnIndex < columns.Count)
                        {
                            moveDeliverable.AccumulatedDays += moveDeliverable.CompletionDays;
                            ++interval;
                            PrintDeliverable(columns, column, interval, moveDeliverable, nextColumnIndex);
                            columnDeliverables[columns[nextColumnIndex]].Add(moveDeliverable);
                        }
                    }
                    var lastColumnIndexInStack = columns.IndexOf(column) + 1;
                    if (lastColumnIndexInStack < columns.Count)
                    {
                        deliverable.AccumulatedDays += deliverable.CompletionDays;
                        ++interval;
                        PrintDeliverable(columns, column, interval, deliverable, lastColumnIndexInStack);
                        columnDeliverables[columns[lastColumnIndexInStack]].Add(deliverable);
                    }
                    wipQueue.Remove(deliverable);
                }
                // When the WIP stack is not full, keep adding them to the stack and remove from Queue
                else
                {
                    // Add deliverable to the WIP stack
                    wipStack.Add(deliverable);
                    wipQueue.Remove(deliverable);
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
                        deliverable!.CompletionDays = completionDays * probability;
                        deliverable.IsCalculated = true;
                    }
                    if (column.IsBuffer == true)
                    {
                        deliverable.CompletionDays = 0.0;
                    }

                    // Check if deliverable is done with this column
                    if (deliverable.AccumulatedDays <= currentDay)
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
                            deliverable.AccumulatedDays += deliverable.CompletionDays;
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
                    { 1, 30 },
                    { 2, 60 },
                    { 3, 90 },
                    { 4, 120 },
                    { 5, 150 }
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
            deliverablesCopy.Sort((x, y) => x.AccumulatedDays.CompareTo(y.AccumulatedDays));
            foreach (var deliverableToPrint in deliverablesCopy)
            {
                // check if deliverableToPrint is in wipQueue
                if (deliverableToPrint.ColumnIndex == columns.IndexOf(printColumn))
                {
                    Console.SetCursorPosition(columnsToPrint[deliverableToPrint.ColumnIndex], index + 2);
                    Console.WriteLine($"Task:{deliverableToPrint.Nr} Days: {double.Round(deliverableToPrint.AccumulatedDays)},  Wait: {double.Round(deliverableToPrint.WaitTime)}");
                    index++;
                }
                // Pause every print
            }
        }
        System.Threading.Thread.Sleep(TimeSpan.FromSeconds(0.8));
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
