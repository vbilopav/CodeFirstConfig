using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace WebExample.Controllers
{
    public class LogController : ApiController
    {
        // GET: api/Log
        public async Task<HttpResponseMessage> Get()
        {
            HttpContext.Current.Response.Output.Write(Log.Content());
            HttpContext.Current.Response.ContentType = "application/json charset=utf-8";
            await HttpContext.Current.Response.Output.FlushAsync();
            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}
