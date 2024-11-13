using ProjectRiskManagementSim.SimulationBlazor.Components;
using ProjectRiskManagementSim.SimulationBlazor.Components.Shared;
using ProjectRiskManagementSim.SimulationBlazor.Components.Pages.Simulations;
using ProjectRiskManagementSim.SimulationBlazor.Components.Pages.Dashboard;
using ProjectRiskManagementSim.SimulationBlazor.Components.Pages.Dashboard.ForecastAnalysis;

using ProjectRiskManagementSim.SimulationBlazor.Components.Pages.Dashboard.SensitivityAnalysis;
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
using System.Globalization;
using Microsoft.EntityFrameworkCore.Storage.Json;

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

        app.MapDelete("/delete-simulation/{SimulationId:int}", async (int SimulationId, OxygenSimulationContext _context) =>
        {
            var simulation = await _context.GetSimulationByIdAsync(SimulationId);
            if (simulation == null)
            {
                return Results.NotFound();
            }
            _context.Remove(simulation);
            _context.SaveChanges();
            // Delete the simulation with the given ID
            return Results.Ok();
        });

        app.MapPost("/update-simulation", async (HttpRequest request, IMonteCarloSimulation MCS, OxygenSimulationContext _context) =>
        {
            var form = await request.ReadFormAsync();

            string? jiraProjectId, jiraId, jiraProjectName, name;
            double hours, revenueAmount, cost, budget, backendDevs, frontendDevs, testers, issueNumber, deliverableNumber, percentageLowBound, percentageHighBound, issueDoneCount, costPrDay, targetDays;

            double awaitCustPercentage, awaitCustPercentilesLowBound, awaitCustPercentilesHighBound;
            double awaitTaskPercentage, awaitTaskPercentilesLowBound, awaitTaskPercentilesHighBound;
            double await3pPercentage, await3PPercentilesLowBound, await3PPercentilesHighBound;
            double bugCount, bugPercentage, bugCycleTimePercentilesLowBound, bugCycleTimePercentilesHighBound;

            DateTime startDate, targetDate;
            int daysSinceStartInt;
            int? simProjectId;
            List<ViewColumnModel> columns;
            List<ViewDefectModel> defects;
            List<ViewBlockingEventModel> blockingEvents;

            ParseAndExtractForm(form,
                                out simProjectId,
                                out jiraProjectId,
                                out jiraId,
                                out jiraProjectName,
                                out name,
                                out hours,
                                out startDate,
                                out targetDate,
                                out revenueAmount,
                                out cost,
                                out budget,
                                out backendDevs,
                                out frontendDevs,
                                out testers,
                                out issueNumber,
                                out deliverableNumber,
                                out percentageLowBound,
                                out percentageHighBound,
                                out issueDoneCount,
                                out bugCount,
                                out bugPercentage,
                                out awaitCustPercentage,
                                out awaitTaskPercentage,
                                out await3pPercentage,
                                out daysSinceStartInt,
                                out costPrDay,
                                out columns,
                                out targetDays,
                                out awaitCustPercentilesLowBound,
                                out awaitCustPercentilesHighBound,
                                out awaitTaskPercentilesLowBound,
                                out awaitTaskPercentilesHighBound,
                                out await3PPercentilesLowBound,
                                out await3PPercentilesHighBound,
                                out bugCycleTimePercentilesLowBound,
                                out bugCycleTimePercentilesHighBound,
                                out defects,
                                out blockingEvents
                                  );

            // Store simulation in database
            int simIdFromDb = 0; // Access the ID for later use

            // Check if the project already exists in the database
            if (simProjectId != null && simProjectId != 0)
            {
                var existingSimProject = await _context.GetSimulationByIdAsync((int)simProjectId);
                existingSimProject.Name = name;
                existingSimProject.StartDate = startDate;
                existingSimProject.TargetDate = targetDate;
                existingSimProject.TargetDays = targetDays;
                existingSimProject.ActualDays = daysSinceStartInt;
                existingSimProject.ActualRevenue = revenueAmount;
                existingSimProject.ActualCosts = cost;
                existingSimProject.BudgetCosts = budget;
                existingSimProject.CostPrDay = costPrDay;
                existingSimProject.BackendDevs = backendDevs;
                existingSimProject.FrontendDevs = frontendDevs;
                existingSimProject.Testers = testers;
                existingSimProject.ActualHours = hours;
                existingSimProject.UpdatedAt = DateTime.Now;
                existingSimProject.DeliverablesCount = deliverableNumber;
                existingSimProject.IssueCount = issueNumber;
                existingSimProject.IssueDoneCount = issueDoneCount;
                existingSimProject.BugCount = bugCount;
                existingSimProject.BugPercentage = bugPercentage;

                existingSimProject.PercentageLowBound = percentageLowBound;
                existingSimProject.PercentageHighBound = percentageHighBound;

                // update existing columns
                existingSimProject.Columns.Select((column, index) =>
                {
                    column.Name = columns[index].Name;
                    column.EstimatedLowBound = columns[index].EstimatedLowBound;
                    column.EstimatedHighBound = columns[index].EstimatedHighBound;
                    column.WIP = columns[index].WIP;
                    column.WIPMax = columns[index].WIPMax;
                    column.IsBuffer = columns[index].IsBuffer;
                    return column;
                }).ToList();

                // update existing Defects
                existingSimProject.Defects.Select((column, index) =>
                {
                    column.Name = defects[index].Name;
                    column.DefectPercentage = defects[index].DefectPercentage;
                    column.DefectsPercentageLowBound = defects[index].DefectsPercentageLowBound;
                    column.DefectsPercentageHighBound = defects[index].DefectsPercentageHighBound;
                    return column;
                }).ToList();

                // update existing BlockingEvents
                existingSimProject.BlockingEvents.Select((column, index) =>
                {
                    column.Name = blockingEvents[index].Name;
                    column.BlockingEventPercentage = blockingEvents[index].BlockingEventPercentage;
                    column.BlockingEventsPercentageLowBound = blockingEvents[index].BlockingEventsPercentageLowBound;
                    column.BlockingEventsPercentageHighBound = blockingEvents[index].BlockingEventsPercentageHighBound;
                    return column;
                }).ToList();

                await _context.UpdateSimulationAsync(existingSimProject);
                // Save all changes at once
            };
            // Prepare simulation result HTML to return
            var htmlContent = $@"
                        <div class=""flex justify-end  id=""simulation-dashboar-result"">
                            <button class=""bg-[#7e44eb] flex gap-2 text-white px-4 py-2 rounded hover:bg-red-400"">
                                <a href=""/dashboard/{simIdFromDb.ToString()}"" hx-get>
                                        <div >Open Dashboard</div>
                                      </a>
                            </button>
                        </div>
                  ";
            return Results.Content(htmlContent, "text/html");
        });

        app.MapPost("/create-simulation", async (HttpRequest request, IMonteCarloSimulation MCS, OxygenSimulationContext _context) =>
        {
            var form = await request.ReadFormAsync();

            string? jiraProjectId, jiraId, jiraProjectName, name;
            double hours, revenueAmount, cost, budget, backendDevs, frontendDevs, testers, issueNumber, deliverableNumber, percentageLowBound, percentageHighBound, issueDoneCount, costPrDay, targetDays;

            double awaitCustPercentage, awaitCustPercentilesLowBound, awaitCustPercentilesHighBound;
            double awaitTaskPercentage, awaitTaskPercentilesLowBound, awaitTaskPercentilesHighBound;
            double await3pPercentage, await3PPercentilesLowBound, await3PPercentilesHighBound;
            double bugCount, bugPercentage, bugCycleTimePercentilesLowBound, bugCycleTimePercentilesHighBound;

            DateTime startDate, targetDate;
            int daysSinceStartInt;
            int? simProjectId;
            List<ViewColumnModel> columns;
            List<ViewDefectModel> defects;
            List<ViewBlockingEventModel> blockingEvents;
            ParseAndExtractForm(form,
                                out simProjectId,
                                out jiraProjectId,
                                out jiraId,
                                out jiraProjectName,
                                out name,
                                out hours,
                                out startDate,
                                out targetDate,
                                out revenueAmount,
                                out cost,
                                out budget,
                                out backendDevs,
                                out frontendDevs,
                                out testers,
                                out issueNumber,
                                out deliverableNumber,
                                out percentageLowBound,
                                out percentageHighBound,
                                out issueDoneCount,
                                out bugCount,
                                out bugPercentage,
                                out awaitCustPercentage,
                                out awaitTaskPercentage,
                                out await3pPercentage,
                                out daysSinceStartInt,
                                out costPrDay,
                                out columns,
                                out targetDays,
                                out awaitCustPercentilesLowBound,
                                out awaitCustPercentilesHighBound,
                                out awaitTaskPercentilesLowBound,
                                out awaitTaskPercentilesHighBound,
                                out await3PPercentilesLowBound,
                                out await3PPercentilesHighBound,
                                out bugCycleTimePercentilesLowBound,
                                out bugCycleTimePercentilesHighBound,
                                out defects,
                                out blockingEvents
                                  );

            // Store simulation in database
            int simIdFromDb = 0; // Access the ID for later use

            // Check if the project already exists in the database
            var existingProject = _context.GetProjectById(jiraId);
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
                    TargetDays = targetDays,
                    ActualDays = daysSinceStartInt,
                    ActualRevenue = revenueAmount,
                    ActualCosts = cost,
                    BudgetCosts = budget,
                    CostPrDay = costPrDay,
                    BackendDevs = backendDevs,
                    FrontendDevs = frontendDevs,
                    Testers = testers,
                    ActualHours = hours,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    DeliverablesCount = deliverableNumber,
                    IssueCount = issueNumber,
                    IssueDoneCount = issueDoneCount,
                    BugCount = bugCount,
                    BugPercentage = bugPercentage,

                    PercentageLowBound = percentageLowBound,
                    PercentageHighBound = percentageHighBound,
                    Columns = new List<ColumnModel>(),
                    Defects = new List<DefectModel>(),
                    BlockingEvents = new List<BlockingEventModel>()
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
                foreach (var defect in defects)
                {
                    DefectModel defectModel = new DefectModel
                    {
                        Name = defect.Name,
                        DefectPercentage = defect.DefectPercentage,
                        DefectsPercentageLowBound = defect.DefectsPercentageLowBound,
                        DefectsPercentageHighBound = defect.DefectsPercentageHighBound,
                    };
                    projectSimulationModel.Defects.Add(defectModel);
                }
                foreach (var blockingEvent in blockingEvents)
                {
                    BlockingEventModel blockingEventModel = new BlockingEventModel
                    {
                        Name = blockingEvent.Name,
                        BlockingEventPercentage = blockingEvent.BlockingEventPercentage,
                        BlockingEventsPercentageLowBound = blockingEvent.BlockingEventsPercentageLowBound,
                        BlockingEventsPercentageHighBound = blockingEvent.BlockingEventsPercentageHighBound,
                    };
                    projectSimulationModel.BlockingEvents.Add(blockingEventModel);
                }
                // Add ProjectSimulationModel to ProjectModel
                projectModel.ProjectSimulationModels.Add(projectSimulationModel);

                // Add ProjectModel (and related entities) to the context
                _context.ProjectModel.Add(projectModel);

                // Save all changes at once
                _context.SaveChanges();
                simIdFromDb = projectSimulationModel.Id;
            }
            else if (existingProject != null)
            {

                ProjectSimulationModel projectSimulationModel = new ProjectSimulationModel
                {
                    Name = name,
                    StartDate = startDate,
                    TargetDate = targetDate,
                    TargetDays = targetDays,
                    ActualDays = daysSinceStartInt,
                    ActualRevenue = revenueAmount,
                    ActualCosts = cost,
                    BudgetCosts = budget,
                    CostPrDay = costPrDay,
                    BackendDevs = backendDevs,
                    FrontendDevs = frontendDevs,
                    Testers = testers,
                    ActualHours = hours,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    DeliverablesCount = deliverableNumber,
                    IssueCount = issueNumber,
                    IssueDoneCount = issueDoneCount,
                    BugCount = bugCount,
                    BugPercentage = bugPercentage,

                    PercentageLowBound = percentageLowBound,
                    PercentageHighBound = percentageHighBound,
                    Columns = new List<ColumnModel>(),
                    Defects = new List<DefectModel>(),
                    BlockingEvents = new List<BlockingEventModel>(),
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

                foreach (var defect in defects)
                {
                    DefectModel defectModel = new DefectModel
                    {
                        Name = defect.Name,
                        DefectPercentage = defect.DefectPercentage,
                        DefectsPercentageLowBound = defect.DefectsPercentageLowBound,
                        DefectsPercentageHighBound = defect.DefectsPercentageHighBound,
                    };
                    projectSimulationModel.Defects.Add(defectModel);
                }
                foreach (var blockingEvent in blockingEvents)
                {
                    BlockingEventModel blockingEventModel = new BlockingEventModel
                    {
                        Name = blockingEvent.Name,
                        BlockingEventPercentage = blockingEvent.BlockingEventPercentage,
                        BlockingEventsPercentageLowBound = blockingEvent.BlockingEventsPercentageLowBound,
                        BlockingEventsPercentageHighBound = blockingEvent.BlockingEventsPercentageHighBound,
                    };
                    projectSimulationModel.BlockingEvents.Add(blockingEventModel);
                }
                // Add ProjectSimulationModel to ProjectModel
                existingProject.ProjectSimulationModels.Add(projectSimulationModel);

                // Save all changes at once
                await _context.SaveChangesAsync();
                simIdFromDb = projectSimulationModel.Id;
            }
            // Prepare simulation result HTML to return
            var htmlContent = $@"
                        <div class=""flex justify-end  id=""simulation-dashboar-result"">
                            <button class=""bg-[#7e44eb] flex gap-2 text-white px-4 py-2 rounded hover:bg-red-400"">
                                <a href=""/dashboard/{simIdFromDb.ToString()}"" hx-get>
                                        <div >Open Dashboard</div>
                                      </a>
                            </button>
                        </div>
                  ";
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

    private static void ParseAndExtractForm(IFormCollection form,
                                            out int? simProjectId,
                                            out string? jiraProjectId,
                                            out string? jiraId,
                                            out string? jiraProjectName,
                                            out string? name,
                                            out double hours,
                                            out DateTime startDate,
                                            out DateTime targetDate,
                                            out double revenueAmount,
                                            out double cost,
                                            out double budget,
                                            out double backendDevs,
                                            out double frontendDevs,
                                            out double testers,
                                            out double issueNumber,
                                            out double deliverableNumber,
                                            out double percentageLowBound,
                                            out double percentageHighBound,
                                            out double issueDoneCount,
                                            out double bugCount,
                                            out double bugPercentage,
                                            out double awaitCustPercentage,
                                            out double awaitTaskPercentage,
                                            out double await3pPercentage,
                                            out int daysSinceStartInt,
                                            out double costPrDay,
                                            out List<ViewColumnModel> columns,
                                            out double targetDays,
                                            out double awaitCustPercentilesLowBound,
                                            out double awaitCustPercentilesHighBound,
                                            out double awaitTaskPercentilesLowBound,
                                            out double awaitTaskPercentilesHighBound,
                                            out double await3PPercentilesLowBound,
                                            out double await3PPercentilesHighBound,
                                            out double bugCycleTimePercentilesLowBound,
                                            out double bugCycleTimePercentilesHighBound,
                                            out List<ViewDefectModel> defects,
                                            out List<ViewBlockingEventModel> blockingEvents
                                            )
    {
        // Extract and parse form data
        string? stringSimProjectId = form["SimProjectId"];
        jiraProjectId = form["JiraProjectId"];
        jiraId = form["JiraId"];
        jiraProjectName = form["JiraProjectName"];
        name = form["Name"];
        string? stringStartDate = form["StartDate"];
        string? stringLastDate = form["LastDate"];
        string? stringTargetDate = form["TargetDate"];
        string? stringRevenueAmount = form["RevenueAmount"];
        string? stringCost = form["Cost"];
        string? stringBudget = form["Budget"];
        string? stringBackendDevs = form["BackendDevs"];
        string? stringFrontendDevs = form["FrontendDevs"];
        string? stringTesters = form["Testers"];
        string? stringHours = form["Hours"];
        string? stringIssueNumber = form["IssueNumber"];
        string? stringDeliverablesNumber = form["DeliverablesNumber"];
        string? stringPercentageLowBound = form["PercentageLowBound"];
        string? stringPercentageHighBound = form["PercentageHighBound"];
        string? stringIssueDoneCount = form["IssuesDoneCount"];
        string? stringBugCount = form["BugCount"];
        string? stringBugPercentage = form["BugPercentage"];
        string? stringAwaitCustPercentage = form["AwaitCustPercentage"];
        string? stringAwaitTaskPercentage = form["AwaitTaskPercentage"];
        string? stringAwait3pPercentage = form["Await3pPercentage"];
        string? stringAwaitCustPercentilesLowBound = form["AwaitCustPercentilesLowBound"];
        string? stringAwaitCustPercentilesHighBound = form["AwaitCustPercentilesHighBound"];
        string? stringAwaitTaskPercentilesLowBound = form["AwaitTaskPercentilesLowBound"];
        string? stringAwaitTaskPercentilesHighBound = form["AwaitTaskPercentilesHighBound"];
        string? stringAwait3PPercentilesLowBound = form["Await3PPercentilesLowBound"];
        string? stringAwait3PPercentilesHighBound = form["Await3PPercentilesHighBound"];
        string? stringBugCycleTimePercentilesLowBound = form["BugCycleTimePercentilesLowBound"];
        string? stringBugCycleTimePercentilesHighBound = form["BugCycleTimePercentilesHighBound"];

        if (jiraProjectName == null
            || jiraId == null
            || jiraProjectId == null
            || stringStartDate == null
            || stringLastDate == null
            || stringTargetDate == null
            || stringRevenueAmount == null
            || stringCost == null
            || stringBudget == null
            || stringBackendDevs == null
            || stringFrontendDevs == null
            || stringTesters == null
            || stringHours == null
            || stringIssueNumber == null
            || stringDeliverablesNumber == null
            || stringPercentageLowBound == null
            || stringPercentageHighBound == null
            || stringIssueDoneCount == null
            || stringBugCount == null
            || stringBugPercentage == null
            || stringAwaitCustPercentage == null
            || stringAwaitTaskPercentage == null
            || stringAwait3pPercentage == null
            || stringAwaitCustPercentilesLowBound == null
            || stringAwaitCustPercentilesHighBound == null
            || stringAwaitTaskPercentilesLowBound == null
            || stringAwaitTaskPercentilesHighBound == null
            || stringAwait3PPercentilesLowBound == null
            || stringAwait3PPercentilesHighBound == null
            || stringBugCycleTimePercentilesLowBound == null
            || stringBugCycleTimePercentilesHighBound == null
            )
        {
            throw new ArgumentException("Invalid input.");
        }

        if (stringSimProjectId == null)
        {
            simProjectId = null;
        }
        else
        {
            simProjectId = int.Parse(stringSimProjectId);
        }


        hours = double.Parse(stringHours.Replace(',', '.'), CultureInfo.InvariantCulture);
        startDate = DateTime.Parse(stringStartDate);
        var lastDate = DateTime.Parse(stringLastDate);
        targetDate = DateTime.Parse(stringTargetDate);
        revenueAmount = double.Parse(stringRevenueAmount.Replace(',', '.'), CultureInfo.InvariantCulture);
        cost = double.Parse(stringCost.Replace(',', '.'), CultureInfo.InvariantCulture);
        budget = double.Parse(stringBudget.Replace(',', '.'), CultureInfo.InvariantCulture);
        backendDevs = double.Parse(stringBackendDevs.Replace(',', '.'), CultureInfo.InvariantCulture);
        frontendDevs = double.Parse(stringFrontendDevs.Replace(',', '.'), CultureInfo.InvariantCulture);
        testers = double.Parse(stringTesters.Replace(',', '.'), CultureInfo.InvariantCulture);
        var totalDevelopers = backendDevs + frontendDevs;
        issueNumber = double.Parse(stringIssueNumber.Replace(',', '.'), CultureInfo.InvariantCulture);
        deliverableNumber = double.Parse(stringDeliverablesNumber.Replace(',', '.'), CultureInfo.InvariantCulture);
        percentageLowBound = double.Parse(stringPercentageLowBound.Replace(',', '.'), CultureInfo.InvariantCulture) / 100;
        percentageHighBound = double.Parse(stringPercentageHighBound.Replace(',', '.'), CultureInfo.InvariantCulture) / 100;
        issueDoneCount = double.Parse(stringIssueDoneCount.Replace(',', '.'), CultureInfo.InvariantCulture);
        bugCount = double.Parse(stringBugCount.Replace(',', '.'), CultureInfo.InvariantCulture);
        bugPercentage = double.Parse(stringBugPercentage.Replace(',', '.'), CultureInfo.InvariantCulture);
        awaitCustPercentage = double.Parse(stringAwaitCustPercentage.Replace(',', '.'), CultureInfo.InvariantCulture);
        awaitTaskPercentage = double.Parse(stringAwaitTaskPercentage.Replace(',', '.'), CultureInfo.InvariantCulture);
        await3pPercentage = double.Parse(stringAwait3pPercentage.Replace(',', '.'), CultureInfo.InvariantCulture);
        awaitCustPercentilesLowBound = double.Parse(stringAwaitCustPercentilesLowBound.Replace(',', '.'), CultureInfo.InvariantCulture);
        awaitCustPercentilesHighBound = double.Parse(stringAwaitCustPercentilesHighBound.Replace(',', '.'), CultureInfo.InvariantCulture);
        awaitTaskPercentilesLowBound = double.Parse(stringAwaitTaskPercentilesLowBound.Replace(',', '.'), CultureInfo.InvariantCulture);
        awaitTaskPercentilesHighBound = double.Parse(stringAwaitTaskPercentilesHighBound.Replace(',', '.'), CultureInfo.InvariantCulture);
        await3PPercentilesLowBound = double.Parse(stringAwait3PPercentilesLowBound.Replace(',', '.'), CultureInfo.InvariantCulture);
        await3PPercentilesHighBound = double.Parse(stringAwait3PPercentilesHighBound.Replace(',', '.'), CultureInfo.InvariantCulture);
        bugCycleTimePercentilesLowBound = double.Parse(stringBugCycleTimePercentilesLowBound.Replace(',', '.'), CultureInfo.InvariantCulture);
        bugCycleTimePercentilesHighBound = double.Parse(stringBugCycleTimePercentilesHighBound.Replace(',', '.'), CultureInfo.InvariantCulture);


        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(jiraProjectId) || string.IsNullOrWhiteSpace(jiraProjectName))
        {
            throw new ArgumentException("Invalid input.");
        }

        if (targetDate < startDate)
        {
            throw new ArgumentException("Invalid date range.");
        }
        // check if percentageLowBound is less than percentageHighBound and with 0 and 100
        if (percentageLowBound < 0 || percentageLowBound > 1 || percentageHighBound < 0 || percentageHighBound > 1 || percentageLowBound > percentageHighBound)
        {
            throw new ArgumentException("Invalid percentage range.");
        }
        daysSinceStartInt = 1;
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
        costPrDay = cost / daysSinceStartInt;

        //defects = new List<ViewDefectModel>();

        for (int i = 0; i < 1; i++)
        {

        }

        defects = new List<ViewDefectModel>();
        var defect = new ViewDefectModel
        {
            Name = "Bugs",
            DefectPercentage = bugPercentage,
            DefectsPercentageLowBound = bugCycleTimePercentilesLowBound,
            DefectsPercentageHighBound = bugCycleTimePercentilesHighBound
        };
        defects.Add(defect);

        blockingEvents = new List<ViewBlockingEventModel>();
        var blockCustomer = new ViewBlockingEventModel
        {
            Name = "Awaiting Customer",
            BlockingEventPercentage = awaitCustPercentage,
            BlockingEventsPercentageLowBound = awaitCustPercentilesLowBound,
            BlockingEventsPercentageHighBound = awaitCustPercentilesHighBound
        };
        blockingEvents.Add(blockCustomer);
        var blockTask = new ViewBlockingEventModel
        {
            Name = "Awaiting Task",
            BlockingEventPercentage = awaitTaskPercentage,
            BlockingEventsPercentageLowBound = awaitTaskPercentilesLowBound,
            BlockingEventsPercentageHighBound = awaitTaskPercentilesHighBound
        };
        blockingEvents.Add(blockTask);
        var block3p = new ViewBlockingEventModel
        {
            Name = "Awaiting 3rd Party",
            BlockingEventPercentage = await3pPercentage,
            BlockingEventsPercentageLowBound = await3PPercentilesLowBound,
            BlockingEventsPercentageHighBound = await3PPercentilesHighBound
        };
        blockingEvents.Add(block3p);


        columns = new List<ViewColumnModel>();
        for (int i = 0; i <= 11; i++)
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

                if (colName == "Backlog" || colName == "Done")
                {
                    if (int.Parse(wip) < deliverableNumber || int.Parse(wipMax) < deliverableNumber)
                    {
                        columns.Add(new ViewColumnModel
                        {
                            Name = colName,
                            WIP = (int)deliverableNumber,
                            WIPMax = (int)deliverableNumber,
                            EstimatedLowBound = int.Parse(lowBound),
                            EstimatedHighBound = int.Parse(highBound),
                            IsBuffer = isBuffer == "on"
                        });
                    }
                    else
                    {
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
                else if (colName == "In Progress")
                {
                    if (int.Parse(wip) < totalDevelopers || int.Parse(wipMax) < totalDevelopers)
                    {
                        columns.Add(new ViewColumnModel
                        {
                            Name = colName,
                            WIP = (int)totalDevelopers,
                            WIPMax = (int)totalDevelopers,
                            EstimatedLowBound = int.Parse(lowBound),
                            EstimatedHighBound = int.Parse(highBound),
                            IsBuffer = isBuffer == "on"
                        });
                    }
                    else
                    {
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
                else if (colName == "Testing on Development")
                {
                    if (int.Parse(wip) < testers || int.Parse(wipMax) < testers)
                    {
                        columns.Add(new ViewColumnModel
                        {
                            Name = colName,
                            WIP = (int)testers,
                            WIPMax = (int)testers,
                            EstimatedLowBound = int.Parse(lowBound),
                            EstimatedHighBound = int.Parse(highBound),
                            IsBuffer = isBuffer == "on"
                        });
                    }
                    else
                    {
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
                else
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

        var targetTimeSpan = targetDate - startDate;
        targetDays = targetTimeSpan.Days;
    }
}
