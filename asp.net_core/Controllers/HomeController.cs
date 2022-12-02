using Microsoft.AspNetCore.Mvc;
using MVC_Demo.Models;
using MVC_Demo.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace MVC_Demo.Controllers
{
    public class HomeController : Controller
    {
        //定义数据库上下文
        private readonly McStoreContext _db;

        /// <summary>
        /// 构造方法 构造注入
        /// </summary>
        public HomeController(McStoreContext db)
        {
            _db = db;
        }

        [Route("/")]
        [Route("/Index.html")]
        public IActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cId">默认为0代表全部 >0代表需要查询的分类</param>
        /// <returns></returns>
        [Route("/Market.html")]
        public IActionResult Market(int cid = 0)
        {
            //1.从数据库查询数据（数据库操作）
            //上下文.表名
            var list = _db.Categories.ToList();

            var goods = _db.Goods
                .OrderByDescending(x => x.Price)
                .ToList();
            //如果CId大于0 代表需要查询分类
            if (cid > 0)
            {
                //将筛选后的数据赋值给goods
                goods = goods.Where(x => x.CateId == cid).ToList();
            }

            //2.把数据从控制器方法传递到页面（参数传递(ViewData\ViewBag\ViewModel)）
            ViewData["category"] = list;
            ViewData["goods"] = goods;
            ViewData["cid"] = cid;

            return View();
        }

        [Route("/About.html")]
        public IActionResult About()
        {
            return View();
        }

        [Route("/Detail.html")]
        public IActionResult Detail(int id)
        {
            var good = _db.Goods.FirstOrDefault(x => x.Id == id);
            ViewData["good"] = good;

            var userId = Convert.ToInt32(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value);
            ViewData["User"] = _db.Users.FirstOrDefault(x => x.UserId == userId);
            return View();
        }

        [Route("/UserInfo.html")]
        public IActionResult UserInfo()
        {
            //获取授权中的用户Id
            var userId = Convert.ToInt32(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value);
            //在数据库查询用户数据
            var user = _db.Users.FirstOrDefault(x => x.UserId == userId);
            ViewData["User"] = user;
            //获取用户购物车
            /*var car = _db.Cars.OrderByDescending(x => x.UserId == userId).ToList();*/

            //匿名类型 自己创建一个类型
            ViewData["CarGood"] = _db.Cars.Join(_db.Goods, x => x.GoodId, y => y.Id, (x, y) => new CarGood
            {
                GoodId = y.Id,
                GoodName = y.Name,
                Count = x.Count,
                Cover = y.Cover,
                Price = y.Price,
                UserId = x.UserId.ToString()
            }).Where(x => x.UserId == userId.ToString()).ToList();

            return View();
        }

        [Route("/Register.html")]
        public IActionResult Register()
        {
            return View();
        }

        [Route("/Login.html")]
        public IActionResult Login()
        {
            return View();
        }

        /// 注册账号
        [HttpPost]
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

        //登录账号
        [HttpPost]
        public IActionResult LoginAccount(LoginAccountRequest request)
        {
            //获取表单数据
            var userName = (string)HttpContext.Request.Form["username"];
            var password = (string)HttpContext.Request.Form["password"];
            //数据库查询账号密码是否存在
            var user = _db.Users.FirstOrDefault(x => x.UserName == userName);
            if (user == null)
                return Content("用户不存在");
            if (user.Password != password)
                return Content("密码错误");

            //存在的话 走登录流程
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier,user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Thumbprint, user.Photo),
            };
            HttpContext.SignInAsync(new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)));
            return Content("登录成功");
        }

        //修改密码
        [HttpPost]
        public IActionResult Changepassword()
        {
            //获取表单数据
            var userName = (string)HttpContext.Request.Form["username"];
            var password = (string)HttpContext.Request.Form["password"];
            var password_new = (string)HttpContext.Request.Form["password_new"];
            //数据库查询账号密码是否存在
            var user = _db.Users.FirstOrDefault(x => x.UserName == userName);
            if (user.Password != password) return Content("原密码错误");

            //修改密码
            user.Password = password_new;
            _db.Users.Update(user);
            if (_db.SaveChanges() > 0) return Content("修改成功");
            else return Content("修改失败");

        }

        //添加购物车
        [HttpPost]
        public IActionResult Addcart()
        {
            //获取表单数据
            var goodid = (string)HttpContext.Request.Form["GoodId"];
            var userid = (string)HttpContext.Request.Form["UserId"];
            //查询数据库是否已添加该商品
            var good = _db.Cars.FirstOrDefault(x => x.GoodId.ToString() == goodid && x.UserId.ToString() == userid);
            //判断购物车是否已有该商品
            if (good == null)
            {
                var data = new Car
                {
                    GoodId = int.Parse(goodid),
                    UserId = int.Parse(userid),
                    Count = 1,
                    CreateDate = DateTime.Now
                };
                _db.Cars.Add(data);
                if (_db.SaveChanges() <= 0) return Content("添加失败");
            }
            else
            {
                good.Count = good.Count + 1;
                good.CreateDate = DateTime.Now;
                _db.Update(good);
                if (_db.SaveChanges() <= 0) return Content("添加失败");
            }
            return Content("添加成功");
        }

        //修改购物车
        [HttpPost]
        public IActionResult CarChanges(CarGood carGood)
        {
            //查询对应商品
            var carchanges = _db.Cars.FirstOrDefault(x => x.GoodId == carGood.GoodId && x.UserId == int.Parse(carGood.UserId));

            //执行数量增减
            if (carGood.action == "+") carchanges.Count++;
            else if (carGood.action == "-") carchanges.Count--;

            //修改对应购物城商品数量，若为0则删除该条目
            if (carchanges.Count != 0) _db.Update(carchanges);
            else _db.Cars.Remove(carchanges);
            if (_db.SaveChanges() <= 0) return Content("添加失败");
            else return Content("修改成功");
        }

        [HttpGet]
        public IActionResult LoginOut()
        {
            HttpContext.SignOutAsync();
            return RedirectToAction("Index");
        }

    }
}
