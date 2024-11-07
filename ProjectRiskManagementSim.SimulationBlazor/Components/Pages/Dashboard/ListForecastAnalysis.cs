using ProjectRiskManagementSim.DataAccess.Models;
using ProjectRiskManagementSim.ProjectSimulation.Interfaces;

namespace ProjectRiskManagementSim.SimulationBlazor.Components.Pages.Dashboard;

public class ListForeCastAnalysis
{
    public List<ForecastAnalysisModel> ForecastAnalysis { get; set; }
    public ProjectSimulationModel? Simulation { get; set; }
    public IMonteCarloSimulation MonteCarloSimulation { get; set; }
    public ListForeCastAnalysis(ProjectSimulationModel? simulation, IMonteCarloSimulation monteCarloSimulation)
    {
        ForecastAnalysis = ForecastAnalysisModel.InitialSimulationResults();
        Simulation = simulation;
        MonteCarloSimulation = monteCarloSimulation;
    }
}
public class ForecastAnalysisModel
{
    public string Percentage { get; set; }
    public string EndDate { get; set; }
    public int Days { get; set; }
    public string Cost { get; set; }
    public string CostOfDelay { get; set; }
    public static List<ForecastAnalysisModel> InitialSimulationResults()
    {
        return new List<ForecastAnalysisModel>
        {
            new ForecastAnalysisModel { Percentage = "99%", EndDate = "Des 2024", Days = 35, Cost = "1.150.000", CostOfDelay = "15.000" },
            new ForecastAnalysisModel { Percentage = "95%", EndDate = "Nov 2024", Days = 20, Cost = "1.110.000", CostOfDelay = "10.000" },
            new ForecastAnalysisModel { Percentage = "90%", EndDate = "Nov 2024", Days = 10, Cost = "1.070.000", CostOfDelay = "7.000" },
            new ForecastAnalysisModel { Percentage = "85%", EndDate = "Nov 2024", Days = 0, Cost = "1.000.000", CostOfDelay = "0" },
            new ForecastAnalysisModel { Percentage = "80%", EndDate = "Okt 2024", Days = -10, Cost = "970.000", CostOfDelay = "-7.000" },
            new ForecastAnalysisModel { Percentage = "75%", EndDate = "Okt 2024", Days = -21, Cost = "900.000", CostOfDelay = "-14.000" },
            new ForecastAnalysisModel { Percentage = "70%", EndDate = "Okt 2024", Days = -22, Cost = "890.000", CostOfDelay = "-15.000" },
            new ForecastAnalysisModel { Percentage = "65%", EndDate = "Okt 2024", Days = -23, Cost = "880.000", CostOfDelay = "-16.000" },
            new ForecastAnalysisModel { Percentage = "60%", EndDate = "Okt 2024", Days = -24, Cost = "870.000", CostOfDelay = "-17.000" },
            new ForecastAnalysisModel { Percentage = "55%", EndDate = "Okt 2024", Days = -25, Cost = "860.000", CostOfDelay = "-18.000" },
            new ForecastAnalysisModel { Percentage = "50%", EndDate = "Okt 2024", Days = -26, Cost = "850.000", CostOfDelay = "-19.000" },
            new ForecastAnalysisModel { Percentage = "45%", EndDate = "Okt 2024", Days = -27, Cost = "840.000", CostOfDelay = "-20.000" },
            new ForecastAnalysisModel { Percentage = "40%", EndDate = "Okt 2024", Days = -28, Cost = "830.000", CostOfDelay = "-21.000" }
        };
    }
}


