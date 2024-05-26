// Set new default font family and font color to mimic Bootstrap's default styling
Chart.defaults.global.defaultFontFamily = 'Nunito', '-apple-system,system-ui,BlinkMacSystemFont,"Segoe UI",Roboto,"Helvetica Neue",Arial,sans-serif';
Chart.defaults.global.defaultFontColor = '#858796';
var listDataYear = [];
$(document).ready(function () {
    $.ajax({
        url: '/Admin/Statistics/GetStatisticsYear',
        type: 'GET',
        dataType: 'json',
        success: function (response) {
            if (response.success) {
                var statisticsYear = response.statisticsYear;
                statisticsYear.forEach(function (item) {
                    var month = item.months;
                    var total = item.total;
                    listDataYear[month - 1] = total;
                });
                console.log(listDataYear);

                // Update the chart after fetching data
                myLineChart.data.datasets[0].data = listDataYear;
                myLineChart.update();
            } else {
                console.error('Lỗi khi lấy dữ liệu: ' + response.message);
            }
        },
        error: function (xhr, status, error) {
            console.error('Lỗi khi gọi API: ' + error);
        }
    });
});

// Pie Chart Example
var ctx = document.getElementById("myPieChart");
var myPieChart = new Chart(ctx, {
  type: 'doughnut',
  data: {
    labels: ["Direct", "Referral", "Social"],
    datasets: [{
      data: [55, 30, 15],
      backgroundColor: ['#4e73df', '#1cc88a', '#36b9cc'],
      hoverBackgroundColor: ['#2e59d9', '#17a673', '#2c9faf'],
      hoverBorderColor: "rgba(234, 236, 244, 1)",
    }],
  },
  options: {
    maintainAspectRatio: false,
    tooltips: {
      backgroundColor: "rgb(255,255,255)",
      bodyFontColor: "#858796",
      borderColor: '#dddfeb',
      borderWidth: 1,
      xPadding: 15,
      yPadding: 15,
      displayColors: false,
      caretPadding: 10,
    },
    legend: {
      display: false
    },
    cutoutPercentage: 80,
  },
});
