namespace Coffee.WebUI.Models
{
    public class PaymentData
    {
        private object items;

        public PaymentData(string OrderCode, long amount, string description, object items, string? cancelUrl, string? returnUrl)
        {
            this.OrderCode = OrderCode;
            Amount = amount;
            Description = description;
            this.items = items;
            CancelUrl = cancelUrl;
            ReturnUrl = returnUrl;
        }

        public string OrderCode { get; set; }
        public long Amount { get; set; }
        public string Description { get; set; }
        public string CancelUrl { get; set; }
        public string ReturnUrl { get; set; }
    }
}