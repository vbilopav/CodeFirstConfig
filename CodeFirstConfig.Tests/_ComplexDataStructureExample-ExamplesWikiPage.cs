using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace CodeFirstConfig.Tests
{
    public class ComplexClass
    {
        public string Value1 { get; set; }
        public int Value2 { get; set; }
        public bool Value3 { get; set; }
    }

    public class ComplexDataStructuresModule
    {
        public int[] IntArray { get; set; }
        public string[] StringArray { get; set; }
        public IEnumerable<string> StringEnumerable { get; set; }
        public IDictionary<string, string> StringStringDictionary { get; set; }
        public IDictionary<int, string> IntStringDictionary { get; set; }
        public ComplexClass ComplexClass1 { get; set; }
        public ComplexClass ComplexClass2 { get; set; }
        public IEnumerable<ComplexClass> ComplexClassEnumerable { get; set; }
    }

    public partial class ExamplesWikiPage
    {
        /// <summary>
        /// Before running set appSettings config section to point to file "_ComplexDataStructureExample.config":
        /// <appSettings configSource="_ComplexDataStructureExample.config"></appSettings>
        /// </summary>
        [TestMethod]
        public void ComplexDataStructureExample()
        {
            var c = ConfigManager<ComplexDataStructuresModule>.Config;

            /*
            Assert.AreEqual("Configured1", MyConfigManager.Config.Value1);
            Assert.AreEqual(-2, MyConfigManager.Config.Value4);
            Assert.AreEqual(new DateTime(1977, 5, 19), MyConfigManager.Config.Value7);
            Assert.AreEqual(false, MyConfigManager.Config.Value10);

            Assert.AreEqual("Configured1", ConfigManager<MyPocoConfigurableModule>.Config.Value1);
            Assert.AreEqual(-2, ConfigManager<MyPocoConfigurableModule>.Config.Value4);
            Assert.AreEqual(new DateTime(1977, 5, 19), ConfigManager<MyPocoConfigurableModule>.Config.Value7);
            Assert.AreEqual(false, ConfigManager<MyPocoConfigurableModule>.Config.Value10);
            */
        }        
    }
}
