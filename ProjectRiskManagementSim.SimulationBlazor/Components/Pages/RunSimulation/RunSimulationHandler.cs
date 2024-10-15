using ProjectRiskManagementSim.ProjectSimulation.Interfaces;
using ProjectRiskManagementSim.SimulationBlazor.Lib;
using ProjectRiskManagementSim.SimulationBlazor.Models;

namespace ProjectRiskManagementSim.SimulationBlazor.Components.Pages.RunSimulation;

public class RunSimulationHandler
{
    public IMonteCarloSimulation MCS { get; set; }
    public List<ColumnModel> columns { get; set; }
    public List<DeliverableModel> deliverables { get; set; }
    public Dictionary<ColumnModel, List<DeliverableModel>> columnDeliverables { get; set; }
    public int currentDay { get; set; } = 0;
    public bool simulationRunning { get; set; } = false;

    public RunSimulationHandler()
    {
        // Initialize from the Monte Carlo simulation, this is shared throughout the simulation
        MCS = SimulationManager.GetFirstSimulation();
        columns = MCS.ProjectSimulationModel.Columns
            .Select(ModelMapper.MapToBlazorColumnModel)
            .ToList();

        deliverables = MCS.ProjectSimulationModel.Backlog.Deliverables
            .Select(ModelMapper.MapToBlazorDeliverableModel)
            .ToList();

        // Initialize columnDeliverables as empty for now
        columnDeliverables = columns.ToDictionary(c => c, c => new List<DeliverableModel>());
    }

    public async Task StartSimulation()
    {
        simulationRunning = true;

        // Simulate day-by-day and update the Kanban board
        while (!MCS.IsCompleted)
        {
            currentDay++;

            // Simulate one step of the Monte Carlo Simulation
            var updatedDeliverables = MCS.RunSimulationStep(MCS.ProjectSimulationModel, currentDay);
            var mappedUpdatedDeliverables = updatedDeliverables
                .Select(ModelMapper.MapToBlazorDeliverableModel)
                .ToList();

            // Update the UI
            UpdateBoard(mappedUpdatedDeliverables);

            // Simulate delay between steps (like Thread.Sleep in console)
            await Task.Delay(100);
        }

        simulationRunning = false;
    }

    private void UpdateBoard(List<DeliverableModel> updatedDeliverables)
    {
        // Clear only the deliverables inside the columns, not the columns themselves
        foreach (var column in columns)
        {
            columnDeliverables[column].Clear();
        }

        // Place updated deliverables into their respective columns
        foreach (var deliverable in updatedDeliverables)
        {
            var columnIndex = deliverable.ColumnIndex;
            if (columnIndex >= 0 && columnIndex < columns.Count)
            {
                columnDeliverables[columns[columnIndex]].Add(deliverable);
            }
        }
    }
}
