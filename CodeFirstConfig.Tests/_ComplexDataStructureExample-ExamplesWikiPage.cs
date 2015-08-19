using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

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

    public class ComplexDataStructureManager : ConfigManager<ComplexDataStructuresModule> { }

    public partial class ExamplesWikiPage
    {
        /// <summary>
        /// Before running set appSettings config section to point to file "_ComplexDataStructureExample.config":
        /// <appSettings configSource="_ComplexDataStructureExample.config"></appSettings>
        /// </summary>
        [TestMethod]
        public void ComplexDataStructureExample()
        {
            var c = ComplexDataStructureManager.Config;

            Assert.AreEqual(3, c.IntArray.Length);
            Assert.AreEqual(1, c.IntArray[0]);
            Assert.AreEqual(2, c.IntArray[1]);
            Assert.AreEqual(3, c.IntArray[2]);

            Assert.AreEqual(3, c.StringArray.Length);
            Assert.AreEqual("first", c.StringArray[0]);
            Assert.AreEqual("second", c.StringArray[1]);
            Assert.AreEqual("third", c.StringArray[2]);

            Assert.AreEqual(4, c.StringEnumerable.Count());
            Assert.AreEqual("first", c.StringEnumerable.ElementAt(0));
            Assert.AreEqual("second", c.StringEnumerable.ElementAt(1));
            Assert.AreEqual("third", c.StringEnumerable.ElementAt(2));
            Assert.AreEqual("fourth", c.StringEnumerable.ElementAt(3));

            Assert.AreEqual(3, c.StringStringDictionary.Count());
            Assert.AreEqual("value1", c.StringStringDictionary["key1"]);
            Assert.AreEqual("value2", c.StringStringDictionary["key2"]);
            Assert.AreEqual("value3", c.StringStringDictionary["key3"]);

            Assert.AreEqual(3, c.IntStringDictionary.Count());
            Assert.AreEqual("value1", c.IntStringDictionary[1]);
            Assert.AreEqual("value2", c.IntStringDictionary[2]);
            Assert.AreEqual("value3", c.IntStringDictionary[3]);

            Assert.AreEqual(null, c.ComplexClass1);

            Assert.AreEqual("value1", c.ComplexClass2.Value1);
            Assert.AreEqual(1, c.ComplexClass2.Value2);
            Assert.AreEqual(true, c.ComplexClass2.Value3);

            Assert.AreEqual(3, c.ComplexClassEnumerable.Count());
            Assert.AreEqual("value1", c.ComplexClassEnumerable.ElementAt(0).Value1);
            Assert.AreEqual(1, c.ComplexClassEnumerable.ElementAt(0).Value2);
            Assert.AreEqual(true, c.ComplexClassEnumerable.ElementAt(0).Value3);
            Assert.AreEqual("value2", c.ComplexClassEnumerable.ElementAt(1).Value1);
            Assert.AreEqual(2, c.ComplexClassEnumerable.ElementAt(1).Value2);
            Assert.AreEqual(false, c.ComplexClassEnumerable.ElementAt(1).Value3);
            Assert.AreEqual("value3", c.ComplexClassEnumerable.ElementAt(2).Value1);
            Assert.AreEqual(3, c.ComplexClassEnumerable.ElementAt(2).Value2);
            Assert.AreEqual(true, c.ComplexClassEnumerable.ElementAt(2).Value3);
        }        
    }
}
