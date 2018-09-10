using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;
using MortageAppApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Http.Cors;


namespace MortageAppApi.Controllers
{
    [EnableCors("*","*","*")]
    public class CaseController : ApiController
    {
        
        [HttpPost]
        public IHttpActionResult Post([FromBody]Case incident)
        {
            try
            {
                // Get the CRM connection string and connect to the CRM Organization
                CrmServiceClient client = new CrmServiceClient("Url=https://revmortage.crm.dynamics.com; Username=isteiner@RevMortage.onmicrosoft.com; Password=9V5$m9xjm#*HDuHT; authtype=Office365");
                IOrganizationService service = (IOrganizationService)
                client.OrganizationWebProxyClient ?? (IOrganizationService)client.OrganizationServiceProxy;


                Entity Case = new Entity("incident");
                Case.Attributes.Add("title", incident.mortgageid);
                Case.Attributes.Add("customerid", new EntityReference("contact", new Guid(incident.customer)));
                Case.Attributes.Add("description", incident.description);
                service.Create(Case);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
