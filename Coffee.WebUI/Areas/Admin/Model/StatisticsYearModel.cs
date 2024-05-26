namespace Coffee.WebUI.Areas.Admin.Model
{
    public class StatisticsYearModel
    {
        public int Months { get; set; }
        public decimal Total { get; set; } 
    }
    public class Categorynumber
    {
        public int number { get; set; }
        public string name { get; set; }
        public double percent { get; set; }
    }
    public class Sold
    {
        public string Name { get; set; }
        public decimal Total { get; set; }
        public int TotalQuantity { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal Max { get; set; }
    }

}
