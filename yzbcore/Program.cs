using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using yzbcore.Socket;

namespace yzbcore
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>().ConfigureServices(svc => svc.AddScoped<IWSSendData, WSSendData>())
                    .ConfigureServices(svc => svc.AddHostedService<ConsumeScopedServiceHostedService>())
                    ;
                    webBuilder.UseUrls("http://0.0.0.0:2347");
                });
    }
}
