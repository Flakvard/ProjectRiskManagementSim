using ProjectRiskManagementSim.DataAccess;
using ProjectRiskManagementSim.DataAccess.Models;

public class CreateNewProjectHandler
{
    public int ProjectId { get; set; }
    public string? Name { get; set; }
    public string? ProjectCategory { get; set; }
    public string? Manager { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string JiraId { get; set; }
    public string JiraProjectId { get; set; }
    public List<IssueLeadTime> IssueLeadTimes { get; set; }
    public int IssuesCount { get; set; }
    public int EpicCount { get; set; }
    public int BugCount { get; set; }

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


    public async Task InitializeProjectsAsync(OxygenAnalyticsContext context)
    {
        await FetchProjectInfo(context);
    }
    private async Task FetchProjectInfo(OxygenAnalyticsContext context)
    {
        // var projects = await context.Projects.ToListAsync();
        var project = await context.GetProjectByIdAsync(ProjectId);
        if (project == null)
        {
            // Handle the case where the project is not found
            throw new InvalidOperationException($"Project with ID {ProjectId} not found.");
        }
        Name = project.Name;
        ProjectCategory = project.ProjectCategory;
        JiraId = project.JiraId;
        JiraProjectId = project.JiraProjectId;
        IssueLeadTimes = await context.GetIssueLeadTimesByProjectAsync(project.Name);
        if (IssueLeadTimes == null || IssueLeadTimes.Count == 0)
        {
            IssueLeadTimes = new List<IssueLeadTime>(); // Ensure it’s an empty list instead of null
        }

        CalculateCount();

        CalculatePercentiles();
    }
    private void CalculateCount()
    {
        var issuesCount = 0;
        var epicCount = 0;
        var bugCount = 0;
        foreach (var issueLeadTime in IssueLeadTimes)
        {
            if (issueLeadTime.IssueType == "Task" || issueLeadTime.IssueType == "Sub-task" || issueLeadTime.IssueType == "Subtask" || issueLeadTime.IssueType == "Tech Task")
            {
                issuesCount += 1;
            }
            else if (issueLeadTime.IssueType == "Bug" || issueLeadTime.IssueType == "Sub-bug")
            {
                bugCount += 1;
            }
            else if (issueLeadTime.IssueType == "Epic")
            {
                epicCount += 1;
            }
        }
        IssuesCount = issuesCount;
        EpicCount = epicCount;
        BugCount = bugCount;
    }
    private void CalculatePercentiles()
    {
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
