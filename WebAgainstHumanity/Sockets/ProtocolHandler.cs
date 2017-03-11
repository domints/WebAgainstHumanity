using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace WebAgainstHumanity.Sockets
{
    public interface IProtocolHandler
    {
        Task HandleMessage(WebSocket socket, WebSocketReceiveResult result, byte[] buffer, WebSocketHandler wsHandler);
    }

    public enum ProtocolCommand
    {
        Chat,
        ChatToChannel,
        ChatToAll,

        Unknown,
    }

    public class ProtocolHandler : IProtocolHandler
    {
        public Dictionary<string, ProtocolCommand> CommandMapping = new Dictionary<string, ProtocolCommand>
        {
            { "CHAT", ProtocolCommand.Chat },
            { "CHCH", ProtocolCommand.ChatToChannel},
            { "CHAL", ProtocolCommand.ChatToAll }
        };

        public async Task HandleMessage(WebSocket socket, WebSocketReceiveResult result, byte[] buffer, WebSocketHandler wsHandler)
        {
            if (buffer.Length < 4)
            {
                return;
            }

            switch(FetchCommand(buffer))
            {
                case ProtocolCommand.Chat:
                    await wsHandler.SendMessageAsync(socket, GetData(buffer));
                    break;
                case ProtocolCommand.ChatToAll:
                    await wsHandler.SendMessageToAllAsync(GetData(buffer));
                    break;
                case ProtocolCommand.ChatToChannel:
                    await wsHandler.SendMessageToChannelAsync(socket, GetData(buffer));
                    break;
                default:
                    return;
            }
        }

        private byte[] GetData(byte[] buffer)
        {
            return buffer.Skip(4).ToArray();
        }

        private ProtocolCommand FetchCommand(byte[] buffer)
        {
            string commStr = Encoding.UTF8.GetString(buffer, 0, 4);
            if(CommandMapping.ContainsKey(commStr))
            {
                return CommandMapping[commStr];
            }

            return ProtocolCommand.Unknown;
        }
    }
}