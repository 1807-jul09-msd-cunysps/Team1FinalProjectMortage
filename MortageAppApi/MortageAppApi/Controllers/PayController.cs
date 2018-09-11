using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;
using System;
using MortageAppApi.Models;
using System.Web.Http;
using System.Web.Http.Cors;
using Microsoft.Xrm.Sdk.Query;

namespace MortageAppApi.Controllers
{
    [EnableCors("*","*","*")]
    public class PayController : ApiController
    {
        [HttpPost]
        public IHttpActionResult Post(Payment payment) {
            try
            {
                // Get the CRM connection string and connect to the CRM Organization
                CrmServiceClient client = new CrmServiceClient("Url=https://revmortage.crm.dynamics.com; Username=isteiner@RevMortage.onmicrosoft.com; Password=9V5$m9xjm#*HDuHT; authtype=Office365");
                IOrganizationService service = (IOrganizationService)
                client.OrganizationWebProxyClient ?? (IOrganizationService)client.OrganizationServiceProxy;

                 string query = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
  <entity name='mortage_payment'>
    <attribute name='mortage_paymentid' />
    <attribute name='mortage_name' />
    <attribute name='createdon' />
    <order attribute='mortage_name' descending='false' />
    <filter type='and'>
      <condition attribute='mortage_mortgageid' operator='eq' uitype='mortage_mortgage' value='{" + payment.mortgageID + @"}' />
      <condition attribute='statecode' operator='eq' value='0' />
    </filter>
  </entity>
</fetch>";
                EntityCollection collection =  service.RetrieveMultiple(new FetchExpression(query));
                Entity latestPayment = collection.Entities[0];
                latestPayment.Attributes.Add("mortage_amountpaid", new Money(Convert.ToDecimal(payment.payAmount)));
                
                service.Update(latestPayment);

                return Ok();
            }
            catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }
    }
}
