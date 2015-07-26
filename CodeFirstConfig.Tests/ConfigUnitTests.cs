using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
/*
namespace Infrastructure.Tests
{
    using CodeFirstConfig;

    [TestClass]
    public class ConfigUnitTests
    {


       

    reEqual("StringRequiredValueFromConfig", config.StringRequiredValueConfig);
        }
        

        [TestMethod]
        public void TestMethod7()
        {
            Configurator.OnAfterSet = args =>
                Log.Info("Key '{0}' with name '{1}' is set to '{2}' in namespace '{3}'", new[] { args.Key, args.Name, args.Value, args.Namespace });
            Configurator.OnBeforeSet = args =>
                Log.Info("Key '{0}' with name '{1}' is set to '{2}' in namespace '{3}'", new[] { args.Key, args.Name, args.Value, args.Namespace });

            Configurator.Configure(new SqlConfigOptions());
        }

        [TestMethod]
        public void TestMethod7_Async()
        {
            Configurator.OnAfterSet = args =>
                Log.Info("Key '{0}' with name '{1}' is set to '{2}' in namespace '{3}'", new[] { args.Key, args.Name, args.Value, args.Namespace });
            Configurator.OnBeforeSet = args =>
                Log.Info("Key '{0}' with name '{1}' is set to '{2}' in namespace '{3}'", new[] { args.Key, args.Name, args.Value, args.Namespace });

            Configurator.ConfigureAsync().ContinueWith(t =>
            {
                var e = t.Exception;
            }).Wait();            
        }

        [TestMethod]
        public void TestMethod8()
        {
            Configurator.OnAfterSet = args =>
              Log.Info("Key '{0}' with name '{1}' is set to '{2}' in namespace '{3}'", new[] { args.Key, args.Name, args.Value, args.Namespace });
            Configurator.OnBeforeSet = args =>
                Log.Info("Key '{0}' with name '{1}' is set to '{2}' in namespace '{3}'", new[] { args.Key, args.Name, args.Value, args.Namespace });

            Configurator.Configure();

            Assert.AreEqual("Infrastructure.Tests", App.Config.Name);
        }


        
        public class TestSqlClientConfig
        {
            public string StringValue = "StringValueFromCode";
            public string StringValueConfig = "StringValueFromCode"; //StringValueFromCONFIG
            public string StringValueFromDb = "StringValueFromCode"; //StringValueFromDb
            public string StringValueFromDbOverConfig = "StringValueFromCode"; //StringValueFromDbOverConfig            
        }

        public class TestSqlClientConfigManager : ConfigManager<TestSqlClientConfig> { }

        [TestMethod]
        public void TestMethod9()
        {
            Configurator.SetDatabaseOptions(new SqlConfigOptions());
            Assert.AreEqual("StringValueFromDbOverConfig", TestSqlClientConfigManager.Config.StringValueFromDbOverConfig);
        }

    }
}
*/