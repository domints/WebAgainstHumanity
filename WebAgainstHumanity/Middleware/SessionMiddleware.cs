using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using WebAgainstHumanity.Managers;

namespace WebAgainstHumanity.Middleware
{
    
    public interface ISessionMiddleware
    {
        Task ProcessRequest(HttpContext context, Func<Task> next);
    }

    public class SessionMiddleware : ISessionMiddleware
    {
        public const string SessionIdKey = "SessionId";
        ISessionManager _sessionManager;
        public SessionMiddleware(ISessionManager sessionManager)
        {
            this._sessionManager = sessionManager;
        }
        public async Task ProcessRequest(HttpContext context, Func<Task> next)
        {
            var id = context.Session.GetString(SessionIdKey);
            if(string.IsNullOrWhiteSpace(id))
            {
                id = _sessionManager.AddSession(null);
                context.Session.SetString(SessionIdKey, id);
            }
            context.Items.Add(SessionIdKey, _sessionManager.GetSession(id));

            await next();
        }
    }
}