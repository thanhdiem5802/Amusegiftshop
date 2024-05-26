// Set new default font family and font color to mimic Bootstrap's default styling
Chart.defaults.global.defaultFontFamily = 'Nunito', '-apple-system,system-ui,BlinkMacSystemFont,"Segoe UI",Roboto,"Helvetica Neue",Arial,sans-serif';
Chart.defaults.global.defaultFontColor = '#858796';

function number_format(number, decimals, dec_point, thousands_sep) {
    // *     example: number_format(1234.56, 2, ',', ' ');
    // *     return: '1 234,56'
    number = (number + '').replace(',', '').replace(' ', '');
    var n = !isFinite(+number) ? 0 : +number,
        prec = !isFinite(+decimals) ? 0 : Math.abs(decimals),
        sep = (typeof thousands_sep === 'undefined') ? ',' : thousands_sep,
        dec = (typeof dec_point === 'undefined') ? '.' : dec_point,
        s = '',
        toFixedFix = function (n, prec) {
            var k = Math.pow(10, prec);
            return '' + Math.round(n * k) / k;
        };
    // Fix for IE parseFloat(0.55).toFixed(0) = 0;
    s = (prec ? toFixedFix(n, prec) : '' + Math.round(n)).split('.');
    if (s[0].length > 3) {
        s[0] = s[0].replace(/\B(?=(?:\d{3})+(?!\d))/g, sep);
    }
    if ((s[1] || '').length < prec) {
        s[1] = s[1] || '';
        s[1] += new Array(prec - s[1].length + 1).join('0');
    }
    return s.join(dec);
}

$(document).ready(function () {
    $.ajax({
        url: '/Admin/Statistics/Productsold',
        type: 'GET',
        dataType: 'json',
        success: function (response) {
            if (response.success) {
                // Lấy dữ liệu từ phản hồi AJAX
                var data = response.productsold;
                var total = response.totalRevenue;
                var max = response.maxprice;
                // Tạo mảng màu sắc ngẫu nhiên cho mỗi sản phẩm
                var colors = data.map(function () {
                    // Hàm để tạo màu ngẫu nhiên
                    return 'rgba(' + Math.floor(Math.random() * 256) + ',' + Math.floor(Math.random() * 256) + ',' + Math.floor(Math.random() * 256) + ',0.7)';
                });
                // Tạo mảng nhãn và mảng dữ liệu từ dữ liệu nhận được
                var labels = data.map(function (item) {
                    return item.name;
                });
                var prices = data.map(function (item) {
                    return item.totalPrice;
                });
                console.log("total là " + total)
                // Cập nhật dữ liệu cho biểu đồ
                myBarChart.data.labels = labels;
                myBarChart.data.datasets[0].data = prices;
                myBarChart.options.scales.yAxes[0].ticks.max = max;
                myBarChart.data.datasets[0].backgroundColor = colors; // Thiết lập màu nền
                myBarChart.data.datasets[0].hoverBackgroundColor = colors; // Thiết lập màu khi hover

                // Cập nhật biểu đồ
                myBarChart.update();
            } else {
                console.error('Lỗi khi lấy dữ liệu: ' + response.error);
            }
        },
        error: function (xhr, status, error) {
            console.error('Lỗi khi gọi API: ' + error);
        }
    });
});


// Bar Chart Example
var ctx = document.getElementById("myBarChart");

var myBarChart = new Chart(ctx, {
    type: 'bar',
    data: {
        labels: ["January", "February", "March", "April", "May", "June"],
        datasets: [{
            label: "Revenue",
            backgroundColor: "#4e73df",
            hoverBackgroundColor: "#2e59d9",
            borderColor: "#4e73df",
            data: [4215, 5312, 6251, 7841, 9821, 14984],
        }],
    },
    options: {
        maintainAspectRatio: false,
        layout: {
            padding: {
                left: 10,
                right: 10,
                top: 0,
                bottom: 0
            }
        },
        scales: {
            xAxes: [{
                time: {
                    unit: 'month'
                },
                gridLines: {
                    display: false,
                    drawBorder: false
                },
                ticks: {
                    maxTicksLimit: 6
                },
                maxBarThickness: 30,
            }],
            yAxes: [{
                ticks: {
                    min: 0,
                    max: 15000,
                    maxTicksLimit: 10,
                    padding: 0,
                    // Include a dollar sign in the ticks
                    callback: function (value, index, values) {
                        return number_format(value) + 'VNĐ';
                    }
                },
                gridLines: {
                    color: "rgb(234, 236, 244)",
                    zeroLineColor: "rgb(234, 236, 244)",
                    drawBorder: false,
                    borderDash: [2],
                    zeroLineBorderDash: [2]
                }
            }],
        },
        legend: {
            display: false
        },
        tooltips: {
            titleMarginBottom: 10,
            titleFontColor: '#6e707e',
            titleFontSize: 20,
            backgroundColor: "rgb(255,255,255)",
            bodyFontColor: "#858796",
            borderColor: '#dddfeb',
            borderWidth: 1, 
            xPadding: 15,
            yPadding: 15,
            displayColors: false,
            caretPadding: 10,
            callbacks: {
                label: function (tooltipItem, chart) {
                    var datasetLabel = chart.datasets[tooltipItem.datasetIndex].label || '';
                    return datasetLabel + ': $' + number_format(tooltipItem.yLabel);
                }
            }
        },
    }
});
