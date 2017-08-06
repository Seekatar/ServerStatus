using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServerStatus.Services;

namespace ServerStatus
{
    public class Startup
    {
        const int BUFFER_SIZE = 100;
        const int KEEP_ALIVE_SEC = 120;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IStatusService, StatusService>();
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
                {
                    HotModuleReplacement = true,
                    ReactHotModuleReplacement = true
                });
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            #region WebSocket
            var webSocketOptions = new WebSocketOptions()
            {
                KeepAliveInterval = TimeSpan.FromSeconds(KEEP_ALIVE_SEC),
                ReceiveBufferSize = BUFFER_SIZE
            };
            app.UseWebSockets(webSocketOptions);

            app.Use(async (context, next) =>
            {
                if (context.Request.Path == "/ws")
                {
                    if (context.WebSockets.IsWebSocketRequest)
                    {
                        WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
                        await SendClientUpdates(context, webSocket);
                    }
                    else
                    {
                        context.Response.StatusCode = 400;
                    }
                }
                else
                {
                    await next();
                }

            });
            #endregion

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}");

                routes.MapSpaFallbackRoute(
                    name: "spa-fallback",
                    defaults: new { controller = "Home", action = "Index" });
            });
        }

        #region SendClientUpdates
        private async Task SendClientUpdates(HttpContext context, WebSocket webSocket)
        {
            // must receive to get socket close from client
			var rcvBuffer = new byte[BUFFER_SIZE];
			var rcv = webSocket.ReceiveAsync(new ArraySegment<byte>(rcvBuffer), CancellationToken.None);

			while (webSocket.State == WebSocketState.Open)
			{
				if (rcv.IsCompleted )
				{
					if ( rcv.Status == TaskStatus.RanToCompletion && !rcv.Result.CloseStatus.HasValue )
						rcv = webSocket.ReceiveAsync(new ArraySegment<byte>(rcvBuffer), CancellationToken.None);
				}

				// TESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTEST
				var buffer = System.Text.Encoding.UTF8.GetBytes(DateTime.Now.ToString());
				await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, buffer.Length),
											 WebSocketMessageType.Text,
											 true, // eom
											 CancellationToken.None);

				await Task.Delay(1000);
				// TESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTEST
			}
			await webSocket.CloseAsync(rcv.Result.CloseStatus.Value, rcv.Result.CloseStatusDescription, CancellationToken.None);
		}
        #endregion

    }
}
