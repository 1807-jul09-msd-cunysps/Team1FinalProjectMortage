using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace MortagePort.Controllers
{
    [EnableCors("*", "*", "*")]
    public class AprController : ApiController
    {
        [HttpGet]
        public IHttpActionResult Get()
        {
            Random rdm = new Random();
            return Ok(Math.Round((rdm.NextDouble() * (.06 - .01) + .01), 2));
        }
    }
}
