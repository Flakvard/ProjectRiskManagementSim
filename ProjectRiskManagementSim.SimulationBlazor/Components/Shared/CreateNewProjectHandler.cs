using ProjectRiskManagementSim.DataAccess;
using ProjectRiskManagementSim.DataAccess.Models;

public class CreateNewProjectHandler
{
    public int ProjectId { get; set; }
    public int SimProjectId { get; set; } = 0;
    public string? Name { get; set; }
    public string? ProjectCategory { get; set; }
    public string? Manager { get; set; }
    public string? JiraId { get; set; }
    public string? JiraProjectId { get; set; }
    public List<IssueLeadTime>? IssueLeadTimes { get; set; }
    public int IssuesCount { get; set; }
    public int IssuesDoneCount { get; set; }
    public int EpicCount { get; set; }
    public int BugCount { get; set; }
    public double BugPercentage { get; set; }

    public double AwaitCustPercentage { get; set; }
    public double AwaitTaskPercentage { get; set; }
    public double Await3pPercentage { get; set; }

    public int AwaitCustCount { get; set; }
    public int AwaitTaskCount { get; set; }
    public int Await3pCount { get; set; }

    // Properties for percentiles
    public List<double> OpenPercentiles { get; set; } = new List<double> { 0, 0 };
    public List<double> InProgressPercentiles { get; set; } = new List<double> { 0, 0 };
    public List<double> ReopenedPercentiles { get; set; } = new List<double> { 0, 0 };
    public List<double> ResolvedPercentiles { get; set; } = new List<double> { 0, 0 };
    public List<double> ClosedPercentiles { get; set; } = new List<double> { 0, 0 };
    public List<double> BacklogPercentiles { get; set; } = new List<double> { 0, 0 };
    public List<double> SelectedForDevelopmentPercentiles { get; set; } = new List<double> { 0, 0 };
    public List<double> DonePercentiles { get; set; } = new List<double> { 0, 0 };
    public List<double> FinishedPercentiles { get; set; } = new List<double> { 0, 0 };
    public List<double> ReadyForTestOnStagePercentiles { get; set; } = new List<double> { 0, 0 };
    public List<double> AwaitingCustomerPercentiles { get; set; } = new List<double> { 0, 0 };
    public List<double> AwaitingThirdPartyPercentiles { get; set; } = new List<double> { 0, 0 };
    public List<double> AwaitingTaskPercentiles { get; set; } = new List<double> { 0, 0 };
    public List<double> TestingOnStagePercentiles { get; set; } = new List<double> { 0, 0 };
    public List<double> WaitingForDeploymentToProductionPercentiles { get; set; } = new List<double> { 0, 0 };
    public List<double> ReadyToTestOnProductionPercentiles { get; set; } = new List<double> { 0, 0 };
    public List<double> AnsweredPercentiles { get; set; } = new List<double> { 0, 0 };
    public List<double> FailedTestPercentiles { get; set; } = new List<double> { 0, 0 };
    public List<double> ReadyForTestOnDevPercentiles { get; set; } = new List<double> { 0, 0 };
    public List<double> TestingOnDevPercentiles { get; set; } = new List<double> { 0, 0 };
    public List<double> WaitingForDeploymentToStagePercentiles { get; set; } = new List<double> { 0, 0 };
    public List<double> ToDoPercentiles { get; set; } = new List<double> { 0, 0 };
    public List<double> AwaitingEstimationPercentiles { get; set; } = new List<double> { 0, 0 };
    public List<double> InRefinementPercentiles { get; set; } = new List<double> { 0, 0 };
    public List<double> AwaitingApprovalPercentiles { get; set; } = new List<double> { 0, 0 };
    public List<double> InDesignPercentiles { get; set; } = new List<double> { 0, 0 };
    public List<double> AwaitingRefinementPercentiles { get; set; } = new List<double> { 0, 0 };
    public List<double> OnHoldPercentiles { get; set; } = new List<double> { 0, 0 };
    public List<double> CreatedPercentiles { get; set; } = new List<double> { 0, 0 };

    public List<double> CycleTimePercentiles { get; set; } = new List<double> { 0, 0 };
    public List<double> BugCycleTimePercentiles { get; set; } = new List<double> { 0, 0 };
    public List<double> AwaitCustPercentiles { get; set; } = new List<double> { 0, 0 };
    public List<double> AwaitTaskPercentiles { get; set; } = new List<double> { 0, 0 };
    public List<double> Await3PPercentiles { get; set; } = new List<double> { 0, 0 };

    public List<IssueLeadTime> ListOfBugIssues { get; set; } = new List<IssueLeadTime>();
    public List<IssueLeadTime> ListOfAwaitCust { get; set; } = new List<IssueLeadTime>();
    public List<IssueLeadTime> ListOfAwaitTask { get; set; } = new List<IssueLeadTime>();
    public List<IssueLeadTime> ListOfAwait3P { get; set; } = new List<IssueLeadTime>();


    public DateTime StartDate { get; set; } = new DateTime();
    public DateTime? SimEndDate { get; set; }
    public DateTime? TargetDate { get; set; } = new DateTime();
    public List<IssueModel> Issues { get; set; } = new List<IssueModel>();
    public double TotalHours { get; set; }
    public double TotalCost { get; set; }
    public double TotalRevenue { get; set; }
    public DateTime FirstIssueDate { get; set; }
    public DateTime LastIssueDate { get; set; }
    public double? ActualRevenue { get; private set; }
    public double? BudgetCosts { get; private set; }
    public double? ActualCosts { get; private set; }
    public double? SimulationCosts { get; private set; }
    public double? CostPrDay { get; private set; }
    public double FrontendDevs { get; private set; }
    public double BackendDevs { get; private set; }
    public double Testers { get; private set; }
    public double? ActualDays { get; private set; }
    public double TargetDays { get; private set; }
    public double? SimulationDays { get; private set; }
    public double? SimulationDaysOfDelay { get; private set; }
    public double? ActualHours { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public double DeliverablesCount { get; private set; }
    public double PercentageLowBound { get; private set; }
    public double PercentageHighBound { get; private set; }
    public string NameOfSim { get; set; } = "";
    public bool AlreadySimulated { get; set; } = false;

    public async Task InitializeProjectsAsync(OxygenAnalyticsContext context, OxygenSimulationContext context1)
    {
        await FetchProjectInfo(context, context1);
    }
    private async Task FetchProjectInfo(OxygenAnalyticsContext contextAnalytics, OxygenSimulationContext contextSimulation)
    {
        if (SimProjectId != 0)
        {
            var simProjectModel = await contextSimulation.GetSimulationByIdAsync(SimProjectId);

            if (simProjectModel != null && simProjectModel.Name != null)
            {
                NameOfSim = simProjectModel.Name;
                MapProjectBackToModal(simProjectModel!.Project, simProjectModel);
                AlreadySimulated = true;
                return;
            }
        }
        // var projects = await context.Projects.ToListAsync();
        var project = await contextAnalytics.GetProjectByIdAsync(ProjectId);
        if (project == null)
        {
            // Handle the case where the project is not found
            throw new InvalidOperationException($"Project with ID {ProjectId} not found.");
        }
        IssueLeadTimes = await contextAnalytics.GetIssueLeadTimesByProjectAsync(project.Name);
        if (IssueLeadTimes == null || IssueLeadTimes.Count == 0)
        {
            IssueLeadTimes = new List<IssueLeadTime>(); // Ensure it’s an empty list instead of null
        }

        var simProject = contextSimulation.GetProjectById(project.JiraId);
        if (simProject == null)
        {
            Name = project.Name;
            ProjectCategory = project.ProjectCategory;
            JiraId = project.JiraId;
            JiraProjectId = project.JiraProjectId;



            await CalculateCount(contextAnalytics);

            CalculatePercentiles();
            AlreadySimulated = false;
            return;
        }
        else
        {
            // map the last inserted simualtion to the CreateNewProjectHandler
            var listOfSimProject = await contextSimulation.GetProjectSimulationsAsync(simProject.Id);
            ProjectSimulationModel lastSimProject = listOfSimProject.Last();
            await CalculateCount(contextAnalytics);
            CalculatePercentiles();
            AlreadySimulated = true;
            MapProjectBackToModal(lastSimProject.Project, lastSimProject);
            return;

        }
    }
    public async Task UpdateAndFetchProjectInfor(int simProjectId, OxygenAnalyticsContext contextAnalytics, OxygenSimulationContext contextSimulation)
    {
        SimProjectId = simProjectId;
        if (SimProjectId != 0)
        {

            var simProjectModel = await contextSimulation.GetSimulationByIdAsync(SimProjectId);

            IssueLeadTimes = await contextAnalytics.GetIssueLeadTimesByProjectAsync(simProjectModel.Project.Name);
            if (IssueLeadTimes == null || IssueLeadTimes.Count == 0)
            {
                IssueLeadTimes = new List<IssueLeadTime>(); // Ensure it’s an empty list instead of null
            }

            await CalculateCount(contextAnalytics);
            CalculatePercentiles();
            MapProjectBackToModal(simProjectModel!.Project, simProjectModel);
            return;
        }

    }

    private void MapProjectBackToModal(ProjectModel project, ProjectSimulationModel lastSimProject)
    {
        // Map to CreateNewProjectHandler
        ProjectId = project.Id;
        Name = project.Name;
        StartDate = lastSimProject.StartDate;
        TargetDate = lastSimProject.TargetDate;
        JiraId = project.JiraId;
        JiraProjectId = project.JiraProjectId;
        TotalRevenue = (double)lastSimProject.ActualRevenue;
        BudgetCosts = lastSimProject.BudgetCosts;
        TotalCost = (double)lastSimProject.ActualCosts;
        SimulationCosts = lastSimProject.SimulationCosts;
        CostPrDay = lastSimProject.CostPrDay;
        FrontendDevs = lastSimProject.FrontendDevs;
        BackendDevs = lastSimProject.BackendDevs;
        Testers = lastSimProject.Testers;
        ActualDays = lastSimProject.ActualDays;
        TargetDays = lastSimProject.TargetDays;
        SimulationDays = lastSimProject.SimulationDays;
        SimulationDaysOfDelay = lastSimProject.SimulationDaysOfDelay;
        ActualHours = lastSimProject.ActualHours;
        CreatedAt = lastSimProject.CreatedAt;
        UpdatedAt = lastSimProject.UpdatedAt;
        DeliverablesCount = lastSimProject.DeliverablesCount;
        IssuesCount = (int)lastSimProject.IssueCount;
        PercentageLowBound = lastSimProject.PercentageLowBound;
        PercentageHighBound = lastSimProject.PercentageHighBound;

        EpicCount = (int)lastSimProject.DeliverablesCount;
        IssuesDoneCount = (int)lastSimProject.IssueDoneCount;
        BugCount = (int)lastSimProject.BugCount;
        BugPercentage = lastSimProject.BugPercentage;

        // Map the last inserted simulation to the CreateNewProjectHandler

        var projectLowBound = lastSimProject.Columns.Select(x => x.EstimatedLowBound).ToList();
        var projectHighBound = lastSimProject.Columns.Select(x => x.EstimatedHighBound).ToList();
        BacklogPercentiles = new List<double> { projectLowBound[0], projectHighBound[0] };
        OpenPercentiles = new List<double> { projectLowBound[1], projectHighBound[1] };
        InProgressPercentiles = new List<double> { projectLowBound[2], projectHighBound[2] };
        AwaitingCustomerPercentiles = new List<double> { projectLowBound[3], projectHighBound[3] };
        FinishedPercentiles = new List<double> { projectLowBound[4], projectHighBound[4] };
        ReadyForTestOnStagePercentiles = new List<double> { projectLowBound[5], projectHighBound[5] };
        TestingOnStagePercentiles = new List<double> { projectLowBound[6], projectHighBound[6] };
        WaitingForDeploymentToProductionPercentiles = new List<double> { projectLowBound[7], projectHighBound[7] };
        ReadyToTestOnProductionPercentiles = new List<double> { projectLowBound[8], projectHighBound[8] };
        DonePercentiles = new List<double> { projectLowBound[9], projectHighBound[9] };
    }

    private async Task CalculateCount(OxygenAnalyticsContext context)
    {
        var issuesCount = 0;
        var epicCount = 0;
        var bugCount = 0;
        var issuesDoneCount = 0;
        var awaitCust = 0;
        var awaitTask = 0;
        var await3P = 0;
        if (IssueLeadTimes == null)
        {
            IssuesCount = issuesCount;
            IssuesDoneCount = issuesDoneCount;
            EpicCount = epicCount;
            BugCount = bugCount;
            AwaitCustCount = awaitCust;
            AwaitTaskCount = awaitTask;
            Await3pCount = await3P;
            return;
        }
        foreach (var issueLeadTime in IssueLeadTimes)
        {
            if (issueLeadTime.IssueType == "Task" || issueLeadTime.IssueType == "Sub-task" || issueLeadTime.IssueType == "Subtask" || issueLeadTime.IssueType == "Tech Task")
            {
                issuesCount += 1;
                if (issueLeadTime.CurrentStatus == "Done" || issueLeadTime.CurrentStatus == "Closed" || issueLeadTime.CurrentStatus == "Resolved")
                {
                    issuesDoneCount += 1;
                }
            }
            else if (issueLeadTime.IssueType == "Bug" || issueLeadTime.IssueType == "Sub-bug")
            {
                bugCount += 1;
                ListOfBugIssues.Add(issueLeadTime);
            }
            else if (issueLeadTime.IssueType == "Epic")
            {
                epicCount += 1;
            }
            var issueModel = await context.GetIssueById(issueLeadTime.IssueId);
            if (issueModel != null)
            {
                Issues.Add(issueModel);
            }
            var issueCreatedDate = new DateTime();
            if (issueLeadTime.CreatedDate != null)
            {
                issueCreatedDate = DateTime.Parse(issueLeadTime.CreatedDate);
            }
            if (FirstIssueDate == DateTime.MinValue || issueCreatedDate < FirstIssueDate)
            {
                FirstIssueDate = issueCreatedDate;
                StartDate = issueCreatedDate;
            }
            var issueUpdatedDate = new DateTime();
            if (issueLeadTime.LastUpdated != null)
            {
                issueUpdatedDate = DateTime.Parse(issueLeadTime.LastUpdated);
            }
            if (LastIssueDate == DateTime.MinValue || issueUpdatedDate > LastIssueDate)
            {
                LastIssueDate = issueUpdatedDate;
            }
            if (issueLeadTime.AwaitingCustomer > 0)
            {
                awaitCust += 1;
                ListOfAwaitCust.Add(issueLeadTime);
            }
            if (issueLeadTime.AwaitingTask > 0)
            {
                awaitTask += 1;
                ListOfAwaitTask.Add(issueLeadTime);
            }
            if (issueLeadTime.AwaitingThirdParty > 0)
            {
                await3P += 1;
                ListOfAwait3P.Add(issueLeadTime);
            }
        }
        IssuesCount = issuesCount;
        EpicCount = epicCount;
        BugCount = bugCount;
        IssuesDoneCount = issuesDoneCount;
        AwaitCustCount = awaitCust;
        AwaitTaskCount = awaitTask;
        Await3pCount = await3P;
        if (issuesDoneCount > 0)
        {
            BugPercentage = (double)bugCount / issuesDoneCount * 100;

        }
        else
        {
            BugPercentage = 0; // Default to 0 if no issues are done
        }

        if (issuesCount > 0)
        {
            AwaitCustPercentage = ((double)awaitCust / issuesCount) * 100;
            AwaitTaskPercentage = ((double)awaitTask / issuesCount) * 100;
            Await3pPercentage = ((double)await3P / issuesCount) * 100;
        }
        var totalHours = Issues.Sum(x => x.TimeSpentSeconds) / 3600;
        TotalHours = totalHours != null ? (double)totalHours : 0.0;
        var totalCost = Issues.Sum(x => x.TimeSpentSeconds) / 3600 * 370;
        TotalCost = totalCost != null ? (double)totalCost : 0.0;
        var totalRevenue = Issues.Sum(x => x.TimeSpentSeconds) / 3600 * 1175;
        TotalRevenue = totalRevenue != null ? (double)totalRevenue : 0.0;

    }
    private void CalculatePercentiles()
    {
        if (IssueLeadTimes == null || IssueLeadTimes.Count == 0)
        {
            return;
        }
        OpenPercentiles = CalculatePercentile(IssueLeadTimes.Select(x => x.Open).ToList());
        InProgressPercentiles = CalculatePercentile(IssueLeadTimes.Select(x => x.InProgress).ToList());
        ReopenedPercentiles = CalculatePercentile(IssueLeadTimes.Select(x => x.Reopened).ToList());
        ResolvedPercentiles = CalculatePercentile(IssueLeadTimes.Select(x => x.Resolved).ToList());
        ClosedPercentiles = CalculatePercentile(IssueLeadTimes.Select(x => x.Closed).ToList());
        BacklogPercentiles = CalculatePercentile(IssueLeadTimes.Select(x => x.Backlog).ToList());
        SelectedForDevelopmentPercentiles = CalculatePercentile(IssueLeadTimes.Select(x => x.SelectedForDevelopment).ToList());
        DonePercentiles = CalculatePercentile(IssueLeadTimes.Select(x => x.Done).ToList());
        FinishedPercentiles = CalculatePercentile(IssueLeadTimes.Select(x => x.Finished).ToList());
        ReadyForTestOnStagePercentiles = CalculatePercentile(IssueLeadTimes.Select(x => x.ReadyForTestOnStage).ToList());
        AwaitingCustomerPercentiles = CalculatePercentile(IssueLeadTimes.Select(x => x.AwaitingCustomer).ToList());
        AwaitingThirdPartyPercentiles = CalculatePercentile(IssueLeadTimes.Select(x => x.AwaitingThirdParty).ToList());
        AwaitingTaskPercentiles = CalculatePercentile(IssueLeadTimes.Select(x => x.AwaitingTask).ToList());
        TestingOnStagePercentiles = CalculatePercentile(IssueLeadTimes.Select(x => x.TestingOnStage).ToList());
        WaitingForDeploymentToProductionPercentiles = CalculatePercentile(IssueLeadTimes.Select(x => x.WaitingForDeploymentToProduction).ToList());
        ReadyToTestOnProductionPercentiles = CalculatePercentile(IssueLeadTimes.Select(x => x.ReadyToTestOnProduction).ToList());
        AnsweredPercentiles = CalculatePercentile(IssueLeadTimes.Select(x => x.Answered).ToList());
        FailedTestPercentiles = CalculatePercentile(IssueLeadTimes.Select(x => x.FailedTest).ToList());
        ReadyForTestOnDevPercentiles = CalculatePercentile(IssueLeadTimes.Select(x => x.ReadyForTestOnDev).ToList());
        TestingOnDevPercentiles = CalculatePercentile(IssueLeadTimes.Select(x => x.TestingOnDev).ToList());
        WaitingForDeploymentToStagePercentiles = CalculatePercentile(IssueLeadTimes.Select(x => x.WaitingForDeploymentToStage).ToList());
        ToDoPercentiles = CalculatePercentile(IssueLeadTimes.Select(x => x.ToDo).ToList());
        AwaitingEstimationPercentiles = CalculatePercentile(IssueLeadTimes.Select(x => x.AwaitingEstimation).ToList());
        InRefinementPercentiles = CalculatePercentile(IssueLeadTimes.Select(x => x.InRefinement).ToList());
        AwaitingApprovalPercentiles = CalculatePercentile(IssueLeadTimes.Select(x => x.AwaitingApproval).ToList());
        InDesignPercentiles = CalculatePercentile(IssueLeadTimes.Select(x => x.InDesign).ToList());
        AwaitingRefinementPercentiles = CalculatePercentile(IssueLeadTimes.Select(x => x.AwaitingRefinement).ToList());
        OnHoldPercentiles = CalculatePercentile(IssueLeadTimes.Select(x => x.OnHold).ToList());
        CreatedPercentiles = CalculatePercentile(IssueLeadTimes.Select(x => x.Created).ToList());

        // Filter out null CycleTime values before calculating percentiles
        CycleTimePercentiles = CalculatePercentile(
            IssueLeadTimes
                .Where(x => x.CycleTime.HasValue) // Filters out null CycleTime values
                .Select(x => x.CycleTime!.Value)   // Selects only the non-null values
                .ToList()
          );
        BugCycleTimePercentiles = CalculatePercentile(
            ListOfBugIssues
                .Where(x => x.CycleTime.HasValue) // Filters out null CycleTime values
                .Select(x => x.CycleTime!.Value)   // Selects only the non-null values
                .ToList()
          );
        AwaitCustPercentiles = CalculatePercentile(
            ListOfAwaitCust
            .Where(x => x.AwaitingCustomer > 0) // Filters out null CycleTime values
                .Select(x => x.AwaitingCustomer)   // Selects only the non-null values
                .ToList()
            );
        var cycleTimeLowBound = CycleTimePercentiles[0];
        var cycleTimeHighBound = CycleTimePercentiles[1];
        var bugCycleTimeLow = BugCycleTimePercentiles[0];
        var bugCycleTimeHigh = BugCycleTimePercentiles[1];

        // Safely calculate multipliers with checks for zero values to avoid NaN results
        double bugCycleTimeMultiplierLow = (bugCycleTimeLow != 0) ? (bugCycleTimeLow / cycleTimeLowBound) * 100 : 0;
        double bugCycleTimeMultiplierHigh = (bugCycleTimeHigh != 0) ? (bugCycleTimeHigh / cycleTimeHighBound) * 100 : 0;

        // Update BugCycleTimePercentiles list
        BugCycleTimePercentiles = new List<double> { bugCycleTimeMultiplierLow, bugCycleTimeMultiplierHigh };





        AwaitTaskPercentiles = CalculatePercentile(
            ListOfAwaitTask
            .Where(x => x.AwaitingTask > 0) // Filters out null CycleTime values
                .Select(x => x.AwaitingTask)   // Selects only the non-null values
                .ToList()
            );
        Await3PPercentiles = CalculatePercentile(
            ListOfAwait3P
            .Where(x => x.AwaitingThirdParty > 0) // Filters out null CycleTime values
                .Select(x => x.AwaitingThirdParty)   // Selects only the non-null values
                .ToList()
            );


    }

    private List<double> CalculatePercentile(List<double> values)
    {
        if (values.Count == 0)
            return new List<double> { 0, 0 };
        values.Sort();
        var lowerBound = values.Percentile(0.05);
        var upperBound = values.Percentile(0.95);
        return new List<double> { lowerBound, upperBound };
    }
}
