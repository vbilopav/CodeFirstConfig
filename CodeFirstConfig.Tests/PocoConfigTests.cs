using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CodeFirstConfig.Tests
{
    [TestClass]
    public class PocoConfigTests
    {
        public class TestComplexClass { public string Field1 = "Field1"; public string Field2 = "Field2"; }        
        public enum TestEnum { Enum1, Enum2, Enum3 }

        public class TestClass
        {
            public string Value1 { get; set; } = "MyValueFromCode1";
            public string Value2 { get; set; } = "MyValueFromCode2";
            public string Value3 { get; set; } = "MyValueFromCode3";
            public string Value4 { get; set; } = "MyValueFromCode4";
            public string Value5 { get; set; } = "MyValueFromCode5";
            public string Value6 { get; set; } = "MyValueFromCode6";
            public string Value7 { get; set; } = "MyValueFromCode7";
            public TestEnum EnumValue { get; set; } = TestEnum.Enum1;
            public TestEnum EnumConfig { get; set; } = TestEnum.Enum2; //Enum3
            public DateTime DateTimeValue { get; set; } = new DateTime(2015, 1, 1);
            public DateTime DateTimeValueConfig { get; set; } = new DateTime(2015, 1, 1); //20.7.2015
            public bool BoolValue { get; set; } = true;
            public bool BoolValueConfig { get; set; } = true; //false           
            public TestComplexClass ComplexClassConfig { get; set; } //Field1 = "Field1CONFIG", Field2 = "Field2CONFIG" 
            public string[] StringArray { get; set; } //config1, config2
        }

        public class PocoConfigManager : ConfigManager<TestClass> { }

        [TestMethod]
        public void TestMethod1()
        {
            Assert.AreEqual("MyValueFromCode1", PocoConfigManager.Config.Value1);
            Assert.AreEqual("MyValueFromConfig2", PocoConfigManager.Config.Value2);
            Assert.AreEqual("MyValueFromConfig3", PocoConfigManager.Config.Value3);
            Assert.AreEqual("MyValueFromConfig4", PocoConfigManager.Config.Value4);
            Assert.AreEqual("MyValueFromConfig5", PocoConfigManager.Config.Value5);
            Assert.AreEqual("MyValueFromConfig6", PocoConfigManager.Config.Value6);
            Assert.AreEqual("MyValueFromConfig7_5", PocoConfigManager.Config.Value7);

            Assert.AreEqual(TestEnum.Enum1, PocoConfigManager.Config.EnumValue);
            Assert.AreEqual(TestEnum.Enum3, PocoConfigManager.Config.EnumConfig);
            Assert.AreEqual(new DateTime(2015, 1, 1), PocoConfigManager.Config.DateTimeValue);
            Assert.AreEqual(new DateTime(2015, 7, 20), PocoConfigManager.Config.DateTimeValueConfig);
            Assert.AreEqual(true, PocoConfigManager.Config.BoolValue);
            Assert.AreEqual(false, PocoConfigManager.Config.BoolValueConfig);
            Assert.AreEqual("Field1CONFIG", PocoConfigManager.Config.ComplexClassConfig.Field1);
            Assert.AreEqual("Field2CONFIG", PocoConfigManager.Config.ComplexClassConfig.Field2);
            Assert.AreEqual(2, PocoConfigManager.Config.StringArray.Length);
            Assert.AreEqual("value1", PocoConfigManager.Config.StringArray[0]);
            Assert.AreEqual("value2", PocoConfigManager.Config.StringArray[1]);        
        }
    }
}
