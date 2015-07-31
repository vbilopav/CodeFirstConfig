using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using CodeFirstConfig;

namespace WebExample.Controllers
{
    public class ConfigController : ApiController
    {
        // GET: api/Config
        public async Task<HttpResponseMessage> Get()
        {
            var format = ConfigSettings.Instance.Format;
            if (HttpContext.Current.Request.QueryString.Count > 0)
            {
                if (!Enum.TryParse(HttpContext.Current.Request.QueryString[0], true, out format))
                    format = ConfigSettings.Instance.Format;
            }
            Configurator.SerializeCurrent(HttpContext.Current.Response.Output, format);
            HttpContext.Current.Response.ContentType = "application/json charset=utf-8";
            await HttpContext.Current.Response.Output.FlushAsync();
            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}
