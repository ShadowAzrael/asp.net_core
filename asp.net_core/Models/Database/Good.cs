using System;
using System.Collections.Generic;

#nullable disable

namespace MVC_Demo.Models.Database
{
    public partial class Good
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CateId { get; set; }
        public string Cover { get; set; }
        public decimal Price { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
        public int Stock { get; set; }
    }
}
