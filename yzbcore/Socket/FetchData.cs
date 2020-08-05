using log4net;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SuperSocket;
using SuperSocket.ProtoBase;
using System.Text;
using System.Threading.Tasks;
using yzbcore.Bussiness;
using SuperSocket.Server;
using System;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;

namespace yzbcore.Socket
{
    //public interface IFetchData
    //{
    //    #region Public Methods

    //    public Task Start();

    //    #endregion Public Methods
    //}
    #region 定时任务
    //internal interface IScopedProcessingService
    //{
    //    Task DoWork(CancellationToken stoppingToken);
    //}

    //internal class ScopedProcessingService : IScopedProcessingService
    //{
    //    private int executionCount = 0;
    //    private readonly ILogger _logger;

    //    public ScopedProcessingService(ILogger<ScopedProcessingService> logger)
    //    {
    //        _logger = logger;
    //    }

    //    public async Task DoWork(CancellationToken stoppingToken)
    //    {
    //        LogHelper.Error("ScopedProcessingService DoWork stoppingToken");
    //        while (!stoppingToken.IsCancellationRequested)
    //        {
    //            executionCount++;

    //            _logger.LogInformation(
    //                "Scoped Processing Service is working. Count: {Count}", executionCount);
    //            LogHelper.Error("ScopedProcessingService DoWork");
    //            await Task.Delay(10000, stoppingToken);
    //        }
    //    }
    //}
    #endregion

    public class ConsumeScopedServiceHostedService : BackgroundService
    {
        private readonly ILogger<ConsumeScopedServiceHostedService> _logger;
        //public IParseData _parser;
        IServiceScopeFactory _serviceScopeFactory;
        public ConsumeScopedServiceHostedService(IServiceProvider services, IServiceScopeFactory serviceScopeFactory,
            //, IParseData parser
            ILogger<ConsumeScopedServiceHostedService> logger)
        {
            Services = services;
            //_parser = parser;
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }
        public override async Task StartAsync(CancellationToken cancellationToken)
        {
           
            LogHelper.Error("FetchDataService StartAsync");
            //Console.WriteLine("FetchDataService start");
           
            await ExecuteAsync(cancellationToken);
        }
        public IServiceProvider Services { get; }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //_logger.LogInformation(
            //    "Consume Scoped Service Hosted Service running.");
            //LogHelper.Error("Consume Scoped Service Hosted Service running");
            Task taskOne = RunTaskOne(stoppingToken);
            //await DoWork(stoppingToken);
        }
        protected async Task RunTaskOne(CancellationToken stoppingToken)
        {
             Task.Run(() =>
            {
                LogHelper.Error("FetchDataService start01");
                //如果服务被停止，那么下面的IsCancellationRequested会返回true，我们就应该结束循环
                //while (!stoppingToken.IsCancellationRequested)
                {
                    LogHelper.Error("FetchDataService start02");
                    //LogHelper.Error("Consume Scoped Service Hosted Service is working");
                    var host = SuperSocketHostBuilder.Create<StringPackageInfo, CommandLinePipelineFilter>()
                               .UsePackageHandler(async (s, p) =>
                               {
                           // handle packages
                           LogHelper.Error("handle packages");
                                   LogHelper.Error("MESSAGE" + JsonConvert.SerializeObject(p.Key));
                                   using (var scope = _serviceScopeFactory.CreateScope())
                                   {
                                       var context = scope.ServiceProvider.GetRequiredService<IParseData>();
                                       //other logic
                                       LogHelper.Error("FetchDataService ProcessDATA");
                                       context.ProcessDATA(p.Key);
                           }

                               }).UseSession<MyAppSession>()
                               .ConfigureLogging((hostCtx, loggingBuilder) =>
                               {
                                   loggingBuilder.AddConsole();
                               })
                               .Build();
                    host.RunAsync();
                }
            }, stoppingToken);
        }
        private async Task DoWork(CancellationToken stoppingToken)
        {
            LogHelper.Error("FetchDataService DoWork");
            //_logger.LogInformation(
            //    "Consume Scoped Service Hosted Service is working.");

            //using (var scope = Services.CreateScope())
            //{
            //    var scopedProcessingService =
            //        scope.ServiceProvider
            //            .GetRequiredService<IScopedProcessingService>();

            //    await scopedProcessingService.DoWork(stoppingToken);
            //}
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            //LogHelper.Error("Consume Scoped Service Hosted Service is stopping");
            //_logger.LogInformation(
            //    "Consume Scoped Service Hosted Service is stopping.");
            LogHelper.Error("FetchDataService StopAsync");
            await Task.CompletedTask;
        }
    }
    //public class FetchDataService : BackgroundService
    //{
    //    public IFetchData _fetchData;
    //    public FetchDataService(IFetchData fetchData)
    //    {
    //        _fetchData = fetchData;
    //    }

    //    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    //    {
    //        while (!stoppingToken.IsCancellationRequested)
    //        {
    //            Console.WriteLine("FetchDataService test");

    //            //await run job

    //            await Task.Delay(TimeSpan.FromSeconds(1));
    //        }
    //    }

    //    public override async Task StartAsync(CancellationToken cancellationToken)
    //    {
    //        Console.WriteLine("FetchDataService start");
    //        _fetchData.Start();
    //        await ExecuteAsync(cancellationToken);
    //    }

    //    public override Task StopAsync(CancellationToken cancellationToken)
    //    {
    //        Console.WriteLine("stop");

    //        return Task.CompletedTask;
    //    }

    //    //protected override Task ExecuteAsync(CancellationToken stoppingToken)
    //    //{
    //    //    throw new NotImplementedException();
    //    //}
    //}
    //public class FetchData : IFetchData
    //{
    //    public IParseData _parser;
    //    public FetchData(IParseData parser) 
    //    {
    //        _parser = parser;
    //    }
    //    #region Public Methods

    //    public async Task Start()
    //    {
    //        var host = SuperSocketHostBuilder.Create<StringPackageInfo, CommandLinePipelineFilter>()
    //                    .UsePackageHandler(async (s, p) =>
    //                    {
    //                        // handle packages
    //                        LogHelper.Error("UsePackageHandler" + JsonConvert.SerializeObject(p));
    //                        LogHelper.Error("MESSAGE" + JsonConvert.SerializeObject(p.Key));
    //                        //_parser.ProcessDATA(p.Key);
    //                    }).UseSession<MyAppSession>()
    //                    .ConfigureLogging((hostCtx, loggingBuilder) =>
    //                    {
    //                        loggingBuilder.AddConsole();
    //                    })
    //                    .Build();

    //        await host.RunAsync();
    //    }
    //    //public async Task Start2()
    //    //{
    //    //    var host = SuperSocketHostBuilder
    //    //.Create<StringPackageInfo, CommandLinePipelineFilter>()
    //    //    //用Package的类型和PipeLineFilter的类型创建SuperSocket宿主。
    //    //    //注册用于处理接收到的数据的包处理器
    //    //    .UsePackageHandler(async (s, p) =>
    //    //    {
    //    //        var result = 0;
    //    //        LogHelper.Error("UsePackageHandler" + JsonConvert.SerializeObject(p));
    //    //        //await s.SendAsync(Encoding.UTF8.GetBytes(result.ToString() + "\r\n"));
    //    //    })
    //    //    //将收到的文字发送给客户端。

    //    //    //配置日志
    //    //    .ConfigureLogging((hostCtx, loggingBuilder) =>
    //    //    {
    //    //        loggingBuilder.AddConsole();
    //    //    })
    //    //    //仅仅启用Console日志输出, 你也可以在此处注册你自己需要的第三方日志类库。

    //    //    .Build();
    //    //    //启动宿主
    //    //    await host.RunAsync();
    //    //}
    //        #endregion Public Methods
    //    }
    public class MyAppSession : AppSession
    {
        protected override ValueTask OnSessionConnectedAsync()
        {
            // 会话连接建立后的逻辑
            LogHelper.Error("会话连接建立" + this.StartTime);
            return new ValueTask { };
        }

        protected override ValueTask OnSessionClosedAsync(EventArgs e)
        {
            // 会话连接断开后的逻辑
            LogHelper.Error("会话连接断开" + this.StartTime);
            this.Channel.Start();
            return new ValueTask { };
        }
    }
}