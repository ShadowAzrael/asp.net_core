using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using StackExchange.Redis;
using WebAPITest.Models;
using WebAPITest.Models.Database;
using WebAPITest.Service;
using IDatabase = StackExchange.Redis.IDatabase;

namespace WebAPITest.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class RedisController : ControllerBase
    {
        private readonly IDatabase _redis;
        private readonly WebEnterpriseIIContext _db;
        private readonly IRedisService _redisService;

        public RedisController(RedisHelper client, WebEnterpriseIIContext db, IRedisService redisService)
        {
            _db = db;
            //创建Redis连接对象
            _redis = client.GetDatabase();
            _redisService = redisService;
        }

        [HttpGet]
        public void SetRedisCache()
        {
            //设置字符串类型缓存 key=RedisCache  Value="6666" 等同于命令 set RedisCache "6666"
            _redis.StringSet("RedisCache", "6666");
            //get RedisCache
            var value = _redis.StringGet("RedisCache");
        }

        [HttpGet]
        public void RedisString()
        {
            var str = "广东工程职业技术学院";
            //写入String的值
            _redis.StringSet("StringCache", str);
            //获取String缓存的值
            var value = _redis.StringGet("StringCache");

            //追加字符
            _redis.StringAppend("StringCache", "666");

            //写入数据 5小时5分钟5秒 后过期
            _redis.StringSet("StringCache1", 1, new TimeSpan(5, 5, 5));

            //key为StringCache1的值自增1
            _redis.StringIncrement("StringCache1");

            //key为StringCache1的值增加10
            _redis.StringIncrement("StringCache1", 10);

            //key为StringCache1的值自减1
            _redis.StringDecrement("StringCache1");

            //key为StringCache1的值减少5
            _redis.StringDecrement("StringCache1", 5);
        }

        [HttpGet]
        public void StringTest()
        {
            //添加key为goodName,value为"华为手机"的string类型数据
            _redis.StringSet("goodName", "华为手机");
            //获取key为goodName的数据x
            var value = _redis.StringGet("goodName");
            //把key为goodName的数据，value设置为"小米手机"，并且设置过期时间为20秒
            _redis.StringSet("goodName", "小米手机", new TimeSpan(0, 0, 20));
            //在key为goodName的值后面添加字符串"666"
            _redis.StringAppend("goodName", "666");
            //查看key为goodName的值长度
            _redis.StringLength("goodName");
        }

        [HttpGet]
        public void RedisHash()
        {
            var hashKey = "testhashkey";
            //hash写入单个数据
            _redis.HashSet(hashKey, "name", "刘备");

            //hash写入多个数据
            var hashEntrys = new HashEntry[] { new HashEntry("num", "1406200115"), new HashEntry("class", "软件A班"), new HashEntry("age", 18) };
            _redis.HashSet(hashKey, hashEntrys);

            //hash获取数据
            var value1 = _redis.HashGet(hashKey, "name");

            //hash获取所有数据
            var value2 = _redis.HashGetAll(hashKey);

            //判断hash的属性是否存在
            var value3 = _redis.HashExists(hashKey, "name");

            //hash的属性值自增，自减
            var value4 = _redis.HashIncrement(hashKey, "age");
            value4 = _redis.HashDecrement(hashKey, "age");

            //hash删除某个属性
            _redis.HashDelete(hashKey, "class");
        }

        [HttpGet]
        public void HashTest()
        {
            //把以上json数据使用hash的数据格式添加redis数据库中，key为"liubeiinfo"
            _redis.HashSet("liubeiinfo", "name", "刘备");
            _redis.HashSet("liubeiinfo", "class", "软件技术X班");
            _redis.HashSet("liubeiinfo", "school", "广东工程职业技术学院");
            _redis.HashSet("liubeiinfo", "age", "1862");
            _redis.HashSet("liubeiinfo", "num", "1406200228");

            //获取显示上述hash数据所有属性
            var data = _redis.HashGetAll("liubeiinfo");
            //修改学号num的值为"1406200230"
            _redis.HashSet("liubeiinfo", "num", "1406200230");

            //查询修改后学号的值
            var num = _redis.HashGet("liubeiinfo", "num");

        }

        [HttpGet]
        public void RedisList()
        {
            var listkey = "testlistkey1";
            //左侧推入单个元素
            _redis.ListLeftPush(listkey, "刘备");
            //左侧推入多个元素
            var redisValues1 = new RedisValue[] { "张飞", "关羽" };
            _redis.ListLeftPush(listkey, redisValues1);
            //右侧同理
            _redis.ListRightPush(listkey, "马超");
            var redisValues2 = new RedisValue[] { "赵云", "马岱" };
            _redis.ListRightPush(listkey, redisValues2);

            //获取所有元素
            _redis.ListRange(listkey, 0, -1);

            //获取指定索引的元素
            _redis.ListGetByIndex(listkey, 2);

            //从左边取出并一个并删除元素
            _redis.ListLeftPop(listkey);
            //从右边取出并一个并删除元素
            _redis.ListRightPop(listkey);
        }
        //[
        //    "刘备",//1
        //    "关羽",//2
        //    "张飞",//3
        //    "赵云",//4
        //    "马超",//5
        //    "曹操",//6
        //    "吕布" //7
        //]
        [HttpGet]
        public void ListTest()
        {
            //1234567
            //把以上json数据列表使用list数据格式以上述指定顺序添加到redis数据库中,key为"userList"
            //7 6 5 4 3 2 1
            //1 2 3 4 5 6 7
            //var redisValues1 = new RedisValue[] { "吕布", "关羽", "张飞", "赵云", "马超", "曹操", "刘备" };
            //_redis.ListLeftPush(listkey, "刘备");
            //1 2 3 4 5 6 7 
            var redisValues1 = new RedisValue[] { "刘备", "关羽", "张飞", "赵云", "马超", "曹操", "吕布" };
            _redis.ListRightPush("userList", redisValues1);
            //查看目前列表数据并核对顺序
            var list = _redis.ListRange("userList", 0, -1);
            //再赵云和马超中间加入"黄盖"
            _redis.ListInsertBefore("userList", "马超", "黄盖");
            //查看目前列表数据
            list = _redis.ListRange("userList", 0, -1);
        }

        [HttpGet]
        public void RedisSet()
        {
            var setkey = "testsetkey1";
            //添加单个元素
            _redis.SetAdd(setkey, "刘备");
            //添加多个元素
            var redisValues1 = new RedisValue[] { "张飞", "关羽", "周瑜" };
            _redis.SetAdd(setkey, redisValues1);
            //移除某个元素
            _redis.SetRemove(setkey, "刘备");

            //获取所有成员
            var value1 = _redis.SetMembers(setkey);
            //随机获取一个元素
            var value2 = _redis.SetRandomMember(setkey);
            //随机获取多个元素
            var value3 = _redis.SetRandomMembers(setkey, 2);
            //判断是否包含某个元素
            _redis.SetContains(setkey, "马超");

            var setkey2 = "testsetkey2";
            var redisValues2 = new RedisValue[] { "张飞", "关羽", "马超", "赵云", "曹操", "黄盖" };
            //_redis.SetAdd(setkey2, redisValues2);
            //获取两个集合的交集
            var value5 = _redis.SetCombine(SetOperation.Intersect, setkey, setkey2);
            //获取两个集合的差集
            var value4 = _redis.SetCombine(SetOperation.Difference, setkey, setkey2);
            //获取两个集合的并集
            var value6 = _redis.SetCombine(SetOperation.Union, setkey, setkey2);
        }

        ////以下为刘备同学关注的用户Id列表
        //[
        //    1214,
        //    5651,
        //    5654,
        //    2668,
        //    9595,
        //    9955,
        //]
        ////以下为张飞同学关注的用户Id列表
        //[
        //    5985,
        //    5651,
        //    5654,
        //    6555,
        //    9595,
        //    9998,
        //]
        [HttpGet]
        public void SetTest()
        {
            //把刘备同学关注的用户Id列表使用Set数据类型添加到Redis数据库中，设置key为"followList_liu"
            var redisValues1 = new RedisValue[] {
                        1214,
                        5651,
                        5654,
                        2668,
                        9595,
                        9955,
            };
            _redis.SetAdd("followList_liu", redisValues1);
            //把张飞同学关注的用户Id列表使用Set数据类型添加到Redis数据库中，设置key为"followList_zhang"
            var redisValues2 = new RedisValue[] {
                5985,
                5651,
                5654,
                6555,
                9595,
                9998,
            };
            _redis.SetAdd("followList_zhang", redisValues2);
            //使用set类型数据命令获取刘备同学和张飞同学共同关注的人 集合的交集
            var list = _redis.SetCombine(SetOperation.Intersect, "followList_liu", "followList_zhang");
        }

        [HttpGet]
        public void RedisZSet()
        {
            var zsetkey = "testzsetkey1";
            //添加单个元素
            _redis.SortedSetAdd(zsetkey, "刘备", 50);
            //添加多个元素
            var values = new[]
                { new SortedSetEntry("马超", 23), new SortedSetEntry("赵云", 50), new SortedSetEntry("关羽", 56) };
            _redis.SortedSetAdd(zsetkey, values);
            //移除某个元素
            _redis.SortedSetRemove(zsetkey, "马超");

            //给某个元素添加指定分数
            _redis.SortedSetIncrement(zsetkey, "马超", 1);
            //给某个元素减少指定分数
            _redis.SortedSetDecrement(zsetkey, "马超", 1);

            //获取某个分数段的元素
            var value1 = _redis.SortedSetRangeByScore(zsetkey, 50, 60);
            //获取某个分数段的元素及分数
            var value2 = _redis.SortedSetRangeByScoreWithScores(zsetkey, 50, 60);
            //获取某个排名段的元素
            var value3 = _redis.SortedSetRangeByRank(zsetkey, 0, 2);
            //获取某个排名段的元素及分数
            var value4 = _redis.SortedSetRangeByRankWithScores(zsetkey, 0, 2);
        }
        //[
        //  {
        //        "name":"刘备",
        //        "score":90
        //    },
        //    {
        //        "name":"张飞",
        //        "score":44
        //    },
        //    {
        //       "name":"关羽",
        //        "score":55
        //    },
        //    {
        //    "name":"赵云",
        //        "score":98
        //    },
        //    {
        //    "name":"马超",
        //        "score":41
        //    },
        //    {
        //    "name":"曹操",
        //        "score":91
        //    },
        //    {
        //    "name":"吕布",
        //        "score":55
        //    },
        //    {
        //    "name":"周瑜",
        //        "score":55
        //    },
        //]
        [HttpGet]
        public void ZSetTest()
        {
            var values = new[]
            {
                new SortedSetEntry("刘备", 90),
                new SortedSetEntry("张飞", 44),
                new SortedSetEntry("关羽", 55),
                new SortedSetEntry("赵云", 98),
                new SortedSetEntry("马超", 41),
                new SortedSetEntry("曹操", 91),
                new SortedSetEntry("吕布", 55),
                new SortedSetEntry("周瑜", 55),
            };
            //以上数据是该班级各同学的考试成绩，使用Zset数据类型添加到Redis数据库中，设置key为 "scorelist"
            _redis.SortedSetAdd("scorelist", values);
            //查询分数高的排名为第2 - 5名成绩的同学名单
            _redis.SortedSetRangeByRank("scorelist", 1, 4);
            //查询60分到100分的同学名单
            _redis.SortedSetRangeByScore("scorelist", 60, 100);
        }

        /// <summary>
        /// 
        /// </summary>
        [HttpGet]
        public void SerializeJSON()
        {
            //查询商品
            var good = _db.Goods.FirstOrDefault(x => x.Id == 10);

            //转化为json字符串 序列化
            var jsonStr = JsonConvert.SerializeObject(good);

            //json字符串保存到redis
            _redis.StringSet("goodinfo", jsonStr);
            //从redis取出json字符串
            var redisStr = _redis.StringGet("goodinfo");

            //转化为对象 反序列化
            var jsonObj = JsonConvert.DeserializeObject<Good>(redisStr);
        }

        /// <summary>
        /// 获取商品列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public List<Good> GetGoodList()
        {
            //完成！！！ 使用本地缓存保存查询结果，缓存时间为30分钟
            //使用redis完成

            var cache = _redis.StringGet("GoodCache");
            //1、先查看缓存有没有商品数据 key     变量类型和变量名
            if (cache.HasValue) //true
            {
                //字符串转C# 列表对象
                var goodList = JsonConvert.DeserializeObject<List<Good>>(cache);
                //3、如果有则直接返回缓存数据
                return goodList;
            }
            else //没有缓存
            {
                //2、如果没有则去数据库查询并且把查询结果保存到缓存 缓存时间为30分钟
                var goods = _db.Goods.ToList();

                //转化为json字符串 序列化
                var jsonStr = JsonConvert.SerializeObject(goods);
                //存进去Redis
                _redis.StringSet("GoodCache", jsonStr, new TimeSpan(0, 30, 0));
                //等于
                //cache.Set("GoodCache", goods, TimeSpan.FromMinutes(30));

                return goods;
            }
        }

        /// <summary>
        /// 获取商品详情
        /// </summary>
        /// <param name="goodId"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize] //授权
        public Good GetGoodInfo(int goodId)
        {
            //商品详情
            var good = _db.Goods.FirstOrDefault(x => x.Id == goodId);

            //使用本地缓存保存登录用户浏览商品记录(多个商品)
            var userId = Response.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;

            //使用redis取代本地缓存
            ////之前缓存下来的浏览记录  每个用户浏览记录都是单独的 每个用户都有自己的key  UserViews28\UserViews20
            // 
            var key = $"UserViews{userId}";

            //取之前的浏览记录
            //var record = cache.Get<List<Good>>(key);
            //等于
            //redis 
            var cache = _redis.StringGet(key);

            ////浏览商品记录
            var views = new List<Good>();
            views.Add(good); //永远只有刚刚看的这一条商品数据

            //是不是还应该有前面的数据
            if (cache.HasValue)
            {
                var record = JsonConvert.DeserializeObject<List<Good>>(cache);//反序列化
                views.AddRange(record); //我前面看过的数据
            }

            //cache.Set($"UserViews{userId}", views);
            //等于 先序列化成json字符串 再存进redis
            var jsonStr = JsonConvert.SerializeObject(views);//序列化
            _redis.StringSet(key, jsonStr);

            return good;
        }

        //List 17:00  18:00
        //zset 唯一的 18：00

        //Redis5种数据类型 是否有更好类型处理这些场景 STRING HASH SET LIST ZSET
        //商品浏览记录数据 商品 先后顺序  LIST ZSET
        /// <summary>
        /// 获取商品详情 使用List
        /// </summary>
        /// <param name="goodId"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize] //授权
        public Good GetGoodInfo1(int goodId)
        {
            //获取商品详情接口
            //两个作用
            //1、获取商品详情返回给前端显示
            //商品详情
            var good = _db.Goods.FirstOrDefault(x => x.Id == goodId);
            //2、顺带记录用户的浏览商品记录

            //使用本地缓存保存登录用户浏览商品记录(多个商品)
            var userId = Response.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;

            _redisService.AddGoodViews(Convert.ToInt32(userId), good);
            return good;
        }
        /// <summary>
        /// 获取商品详情 使用ZSet 分数 
        /// </summary>
        /// <param name="goodId"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize] //授权
        public Good GetGoodInfo2(int goodId)
        {
            //商品详情
            var good = _db.Goods.FirstOrDefault(x => x.Id == goodId);

            //使用本地缓存保存登录用户浏览商品记录(多个商品)
            var userId = Response.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;

            //使用redis取代本地缓存
            ////之前缓存下来的浏览记录  每个用户浏览记录都是单独的 每个用户都有自己的key  UserViews28\UserViews20
            var key = $"UserViews{userId}";

            //记录商品浏览时间 把分数和时间关联
            //在用户浏览商品的时候 添加商品浏览记录 商品 
            //新数据在左边还是右边
            //最近浏览的商品
            //新 旧

            //浏览商品时间的时间戳 1970年1月1日 0时0分0秒(北京时间要加8个小时) 到现在的时间 就是时间戳
            var score = (DateTime.Now - new DateTime(1970, 1, 1, 8, 0, 0)).TotalSeconds;

            var jsonStr = JsonConvert.SerializeObject(good);//序列化
            _redis.SortedSetAdd(key, jsonStr, score);// 1 1 1

            //以浏览时间为分数的 商品数据 
            //1、获取到最新的浏览商品记录
            //2、获取到商品浏览时间

            //数据类型的使用实战技巧
            return good;
        }

        //获取商品浏览记录 获取最新的浏览商品记录 上面是最新的
        [HttpGet]
        public void List()
        {
            //长度-获取数量 ， 长度 分页
            _redis.ListRange("", 0, 4);
        }

        [HttpGet]
        [Authorize]//授权
        public string AddCar(int goodId)
        {
            //使用本地缓存保存登录用户浏览商品记录(多个商品)
            var userId = Response.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
            var hashKey = "car_" + userId;
            //对购物车数量增加 如果
            _redis.HashIncrement(hashKey, goodId);
            return "添加购物车成功";
        }
        [HttpGet]
        [Authorize]//授权
        public string DiffCar(int goodId)
        {
            //使用本地缓存保存登录用户浏览商品记录(多个商品)
            var userId = Response.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
            var hashKey = "car_" + userId;
            //对购物车数量减少
            var count = _redis.HashDecrement(hashKey, goodId);
            //如果数量==0时 删除商品
            if (count == 0)
            {
                _redis.HashDelete(hashKey, goodId);
            }
            return "减少购物车成功";
        }
        /// <summary>
        /// 删除商品
        /// </summary>
        /// <param name="goodId"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]//授权
        public string DeleteCar(int goodId)
        {
            //使用本地缓存保存登录用户浏览商品记录(多个商品)
            var userId = Response.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
            var hashKey = "car_" + userId;
            _redis.HashDelete(hashKey, goodId);
            return "删除购物车商品成功";
        }
        /// <summary>
        /// 查询商品
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]//授权
        public List<GetCarModel> GetCar()
        {
            //使用本地缓存保存登录用户浏览商品记录(多个商品)
            var userId = Response.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
            var hashKey = "car_" + userId;
            //查询所有hash数据
            var data = _redis.HashGetAll(hashKey);
            var list = new List<GetCarModel>();
            foreach (var item in data)
            {
                //从数据库查询数据
                var good = _db.Goods.FirstOrDefault(x => x.Id == (int)item.Name);
                if (good != null)
                {
                    //添加到list
                    list.Add(new GetCarModel
                    {
                        GoodId = (int)item.Name,
                        Count = (int)item.Value,
                        Cover = good.Cover,
                        Name = good.Name,
                        Price = good.Price
                    });
                }
            }
            return list;
        }
        [HttpGet]
        public string AddKeyword(string keyword)
        {
            var key = "hotkey";
            //给某个元素添加指定分数
            _redis.SortedSetIncrement(key, keyword, 1);
            _redis.SortedSetIncrement(key + DateTime.Now.ToString("yyyy-MM-dd"), keyword, 1);
            return "添加成功";
        }
        /// <summary>
        /// 查询前10名热搜结果及搜索次数
        /// </summary>
        /// <returns></returns>
        public List<GetKeywordModel> GetKeyword()
        {
            var key = "hotkey";
            //查询前10名热搜结果及搜索次数
            var data = _redis.SortedSetRangeByRankWithScores(key, 0, 9, Order.Descending);
            var list = new List<GetKeywordModel>();
            foreach (var item in data)
            {
                list.Add(new GetKeywordModel()
                {
                    Keyword = item.Element,
                    Count = item.Score
                });
            }
            return list;
        }
        /// <summary>
        /// 查询前10名热搜结果及搜索次数 日热搜榜
        /// </summary>
        /// <returns></returns>
        public List<GetKeywordModel> GetKeywordByDate(string date)
        {
            var key = "hotkey";
            //查询前10名热搜结果及搜索次数
            var data = _redis.SortedSetRangeByRankWithScores(key + date, 0, 9, Order.Descending);
            var list = new List<GetKeywordModel>();
            foreach (var item in data)
            {
                list.Add(new GetKeywordModel()
                {
                    Keyword = item.Element,
                    Count = item.Score
                });
            }
            return list;
        }

        //关注用户和取消关注用户
        [HttpGet]
        [Authorize]
        public string FollowUser(int followUserId)
        {
            var userId = Response.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
            var key = "follow_" + userId;
            _redis.SetAdd(key, followUserId);
            return "关注成功";
        }
        [HttpGet]
        [Authorize]
        public string UnFollowUser(int followUserId)
        {
            var userId = Response.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
            var key = "follow_" + userId;
            _redis.SetRemove(key, followUserId);
            return "取消关注成功";
        }
        //查看某个用户关注的人
        [HttpGet]
        public List<int> GetFollowUserList(int userId)
        {
            var key = "follow_" + userId;
            var data = _redis.SetMembers(key);
            return data.Select(x => (int)x).ToList();
        }
        //获取两个用户同时关注的人
        //查看某个用户关注的人
        [HttpGet]
        public List<int> GetFollowUserListIntersect(int userId1, int userId2)
        {
            var key = "follow_";
            //求交集
            var data = _redis.SetCombine(SetOperation.Intersect, key + userId1, key + userId2);
            return data.Select(x => (int)x).ToList();
        }
        //获取两个用户关注的用户总和
        [HttpGet]
        public List<int> GetFollowUserListUnion(int userId1, int userId2)
        {
            var key = "follow_";
            //求并集
            var data = _redis.SetCombine(SetOperation.Union, key + userId1, key + userId2);
            return data.Select(x => (int)x).ToList();
        }
        //当A进入B页面,求可能认识的人：这里指的是关注B中的用户 扣去 里面也关注A的用户，就是A可能认识的人。
        [HttpGet]
        public List<int> GetFollowUserListDifference(int userId1, int userId2)
        {
            var key = "follow_";
            //求差集
            var data = _redis.SetCombine(SetOperation.Difference, key + userId1, key + userId2);
            return data.Select(x => (int)x).ToList();
        }
    }
}
