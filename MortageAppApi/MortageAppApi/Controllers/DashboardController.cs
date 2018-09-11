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
    [EnableCors("*", "*", "*")]
    public class DashboardController : ApiController
    {
        [HttpGet]
        public IHttpActionResult Get(string guidContact)
        {
            //Calling the organization
            CrmServiceClient client = new CrmServiceClient("Url=https://revmortage.crm.dynamics.com; Username=isteiner@RevMortage.onmicrosoft.com; Password=9V5$m9xjm#*HDuHT; authtype=Office365");
            IOrganizationService service = (IOrganizationService)
            client.OrganizationWebProxyClient ?? (IOrganizationService)client.OrganizationServiceProxy;

            string query = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
  <entity name='mortage_mortgage'>
    <attribute name='mortage_mortgageid' />
    <attribute name='mortage_name' />
    <attribute name='createdon' />
    <attribute name='mortage_amount' />
    <order attribute='mortage_name' descending='false' />
    <filter type='and'>
      <condition attribute='mortage_contactid' operator='eq' uitype='contact' value='{" + new Guid(guidContact) + @"}' />
    </filter>
  </entity>
</fetch>";
            //3DAB67CF - 18B5 - E811 - A95C - 000D3A3AB637
            EntityCollection collection = service.RetrieveMultiple(new FetchExpression(query));
            int size = collection.Entities.Count();
            List<Dashboard> dashes = new List<Dashboard>();
            for (int i = 0; i < size; i++)
            {
                Entity latestMortgage = collection.Entities[i];
                
                    Dashboard dash = new Dashboard();
                    dash.amount = (((Money)latestMortgage.Attributes["mortage_amount"]).Value).ToString();
                    dash.name = latestMortgage.Attributes["mortage_name"].ToString();
                    dashes.Add(dash);
                
            }
          /*Dashboard dash = new Dashboard();
          dash.size = collection.Entities.Count().ToString();
          dash.amount = (((Money)latestMortgage.Attributes["mortage_amount"]).Value).ToString();
          dash.name= latestMortgage.Attributes["mortage_name"].ToString(); */


            return Ok(dashes);
        }
    }
}