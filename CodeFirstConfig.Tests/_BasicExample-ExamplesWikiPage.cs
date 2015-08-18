using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace CodeFirstConfig.Tests
{
    public class MyConfigurableBasicModule : ConfigManager<MyConfigurableBasicModule>
    {
        public string Value1 { get; set; }
        public string Value2 { get; set; }
        public string Value3 { get; set; }

        public int Value4 { get; set; }
        public int? Value5 { get; set; }
        public int? Value6 { get; set; }

        public DateTime Value7 { get; set; }
        public DateTime? Value8 { get; set; }
        public DateTime? Value9 { get; set; }

        public bool Value10 { get; set; }
        public bool? Value11 { get; set; }
        public bool? Value12 { get; set; }

        public double Value13 { get; set; }
        public double? Value14 { get; set; }
        public double? Value15 { get; set; }

        public decimal Value16 { get; set; }
        public decimal? Value17 { get; set; }
        public decimal? Value18 { get; set; }

        public MyConfigurableBasicModule()
        {
            this.Value1 = "Value1";
            this.Value2 = "Value2";
            this.Value3 = null;

            this.Value4 = 4;
            this.Value5 = 5;
            this.Value6 = null;

            this.Value7 = DateTime.Now.AddYears(1);
            this.Value8 = DateTime.Now.AddYears(10);
            this.Value9 = null;

            this.Value10 = false;
            this.Value11 = true;
            this.Value12 = null;

            this.Value13 = 1.234;
            this.Value14 = 3.456;
            this.Value15 = null;

            this.Value16 = 1.234M;
            this.Value17 = 3.456M;
            this.Value18 = null;
        }
    }
                    
    public partial class ExamplesWikiPage
    {        
        /// <summary>
        /// Before running set appSettings config section to point to file "_BasicExample.config":
        /// <appSettings configSource="_BasicExample.config"></appSettings>
        /// </summary>
        [TestMethod]
        public void BasicExample()
        {            
            Assert.AreEqual("Configured1", MyConfigurableBasicModule.Config.Value1);
            Assert.AreEqual(null, MyConfigurableBasicModule.Config.Value2);
            Assert.AreEqual("Configured3", MyConfigurableBasicModule.Config.Value3);

            Assert.AreEqual(-2, MyConfigurableBasicModule.Config.Value4);
            Assert.AreEqual(null, MyConfigurableBasicModule.Config.Value5);
            Assert.AreEqual(-4, MyConfigurableBasicModule.Config.Value6);

            Assert.AreEqual(new DateTime(1977, 5, 19), MyConfigurableBasicModule.Config.Value7);
            Assert.AreEqual(null, MyConfigurableBasicModule.Config.Value8);
            Assert.AreEqual(new DateTime(2015, 8, 17), MyConfigurableBasicModule.Config.Value9);

            Assert.AreEqual(false, MyConfigurableBasicModule.Config.Value10);
            Assert.AreEqual(null, MyConfigurableBasicModule.Config.Value11);
            Assert.AreEqual(true, MyConfigurableBasicModule.Config.Value12);

            Assert.AreEqual(5.678, MyConfigurableBasicModule.Config.Value13);
            Assert.AreEqual(null, MyConfigurableBasicModule.Config.Value14);
            Assert.AreEqual(9.123, MyConfigurableBasicModule.Config.Value15);

            Assert.AreEqual(5.678M, MyConfigurableBasicModule.Config.Value16);
            Assert.AreEqual(null, MyConfigurableBasicModule.Config.Value17);
            Assert.AreEqual(9.123M, MyConfigurableBasicModule.Config.Value18);
        }        
    }
}
