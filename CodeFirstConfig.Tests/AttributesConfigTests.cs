using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CodeFirstConfig.Tests
{
    [TestClass]
    public class AttributesConfigTests
    {
        public class TestAfterSetClass : ConfigManager<TestAfterSetClass>
        {
            [ConfigSettings(ExecuteAfterSet = true)]
            public string Value1 { get; set; } = "MyValueFromCode1";

            [ConfigSettings(ExecuteAfterSet = true)]
            public string Value2 { get; set; } = "MyValueFromCode2";
        }

        [TestMethod]
        public void TestAfterSet()
        {
            var i = 0;
            string key = "", ns = "";

            Configurator.OnAfterSet = args =>
            {
                Debug.WriteLine(
                    $"Key '{args.Key}' with name '{args.Name}' is set to '{args.Value}' in namespace '{args.Namespace}'");
                i++;
                key = args.Key;
                ns = args.Namespace;
            };
            Assert.AreEqual("MyValueFromCode1", TestAfterSetClass.Config.Value1);
            Assert.AreEqual(1, i);
            Assert.AreEqual("Value2", key);
            Assert.AreEqual("CodeFirstConfig.Tests.AttributesConfigTests.TestAfterSetClass", ns);
        }

        public class TestBeforeSetClass : ConfigManager<TestBeforeSetClass>
        {
            [ConfigSettings(ExecuteBeforeSet = true)]
            public string Value1 { get; set; } = "MyValueFromCode1";

            [ConfigSettings(ExecuteBeforeSet = true)]
            public string Value2 { get; set; } = "MyValueFromCode2";
        }

        [TestMethod]
        public void TestBeforeSet()
        {
            var i = 0;
            string key = "", ns = "";
            Configurator.OnBeforeSet = args =>
            {
                Debug.WriteLine(
                    $"Key '{args.Key}' with name '{args.Name}' is set to '{args.Value}' in namespace '{args.Namespace}'");
                i++;
                key = args.Key;
                ns = args.Namespace;
            };
            Assert.AreEqual("MyValueFromCode1", TestBeforeSetClass.Config.Value1);
            Assert.AreEqual(1, i);
            Assert.AreEqual("Value2", key);
            Assert.AreEqual("CodeFirstConfig.Tests.AttributesConfigTests.TestBeforeSetClass", ns);
        }


        public class TestBeforeAndAfterSetClass : ConfigManager<TestBeforeAndAfterSetClass>
        {
            [ConfigSettings(ExecuteBeforeSet = true, ExecuteAfterSet = true)]
            public string Value1 { get; set; } = "MyValueFromCode1";

            [ConfigSettings(ExecuteBeforeSet = true, ExecuteAfterSet = true)]
            public string Value2 { get; set; } = "MyValueFromCode2";
        }

        [TestMethod]
        public void TestBeforeAndAfterSet()
        {
            int i1 = 0, i2 = 0;
            string key1 = "", key2 = "", ns1 = "", ns2 = "";

            Configurator.OnBeforeSet = args =>
            {
                Debug.WriteLine(
                    $"Key '{args.Key}' with name '{args.Name}' is set to '{args.Value}' in namespace '{args.Namespace}'");
                i1++;
                key1 = args.Key;
                ns1 = args.Namespace;
            };
            Configurator.OnAfterSet = args =>
            {
                Debug.WriteLine(
                    $"Key '{args.Key}' with name '{args.Name}' is set to '{args.Value}' in namespace '{args.Namespace}'");
                i2++;
                key2 = args.Key;
                ns2 = args.Namespace;
            };
            Assert.AreEqual("MyValueFromCode1", TestBeforeAndAfterSetClass.Config.Value1);
            Assert.AreEqual(1, i1);
            Assert.AreEqual("Value2", key1);
            Assert.AreEqual("CodeFirstConfig.Tests.AttributesConfigTests.TestBeforeAndAfterSetClass", ns1);
            Assert.AreEqual(1, i2);
            Assert.AreEqual("Value2", key2);
            Assert.AreEqual("CodeFirstConfig.Tests.AttributesConfigTests.TestBeforeAndAfterSetClass", ns2);
        }


        public class TestCancelClass : ConfigManager<TestCancelClass>
        {
            [ConfigSettings(ExecuteBeforeSet = true)]
            public string Value1 { get; set; } = "MyValueFromCode1";

            [ConfigSettings(ExecuteBeforeSet = true)]
            public string Value2 { get; set; } = "MyValueFromCode2";
        }

        [TestMethod]
        public void TestCancel()
        {
            Configurator.OnBeforeSet = args =>
            {
                if (args.Key == "Value2")
                    args.Cancel = true;
            };
            Assert.AreEqual("MyValueFromCode2", TestCancelClass.Config.Value2);
        }

        public class TestRequiredClass
        {           
            [ConfigSettings(Required = true)] public string Value1 = "MyValueFromCode1";
        }

        [TestMethod]
        public void TestRequired()
        {
            try
            {
                Assert.AreEqual("MyValueFromCode1", ConfigManager<TestRequiredClass>.Config.Value1);
            }
            catch (Exception exception)
            {                
                Assert.IsInstanceOfType(exception, typeof(CodeFirstConfigException));    
                Debug.WriteLine(exception.Message);            
            }            
        }
    }
}
