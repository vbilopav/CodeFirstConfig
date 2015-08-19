using CodeFirstConfig;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace WebExample
{
    public class MvcApplication : System.Web.HttpApplication
    {
        static MvcApplication()
        {          
            Configurator.ConfigureAsync(
                new ConfigSettings
                {
                    OnError = args => Log.Error(args.Exception),
                    OnModelConfigured = args => Log.Info($"Reconfigured {args.Type}"),
                    SaveConfigFile = true
                }).ContinueWith(r => Log.Info("Finished ConfigureAsync"));
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
