using System;
using System.IO;
using System.Linq;
using System.Web;

namespace CodeFirstConfig
{
    public class AppConfigManager : ConfigManager<App> { }

    [ConfigComment("CodeFirstConfig application settings")]
    [ConfigKeySerializeLevel(1)]
    public sealed class App
    {
        private string _id;
        public static App Config => AppConfigManager.Config;
        public static string InstanceHash { get; }

        [ConfigComment("Web context enabled?")]
        public bool IsWebApp { get; }

        [ConfigComment("Debug configuration build?")]
        public bool IsDebugConfiguration { get; }

        [ConfigComment("Debugging session instance?")]
        public bool Debugging { get; }

        [ConfigComment("Test session instance?")]
        public bool Testing { get; }

        [ConfigComment("Test session instance?")]
        public string BinFolder { get; }

        [ConfigComment("Application version. Default is entry assembly version.")]
        public string Version { get; }

        [ConfigComment("Application name. Default is entry assembly name or project folder name.")]
        public string Name { get; set; }

        [ConfigComment("Application id. Default is application name.")]
        public string Id
        {
            get { return _id; }
            set { _id = value; InstanceId = string.Concat(_id, InstanceHash).ToLower(); }
        }

        [ConfigComment("Application running instance id. Default is application id with unique hash.")]
        public string InstanceId { get; set; }

        [ConfigComment("Application folder. Default is AppDomain base directory.")]
        public string Folder { get; set; }

        [ConfigComment("Applicatio data folder. Default is application folder Data or App_Data in web context.")]
        public string DataFolder { get; set; }

        static App()
        {
            InstanceHash = Guid.NewGuid().ToString().Substring(0, 4);
        }

        public App()
        {
            IsWebApp = HttpRuntime.AppDomainId != null;
            Folder = AppDomain.CurrentDomain.BaseDirectory;
            if (AppAssembly.Assembly != null)
            {
                var appName = AppAssembly.Assembly.GetName();
                Name = appName.Name;
                Version = appName.Version.ToString();
            }
            else
            {
                var f = Folder.Split(new[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = f.Length - 1; i >= 0; i--)
                {
                    if (string.IsNullOrEmpty(f[i]) || string.Equals(f[i], "bin") || string.Equals(f[i], "Debug") || string.Equals(f[i], "Release"))
                        continue;
                    Name = f[i];
                    break;
                }
            }
            if (Name != null)
            {
                if (Name.Contains('.'))
                {
                    var s = Name.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                    Id = s[s.Length - 1];
                }
                else
                    Id = Name;
            }
            DataFolder = Path.Combine(Folder, IsWebApp ? "App_Data" : "Data");
            //if (!Directory.Exists(DataFolder)) Directory.CreateDirectory(DataFolder);
#if DEBUG
            IsDebugConfiguration = true;
#else
            IsDebugConfiguration = false; 
#endif
            Debugging = System.Diagnostics.Debugger.IsAttached;
            Testing = AppDomain.CurrentDomain.GetAssemblies().Any(a => a.FullName.StartsWith("Microsoft.VisualStudio.QualityTools.UnitTestFramework"));
            BinFolder = AppDomain.CurrentDomain.RelativeSearchPath ?? AppDomain.CurrentDomain.BaseDirectory;
        }
    }
}