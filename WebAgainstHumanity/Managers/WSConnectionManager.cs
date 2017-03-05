using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using WebAgainstHumanity.Models;

namespace WebAgainstHumanity.Managers
{
    public interface IConnectionManager
    {
        Connection GetConnectionById(string id);
        List<Connection> GetAll();
        string GetId(WebSocket socket);
        void AddConnection(Connection conn);
        Task RemoveSocket(string id);
    }

    public class WSConnectionManager : IConnectionManager
    {
        private ConcurrentDictionary<string, Connection> _sockets = new ConcurrentDictionary<string, Connection>();

        public Connection GetConnectionById(string id)
        {
            return _sockets.FirstOrDefault(p => p.Key == id).Value;
        }

        public List<Connection> GetAll()
        {
            return _sockets.Select(s => s.Value).ToList();
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