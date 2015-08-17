using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace CodeFirstConfig.Tests
{
    public class MyPocoConfigurableModule
    {
        public string Value1 { get; set; }
        public int Value4 { get; set; }
        public DateTime Value7 { get; set; }
        public bool Value10 { get; set; }
        
        public MyPocoConfigurableModule()
        {
            this.Value1 = "Value1";           
            this.Value4 = 4;            
            this.Value7 = DateTime.Now.AddYears(1);           
            this.Value10 = false;           
        }
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
            Assert.AreEqual(-2, MyConfigManager.Config.Value4);
            Assert.AreEqual(new DateTime(1977, 5, 19), MyConfigManager.Config.Value7);
            Assert.AreEqual(false, MyConfigManager.Config.Value10);
        }        
    }
}
