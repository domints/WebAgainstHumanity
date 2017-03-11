using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WebAgainstHumanity.Managers;
using WebAgainstHumanity.Middleware;
using WebAgainstHumanity.Models.Db;
using WebAgainstHumanity.Sockets;

namespace WebAgainstHumanity
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();
            var connectionString = Configuration["Data:Conn"];
            services.AddDbContext<WahContext>(
                opts => opts.UseNpgsql(connectionString)
            );
            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.CookieName = ".WebAgainstHumanity.Session";
                options.IdleTimeout = TimeSpan.FromSeconds(7200);
                options.CookieHttpOnly = true;
            });
            services.AddSingleton(typeof(IGameRoomManager), typeof(GameRoomManager));
            services.AddSingleton(typeof(IConnectionManager), typeof(WSConnectionManager));
            services.AddSingleton(typeof(ISessionManager), typeof(SessionsManager));
            services.AddTransient(typeof(ISessionMiddleware), typeof(SessionMiddleware));
            services.AddTransient(typeof(IWebSocketManagerMiddleware), typeof(WebSocketManagerMiddleware));
            services.AddScoped(typeof(WebSocketHandler));
            services.AddTransient<IProtocolHandler, ProtocolHandler>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseSession();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.Use(async (http, next) => 
            {
                var sessionMiddleware = http.RequestServices.GetService<ISessionMiddleware>();
                await sessionMiddleware.ProcessRequest(http, next);
            });
            app.UseWebSockets();
            app.Use(async (http, next) =>
            {
                var wsMiddleWare = http.RequestServices.GetService<IWebSocketManagerMiddleware>();
                await wsMiddleWare.ProcessRequest(http, next);
            });
            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
