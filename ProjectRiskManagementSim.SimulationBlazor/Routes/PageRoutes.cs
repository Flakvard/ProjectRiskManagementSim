using ProjectRiskManagementSim.SimulationBlazor.Components;
using ProjectRiskManagementSim.SimulationBlazor.Components.Shared;
using ProjectRiskManagementSim.SimulationBlazor.Components.Pages.Simulations;
using ProjectRiskManagementSim.SimulationBlazor.Components.Pages.Dashboard;
using ProjectRiskManagementSim.SimulationBlazor.Models;
using ProjectRiskManagementSim.SimulationBlazor.Lib;
using Microsoft.AspNetCore.Http.HttpResults;
using ProjectRiskManagementSim.ProjectSimulation.Interfaces;


namespace ProjectRiskManagementSim.SimulationBlazor.Routes;

public static class PageRoutes
{
    public static WebApplication MapPageRoutes(this WebApplication app)
    {
        app.MapGet("/htmx-test",
            () => new RazorComponentResult<HtmxTest>());

        app.MapGet("/simulations",
            () => new RazorComponentResult<Simulations>());

        app.MapPost("/start-simulation", (ProjectSimulationModel projectData, IMonteCarloSimulation MCS) =>
        {
            var simulationId = Guid.NewGuid(); // Create a unique ID for this simulation session
            var mappedProjectData = ModelMapper.MapToProjectSimulationModel(projectData);

            // Use the injected IMonteCarloSimulation instance to get a new simulation instance
            var simulation = MCS.GetSimulationInstance();
            simulation.InitiateAndRunSimulation(mappedProjectData);

            // Store the simulation instance with the simulationId for later retrieval
            SimulationManager.StartSimulation(simulationId, simulation);

            // Generate HTML content to return
            var htmlContent = $@"
        <div>
            <h2>Simulation Started</h2>
            <p>Simulation ID: {simulationId}</p>
        </div>";

            return Results.Content(htmlContent, "text/html");
        });

        app.MapGet("/simulation-progress/{simulationId}", (Guid simulationId) =>
        {
            var simulation = SimulationManager.GetSimulation(simulationId);

            if (simulation == null)
            {
                return Results.NotFound("Simulation not found.");
            }

            // Return current state (e.g., current day, deliverables, column states)
            //var currentState = simulation.GetCurrentState();  // You'd define this method to get current simulation data
            //return Results.Ok(currentState);
            return Results.Ok();
        });
        return app;
    }
}
