using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVC_Demo.Models
{
    public class CarGood
    {
        public int GoodId { get; set; }
        public string UserId { get; set; }
        public int Count { get; set; }
        public string GoodName { get; set; }
        public string Cover { get; set; }
        public decimal Price { get; set; }
        public string action { get; set; }
    }
}
