using System;
using MortageAppApi.Models;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using Microsoft.Xrm.Tooling.Connector;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;


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


        [HttpGet]
        public IHttpActionResult Get(string contactID) {
            // Get the CRM connection string and connect to the CRM Organization
            CrmServiceClient client = new CrmServiceClient("Url=https://revmortage.crm.dynamics.com; Username=isteiner@RevMortage.onmicrosoft.com; Password=9V5$m9xjm#*HDuHT; authtype=Office365");
            IOrganizationService service = (IOrganizationService)
            client.OrganizationWebProxyClient ?? (IOrganizationService)client.OrganizationServiceProxy;
            try { 
            
                string query = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
  <entity name='incident'>
    <attribute name='title' />
    <attribute name='ticketnumber' />
    <attribute name='incidentid' />
    <attribute name='statecode' />
    <attribute name='mortage_priority' />
    <attribute name='mortage_highpriorityreason' />
    <order attribute='title' descending='false' />
    <filter type='and'>
      <condition attribute='customerid' operator='eq' uitype='contact' value= '{" + new Guid(contactID) + @"}' />
    </filter>
  </entity>
</fetch>";
                EntityCollection collection = service.RetrieveMultiple(new FetchExpression(query));
                 var okay = collection.Entities.ToArray();
                List<Case> cases = new List<Case>();
                int size = collection.Entities.Count();
                for (int i = 0; i < size; i++) {
                    Entity latestCase = collection.Entities[i];
                    Case caseTest = new Case();
                    caseTest.mortgageid = latestCase.Attributes["title"].ToString();
                    caseTest.ticketnumber = latestCase.Attributes["ticketnumber"].ToString();
                    caseTest.code = latestCase.FormattedValues["statecode"].ToString();
                    caseTest.priority = latestCase.Attributes["mortage_priority"].ToString();
                  //  caseTest.highpriority = latestCase.FormattedValues["mortage_highpriorityreason"].ToString();
                    cases.Add(caseTest);
                }

                return Ok(cases);
            }
            catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }
    }
}
