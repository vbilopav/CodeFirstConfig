using System;
using System.Linq;
using System.Reflection;
using System.Web;

namespace CodeFirstConfig
{
    public static class AppAssembly
    {
        private static Assembly _assembly;

        private static Assembly GetCurrentDomainAssembly()
        {
            return
                AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.GlobalAssemblyCache && !a.CodeBase.Contains("Temporary") && a.GetName().GetPublicKeyToken().Length == 0)
                .LastOrDefault(a => a.FullName.Contains(
                    AppDomain.CurrentDomain.BaseDirectory.Split(new[] { '\\' }, StringSplitOptions.RemoveEmptyEntries).Last()));
        }

        private static Assembly GetWebEntryAssembly()
        {
            if (HttpContext.Current == null || HttpContext.Current.ApplicationInstance == null) return null;
            Type type = HttpContext.Current.ApplicationInstance.GetType();
            while (type != null && type.Namespace == "ASP") { type = type.BaseType; }
            return type?.Assembly;
        }

        public static Assembly GetAssembly()
        {
            return _assembly = Assembly.GetEntryAssembly() ?? GetWebEntryAssembly() ?? GetCurrentDomainAssembly();
        }

        public static string GetAssemblyDirectory()
        {
            string codeBase = _assembly.CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            return System.IO.Path.GetDirectoryName(path);
        }

        public static Assembly Assembly => _assembly;

        static AppAssembly()
        {
            GetAssembly();
        }
    }
}