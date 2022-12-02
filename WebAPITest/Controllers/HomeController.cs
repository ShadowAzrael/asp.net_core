using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPITest.Filter;
using WebAPITest.Service;

namespace WebAPITest.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ResultFilter]
    public class HomeController : ControllerBase
    {
        //定义服务变量
        private readonly IDataService _dataService;
        private readonly IScopedService _scopedService;
        private readonly ITransientService _transientService;
        private readonly ISingletonService _singletonService;

        /// <summary>
        /// 第一种：构造注入 从构造方法去取（IOC容器）服务
        /// </summary>
        /// <param name="dataService"></param>
        public HomeController(IDataService dataService, IScopedService scopedService, ITransientService transientService = null, ISingletonService singletonService = null)
        {
            _dataService = dataService;
            _scopedService = scopedService;
            _transientService = transientService;
            _singletonService = singletonService;
        }
        /// <summary>
        ///第二种 方法注入 从方法取
        /// </summary>
        /// <param name="">[FromServices] 服务类的接口 变量名</param>
        [HttpGet]
        public void Index([FromServices] IDataService dataService2, [FromServices] IScopedService scopedService2, [FromServices] ITransientService transientService)
        {
            //1：需要自己去实例化类 一旦类发生修改 就会影响很多的地方
            //DataService dataService = new DataService();
            //dataService.GetData();

            //2：依赖注入 吧new实例化 这一步交给IOC容器去完成
            _dataService.GetData();
            dataService2.GetHashCode();
            //这是从构造方法取得
            var s = _scopedService.GetHashCode();
            //这是从方法取得;
            var s1 = scopedService2.GetHashCode();
        }

        //在应用程序的整个生命周期 无论获取多少次 都是同一个
        [HttpGet]
        public int SingletonFun()
        {
            return _singletonService.GetHashCode();
        }

        //当我们每次从IOC容器获取对象时 都是不同的对象
        [HttpGet]
        public string TransientFun([FromServices] ITransientService transientService2)
        {
            var t1 = _transientService.GetHashCode();

            var t2 = transientService2.GetHashCode();

            return $"t1={t1},t2={t2}";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        //当我们同一个请求内 每次从IOC容器获取对象时 都是同一个，在不同的请求 会是不一样的对象
        [HttpGet]
        public string ScopedFun([FromServices] IScopedService scopedService2)
        {
            var s1 = _scopedService.GetHashCode();

            var s2 = scopedService2.GetHashCode();

            return $"s1={s1},s2={s2}";
        }


        [HttpGet]
        public void Test()
        {
            ClassA a = null;

            a.GetHashCode();
        }

        //需求：添加一个过滤器 来统一对消息文本进行判断，如果涉及不好的消息，直接返回错误提示
        /// <summary>
        /// 发送消息
        /// </summary>
        [HttpGet]
        public string SendMessage(string message, int memberId)
        {
            return "请求成功";
        }
        /// <summary>
        /// 发送评论
        /// </summary>
        [HttpGet]
        public string SendComment(string message, int vodId)
        {
            return "请求成功";
        }

        //cookie 记录在浏览器上面 携带用户信息给服务端 缺点：不安全

        //session 会话 aspx 跟服务端保持会话连接 缺点：耗服务器资源

        //jwt 授权 
        [HttpGet]
        //添加客户端缓存
        [ResponseCache(Duration = 20)]
        public DateTime Now()
        {
            return DateTime.Now;
        }
    }
}
