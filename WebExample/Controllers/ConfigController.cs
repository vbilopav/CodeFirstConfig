using CodeFirstConfig;
using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace WebExample.Controllers
{
    public class ConfigController : ApiController
    {
        // GET: api/Config
        public async Task<HttpResponseMessage> Get()
        {
            var format = ConfigSettings.Instance.ConfigFileFormat;
            if (HttpContext.Current.Request.QueryString.Count > 0)
            {
                if (!Enum.TryParse(HttpContext.Current.Request.QueryString[0], true, out format))
                    format = ConfigSettings.Instance.ConfigFileFormat;
            }
            Configurator.SerializeCurrent(HttpContext.Current.Response.Output, format);
            HttpContext.Current.Response.ContentType = "application/json charset=utf-8";
            await HttpContext.Current.Response.Output.FlushAsync();
            HttpContext.Current.Response.Headers.Add("_timestamp", DateTime.Now.ToString("O"));
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        [Route("api/Config/AppSettings")]
        [HttpGet]
        public async Task<HttpResponseMessage> AppSettings()
        {
            await HttpContext.Current.Response.Output.WriteLineAsync("<appSettings>");
            foreach (var key in ConfigurationManager.AppSettings.AllKeys)
            {                
                await HttpContext.Current.Response.Output.WriteLineAsync($"\t<add key =\"{key}\" value=\"{ConfigurationManager.AppSettings[key]}\" />");
            }
            await HttpContext.Current.Response.Output.WriteLineAsync("</appSettings>");
            HttpContext.Current.Response.ContentType = "application/json charset=utf-8";
            await HttpContext.Current.Response.Output.FlushAsync();
            HttpContext.Current.Response.Headers.Add("_timestamp", DateTime.Now.ToString("O"));
            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}
