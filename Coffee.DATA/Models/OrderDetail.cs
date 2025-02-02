﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coffee.DATA.Models
{
    public class OrderDetail : BaseEntities
    {
        public int? ProductId { get; set; }

        public int? OrderId { get; set; }

        public decimal? Price { get; set; }

        public int? Quanlity { get; set; }

        public virtual Order? Order { get; set; }

        public virtual Product? Product { get; set; } 
        //public virtual Promotion? Promotion { get; set; }
    }
}
