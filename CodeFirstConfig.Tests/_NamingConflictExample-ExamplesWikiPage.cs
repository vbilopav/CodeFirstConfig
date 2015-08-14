using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace CodeFirstConfig.Tests
{
    public class FirstConfigurableModule : ConfigManager<FirstConfigurableModule>
    {
       public string Value1 { get; set; }
       public string Value2 { get; set; }

       public FirstConfigurableModule()
       {
           this.Value1 = "FirstConfigurableModule.Value1";
           this.Value2 = "FirstConfigurableModule.Value2";
       }
    }

    public class SecondConfigurableModule : ConfigManager<SecondConfigurableModule>
    {
       public string Value1 { get; set; }
       public string Value2 { get; set; }

       public SecondConfigurableModule()
       {
           this.Value1 = "SecondConfigurableModule.Value1";
           this.Value2 = "SecondConfigurableModule.Value2";
       }
    }

    public partial class ExamplesWikiPage
    {
        /// <summary>
        /// Before running set appSettings config section to point to file "_NamingConflictExample.config":
        /// <appSettings configSource="_NamingConflictExample.config"></appSettings>
        /// </summary>
        [TestMethod]
       public void NamingConflictExample()
       {
           Assert.AreEqual("Configured1", FirstConfigurableModule.Config.Value1);
           Assert.AreEqual("Configured1", SecondConfigurableModule.Config.Value1);

           Assert.AreEqual("FirstConfigurableModule.Configured2", FirstConfigurableModule.Config.Value2);
           Assert.AreEqual("SecondConfigurableModule.Configured2", SecondConfigurableModule.Config.Value2);
       }
    }
}
