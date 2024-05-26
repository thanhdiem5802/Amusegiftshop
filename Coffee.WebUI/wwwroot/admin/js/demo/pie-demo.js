// Set new default font family and font color to mimic Bootstrap's default styling
Chart.defaults.global.defaultFontFamily = 'Nunito', '-apple-system,system-ui,BlinkMacSystemFont,"Segoe UI",Roboto,"Helvetica Neue",Arial,sans-serif';
Chart.defaults.global.defaultFontColor = '#858796';
// Định nghĩa trước biểu đồ để có thể tham chiếu và cập nhật sau này
var myPieChart;

$(document).ready(function () {
    $.ajax({
        url: '/Admin/Statistics/CountCategory',
        type: 'GET',
        dataType: 'json',
        success: function (response) {
            if (response.success) {
                var categoryCounts = response.categoryCounts;
                var legendHTML = '';

                categoryCounts.forEach(function (item, index) {
                    var name = item.name;
                    var number = item.number;
                    var percent = item.percent
                    legendHTML += '<span class="mr-2"><i class="fas fa-circle text-' + getColorClass(index) + '"></i> ' + name + ': ' + number + '</span>';
                });

                $('#legend').html(legendHTML);

                // Khởi tạo biểu đồ với dữ liệu được lấy từ phản hồi
                var ctx = document.getElementById("myPieChart");
                myPieChart = new Chart(ctx, {
                    type: 'doughnut',
                    data: {
                        labels: categoryCounts.map(function (item) {
                            return item.name;
                        }),
                        datasets: [{
                            data: categoryCounts.map(function (item) {
                                return parseFloat(item.percent);
                            }),
                            backgroundColor: ['#4e73df', '#1cc88a', '#36b9cc'],
                            hoverBackgroundColor: ['#2e59d9', '#17a673', '#2c9faf'],
                            hoverBorderColor: "rgba(234, 236, 244, 1)",
                        }],
                    },
                    options: {
                        // ... các tùy chọn khác của bạn
                    },
                });
            } else {
                console.error('Lỗi khi lấy dữ liệu: ' + response.message);
            }
        },
        error: function (xhr, status, error) {
            console.error('Lỗi khi gọi API: ' + error);
        }
    });
});

// Function to get color class based on index
function getColorClass(index) {
    switch (index) {
        case 0:
            return 'primary';
        case 1:
            return 'success';
        case 2:
            return 'info';
        default:
            return 'primary';
    }
}


