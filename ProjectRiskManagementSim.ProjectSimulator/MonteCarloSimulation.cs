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
        int wipStack = 0;
        int interval = 0;

        // Columns are sorted in the right flow order
        foreach (var column in columns)
        {
            var delivarableCompletionList = new List<double>();
            foreach (var deliverable in deliverables)
            {
                // Probability is always a random % between the low and high bounds
                var probability = backlog.PercentageLowBound + (backlog.PercentageHighBound - backlog.PercentageLowBound) * ThreadSafeRandom.ThisThreadsRandom.NextDouble();

                // CompletionDays is always a random day between the low and high bounds for the column
                var completionDays = column.EstimatedLowBound + (column.EstimatedHighBound - column.EstimatedLowBound) * ThreadSafeRandom.ThisThreadsRandom.NextDouble();

                var deliverableCompletionDay = completionDays * probability;
                delivarableCompletionList.Add(deliverableCompletionDay);

                wipStack += 1;

                if (wipStack == column.WIP)
                {
                    var moveDelivToNextColumn = delivarableCompletionList.Min();
                    // Reset the wipStack
                    wipStack -= 1;
                }
            }
        }
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
    [ThreadStatic] private static Random Local;

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
