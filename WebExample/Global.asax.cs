using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Http;
using CodeFirstConfig;

namespace WebExample
{
    public class MvcApplication : System.Web.HttpApplication
    {
        static MvcApplication()
        {
            Configurator.Configure(
                new ConfigSettings
                {
                    OnError = args => Log.Error(args.Exception),
                    OnModelConfigured = args => Log.Info($"Configured {args.Type}")
                });
            Log.Info("Finished ConfigureAsync");           
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
            AppFinalizator.PerformCleanup();
            Log.Info("Application_End");
        }

        protected void Application_Error()
        {
            Log.Error(Server.GetLastError());
        }
    }
}
