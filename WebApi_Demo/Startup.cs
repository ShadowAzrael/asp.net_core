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
            //���cors
            services.AddCors(options =>
            {
                //��ӿ�������
                //���core ����
                options.AddPolicy("Policy1", //������
                    builder =>
                    {
                        builder.WithOrigins("*") //��ʾ���Ա����е�ַ�������)
                                            .SetIsOriginAllowedToAllowWildcardSubdomains()//���ò��������������ͨ���ƥ�䣬���ǲ������ա�
                                                                                          //����http://localhost:3001 ���ᱻͨ��
                                                                                          // http://xxx.localhost:3001 ����ͨ��
                                            .AllowAnyHeader()//��������ͷ
                                            .AllowAnyMethod();//���������κ� HTTP ��������
                    });
                //�������һ�� core ���� ��Ӷ�����Կ���Ϊ��ͬ������Դ�������á� �����������ظ�
                options.AddPolicy("Policy2",
                    builder =>
                    {
                        builder.WithOrigins("http://www.contoso.com")
                                            .AllowAnyHeader()
                                            .AllowAnyMethod();
                    });
            });

            services.AddControllers();
            //ע�����ݿ���� ��һ��ע��
            services.AddDbContext<McStoreContext>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //����ǿ�����������ʾ�쳣��Ϣҳ
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //��http����ת��https
            app.UseHttpsRedirection();

            app.UseRouting();

            //ʹ��cors��ע���м����λ��λ��UseRouting��UserAuthorization֮��
            app.UseCors("Policy1");//�˴������д��app.UserCors("Policy1")�����������Ĭ��ʹ�øò��ԣ����Ҳ���Ҫ�ڿ����������[EnableCors("Policy1")]

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                //default
                endpoints.MapControllers();
            });
        }
    }
}
