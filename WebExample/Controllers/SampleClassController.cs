using System.Web.Http;

namespace WebExample.Controllers
{
    public class SampleClassController : ApiController
    {
        // GET: api/SampleClass
        public SampleClass Get() => SampleClassConfigManager.Config;
    }
}
