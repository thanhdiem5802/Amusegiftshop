using Coffee.DATA.Common;
using Coffee.DATA.Models;
using Coffee.DATA.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;

namespace Coffee.WebUI.Controllers
{
    public class AccountController : Controller
    {
        private readonly IRepository<User> _userRepostory;
        private readonly IWebHostEnvironment _webHostEnvironment;
        
        public AccountController(IRepository<User> userRepostory, IWebHostEnvironment webHostEnvironment)
        {
            _userRepostory = userRepostory;
            _webHostEnvironment = webHostEnvironment;

        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Index(string email)
        {
            var _user = await _userRepostory.GetAllAsync();
            var _userEmail = _user.Where(x => x.Email.Contains(email));
            if (_userEmail.Any())
            {
                Random random = new Random();
                var randomNumber = random.Next(100000, 1000000);
                string htmlBody = GetHtmlTemplate("forgetpass.html");
                htmlBody = htmlBody.Replace("{{link}}", $"http://amusegift.runasp.net/Account/Reset?code={randomNumber}&email={email}");

                MailMessage message = new MailMessage("amusestuff001@gmail.com", email, "CodeResetPassword", htmlBody);
                message.IsBodyHtml = true;

                using (SmtpClient client = new SmtpClient("smtp.gmail.com", 587))
                {
                    client.EnableSsl = true;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new System.Net.NetworkCredential("amusestuff001@gmail.com", "wyhy dppg hlac oqxt");
                    await client.SendMailAsync(message);
                }

                HttpContext.Session.SetString("reset", randomNumber.ToString());

                var codeResetPass = HttpContext.Session.GetString("reset");
                

                return Json(new { success = true, message = "Vui lòng check email!" });
            }
            else
            {
                return Json(new { success = false, message = "Không tồn tại email!" });
            }
        }

        public string GetHtmlTemplate(string templateName)
        {
            var path = Path.Combine(_webHostEnvironment.WebRootPath, "assets", "sendmail", templateName);
            var htmlBody = System.IO.File.ReadAllText(path);
            return htmlBody;
        }
        public IActionResult Reset(string code, string email)
        {
            var codeResetPass = HttpContext.Session.GetString("reset");
            if (codeResetPass == code)
            {
                ViewBag.Code = code;
                ViewBag.Email = email;
                return View();
            }
            return StatusCode(404);
        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(string password, string email)
        {
            try
            {
                var _user = await _userRepostory.GetAllAsync();
                var _userDetail = _user.First(x => x.Email.Contains(email));
                _userDetail.Password = md5.ComputeMD5Hash(password);
                await _userRepostory.UpdateAsync(_userDetail);
                HttpContext.Session.Remove("CodeResetPassword");
                return Json(new { success = true, message = "Cập nhật mật khẩu thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Fail :" + ex.Message });
            }
        }
        
        }
    }


