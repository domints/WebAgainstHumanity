using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using WebAgainstHumanity.Models;
using WebAgainstHumanity.Sockets;

namespace WebAgainstHumanity.Middleware
{
    public interface IWebSocketManagerMiddleware
    {
        Task ProcessRequest(HttpContext context, Func<Task> next);
    }
    public class WebSocketManagerMiddleware : IWebSocketManagerMiddleware
    {
        private WebSocketHandler _webSocketHandler { get; set; }

        public WebSocketManagerMiddleware(WebSocketHandler webSocketHandler)
        {
            _webSocketHandler = webSocketHandler;
        }

        public async Task ProcessRequest(HttpContext context, Func<Task> next)
        {
            if (!context.WebSockets.IsWebSocketRequest)
            {
                await next();
            }
            else
            {
                var socket = await context.WebSockets.AcceptWebSocketAsync();
                var path = context.Request.Path.HasValue ? context.Request.Path.Value.Split('/') : new string[0];
                bool isRoom = path.Length > 1;
                string roomId = isRoom ? path[2] : string.Empty;

                await _webSocketHandler.OnConnected(
                    socket,
                    (Session)context.Items[SessionMiddleware.SessionIdKey],
                    isRoom ? ConnectionLocation.GameRoom : ConnectionLocation.Lobby,
                    roomId);

                await Receive(socket, async (result, buffer) =>
                {
                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        await _webSocketHandler.ReceiveAsync(socket, result, buffer);
                        return;
                    }

                    else if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await _webSocketHandler.OnDisconnected(socket);
                        return;
                    }

                });
            }
        }

        private async Task Receive(WebSocket socket, Action<WebSocketReceiveResult, byte[]> handleMessage)
        {
            var buffer = new byte[1024 * 4];

            while (socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(buffer: new ArraySegment<byte>(buffer),
                                                       cancellationToken: CancellationToken.None);

                handleMessage(result, buffer);
            }
        }
    }
}