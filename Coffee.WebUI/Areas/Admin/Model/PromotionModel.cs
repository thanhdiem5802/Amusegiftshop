namespace Coffee.WebUI.Areas.Admin.Model
{
    public class PromotionModel
    {
        public int Id { get; set; }
        public string PromoName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Code { get; set; }
        public bool Used { get; set; }
        public string description { get; set; }
        public int Quantity { get; set; }
    }
}
