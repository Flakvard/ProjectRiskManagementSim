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
using ProjectRiskManagementSim.DataAccess;
using ProjectRiskManagementSim.DataAccess.Models;

namespace ProjectRiskManagementSim.SimulationBlazor.Routes;

public static class PageRoutes
{
    public static WebApplication MapPageRoutes(this WebApplication app)
    {

        app.MapGet("/dashboardMain",
            () => new RazorComponentResult<DashboardMain>());

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


        app.MapPost("/create-simulation", async (HttpRequest request, IMonteCarloSimulation MCS, OxygenSimulationContext _context) =>
        {
            var form = await request.ReadFormAsync();

            // Extract and parse form data
            string? jiraProjectId = form["JiraProjectId"];
            string? jiraId = form["JiraId"];
            string? jiraProjectName = form["JiraProjectName"];
            string? name = form["Name"];

            string? stringStartDate = form["StartDate"];
            string? stringLastDate = form["LastDate"];
            string? stringTargetDate = form["TargetDate"];
            string? stringRevenueAmount = form["RevenueAmount"];
            string? stringCost = form["Cost"];
            string? stringHours = form["Hours"];
            string? stringDeliverablesNumber = form["DeliverablesNumber"];
            string? stringPercentageLowBound = form["PercentageLowBound"];
            string? stringPercentageHighBound = form["PercentageHighBound"];
            if (jiraProjectName == null
                || jiraId == null
                || jiraProjectId == null
                || stringStartDate == null
                || stringLastDate == null
                || stringTargetDate == null
                || stringRevenueAmount == null
                || stringCost == null
                || stringHours == null
                || stringDeliverablesNumber == null
                || stringPercentageLowBound == null
                || stringPercentageHighBound == null)
            {
                return Results.BadRequest("Invalid input.");
            }
            var percentageLowBound = double.Parse(stringPercentageLowBound);
            var hours = double.Parse(stringHours);
            var startDate = DateTime.Parse(stringStartDate);
            var lastDate = DateTime.Parse(stringLastDate);
            var targetDate = DateTime.Parse(stringTargetDate);
            var revenueAmount = double.Parse(stringRevenueAmount);
            var cost = double.Parse(stringCost);
            var deliverableNumber = double.Parse(stringDeliverablesNumber);
            var percentageHighBound = double.Parse(stringPercentageHighBound);
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(jiraProjectId) || string.IsNullOrWhiteSpace(jiraProjectName))
            {
                return Results.BadRequest("Invalid input.");
            }

            if (targetDate < startDate)
            {
                return Results.BadRequest("Invalid date range.");
            }
            var daysSinceStartInt = 1;
            if (lastDate < startDate)
            {
                var todayDate = DateTime.Now;
                var daysSinceStart = todayDate - startDate;
                daysSinceStartInt = daysSinceStart.Days;
            }
            else
            {
                var daysSinceStart = lastDate - startDate;
                daysSinceStartInt = daysSinceStart.Days;

            }
            var costPrDay = cost / daysSinceStartInt;

            var columns = new List<ViewColumnModel>();
            for (int i = 1; i <= 11; i++)
            {
                if (form.ContainsKey($"name{i}"))
                {
                    // Check possible null reference
                    string? colName = form[$"name{i}"];
                    string? wip = form[$"wip{i}"];
                    string? wipMax = form[$"wipMax{i}"];
                    string? lowBound = form[$"lowBound{i}"];
                    string? highBound = form[$"highBound{i}"];
                    string? isBuffer = form[$"isBuffer{i}"];
                    if (colName == null || wip == null || wipMax == null || lowBound == null || highBound == null)
                    {
                        continue;
                    }
                    columns.Add(new ViewColumnModel
                    {
                        Name = colName,
                        WIP = int.Parse(wip),
                        WIPMax = int.Parse(wipMax),
                        EstimatedLowBound = int.Parse(lowBound),
                        EstimatedHighBound = int.Parse(highBound),
                        IsBuffer = isBuffer == "on"
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
            var projectData = new ViewProjectSimulationModel
            {
                Name = name,
                StartDate = startDate,
                TargetDate = targetDate,
                Revenue = new RevenueModel { Amount = revenueAmount },
                Costs = new CostModel { Cost = costPrDay, Days = daysSinceStartInt },
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

            // Initialize the simulation
            var simulationId = Guid.NewGuid(); // Create a unique ID for this simulation session
            var mappedProjectData = ModelMapper.MapToSimProjProjectSimulationModel(projectData);

            // Use the injected IMonteCarloSimulation instance to get a new simulation instance
            var simulation = MCS.GetSimulationInstance();
            simulation.InitiateSimulation(mappedProjectData, simulationId);

            // Store the simulation instance with the simulationId for later retrieval
            SimulationManager.AddSimulationToManager(simulationId, simulation);

            // Store simulation in database

            // Check if the project already exists in the database
            var existingProject = _context.GetProjectByIdAsync(jiraId);
            if (existingProject == null)
            {
                ProjectModel projectModel = new ProjectModel
                {
                    JiraId = jiraId,
                    JiraProjectId = jiraProjectId,
                    Name = jiraProjectName,
                    ProjectSimulationModels = new List<ProjectSimulationModel>()
                };

                ProjectSimulationModel projectSimulationModel = new ProjectSimulationModel
                {
                    Name = name,
                    StartDate = startDate,
                    TargetDate = targetDate,
                    ActualRevenue = revenueAmount,
                    ActualCosts = cost,
                    ActualHours = hours,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    DeliverablesCount = deliverableNumber,
                    PercentageLowBound = percentageLowBound,
                    PercentageHighBound = percentageHighBound,
                    Columns = new List<ColumnModel>()
                };
                // Log the number of columns

                foreach (var column in columns)
                {
                    ColumnModel columnModel = new ColumnModel
                    {
                        Name = column.Name,
                        EstimatedLowBound = column.EstimatedLowBound,
                        EstimatedHighBound = column.EstimatedHighBound,
                        WIP = column.WIP,
                        WIPMax = column.WIPMax,
                        IsBuffer = column.IsBuffer,
                    };
                    projectSimulationModel.Columns.Add(columnModel);
                }
                // Add ProjectSimulationModel to ProjectModel
                projectModel.ProjectSimulationModels.Add(projectSimulationModel);

                // Add ProjectModel (and related entities) to the context
                _context.ProjectModel.Add(projectModel);

                // Save all changes at once
                _context.SaveChanges();
            }
            else if (existingProject != null)
            {

                ProjectSimulationModel projectSimulationModel = new ProjectSimulationModel
                {
                    Name = name,
                    StartDate = startDate,
                    TargetDate = targetDate,
                    ActualRevenue = revenueAmount,
                    ActualCosts = cost,
                    ActualHours = hours,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    DeliverablesCount = deliverableNumber,
                    PercentageLowBound = percentageLowBound,
                    PercentageHighBound = percentageHighBound,
                    Columns = new List<ColumnModel>()
                };
                // Log the number of columns

                foreach (var column in columns)
                {
                    ColumnModel columnModel = new ColumnModel
                    {
                        Name = column.Name,
                        EstimatedLowBound = column.EstimatedLowBound,
                        EstimatedHighBound = column.EstimatedHighBound,
                        WIP = column.WIP,
                        WIPMax = column.WIPMax,
                        IsBuffer = column.IsBuffer,
                    };
                    projectSimulationModel.Columns.Add(columnModel);
                }
                // Add ProjectSimulationModel to ProjectModel
                existingProject.ProjectSimulationModels.Add(projectSimulationModel);

                // Save all changes at once
                _context.SaveChanges();
            }


            // Prepare simulation result HTML to return
            var htmlContent = $@"Saved!";
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
