using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace CodeFirstConfig.Tests
{
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
    }
}
