using Coffee.DATA.Models;

namespace Coffee.WebUI.Areas.Admin.Model
{
    public class ReviewModel
    {
        public string? ContentReview { get; set; }
        public int? Rating { get; set; }
        public int? UserId { get; set; }
        public string UserName { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string? Reply { get; set; }
        public int ?Id { get; set; }
    }
}
