using Dumpify;

namespace ProjectRiskManagementSim.ProjectSimulation;

internal class MonteCarloSimulation
{
    private readonly ProjectSimulationModel _projectSimulationModel;
    private readonly Random _random = new Random();

    public MonteCarloSimulation(ProjectSimulationModel projectSimulationModel)
    {
        _projectSimulationModel = projectSimulationModel;
    }

    public void InitiateSimulation()
    {
        var staff = _projectSimulationModel.Staff;
        var startDate = _projectSimulationModel.StartDate;
        var targetDate = _projectSimulationModel.TargetDate;
        var revenue = _projectSimulationModel.Revenue;
        var cost = _projectSimulationModel.Costs;
        var backlog = _projectSimulationModel.Backlog;
        var columns = _projectSimulationModel.Columns;

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
        var revenuePerDay = revenue?.Amount / totalDays;
        var costPerDay = cost.Cost / totalDays;

        Console.WriteLine($"Total Days: {totalDays}");
        Console.WriteLine($"Total Revenue: {totalRevenue}");
        Console.WriteLine($"Total Cost: {totalCost}");
        Console.WriteLine($"Total Deliverables: {backlog.Deliverables.Count}");

        MonteCarloSimulation.RunSimulation(backlog, columns, totalDays, revenuePerDay, costPerDay);
    }

    private static void RunSimulation(BacklogModel backlog,
                                      List<ColumnModel> columns,
                                      double totalDays,
                                      double? revenuePerDay,
                                      double costPerDay)
    {
        var deliverables = backlog.Deliverables;
        // Simulation logic
        // For each deliverable, simulate the probability of 
        // completion for each column holding the contrains of 
        // the WIPs in each column

        // var columnDeliverables = columns.ToDictionary(c => c,
        //                                               c => new SortedSet<DeliverableModel>(
        //                                                 Comparer<DeliverableModel>.Create(
        //                                                   (x, y) => x.CompletionDays.CompareTo(y.CompletionDays))));
        var columnDeliverables = columns.ToDictionary(c => c, c => new List<DeliverableModel>());

        // Initialize deliverables
        foreach (var deliverable in backlog.Deliverables)
        {
            Console.Write(deliverable.Nr);
            var firstColumn = columns.First();
            columnDeliverables[firstColumn].Add(deliverable);
            // columnDeliverables.Dump();
        }

        // foreach (var deliv in columnDeliverables)
        // {
        //     Console.WriteLine(deliv.Key.Name);
        //     foreach (var d in deliv.Value)
        //     {
        //         Console.WriteLine(d.Nr);
        //     }
        // }

        foreach (var column in columns)
        {
            Console.WriteLine($"Loop column: {column.Name}");
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
                        ++interval;
                        Console.WriteLine($"Interval = {interval} Deliverable Nr.: {moveDeliverable.Nr}, Days {moveDeliverable.CompletionDays} - moved from {column.Name} to column {columns[nextColumnIndex].Name}");
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
                            ++interval;
                            Console.WriteLine($"Interval = {interval} Deliverable Nr.: {moveDeliverable.Nr}, Days {moveDeliverable.CompletionDays} - moved from {column.Name} to column {columns[nextColumnIndex].Name}");
                            columnDeliverables[columns[nextColumnIndex]].Add(moveDeliverable);
                        }
                    }
                    var lastColumnIndexInStack = columns.IndexOf(column) + 1;
                    if (lastColumnIndexInStack < columns.Count)
                    {
                        ++interval;
                        Console.WriteLine($"Interval = {interval} Deliverable Nr.: {deliverable.Nr}, Days {deliverable.CompletionDays} - moved from {column.Name} to column {columns[lastColumnIndexInStack].Name}");
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
        // columnDeliverables.Dump();
    }
    static void ValidateProps(List<StaffModel>? staff, RevenueModel? revenue, CostModel cost, BacklogModel deliverables)
    {
        if (staff == null || revenue == null || cost == null || deliverables == null)
        {
            throw new InvalidOperationException("Staff, Revenue, Cost, and Deliverables must be set.");
        }
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
