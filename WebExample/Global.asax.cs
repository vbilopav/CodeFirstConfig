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
                    SaveConfigFile = false
                }).ContinueWith(result =>
                {
                    if (result.Exception == null)
                        Log.Info("Finished ConfigureAsync");
                    else
                        Log.Info($"Finished ConfigureAsync with errors: {result.Exception}");
                });            
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
