using Microsoft.AspNetCore.Http.HttpResults;
using ProjectRiskManagementSim.SimulationBlazor.Components;
using ProjectRiskManagementSim.SimulationBlazor.Components.Shared;
using ProjectRiskManagementSim.SimulationBlazor.Components.Pages.Simulations;
using ProjectRiskManagementSim.SimulationBlazor.Components.Pages.Dashboard;
using ProjectRiskManagementSim.ProjectSimulation;
using ProjectRiskManagementSim.SimulationBlazor.Lib;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddRazorComponents()
    //.AddInteractiveServerComponents()
    .AddHtmx();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();
app.UseHtmxAntiforgery();

app.MapRazorComponents<App>()
    //.AddInteractiveServerRenderMode()
    .AddHtmxorComponentEndpoints(app);

    // Initialize and run the simulation (in the background or with some state tracking)
    var simulation = new MonteCarloSimulation(projectData, 100);

    // Store the simulation instance with the simulationId for later retrieval
    SimulationManager.StartSimulation(simulationId, simulation);

    return Results.Ok(new { SimulationId = simulationId });
});

app.MapGet("/simulation-progress/{simulationId}", (Guid simulationId) =>
{
    var simulation = SimulationManager.GetSimulation(simulationId);

    if (simulation == null)
    {
        return Results.NotFound("Simulation not found.");
    }

    // Return current state (e.g., current day, deliverables, column states)
    var currentState = simulation.GetCurrentState();  // You'd define this method to get current simulation data
    return Results.Ok(currentState);
});

app.Run();
