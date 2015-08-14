using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace CodeFirstConfig.Tests
{
    public enum MyEnum { Enum1, Enum2, Enum3 }

    public class MyConfigurableModule : ConfigManager<MyConfigurableModule>
    {
        public string Value1 { get; set; }
        public int Value2 { get; set; }
        public int? Value3 { get; set; }
        public DateTime Value4 { get; set; }
        public DateTime? Value5 { get; set; }
        public MyEnum Value6 { get; set; }

        public MyConfigurableModule()
        {
            this.Value1 = "Value1";
            this.Value2 = 2;
            this.Value3 = 3;
            this.Value4 = DateTime.Now.AddYears(1);
            this.Value5 = DateTime.Now.AddYears(10);
            this.Value6 = MyEnum.Enum1;
        }
    }

    [TestClass]
    public partial class ExamplesWikiPage
    {
        static ExamplesWikiPage()
        {
            //
            //  By defualt, exceptions are not thrown! 
            //  Instead, default values are used and exception is accessable via Configurator.Exceptions
            //
            //  For this tests, throw exceptions...
            //  
            ConfigSettings.Instance.ThrowOnConfigureException = true;
        }

        /// <summary>
        /// Before running set appSettings config section to point to file "_BasicExample.config":
        /// <appSettings configSource="_BasicExample.config"></appSettings>
        /// </summary>
        [TestMethod]
        public void BasicExample()
        {            
            Assert.AreEqual("Configured1", MyConfigurableModule.Config.Value1);
            Assert.AreEqual(-2, MyConfigurableModule.Config.Value2);
            Assert.AreEqual(null, MyConfigurableModule.Config.Value3);
            Assert.AreEqual(new DateTime(1977, 5, 19), MyConfigurableModule.Config.Value4);
            Assert.AreEqual(null, MyConfigurableModule.Config.Value5);
            Assert.AreEqual(MyEnum.Enum2, MyConfigurableModule.Config.Value6);
        }        
    }
}
