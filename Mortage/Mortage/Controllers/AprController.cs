using Mortage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;


namespace Mortage.Controllers
{
    [EnableCors("*", "*", "*")]
    public class AprController : ApiController
    {
        Apr aprValue = new Apr();
        [HttpGet]
        public IHttpActionResult Get() {
            //1.	Final APR for Mortgage = (Base APR + Margin) + Log (Risk Score)  + Sales Tax based on State;
            Random rdm = new Random();
            aprValue.apr = (rdm.Next(3, 6)/100);
            return Ok(aprValue);
        }
    }
}
