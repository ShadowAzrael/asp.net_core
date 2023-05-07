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
        /// <param name="services">services��һ������ļ���</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(logBuilder => {
                //���ԭ����־���
                logBuilder.ClearProviders();
                logBuilder.AddNLog();
            });

            //ע�����������
            services.AddControllers(x => {
                //���ȫ�ֵ�Ȩ��ɸѡ��
                x.Filters.Add<AuthorizeFilter>();
                x.Filters.Add<GlobalActionFilter>();
                x.Filters.Add<ResourceFilter>();
                x.Filters.Add<GlobalExceptionFilter>();
                x.Filters.Add<ResultFilter>();
            });

            //���ñ��ػ������
            services.AddMemoryCache();

            //���swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Web��ҵ������II", Version = "v1" });
                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Description = "���¿�����������ͷ����Ҫ���Jwt��ȨToken��Bearer Token",
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

            //��ȡappseting.json
            services.Configure<JWTConfig>(Configuration.GetSection("JWTConfig"));
            var tokenConfigs = Configuration.GetSection("JWTConfig").Get<JWTConfig>();
            //Authentication �����Ȩ����
            services.AddAuthentication(x =>
            {
                //ָ��ʹ�õ���Ȩ��ʽ
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
            //ע�����ݿ����

            //ע���Լ��������
            //new ServiceDescriptor�� ���������������� 1������ӿڵ����ͣ�2������������� 3���������������
            //typeof()��������ǻ�ȡ����
            services.Add(new ServiceDescriptor(typeof(IDataService), typeof(DataService), ServiceLifetime.Scoped));

            //�������� ��������
            //��һ�� ���� Singleton: �����ᴴ�����������ĵ�������һֱ�������Ӧ�ó�����������������ڡ�
            //��Ӧ�ó���������������� ���ۻ�ȡ���ٴ� ����ͬһ��
            services.Add(new ServiceDescriptor(typeof(ISingletonService), typeof(SingletonService), ServiceLifetime.Singleton));
            //�ȼ���
            services.AddSingleton<ISingletonService, SingletonService>();

            //�ڶ��� ˲ʱTransient:ÿ�η�������ʱ���ܻᴴ����ʵ����
            //������ÿ�δ�IOC������ȡ����ʱ ���ǲ�ͬ�Ķ���
            services.Add(new ServiceDescriptor(typeof(ITransientService), typeof(TransientService), ServiceLifetime.Transient));
            //�ȼ���
            services.AddTransient<ITransientService, TransientService>();

            //������ Scoped:��ÿһ������ʱ�ᴴ���������ʵ�����������������һֱ�������ʵ����
            //������ͬһ�������� ÿ�δ�IOC������ȡ����ʱ ����ͬһ�����ڲ�ͬ������ ���ǲ�һ���Ķ���
            services.Add(new ServiceDescriptor(typeof(IScopedService), typeof(ScopedService), ServiceLifetime.Scoped));
            //�ȼ���
            services.AddScoped<IScopedService, ScopedService>();

            //ע������������
            services.AddDbContext<WebEnterpriseIIContext>();

            //���Redis����
            //redis����
            var section = Configuration.GetSection("Redis:Default");
            //�����ַ���
            string _connectionString = section.GetSection("Connection").Value;
            //ʵ������
            string _instanceName = section.GetSection("InstanceName").Value;
            //Ĭ�����ݿ� 
            int _defaultDB = int.Parse(section.GetSection("DefaultDB").Value ?? "0");
            //����
            string _password = section.GetSection("Password").Value;
            //ע������
            services.AddSingleton(new RedisHelper(_connectionString, _instanceName, _password, _defaultDB));

            //ע���û��������
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

            //������Ȩ����
            app.UseAuthentication();

            app.UseRouting();
            //������֤
            app.UseAuthorization();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "coreMVC3.1");
                //c.RoutePrefix = string.Empty;
                c.RoutePrefix = "swagger";     //�����Ϊ�� ����·����Ϊ ������/index.html,ע��localhost:�˿ں�/swagger�Ƿ��ʲ�����
                                               //·�����ã�����Ϊ�գ���ʾֱ���ڸ�������localhost:8001�����ʸ��ļ�
                                               // c.RoutePrefix = "swagger"; // ������뻻һ��·����ֱ��д���ּ��ɣ�����ֱ��дc.RoutePrefix = "swagger"; �����·��Ϊ ������/swagger/index.html
            });
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
