using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CodeFirstConfig.Tests
{
    [TestClass]
    public class CompositConfigTest
    {
        public class TestComplexClass { public string Field1 = "Field1"; public string Field2 = "Field2"; }        
        public enum TestEnum { Enum1, Enum2, Enum3 }

        public class TestClass : ConfigManager<TestClass>
        {
            [ConfigSettings(ExecuteAfterSet = true, ExecuteBeforeSet = true)]
            public string Value1 = "MyValueFromCode1";

            [ConfigSettings(ExecuteAfterSet = true, ExecuteBeforeSet = true)]
            public string Value2 = "MyValueFromCode2";

            [ConfigSettings(ExecuteAfterSet = true, ExecuteBeforeSet = true)]
            public string Value3 = "MyValueFromCode3";

            public string Value4 = "MyValueFromCode4";
            public string Value5 = "MyValueFromCode5";
            public string Value6 = "MyValueFromCode6";

            [ConfigSettings(ExecuteAfterSet = true, ExecuteBeforeSet = true)]
            public string Value7 = "MyValueFromCode7";

            [ConfigSettings(Required = false)]
            public TestEnum EnumValue = TestEnum.Enum1;

            [ConfigSettings(Required = true)]
            public TestEnum EnumConfig = TestEnum.Enum2;     
        }


        [TestMethod]
        public void TestMethod1()
        {
            Configurator.OnAfterSet = args =>
            {
                //Log.Info("Key '{0}' with name '{1}' is set to '{2}' in namespace '{3}'", new[] { args.Key, args.Name, args.Value, args.Namespace });
                //v = args.Value.ToString();
            };


            Assert.AreEqual("MyValueFromCode1", TestClass.Config.Value1);
            Assert.AreEqual("MyValueFromConfig2", TestClass.Config.Value2);
            Assert.AreEqual("MyValueFromConfig3", TestClass.Config.Value3);
            Assert.AreEqual("MyValueFromCode4", TestClass.Config.Value4); // different class name
            Assert.AreEqual("MyValueFromCode5", TestClass.Config.Value5); // different class name
            Assert.AreEqual("MyValueFromCode6", TestClass.Config.Value6); // different class name
            Assert.AreEqual("MyValueFromConfig7_2", TestClass.Config.Value7); // different class name

            Assert.AreEqual(TestEnum.Enum1, TestClass.Config.EnumValue);
            Assert.AreEqual(TestEnum.Enum3, TestClass.Config.EnumConfig);
        }
    }
}
