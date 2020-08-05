using SuperSocket.WebSocket.Server;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace yzbcore.Socket
{
    public static class ConnectionManager
    {
        private static ConcurrentDictionary<string, WebSocket> _sockets = new ConcurrentDictionary<string, WebSocket>();
        private static ConcurrentDictionary<string, WebSocketSession> _socketsession = new ConcurrentDictionary<string, WebSocketSession>();
        public static Dictionary<string, string> uidtoguid = new Dictionary<string, string>();
        public static WebSocket GetSocketById(string id)
        {
            return _sockets.FirstOrDefault(p => p.Key == id).Value;
        }
        public static WebSocketSession GetSocketSessionById(string id)
        {
            //foreach (var item in _socketsession)
            //{
            //    if(item.Value.State== SuperSocket.SessionState.Closed)
            //}
            
            //_socketsession.TryRemove()
            return _socketsession.FirstOrDefault(p => p.Key == id&&p.Value.State== SuperSocket.SessionState.Connected).Value;
        }
        #region serial to guid
        public static string GetSocketIdBySerial(string serialid)
        {
            return uidtoguid.FirstOrDefault(p => p.Key == serialid).Value;
        }
        public static void AddSerial(string serialid,string guid)
        {
            if (uidtoguid.ContainsKey(serialid))
            {
               var guidold= GetSocketIdBySerial(serialid);
                RemoveSocketSession(guidold);
                uidtoguid.Remove(serialid);

            }
            
            uidtoguid.TryAdd(serialid, guid);
        }

        public static async Task RemoveSocketSerial(string id)
        {
            string guid;
            uidtoguid.Remove(id, out guid);
            await RemoveSocketSession(guid);
               
        }
        #endregion
        public static ConcurrentDictionary<string, WebSocket> GetAll()
        {
            return _sockets;
        }

        public static ConcurrentDictionary<string, WebSocketSession> GetAllSession()
        {
            return _socketsession;
        }

        public static string GetId(WebSocket socket)
        {
            return _sockets.FirstOrDefault(p => p.Value == socket).Key;
        }
        public static string GetId(WebSocketSession socket)
        {
            return _socketsession.FirstOrDefault(p => p.Value == socket).Key;
        }
        public static void AddSocket(WebSocket socket)
        {
            _sockets.TryAdd(CreateConnectionId(), socket);
        }
        public static void AddSocket(WebSocketSession socket)
        {
            _socketsession.TryAdd(CreateConnectionId(), socket);
        }
        public static async Task RemoveSocket(string id)
        {
            WebSocket socket;
            _sockets.TryRemove(id, out socket);

            await socket.CloseAsync(closeStatus: WebSocketCloseStatus.NormalClosure, 
                                    statusDescription: "Closed by the ConnectionManager", 
                                    cancellationToken: CancellationToken.None);
        }
        public static async Task RemoveSocketSession(string id)
        {
            WebSocketSession socket;
            _socketsession.TryRemove(id, out socket);

            await socket.CloseAsync( SuperSocket.WebSocket.CloseReason.GoingAway);
        }
        private static string CreateConnectionId()
        {
            return Guid.NewGuid().ToString();
        }
    }
}