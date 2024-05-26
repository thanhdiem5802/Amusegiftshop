using Coffee.DATA;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Coffee.WebUI.Models;
using Coffee.DATA.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Google;
using System.Net.Mail;
using static System.Net.WebRequestMethods;
using Microsoft.AspNetCore.Http;
using Coffee.DATA.Models;
using System.Net.Http;
using Coffee.DATA.Repository;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.AspNetCore.Hosting;

namespace Coffee.WebUI.Controllers
{
    public class LoginController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly DbCoffeeDbContext _dbCoffeeDbContext;
        private readonly IRepository<User> _userRepository;

        public LoginController(DbCoffeeDbContext dbCoffeeDbContext, IWebHostEnvironment webHostEnvironment, IRepository<User> userRepository)
        {
            _webHostEnvironment = webHostEnvironment;
            _dbCoffeeDbContext = dbCoffeeDbContext;
            _userRepository = userRepository;
        }

        public IActionResult Index(string? error)
        {
            if (error == "false")
            {
                ViewData["ErrorMessage"] = "Tài khoản của bạn đã bị khoá vui lòng liên hệ Admin để biết thêm!";
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var hashedPassword = md5.ComputeMD5Hash(model.Password);
                var user = await _dbCoffeeDbContext.Users.FirstOrDefaultAsync(x => x.UserName.Contains(model.Username) && x.Password == hashedPassword);
                if (user == null)
                {
                    ViewData["ErrorMessage"] = "Tên đăng nhập hoặc mật khẩu không chính xác.";
                    return View(model);
                }
                if (user.Status == false)
                {
                    ViewData["ErrorMessage"] = "Tài khoản của bạn đã bị khoá vui lòng liên hệ Admin để biết thêm!";
                    return View(model);
                }
                var role = await _dbCoffeeDbContext.Roles.FirstOrDefaultAsync(x => x.Id == user.RoleId);
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.Role, role.Name)
                };
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View(model);
            }
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult GoogleLogin()
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("GoogleResponse", "Login")
            };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (result.Succeeded)
            {
                var user = result.Principal;
                var emailClaim = user.FindFirst(ClaimTypes.Email).Value;
                var checkEmail = _dbCoffeeDbContext.Users.Where(x => x.Email == emailClaim);
                if (checkEmail.Count() < 1)
                {
                    var newUser = new User { Email = emailClaim, RoleId = 2, Status = true, CreatedOn = DateTime.Now };
                    _dbCoffeeDbContext.Users.Add(newUser);
                    _dbCoffeeDbContext.SaveChanges();
                }
                if (checkEmail.First().Status == false)
                {
                    await HttpContext.SignOutAsync();
                    return RedirectToAction("Index", "Login", new { area = "", error = "false" });
                }
            }
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Register()
        {
            return View();
        }

        public string GetHtmlTemplate(string templateName)
        {
            var path = Path.Combine(_webHostEnvironment.WebRootPath, "assets", "sendmail", templateName);
            var htmlBody = System.IO.File.ReadAllText(path);
            return htmlBody;
        }
        //send otp
        [HttpPost]
        [Route("/send-otp")]
        public async Task<IActionResult> SendOTPEmail(string email)
        {
            

            var checkEmail = await _userRepository.GetAllAsync();
            if (checkEmail.Where(x => x.Email == email).Count() > 0)
            {
                return Json(new { success = false, message = "Email đã tồn tại!" });
            }

            // Generate OTP 
            Random random = new Random();
            var randomNumber = random.Next(100000, 1000000);

            // Read HTML template content from file
            string htmlBody = GetHtmlTemplate("sendmail.html");

            // Replace OTP placeholder with actual OTP
            htmlBody = htmlBody.Replace("{{OTP}}", randomNumber.ToString());

            // Create MailMessage object
            MailMessage message = new MailMessage("amusestuff001@gmail.com", email, "OTP", htmlBody);
            message.IsBodyHtml = true;

            // SMTP client configuration
            SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
            client.EnableSsl = true;
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential("amusestuff001@gmail.com", "wyhy dppg hlac oqxt");

            try
            {
                // Send email
                await client.SendMailAsync(message);

                // Store OTP in session
                HttpContext.Session.SetString("OTP", randomNumber.ToString());
                HttpContext.Session.SetString("OTPGenerationTime", DateTime.Now.ToString());

                // Return success message
                return Json(new { success = true, message = "Vui lòng xem email để lấy mã OTP!" });
            }
            catch (Exception ex)
            {
                // Handle email sending error
                return Json(new { success = false, message = "Failed to send email: " + ex.Message });
            }
        }
        //resend otp
        [HttpPost]
        [Route("/resend-otp")]
        public async Task<IActionResult> ResendOTPEmail(string email)
        {
            // Generate new OTP
            Random random = new Random();
            var newOtp = random.Next(100000, 1000000);

            // Read HTML template content from file
            string htmlBody = GetHtmlTemplate("sendmail.html");

            // Replace OTP placeholder with the new OTP
            htmlBody = htmlBody.Replace("{{OTP}}", newOtp.ToString());

            // Create MailMessage object
            MailMessage message = new MailMessage("amusestuff001@gmail.com", email, "OTP", htmlBody);
            message.IsBodyHtml = true;

            // SMTP client configuration
            SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
            client.EnableSsl = true;
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential("amusestuff001@gmail.com", "wyhy dppg hlac oqxt");

            try
            {
                // Send email
                await client.SendMailAsync(message);

                // Remove old OTP from session
                HttpContext.Session.Remove("OTP");
                HttpContext.Session.Remove("reOTP");

                // Store new OTP in session
                HttpContext.Session.SetString("reOTP", newOtp.ToString());
                HttpContext.Session.SetString("OTPGenerationTime", DateTime.Now.ToString());

                // Return success message
                return Json(new { success = true, message = "Vui lòng xem email để lấy mã OTP mới!" });
            }
            catch (Exception ex)
            {
                // Handle email sending error
                return Json(new { success = false, message = "Failed to send email: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Register(string email, string password, string otp, string name, string username)
        {
            
            var checkUsername = await _userRepository.GetAllAsync();
            if (checkUsername.Any(x => x.UserName == username && x.UserName != null))
            {
                return Json(new { success = false, message = "Tên đăng nhập đã tồn tại!" });
            }

            var otpGenerationTimeStr = HttpContext.Session.GetString("OTPGenerationTime");
            var otpss = HttpContext.Session.GetString("OTP");
            var reotpss = HttpContext.Session.GetString("reOTP");
            DateTime otpGenerationTime;

            if (!DateTime.TryParse(otpGenerationTimeStr, out otpGenerationTime) || (DateTime.Now - otpGenerationTime).TotalMinutes > 2)
            {
                return Json(new { success = false, message = "Mã OTP đã hết hạn." });
            }

            if (otpss == otp || otp == reotpss )
            {
                var _user = new User { Email = email, Password = md5.ComputeMD5Hash(password), Status = true, CreatedOn = DateTime.Now, RoleId = 2, Name = name, UserName = username };
                try
                {
                    await _userRepository.InsertAsync(_user);
                    HttpContext.Session.Remove("OTP");
                    HttpContext.Session.Remove("reOTP");

                    return Json(new { success = true, message = "Đăng kí thành công!" });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "Fail: " + ex });
                }
            }
            else
            {
                return Json(new { success = false, message = "Mã OTP không khớp" });
            }
        }
    }
}


