using System;
using System.Collections.Generic;

#nullable disable

namespace MVC_Demo.Models.Database
{
    public partial class Car
    {
        public int RecordId { get; set; }
        public int GoodId { get; set; }
        public int UserId { get; set; }
        public int Count { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
