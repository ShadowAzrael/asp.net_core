using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPITest.Models;
using WebAPITest.Models.Database;

namespace WebAPITest.Service
{
    /// <summary>
    /// 用户领域相关
    /// </summary>
    public class UserService : IUserService
    {
        private readonly WebEnterpriseIIContext _db;
        /// <summary>
        /// 构造函数
        /// </summary>
        public UserService(WebEnterpriseIIContext db)
        {
            _db = db;
        }

        /// <summary>
        /// 根据用户名 从数据库查询用户
        /// </summary>
        public User GetUserByUserName(string userName)
        {
            var user = _db.Users.FirstOrDefault(x => x.UserName == userName);
            return user;
        }

        /// <summary>
        /// 根据用户名 判断用户是否存在
        /// </summary>
        public bool ExistUserByUserName(string userName)
        {
            return _db.Users.Any(x => x.UserName == userName);
        }

        //领域服务方法       接口
        //1、传入参数        1、传入参数
        //2、方法名          2、接口名
        //3、处理数据        3、处理逻辑
        //4、返回数据        4、返回数据

        /// <summary>
        /// 插入新用户数据
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="email"></param>
        /// <param name="nickname"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public int AddNewUser(string userName, string email, string nickname, string password)
        {
            var user = new User()
            {
                UserName = userName,
                Email = email,
                NickName = nickname,
                Password = password,
                CreateTime = DateTime.Now,
                Salt = 0,
                UserLevel = 1,
            };
            //上下文.表名.操作方法(需要插入的数据对象)
            _db.Users.Add(user);
            //影响数量
            var row = _db.SaveChanges();
            if (row > 0)
                return user.UserId;
            return 0;
        }
    }
}
