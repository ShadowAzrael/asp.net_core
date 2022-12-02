using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPITest.Models
{
    /// <summary>
    /// DTO  数据转换对象
    /// </summary>
    public class RegRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string NickName { get; set; }
    }
}