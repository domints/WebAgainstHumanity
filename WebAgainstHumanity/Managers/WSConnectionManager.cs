using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using WebAgainstHumanity.Models;

namespace WebAgainstHumanity.Managers
{
    public class WSConnectionManager
    {
        private ConcurrentDictionary<string, Connection> _sockets = new ConcurrentDictionary<string, Connection>();

        public Connection GetConnectionById(string id)
        {
            return _sockets.FirstOrDefault(p => p.Key == id).Value;
        }

        public ConcurrentDictionary<string, Connection> GetAll()
        {
            return _sockets;
        }

        public string GetId(WebSocket socket)
        {
            return _sockets.FirstOrDefault(p => p.Value.Socket == socket).Key;
        }
        public void AddConnection(Connection conn)
        {
            _sockets.TryAdd(CreateConnectionId(), conn);
        }

        public async Task RemoveSocket(string id)
        {
            Connection conn;
            _sockets.TryRemove(id, out conn);

            await conn.Socket.CloseAsync(closeStatus: WebSocketCloseStatus.NormalClosure, 
                                    statusDescription: "Closed by the WebSocketManager", 
                                    cancellationToken: CancellationToken.None);
        }

        private string CreateConnectionId()
        {
            return Guid.NewGuid().ToString();
        }
    }
}