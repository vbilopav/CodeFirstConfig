﻿using System;
using System.Linq;
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
            if (HttpContext.Current.Request.QueryString.Count > 0 &&     
                HttpContext.Current.Request.QueryString[0].Equals("reverse"))
            {
                foreach (var s in Log.Lines().Reverse())
                {
                    await HttpContext.Current.Response.Output.WriteAsync(s);
                    await HttpContext.Current.Response.Output.WriteLineAsync();
                }
            }
            else
                await HttpContext.Current.Response.Output.WriteAsync(Log.Content());
            HttpContext.Current.Response.ContentType = "application/json charset=utf-8";
            await HttpContext.Current.Response.Output.FlushAsync();
            HttpContext.Current.Response.Headers.Add("_timestamp", DateTime.Now.ToString("O"));
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        [Route("api/Log/Clear")]
        [HttpGet]
        public HttpResponseMessage Clear()
        {
            Log.Clear();
            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}
