using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;

namespace CodeFirstConfig
{
    public class AppConfigManager : ConfigManager<App> { }

    [ConfigComment("CodeFirstConfig application settings")]
    [ConfigKeySerializeLevel(1)]
    public sealed class App
    {
        public static App Config => AppConfigManager.Config;
        public static Assembly Assembly => AppAssembly.Assembly;
        public static string InstanceHash { get; }
        public static bool IsWebApp { get; }     
        public static bool IsDebugConfiguration { get; }       
        public static bool Debugging { get; }       
        public static bool Testing { get; }      
        public static string BinFolder { get; }        
                       
        public string Name { get; set; }       
        public string Id { get; set; }      
        public string InstanceId { get; set; }       
        public string Folder { get; set; }       
        public string DataFolder { get; set; }
        public string Version { get; set; }

        static App()
        {
            InstanceHash = Guid.NewGuid().ToString().Substring(0, 4);
            IsWebApp = HttpRuntime.AppDomainId != null;
#if DEBUG
            IsDebugConfiguration = true;
#else
            IsDebugConfiguration = false; 
#endif
            Debugging = System.Diagnostics.Debugger.IsAttached;
            Testing = AppDomain.CurrentDomain.GetAssemblies().Any(a => a.FullName.StartsWith("Microsoft.VisualStudio.QualityTools.UnitTestFramework"));
            BinFolder = AppDomain.CurrentDomain.RelativeSearchPath ?? AppDomain.CurrentDomain.BaseDirectory;
        }

        public App()
        {
            Folder = AppDomain.CurrentDomain.BaseDirectory;
            if (IsWebApp && ConfigSettings.Instance.ConfigFileName.Contains(".\\"))
                ConfigSettings.Instance.ConfigFileName = ConfigSettings.Instance.ConfigFileName.Replace(".\\", Folder);
            if (AppAssembly.Assembly != null)
            {
                var appName = AppAssembly.Assembly.GetName();
                Name = appName.Name;
                Version = appName.Version.ToString();
            }
            else
            {
                var f = Folder.Split(new[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
                for (var i = f.Length - 1; i >= 0; i--)
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
                InstanceId = string.Concat(Id, InstanceHash);
            }
            DataFolder = Path.Combine(Folder, IsWebApp ? "App_Data" : "Data");
            //if (!Directory.Exists(DataFolder)) Directory.CreateDirectory(DataFolder);
        }
    }
}