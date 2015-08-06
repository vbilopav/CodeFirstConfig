using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CodeFirstConfig.Tests
{
    [TestClass]
    public class InheritConfigTests
    {
        public class TestClass : ConfigManager<TestClass>
        {
            public string Value1 { get; set; } = "MyValueFromCode1";
            public string Value2 { get; set; } = "MyValueFromCode2";
        }

        public class TestClass2 : TestClass
        {
            private abstract class TestInheritConfig2Manager : ConfigManager<TestClass2> { }
            public static new TestClass2 Config => TestInheritConfig2Manager.Config;

            public string Value3 { get; set; } = "MyValueFromCode3";
            public string Value4 { get; set; } = "MyValueFromCode4";
        }

        [TestMethod]
        public void InheritConfig_Test()
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
