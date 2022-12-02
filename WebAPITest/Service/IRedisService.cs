using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPITest.Models.Database;

namespace WebAPITest.Service
{
    public interface IRedisService
    {
        /// <summary>
        /// 记录用户的浏览商品记录
        /// </summary>
        void AddGoodViews(int userId, Good good);

        /// <summary>
        /// 查询用户的商品浏览记录
        /// </summary>
        List<Good> GetUserViews(int userId);
    }
}
