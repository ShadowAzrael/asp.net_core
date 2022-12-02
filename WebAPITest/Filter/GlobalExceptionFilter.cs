using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPITest.Filter
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<GlobalExceptionFilter> _logger;
        /// <summary>
        /// 构造方法
        /// </summary>
        public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger = null)
        {
            _logger = logger;
        }
        /// <summary>
        /// 当程序出现异常的时候 进入这个方法
        /// </summary>
        /// <param name="context"></param>
        public void OnException(ExceptionContext context)
        {
            //处理程序中出现异常
            var operation = context.HttpContext.Request.RouteValues["controller"] + "/" + context.HttpContext.Request.RouteValues["action"];

            _logger.LogError(context.Exception, $"{operation}  Exception:" + context.Exception.Message); //记录错误日志

            //拦截处理
            if (!context.ExceptionHandled)
            {
                //通过 数据方式返回异常信息
                context.Result = new JsonResult(new
                {
                    status = false,
                    msg = "系统内部错误:" + context.Exception.Message
                });
                context.ExceptionHandled = true;
            }

        }
    }
}