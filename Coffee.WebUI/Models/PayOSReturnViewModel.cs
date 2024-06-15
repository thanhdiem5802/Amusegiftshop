namespace Coffee.WebUI.Models
{
    public class PayOSReturnViewModel
    {
        public string? Code { get; set; }
        public string? Id { get; set; }
        public bool? Cancel { get; set; }
        public string? Status { get; set; }
        public long? OrderCode { get; set; }
    }
}
