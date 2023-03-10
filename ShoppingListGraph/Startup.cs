using Microsoft.Owin;
using Owin;


[assembly: OwinStartup(typeof(GasGraph.Startup))]

namespace GasGraph
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
             ConfigureAuth(app);
        }
    }
}
