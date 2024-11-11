// Function to initialize or update the chart
function initializeChart(newData) {
    // Check if chart already exists
    let chart = ApexCharts.getChartByID("chart");

    // If chart exists, update the series; otherwise, create a new chart
    if (chart) {
        chart.updateSeries([
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
        // Chart options for a new chart
        const options = optionFactory(newData);

        chart = new ApexCharts(document.querySelector("#chart"), options);
        chart.render();
    }

    console.log("Chart initialized or updated.");
}

// Chart options factory
function optionFactory(newData) {
    return {
        chart: {
            id: "chart",
            type: 'line',
            height: '100%',
            width: '100%',
            animations: {
              enabled: false,
            }
        },
        series: [
            {
                name: 'SimDate',
                data: newData.simDateData
            },
            {
                name: 'TargetDate',
                data: newData.targetDateData
            }
        ],
        xaxis: {
            type: 'datetime',
            labels: {
                style: {
                    colors: '#7e44eb',
                    fontSize: '12px',
                    fontFamily: 'Helvetica, Arial, sans-serif',
                    fontWeight: 400,
                    cssClass: 'apexcharts-yaxis-label',
                }
            }
        },
        yaxis: {
            type: 'datetime',
            labels: {
                style: {
                    colors: '#7e44eb',
                    fontSize: '12px',
                    fontFamily: 'Helvetica, Arial, sans-serif',
                    fontWeight: 400,
                    cssClass: 'apexcharts-yaxis-label',
                },
                formatter: function (value) {
                    const date = new Date(value);
                    return date.toLocaleDateString();
                }
            }
        },
        colors: ['#ff7c87', '#7e44eb', '#9C27B0'],
        legend: {
            position: 'top',
            horizontalAlign: 'center',
            fontSize: '12px',
            fontFamily: 'Helvetica, Arial, sans-serif',
            fontWeight: 400,
            labels: {
                colors: '#7e44eb',
            }
        },
    };
}

// HTMX event listeners to initialize the chart after content swap
document.addEventListener("htmx:afterSwap", (event) => {
    if (event.detail.target.id === "chartContainer") { // Adjust if necessary
        initializeChart(window.newChartData); // Use globally available data or refetch it
    }
});

document.body.addEventListener("apexCharts", function(evt){
    if (event.detail.target.id === "chartContainer") { // Adjust if necessary
        initializeChart(window.newChartData); // Use globally available data or refetch it
    }
})

// Initial chart rendering if data is already available
if (window.newChartData) {
    initializeChart(window.newChartData);
}
