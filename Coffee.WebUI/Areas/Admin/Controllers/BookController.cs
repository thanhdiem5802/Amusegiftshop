using Coffee.DATA.Models;
using Coffee.DATA.Repository;
using Coffee.DATA.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using NuGet.Protocol.Core.Types;
using System.Net.Mail;

namespace Coffee.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class BookController : Controller
    {
        private readonly IRepository<Book> _bookingRepository;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public BookController(IRepository<Book> bookingRepository, IHubContext<NotificationHub> hubContext, IWebHostEnvironment webHostEnvironment)
        {
            _bookingRepository = bookingRepository;
            _hubContext = hubContext;
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
                var _book = await _bookingRepository.GetByIdAsync(Id);
                if (_book == null)
                {
                    return StatusCode(404);
                }
                else
                {
                    // Mật khẩu ứng dụng OtpEmail : kemz hkfu jode ctfp
                    string htmlBody = GetHtmlTemplate("confirm.html");
                    MailMessage message = new MailMessage("amusestuff001@gmail.com", _book.Email, "Xác nhận đơn đặt hàng", htmlBody);
                    message.IsBodyHtml = true;
                    SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
                    client.EnableSsl = true;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new System.Net.NetworkCredential("amusestuff001@gmail.com", "wyhy dppg hlac oqxt");
                    client.Send(message);

                    _book.Status = true;
                    await _bookingRepository.UpdateAsync(_book);
                }
                return Json(new { success = true, message = "Duyệt thành công và gữi email xác nhận tới khách hàng!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Fail: " + ex });
            }
        }
        public async Task<IActionResult> GetAllBook()
        {
            try
            {
                var result = await _bookingRepository.GetAllAsync();
                result = result.OrderByDescending(x => x.Id);
                return Json(new { success = true, result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Fail: " + ex });
            }
        }
    }
}
