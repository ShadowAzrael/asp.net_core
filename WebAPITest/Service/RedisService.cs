using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPITest.Models;
using WebAPITest.Models.Database;
using IDatabase = StackExchange.Redis.IDatabase;

namespace WebAPITest.Service
{
    public class RedisService : IRedisService
    {
        private readonly IDatabase _redis;
        /// <summary>
        /// 构造注入
        /// </summary>
        /// <param name="client"></param>
        public RedisService(RedisHelper client)
        {
            //创建Redis连接对象
            _redis = client.GetDatabase();
        }

        /// <summary>
        /// 记录用户的浏览商品记录
        /// </summary>
        public void AddGoodViews(int userId, Good good)
        {
            //使用redis取代本地缓存
            ////之前缓存下来的浏览记录  每个用户浏览记录都是单独的 每个用户都有自己的key  UserViews28\UserViews20
            var key = $"UserViews{userId}";

            //在用户浏览商品的时候 添加商品浏览记录 商品
            //新数据在左边还是右边
            //最近浏览的商品
            //新 旧

            var jsonStr = JsonConvert.SerializeObject(good);//序列化
            _redis.ListLeftPush(key, jsonStr);// 1 1 1
        }

        /// <summary>
        /// 查询用户的商品浏览记录
        /// </summary>
        public List<Good> GetUserViews(int userId)
        {
            var key = $"UserViews{userId}";
            var data = _redis.ListRange(key, 0, 4);
            var list = new List<Good>();
            foreach (var item in data)
            {
                var good = JsonConvert.DeserializeObject<Good>(item);
                list.Add(good);
            }
            return list;
        }

    }
}