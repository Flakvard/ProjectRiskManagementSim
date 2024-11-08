using ProjectRiskManagementSim.DataAccess;
using ProjectRiskManagementSim.ProjectSimulation.Interfaces;
using ProjectRiskManagementSim.SimulationBlazor.Components.Pages.Simulations;
using ProjectRiskManagementSim.SimulationBlazor.Lib;
using ProjectRiskManagementSim.SimulationBlazor.Models;

namespace ProjectRiskManagementSim.SimulationBlazor.Components.Pages.RunSimulation;

public class RunSimulationHandler
{
    public IMonteCarloSimulation MCS { get; set; }
    public List<ViewColumnModel> columns { get; set; }
    public List<DeliverableModel> deliverables { get; set; }
    public Dictionary<ViewColumnModel, List<DeliverableModel>> columnDeliverables { get; set; }
    public int currentDay { get; set; } = 0;
    public bool simulationRunning { get; set; } = false;
    private SimulationManager _simulationManager { get; set; }
    public Guid SimulationId { get; private set; }

    // Inject SimulationManager via constructor
    public RunSimulationHandler(SimulationManager simulationManager, IMonteCarloSimulation monteCarloSimulation)
    {
        _simulationManager = simulationManager;
        SimulationId = Guid.NewGuid(); // Generate a new ID for each instance
        MCS = monteCarloSimulation;
        // await InitializeSimulation(OxygenSimulationContext context, int dbSimulationId);
    }

    public void SetSimulationId(Guid id)
    {
        SimulationId = id;
        // InitializeSimulation();
    }

    public async Task InitializeSimulation(OxygenSimulationContext context, int dbSimulationId)
    {
        var simulation = await context.GetSimulationByIdAsync(dbSimulationId);
        var mappedProjectData = ModelMapper.MapDBProjectSimulationModelToProjectSimulationModel(simulation!);
        var simulationId = Guid.NewGuid(); // Create a unique ID for this simulation session
        SimulationId = simulationId; // Store the simulationId for later  
        MCS.InitiateSimulation(mappedProjectData, simulationId);
        SimulationManager.AddSimulationToManager(simulationId, MCS);

        // Initialize from the Monte Carlo simulation, this is shared throughout the simulation
        MCS = _simulationManager.GetFirstSimulation(SimulationId); // Pass the simulation ID to get the unique simulation instance
        columns = MCS.ProjectSimulationModel.Columns
            .Select(ModelMapper.MapToBlazorColumnModel)
            .ToList();
        deliverables = MCS.ProjectSimulationModel.Backlog.Deliverables
          .Select(ModelMapper.MapToBlazorDeliverableModel)
          .ToList();

    }

    public void RunningSimulation()
    {
        MCS = _simulationManager.GetFirstSimulation(SimulationId); // Pass the simulation ID to get the unique simulation instance
        SimulationManager.AddSimulationToManager(SimulationId, MCS);

    }


    public async Task StartSimulation()
    {
        MCS.IsCompleted = false;
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
            columns = mappedUpdatedColumns;
            deliverables = mappedUpdatedDeliverables;
            columnDeliverables = columns.ToDictionary(c => c, c => new List<DeliverableModel>());

            // Update the UI
            UpdateBoard(mappedUpdatedColumns, mappedUpdatedDeliverables);

            // Simulate delay between steps (like Thread.Sleep in console)
            await Task.Delay(100);
        }

        simulationRunning = false;
    }

    private void UpdateBoard(List<ViewColumnModel> mappedUpdatedColumns, List<DeliverableModel> updatedDeliverables)
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
            if (columnDeliverables.ContainsKey(column))
            {
                columnDeliverables[column].Clear();
            }

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
        //MCS = _simulationManager.GetFirstSimulation(SimulationId);
        MCS.ResetSimulation();

        // Reset columns & deliverables
        columns = MCS.ProjectSimulationModel.Columns
          .Select(ModelMapper.MapToBlazorColumnModel)
          .ToList();
        deliverables = MCS.ProjectSimulationModel.Backlog.Deliverables
          .Select(ModelMapper.MapToBlazorDeliverableModel)
          .ToList();
        // Initialize columnDeliverables as empty for now
        columnDeliverables = columns.ToDictionary(c => c, c => new List<DeliverableModel>());

        // Reset the simulationRunning state
        simulationRunning = false;
        MCS.IsCompleted = true;
    }
}
