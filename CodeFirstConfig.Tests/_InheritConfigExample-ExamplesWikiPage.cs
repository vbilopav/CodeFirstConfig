using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CodeFirstConfig.Tests
{
    public class BaseConfigModuleCs6
    {
        public static BaseConfigModuleCs6 Config => ConfigManager<BaseConfigModuleCs6>.Config;

        public string Value1 { get; set; } = "Value1";
        public string Value2 { get; set; } = "Value2";
        public string Value3 { get; set; } = null;  
    }

    public class InheritConfigModuleCs6 : BaseConfigModuleCs6
    {
        public static new InheritConfigModuleCs6 Config => ConfigManager<InheritConfigModuleCs6>.Config;

        public int Value4 { get; set; } = 4;
        public int? Value5 { get; set; } = 5;
        public int? Value6 { get; set; } = null;
    }

 
    public partial class ExamplesWikiPage
    {
        /// <summary>
        /// Before running set appSettings config section to point to file "_InheritExample.config":
        /// <appSettings configSource="_InheritExample.config"></appSettings>
        /// </summary>
        [TestMethod]
        public void InheritConfigExample()
        {            
            Assert.AreEqual("Configured1", BaseConfigModuleCs6.Config.Value1);
            Assert.AreEqual(null, BaseConfigModuleCs6.Config.Value2);
            Assert.AreEqual("Configured3", BaseConfigModuleCs6.Config.Value3);

            Assert.AreEqual("Configured1", InheritConfigModuleCs6.Config.Value1);
            Assert.AreEqual("Configured2", InheritConfigModuleCs6.Config.Value2);
            Assert.AreEqual(null, InheritConfigModuleCs6.Config.Value3);

            Assert.AreEqual(-2, InheritConfigModuleCs6.Config.Value4);
            Assert.AreEqual(null, InheritConfigModuleCs6.Config.Value5);
            Assert.AreEqual(-4, InheritConfigModuleCs6.Config.Value6);
        }        
    }
}
