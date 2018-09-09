using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;
using System;
using MortageAppApi.Models;
using System.Web.Http;
using System.Web.Http.Cors;

namespace MortageAppApi.Controllers
{
    [EnableCors("*","*","*")]
    public class ApplicationController : ApiController
    {
        [HttpPost]
        public IHttpActionResult Post([FromBody]Application app) {
            try
            {
                // Get the CRM connection string and connect to the CRM Organization
                CrmServiceClient client = new CrmServiceClient("Url=https://revmortage.crm.dynamics.com; Username=isteiner@RevMortage.onmicrosoft.com; Password=9V5$m9xjm#*HDuHT; authtype=Office365");
                IOrganizationService service = (IOrganizationService)
                client.OrganizationWebProxyClient ?? (IOrganizationService)client.OrganizationServiceProxy;

                Entity contact = new Entity("contact");
                contact.Attributes.Add("firstname", app.firstName);
                contact.Attributes.Add("lastname", app.lastName);
                contact.Attributes.Add("mortage_ssn", app.ssn);
                contact.Attributes.Add("address1_line1", app.address1);
                contact.Attributes.Add("address1_line2", app.address2);
                contact.Attributes.Add("address1_city", app.cityOrTown);
                contact.Attributes.Add("address1_stateorprovince", app.stateOrProvince);
                contact.Attributes.Add("address1_postalcode", app.zip);
                contact.Attributes.Add("address1_country", app.country);

                //Retreive guid for mortgage
                Guid myContact = service.Create(contact);

                Entity mortgage = new Entity("mortage_mortgage");
                mortgage.Attributes.Add("mortage_contactid", new EntityReference("contact", myContact));
                mortgage.Attributes.Add("mortage_amount", new Money(app.amount));
                mortgage.Attributes.Add("mortage_termmonths", 144);
                mortgage.Attributes.Add("mortage_region", new OptionSetValue(282450000));
                service.Create(mortgage);

                return Ok(contact);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
    }
}
