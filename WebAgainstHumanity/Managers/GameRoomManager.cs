using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using WebAgainstHumanity.Models;

namespace WebAgainstHumanity.Managers
{
    public interface IGameRoomManager
    {
        List<GameRoom> GetAllRooms();
        string AddRoom(GameRoom room);
        GameRoom GetById(string id);
        void RemoveRoom(string id);
    }

    public class GameRoomManager : IGameRoomManager
    {
        private ConcurrentDictionary<string, GameRoom> _rooms = new ConcurrentDictionary<string, GameRoom>();

        public string AddRoom(GameRoom room)
        {
            var id = GetNewId();
            room.Id = id;
            _rooms.TryAdd(id, room);
            return id;
        }

        public List<GameRoom> GetAllRooms()
        {
            return _rooms.Select(r => r.Value).ToList();
        }

        public GameRoom GetById(string id)
        {
            GameRoom gr;
            bool result = _rooms.TryGetValue(id, out gr);
            if(!result) return null;
            return gr;
        }

        public void RemoveRoom(string id)
        {
            GameRoom gr;
            _rooms.TryRemove(id, out gr);
        }

        private string GetNewId()
        {
            return Guid.NewGuid().ToString();
        }
    }
}