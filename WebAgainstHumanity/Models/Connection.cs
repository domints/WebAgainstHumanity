using System.Collections.Generic;
using System.Net.WebSockets;
using WebAgainstHumanity.Models.Db;

namespace WebAgainstHumanity.Models
{
    public enum ConnectionLocation
    {
        Lobby = 0,
        GameRoom = 1

    }

    public class Connection
    {
        public Connection()
        {
            Room = new List<string>();
        }
        
        public string Id { get; set; }
        public WebSocket Socket { get; set; }
        public ConnectionLocation Location { get; set; }
        public Session Session { get; set; }
        public List<string> Room { get; set; }
    }
}