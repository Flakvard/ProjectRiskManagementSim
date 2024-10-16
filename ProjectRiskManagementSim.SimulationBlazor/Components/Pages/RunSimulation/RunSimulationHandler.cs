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
            (var updatedColumns, var updatedDeliverables) = MCS.RunSimulationStep(MCS.ProjectSimulationModel, currentDay);
            var mappedUpdatedDeliverables = updatedDeliverables
                .Select(ModelMapper.MapToBlazorDeliverableModel)
                .ToList();
            var mappedUpdatedColumns = updatedColumns
                .Select(ModelMapper.MapToBlazorColumnModel)
                .ToList();

            // Update the UI
            UpdateBoard(mappedUpdatedColumns, mappedUpdatedDeliverables);

            // Simulate delay between steps (like Thread.Sleep in console)
            await Task.Delay(100);
        }

        simulationRunning = false;
    }

    private void UpdateBoard(List<ColumnModel> mappedUpdatedColumns, List<DeliverableModel> updatedDeliverables)
    {
        // Update the columns
        foreach (var column in mappedUpdatedColumns)
        {
            var existingColumn = columns.FirstOrDefault(c => c.Name == column.Name);
            if (existingColumn != null)
            {
                existingColumn.EstimatedLowBound = column.EstimatedLowBound;
                existingColumn.EstimatedHighBound = column.EstimatedHighBound;
                existingColumn.WIP = column.WIP;
                existingColumn.WIPMax = column.WIPMax;
                existingColumn.IsBuffer = column.IsBuffer;
            }
        }

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
    public void ResetSimulation()
    {
        // Reset the current day
        currentDay = 0;

        // Fetch the simulation from the simulation manager again (or reset if applicable)
        MCS = SimulationManager.GetFirstSimulation();

        // Reset columns
        columns = MCS.ProjectSimulationModel.Columns
            .Select(ModelMapper.MapToBlazorColumnModel)
            .ToList();

        // Reset deliverables
        deliverables = MCS.ProjectSimulationModel.Backlog.Deliverables
            .Select(ModelMapper.MapToBlazorDeliverableModel)
            .ToList();

        // Clear the column deliverables
        columnDeliverables = columns.ToDictionary(c => c, c => new List<DeliverableModel>());

        // Reset the simulationRunning state
        simulationRunning = false;
    }
}
