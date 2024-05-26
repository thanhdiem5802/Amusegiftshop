using AutoMapper;
using Coffee.DATA.Models;
using Coffee.DATA.Repository;
using Coffee.WebUI.Areas.Admin.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Coffee.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ReviewController : Controller
    {
        private readonly IRepository<Review> _reviewRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IWebHostEnvironment _environment;
        private readonly IRepository<ReviewModel> _reviewModelRepository;
        private readonly IMapper _mapper;
        public ReviewController(IRepository<Review> ReviewReposity, IRepository<ReviewModel> ReviewModelRepository, IWebHostEnvironment environment, IRepository<User> userRepository, IRepository<Product> productRepository, IMapper mapper)
        {

            _environment = environment;
            _userRepository = userRepository;
            _reviewRepository = ReviewReposity;
            _productRepository = productRepository;
            _mapper = mapper;
            _reviewModelRepository = ReviewModelRepository;

        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> GetReview()
        {
            try
            {
                var _review = await _reviewRepository.GetAllAsync();
                var _user = await _userRepository.GetAllAsync();
                var _product = await _productRepository.GetAllAsync();
                var result = _mapper.Map<List<ReviewModel>>(_review);
                foreach (var item in result)
                {
                    item.UserName = _user.First(x => x.Id == item.UserId).Name;
                    item.ProductName = _product.First(p => p.Id == item.ProductId).Name;
                    
                }
                return Json(new { success = true, result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Fail: " + ex });
            }
        }
        [HttpPost]
        public async Task<IActionResult> AdminReply(int id, string reply)
        {
            try
            {
                // Retrieve the review by id from the repository
                var review = await _reviewRepository.GetByIdAsync(id);
                
                

                // Check if the review exists
                if (review == null)
                {
                    return NotFound(new { success = false, message = "Review not found." });
                }

                // Update the review with the admin reply
                review.Reply = reply;

                // Save the updated review to the repository
                await _reviewRepository.UpdateAsync(review);

                // Return success response
                return Json(new { success = true, message = "Update successful!", reply });
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                // For example: _logger.LogError(ex, "Error updating review with id {Id}", id);

                // Return an error response
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }




    }
}
