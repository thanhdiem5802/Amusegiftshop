using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coffee.DATA.Models
{
    public class Promotion : BaseEntities
    {

        
        public string? PromoName { get; set; }
        
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Code { get; set; }
        public decimal? discount_percentage { get; set; }
        public bool? Used { get; set; }
        public string? description { get; set; }
        public int? Quantity { get; set; }
    }
}
