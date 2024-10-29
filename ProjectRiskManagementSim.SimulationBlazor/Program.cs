using Microsoft.AspNetCore.Http.HttpResults;
using ProjectRiskManagementSim.SimulationBlazor.Components;
using ProjectRiskManagementSim.ProjectSimulation.Interfaces;
using ProjectRiskManagementSim.ProjectSimulation;
using ProjectRiskManagementSim.SimulationBlazor.Routes;
using ProjectRiskManagementSim.SimulationBlazor.Lib;
using ProjectRiskManagementSim.SimulationBlazor.Components.Pages.RunSimulation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddRazorComponents()
    //.AddInteractiveServerComponents()
    .AddHtmx();

// Register as Scoped or Transient depending on how you want the lifecycle to work
builder.Services.AddScoped<SimulationManager>();
builder.Services.AddScoped<IMonteCarloSimulation, MonteCarloSimulation>();
builder.Services.AddScoped<RunSimulationHandler>(); // Register RunSimulationHandler

// Dictionary to store handlers per simulationId
builder.Services.AddSingleton<IDictionary<Guid, RunSimulationHandler>>(new Dictionary<Guid, RunSimulationHandler>());
builder.Services.AddScoped<Func<Guid, RunSimulationHandler>>(serviceProvider =>
{
    var handlers = serviceProvider.GetRequiredService<IDictionary<Guid, RunSimulationHandler>>();
    return simulationId =>
    {
        if (!handlers.TryGetValue(simulationId, out var handler))
        {
            handler = ActivatorUtilities.CreateInstance<RunSimulationHandler>(serviceProvider);
            handler.SetSimulationId(simulationId);
            handlers[simulationId] = handler;
        }
        return handler;
    };
});

// Dictionary to store project selection from modal
builder.Services.AddSingleton<IDictionary<Guid, ProjectListViewModel>>(new Dictionary<Guid, ProjectListViewModel>());
builder.Services.AddScoped<Func<Guid, ProjectListViewModel>>(serviceProvider =>
{
    var handlers = serviceProvider.GetRequiredService<IDictionary<Guid, ProjectListViewModel>>();
    return simulationId =>
    {
        if (!handlers.TryGetValue(simulationId, out var handler))
        {
            handler = ActivatorUtilities.CreateInstance<ProjectListViewModel>(serviceProvider);
            handler.SetProjectListViewModelId(simulationId);
            handlers[simulationId] = handler;
        }
        return handler;
    };
});

builder.Services.AddInfrastructureServices(Configuration);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();
app.UseHtmxAntiforgery();

app.MapRazorComponents<App>()
    //.AddInteractiveServerRenderMode()
    .AddHtmxorComponentEndpoints(app);


app.MapPageRoutes();


app.Run();
