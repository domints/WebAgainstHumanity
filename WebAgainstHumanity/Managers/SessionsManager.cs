using System;
using System.Collections.Concurrent;
using WebAgainstHumanity.Models;
using WebAgainstHumanity.Models.Db;

namespace WebAgainstHumanity.Managers
{
    public interface ISessionManager
    {
        string AddSession(string browserFootprint);
        bool AddUserToSession(string id, User user);
        Session GetSession(string id, int timeout = 3600, bool amendTime = true);
        bool DropSession(string id);
    }

    public class SessionsManager : ISessionManager
    {
        private ConcurrentDictionary<string, Session> _sessions  = new ConcurrentDictionary<string, Session>();

        public string AddSession(string browserFootprint)
        {
            string id = GetNewSessionId();
            return _sessions.TryAdd(id, new Session
            {
                Id = id,
                CreationDate = DateTime.Now,
                BrowserFootprint = browserFootprint
            }) ? id : null;
        }

        public bool AddUserToSession(string id, User user)
        {
            Session sess;
            bool success = _sessions.TryGetValue(id, out sess);
            if(!success) return false;

            sess.User = user;
            return true;
        }

        public bool DropSession(string id)
        {
            Session sess;
            return _sessions.TryRemove(id, out sess);
        }

        public Session GetSession(string id, int timeout = 3600, bool amendTime = true)
        {
            Session sess;
            bool success = _sessions.TryGetValue(id, out sess);
            if(!success) return null;
            if((DateTime.Now - sess.LastConnection).Seconds > timeout)
            {
                DropSession(id);
            }

            sess.LastConnection = DateTime.Now;
            return sess;
        }

        private string GetNewSessionId()
        {
            return Guid.NewGuid().ToString();
        }
    }
}