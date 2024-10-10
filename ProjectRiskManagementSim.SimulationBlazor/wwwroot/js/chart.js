// Purpose: To create a chart that displays the SimDate and TargetDate
var options = {
  chart: {
    type: 'line',
    height: 400,
    width: 670
  },
  series: [{
    name: 'SimDate',
    data: [
      {x: new Date('2024-09-25').getTime(), y: new Date('2024-09-25').getTime()},
      {x: new Date('2024-10-20').getTime(), y: new Date('2024-10-20').getTime()},
      {x: new Date('2024-11-09').getTime(), y: new Date('2024-11-09').getTime()},
      {x: new Date('2024-11-19').getTime(), y: new Date('2024-11-19').getTime()},
      {x: new Date('2024-12-09').getTime(), y: new Date('2024-12-09').getTime()},
      {x: new Date('2024-12-19').getTime(), y: new Date('2024-12-19').getTime()},
      {x: new Date('2025-01-08').getTime(), y: new Date('2025-01-08').getTime()}
    ]
  }, {
    name: 'TargetDate',
    data: [
      {x: new Date('2024-09-25').getTime(), y: new Date('2024-10-01').getTime()},
      {x: new Date('2024-10-20').getTime(), y: new Date('2024-10-01').getTime()},
      {x: new Date('2024-11-09').getTime(), y: new Date('2024-11-09').getTime()},
      {x: new Date('2024-11-19').getTime(), y: new Date('2024-11-09').getTime()},
      {x: new Date('2024-12-09').getTime(), y: new Date('2024-11-09').getTime()},
      {x: new Date('2024-12-19').getTime(), y: new Date('2024-11-09').getTime()},
      {x: new Date('2025-01-08').getTime(), y: new Date('2025-01-01').getTime()}
    ]
  }],
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
        var date = new Date(value);
        return date.toLocaleDateString();
      }
    }
  },
  colors:['#ff7c87', '#7e44eb', '#9C27B0'],
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

var chart = new ApexCharts(document.querySelector("#chart"), options);
chart.render();
