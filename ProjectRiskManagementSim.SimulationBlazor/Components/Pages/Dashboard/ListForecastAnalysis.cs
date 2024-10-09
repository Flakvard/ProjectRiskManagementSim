namespace ProjectRiskManagementSim.SimulationBlazor.Components.Pages.Dashboard;

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
            new ForecastAnalysisModel { Percentage = "75%", EndDate = "Mai 2024", Days = -21, Cost = "900.000", CostOfDelay = "0" }
        };
    }
}


