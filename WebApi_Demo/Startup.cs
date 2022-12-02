using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MVC_Demo.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi_Demo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //添加cors
            services.AddCors(options =>
            {
                //添加跨域配置
                //添加core 策略
                options.AddPolicy("Policy1", //策略名
                    builder =>
                    {
                        builder.WithOrigins("*") //表示可以被所有地址跨域访问)
                                            .SetIsOriginAllowedToAllowWildcardSubdomains()//设置策略里的域名允许通配符匹配，但是不包括空。
                                                                                          //例：http://localhost:3001 不会被通过
                                                                                          // http://xxx.localhost:3001 可以通过
                                            .AllowAnyHeader()//配置请求头
                                            .AllowAnyMethod();//配置允许任何 HTTP 方法访问
                    });
                //添加另外一个 core 策略 添加多个策略可以为不同域名的源进行配置。 策略名不能重复
                options.AddPolicy("Policy2",
                    builder =>
                    {
                        builder.WithOrigins("http://www.contoso.com")
                                            .AllowAnyHeader()
                                            .AllowAnyMethod();
                    });
            });

            services.AddControllers();
            //注入数据库服务 第一步注入
            services.AddDbContext<McStoreContext>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //如果是开发环境则显示异常信息页
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //把http请求转到https
            app.UseHttpsRedirection();

            app.UseRouting();

            //使用cors，注意中间件的位置位于UseRouting与UserAuthorization之间
            app.UseCors("Policy1");//此处如果填写了app.UserCors("Policy1")，则控制器中默认使用该策略，并且不需要在控制器上添加[EnableCors("Policy1")]

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                //default
                endpoints.MapControllers();
            });
        }
    }
}
