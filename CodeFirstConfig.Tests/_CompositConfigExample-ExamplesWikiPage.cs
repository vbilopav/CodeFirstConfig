using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace CodeFirstConfig.Tests
{
    public class CompositConfigModuleCs5
    {
        public static CompositConfigModuleCs5 Config
        {
            get { return ConfigManager<CompositConfigModuleCs5>.Config; }
        }

        public string Value1 { get; set; }
        public int Value4 { get; set; }
        public DateTime Value7 { get; set; }
        public bool Value10 { get; set; }
        
        public CompositConfigModuleCs5()
        {
            this.Value1 = "Value1";           
            this.Value4 = 4;            
            this.Value7 = DateTime.Now.AddYears(1);           
            this.Value10 = false;           
        }
    }

    public class CompositConfigModuleCs6
    {
        public static CompositConfigModuleCs6 Config => ConfigManager<CompositConfigModuleCs6>.Config;

        public string Value1 { get; set; } = "Value1";
        public int Value4 { get; set; } = 4;
        public DateTime Value7 { get; set; } = DateTime.Now.AddYears(1);
        public bool Value10 { get; set; } = false;
    }

    //public class MyConfigManager : ConfigManager<MyPocoConfigurableModule> { }

    public partial class ExamplesWikiPage
    {
        /// <summary>
        /// Before running set appSettings config section to point to file "_BasicExample.config":
        /// <appSettings configSource="_BasicExample.config"></appSettings>
        /// </summary>
        [TestMethod]
        public void CompositConfigExample()
        {            
            Assert.AreEqual("Configured1", CompositConfigModuleCs5.Config.Value1);
            Assert.AreEqual(-2, CompositConfigModuleCs5.Config.Value4);
            Assert.AreEqual(new DateTime(1977, 5, 19), CompositConfigModuleCs5.Config.Value7);
            Assert.AreEqual(false, CompositConfigModuleCs5.Config.Value10);

            Assert.AreEqual("Configured1", CompositConfigModuleCs6.Config.Value1);
            Assert.AreEqual(-2, CompositConfigModuleCs6.Config.Value4);
            Assert.AreEqual(new DateTime(1977, 5, 19), CompositConfigModuleCs6.Config.Value7);
            Assert.AreEqual(false, CompositConfigModuleCs6.Config.Value10);
        }        
    }
}
