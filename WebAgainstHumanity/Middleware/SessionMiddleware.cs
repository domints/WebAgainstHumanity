using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using WebAgainstHumanity.Managers;

namespace WebAgainstHumanity.Middleware
{
    
    public interface ISessionMiddleware
    {
        /// <summary>
        /// Fetches SessionId from HttpContext and adds it to <see cref="context.Items" />
        /// </summary>
        /// <param name="context">Http Context</param>
        /// <param name="next">Next task in chain</param>
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

        /// <summary>
        /// Fetches SessionId from HttpContext and adds it to <see cref="context.Items" />
        /// </summary>
        /// <param name="context">Http Context</param>
        /// <param name="next">Next task in chain</param>
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