using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebAPITest.Models;
using WebAPITest.Models.Database;
using WebAPITest.Service;

namespace WebAPITest.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IJWTService _jwtService;
        private readonly WebEnterpriseIIContext _db;
        private readonly IUserService _userService;
        /// <summary>
        /// 构造函数注入
        /// </summary>
        /// <param name="jwtService"></param>
        public AuthenticationController(IJWTService jwtService, WebEnterpriseIIContext db, IUserService userService)
        {
            _jwtService = jwtService;
            _db = db;
            //从构造函数 获取用户服务
            _userService = userService;
        }
        [HttpGet]
        public string CreateToken(string name)
        {
            //如果是登录
            //判断用户名和密码是否正确

            //如果正确才创建token 用这个用户名的用户信息去创建token
            return _jwtService.CreateToken(name, 0, "", "");
        }

        [HttpGet]
        [Authorize]//授权标识
        public void Test()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns> 
        [HttpGet]
        [Authorize]
        public object UserInfo()
        {
            //获取用户信息                    信息列表 查询第一条数据(查询条件)
            //数据来源于Token用户信息保存
            //return new
            //{
            //     Email = Response.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email).Value,
            //     UserName= Response.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name).Value,
            //     NickName = Response.HttpContext.User.Claims.FirstOrDefault(x => x.Type =="NickName").Value,
            //     UserId = Response.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value
            //};

            //通过userId查询数据库数据 
            var userId = Response.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
            var user = _db.Users.FirstOrDefault(x => x.UserId == Convert.ToInt32(userId));
            return user;

            //区别
            // 假如生成TOKEN之后 修改了数据库数据 修改了用户昵称 邮箱这些数据 
            // 从数据库里面查询出来的数据 TOKEN的数据是不一致的
            // 在从新登陆之前Token保存的用户数据不会变化 而数据库数据是实时变化的
        }

        /// <summary>
        /// 注册 把用户信息插入用户表 
        /// </summary>
        [HttpPost]
        public string Reg(RegRequest request)
        {
            if (_userService.ExistUserByUserName(request.UserName))
                return "用户存在";

            var userId = _userService.AddNewUser(request.UserName, request.Email, request.NickName, request.Password);
            if (userId > 0)
            {
                return "注册成功";
            }
            return "注册失败";
        }

        [HttpPost]
        public string Login(string username, string password)
        {
            //var user = _db.Users.FirstOrDefault(x=>x.UserName==username);

            //第一步 从数据库查询用户为username的用户 
            var user = _userService.GetUserByUserName(username);
            //如果不存在用户就不存在
            if (user == null)
                return "用户不存在";

            //如果存在 判断查询出来的用户密码是否 跟password相等
            if (user.Password != password)
            {
                //如果不相等 返回密码不正确
                return "密码不正确";
            }
            //如果相等呢？？？
            //创建Token 返回token给前端
            return _jwtService.CreateToken(user.UserName, user.UserId, user.Email, user.NickName);
        }
    }
}