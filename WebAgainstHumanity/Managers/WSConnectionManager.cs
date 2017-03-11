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
        Connection GetConnectionByWs(WebSocket socket);
        List<Connection> GetAll();
        List<Connection> GetAllInRoom(string room);
        string GetId(WebSocket socket);
        string AddWebSocket(WebSocket conn, Session session, ConnectionLocation location = ConnectionLocation.Lobby);
        void AddConnectionToRoom(string room, Connection conn);
        Task RemoveConnection(string id);
        Connection RemoveConnectionFromRoom(string group, string id);
    }

    public class WSConnectionManager : IConnectionManager
    {
        private ConcurrentDictionary<string, Connection> _sockets = new ConcurrentDictionary<string, Connection>();
        private ConcurrentDictionary<string, ConcurrentDictionary<string, Connection>> _rooms = new ConcurrentDictionary<string, ConcurrentDictionary<string, Connection>>();

        public Connection GetConnectionById(string id)
        {
            return _sockets.FirstOrDefault(p => p.Key == id).Value;
        }

        public Connection GetConnectionByWs(WebSocket socket)
        {
            return _sockets.FirstOrDefault(p => p.Value.Socket == socket).Value;
        }

        public string GetId(WebSocket socket)
        {
            return _sockets.FirstOrDefault(p => p.Value.Socket == socket).Key;
        }

        public List<Connection> GetAll()
        {
            return _sockets.Select(s => s.Value).ToList();
        }

        public List<Connection> GetAllInRoom(string room)
        {
            ConcurrentDictionary<string, Connection> list;
            bool success = _rooms.TryGetValue(room, out list);
            return success ? list.Select(c => c.Value).ToList() : Enumerable.Empty<Connection>().ToList();
        }

        public string AddWebSocket(WebSocket conn, Session session, ConnectionLocation location = ConnectionLocation.Lobby)
        {
            string connId = CreateConnectionId();
            bool success = _sockets.TryAdd(connId, new Connection
            {
                Id = connId,
                Socket = conn,
                Location = location,
                Session = session,
            });

            return success ? connId : null;
        }

        public void AddConnectionToRoom(string room, Connection conn)
        {
            _rooms.TryAdd(room, new ConcurrentDictionary<string, Connection>());
            bool added = _rooms[room].TryAdd(conn.Id, conn);
            conn.Room.Add(room);
        }

        public async Task RemoveConnection(string id)
        {
            Connection conn;
            _sockets.TryRemove(id, out conn);
            var rooms = conn.Room.ToList();
            foreach(var room in rooms)
            {
                RemoveConnectionFromRoom(room, conn.Id);
            }

            await conn.Socket.CloseAsync(closeStatus: WebSocketCloseStatus.NormalClosure, 
                                    statusDescription: "Closed by the WebSocketManager", 
                                    cancellationToken: CancellationToken.None);
        }

        public Connection RemoveConnectionFromRoom(string room, string id)
        {
            Connection result;
            _rooms.TryAdd(room, new ConcurrentDictionary<string, Connection>());
            bool removed = _rooms[room].TryRemove(id, out result);
            if (removed)
            {
                result.Room.Remove(room);
            }

            return result;
        }

        private string CreateConnectionId()
        {
            return Guid.NewGuid().ToString();
        }
    }
}