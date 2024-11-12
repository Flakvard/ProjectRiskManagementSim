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

        var simProject = contextSimulation.GetProjectById(project.JiraId);
        if (simProject == null)
        {
            Name = project.Name;
            ProjectCategory = project.ProjectCategory;
            JiraId = project.JiraId;
            JiraProjectId = project.JiraProjectId;
            IssueLeadTimes = await contextAnalytics.GetIssueLeadTimesByProjectAsync(project.Name);
            if (IssueLeadTimes == null || IssueLeadTimes.Count == 0)
            {
                IssueLeadTimes = new List<IssueLeadTime>(); // Ensure it’s an empty list instead of null
            }

            await CalculateCount(contextAnalytics);

            CalculatePercentiles();
        }
        else
        {
            // map the last inserted simualtion to the CreateNewProjectHandler
            var listOfSimProject = await contextSimulation.GetProjectSimulationsAsync(simProject.Id);
            ProjectSimulationModel lastSimProject = listOfSimProject.Last();
            MapProjectBackToModal(lastSimProject.Project, lastSimProject);

        }
    }
    private async Task UpdateAndFetchProjectInfor(int simProjectId, OxygenAnalyticsContext contextAnalytics, OxygenSimulationContext contextSimulation)
    {
        SimProjectId = simProjectId;
        if (SimProjectId != 0)
        {
            var simProjectModel = await contextSimulation.GetSimulationByIdAsync(SimProjectId);
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
        if (IssueLeadTimes == null)
        {
            IssuesCount = issuesCount;
            IssuesDoneCount = issuesDoneCount;
            EpicCount = epicCount;
            BugCount = bugCount;
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
        }
        IssuesCount = issuesCount;
        EpicCount = epicCount;
        BugCount = bugCount;
        IssuesDoneCount = issuesDoneCount;
        if (issuesDoneCount > 0)
        {
            BugPercentage = (double)bugCount / issuesDoneCount * 100;

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
    }

    private List<double> CalculatePercentile(List<double> values)
    {
        values.Sort();
        var lowerBound = values.Percentile(0.05);
        var upperBound = values.Percentile(0.95);
        return new List<double> { lowerBound, upperBound };
    }
}
