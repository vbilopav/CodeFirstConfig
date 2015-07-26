﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CodeFirstConfig.Tests
{
    [TestClass]
    public class CompositConfigTest
    {
        public class TestComplexClass { public string Field1 = "Field1"; public string Field2 = "Field2"; }        
        public enum TestEnum { Enum1, Enum2, Enum3 }

        public class TestClass
        {
            //will NOT be written to config file!
            public static TestClass Config => ConfigManager<TestClass>.Config;

            public string Value1 = "MyValueFromCode1";
            public string Value2 = "MyValueFromCode2";
            public string Value3 = "MyValueFromCode3";
            public string Value4 = "MyValueFromCode4";
            public string Value5 = "MyValueFromCode5";
            public string Value6 = "MyValueFromCode6";
            public string Value7 = "MyValueFromCode7";
            public TestEnum EnumValue = TestEnum.Enum1;
            public TestEnum EnumConfig = TestEnum.Enum2; //Enum3
            public DateTime DateTimeValue = new DateTime(2015, 1, 1);
            public DateTime DateTimeValueConfig = new DateTime(2015, 1, 1); //20.7.2015
            public bool BoolValue = true;
            public bool BoolValueConfig = true; //false           
            public TestComplexClass ComplexClassConfig; //Field1 = "Field1CONFIG", Field2 = "Field2CONFIG" 
            public string[] StringArray { get; set; } //config1, config2        
        }


        [TestMethod]
        public void TestMethod1()
        {
            Assert.AreEqual("MyValueFromCode1", TestClass.Config.Value1);
            Assert.AreEqual("MyValueFromConfig2", TestClass.Config.Value2);
            Assert.AreEqual("MyValueFromConfig3", TestClass.Config.Value3);
            Assert.AreEqual("MyValueFromCode4", TestClass.Config.Value4); // different class name
            Assert.AreEqual("MyValueFromCode5", TestClass.Config.Value5); // different class name
            Assert.AreEqual("MyValueFromCode6", TestClass.Config.Value6); // different class name
            Assert.AreEqual("MyValueFromConfig7_2", TestClass.Config.Value7); // different class name

            Assert.AreEqual(TestEnum.Enum1, TestClass.Config.EnumValue);
            Assert.AreEqual(TestEnum.Enum3, TestClass.Config.EnumConfig);
            Assert.AreEqual(new DateTime(2015, 1, 1), TestClass.Config.DateTimeValue);
            Assert.AreEqual(new DateTime(2015, 7, 20), TestClass.Config.DateTimeValueConfig);
            Assert.AreEqual(true, TestClass.Config.BoolValue);
            Assert.AreEqual(false, TestClass.Config.BoolValueConfig);
            Assert.AreEqual("Field1CONFIG", TestClass.Config.ComplexClassConfig.Field1);
            Assert.AreEqual("Field2CONFIG", TestClass.Config.ComplexClassConfig.Field2);
            Assert.AreEqual(2, TestClass.Config.StringArray.Length);
            Assert.AreEqual("value1", TestClass.Config.StringArray[0]);
            Assert.AreEqual("value2", TestClass.Config.StringArray[1]);    
        }
    }
}
