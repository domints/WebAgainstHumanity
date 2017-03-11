using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebAgainstHumanity.Managers;
using WebAgainstHumanity.Models;

namespace WebAgainstHumanity.Sockets
{
    public class WebSocketHandler
    {
        private readonly IConnectionManager _wsConnectionManager;
        private readonly IProtocolHandler _protocolHandler;

        public WebSocketHandler(
            IConnectionManager webSocketConnectionManager,
            IProtocolHandler protocolHandler)
        {
            this._wsConnectionManager = webSocketConnectionManager;
            this._protocolHandler = protocolHandler;
        }

        public virtual async Task OnConnected(WebSocket socket, Session session, ConnectionLocation location, string roomId)
        {
            string connId = this._wsConnectionManager.AddWebSocket(socket, session, location);
            Connection conn = this._wsConnectionManager.GetConnectionById(connId);
            await SendMessageAsync(conn.Socket, "CONN REGISTERED");
            if(location == ConnectionLocation.GameRoom && !string.IsNullOrWhiteSpace(roomId))
            {
                this._wsConnectionManager.AddConnectionToRoom(roomId, conn);
                await SendMessageAsync(conn.Socket, "CONN ADDEDTOROOM " + roomId);
            }
        }

        public virtual async Task OnDisconnected(WebSocket socket)
        {
            await this._wsConnectionManager.RemoveConnection(_wsConnectionManager.GetId(socket));
        }

        public async Task SendMessageAsync(WebSocket socket, string message)
        {
            if(socket.State != WebSocketState.Open)
                return;
            
            var messageBytes = Encoding.UTF8.GetBytes(message);
            await socket.SendAsync(buffer: new ArraySegment<byte>(array: messageBytes,
                                                                  offset: 0, 
                                                                  count: messageBytes.Length),
                                   messageType: WebSocketMessageType.Text,
                                   endOfMessage: true,
                                   cancellationToken: CancellationToken.None);          
        }

        public async Task SendMessageAsync(WebSocket socket, byte[] buffer, WebSocketMessageType messageType = WebSocketMessageType.Text)
        {
            if(socket.State != WebSocketState.Open)
                return;

            await socket.SendAsync(buffer: new ArraySegment<byte>(array: buffer,
                                                                  offset: 0, 
                                                                  count: buffer.Length),
                                   messageType: messageType,
                                   endOfMessage: true,
                                   cancellationToken: CancellationToken.None);          
        }

        public async Task SendMessageAsync(string socketId, string message)
        {
            await SendMessageAsync(this._wsConnectionManager.GetConnectionById(socketId).Socket, message);
        }

        public async Task SendMessageToAllAsync(string message)
        {
            await SendMessageToWebSocketsAsync(this._wsConnectionManager.GetAll().Select(c => c.Socket), message);
        }

        public async Task SendMessageToAllAsync(byte[] data, WebSocketMessageType messageType = WebSocketMessageType.Text)
        {
            await SendMessageToWebSocketsAsync(this._wsConnectionManager.GetAll().Select(c => c.Socket), data, messageType);
        }

        public async Task SendMessageToChannelAsync(WebSocket socket, string message)
        {
            Connection conn = this._wsConnectionManager.GetConnectionByWs(socket);
            await SendMessageToWebSocketsAsync(
                this._wsConnectionManager.GetAllInRoom(conn.Room.FirstOrDefault()).Select(c => c.Socket),
               message);
        }

        public async Task SendMessageToChannelAsync(WebSocket socket, byte[] data, WebSocketMessageType messageType = WebSocketMessageType.Text)
        {
            Connection conn = this._wsConnectionManager.GetConnectionByWs(socket);
            await SendMessageToWebSocketsAsync(
                this._wsConnectionManager.GetAllInRoom(conn.Room.FirstOrDefault()).Select(c => c.Socket),
                data,
                messageType);
        }

        public async Task SendMessageToWebSocketsAsync(IEnumerable<WebSocket> sockets, byte[] data, WebSocketMessageType messageType = WebSocketMessageType.Text)
        {
            foreach(var s in sockets)
            {
                if(s.State == WebSocketState.Open)
                    await SendMessageAsync(s, data, messageType);
            }
        }

        public async Task SendMessageToWebSocketsAsync(IEnumerable<WebSocket> sockets, string message)
        {
            foreach(var s in sockets)
            {
                if(s.State == WebSocketState.Open)
                    await SendMessageAsync(s, message);
            }
        }

        public virtual async Task ReceiveAsync(WebSocket socket, WebSocketReceiveResult result, byte[] buffer)
        {
            await this._protocolHandler.HandleMessage(socket, result, buffer, this);
        }
    }
}