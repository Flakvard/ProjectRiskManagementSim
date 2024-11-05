using ProjectRiskManagementSim.SimulationBlazor.Models;

namespace ProjectRiskManagementSim.SimulationBlazor.Data;

public static class Database
{
    public static List<ViewProjectSimulationModel> ProjectSimulations { get; set; } = new List<ViewProjectSimulationModel>();

    public static ViewProjectSimulationModel ProjectModelInit()
    {

        var deliverableModel = new List<DeliverableModel>();
        for (var i = 1; i < 36; i++)
        {
            deliverableModel.Add(new DeliverableModel
            {
                Id = Guid.NewGuid(),
                Nr = i
            });
        }
        var backLogModel = new BacklogModel { Deliverables = deliverableModel, PercentageLowBound = 0.0, PercentageHighBound = 1 };
        var wip = 30;
        ViewProjectSimulationModel projectData = new ViewProjectSimulationModel
        {
            Name = "Baseline",
            Staff = new List<StaffModel>
                  {
                  new StaffModel { Name = "John Doe", Role = Role.ProjectManager, Sale = 1000, Cost = 370, Days = 20 },
                  new StaffModel { Name = "Jane Doe", Role = Role.FrontendDeveloper, Sale = 1000, Cost = 370, Days = 20 },
                  new StaffModel { Name = "Jack Doe", Role = Role.BackendDeveloper, Sale = 1000, Cost = 370, Days = 20 },
                  new StaffModel { Name = "Jill Doe", Role = Role.SoftwareTester, Sale = 1000, Cost = 370, Days = 20 },
                  new StaffModel { Name = "Jim Doe", Role = Role.UXUIDesigner, Sale = 1000, Cost = 370, Days = 20 }
                  },
            // 74 days beween start and target date
            StartDate = new DateTime(2024, 1, 1),
            TargetDate = new DateTime(2024, 3, 15),
            Revenue = new RevenueModel { Amount = 480000 },
            Costs = new CostModel { Cost = 177600, Days = 20 },
            Backlog = backLogModel,
            // Bech Bruun model
            Columns = new List<ViewColumnModel>
                  {
                  new ViewColumnModel(wip: backLogModel.Deliverables.Count) { Name = "Backlog", IsBuffer=true, EstimatedLowBound = 1,
                  EstimatedHighBound = 54 },
                  new ViewColumnModel(wip: wip, wipMax: wip) { Name = "Open", IsBuffer=true, EstimatedLowBound = 1, EstimatedHighBound
                  = 54 },
                  new ViewColumnModel(wip: 5, wipMax: 5) { Name = "In Progress", EstimatedLowBound = 1, EstimatedHighBound = 47 },
                  new ViewColumnModel(wip: wip, wipMax: wip) { Name = "Rdy4Test", IsBuffer=true, EstimatedLowBound = 1,
                  EstimatedHighBound = 50 },
                  new ViewColumnModel(wip: 0, wipMax: 2) { Name = "Test Stage", EstimatedLowBound = 1, EstimatedHighBound = 11 },
                  new ViewColumnModel(wip: wip, wipMax: wip) { Name = "Await Dply Prod", IsBuffer=true, EstimatedLowBound = 1,
                  EstimatedHighBound = 22 },
                  new ViewColumnModel(wip: 0, wipMax: 2) { Name = "Rdy4TestProd", EstimatedLowBound = 1, EstimatedHighBound = 54 },
                  new ViewColumnModel(wip: backLogModel.Deliverables.Count) { Name = "Done", IsBuffer=true, EstimatedLowBound = 1,
                  EstimatedHighBound = 54 }
                  },
        };

        return projectData;

    }
}
