using Microsoft.AspNetCore.Mvc.Filters;

public class AuthorizeFilter : IAuthorizationFilter
{
    /// <summary>
    /// 请求验证，当前验证部分不要抛出异常，ExceptionFilter不会处理
    /// </summary>
    /// <param name="context"></param>
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        //这里可以做复杂的权限控制操作
        //if (context.HttpContext.User.Identity.Name != "1") //简单的做一个示范
        //{
        // //未通过验证则跳转到无权限提示页
        // RedirectToActionResult content = new RedirectToActionResult("NoAuth", "Exception", null);
        // context.Result = content;
        //
    }
}