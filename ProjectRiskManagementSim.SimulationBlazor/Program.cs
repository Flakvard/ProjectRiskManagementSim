using Microsoft.AspNetCore.Http.HttpResults;
using ProjectRiskManagementSim.SimulationBlazor.Components;
using ProjectRiskManagementSim.ProjectSimulation.Interfaces;
using ProjectRiskManagementSim.ProjectSimulation;
using ProjectRiskManagementSim.SimulationBlazor.Routes;
using ProjectRiskManagementSim.SimulationBlazor.Lib;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddRazorComponents()
    //.AddInteractiveServerComponents()
    .AddHtmx();

// Dependency Injection
builder.Services.AddSingleton<IMonteCarloSimulation, MonteCarloSimulation>();

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
    // Initiate the database with a SimulationManager instance
    IMonteCarloSimulation monteCarloSimulation = new MonteCarloSimulation();
    SimulationManager.InitSimulationsFromDb(monteCarloSimulation);
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
