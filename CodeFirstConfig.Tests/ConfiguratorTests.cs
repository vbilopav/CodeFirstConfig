using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CodeFirstConfig.Tests
{
    [TestClass]
    public class ConfiguratorTests
    {       
        [TestMethod]
        public void ConfigureDefault()
        {
            Configurator.OnBeforeSet = args => { };
            Configurator.OnAfterSet = args => { };

            Configurator.Configure();

            if (ConfigSettings.Instance.SaveConfigFile)
                Debug.WriteLine(File.ReadAllText(ConfigSettings.Instance.ConfigFileName));
        }

        [TestMethod]
        public void ConfigureToJson()
        {
            Configurator.OnBeforeSet = args => { };
            Configurator.OnAfterSet = args => { };

            Configurator.Configure(new ConfigSettings{ ConfigFileFormat = ConfigFormat.Json});

            if (ConfigSettings.Instance.SaveConfigFile)
                Debug.WriteLine(File.ReadAllText(ConfigSettings.Instance.ConfigFileName));
        }

        [TestMethod]
        public void ConfigureDefaultAsync()
        {
            Configurator.OnBeforeSet = args => { };
            Configurator.OnAfterSet = args => { };

            Configurator.ConfigureAsync().Wait();
   
            if (ConfigSettings.Instance.SaveConfigFile)
                Debug.WriteLine(File.ReadAllText(ConfigSettings.Instance.ConfigFileName));
        }
    }
}
