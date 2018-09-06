using Mortage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Http.Cors;


namespace Mortage.Controllers
{
    [EnableCors ("*", "*", "*")]
    public class CreditController : ApiController
    {
        [HttpGet]
        public IHttpActionResult Get()
        {
            return BadRequest("Please send customer SSN.");
        }

        [HttpPost]
        public IHttpActionResult Post([FromBody]string ssn) {
            try {
                //Convert ssn to an integer
                int ssnNum = 0;

                foreach (var bytes in Encoding.ASCII.GetBytes(ssn))
                {
                    ssnNum += bytes;
                }

                Random ranNum = new Random(ssnNum);
                int riskRan = ranNum.Next(1, 101);
                return Ok(riskRan);

            }
            catch (Exception ex){
                return BadRequest(ex.Message);
            }

        }
    }
}
