using System;
using System.Web;
using System.Web.Http;

namespace WebExample.Controllers
{
    public class SampleClassController : ApiController
    {
        // GET: api/SampleClass
        public SampleClass Get()
        {
            var ret = SampleClassConfigManager.Config;
            HttpContext.Current.Response.Headers.Add("_timestamp", DateTime.Now.ToString("O"));
            return ret;
        }
    }
}
