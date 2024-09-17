namespace ProjectRiskManagementSim.ProjectSimulation;

internal class MonteCarloSimulation
{
    private readonly ProjectSimulationModel _projectSimulationModel;
    private readonly Random _random = new Random();

    public MonteCarloSimulation(ProjectSimulationModel projectSimulationModel)
    {
        _projectSimulationModel = projectSimulationModel;
    }

    public void RunSimulation()
    {
        var staff = _projectSimulationModel.Staff;
        var startDate = _projectSimulationModel.StartDate;
        var targetDate = _projectSimulationModel.TargetDate;
        var revenue = _projectSimulationModel.Revenue;

        if (staff == null || revenue == null)
        {
            throw new InvalidOperationException("Staff and Revenue must be set before running simulation");
        }

        var days = (targetDate - startDate).TotalDays;
        var revenuePerDay = revenue.Amount / days;

        var totalRevenue = 0.0;
        for (var i = 0; i < staff.Count; i++)
        {
            var staffMember = staff[i];
            var staffDays = staffMember.Days;
            var staffRevenue = staffDays * revenuePerDay;
            totalRevenue += staffRevenue;
        }

        Console.WriteLine($"Total Revenue: {totalRevenue}");
    }
}
