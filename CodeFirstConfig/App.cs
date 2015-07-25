using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;

namespace CodeFirstConfig
{
    public class AppConfigManager : ConfigManager<App> { }

    [ConfigComment("Application settings")]
    public sealed class App
    {
        private string _id;
        public static App Config { get { return AppConfigManager.Config; } }
        public static string InstanceHash { get; private set; }

        public bool IsWebApp { get; private set; }
        public bool IsDebugConfiguration { get; private set; }
        public bool Debugging { get; private set; }
        public bool Testing { get; private set; }
        public string BinFolder { get; private set; }
        public string Version { get; private set; }

        [ConfigComment("Usualy entry assembly name without extension or if not available application folder name (!= bin, debug, release).")]
        public string Name { get; set; }

        [ConfigComment("Same as ApplicationName if not set. Also, changes ApplicationId.")]
        public string Id
        {
            get { return _id; }
            set { _id = value; this.InstanceId = string.Concat(_id, InstanceHash).ToLower(); }
        }

        [ConfigComment("Unique instance id. If not set ApplicationId with unique hash.")]
        public string InstanceId { get; set; }

        public string Folder { get; set; }
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
            if (!Directory.Exists(DataFolder)) Directory.CreateDirectory(DataFolder);
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