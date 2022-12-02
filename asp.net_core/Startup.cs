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
        //Ϊ����ע����� 
        public void ConfigureServices(IServiceCollection services)
        {
            //�����Ȩ����Ϊcookie ��δ��¼ʱ��ת��Login.html
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(
                options => { options.LoginPath = new PathString("/Login.html"); } //���õ�¼ҳ��Ϊ/Auth/Signin
            );

            //ע��MVC����
            services.AddControllersWithViews();

            //ע�����ݿ���� ��һ��ע��
            services.AddDbContext<McStoreContext>();
        }

        //����HTTP�ܵ� Ϊ����ע���м��
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            //���������֤��������������Ч��
            //ע��˳���� app.UseRouting();֮��
            //������֤�м��
            app.UseAuthentication();
            //������Ȩ�м��
            app.UseAuthorization();

            //ʹ�þ�̬�ļ�
            app.UseStaticFiles();

            //����·��
            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapGet("/", async context =>
                //{
                //    await context.Response.WriteAsync("Hello World!");
                //});s
                //������ /Home/Index
                endpoints.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");

                // /Home/Index.html
                endpoints.MapControllerRoute(name: "html", pattern: "{controller=Home}/{action=Index}.html");

                endpoints.MapControllerRoute(name: "soft", pattern: "soft/{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
