using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
/*
namespace Infrastructure.Tests
{
    using CodeFirstConfig;

    [TestClass]
    public class ConfigUnitTests
    {


       

        /*
        public class TestAttributesConfig
        {
            [ConfigSettings(ExecuteAfterSet = true)]
            public string StringValue = "StringValueFromCode"; //StringValueFromConfig

            [ConfigSettings(ExecuteAfterSet = true, Required = true)]
            public string StringRequiredValueConfig = "StringRequiredValueFromCode"; //StringRequiredValueFromConfig
        }

        public class TestAttributesConfigManager : ConfigManager<TestAttributesConfig> { }
        */

        //[TestMethod]
        //public void TestMethod6()
        //{
        //    string v = "";
        //    Configurator.OnAfterSet = args =>
        //    {
        //        Log.Info("Key '{0}' with name '{1}' is set to '{2}' in namespace '{3}'", new[] { args.Key, args.Name, args.Value, args.Namespace });
        //        v = args.Value.ToString();
        //    };

        //    var config = TestAttributesConfigManager.Config; //init
        //    Assert.AreEqual("StringRequiredValueFromConfig", config.StringRequiredValueConfig);
        //}

        /*
        public class TestAttributesConfig_a
        {
            [ConfigSettings(ExecuteBeforeSet = true)]
            public string StringValue = "StringValueFromCode"; //StringValueFromConfig

            [ConfigSettings(ExecuteBeforeSet = true, Required = true)]
            public string StringRequiredValueConfig = "StringRequiredValueFromCode"; //StringRequiredValueFromConfig
        }

        public class TestAttributesConfigManager_a : ConfigManager<TestAttributesConfig_a> { }


        [TestMethod]
        public void TestMethod6_a()
        {           
            Configurator.OnBeforeSet = args =>
            {
                if (string.Equals(args.Value, "StringValueFromConfig")) 
                    args.Cancel = true;
            };

            var config = TestAttributesConfigManager_a.Config; //init
            Assert.AreEqual("StringRequiredValueFromConfig", config.StringRequiredValueConfig);
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