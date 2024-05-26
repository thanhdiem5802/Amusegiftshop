using AutoMapper;
using Coffee.DATA.Models;
using Coffee.DATA.Repository;
using Coffee.WebUI.Areas.Admin.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Core.Types;

namespace Coffee.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class StatisticsController : Controller
    {
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<OrderDetail> _orderDetailRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IMapper _mapper;
        public StatisticsController(IRepository<Order> orderRepository, IRepository<User> userRepository ,IRepository<Category> categoryRepository, IRepository<OrderDetail> orderDetailRepository,IRepository<Product> productRepository, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _orderDetailRepository = orderDetailRepository;
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _mapper = mapper;
            _userRepository = userRepository;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> GetStatisticsYear()
        {
            try
            {
                var _order = await _orderRepository.GetAllAsync();
                var statisticsYear = new List<StatisticsYearModel>();
                var _orderDetail = await _orderDetailRepository.GetAllAsync();

                _order = _order.Where(x => x.CreatedOn.Value.Year == DateTime.Now.Year && x.OrderStatus == true);

                for (int i = 1; i <= 12; i++)
                {
                    var _orderListIdMonth = _order.Where(x => x.CreatedOn.Value.Month == i);
                    if (_orderListIdMonth.Count() > 0)
                    {
                        decimal _total = 0;
                        foreach (var item in _orderListIdMonth)
                        {
                            _total += Convert.ToDecimal(_orderDetail.Where(x => x.OrderId == Convert.ToInt32(item.Id)).Sum(x => x.Price));
                        }
                        statisticsYear.Add(new StatisticsYearModel
                        {
                            Months = i,
                            Total = _total
                        });
                    }
                    else
                    {
                        statisticsYear.Add(new StatisticsYearModel
                        {
                            Months = i,
                            Total = 0,
                        });
                    }
                }
                return Json(new { success = true, statisticsYear });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Fail: " + ex.Message });
            }
        }
        public async Task<IActionResult> GetTotalinMonth()

        {
            try
            {
                var _order = await _orderRepository.GetAllAsync();
                var statisticsYear = new List<StatisticsYearModel>();
                var _orderDetail = await _orderDetailRepository.GetAllAsync();

                _order = _order.Where(x => x.CreatedOn.Value.Month == DateTime.Now.Month && x.OrderStatus == true);

               
                    var _orderListIdMonth = _order.Where(x => x.CreatedOn.Value.Month == DateTime.Now.Month);
                    if (_orderListIdMonth.Count() > 0)
                    {
                        decimal _total = 0;
                        foreach (var item in _orderListIdMonth)
                        {
                            _total += Convert.ToDecimal(_orderDetail.Where(x => x.OrderId == Convert.ToInt32(item.Id)).Sum(x => x.Price));
                        }
                        statisticsYear.Add(new StatisticsYearModel
                        {
                            Months = DateTime.Now.Month,
                            Total = _total
                        });
                    }
                    else
                    {
                        statisticsYear.Add(new StatisticsYearModel
                        {
                            Months = DateTime.Now.Month,
                            Total = 0,
                        });
                    }
            
                
                return Json(new { success = true, statisticsYear });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Fail: " + ex.Message });
            }
        }
        public async Task<IActionResult> GetHighestinYear()

        {
            try
            {
                var _order = await _orderRepository.GetAllAsync();
                var statisticsYear = new List<StatisticsYearModel>();
                var _orderDetail = await _orderDetailRepository.GetAllAsync();

                _order = _order.Where(x => x.CreatedOn.Value.Year == DateTime.Now.Year && x.OrderStatus == true);

                for (int i = 1; i <= 12; i++)
                {
                    var _orderListIdMonth = _order.Where(x => x.CreatedOn.Value.Month == i);
                    if (_orderListIdMonth.Count() > 0)
                    {
                        decimal _total = 0;

                        foreach (var item in _orderListIdMonth)
                        {
                            _total += Convert.ToDecimal(_orderDetail.Where(x => x.OrderId == Convert.ToInt32(item.Id)).Sum(x => x.Price));
                        }
                        statisticsYear.Add(new StatisticsYearModel
                        {
                            Months = i,
                            Total = _total
                        });
                    }
                    else
                    {
                        statisticsYear.Add(new StatisticsYearModel
                        {
                            Months = i,
                            Total = 0,
                        });
                    }
                }
                // Tìm tháng có tổng doanh thu lớn nhất
                var highestMonth = statisticsYear.OrderByDescending(x => x.Total).FirstOrDefault();

                return Json(new { success = true, highestMonth = new { Months = highestMonth?.Months, Total = highestMonth?.Total } });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Fail: " + ex.Message });
            }
        }

        public async Task<IActionResult> GetAllProduct()
        {
            var products = await _productRepository.GetAllAsync();
            products = products.Where(x => x.Status != null).ToList();
            var result = _mapper.Map<List<ProductModel>>(products);
            return Json(new { success = true, result });


        }
        public async Task<IActionResult> GetTrend()
        {
            var products = await _productRepository.GetAllAsync();
            products = products.Where(x => x.Status != null && x.Keywords == "TREND").ToList();
            var result = _mapper.Map<List<ProductModel>>(products);
            return Json(new { success = true, result });


        }
        public async Task<IActionResult> GetPendingOrder()
        {
            var orders = await _orderRepository.GetAllAsync();
            int pending = orders.Count(x => x.OrderStatus == false);

            return Json(new { success = true, pending });


        }
        public async Task<IActionResult> GetTask()
        {
            try
            {
                var orders = await _orderRepository.GetAllAsync();
                int trueoder = orders.Count(x => x.OrderStatus == true);
                int total = orders.Count(x => x.OrderStatus != null);

                // Kiểm tra nếu totalorder là 0 trước khi thực hiện phép chia
                double task = total != 0 ? (trueoder / (double)total) * 100 : 0;

                return Json(new { success = true, trueoder, total, task });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Fail: " + ex.Message });
            }
        }
        public async Task<IActionResult> Productsold()
        {
            try
            {
                var orders = await _orderRepository.GetAllAsync();
                var products = await _productRepository.GetAllAsync();
                var orderDetails = await _orderDetailRepository.GetAllAsync();

                // Chỉ lấy đơn hàng có trạng thái hoàn thành và được tạo trong tháng hiện tại
                orders = orders.Where(x => x.Status==true && x.CreatedOn.HasValue && x.CreatedOn.Value.Month == DateTime.Now.Month).ToList();

                var productsold = orders.Join(orderDetails,
                                              order => order.Id,
                                              detail => detail.OrderId,
                                              (order, detail) => new // kết quả join
                                              {
                                                  ProductId = detail.ProductId,
                                                  Quantity = detail.Quanlity,
                                                  Price = detail.Price,
                                                  total = 0
                                              })
                                      .GroupBy(p => p.ProductId) // Nhóm theo ProductId
                                      .Select(g =>  new Sold
                                      {

                                          TotalQuantity = (int)g.Sum(x => x.Quantity), // Tổng số lượng bán ra
                                          TotalPrice = (decimal)g.Sum(x => x.Price),// Tổng doanh thu từ sản phẩm
                                          Max=(decimal)g.Max(x => x.Price),
                                          Name = products.FirstOrDefault(p => p.Id == g.Key)?.Name // Lấy tên sản phẩm
                                      })
                                      .Where(x => x.TotalPrice > 0) // Chỉ lấy những sản phẩm có doanh thu
                                      .ToList();
                // Tính tổng của tất cả các giá trị TotalPrice
                decimal totalRevenue = productsold.Sum(p => p.TotalPrice);
                decimal Maxprice = productsold.Max(p => p.TotalPrice);
                return Json(new { success = true, productsold,totalRevenue,Maxprice });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        //top 5
        public async Task<IActionResult> getTop5()
        {
            try
            {
                var orders = await _orderRepository.GetAllAsync();
                var products = await _productRepository.GetAllAsync();
                var orderDetails = await _orderDetailRepository.GetAllAsync();

                // Chỉ lấy đơn hàng có trạng thái hoàn thành và được tạo trong tháng hiện tại
                orders = orders.Where(x => x.Status == true && x.CreatedOn.HasValue && x.CreatedOn.Value.Year == DateTime.Now.Year).ToList();

                var productsold = orders.Join(orderDetails,
                                              order => order.Id,
                                              detail => detail.OrderId,
                                              (order, detail) => new // kết quả join
                                              {
                                                  ProductId = detail.ProductId,
                                                  Quantity = detail.Quanlity,
                                                  Price = detail.Price,
                                                  total = 0
                                              })
                                      .GroupBy(p => p.ProductId) // Nhóm theo ProductId
                                      .Select(g => new Sold
                                      {

                                          TotalQuantity = (int)g.Sum(x => x.Quantity), // Tổng số lượng bán ra
                                          TotalPrice = (decimal)g.Sum(x => x.Price),// Tổng doanh thu từ sản phẩm
                                          Name = products.FirstOrDefault(p => p.Id == g.Key)?.Name // Lấy tên sản phẩm
                                      })
                                      .Where(x => x.TotalPrice > 0) // Chỉ lấy những sản phẩm có doanh thu
                                      .ToList();
                // Tính tổng của tất cả các giá trị TotalPrice
                decimal totalRevenue = productsold.Sum(p => p.TotalPrice);

                // Tính percent cho mỗi sản phẩm
                var top5Products = productsold.OrderByDescending(p => p.TotalPrice)
                                   .Take(5)
                                   .Select(p => new { p.Name, p.TotalPrice, Percent = (p.TotalPrice / totalRevenue) * 100 })
                                   .ToList();

                return Json(new { success = true, top5Products });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }





        public async Task<IActionResult> CountCategory()
        {
            try
            {
                // Lấy tất cả các category từ repository
                var categories = await _categoryRepository.GetAllAsync();

                // Lấy tất cả các sản phẩm từ repository
                var products = await _productRepository.GetAllAsync();

                // Khởi tạo danh sách để lưu trữ số lượng sản phẩm trong mỗi category và tỉ lệ phần trăm
                var categoryCounts = new List<Categorynumber>();

                // Tính tổng số lượng sản phẩm có status khác null
                var totalProductCount = products.Count(x => x.Status != null);

                // Duyệt qua danh sách các category
                foreach (var category in categories)
                {
                    // Lấy số lượng sản phẩm trong category hiện tại
                    var productCount = products.Count(x => x.Category.Name == category.Name);

                    // Tính tỉ lệ phần trăm
                    double percent = totalProductCount != 0 ? (double)productCount / totalProductCount * 100 : 0;

                    // Thêm vào danh sách
                    categoryCounts.Add(new Categorynumber
                    {
                        name = category.Name,
                        number = productCount,
                        percent = percent
                    });
                }

                return Json(new { success = true, categoryCounts });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Fail: " + ex.Message });
            }
        }




    }
}
