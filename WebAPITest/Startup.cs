using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NLog.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WebAPITest.Filter;
using WebAPITest.Models;
using WebAPITest.Models.Database;
using WebAPITest.Service;

namespace WebAPITest
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services">services是一个服务的集合</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(logBuilder => {
                //清楚原来日志组件
                logBuilder.ClearProviders();
                logBuilder.AddNLog();
            });

            //注入控制器服务
            services.AddControllers(x => {
                //添加全局的权限筛选器
                x.Filters.Add<AuthorizeFilter>();
                x.Filters.Add<GlobalActionFilter>();
                x.Filters.Add<ResourceFilter>();
                x.Filters.Add<GlobalExceptionFilter>();
                x.Filters.Add<ResultFilter>();
            });

            //启用本地缓存服务
            services.AddMemoryCache();

            //添加swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Web企业级开发II", Version = "v1" });
                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Description = "在下框中输入请求头中需要添加Jwt授权Token：Bearer Token",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme{
                            Reference = new OpenApiReference {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"}
                        },new string[] { }
                    }
                });
            });

            //读取appseting.json
            services.Configure<JWTConfig>(Configuration.GetSection("JWTConfig"));
            var tokenConfigs = Configuration.GetSection("JWTConfig").Get<JWTConfig>();
            //Authentication 添加授权服务
            services.AddAuthentication(x =>
            {
                //指定使用的授权方式
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x => {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenConfigs.Secret)),
                    ValidIssuer = tokenConfigs.Issuer,
                    ValidAudience = tokenConfigs.Audience,
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            services.AddScoped<IJWTService, JWTService>();
            //注入数据库服务

            //注入自己定义的类
            //new ServiceDescriptor类 传入三个参数给他 1、服务接口的类型，2、服务类的类型 3，服务的生命周期
            //typeof()这个方法是获取类型
            services.Add(new ServiceDescriptor(typeof(IDataService), typeof(DataService), ServiceLifetime.Scoped));

            //生命周期 三种类型
            //第一种 单例 Singleton: 容器会创建并共享服务的单例，且一直会存在于应用程序的整个生命周期内。
            //在应用程序的整个生命周期 无论获取多少次 都是同一个
            services.Add(new ServiceDescriptor(typeof(ISingletonService), typeof(SingletonService), ServiceLifetime.Singleton));
            //等价于
            services.AddSingleton<ISingletonService, SingletonService>();

            //第二种 瞬时Transient:每次服务被请求时，总会创建新实例。
            //当我们每次从IOC容器获取对象时 都是不同的对象
            services.Add(new ServiceDescriptor(typeof(ITransientService), typeof(TransientService), ServiceLifetime.Transient));
            //等价于
            services.AddTransient<ITransientService, TransientService>();

            //第三种 Scoped:在每一次请求时会创建服务的新实例，并在这个请求内一直共享这个实例。
            //当我们同一个请求内 每次从IOC容器获取对象时 都是同一个，在不同的请求 会是不一样的对象
            services.Add(new ServiceDescriptor(typeof(IScopedService), typeof(ScopedService), ServiceLifetime.Scoped));
            //等价于
            services.AddScoped<IScopedService, ScopedService>();

            //注入数据上下文
            services.AddDbContext<WebEnterpriseIIContext>();

            //添加Redis服务
            //redis缓存
            var section = Configuration.GetSection("Redis:Default");
            //连接字符串
            string _connectionString = section.GetSection("Connection").Value;
            //实例名称
            string _instanceName = section.GetSection("InstanceName").Value;
            //默认数据库 
            int _defaultDB = int.Parse(section.GetSection("DefaultDB").Value ?? "0");
            //密码
            string _password = section.GetSection("Password").Value;
            //注入容器
            services.AddSingleton(new RedisHelper(_connectionString, _instanceName, _password, _defaultDB));

            //注入用户领域服务
            services.AddScoped<IUserService, UserService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            //启用授权服务
            app.UseAuthentication();

            app.UseRouting();
            //启用认证
            app.UseAuthorization();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "coreMVC3.1");
                //c.RoutePrefix = string.Empty;
                c.RoutePrefix = "swagger";     //如果是为空 访问路径就为 根域名/index.html,注意localhost:端口号/swagger是访问不到的
                                               //路径配置，设置为空，表示直接在根域名（localhost:8001）访问该文件
                                               // c.RoutePrefix = "swagger"; // 如果你想换一个路径，直接写名字即可，比如直接写c.RoutePrefix = "swagger"; 则访问路径为 根域名/swagger/index.html
            });
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
