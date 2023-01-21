using Owin;
using Microsoft.Owin;
[assembly: OwinStartup(typeof(DCS_new.Startup))]
namespace DCS_new
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Any connection or hub wire up and configuration should go here
            app.MapSignalR();
        }
    }
}