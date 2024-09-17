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
        var cost = _projectSimulationModel.Costs;
        var deliverables = _projectSimulationModel.Deliverables;

        ValidateProps(staff, revenue, cost, deliverables);

        var days = (targetDate - startDate).TotalDays;

        var totalRevenue = 0.0;
        var totalCost = 0.0;
        for (var i = 0; i < staff.Count; i++)
        {
            var staffMember = staff[i];
            var staffDays = staffMember.Days;
            var staffRevenue = staffMember.Sale * staffDays;
            var staffCost = staffMember.Cost * staffDays;
            totalRevenue += staffRevenue;
            totalCost += staffCost;
        }
        var revenuePerDay = revenue.Amount / days;
        var costPerDay = cost.Cost / days;

        Console.WriteLine($"Total Revenue: {totalRevenue}");
        Console.WriteLine($"Total Cost: {totalCost}");
        Console.WriteLine($"Total Deliverables: {deliverables.Count}");

    }
    static void ValidateProps(List<StaffModel>? staff, RevenueModel? revenue, CostModel cost, List<DeliverableModel> deliverables)
    {
        if (staff == null || revenue == null || cost == null || deliverables == null)
        {
            throw new InvalidOperationException("Staff, Revenue, Cost, and Deliverables must be set.");
        }
    }
}
