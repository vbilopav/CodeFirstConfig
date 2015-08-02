using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Http;
using CodeFirstConfig;
#pragma warning disable 4014

namespace WebExample
{
    public class MvcApplication : System.Web.HttpApplication
    {
        static MvcApplication()
        {            
            Configurator.OnModelConfigured = args =>
            {
                Log.Info($"Configured {args.Type}");
            };  
            Configurator.ConfigureAsync();          
        }

        protected void Application_Start()
        {
            Log.Info("Application_Start");
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);           
            RouteConfig.RegisterRoutes(RouteTable.Routes);            
        }

        protected void Application_End()
        {
            Log.Info("Application_End");
        }

        protected void Application_Error()
        {
            Log.Error(Server.GetLastError());
        }
    }
}
