using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MVC_Demo.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVC_Demo
{
    public class Startup
    {
        //为程序注入服务 
        public void ConfigureServices(IServiceCollection services)
        {
            //添加授权规则为cookie 当未登录时跳转到Login.html
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(
                options => { options.LoginPath = new PathString("/Login.html"); } //设置登录页面为/Auth/Signin
            );

            //注入MVC服务
            services.AddControllersWithViews();

            //注入数据库服务 第一步注入
            services.AddDbContext<McStoreContext>();
        }

        //处理HTTP管道 为程序注入中间件
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            //启用身份验证，加上这句才能生效，
            //注意顺序，在 app.UseRouting();之后
            //开启认证中间件
            app.UseAuthentication();
            //开启授权中间件
            app.UseAuthorization();

            //使用静态文件
            app.UseStaticFiles();

            //常规路由
            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapGet("/", async context =>
                //{
                //    await context.Response.WriteAsync("Hello World!");
                //});s
                //当不填 /Home/Index
                endpoints.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");

                // /Home/Index.html
                endpoints.MapControllerRoute(name: "html", pattern: "{controller=Home}/{action=Index}.html");

                endpoints.MapControllerRoute(name: "soft", pattern: "soft/{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
