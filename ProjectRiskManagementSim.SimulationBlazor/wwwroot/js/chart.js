// Set global Apex options for synchronized charts
window.Apex = {
    chart: {
        height: 100,
        group: "syncedGroup", // Apply the group globally
    },
    dataLabels: {
        enabled: false,
    },
    tooltip: {
        shared: true, // Enable shared tooltips across the group
        intersect: false,
    },
};


// Function to initialize or update the line chart and bar chart
function initializeChart(newData) {
    // Check if the line chart already exists
    let lineChart = ApexCharts.getChartByID("lineChart");
    const minWidht = 40;

    // If line chart exists, update its series; otherwise, create a new chart
    if (lineChart) {
        lineChart.updateSeries([
            {
                name: 'SimDate',
                data: newData.simDateData
            },
            {
                name: 'TargetDate',
                data: newData.targetDateData
            }
        ]);
    } else {
        // Initialize the line chart
        const lineChartOptions = lineChartOptionFactory(newData, minWidht);
        lineChart = new ApexCharts(document.querySelector("#lineChart"), lineChartOptions);
        lineChart.render();
    }

    // Check if the bar chart already exists
    let barChart = ApexCharts.getChartByID("barChart");

    // If bar chart exists, update its series; otherwise, create a new chart
    if (barChart) {
        barChart.updateSeries([
            {
                name: 'BudgetCosts',
                data: newData.budgetCostData
            },
            {
                name: 'SimulationCosts',
                data: newData.simulationCostData
            }
        ]);
    } else {
        // Initialize the bar chart
        const barChartOptions = barChartOptionFactory(newData, minWidht);
        barChart = new ApexCharts(document.querySelector("#barChart"), barChartOptions);
        barChart.render();
    }
}
// Line chart options with unique id, group, and minWidth for y-axis labels
function lineChartOptionFactory(newData, minWidht) {
    return {
        chart: {
            id: "lineChart", // Unique ID for line chart
            group: "syncedGroup", // Group name for synchronization
            type: 'line',
            height: 180,
            width: '100%',
            animations: { enabled: true },
            toolbar: { show: true }
        },
        series: [
            {
                name: 'Simulation Date',
                data: newData.simDateData
            },
            {
                name: 'Target Date',
                data: newData.targetDateData
            }
        ],
        xaxis: {
            type: 'category',
            categories: newData.categories,
            labels: {
                show: false
            },
            crosshairs: {
                width: 1, // Align the width for consistent hover behavior
            }
        },
        yaxis: {
            type: 'datetime',
            labels: {
                minWidth: minWidht, // Consistent minWidth for synced charts
                formatter: (value) => {
                    const date = new Date(value);
                    // Format to "dd mmm yyyy" e.g., "12 Oct 2023"
                    return date.toLocaleDateString("en-GB", {
                        day: "2-digit",
                        month: "short",
                        year: "numeric"
                    });
                }
            }
        },
        markers: {
            size: 6,
        },
        stroke: { width: 5, curve: 'smooth' },
        colors: ['#ff7c87', '#7e44eb'],
        tooltip: {
            enabled: true,
            shared: true, // Enable shared tooltips for synchronized hover
            intersect: false,
            x: {
                show: false,
            },
            y: {
                formatter: (value) => {
                    const date = new Date(value);
                    // Format to "dd mmm yyyy" e.g., "12 Oct 2023"
                    return date.toLocaleDateString("en-GB", {
                        day: "2-digit",
                        month: "short",
                        year: "numeric"
                    });
                },
            }
        }
    };
}

// Bar chart options with unique id, group, and minWidth for y-axis labels
function barChartOptionFactory(newData, minWidht) {
    return {
        chart: {
            id: "barChart", // Unique ID for bar chart
            group: "syncedGroup", // Group name for synchronization
            type: 'area',
            height: 180,
            width: '100%',
            animations: { enabled: true },
            toolbar: { show: true }
        },
        series: [
            {
                name: 'Simulation Costs',
                data: newData.simulationCostData
            },
            {
                name: 'Budget Costs',
                data: newData.budgetCostData
            }
        ],
        xaxis: {
            type: 'category',
            categories: newData.categories,
            labels: {
                show: false
            },
            crosshairs: {
                width: 1, // Align the width for consistent hover behavior
            }
        },
        yaxis: {
            labels: {
                minWidth: minWidht, // Consistent minWidth for synced charts
                formatter: (value) => {
                    return formatted = value.toLocaleString("en", {
                        style: "currency",
                        currency: "DKK",
                        minimumFractionDigits: 0,
                        maximumFractionDigits: 0,
                    });
                }

            }
        },
        markers: {
            size: 6,
        },
        stroke: { width: 5, curve: 'smooth' },
        colors: ['#ff7c87', '#7e44eb'], // Colors for budget and simulation costs
        legend: { position: 'top' },
        tooltip: {
            enabled: true,
            shared: true, // Enable shared tooltips for synchronized hover
            intersect: false,
            x: {
                show: false,
            },
            y: {
                formatter: (value) => {
                    // Format tooltip values here if needed
                    return formatted = value.toLocaleString("en", {
                        style: "currency",
                        currency: "DKK",
                        minimumFractionDigits: 0,
                        maximumFractionDigits: 0,
                    });
                },
            }
        }
    };
}

// HTMX event listeners to initialize the chart after content swap
document.addEventListener("htmx:afterSwap", (evt) => {
    if (evt.detail.target.id === "chartContainer") { // Adjust if necessary
        initializeChart(window.newChartData); // Use globally available data or refetch it
    }
});

document.body.addEventListener("apexCharts", function (evt) {
    if (evt.detail.target.id === "chartContainer") { // Adjust if necessary
        initializeChart(window.newChartData); // Use globally available data or refetch it
    }
})

// Initial chart rendering if data is already available
if (window.newChartData) {
    initializeChart(window.newChartData);
}
