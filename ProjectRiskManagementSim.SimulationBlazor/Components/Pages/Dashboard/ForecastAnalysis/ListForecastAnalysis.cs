using System.Globalization;
using ProjectRiskManagementSim.DataAccess;
using ProjectRiskManagementSim.DataAccess.Models;
using ProjectRiskManagementSim.ProjectSimulation.Interfaces;
using ProjectRiskManagementSim.SimulationBlazor.Lib;
using ProjectRiskManagementSim.SimulationBlazor.Models;

namespace ProjectRiskManagementSim.SimulationBlazor.Components.Pages.Dashboard.ForecastAnalysis;

public class ListForeCastAnalysis
{
    public List<ForecastAnalysisModel> ForecastAnalysis { get; set; }
    public ProjectSimulationModel? Simulation { get; set; }
    public IMonteCarloSimulation? MonteCarloSimulation { get; set; }
    public Guid SimulationId { get; set; }
    public SimResultsModel? SimulationResults { get; set; }
    public List<ForecastModel?> Forecasts { get; set; }
    public ListForeCastAnalysis(ProjectSimulationModel? simulation, IMonteCarloSimulation monteCarloSimulation)
    {
        Simulation = simulation;
        MonteCarloSimulation = monteCarloSimulation;
        if (Simulation != null && Simulation.Forecasts.Count > 0)
        {
            ForecastAnalysis = Simulation.Forecasts.Select(f => new ForecastAnalysisModel
            {
                Percentage = f.Percentage,
                EndDate = f.EndDate.ToString("dd MMM yyyy"),
                Days = f.Days,
                Cost = f.Cost.ToString("N0"),
                DaysDelay = f.DaysDelay,
                CostOfDelay = f.CostOfDelay.ToString("N0"),
            }).ToList();
        }
        else
        {
            ForecastAnalysis = ForecastAnalysisModel.InitialSimulationResults();
        }
    }
    public ListForeCastAnalysis()
    {
        ForecastAnalysis = ForecastAnalysisModel.InitialSimulationResults();
    }
    // Run the simulation forcast analysis
    public async Task RunSimulationAnalysis(OxygenSimulationContext context, int dbSimulationId)
    {
        Simulation = await context.GetSimulationByIdAsync(dbSimulationId);
        var mappedProjectData = ModelMapper.MapDBProjectSimulationModelToProjectSimulationModel(Simulation!);
        var simulationId = Guid.NewGuid(); // Create a unique ID for this simulation session
        SimulationId = simulationId; // Store the simulationId for later  

        // Use the injected IMonteCarloSimulation instance to get a new simulation instance
        MonteCarloSimulation.InitiateAndRunSimulation(mappedProjectData, 5000, simulationId);

        // Store the simulation instance with the simulationId for later retrieval
        SimulationManager.AddSimulationToManager(simulationId, MonteCarloSimulation);
        UpdateForecastAnalysis();

        // Update the database with the simulation results
        if (SimulationResults != null)
        {
            Simulation!.SimulationCosts = SimulationResults.CostsResult;
            Simulation!.SimulationDays = SimulationResults.DaysResult;
            Simulation!.SimEndDate = SimulationResults.EndDate;
            Simulation!.SimulationDaysOfDelay = SimulationResults.DaysDelay;
            await context.UpdateSimulationAsync(Simulation);
        }
        // Add or Update the Forecast in database
        var simForecast = Simulation!.Forecasts.ToList();
        if (simForecast.Count == 0)
        {
            var forecasts = Forecasts.Select(f => new ForecastModel
            {
                Percentage = f!.Percentage,
                EndDate = f.EndDate,
                Days = f.Days,
                Cost = f.Cost,
                DaysDelay = f.DaysDelay,
                CostOfDelay = f.CostOfDelay,
                ProjectSimulationModelId = Simulation!.Id
            }).ToList();
            context.ForecastModel.AddRange(forecasts);
            context.SaveChanges();
        }
        else
        {
            for (int i = 0; i < Forecasts.Count; i++)
            {
                if (Forecasts[i] == null)
                    continue;

                simForecast[i].Percentage = Forecasts[i]!.Percentage;
                simForecast[i].EndDate = Forecasts[i]!.EndDate;
                simForecast[i].Days = Forecasts[i]!.Days;
                simForecast[i].Cost = Forecasts[i]!.Cost;
                simForecast[i].DaysDelay = Forecasts[i]!.DaysDelay;
                simForecast[i].CostOfDelay = Forecasts[i]!.CostOfDelay;
            }
            context.ForecastModel.UpdateRange(simForecast);
            context.SaveChanges();
        }

    }
    public void UpdateForecastAnalysis()
    {
        var simulation = ModelMapper.MapDBProjectSimulationModelToProjectSimulationModel(Simulation!);
        var simulationResults = SimulationManager.GetSimulation(SimulationId);

        if (simulationResults == null)
        {
            return;
        }

        var listOfPercentileMarks = new List<double>
        {
          0.99,
          0.95,
          0.90,
          0.85,
          0.80,
          0.75,
          0.70,
          0.65,
          0.60,
          0.55,
          0.50,
          0.45,
          0.40
        };

        var listOfDayPercentileMarks = listOfPercentileMarks.Select(percentile => simulationResults.SimTotalDaysResult.Percentile(percentile)).ToList();
        var listOfEndDays = listOfDayPercentileMarks.Select(days => simulation.StartDate.AddDays((int)days)).ToList();
        var listOfCosts = listOfPercentileMarks.Select(percentile => simulationResults.SimTotalCostsResult.Percentile(percentile)).ToList();
        var listOfCostOfDelays = listOfPercentileMarks.Select(percentile => (double)Simulation.CostPrDay * (listOfEndDays[listOfPercentileMarks.IndexOf(percentile)] - simulation.TargetDate).Days).ToList();

        var forecastAnalysis = new List<ForecastAnalysisModel>();
        var forecastAnalysisModelDb = new List<ForecastModel>();
        for (int i = 0; i < listOfPercentileMarks.Count(); i++)
        {
            var percentile = listOfPercentileMarks[i];
            var forecastAnalysisModel = new ForecastAnalysisModel
            {
                Percentage = listOfPercentileMarks[i].ToString("P0"),
                EndDate = listOfEndDays[i].ToString("dd MMM yyyy"),
                Days = (int)listOfDayPercentileMarks[i],
                Cost = listOfCosts[i].ToString("N0"),
                DaysDelay = listOfEndDays[i].Subtract(simulation.TargetDate).Days,
                CostOfDelay = listOfCostOfDelays[i].ToString("N0")
            };
            var forcastModelDb = new ForecastModel
            {
                Percentage = listOfPercentileMarks[i].ToString("P0"),
                EndDate = listOfEndDays[i],
                Days = (int)listOfDayPercentileMarks[i],
                Cost = listOfCosts[i],
                DaysDelay = listOfEndDays[i].Subtract(simulation.TargetDate).Days,
                CostOfDelay = listOfCostOfDelays[i],
                ProjectSimulationModelId = Simulation!.Id
            };
            if (percentile == 0.90)
            {
                SimulationResults = new SimResultsModel
                {
                    CostsResult = listOfCosts[i],
                    DaysResult = listOfDayPercentileMarks[i],
                    EndDate = listOfEndDays[i],
                    DaysDelay = listOfEndDays[i].Subtract(simulation.TargetDate).Days,
                };
            }
            forecastAnalysis.Add(forecastAnalysisModel);
            forecastAnalysisModelDb.Add(forcastModelDb);

        }
        ForecastAnalysis = forecastAnalysis;
        Forecasts = forecastAnalysisModelDb;

    }
}

public class ForecastAnalysisModel
{
    public string Percentage { get; set; } = "";
    public string EndDate { get; set; } = "Month";
    public int Days { get; set; }
    public string Cost { get; set; } = "0";
    public double DaysDelay { get; set; }
    public string CostOfDelay { get; set; } = "0";
    public static List<ForecastAnalysisModel> InitialSimulationResults()
    {
        return new List<ForecastAnalysisModel>
        {
            new ForecastAnalysisModel { Percentage = "99%", EndDate = "Month", Days = 0, Cost = "0", CostOfDelay = "0", DaysDelay = 0},
            new ForecastAnalysisModel { Percentage = "95%", EndDate = "Month", Days = 0, Cost = "0", CostOfDelay = "0",  DaysDelay = 0},
            new ForecastAnalysisModel { Percentage = "90%", EndDate = "Month", Days = 0, Cost = "0", CostOfDelay = "0",  DaysDelay = 0},
            new ForecastAnalysisModel { Percentage = "85%", EndDate = "Month", Days = 0, Cost = "0", CostOfDelay = "0",  DaysDelay = 0},
            new ForecastAnalysisModel { Percentage = "80%", EndDate = "Month", Days = 0, Cost = "0", CostOfDelay = "0",  DaysDelay = 0},
            new ForecastAnalysisModel { Percentage = "75%", EndDate = "Month", Days = 0, Cost = "0", CostOfDelay = "0",  DaysDelay = 0},
            new ForecastAnalysisModel { Percentage = "70%", EndDate = "Month", Days = 0, Cost = "0", CostOfDelay = "0",  DaysDelay = 0},
        };
    }
}
public class SimResultsModel
{
    public double CostsResult { get; set; }
    public double DaysResult { get; set; }
    public DateTime EndDate { get; set; }
    public double DaysDelay { get; set; }
}

