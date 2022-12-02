using System;
using System.Collections.Generic;

#nullable disable

namespace MVC_Demo.Models.Database
{
    public partial class User
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public DateTime CraeteTime { get; set; }
        public string Photo { get; set; }
        public string Desc { get; set; }
        public string Str { get; set; }
    }
}
