using AutoMapper;
using Coffee.DATA.Models;
using Coffee.DATA.Repository;
using Coffee.WebUI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.Identity.Client;
using NuGet.Protocol.Core.Types;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.Xml;
using System.Security.Policy;

namespace Coffee.WebUI.Controllers
{
    public class ProductDetailController : Controller
    {
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<ProductImage> _productImageRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Review> _reviewRepository;
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<OrderDetail> _orderDetailRepository;
        private readonly IRepository<Promotion> _promotionRepository;
        private readonly IMapper _mapper;
        public ProductDetailController(IRepository<Order> orderRepository, IRepository<Promotion> promotionReposity, IRepository<OrderDetail> orderDetailRepository, IRepository<Review> reviewRepository, IRepository<User> userRepository, IMapper mapper, IRepository<Product> productRepository, IRepository<ProductImage> productImageRepository, IRepository<Category> categoryRepository)
        {
            _orderRepository = orderRepository;
            _orderDetailRepository = orderDetailRepository;
            _reviewRepository = reviewRepository;
            _userRepository = userRepository;
            _mapper = mapper;
            _productRepository = productRepository;
            _productImageRepository = productImageRepository;
            _categoryRepository = categoryRepository;
            _promotionRepository = promotionReposity;
        }

        public async Task<IActionResult> Index(int id)
        {
            var _productDetail = await _productRepository.GetByIdAsync(id);
            if (_productDetail == null)
            {
                return StatusCode(404);
            }
            if (_productDetail.Status == false)
            {
                return StatusCode(404);
            }
            var _category = await _categoryRepository.GetAllAsync();
            var _productImage = await _productImageRepository.GetAllAsync();
            _productImage = _productImage.Where(x => x.ProductId == id);
            ViewBag.Name = _productDetail.Name;
            // Khởi tạo danh sách hình ảnh sản phẩm
            var productImages = new List<ProductImageModel>();

            // Lặp qua danh sách hình ảnh sản phẩm và tạo đối tượng ProductImageModel cho mỗi hình ảnh
            foreach (var productImage in _productImage)
            {
                productImages.Add(new ProductImageModel
                {
                    ProductImageId = productImage.Id,
                    ImageUrl = productImage.UrlImage,
                });
            }
            var result = new ProductModel
            {
                ProductId = _productDetail.Id,
                Name = _productDetail.Name,
                Image = _productDetail.Image,
                Description = _productDetail.Description,
                DescriptionShort = _productDetail.DescriptionShort,
                Quantity = _productDetail.Quantity,
                Keywords = _productDetail.Keywords,
                Price = _productDetail.Price,
                DiscountPrice = _productDetail.DiscountPrice,
                categoryId = _productDetail.CategoryId,
                categoryName = _category.First(x => x.Id == _productDetail.CategoryId).Name,
                ProductImages = productImages
            };
            var _reletadProduct = await _productRepository.GetAllAsync();
            _reletadProduct = _reletadProduct.Where(x => x.CategoryId == _productDetail?.CategoryId);
            // Tạo một instance của lớp Random
            Random random = new Random();

            // Randomize các phần tử trong _reletadProduct
            _reletadProduct = _reletadProduct.OrderBy(x => random.Next());
            ViewBag.ReletadProduct = _reletadProduct.Take(4).ToList();
            var _review = await _reviewRepository.GetAllAsync();
            if (_review == null)
            {
                return StatusCode(404, "No reviews found.");
            }

            // Bây giờ lọc review dựa trên ProductId và xử lý trường hợp mà 'Reply' mà null
            var filteredReviews = _review
                .Where(r => r.ProductId == id)
                .Select(r =>
                {
                    // Đưa ra giá trị mặc định nếu 'Reply' là null
                    r.Reply = r.Reply ;
                    return r;
                })
                .ToList();

            // Ánh xạ từ đối tượng Review sang ReviewModel
            var listReview = _mapper.Map<List<ReviewModel>>(filteredReviews);

            // Lấy thông tin người dùng - chỉ làm điều này nếu cần thông tin người dùng trong chức năng của bạn
            var _user = await _userRepository.GetAllAsync(); // Lưu ý: Đoạn code này vẫn không hiệu quả nếu bạn không cần thông tin user cho mỗi review.

            if (listReview.Count() > 0)
            {
                // Định cấu hình CultureInfo cho Việt Nam
                CultureInfo cultureInfo = new CultureInfo("vi-VN");

                // Định cấu hình định dạng ngày giờ theo ngôn ngữ Việt Nam
                cultureInfo.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";
                cultureInfo.DateTimeFormat.LongTimePattern = "HH:mm:ss";

                foreach (var item in listReview)
                {
                    item.UserName = _user.First(x => x.Id == item.UserId).Name;
                }
                ViewBag.Review = listReview;
            }

            var _reviewRating = _review.Where(x => x.ProductId == id);
            if (_reviewRating.Count() > 0)
            {
                decimal _rating = (decimal)(_review.Where(x => x.ProductId == id).Sum(x => x.Rating) / _review.Where(x => x.ProductId == id).Count());
                ViewBag.Rating = Math.Round(_rating, 1);
                ViewBag.CountRating = _reviewRating.Count();
            }
            else
            {
                ViewBag.CountRating = 0;
                ViewBag.Rating = 0;
            }
            if (User.FindFirst(ClaimTypes.Email)?.Value != null)
            {
                var userReviewId = _user.First(x => x.Email == User.FindFirst(ClaimTypes.Email)?.Value).Id;
                if (_review.Where(x => x.UserId == userReviewId && x.ProductId == id).Count() > 0)
                {
                    ViewBag.ReviewStatus = 1;
                }
                else
                {
                    var _order = await _orderRepository.GetAllAsync();
                    _order = _order.Where(x => x.UserId == userReviewId);

                    // Lấy danh sách các orderId từ _order
                    var orderIds = _order.Select(order => order.Id).ToList();

                    // Lấy các _orderDetail có orderId trong danh sách orderIds
                    var _orderDetails = await _orderDetailRepository.GetAllAsync();
                    var filteredOrderDetails = _orderDetails.Where(detail => orderIds.Contains((int)detail.OrderId));
                    filteredOrderDetails = filteredOrderDetails.Where(x => x.ProductId == id);
                    if (filteredOrderDetails.Count() > 0)
                    {
                        ViewBag.ReviewStatus = 0;
                    }
                }
            }
            return View(result);
        }
        public async Task<IActionResult> Review(int rating, string message, int Id)
        {
            try
            {
                var _user = await _userRepository.GetAllAsync();
                var _userId = _user.First(x => x.Email == User.FindFirst(ClaimTypes.Email)?.Value).Id;
                var review = new Review
                {
                    Rating = rating,
                    ContentReview = message,
                    UserId = _userId,
                    ProductId = Id,
                    CreatedOn = DateTime.Now,
                    Status = true,

                };
                await _reviewRepository.InsertAsync(review);
                return Json(new
                {
                    success = true,
                    message = "Cảm ơn bạn đã đánh giá sản phẩm!"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Fail: " + ex.Message
                });
            }
        }


        public async Task<IActionResult> ApplyPromotionCode(int id, string promoCode)
        {
            var currentDate = DateTime.Now;

            // Fetch the product by its ID
            var _productDetail = await _productRepository.GetByIdAsync(id);
            if (_productDetail == null || _productDetail.Status == false)
            {
                return StatusCode(404);
            }

            // Trim leading and trailing whitespaces from promoCode
            promoCode = promoCode.Trim();

            // Fetch all promotions
            var _promotions = await _promotionRepository.GetAllAsync();

            // Initialize flag to track if promo code is found
            bool isPromoCodeFound = false;

            // Check each promotion for matching code
            foreach (var promotion in _promotions)
            {
                var promotionCode = promotion.Code.Trim();

                if (promotionCode == promoCode)
                {
                    isPromoCodeFound = true;

                    if (promotion.Used == false)
                    {
                        // Check if the promotion is within the valid date range
                        if (currentDate >= promotion.StartDate && currentDate <= promotion.EndDate)
                        {
                            // Calculate the discounted price
                            var discountPrice = _productDetail.Price - (_productDetail.Price * promotion.discount_percentage / 100);
                            var percentage = promotion.discount_percentage;

                            HttpContext.Session.SetString("AppliedPromoCode", promoCode);

                            // Return the discounted price and success status
                            return Json(new { success = true, originalPrice = _productDetail.Price, discountPrice, percentage });
                        }
                        else
                        {
                            // Promotion has expired
                            return Json(new { success = false, message = "Mã khuyến mãi đã hết hạn vào ngày " + promotion.EndDate });
                        }
                    }
                    else
                    {
                        // Promotion code has already been used
                        return Json(new { success = false, message = "Mã khuyến mãi đã được sử dụng trước đó" });
                    }
                }
            }

            // If no matching promotion is found
            if (!isPromoCodeFound)
            {
                return Json(new { success = false, message = "Không tìm thấy mã khuyến mãi" });
            }

            // Default error message
            return Json(new { success = false, message = "Có lỗi xảy ra khi nhập mã" });
        }

    }
}







