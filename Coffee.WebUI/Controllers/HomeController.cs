using Coffee.WebUI.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using System.Security.Cryptography;
using Coffee.DATA;
using Coffee.DATA.Models;
using System.Net.Mail;
using Microsoft.AspNetCore.SignalR;
using Coffee.DATA.Service;
using Coffee.DATA.Repository;
using Microsoft.AspNetCore.Hosting;

namespace Coffee.WebUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IRepository<Book> _bookRepository;
        private readonly IRepository<Contact> _contactRepository;
        private readonly IRepository<Product> _productRepository;

        public HomeController(IRepository<Product> productRepository, ILogger<HomeController> logger, IHubContext<NotificationHub> hubContext, IRepository<Contact> contactRepository, IRepository<Book> bookRepository, IWebHostEnvironment webHostEnvironment)
        {
            _productRepository = productRepository;
            _logger = logger;
            _hubContext = hubContext;
            _contactRepository = contactRepository;
            _bookRepository = bookRepository;
            _webHostEnvironment = webHostEnvironment;
            
        }
        public async Task<IActionResult> Index()
        {
            var _product = await _productRepository.GetAllAsync();
            _product = _product.Where(x => x.DiscountPrice > 0 && x.Status == true);

            var _proDiscount = _product.OrderBy(x => Guid.NewGuid()).Take(6).ToList();
            ViewBag.ProductDiscount = _proDiscount;
            return View();
        }


        public IActionResult Introduce()
        {
            return View();
        }
        public IActionResult Contact()
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
        public async Task<IActionResult> Contact(Contact model)
        {
            if (ModelState.IsValid)
            {
                var _contact = new Contact
                {
                    Name = model.Name,
                    Email = model.Email,
                    Title = model.Title,
                    Content = model.Content,
                    CreatedOn = DateTime.Now,
                    Status = true,
                };
                await _contactRepository.InsertAsync(_contact);
                string htmlBody = GetHtmlTemplate("forgetpass.html");
                MailMessage message = new MailMessage("amusestuff001@gmail.com", "amusestuff001@gmail.com", "Khách hàng muốn contact", $"Tên khách hàng: {model.Name},  Email:{model.Email},Title:{model.Title},Nội Dung:{model.Content}");
                MailMessage Diem = new MailMessage("amusestuff001@gmail.com", "diemttde160165@fpt.edu.vn", "Khách hàng muốn contact", $"Tên khách hàng: {model.Name},  Email:{model.Email}, Title:{model.Title},Nội Dung:{model.Content}");
                MailMessage Quyen = new MailMessage("amusestuff001@gmail.com", "ngoquyen2405@gmail.com", "Khách hàng muốn contact", $"Tên khách hàng: {model.Name},  Email:{model.Email}, Title:{model.Title},Nội Dung:{model.Content}");
                MailMessage Tram = new MailMessage("amusestuff001@gmail.com", "tramhbds160024@fpt.edu.vn", "Khách hàng muốn contact", $"Tên khách hàng: {model.Name},  Email:{model.Email}, Title:{model.Title},Nội Dung:{model.Content}");
                MailMessage Huy = new MailMessage("amusestuff001@gmail.com", "Caoxuanhuy29102002@gmail.com", "Khách hàng muốn contact", $"Tên khách hàng: {model.Name},  Email:{model.Email}, Title:{model.Title},Nội Dung:{model.Content}");
                MailMessage Nhi = new MailMessage("amusestuff001@gmail.com", "nhihhds160051@fpt.edu.vn", "Khách hàng muốn contact", $"Tên khách hàng: {model.Name},  Email:{model.Email}, Title:{model.Title},Nội Dung:{model.Content}");
                MailMessage Nhung = new MailMessage("amusestuff001@gmail.com", "nhunghtkds160479@fpt.edu.vn", "Khách hàng muốn contact", $"Tên khách hàng: {model.Name},  Email:{model.Email}, Title:{model.Title},Nội Dung:{model.Content}");
                SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
                client.EnableSsl = true;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Credentials = new System.Net.NetworkCredential("amusestuff001@gmail.com", "webb dong hdwk seil");
                ViewBag.Success = "Cảm ơn quý khách đã gữi liên hệ!";
                //client.Send(message);
                client.Send(Diem);
                //client.Send(Quyen);
                //client.Send(Huy);
                //client.Send(Tram);
                //client.Send(Nhi);
                //client.Send(Nhung);
            }
            return View();
        }
        public IActionResult Reservation()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Reservation(Book model)
        {
            try
            {
                var _book = new Book
                {
                    Name = model.Name,
                    Email = model.Email,
                    Phone = model.Phone,
                    Day = model.Day,
                    PaymentMethod = model.PaymentMethod,
                    Seates = model.Seates,
                    Status = false,
                    Noted = model.Noted,

                };
                await _bookRepository.InsertAsync(_book);
                ViewBag.Success = "Cảm ơn quý khách đã đặt hàng. Vui lòng check lại email để lấy thông tin ";
                // Mật khẩu ứng dụng OtpEmail : webb dong hdwk seil
                string htmlBody = GetHtmlTemplate("reservation.html");

                MailMessage message = new MailMessage("amusestuff001@gmail.com", model.Email, "Thông tin đặt Hàng", htmlBody);
                message.IsBodyHtml = true;
                MailMessage Diem = new MailMessage("amusestuff001@gmail.com", "diemttde160165@fpt.edu.vn", "    Đơn đặt hàng  ", $" Điềm ơi bạn có một đơn đặt hàng của  {model.Name}, Số lượng: {model.Seates}, Phone: {model.Phone}, Email:{model.Email}, Hình thức thanh toán:{model.PaymentMethod},Ngày đặt hàng:{model.Day},Noted: {model.Noted}");
                MailMessage Quyen = new MailMessage("amusestuff001@gmail.com", "ngoquyen2405@gmail.com", " Đơn đặt hàng  ", $"Quyền ơi bạn có một đơn đặt hàng của {model.Name}, Số lượng: {model.Seates}, Phone: {model.Phone}, Email:{model.Email}, Hình thức thanh toán:{model.PaymentMethod},Ngày đặt hàng:{model.Day},Noted: {model.Noted}");
                MailMessage Tram = new MailMessage("amusestuff001@gmail.com", "tramhbds160024@fpt.edu.vn", " Đơn đặt hàng  ", $"Trâm ơi bạn có một đơn đặt hàng của {model.Name}, Số lượng: {model.Seates}, Phone: {model.Phone}, Email:{model.Email}, Hình thức thanh toán:{model.PaymentMethod},Ngày đặt hàng:{model.Day},Noted: {model.Noted}");
                MailMessage Nhi = new MailMessage("amusestuff001@gmail.com", "nhihhds160051@fpt.edu.vn", " Đơn đặt hàng  ", $"Nhi ơi bạn có một đơn đặt hàng của {model.Name}, Số lượng: {model.Seates}, Phone: {model.Phone}, Email:{model.Email}, Hình thức thanh toán:{model.PaymentMethod},Ngày đặt hàng:{model.Day},Noted: {model.Noted}");
                MailMessage Huy = new MailMessage("amusestuff001@gmail.com", "Caoxuanhuy29102002@gmail.com", " Đơn đặt hàng  ", $"Huy ơi bạn có một đơn đặt hàng của {model.Name}, Số lượng: {model.Seates}, Phone: {model.Phone}, Email:{model.Email}, Hình thức thanh toán:{model.PaymentMethod},Ngày đặt hàng:{model.Day},Noted: {model.Noted}");
                MailMessage Nhung = new MailMessage("amusestuff001@gmail.com", "nhunghtkds160479@fpt.edu.vn", " Đơn đặt hàng  ", $"Nhung ơi bạn có một đơn đặt hàng của {model.Name}, Số lượng: {model.Seates}, Phone: {model.Phone}, Email:{model.Email}, Hình thức thanh toán:{model.PaymentMethod},Ngày đặt hàng:{model.Day},Noted: {model.Noted}");

                SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
                client.EnableSsl = true;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Credentials = new System.Net.NetworkCredential("amusestuff001@gmail.com", " webb dong hdwk seil");
                client.Send(message);
                client.Send(Diem);
                //client.Send(Quyen);
                //client.Send(Huy);
                //client.Send(Tram);
                //client.Send(Nhi);
                //client.Send(Nhung);
                await _hubContext.Clients.All.SendAsync("ReceiveMessage");
            }
            catch (Exception ex)
            {
                return View(ex);
            }
            return View();

        }
        public IActionResult Error()
        {
            return View();
        }
    }
}
