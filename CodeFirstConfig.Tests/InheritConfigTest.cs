using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CodeFirstConfig.Tests
{
    [TestClass]
    public class InheritConfigTest
    {
        public class TestClass : ConfigManager<TestClass>
        {
            public string Value1 = "MyValueFromCode1";
            public string Value2 = "MyValueFromCode2";
        }

        public class TestClass2 : TestClass
        {
            private abstract class TestInheritConfig2Manager : ConfigManager<TestClass2> { }
            public static new TestClass2 Config { get { return TestInheritConfig2Manager.Config; } }

            public string Value3 = "MyValueFromCode3";
            public string Value4 = "MyValueFromCode4";
        }

        [TestMethod]
        public void TestMethod1()
        {            
            Assert.AreEqual("MyValueFromCode1", TestClass.Config.Value1);
            Assert.AreEqual("MyValueFromConfig2", TestClass.Config.Value2);

            Assert.AreEqual("MyValueFromCode1", TestClass2.Config.Value1);
            Assert.AreEqual("MyValueFromConfig2", TestClass2.Config.Value2);
            Assert.AreEqual("MyValueFromConfig3", TestClass2.Config.Value3);
            Assert.AreEqual("MyValueFromCode4", TestClass2.Config.Value4);
        }
    }
}
