using System;
using System.Collections.Generic;

#nullable disable

namespace WebAPITest.Models.Database
{
    public partial class User
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public byte Salt { get; set; }
        public string Email { get; set; }
        public string NickName { get; set; }
        public byte UserLevel { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
