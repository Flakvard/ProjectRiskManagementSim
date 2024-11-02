using ProjectRiskManagementSim.SimulationBlazor.Components;
using ProjectRiskManagementSim.SimulationBlazor.Components.Shared;
using ProjectRiskManagementSim.SimulationBlazor.Components.Pages.Simulations;
using ProjectRiskManagementSim.SimulationBlazor.Components.Pages.Dashboard;
using ProjectRiskManagementSim.SimulationBlazor.Components.Pages.RunSimulation;
using ProjectRiskManagementSim.SimulationBlazor.Models;
using ProjectRiskManagementSim.SimulationBlazor.Lib;
using Microsoft.AspNetCore.Http.HttpResults;
using ProjectRiskManagementSim.ProjectSimulation.Interfaces;
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace ProjectRiskManagementSim.SimulationBlazor.Routes;

public static class PageRoutes
{
    public static WebApplication MapPageRoutes(this WebApplication app)
    {

        app.MapGet("/dashboardMain",
            () => new RazorComponentResult<DashboardMain>());

        app.MapGet("/simulations",
            () => new RazorComponentResult<Simulations>());

        app.MapGet("/run-simulation",
            () => new RazorComponentResult<RunSimulation>());

        app.MapGet("/running-simulation/{simulationId:guid}", async (Guid simulationId, [FromServices] Func<Guid, RunSimulationHandler> handlerFactory) =>
        {
            var handler = handlerFactory(simulationId);
            if (!handler.simulationRunning)
            {
                await handler.StartSimulation();
            }

            var html = $@"<button hx-get='/running-simulation/{simulationId}' hx-trigger='click'class='btn text-white'> Start Simulation </button>";

            return Results.Content(html, "text/html");
        });

        app.MapGet("/kanban-simulation/{simulationId:guid}", async (Guid simulationId, [FromServices] Func<Guid, RunSimulationHandler> handlerFactory) =>
        {
            var handler = handlerFactory(simulationId);
        });

        app.MapPost("/reset-simulation/{simulationId:guid}", (Guid simulationId, [FromServices] Func<Guid, RunSimulationHandler> handlerFactory) =>
        {
            var handler = handlerFactory(simulationId);
            handler.ResetSimulation(); // Reset the specific simulation
            var html = $@"<div class='flex flex-row gap-2'>
                            <div class='bg-[#7e44eb] rounded-md w-fit p-2'>
                              <button hx-get='/running-simulation/@runSimulationHandler.SimulationId' hx-trigger='click'
                                class='btn text-white'>Start Simulation</button>
                            </div>
                            <div class='bg-[#7e44eb] rounded-md w-fit p-2'>
                              <button hx-post='/reset-simulation/@runSimulationHandler.SimulationId' hx-swap='none' hx-trigger='click'
                                class='btn text-white'>Reset Simulation</button>
                            </div>
                          </div>";
            return Results.Content(html, "text/html");
        });

        app.MapGet("/update-kanban-board", (HttpContext context, [FromServices] RunSimulationHandler handler) =>
        {
            // Simulate fetching the updated deliverables and columns for the Kanban board.
            return Results.Content($@"
            <table class=""table table-bordered"">
                <thead>
                    <tr>
                        {string.Join("", handler.columns.Select(column => $"<th>{column.Name} (WIP: {column.WIP})</th>"))}
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        {string.Join("", handler.columns.Select(column =>
                                    $@"<td>
                                {string.Join("", handler.columnDeliverables[column].Select(deliverable => $@"
                                    <div>
                                        <strong>Task: {deliverable.Nr}</strong><br />
                                        Completion Days: {deliverable.CompletionDays}<br />
                                        Wait Time: {deliverable.WaitTime}<br />
                                        Stopped Time: {deliverable.StoppedWorkingTime}
                                    </div>"))}
                              </td>"))}
                    </tr>
                </tbody>
            </table>", "text/html");
        });


        app.MapPost("/create-simulation", async (HttpRequest request, IMonteCarloSimulation MCS) =>
        {
            var form = await request.ReadFormAsync();

#nullable disable
            // Extract and parse form data
            var jiraProjectId = form["JiraProjectId"];
            var jiraProjectName = form["JiraProjectName"];
            var name = form["Name"];
            var startDate = DateTime.Parse(form["StartDate"]);
            var targetDate = DateTime.Parse(form["TargetDate"]);
            var revenueAmount = double.Parse(form["RevenueAmount"]);
            var cost = double.Parse(form["Cost"]);
            var costDays = double.Parse(form["CostDays"]);
            var hours = double.Parse(form["hours"]);
            var wip = int.Parse(form["WIP"]);
            var deliverableNumber = double.Parse(form["DeliverablesNumber"]);
            var percentageLowBound = double.Parse(form["PercentageLowBound"]);
            var percentageHighBound = double.Parse(form["PercentageHighBound"]);

            var columns = new List<ColumnModel>();
            for (int i = 1; i <= 11; i++)
            {
                if (form.ContainsKey($"name{i}"))
                {
                    columns.Add(new ColumnModel
                    {
                        Name = form[$"name{i}"],
                        WIP = int.Parse(form[$"wip{i}"]),
                        WIPMax = int.Parse(form[$"wipMax{i}"]),
                        EstimatedLowBound = int.Parse(form[$"lowBound{i}"]),
                        EstimatedHighBound = int.Parse(form[$"highBound{i}"]),
                        IsBuffer = form[$"isBuffer{i}"] == "on"
                    });
                }
            }

            var deliverableModel = new List<DeliverableModel>();
            for (var i = 1; i <= deliverableNumber; i++)
            {
                deliverableModel.Add(new DeliverableModel
                {
                    Id = Guid.NewGuid(),
                    Nr = i
                });
            }

            // Initialize the ProjectSimulationModel
            var projectData = new ProjectSimulationModel
            {
                Name = name,
                StartDate = startDate,
                TargetDate = targetDate,
                Revenue = new RevenueModel { Amount = revenueAmount },
                Costs = new CostModel { Cost = cost, Days = costDays },
                // Hours = hours,
                // Initialize the staff list
                Staff = new List<StaffModel>
                {
            new StaffModel { Name = "John Doe", Role = Role.ProjectManager, Sale = 1000, Cost = 370, Days = 20 },
            new StaffModel { Name = "Jane Doe", Role = Role.FrontendDeveloper, Sale = 1000, Cost = 370, Days = 20 },
            new StaffModel { Name = "Jack Doe", Role = Role.BackendDeveloper, Sale = 1000, Cost = 370, Days = 20 }
                },
                Backlog = new BacklogModel
                {
                    Deliverables = deliverableModel,
                    PercentageLowBound = percentageLowBound,
                    PercentageHighBound = percentageHighBound
                },
                Columns = columns
            };

            var simulationId = Guid.NewGuid(); // Create a unique ID for this simulation session
            var mappedProjectData = ModelMapper.MapToSimProjProjectSimulationModel(projectData);

            // Use the injected IMonteCarloSimulation instance to get a new simulation instance
            var simulation = MCS.GetSimulationInstance();
            simulation.InitiateSimulation(mappedProjectData, simulationId);

            // Store the simulation instance with the simulationId for later retrieval
            SimulationManager.AddSimulationToManager(simulationId, simulation);

            // Prepare simulation result HTML to return
            var htmlContent = $@"
        <div>
            <h2>Simulation Started</h2>
            <p>Simulation ID: {simulationId}, is simulation complete? {simulation.IsCompleted}</p>
            <p>Project Name: {projectData.Name}</p>
            <p>Start Date: {projectData.StartDate.ToShortDateString()}</p>
            <p>Target Date: {projectData.TargetDate.ToShortDateString()}</p>
            <p>Revenue: {projectData.Revenue.Amount}</p>
            <p>Cost: {projectData.Costs.Cost}</p>
            <button onclick=""window.location.href='/simulation-progress/{simulationId}'"">View Progress</button>
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

            // Assuming the simulation object has fields like "IsCompleted" and progress metrics
            var response = $@"
            <div>
                <h2>Simulation Progress</h2>
                <p>Simulation ID: {simulationId}</p>
                <p>Is simulation complete? {simulation.IsCompleted}</p>
                <!-- Add more fields as necessary -->
            </div>";

            return Results.Content(response, "text/html");
        });

        app.MapGet("/get-simulations", () =>
        {
            var allSimulations = SimulationManager.GetAllSimulations(); // Assuming this method exists

            if (!allSimulations.Any())
            {
                return Results.Content("<p>No simulations found.</p>", "text/html");
            }

            // Build a response that lists all simulations
            var simulationsListHtml = "<h2>All Simulations</h2><ul>";
            foreach (var sim in allSimulations)
            {
                simulationsListHtml += $@"
                    <li>
                        <p>Simulation ID: {sim.SimulationId}</p>
                        <p>Project Name: {sim.ProjectSimulationModel.Name}</p>
                        <p>Is Completed: {sim.IsCompleted}</p>
                        <p>Start Date: {sim.ProjectSimulationModel.StartDate}</p>
                        <p>Target Date: {sim.ProjectSimulationModel.TargetDate}</p>
                        <a href='/simulation-progress/{sim.SimulationId}'>View Progress</a>
                    </li>";
            }
            simulationsListHtml += "</ul>";

            return Results.Content(simulationsListHtml, "text/html");
        });

        return app;
    }
}
