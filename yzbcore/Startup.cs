using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using System.Threading.Tasks;
using log4net;
using log4net.Config;
using log4net.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.PlatformAbstractions;
using SuperSocket;
using SuperSocket.ProtoBase;
using Swashbuckle.AspNetCore.Swagger;
using yzbcore.Bussiness;
using yzbcore.Repository;
using yzbcore.Socket;

namespace yzbcore
{
    public class Startup
    {
        public Startup( IConfiguration configuration)
        {
            Configuration = configuration;
            // log4net 仓储
            repository = LogManager.CreateRepository("CoreLogRepository");
            XmlConfigurator.Configure(repository, new FileInfo("log4net.config"));
            Log4NetRepository.loggerRepository = repository;
            LogHelper.Error("staart");
            //await Task.Delay(TimeSpan.FromMinutes(x miutes))
            //, IWSSendData sendData
            //_sendData = LogManager.CreateRepository("CoreLogRepository");
            //_sendData = sendData;
            //_sendData.Build();
            //var host = SuperSocketHostBuilder.Create<StringPackageInfo, CommandLinePipelineFilter>()
            //.UsePackageHandler(async (s, p) =>
            //{
            //    // handle packages
            //})
            //.Build();

            //await host.RunAsync();
        }

        public static IWSSendData _sendData { get; set; }
        public static ILoggerRepository repository { get; set; }
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            ////注册Swagger生成器，定义一个和多个Swagger 文档
            //services.AddSwaggerGen(c =>
            //{
            //    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "My API", Version = "v1" });
            //});
            //services.AddSwaggerGen();
            //services.ConfigureSwaggerGen(options =>
            //{
            //    //options..SingleApiVersion(new Swashbuckle.Swagger.Model.Info
            //    //{
            //    //    Version = "v1",
            //    //    Title = "My Web Application",
            //    //    Description = "RESTful API for My Web Application",
            //    //    TermsOfService = "None"
            //    //});
            //    options.IncludeXmlComments(Path.Combine(PlatformServices.Default.Application.ApplicationBasePath,
            //        "yzbcore.XML")); // 注意：此处替换成所生成的XML documentation的文件名。
            //    options.DescribeAllEnumsAsStrings();
            //});
            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
            });
            //配置跨域访问问题
            services.AddCors(
                options => options.AddPolicy("CorsTest",
                p => p.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod()
                )
             );
            services.AddScoped<IAdminRepository, AdminRepository>();
            services.AddScoped<IBirdhouseRepository, BirdhouseRepository>();
            services.AddScoped<IEquipment_status_logsRepository, Equipment_status_logsRepository>();
            services.AddScoped<IEquipmentRepository, EquipmentRepository>();
            services.AddScoped<IMemberRepository, MemberRepository>();
            services.AddScoped<ISms_logsRepository, Sms_logsRepository>();
            services.AddScoped<IWarning_logsRepository, Warning_logsRepository>();
            services.AddScoped<IConnectionProvider,ConnectionProvider>();
            //services.AddScoped<IFetchData, FetchData>();
            services.AddScoped<IParseData, ParseData>();
            //services.AddScoped<IWSSendData, WSSendData>();
            services.AddScoped<ICache, Cache>();
            services.AddScoped<ICALL, CALL>();
            services.AddScoped<ISMS, SMS>();
            services.AddScoped<IWECHAT, WECHAT>();
            services.AddScoped<IWSSendData,WSSendData>();
            //services.AddHostedService<ConsumeScopedServiceHostedService>();
            //services.AddScoped<IScopedProcessingService, ScopedProcessingService>();
            services.AddWebSocketManager();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,IWSSendData sendData)
        {
            var serviceScopeFactory = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>();
            var serviceProvider = serviceScopeFactory.CreateScope().ServiceProvider;

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();
            app.UseRouting();
            app.UseCors("CorsTest");//其中app.UseCors()必须放在app.UseRouting()和app.UseEndpoints之间，不然还是解决不了问题。
            app.UseAuthorization();
            app.UseHttpsRedirection();
            app.UseCookiePolicy();
            //app.UseMvc();
            //app.UseSwagger();
            //app.UseSwaggerUI();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapControllerRoute("apiDefault", "app/[controller]/[action]");
                endpoints.MapAreaControllerRoute(
                     name: "api", "api",
                     pattern: "{api:exists}/{controller=Home}/{action=Index}/{id?}");
                //endpoints.MapRazorPages();
            });
            app.UseWebSockets();
            //app.MapWebSocketManager("/ws", serviceProvider.GetService<WSSendData>());
            //fetch.Start();
            _sendData = sendData;
            _sendData.Build();
        }
    }
}
