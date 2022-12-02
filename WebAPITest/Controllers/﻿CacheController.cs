using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using WebAPITest.Models.Database;

namespace WebAPITest.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CacheController : ControllerBase
    {
        private readonly IMemoryCache cache;
        private readonly WebEnterpriseIIContext _db;
        /// <summary>
        /// 构造注入 从IOC容器取出 缓存服务
        /// </summary>
        /// <param name="cache"></param>
        public CacheController(IMemoryCache cache, WebEnterpriseIIContext db)
        {
            this.cache = cache;
            _db = db;
            //1:key / value
            //2:key是字符串 缓存数据的名字
            //3：value 可以是任何的类型 string int byte list object class
        }

        /// <summary>
        /// 判断缓存是否存在
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<string> TryGetValueCache()
        {
            //方法名：TryGetValue 两个参数 1：key 2：可返回的变量 
            if (cache.TryGetValue("UserName", out string username))
            {
                return "该缓存存在" + username;
            }
            else
            {
                return "该缓存不存在";
            }
        }
        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<string> SetCache()
        {
            return cache.Set("UserName", "liudacheng");
        }

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<string> GetCache()
        {
            return cache.Get<string>("UserName");
        }

        [HttpGet]
        public ActionResult<string> RemoveCache()
        {
            cache.Remove("UserName");
            return "删除缓存成功";
        }

        /// <summary>
        /// 永不过期
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<string> CacheTime1()
        {
            string msg = string.Empty;
            //设置缓存 永不过期
            cache.Set("UserName", "admin");
            // 读取缓存
            for (int i = 1; i <= 5; i++)
            {
                //每一次循环读取一次缓存
                msg += $"第{i}秒缓存值：{cache.Get<string>("UserName")}\n";
                //睡眠一秒
                Thread.Sleep(1000);
            }

            // 返回
            return msg;
        }

        /// <summary>
        /// 绝对时间过期
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<string> CacheTime2()
        {
            string msg = string.Empty;
            //设置缓存 指定3秒后过期
            cache.Set("UserName", "admin", TimeSpan.FromSeconds(3));

            // 读取缓存
            for (int i = 1; i <= 5; i++)
            {
                msg += $"第{i}秒缓存值：{cache.Get<string>("UserName")}\n";
                Thread.Sleep(1000);
            }

            // 返回
            return msg;
        }

        /// <summary>
        /// 滑动时间过期
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<string> CacheTime3()
        {
            string msg = string.Empty;
            //设置缓存 并且使以滑动时间过期 3秒
            cache.Set("UserName", "admin", new MemoryCacheEntryOptions
            {
                //滑动时间
                //在3秒内 如果有人访问，自动再次变成3秒 如果在1.5秒访问缓存 那么缓存剩余时间又变成3秒
                //如果没人访问 删除缓存
                SlidingExpiration = TimeSpan.FromSeconds(0.5)
            });

            // 读取缓存
            for (int i = 1; i <= 5; i++)
            {

                msg += $"第{i}秒缓存值：{cache.Get<string>("UserName")}\n";
                Thread.Sleep(1000);
            }

            // 返回
            return msg;
        }

        /// <summary>
        /// 绝对时间过期+滑动时间过期
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<string> Get()
        {
            string msg = string.Empty;
            //两种策略一起的话， 其中一种触发 其他的就会失效
            cache.Set("UserName", "admin", new MemoryCacheEntryOptions
            {
                //滑动时间
                SlidingExpiration = TimeSpan.FromSeconds(0.5),
                //绝对时间 3秒后 必定过期
                AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(3)
            });

            // 读取缓存
            for (int i = 1; i <= 5; i++)
            {
                msg += $"第{i}秒缓存值：{cache.Get<string>("UserName")}\n";
                Thread.Sleep(1000);
            }

            // 返回
            return msg;
        }
        /// <summary>
        /// 获取商品列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public List<Good> GetGoodList()
        {
            //完成！！！ 使用本地缓存保存查询结果，缓存时间为30分钟
            //1、先查看缓存有没有商品数据 key     变量类型和变量名
            if (cache.TryGetValue("GoodCache", out List<Good> goodList))
            {
                //3、如果有则直接返回缓存数据
                return goodList;
            }
            else
            {
                //2、如果没有则去数据库查询并且把查询结果保存到缓存 缓存时间为30分钟
                var goods = _db.Goods.ToList();
                cache.Set("GoodCache", goods, TimeSpan.FromMinutes(30));
                return goods;
            }
        }

        /// <summary>
        /// 获取商品详情
        /// </summary>
        /// <param name="goodId"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public Good GetGoodInfo(int goodId)
        {
            //商品详情
            var good = _db.Goods.FirstOrDefault(x => x.Id == goodId);

            //使用本地缓存保存登录用户浏览商品记录(多个商品)
            var userId = Response.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
            //之前缓存下来的浏览记录  每个用户浏览记录都是单独的 每个用户都有自己的key  UserViews28\UserViews20
            var record = cache.Get<List<Good>>($"UserViews{userId}");
            //浏览商品记录
            var views = new List<Good>();
            views.Add(good); //永远只有刚刚看的这一条商品数据

            //是不是还应该有前面的数据
            if (record != null)
                views.AddRange(record); //我前面看过的数据

            cache.Set($"UserViews{userId}", views);
            return good;
        }

        //对象的序列化 和 反序列化
    }
}
