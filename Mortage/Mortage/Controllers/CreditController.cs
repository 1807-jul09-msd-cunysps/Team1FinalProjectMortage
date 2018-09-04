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
        CreditCheck num = new CreditCheck();
        [HttpGet]
        public IHttpActionResult Get(string ssn) {
            try {
                //Convert ssn to an integer
                int ssnNum = 0;

                foreach (var bytes in Encoding.ASCII.GetBytes(ssn))
                {
                    ssnNum += bytes;
                }

                Random ranNum = new Random(ssnNum);
                int riskRan = ranNum.Next(1, 101);
                num.riskScore = riskRan;
                return Ok(num);

            }
            catch (Exception ex){
                return BadRequest(ex.Message);
            }

        }
    }
}
