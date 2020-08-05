using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using SuperSocket.ProtoBase;
using SuperSocket.WebSocket;
using SuperSocket.WebSocket.Server;
using Microsoft.Extensions.Logging;
using SuperSocket.Command;
using System.Net.WebSockets;
using Microsoft.AspNetCore.Http;
using System.Threading;
using System.Text;
using Newtonsoft.Json;
using yzbcore.Bussiness;
using Microsoft.Extensions.DependencyInjection;

namespace yzbcore.Socket
{
    //public class NotificationsMessageHandler : WebSocketHandler
    //{
    //    public NotificationsMessageHandler(ConnectionManager webSocketConnectionManager) : base(webSocketConnectionManager)
    //    {
    //    }
    //    public override async Task OnConnected(WebSocket socket)
    //    {
    //        await base.OnConnected(socket);

    //        var socketId = WebSocketConnectionManager.GetId(socket);
    //        await SendMessageToAllAsync($"{socketId} is now connected");
    //        yzbcore.Bussiness.LogHelper.Info($"{socketId} is now connected");
    //    }
    //    public override async Task ReceiveAsync(WebSocket socket, WebSocketReceiveResult result, byte[] buffer)
    //    {
    //        var socketId = WebSocketConnectionManager.GetId(socket);
    //        var message = $"{socketId} said: {Encoding.UTF8.GetString(buffer, 0, result.Count)}";
    //        yzbcore.Bussiness.LogHelper.Info(message);
    //        await SendMessageToAllAsync(message);
    //    }
    //}
    public class WSSendData: WebSocketHandler,IWSSendData
    {
        //public ConnectionManager connectionManager;
        //public WSSendData(ConnectionManager webSocketConnectionManager) : base(webSocketConnectionManager)
        //{
        //    connectionManager = webSocketConnectionManager;
        //}

        ////public Task Build()
        ////{
        ////    throw new NotImplementedException();
        ////}

        ////public Task Echo(HttpContext context, WebSocket webSocket)
        ////{
        ////    throw new NotImplementedException();
        ////}

        //public override async Task ReceiveAsync(WebSocket socket, WebSocketReceiveResult result, byte[] buffer)
        //{
        //    var socketId = WebSocketConnectionManager.GetId(socket);
        //    var message = $"{socketId} said: {Encoding.UTF8.GetString(buffer, 0, result.Count)}";
        //    yzbcore.Bussiness.LogHelper.Info(message);
        //    await SendMessageToAllAsync(message);
        //}
        //public override async Task OnConnected(WebSocket socket)
        //{
        //    await base.OnConnected(socket);
        //    var socketId = WebSocketConnectionManager.GetId(socket);
        //    await SendMessageToAllAsync($"{socketId} is now connected");
        //    yzbcore.Bussiness.LogHelper.Info($"{socketId} is now connected");
        //}
        public async Task SendToUid(string uid, string json) 
        {
            var id = ConnectionManager.GetSocketIdBySerial(uid);
            if (!string.IsNullOrEmpty(id))
            {
                var session = ConnectionManager.GetSocketSessionById(id);
                if (session != null&& session.State!= SuperSocket.SessionState.Closed) { await session.SendAsync(json); }
                else { LogHelper.Error("SendToUid" + id + "对应会话已丢失"); }
                //await SendMessageAsync(uid, json);
                //await SendMessageToAllAsync(json);
            }
            else { LogHelper.Error("SendToUid" + uid + "没有socket,内容" + json); }
        }
        public async Task Echo(HttpContext context, WebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];
            WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            while (!result.CloseStatus.HasValue)
            {
                await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType, result.EndOfMessage, CancellationToken.None);

                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }
            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }
        public async Task Build()
        {
            var host = WebSocketHostBuilder.Create()
                        .UseWebSocketMessageHandler(
                            async (session, message) =>
                            {
                                yzbcore.Bussiness.LogHelper.Info(JsonConvert.SerializeObject(message));
                                ConnectionManager.AddSocket(session);
                                var socketId = ConnectionManager.GetId(session);
                                var uid = JsonConvert.DeserializeObject<wsmessgae>(message.Message);
                                ConnectionManager.AddSerial(uid.uid, socketId);
                                yzbcore.Bussiness.LogHelper.Info(uid.uid);
                                //await session.SendAsync(uid.uid);
                                LogHelper.Error("收到ws连接uid"+ uid+ "socketId"+ socketId);
                            }
                        )
                        .ConfigureAppConfiguration((hostCtx, configApp) =>
                        {
                            configApp.AddInMemoryCollection(new Dictionary<string, string>
                            {
                                //{ "handshakeoptions:checkinginterval", "1800" },
                                { "serverOptions:name", "YZBServer" },
                                { "serverOptions:listeners:0:ip", "Any" },
                                { "serverOptions:listeners:0:port", "2347" }
                            });
                        }) .ConfigureServices((ctx, services) =>
                        {
                            services.Configure<HandshakeOptions>(options =>
                            {
                                options.CheckingInterval = 1800;
                                options.OpenHandshakeTimeOut = 1;
                                options.CloseHandshakeTimeOut = 1800;
                            });
                        })
                        .ConfigureLogging((hostCtx, loggingBuilder) =>
                        {
                            loggingBuilder.AddConsole();
                        })
                        .Build();

            await host.RunAsync();
            
        }

        public override Task ReceiveAsync(WebSocket socket, WebSocketReceiveResult result, byte[] buffer)
        {
            throw new NotImplementedException();
        }

       
    }

    class StringPackageConverter : IPackageMapper<WebSocketPackage, StringPackageInfo>
    {
        public StringPackageInfo Map(WebSocketPackage package)
        {
            var pack = new StringPackageInfo();
            var arr = package.Message.Split(' ', 3, StringSplitOptions.RemoveEmptyEntries);
            pack.Key = arr[0];
            pack.Parameters = arr.Skip(1).ToArray();
            return pack;
        }
    }
    class ADD : IAsyncCommand<WebSocketSession, StringPackageInfo>
    {
        public async ValueTask ExecuteAsync(WebSocketSession session, StringPackageInfo package)
        {
            var result = package.Parameters
                .Select(p => int.Parse(p))
                .Sum();

            await session.SendAsync(result.ToString());
        }
    }
    public interface IWSSendData
    {
        public  Task Build();
        public  Task Echo(HttpContext context, WebSocket webSocket);
        public Task SendToUid(string uid,string json);
    }
    public class wsmessgae
    {
        /// <summary>
        /// 
        /// </summary>
        public string uid { get; set; }
    }
}
