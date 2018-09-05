using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Baicao.Startup))]
namespace Baicao
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
