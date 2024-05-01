﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coffee.DATA.Models
{
    public class Role : BaseEntities
    {
        public string? Name { get; set; }
        public ICollection<User> Users { get; set;} = new List<User>();
    }
}
