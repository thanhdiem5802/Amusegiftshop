﻿using Coffee.DATA.Models;
using Coffee.DATA.Repository;
using Coffee.WebUI.Areas.Admin.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;

namespace Coffee.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class OrderController : Controller
    {
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<OrderDetail> _orderDetailRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public OrderController(IRepository<Order> orderRepository, IWebHostEnvironment webHostEnvironment, IRepository<User> userRepository, IRepository<OrderDetail> orderDetailRepository, IRepository<Product> productRepository)
        {
            _orderRepository = orderRepository;
            _userRepository = userRepository;
            _orderDetailRepository = orderDetailRepository;
            _productRepository = productRepository;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            return View();
        }
        public string GetHtmlTemplate(string templateName)
        {
            var path = Path.Combine(_webHostEnvironment.WebRootPath, "assets", "sendmail", templateName);
            var htmlBody = System.IO.File.ReadAllText(path);
            return htmlBody;
        }
        [HttpPost]
        public async Task<IActionResult> Index(int Id)
        {
            try
            {
                var orders = await _orderRepository.GetAllAsync();
                orders = orders.Where(x => x.OrderStatus == false);
                var orderDetails = await _orderDetailRepository.GetAllAsync();
                var users = await _userRepository.GetAllAsync();
                

                foreach (var order in orders)
                {
                    var user = users.FirstOrDefault(x => x.Id == order.UserId);
                    if (user != null)
                    {
                        string htmlBody = GetHtmlTemplate("confirmbuy.html");
                        htmlBody = htmlBody.Replace("{{customer}}", user.Name);

                        using (SmtpClient client = new SmtpClient("smtp.gmail.com", 587))
                        {
                            client.EnableSsl = true;
                            client.DeliveryMethod = SmtpDeliveryMethod.Network;
                            client.UseDefaultCredentials = false;
                            client.Credentials = new System.Net.NetworkCredential("amusestuff001@gmail.com", "wyhydppghlacoqxt");

                            MailMessage message = new MailMessage("amusestuff001@gmail.com", user.Email, "Xác nhận mua hàng thành công", htmlBody);
                            message.IsBodyHtml = true;

                            try
                            {
                                await client.SendMailAsync(message);
                            }
                            catch (Exception mailEx)
                            {
                                // Log or handle email sending error
                                return Json(new { success = false, error = mailEx.Message });
                            }
                        }
                    }
                }

                var _order = await _orderRepository.GetByIdAsync(Id);
                var _orderdetail = await _orderDetailRepository.GetAllAsync();

                _order.OrderStatus = true;
                _order.Status = true;
                // _orderdetail.Status = true;
                // _orderdetail.CreatedOn = DateTime.Now;

                await _orderRepository.UpdateAsync(_order);
                return Json(new { success = true, message = "Xác nhận giao hàng thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    error = ex.Message
                });
            }
        }

        public async Task<IActionResult> GetAllOrder()
        {
            try
            {
                var orders = await _orderRepository.GetAllAsync();
                orders = orders.Where(x => x.OrderStatus == false);
                var orderDetails = await _orderDetailRepository.GetAllAsync();
                var users = await _userRepository.GetAllAsync();
                var userOrder = new List<UserOrder>();

                foreach (var order in orders)
                {
                    var user = users.FirstOrDefault(x => x.Id == order.UserId);
                    if (user != null)
                    {
                        var total = orderDetails.Where(x => x.OrderId == order.Id).Sum(x => x.Price);
                        userOrder.Add(new UserOrder
                        {
                            OrderId = order.Id,
                            UserId = (int)order.UserId,
                            Name = user.Name,
                            Address = order.Address,
                            Province = order.Province,
                            District = order.District,
                            Town = order.Town,
                            Phone = user.Phone, 
                            Total = (decimal)total,
                            Status = (bool)order.Status,
                            OrderStatus = (bool)order.OrderStatus
                        });
                    }
                }
                return Json(new { success = true, userOrder });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }
        [HttpPost]
        public async Task<IActionResult> GetAllOrderDetail(int Id)
        {
            try
            {
                var _orderDetail = await _orderDetailRepository.GetAllAsync();
                var _product = await _productRepository.GetAllAsync();
                _orderDetail = _orderDetail.Where(x => x.OrderId == Id);
                var result = new List<OrderDetailModel>();
                foreach (var item in _orderDetail)
                {
                    result.Add(new OrderDetailModel
                    {
                        Id = item.Id,
                        ProductName = _product.First(x => x.Id == item.ProductId).Name,
                        Quantity = item.Quanlity,
                    });
                }
                return Json(new { success = true, result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }
    }
}
