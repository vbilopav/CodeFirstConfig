using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace CodeFirstConfig.Tests
{
    public class MyPocoConfigurableModule
    {
        public string Value1 { get; set; }
        public int Value2 { get; set; }
        public int? Value3 { get; set; }
        public DateTime Value4 { get; set; }
        public DateTime? Value5 { get; set; }
        public MyEnum Value6 { get; set; }       
    }

    public class MyConfigManager : ConfigManager<MyPocoConfigurableModule> { }

    public partial class ExamplesWikiPage
    {
        /// <summary>
        /// Before running set appSettings config section to point to file "_BasicExample.config":
        /// <appSettings configSource="_BasicExample.config"></appSettings>
        /// </summary>
        [TestMethod]
        public void PlainOldClrObjectExample()
        {            
            Assert.AreEqual("Configured1", MyConfigManager.Config.Value1);
            Assert.AreEqual(-2, MyConfigManager.Config.Value2);
            Assert.AreEqual(null, MyConfigManager.Config.Value3);
            Assert.AreEqual(new DateTime(1977, 5, 19), MyConfigManager.Config.Value4);
            Assert.AreEqual(null, MyConfigManager.Config.Value5);
            Assert.AreEqual(MyEnum.Enum2, MyConfigManager.Config.Value6);
        }        
    }
}
