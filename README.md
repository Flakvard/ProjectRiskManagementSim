# My bachelor thesis: 
> ## _User-friendly Monte Carlo Simulation for Data-Driven Decisions in Project Management • A Solution for Optimized Project Management & Risk Management”_
# ProjectRiskManagementSim

<img src="https://github.com/Flakvard/ProjectRiskManagementSim/blob/master/Home%20screen.png?raw=true" width="30%" height="150px"></img> 
<img src="https://github.com/Flakvard/ProjectRiskManagementSim/blob/master/Anon%20project.png?raw=true" width="30%" height="150px"></img> 
<img src="https://github.com/Flakvard/ProjectRiskManagementSim/blob/master/Anon%20project%20visual.png?raw=true" width="30%" height="150px" ></img> 

Welcome to **ProjectRiskManagementSim**, where the complexities of project management meet the simplicity of intuitive, user-friendly interfaces. This tool is designed with project managers in mind—those who seek to master project dynamics without delving into advanced statistical realms.

Our robust simulation engine mirrors the principles of lean project management, seamlessly accommodating cross-functional teams that self-organize to eliminate bottlenecks. This ensures a fluid workflow, enhancing productivity and efficiency across the board.

### Key Features
- **User-Friendly Interface**: Tailored for project managers with no advanced statistical knowledge required.
- **Lean Project Management Simulation**: Emulates real-world project dynamics with cross-functional teams self-organizing to remove bottlenecks.

### Primary Analyses
1. **Forecast Analysis**: Accurately estimates project end dates and budgets, providing clear visibility into the future.
2. **Sensitivity Analysis**: Identifies the variables that have the most significant impact on your project, helping you make informed decisions. Ranks them in order, so the project manager knows what to focus on.
3. **Staff Analysis**: Evaluates the impact of adding resources, ensuring optimal team composition and workload distribution. Ranks them in order, so the project manager if its a Developer or Tester that has most impact.


### Overview
**ProjectRiskManagementSim** is developed using C# and .NET, comprising the following modules:
- ProjectRiskManagementSim.SimulationBlazor
- ProjectRiskManagementSim.ProjectSimulator
- ProjectRiskManagementSim.DataAccess
- ProjectRiskManagementSim.UnitTest
```powershell
Lines of code by folder:
--------------------------------------------------------------------------------------------------------------------------------------
Folder                                                                           Lines of Code        Percentage
--------------------------------------------------------------------------------------------------------------------------------------
        ProjectRiskManagementSim.SimulationBlazor                                       7565                74.61%
        ProjectRiskManagementSim.ProjectSimulator                                       1818                17.93%
        ProjectRiskManagementSim.DataAccess                                             500                 4.93 %
        ProjectRiskManagementSim.UnitTest                                               256                 2.52 %

--------------------------------------------------------------------------------------------------------------------------------------
File Type                                                                        Lines of Code        Percentage
--------------------------------------------------------------------------------------------------------------------------------------
        .cs                                                                             5876                57.95%
        .razor                                                                          3817                37.65%
        .js                                                                             446                 4.4  %

--------------------------------------------------------------------------------------------------------------------------------------
        Total lines of code in all files: 10139
```

### Tools/Libs Used
- `Frontend`: HTMX + HTML (razor) + JavaScript
- `Backend`: Blazor + Htmxor + Minimal API + Class Library for The Monte Carlo Simulation


### Data Flow
![Data Flow](https://github.com/Flakvard/ProjectRiskManagementSim/blob/master/data%20flow.png?raw=true)
1. Every 5 minutes, an API fetches data from Jira, recording all movements and timestamps for each task and converting dates to numerical days.
2. The system calculates `Lead Time`, `Cycle Time`, and `Throughput` for a project and each column in a Kanban board, enabling project simulation.
3. Our solution is hosted on a server ([https://mcs.oxygen.dk](https://mcs.oxygen.dk)), accessible only within the Oxygen network.
4. A project manager uses the website to:
   - View project status
   - Perform CRUD operations on simulations
   - Run analyses such as `Forecast Analysis`, `Staff Analysis`, and `Sensitivity Analysis`
5. Results are displayed back to the project manager.

### Features
- **Automated Data Collection**: Integration with Jira for real-time data retrieval.
- **Simulation Creation**: Users can create and run simulations based on Jira data.
- **Intuitive Dashboards**: Provide clear access to key metrics and simulation results.
- **User-Friendly Interface**: Designed for both experienced and new project managers, with no advanced statistical knowledge required.
- **Scenario Testing**: Test various scenarios like adding resources or reducing bugs.
- **Asynchronous Simulation**: Multiple users can run different scenarios simultaneously.

### Key Analyses
- **Forecast Analysis**: Estimates project end date and budget.
- **Sensitivity Analysis**: Identifies the variables that most impact the project.
- **Staff Analysis**: Evaluates the impact of adding resources.

### Screenshots
#### Home
![Home Screen](https://github.com/Flakvard/ProjectRiskManagementSim/blob/master/Home%20screen.png?raw=true)
The "Home" screen features user-friendly buttons for creating new simulations and viewing all simulations and projects.

#### Dashboard
![Dashboard](https://github.com/Flakvard/ProjectRiskManagementSim/blob/master/Anon%20project.png?raw=true)
Dashboards display variables, project status, and interactive options for simulating various scenarios. Key analyses include:
- **Forecast Analysis**: Expected end date and budget.
- **Sensitivity Analysis**: Variables that most impact the project.
- **Staff Analysis**: Impact of adding resources.

#### Kanban Board
![Kanbanboard visual](https://github.com/Flakvard/ProjectRiskManagementSim/blob/master/Anon%20project%20visual.png?raw=true)
Illustrates a simulation running day by day until project completion. Purple tasks are deliverables, green are bugs, and red are tasks stuck due to awaiting customer, third-party, or other tasks. The Monte Carlo simulation models the dynamics of a multi-step workflow with WIP limits in various columns, closely approximating Kanban or Lean flow. It also reflects cross-functional teams self-organizing to remove bottlenecks.
