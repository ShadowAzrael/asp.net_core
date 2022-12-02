using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPITest.Models.Database;

namespace WebAPITest.Service
{
    public interface IUserService
    {
        /// <summary>
        /// 根据用户名 从数据库查询用户
        /// </summary>
        User GetUserByUserName(string userName);
        /// </summary>
        /// <summary>
        /// 根据用户名 判断用户是否存在
        /// </summary>
        /// <param name="userName"></param>
        /// <returns>是否存在</returns>
        bool ExistUserByUserName(string userName);

        /// <summary>
        /// 插入新用户数据
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="email">邮件</param>
        /// <param name="nickname"></param>
        /// <param name="password"></param>
        /// <returns>新增用户Id</returns>
        int AddNewUser(string userName, string email, string nickname, string password);
    }
}
