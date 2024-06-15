using Microsoft.AspNetCore.Mvc;

namespace Coffee.WebUI.Areas.Admin.Controllers
{
    public class StaffController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
