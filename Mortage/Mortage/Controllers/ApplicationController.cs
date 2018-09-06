using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Mortage.Models;
using System.Web.Http.Cors;
using Microsoft.Xrm.Tooling.Connector;
using Microsoft.Xrm.Sdk;

namespace Mortage.Controllers
{
    [EnableCors("*", "*", "*")]
    public class ApplicationController : ApiController
    {

        // POST api/<controller>
        [HttpPost]
        public IHttpActionResult Post([FromBody]Application application)
        {
            try {
                //Calling the organization
                CrmServiceClient client = new CrmServiceClient("Url=https://revmortage.crm.dynamics.com; Username=isteiner@RevMortage.onmicrosoft.com; Password=9V5$m9xjm#*HDuHT; authtype=Office365");
                IOrganizationService service = (IOrganizationService)
                client.OrganizationWebProxyClient ?? (IOrganizationService)client.OrganizationServiceProxy;

                //Creating the new mortgage 
                /*  Entity app = new Entity("mortgage");
                          app.Attributes.Add("mortage_name", application.mortgageTitle);
                          app.Attributes.Add("", application.currency);
                          app.Attributes.Add("", application.amount); */

                Entity contact = new Entity("contact");
                contact.Attributes.Add("firstname", application.firstName);
                contact.Attributes.Add("lastname", application.lastName);
                contact.Attributes.Add("mortage_ssn", application.ssn);
                contact.Attributes.Add("address1_line1", application.address1);
                contact.Attributes.Add("address1_line2", application.address2);
                contact.Attributes.Add("address1_city", application.cityOrTown);
                contact.Attributes.Add("address1_stateorprovince", application.stateOrProvince);
                contact.Attributes.Add("address1_postalcode", application.zip);
                contact.Attributes.Add("address1_country", application.country);
                service.Create(contact);
                return Ok();
            }
            catch(Exception ex){
                return BadRequest(ex.Message);
            }
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}