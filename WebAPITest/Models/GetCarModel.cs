using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPITest.Models
{
    /// <summary>
    /// DTO
    /// </summary>
    public class GetCarModel
    {
        public int GoodId { get; set; }

        public int Count { get; set; }
        public string Name { get; set; }
        public string Cover { get; set; }
        public decimal Price { get; set; }
    }
}
