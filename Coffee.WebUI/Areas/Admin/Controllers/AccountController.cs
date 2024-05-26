using AutoMapper;
using Coffee.DATA.Models;
using Coffee.DATA.Repository;
using Coffee.WebUI.Areas.Admin.Model;
using Coffee.WebUI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;

namespace Coffee.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AccountController : Controller
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Review> _reviewRepository;
        private readonly IMapper _mapper;
        public AccountController(IRepository<User> userRepository, IRepository<Review> reviewReposity, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _reviewRepository = reviewReposity;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> GetAllAccount()
        {
            var _user = await _userRepository.GetAllAsync();
            _user = _user.Where(x => x.RoleId != 1);
            var result = _mapper.Map<List<AccountUserModel>>(_user);
            return Json(new { success = true, result });
        }
        public async Task<IActionResult> Toggle(int Id)
        {
            try
            {
                var _user = await _userRepository.GetByIdAsync(Id);
                if (_user == null)
                {
                    return StatusCode(404);
                }
                else
                {
                    if (_user.Status == true)
                    {
                        _user.Status = false;
                        await _userRepository.UpdateAsync(_user);
                        return Json(new { success = true, message = "Đã khoá tài khoản!" });
                    }
                    else
                    {
                        _user.Status = true;
                        await _userRepository.UpdateAsync(_user);
                        return Json(new { success = true, message = "Đã mở khoá tài khoản!" });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Fail: " + ex.Message });
            }
        }
        [HttpPost]
        public async Task<IActionResult> Review(int id)
        {
            try
            {
                // Lấy tất cả các review
                var _review = await _reviewRepository.GetAllAsync();

                // Lọc các review theo userId
                var userReviews = _review.Where(x => x.UserId == id).ToList();

                // Kiểm tra nếu không có review nào cho user này
                if (!userReviews.Any())
                {
                    return StatusCode(404, new { success = false, message = "No reviews found for this user." });
                }

                // Map các review sang mô hình cần thiết (nếu có sử dụng AutoMapper)
                var result = userReviews.Select(review => new Review
                {
                    Id = review.Id,
                    CreatedOn = review.CreatedOn,
                    ContentReview = review.ContentReview,
                    Rating = review.Rating,
                    ProductId = review.ProductId,

                    // Các thuộc tính khác tùy thuộc vào mô hình của bạn
                }).ToList();

                return Json(new { success = true, reviews = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Fail: " + ex.Message });
            }
        }

    }
}
