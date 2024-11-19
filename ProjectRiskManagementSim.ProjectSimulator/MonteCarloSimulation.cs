using System.Linq;
using Dumpify;
using ProjectRiskManagementSim.ProjectSimulation.Utilities;
using ProjectRiskManagementSim.ProjectSimulation.Models;
using ProjectRiskManagementSim.ProjectSimulation.Interfaces;
using Spectre.Console;

namespace ProjectRiskManagementSim.ProjectSimulation;

public class MonteCarloSimulation : IMonteCarloSimulation
{
    public ProjectSimulationModel ProjectSimulationModel { get; set; }

    private const string Rdy4TestProd = "Ready to test on Production";
    private const string Backlog = "Backlog";
    private const string TestStage = "Testing on Development";
    private const string InProgress = "In Progress";
    private const string Open = "Open";
    private const string Rdy4Test = "Ready to test on Development";
    private const string AwaitDplyProd = "Waiting Deployment on Production";
    private readonly Random _random = new Random();
    public List<double>? SimTotalDaysResult { get; set; }
    public List<double>? SimTotalCostsResult { get; set; }
    public List<double>? SimTotalSalesResult { get; set; }
    public List<List<DeliverableModel>>? Simulations { get; set; }
    public DateTime NewDate { get; set; }
    public bool IsCompleted { get; set; }
    public Guid SimulationId { get; set; }
    public Dictionary<ColumnModel, List<DeliverableModel>?>? ColumnDeliverables { get; set; }
    public List<DeliverableModel>? DeliverablesCopy { get; set; }
    public Dictionary<double, (string, double)>? WipAnalysis { get; set; } = new Dictionary<double, (string, double)>();
    public Dictionary<double, (string, double)>? ColumnAnalysis { get; set; } = new Dictionary<double, (string, double)>();


    private int _simulationCount;


    public void InitiateAndRunSimulation(ProjectSimulationModel projectSimulationModel,
                                int simulationCount, Guid simulationId)
    {
        IsCompleted = false;
        SimulationId = simulationId;
        ProjectSimulationModel = projectSimulationModel;
        _simulationCount = simulationCount;
        var staff = projectSimulationModel.Staff;
        var startDate = projectSimulationModel.StartDate;
        var targetDate = projectSimulationModel.TargetDate;
        var revenue = projectSimulationModel.Revenue;
        var cost = projectSimulationModel.Costs;
        var backlog = projectSimulationModel.Backlog;
        var columns = projectSimulationModel.Columns;
        var defects = projectSimulationModel.Defects;
        var blockingEvents = projectSimulationModel.BlockingEvents;

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
        var simulationListofList = new List<List<DeliverableModel>>();
        var simulationDays = new List<double>();
        var simulationsSalesResults = new List<double>();
        var simulationsCostResults = new List<double>();
        for (int i = 0; i < _simulationCount; ++i)
        {
            simulationList = MonteCarloSimulation.RunSimulation(backlog, columns, totalDays, estimatedRevenuePerDay, estimatedCostPerDay, defects, blockingEvents);
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
        IsCompleted = true;
    }

    private static List<DeliverableModel> RunSimulation(BacklogModel backlog,
                                      List<ColumnModel> columns,
                                      double totalDays,
                                      double? revenuePerDay,
                                      double costPerDay,
                                      List<DefectModel> defects,
                                      List<BlockingEventModel> blockingEvents
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
        const int developersHelping = 1;
        int originalTestingProdWIP = columns.First(c => c.Name == Rdy4TestProd).WIP;
        int originalTestingDevWIP = columns.First(c => c.Name == TestStage).WIP;
        int originalDevelopersWIP = columns.First(c => c.Name == InProgress).WIP;

        // While there are deliverables in the system
        while (columnDeliverables.Any(kvp => kvp.Value.Count > 0))
        {
            // Increment the current day for each iteration
            currentDay++;
            if (currentDay > 36000)
            {
                throw new InvalidOperationException("Simulation took too long > 100 years");
            }

            foreach (var column in columns)
            {
                var wipQueue = columnDeliverables[column];
                var wipStack = new List<DeliverableModel>();

                var backlogColumn = columns.FirstOrDefault(c => c.Name == Backlog);
                var openColumn = columns.FirstOrDefault(c => c.Name == Open);
                var testingProdColumn = columns.FirstOrDefault(c => c.Name == Rdy4TestProd);
                var testingDevColumn = columns.FirstOrDefault(c => c.Name == TestStage);
                var developerColumn = columns.FirstOrDefault(c => c.Name == InProgress);

                // Check if the "Ready for Test Stage" column is full
                // if so allocate developers to help testing
                var readyForTestDevColumn = columns.FirstOrDefault(c => c.Name == Rdy4Test);
                if (readyForTestDevColumn != null
                    && IsBottleneck(readyForTestDevColumn, columnDeliverables[readyForTestDevColumn])
                    && testingDevColumn?.WIP <= testingDevColumn?.WIPMax
                    )
                {
                    if (developerColumn.WIP == 0)
                    {
                        ReallocateWIP(developersHelping, testingDevColumn, testingProdColumn);

                    }
                    else
                    {
                        ReallocateWIP(developersHelping, testingDevColumn, developerColumn);
                    }
                }


                // Check if the "Ready for Test on Prod" column is full
                // if so allocate developers to help testing
                var readyForTestProdColumn = columns.FirstOrDefault(c => c.Name == AwaitDplyProd);
                if (readyForTestProdColumn != null
                    && IsBottleneck(readyForTestProdColumn, columnDeliverables[readyForTestProdColumn])
                    && testingProdColumn?.WIP <= testingProdColumn?.WIPMax
                    )
                {
                    if (developerColumn.WIP == 0)
                    {
                        ReallocateWIP(developersHelping, testingProdColumn, testingDevColumn);

                    }
                    else
                    {
                        ReallocateWIP(developersHelping, testingProdColumn, developerColumn);

                    }
                }

                // We use a new list to keep track of deliverables to move to next column
                var deliverablesToMove = new List<DeliverableModel>();

                // Calculate the deliverable completion day and add to list 
                CalculateDeliverableCompletionDayANDAddToList(backlog, currentDay, column, wipQueue, wipStack, deliverablesToMove, defects, blockingEvents);

                // Check how many are working on the deliverable in columns and add a day to the ones that are postponed for the next day.
                AddWaitingTimeForDeliverablesInQueue(column, wipQueue);

                // Move deliverables to the next column if possible
                MoveDeliverablesToNextColumnIfPossible(columns, columnDeliverables, currentDay, column, wipQueue, wipStack, deliverablesToMove, defects, deliverablesCopy, blockingEvents);


                int devStack = developerColumn != null && columnDeliverables.ContainsKey(developerColumn) && columnDeliverables[developerColumn] != null ? columnDeliverables[developerColumn].Count : 0;

                // Check if the "Ready for Test Stage" column is NOT full anymore and give back WIP
                if (readyForTestDevColumn != null
                    && !IsBottleneck(readyForTestDevColumn, columnDeliverables[readyForTestDevColumn])
                    && testingDevColumn?.WIP <= testingDevColumn?.WIPMax
                    && devStack != 0
                    )
                {
                    ReallocateWIP(developersHelping, developerColumn, testingDevColumn);
                }

                // Check if the "Ready for Test on Prod" column is NOT full anymore and give back WIP
                if (readyForTestProdColumn != null
                    && !IsBottleneck(readyForTestProdColumn, columnDeliverables[readyForTestProdColumn])
                    && testingProdColumn?.WIP <= testingProdColumn?.WIPMax
                    && devStack != 0
                    )
                {
                    ReallocateWIP(developersHelping, developerColumn, testingProdColumn);
                }

                int testDevStack = testingDevColumn != null && columnDeliverables.ContainsKey(testingDevColumn) && columnDeliverables[testingDevColumn] != null ? columnDeliverables[testingDevColumn].Count : 0;
                int rdyForTestDevStack = readyForTestDevColumn != null && columnDeliverables.ContainsKey(readyForTestDevColumn) && columnDeliverables[readyForTestDevColumn] != null ? columnDeliverables[readyForTestDevColumn].Count : 0;
                int backlogStack = backlogColumn != null && columnDeliverables.ContainsKey(backlogColumn) && columnDeliverables[backlogColumn] != null ? columnDeliverables[backlogColumn].Count : 0;
                int openStack = openColumn != null && columnDeliverables.ContainsKey(openColumn) && columnDeliverables[openColumn] != null ? columnDeliverables[openColumn].Count : 0;
                int testProdStack = testingProdColumn != null && columnDeliverables.ContainsKey(testingProdColumn) && columnDeliverables[testingProdColumn] != null ? columnDeliverables[testingProdColumn].Count : 0;

                // if Testing on Stage is not doing any work, reallocate back
                if (testDevStack < testingDevColumn?.WIP && devStack != 0)
                {
                    ReallocateWIP(developersHelping, developerColumn, testingDevColumn);
                }

                // if Testing on Prod is not doing any work, reallocate back
                if (testProdStack < testingProdColumn?.WIP && devStack != 0)
                {
                    ReallocateWIP(developersHelping, developerColumn, testingProdColumn);
                }

                // if Testing on Stage is not doing any work, reallocate back
                if (testDevStack < testingDevColumn?.WIP && devStack == 0 && openStack > 0)
                {
                    ReallocateWIP(developersHelping, developerColumn, testingDevColumn);
                }

                // if Testing on Prod is not doing any work, reallocate back
                if (testProdStack < testingProdColumn?.WIP && devStack == 0 && openStack > 0)
                {
                    ReallocateWIP(developersHelping, developerColumn, testingProdColumn);
                }

                // if there is no work "In Progress" reallocate to test on stage
                if (devStack == 0 && rdyForTestDevStack > 0 && backlogStack == 0 && currentDay > 20)
                {
                    ReallocateWIP(developersHelping, testingDevColumn, developerColumn);
                }

                // if there is no work "In Progress" and no work in "Test Stage" reallocate to test prod
                if (devStack == 0 && testDevStack == 0 && rdyForTestDevStack == 0 && backlogStack == 0 && currentDay > 20)
                {
                    ReallocateWIP(developersHelping, testingProdColumn, testingDevColumn);
                }
            }
        }
        var orderdedByAccumulated = deliverablesCopy.OrderBy(d => d.AccumulatedDays).ToList();
        return orderdedByAccumulated;
    }

    public void PrintSimulationResults(ProjectSimulationModel projectSimulationModel,
                                int simulationCount)
    {
        IsCompleted = false;
        var staff = projectSimulationModel.Staff;
        var startDate = projectSimulationModel.StartDate;
        var targetDate = projectSimulationModel.TargetDate;
        var revenue = projectSimulationModel.Revenue;
        var cost = projectSimulationModel.Costs;
        var backlog = projectSimulationModel.Backlog;
        var columns = projectSimulationModel.Columns;
        var defects = projectSimulationModel.Defects;
        var blockingEvents = projectSimulationModel.BlockingEvents;

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
        for (int i = 0; i < simulationCount; ++i)
        {
            simulationList = MonteCarloSimulation.RunSlowSimulation(backlog, columns, totalDays, estimatedRevenuePerDay, estimatedCostPerDay, defects, blockingEvents);
        }
        IsCompleted = true;
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
                                      double costPerDay,
                                      List<DefectModel> defects,
                                      List<BlockingEventModel> blockingEvents
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
        const int developersHelping = 1;
        int originalTestingProdWIP = columns.First(c => c.Name == Rdy4TestProd).WIP;
        int originalTestingDevWIP = columns.First(c => c.Name == TestStage).WIP;
        int originalDevelopersWIP = columns.First(c => c.Name == InProgress).WIP;

        // While there are deliverables in the system
        while (columnDeliverables.Any(kvp => kvp.Value.Count > 0))
        {
            // Increment the current day for each iteration
            currentDay++;

            foreach (var column in columns)
            {
                var wipQueue = columnDeliverables[column];
                var wipStack = new List<DeliverableModel>();

                var backlogColumn = columns.FirstOrDefault(c => c.Name == Backlog);
                var openColumn = columns.FirstOrDefault(c => c.Name == Open);
                var testingProdColumn = columns.FirstOrDefault(c => c.Name == Rdy4TestProd);
                var testingDevColumn = columns.FirstOrDefault(c => c.Name == TestStage);
                var developerColumn = columns.FirstOrDefault(c => c.Name == InProgress);

                // Check if the "Ready for Test Stage" column is full
                // if so allocate developers to help testing
                var readyForTestDevColumn = columns.FirstOrDefault(c => c.Name == Rdy4Test);
                if (readyForTestDevColumn != null
                    && IsBottleneck(readyForTestDevColumn, columnDeliverables[readyForTestDevColumn])
                    && testingDevColumn?.WIP <= testingDevColumn?.WIPMax
                    )
                {
                    if (developerColumn.WIP == 0)
                    {
                        ReallocateWIP(developersHelping, testingDevColumn, testingProdColumn);

                    }
                    else
                    {
                        ReallocateWIP(developersHelping, testingDevColumn, developerColumn);
                    }
                }


                // Check if the "Ready for Test on Prod" column is full
                // if so allocate developers to help testing
                var readyForTestProdColumn = columns.FirstOrDefault(c => c.Name == AwaitDplyProd);
                if (readyForTestProdColumn != null
                    && IsBottleneck(readyForTestProdColumn, columnDeliverables[readyForTestProdColumn])
                    && testingProdColumn?.WIP <= testingProdColumn?.WIPMax
                    )
                {
                    if (developerColumn.WIP == 0)
                    {
                        ReallocateWIP(developersHelping, testingProdColumn, testingDevColumn);

                    }
                    else
                    {
                        ReallocateWIP(developersHelping, testingProdColumn, developerColumn);

                    }
                }

                // We use a new list to keep track of deliverables to move to next column
                var deliverablesToMove = new List<DeliverableModel>();

                // Calculate the deliverable completion day and add to list 
                CalculateDeliverableCompletionDayANDAddToList(backlog, currentDay, column, wipQueue, wipStack, deliverablesToMove, defects, blockingEvents);

                // Check how many are working on the deliverable in columns and add a day to the ones that are postponed for the next day.
                AddWaitingTimeForDeliverablesInQueue(column, wipQueue);

                // Move deliverables to the next column if possible
                MoveDeliverablesToNextColumnIfPossible(columns, columnDeliverables, currentDay, column, wipQueue, wipStack, deliverablesToMove, defects, deliverablesCopy, blockingEvents);


                int devStack = developerColumn != null && columnDeliverables.ContainsKey(developerColumn) && columnDeliverables[developerColumn] != null ? columnDeliverables[developerColumn].Count : 0;

                // Check if the "Ready for Test Stage" column is NOT full anymore and give back WIP
                if (readyForTestDevColumn != null
                    && !IsBottleneck(readyForTestDevColumn, columnDeliverables[readyForTestDevColumn])
                    && testingDevColumn?.WIP <= testingDevColumn?.WIPMax
                    && devStack != 0
                    )
                {
                    ReallocateWIP(developersHelping, developerColumn, testingDevColumn);
                }

                // Check if the "Ready for Test on Prod" column is NOT full anymore and give back WIP
                if (readyForTestProdColumn != null
                    && !IsBottleneck(readyForTestProdColumn, columnDeliverables[readyForTestProdColumn])
                    && testingProdColumn?.WIP <= testingProdColumn?.WIPMax
                    && devStack != 0
                    )
                {
                    ReallocateWIP(developersHelping, developerColumn, testingProdColumn);
                }

                int testDevStack = testingDevColumn != null && columnDeliverables.ContainsKey(testingDevColumn) && columnDeliverables[testingDevColumn] != null ? columnDeliverables[testingDevColumn].Count : 0;
                int rdyForTestDevStack = readyForTestDevColumn != null && columnDeliverables.ContainsKey(readyForTestDevColumn) && columnDeliverables[readyForTestDevColumn] != null ? columnDeliverables[readyForTestDevColumn].Count : 0;
                int backlogStack = backlogColumn != null && columnDeliverables.ContainsKey(backlogColumn) && columnDeliverables[backlogColumn] != null ? columnDeliverables[backlogColumn].Count : 0;
                int openStack = openColumn != null && columnDeliverables.ContainsKey(openColumn) && columnDeliverables[openColumn] != null ? columnDeliverables[openColumn].Count : 0;
                int testProdStack = testingProdColumn != null && columnDeliverables.ContainsKey(testingProdColumn) && columnDeliverables[testingProdColumn] != null ? columnDeliverables[testingProdColumn].Count : 0;

                // if Testing on Stage is not doing any work, reallocate back
                if (testDevStack < testingDevColumn?.WIP && devStack != 0)
                {
                    ReallocateWIP(developersHelping, developerColumn, testingDevColumn);
                }

                // if Testing on Prod is not doing any work, reallocate back
                if (testProdStack < testingProdColumn?.WIP && devStack != 0)
                {
                    ReallocateWIP(developersHelping, developerColumn, testingProdColumn);
                }

                // if Testing on Stage is not doing any work, reallocate back
                if (testDevStack < testingDevColumn?.WIP && devStack == 0 && openStack > 0)
                {
                    ReallocateWIP(developersHelping, developerColumn, testingDevColumn);
                }

                // if Testing on Prod is not doing any work, reallocate back
                if (testProdStack < testingProdColumn?.WIP && devStack == 0 && openStack > 0)
                {
                    ReallocateWIP(developersHelping, developerColumn, testingProdColumn);
                }

                // if there is no work "In Progress" reallocate to test on stage
                if (devStack == 0 && rdyForTestDevStack > 0 && backlogStack == 0 && currentDay > 20)
                {
                    ReallocateWIP(developersHelping, testingDevColumn, developerColumn);
                }

                // if there is no work "In Progress" and no work in "Test Stage" reallocate to test prod
                if (devStack == 0 && testDevStack == 0 && rdyForTestDevStack == 0 && backlogStack == 0 && currentDay > 20)
                {
                    ReallocateWIP(developersHelping, testingProdColumn, testingDevColumn);
                }
            }
            PrintEachIteration(columns, deliverablesCopy, currentDay, columnDeliverables);
        }
        var orderdedByAccumulated = deliverablesCopy.OrderBy(d => d.AccumulatedDays).ToList();
        return orderdedByAccumulated;
    }

    private static void AddWaitingTimeForDeliverablesInQueue(ColumnModel column, List<DeliverableModel> wipQueue)
    {
        if (column.WIP < wipQueue.Count)
        {
            for (var i = 0; i < column.WIP; ++i)
            {
                {
                    if (wipQueue[i].IsBlocked == false)
                        wipQueue[i].CompletionDays += 1;
                    wipQueue[i].StoppedWorkingTime += 1;
                }
            }
        }
    }

    private static void CalculateDeliverableCompletionDayANDAddToList(BacklogModel backlog, double currentDay, ColumnModel column, List<DeliverableModel> wipQueue, List<DeliverableModel> wipStack, List<DeliverableModel> deliverablesToMove, List<DefectModel> defects, List<BlockingEventModel> blockingEvents)
    {
        foreach (var deliverable in wipQueue.ToList()) // iterate over a copy of the list
        {
            if (!deliverable.IsCalculated && column.IsBuffer == false && deliverable.IsBug == false && deliverable.IsBlocked == false)
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
            if (!deliverable.IsCalculated && column.IsBuffer == false && deliverable.IsBug == true && deliverable.IsBlocked == false)
            {
                // check if defect exists
                var defect = defects[deliverable.DefectIndex];
                if (defect != null)
                {

                    // Probability is always a random % between the low and high bounds which noramlly is 0-100
                    // because defects are generally faster than normal deliverables we make it f.x. 0-43
                    var lowBound = defect.DefectsPercentageLowBound / 100;
                    var highBound = defect.DefectsPercentageHighBound / 100;
                    var probability = lowBound
                                      + (highBound - lowBound)
                                      * ThreadSafeRandom.ThisThreadsRandom.NextDouble();

                    var columnNewLowBound = column.EstimatedLowBound * lowBound;
                    var ColumnNewHighBound = column.EstimatedHighBound * highBound;
                    // CompletionDays is always a random day between the low and high bounds for the column
                    var completionDays = columnNewLowBound
                                         + (ColumnNewHighBound - columnNewLowBound)
                                         * ThreadSafeRandom.ThisThreadsRandom.NextDouble();

                    // Random generated completionDay for deliverable
                    deliverable!.CompletionDays = currentDay + (completionDays * probability);
                    deliverable.IsCalculated = true;
                }

            }

            if (deliverable.IsBlocked == true)
            {
                if (deliverable.CompletionDays <= currentDay)
                {
                    var test = deliverable.CompletionDays;
                    deliverable.IsBlocked = false;
                }
            }

            // column index 3 == stuck
            if (deliverable.IsBlocked == false && deliverable.ColumnIndex == 3 && deliverable.WasBlocked == false)
            {
                for (int i = 0; i < blockingEvents.Count(); i++)
                {
                    BlockingEventModel? blockingEvent = blockingEvents[i];
                    var completionDays = 0.0;
                    var IsBlocked = false;
                    if (blockingEvent != null && blockingEvent.BlockingEventPercentage != 0)
                    {
                        var randomVal = ThreadSafeRandom.ThisThreadsRandom.NextDouble();
                        var blockingPercentage = blockingEvent.BlockingEventPercentage / 100;
                        if (randomVal < blockingPercentage)
                        {
                            deliverable.IsBlocked = true;
                            deliverable.WasBlocked = true;
                            IsBlocked = true;
                            // CompletionDays is always a random day between the low and high bounds for the column
                            completionDays = blockingEvent.BlockingEventsPercentageLowBound
                                                + (blockingEvent.BlockingEventsPercentageHighBound - blockingEvent.BlockingEventsPercentageLowBound)
                                                * ThreadSafeRandom.ThisThreadsRandom.NextDouble();
                        }
                    }
                    if (IsBlocked)
                    {
                        deliverable.CompletionDays = currentDay + completionDays;
                        deliverable.IsCalculated = true;
                        var test = deliverable.CompletionDays;
                    }
                }
            }

            // No calucaltion for buffer coulmns, because they only function as buffers
            if (column.IsBuffer == true && deliverable.IsBlocked == false)
            {
                deliverable.CompletionDays = 0.0 + currentDay;
            }

            // Check if someone is working on the deliverables, if not add a new day
            if (column.WIP == 0 && deliverable.IsBlocked == false)
            {
                deliverable.CompletionDays += 1;
                deliverable.StoppedWorkingTime += 1;
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
                if (wipStack.Count < column.WIPMax)
                {
                    wipStack.Add(deliverable);
                }
            }
        }
    }

    private static void MoveDeliverablesToNextColumnIfPossible(List<ColumnModel> columns, Dictionary<ColumnModel, List<DeliverableModel>> columnDeliverables, double currentDay, ColumnModel column, List<DeliverableModel> wipQueue, List<DeliverableModel> wipStack, List<DeliverableModel> deliverablesToMove, List<DefectModel> defects, List<DeliverableModel> deliverablesCopy, List<BlockingEventModel> blockingEvents)
    {
        foreach (var deliverable in deliverablesToMove)
        {
            wipQueue.Remove(deliverable); // remove from current column

            var nextColumnIndex = columns.IndexOf(column) + 1;

            // If there's a next column
            if (nextColumnIndex < columns.Count)
            {
                var nextColumn = columns[nextColumnIndex];

                // Check if next column's WIP limit is not exceeded
                if (columnDeliverables[nextColumn].Count < nextColumn.WIP && deliverable.IsBlocked == false)
                {
                    deliverable.AccumulatedDays = currentDay; // += deliverable.CompletionDays;
                    deliverable.ColumnIndex = nextColumnIndex;
                    deliverable.IsCalculated = false;
                    columnDeliverables[nextColumn].Add(deliverable);

                    // Check if next column is the Done Column
                    // If so, check for a new bug and add to the first column
                    if (nextColumn.Name == "Done" && deliverable.IsBug == false)
                    {
                        for (int i = 0; i < defects.Count; i++)
                        {
                            DefectModel? defect = defects[i];
                            if (defect != null && defect.DefectPercentage != 0)
                            {
                                var randomVal = ThreadSafeRandom.ThisThreadsRandom.NextDouble();
                                var defectPercentage = defect.DefectPercentage / 100;
                                if (randomVal < defectPercentage)
                                {

                                    var newDeliverable = new DeliverableModel
                                    {
                                        Nr = deliverablesCopy.Count + 1,
                                        CompletionDays = 0.0,
                                        AccumulatedDays = 0.0,
                                        IsCalculated = false,
                                        ColumnIndex = 0,
                                        IsBug = true
                                    };
                                    columnDeliverables[columns.First()].Add(newDeliverable);
                                    deliverablesCopy.Add(newDeliverable);
                                    var doneColumn = columns.Last();
                                    doneColumn.WIP += 1;
                                    doneColumn.WIPMax += 1;
                                }
                            }
                        }
                    }
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

    private static void ReallocateWIP(int allocationCount, ColumnModel? allocateToColumn, ColumnModel? allocateFromColumn)
    {
        // If the buffer is full, allocate developers to help testing
        if (allocateToColumn != null && allocateFromColumn != null)
        {
            // Temporarily increase the WIP limit of the "Testing" column
            // Ensure testing does not go over DeveloperColumn WIP
            if (allocateToColumn.WIP <= allocateFromColumn.WIP && allocateFromColumn.WIP > 0)
            {
                allocateToColumn.WIP += allocationCount; // e.g., developersHelping could be a constant or dynamic value
                allocateFromColumn.WIP -= allocationCount; // e.g., developersHelping could be a constant or dynamic value

            }
        }
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
                    { 4, 80 },
                    { 5, 110 },
                    { 6, 135 },
                    { 7, 160 }
                };

        Console.WriteLine($"Current Day: {currentDay}");
        for (var i = 0; i < columns.Count; ++i)
        {
            // Printing each header position
            Console.SetCursorPosition(columnsToPrint[i], 1);
            Console.WriteLine($"{columns[i].Name} WIP: {columns[i].WIP}");

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
                    Console.WriteLine($"T:{deliverableToPrint.Nr} D: {double.Round(deliverableToPrint.CompletionDays)}, W: {double.Round(deliverableToPrint.WaitTime)}, S: {double.Round(deliverableToPrint.StoppedWorkingTime)}");
                    index++;
                }
            }
        }
        // Pause every print
        System.Threading.Thread.Sleep(TimeSpan.FromSeconds(0.03));
    }

    public async Task ColumnEstimateAnalysis(ProjectSimulationModel projectSimModel, int projectSimulationsCount)
    {
        IsCompleted = false;
        const double estimateMultiplier = 0.5;
        var projectWithModifiedEstimates = new List<ProjectSimulationModel> { projectSimModel };
        ProjectColumnMultiplier(projectSimModel, estimateMultiplier, projectWithModifiedEstimates);
        ProjectDefectMultiplier(projectSimModel, estimateMultiplier, projectWithModifiedEstimates);

        int projectsCount = projectWithModifiedEstimates.Count;
        var orderedByTotalDays = await RunSimulationAsync(projectWithModifiedEstimates, projectSimulationsCount);

        var baselineProject = orderedByTotalDays.FirstOrDefault(mcs => mcs.ProjectSimulationModel.Name == projectSimModel.Name);
        var baselineTotalDays = baselineProject!.SimTotalDaysResult!.Percentile(0.9);
        var priority = 1;
        foreach (var MCS in orderedByTotalDays)
        {
            var name = MCS.ProjectSimulationModel.Name;
            var totalDays = MCS.SimTotalDaysResult != null && MCS.SimTotalDaysResult.Any()
                ? MCS.SimTotalDaysResult.Percentile(0.9)
                : 0.0;
            if (totalDays < baselineTotalDays)
            {
                Console.WriteLine($"Simulation {name} is {totalDays}\t\t\t\tdays from {baselineProject.ProjectSimulationModel.Name} {baselineTotalDays}\t\t\tDifference {baselineTotalDays - totalDays}");
                if (priority <= 5 && !ColumnAnalysis!.ContainsKey(priority) && baselineProject != null)
                {
                    var days = double.Round(totalDays, 0);
                    ColumnAnalysis.Add(priority, (name, days));
                }
                priority++;
            }
        }
        IsCompleted = true;
    }

    private static void ProjectDefectMultiplier(ProjectSimulationModel projectSimModel, double estimateMultiplier, List<ProjectSimulationModel> projectWithModifiedEstimates)
    {
        // Loop through each defect and modify EstimatedLowBound and EstimatedHighBound
        for (int i = 0; i < projectSimModel.Defects.Count; i++)
        {
            var defectName = projectSimModel.Defects[i].Name!;
            // Clone the original model
            var newProjectSimModel = projectSimModel.CloneProjectSimModel(projectSimModel, defectName);
            // Modify the estimated bounds of the specific defect
            var defectToModify = newProjectSimModel.Defects[i];
            defectToModify.DefectsPercentageLowBound *= estimateMultiplier;
            defectToModify.DefectsPercentageHighBound *= estimateMultiplier;
            defectToModify.DefectPercentage *= estimateMultiplier;

            projectWithModifiedEstimates.Add(newProjectSimModel);
        }
    }
    private static void ProjectColumnMultiplier(ProjectSimulationModel projectSimModel, double estimateMultiplier, List<ProjectSimulationModel> projectWithModifiedEstimates)
    {
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
    }

    public async Task WIPAnalysis(ProjectSimulationModel projectSimModel, int projectSimulationsCount)
    {
        IsCompleted = false;

        // Rerun the simulation with the modified WIP and get the Results
        for (int k = 1; k <= 5; k++)
        {
            // setting constant for the estimate multiplier
            int wipMultiplier = 1;

            // getting all the projects with modified estimates
            var projectWithModifiedWIP = new List<ProjectSimulationModel> { projectSimModel };
            ProjectWIPMultiplier(projectSimModel, projectWithModifiedWIP, wipMultiplier, "WIP");

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
            Console.WriteLine($"Simulation with the lowest total days: {simulationWithLowestTotalDaysIndex.ProjectSimulationModel.Name} is {double.Round(simulationWithLowestTotalDays, 0)} days from {baselineProject.ProjectSimulationModel.Name} {double.Round(baselineTotalDays, 0)}");
            projectSimModel = simulationWithLowestTotalDaysIndex.ProjectSimulationModel;

            if (!WipAnalysis!.ContainsKey(k) && simulationWithLowestTotalDaysIndex != null)
            {
                var name = simulationWithLowestTotalDaysIndex.ProjectSimulationModel.Name;
                var days = double.Round(baselineTotalDays, 0);
                WipAnalysis.Add(k, (name, days));
            }
            wipMultiplier += 1;

        }
        IsCompleted = true;
    }

    public async Task BlockWIPAnalysis(ProjectSimulationModel projectSimModel, int projectSimulationsCount)
    {
        IsCompleted = false;

        // Rerun the simulation with the modified WIP and get the Results
        for (int k = 1; k <= 5; k++)
        {
            // setting constant for the estimate multiplier
            int wipMultiplier = 5;

            // getting all the projects with modified estimates
            var projectWithModifiedWIP = new List<ProjectSimulationModel> { projectSimModel };
            ProjectWIPMultiplier(projectSimModel, projectWithModifiedWIP, wipMultiplier, $"WIP-{wipMultiplier}", true);

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
            Console.WriteLine($"Simulation with the lowest total days: {simulationWithLowestTotalDaysIndex.ProjectSimulationModel.Name} is {double.Round(simulationWithLowestTotalDays, 0)} days from {baselineProject.ProjectSimulationModel.Name} {double.Round(baselineTotalDays, 0)}");
            projectSimModel = simulationWithLowestTotalDaysIndex.ProjectSimulationModel;
            wipMultiplier += 1;

        }
        IsCompleted = true;
    }
    private static void ProjectWIPMultiplier(ProjectSimulationModel projectSimModel, List<ProjectSimulationModel> projectWithModifiedEstimates, int multiplier, string name, bool? BlockWIPAnalysis = false)
    {
        if (BlockWIPAnalysis == true)
        {

            // Loop through each column and modify WIP
            for (int i = 0; i < projectSimModel.Columns.Count; i++)
            {
                var columnName = projectSimModel.Columns[i].Name! + name;
                var column = projectSimModel.Columns[i];
                // Clone the original model
                if (column.IsBuffer == true)
                {
                    var newProjectSimModel = projectSimModel.CloneProjectSimModel(projectSimModel, columnName);
                    // Modify the estimated bounds of the specific column
                    var columnToModify = newProjectSimModel.Columns[i];
                    columnToModify.WIP = columnToModify.WIP - multiplier <= 0 ? 1 : columnToModify.WIP - multiplier;
                    columnToModify.WIPMax = columnToModify.WIPMax - multiplier <= 0 ? 1 : columnToModify.WIPMax - multiplier;
                    projectWithModifiedEstimates.Add(newProjectSimModel);
                }
            }
        }
        else
        {
            // Loop through each column and modify WIP
            for (int i = 0; i < projectSimModel.Columns.Count; i++)
            {
                var columnName = projectSimModel.Columns[i].Name! + name;
                // Clone the original model
                var newProjectSimModel = projectSimModel.CloneProjectSimModel(projectSimModel, columnName);
                // Modify the estimated bounds of the specific column
                var columnToModify = newProjectSimModel.Columns[i];
                columnToModify.WIP += multiplier;
                columnToModify.WIPMax += multiplier;
                projectWithModifiedEstimates.Add(newProjectSimModel);
            }
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
                var MCS = new MonteCarloSimulation();
                MCS.InitiateAndRunSimulation(projectSimModels, projectSimulationsCount, new Guid());
                return MCS;
            }));
        };

        // Wait for all tasks to finish and print results
        var results = await Task.WhenAll(tasks);
        // Order the results by the average total days
        var orderedByTotalDays = results.OrderBy(r => r.SimTotalDaysResult!.Percentile(0.9)).ToArray();
        return orderedByTotalDays;
    }

    private static bool IsBottleneck(ColumnModel bufferColumn, List<DeliverableModel> deliverablesInBuffer)
    {
        return deliverablesInBuffer.Count >= bufferColumn.WIP;
    }

    public IMonteCarloSimulation GetSimulationInstance()
    {
        return new MonteCarloSimulation();
    }

    public (List<ColumnModel>, List<DeliverableModel>) RunSimulationStep(ProjectSimulationModel projectSimulationModel, int currentDay)
    {
        var simulationList = new List<DeliverableModel>();

        if (ColumnDeliverables.Any(kvp => kvp.Value.Count > 0))
        {
            simulationList = MonteCarloSimulation.RunStepSlowSimulation(projectSimulationModel.Backlog, DeliverablesCopy, projectSimulationModel.Columns, currentDay, ColumnDeliverables, projectSimulationModel.Defects, projectSimulationModel.BlockingEvents);
            return (ProjectSimulationModel.Columns, simulationList);
        }
        IsCompleted = true;
        return (ProjectSimulationModel.Columns, simulationList);
    }


    private static List<DeliverableModel> RunStepSlowSimulation(
                                      BacklogModel backlog,
                                      List<DeliverableModel> deliverablesCopy,
                                      List<ColumnModel> columns,
                                      int currentDay,
                                      Dictionary<ColumnModel, List<DeliverableModel>> columnDeliverables,
                                      List<DefectModel> defects,
                                      List<BlockingEventModel> blockingEvents
                                      )
    {

        // Increment the current day for each iteration
        double currentSimDay = currentDay;
        const int developersHelping = 1;
        int originalTestingProdWIP = columns.First(c => c.Name == Rdy4TestProd).WIP;
        int originalTestingDevWIP = columns.First(c => c.Name == TestStage).WIP;
        int originalDevelopersWIP = columns.First(c => c.Name == InProgress).WIP;

        // While there are deliverables in the system
        if (columnDeliverables.Any(kvp => kvp.Value.Count > 0))
        {
            foreach (var column in columns)
            {
                var wipQueue = columnDeliverables[column];
                var wipStack = new List<DeliverableModel>();

                var backlogColumn = columns.FirstOrDefault(c => c.Name == Backlog);
                var openColumn = columns.FirstOrDefault(c => c.Name == Open);
                var testingProdColumn = columns.FirstOrDefault(c => c.Name == Rdy4TestProd);
                var testingDevColumn = columns.FirstOrDefault(c => c.Name == TestStage);
                var developerColumn = columns.FirstOrDefault(c => c.Name == InProgress);

                // Check if the "Ready for Test Stage" column is full
                // if so allocate developers to help testing
                var readyForTestDevColumn = columns.FirstOrDefault(c => c.Name == Rdy4Test);
                if (readyForTestDevColumn != null
                    && IsBottleneck(readyForTestDevColumn, columnDeliverables[readyForTestDevColumn])
                    && testingDevColumn?.WIP <= testingDevColumn?.WIPMax
                    )
                {
                    if (developerColumn.WIP == 0)
                    {
                        ReallocateWIP(developersHelping, testingDevColumn, testingProdColumn);

                    }
                    else
                    {
                        ReallocateWIP(developersHelping, testingDevColumn, developerColumn);
                    }
                }


                // Check if the "Ready for Test on Prod" column is full
                // if so allocate developers to help testing
                var readyForTestProdColumn = columns.FirstOrDefault(c => c.Name == AwaitDplyProd);
                if (readyForTestProdColumn != null
                    && IsBottleneck(readyForTestProdColumn, columnDeliverables[readyForTestProdColumn])
                    && testingProdColumn?.WIP <= testingProdColumn?.WIPMax
                    )
                {
                    if (developerColumn.WIP == 0)
                    {
                        ReallocateWIP(developersHelping, testingProdColumn, testingDevColumn);

                    }
                    else
                    {
                        ReallocateWIP(developersHelping, testingProdColumn, developerColumn);

                    }
                }

                // We use a new list to keep track of deliverables to move to next column
                var deliverablesToMove = new List<DeliverableModel>();

                // Calculate the deliverable completion day and add to list 
                CalculateDeliverableCompletionDayANDAddToList(backlog, currentSimDay, column, wipQueue, wipStack, deliverablesToMove, defects, blockingEvents);

                // Check how many are working on the deliverable in columns and add a day to the ones that are postponed for the next day.
                AddWaitingTimeForDeliverablesInQueue(column, wipQueue);


                // Move deliverables to the next column if possible
                MoveDeliverablesToNextColumnIfPossible(columns, columnDeliverables, currentSimDay, column, wipQueue, wipStack, deliverablesToMove, defects, deliverablesCopy, blockingEvents);


                int devStack = developerColumn != null && columnDeliverables.ContainsKey(developerColumn) && columnDeliverables[developerColumn] != null ? columnDeliverables[developerColumn].Count : 0;

                // Check if the "Ready for Test Stage" column is NOT full anymore and give back WIP
                if (readyForTestDevColumn != null
                    && !IsBottleneck(readyForTestDevColumn, columnDeliverables[readyForTestDevColumn])
                    && testingDevColumn?.WIP <= testingDevColumn?.WIPMax
                    && devStack != 0
                    )
                {
                    ReallocateWIP(developersHelping, developerColumn, testingDevColumn);
                }

                // Check if the "Ready for Test on Prod" column is NOT full anymore and give back WIP
                if (readyForTestProdColumn != null
                    && !IsBottleneck(readyForTestProdColumn, columnDeliverables[readyForTestProdColumn])
                    && testingProdColumn?.WIP <= testingProdColumn?.WIPMax
                    && devStack != 0
                    )
                {
                    ReallocateWIP(developersHelping, developerColumn, testingProdColumn);
                }

                int testDevStack = testingDevColumn != null && columnDeliverables.ContainsKey(testingDevColumn) && columnDeliverables[testingDevColumn] != null ? columnDeliverables[testingDevColumn].Count : 0;
                int rdyForTestDevStack = readyForTestDevColumn != null && columnDeliverables.ContainsKey(readyForTestDevColumn) && columnDeliverables[readyForTestDevColumn] != null ? columnDeliverables[readyForTestDevColumn].Count : 0;
                int backlogStack = backlogColumn != null && columnDeliverables.ContainsKey(backlogColumn) && columnDeliverables[backlogColumn] != null ? columnDeliverables[backlogColumn].Count : 0;
                int openStack = openColumn != null && columnDeliverables.ContainsKey(openColumn) && columnDeliverables[openColumn] != null ? columnDeliverables[openColumn].Count : 0;
                int testProdStack = testingProdColumn != null && columnDeliverables.ContainsKey(testingProdColumn) && columnDeliverables[testingProdColumn] != null ? columnDeliverables[testingProdColumn].Count : 0;

                // if Testing on Stage is not doing any work, reallocate back
                if (testDevStack < testingDevColumn?.WIP && devStack != 0)
                {
                    ReallocateWIP(developersHelping, developerColumn, testingDevColumn);
                }

                // if Testing on Prod is not doing any work, reallocate back
                if (testProdStack < testingProdColumn?.WIP && devStack != 0)
                {
                    ReallocateWIP(developersHelping, developerColumn, testingProdColumn);
                }

                // if Testing on Stage is not doing any work, reallocate back
                if (testDevStack < testingDevColumn?.WIP && devStack == 0 && openStack > 0)
                {
                    ReallocateWIP(developersHelping, developerColumn, testingDevColumn);
                }

                // if Testing on Prod is not doing any work, reallocate back
                if (testProdStack < testingProdColumn?.WIP && devStack == 0 && openStack > 0)
                {
                    ReallocateWIP(developersHelping, developerColumn, testingProdColumn);
                }

                // if there is no work "In Progress" reallocate to test on stage (>20 days otherwise it will be too soon)
                if (devStack == 0 && rdyForTestDevStack > 0 && backlogStack == 0 && currentSimDay > 20)
                {
                    ReallocateWIP(developersHelping, testingDevColumn, developerColumn);
                }

                // if there is no work "In Progress" and no work in "Test Stage" reallocate to test prod (>20 days otherwise it will be too soon)
                if (devStack == 0 && testDevStack == 0 && rdyForTestDevStack == 0 && backlogStack == 0 && currentSimDay > 20)
                {
                    ReallocateWIP(developersHelping, testingProdColumn, testingDevColumn);
                }
                // if there is no work "In Progress" and no work in "Testing on  Prod" reallocate to test on Stage (>20 days otherwise it will be too soon)
                if (devStack == 0 && testProdStack == 0 && rdyForTestDevStack != 0 && backlogStack == 0 && currentSimDay > 20)
                {
                    ReallocateWIP(developersHelping, testingDevColumn, testingProdColumn);
                }
            }
            var orderdedByAccumulated = deliverablesCopy.OrderBy(d => d.AccumulatedDays).ToList();
            return orderdedByAccumulated;
        }

        return deliverablesCopy;
    }

    public void InitiateSimulation(ProjectSimulationModel projectSimulationModel, Guid simulationId)
    {
        SimulationId = simulationId;

        ProjectSimulationModel = projectSimulationModel;

        InitiateDeliverablesAndColumns(projectSimulationModel);
    }

    private void InitiateDeliverablesAndColumns(ProjectSimulationModel projectSimulationModel)
    {
        var staff = projectSimulationModel.Staff;
        var startDate = projectSimulationModel.StartDate;
        var targetDate = projectSimulationModel.TargetDate;
        var revenue = projectSimulationModel.Revenue;
        var cost = projectSimulationModel.Costs;
        var backlog = projectSimulationModel.Backlog;
        var columns = projectSimulationModel.Columns;

        ValidateProps(staff, revenue, cost, projectSimulationModel.Backlog);

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
        // Simulation logic
        // For each deliverable, simulate the probability of 
        // completion for each column holding the contrains of 
        // the WIPs in each column

        DeliverablesCopy = projectSimulationModel.Backlog.Deliverables.Select(d => new DeliverableModel
        {
            Id = d.Id,
            Nr = d.Nr,
            CompletionDays = d.CompletionDays,
            AccumulatedDays = d.AccumulatedDays
        }).ToList();


        // List of deliverables in each column
        ColumnDeliverables = columns.ToDictionary(c => c, c => new List<DeliverableModel>());

        // Initialize deliverables
        foreach (var deliverable in DeliverablesCopy)
        {
            var firstColumn = columns.First();
            ColumnDeliverables[firstColumn].Add(deliverable);
        }
        IsCompleted = false;
    }

    public void ResetSimulation()
    {
        // Set the deliverables to column index 0
        DeliverablesCopy.Clear();
        ColumnDeliverables.Clear();
        foreach (var column in ProjectSimulationModel.Columns)
        {
            ColumnDeliverables.Add(column, new List<DeliverableModel>());
        }
        InitiateDeliverablesAndColumns(ProjectSimulationModel);

    }
}
