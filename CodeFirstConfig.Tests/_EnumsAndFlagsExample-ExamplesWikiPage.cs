using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CodeFirstConfig.Tests
{
    public enum MyEnum { Enum1, Enum2, Enum3 }

    [Flags]
    public enum MyFlags { Flag1 = 1, Flag2 = 2, Flag3 = 4 }

    public class MyConfigurableEnumModule : ConfigManager<MyConfigurableEnumModule>
    {
        public MyEnum Value1 { get; set; }
        public MyEnum? Value2 { get; set; }
        public MyEnum? Value3 { get; set; }

        public MyFlags Value4 { get; set; }
        public MyFlags? Value5 { get; set; }
        public MyFlags? Value6 { get; set; }

        public MyConfigurableEnumModule()
        {
            this.Value1 = MyEnum.Enum1;
            this.Value2 = MyEnum.Enum2;
            this.Value3 = null;

            this.Value4 = MyFlags.Flag1 | MyFlags.Flag2;
            this.Value5 = MyFlags.Flag2 | MyFlags.Flag3;
            this.Value6 = null;
        }
    }
   
    public partial class ExamplesWikiPage
    {
        /// <summary>
        /// Before running set appSettings config section to point to file "_EnumsAndFlagsExample.config":
        /// <appSettings configSource="_EnumsAndFlagsExample.config"></appSettings>
        /// </summary>
        [TestMethod]
        public void EnumsExample()
        {            
            Assert.AreEqual(MyEnum.Enum2, MyConfigurableEnumModule.Config.Value1);
            Assert.AreEqual(null, MyConfigurableEnumModule.Config.Value2);
            Assert.AreEqual(MyEnum.Enum1, MyConfigurableEnumModule.Config.Value3);
            Assert.AreEqual(MyFlags.Flag2 | MyFlags.Flag3, MyConfigurableEnumModule.Config.Value4);
            Assert.AreEqual(null, MyConfigurableEnumModule.Config.Value5);
            Assert.AreEqual(MyFlags.Flag1 | MyFlags.Flag2 | MyFlags.Flag3, MyConfigurableEnumModule.Config.Value6);
        }        
    }    
}
