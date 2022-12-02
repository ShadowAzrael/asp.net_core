using Microsoft.AspNetCore.Mvc.Filters;
using System;

public class ResourceFilter : Attribute, IResourceFilter
{
    public void OnResourceExecuted(ResourceExecutedContext context)
    {
        // 执行完后的操作
    }

    public void OnResourceExecuting(ResourceExecutingContext context)
    {
        // 执行中的过滤器管道
    }
}