using AutoMapper;
using Coffee.DATA.Common;
using Coffee.DATA.Models;
using Coffee.DATA.Repository;
using Coffee.WebUI.Areas.Admin.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;

namespace Coffee.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class PromotionController : Controller
    {
        private readonly IRepository<Promotion> _promotionRepository;
        private readonly IMapper _mapper;

        public PromotionController(IRepository<Promotion> Promotionrepository, IMapper mapper)
        {
            _promotionRepository = Promotionrepository;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(Promotion model)
        {
            Console.WriteLine($"Received model: {JsonConvert.SerializeObject(model)}");

            if (model.Quantity <= 0)
            {
                return Json(new { success = false, message = "Quantity must be greater than zero." });
            }
            try
            {
                // Loop through the quantity and create promotion codes
                for (int i = 0; i < model.Quantity; i++)
                {
                    var code = GeneratePromoCode(model.PromoName, i + 1); // Generate a unique code
                    var promotion = new Promotion
                    {
                        StartDate = model.StartDate,
                        EndDate = model.EndDate,
                        PromoName = model.PromoName,
                        Code = code,
                        Used = model.Used,
                        description = model.description,
                        discount_percentage = model.discount_percentage
                    };
                    await _promotionRepository.InsertAsync(promotion);
                }

                return Json(new { success = true, message = "Thêm mã giảm giá thành công!" });
            }
            catch (Exception ex)
            {
                var errorMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return Json(new { success = false, message = "Fail: " + errorMessage });
            }
        }

        private string GeneratePromoCode(string promoName, int index)
        {
            var randomString = GenerateRandomString(8);
            return $"{promoName}-{index}-{randomString}";
        }

        private string GenerateRandomString(int length)
        {
            var characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var result = new char[length];
            var random = new Random();
            for (int i = 0; i < length; i++)
            {
                result[i] = characters[random.Next(characters.Length)];
            }
            return new string(result);
        }
    }
}
