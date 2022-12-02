using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using MVC_Demo.Models;
using MVC_Demo.Models.Database;
using System.Linq;

namespace WebApi_Demo.Controllers
{

    [Route("api/[controller]/[action]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        //由于浏览器的同源政策
        //限制不同源的客户端 默认不能请求服务端
        //当然服务器端可以进行解除

        //定义数据库上下文
        private readonly McStoreContext _db;

        /// <summary>
        /// 构造方法 构造注入
        /// </summary>
        public HomeController(McStoreContext db)
        {
            _db = db;
        }
        /// 注册账号 <summary>
        /// 注册账号
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public IActionResult RegisterAccount(RegisterAccountRequest request)
        {
            //判断用户名是否存在
            var result = _db.Users.Any(x => x.UserName == request.UserName);
            if (result)
                return Content("账号已存在");

            //添加数据
            var data = new User
            {
                CraeteTime = DateTime.Now,
                Password = request.Password,
                /*Photo = String.Empty,*/
                Photo = "img/xie.jpg",
                UserName = request.UserName
            };
            _db.Users.Add(data);
            var row = _db.SaveChanges();
            if (row > 0)
                return Content("注册成功");
            return Content("注册失败");
        }

    }
}
