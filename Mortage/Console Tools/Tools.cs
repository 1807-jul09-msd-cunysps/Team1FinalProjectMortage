using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Tooling.Connector;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk;

namespace Console_Tools
{
    public static class Tools
    {
        public static void AutoNumber()
        {
            CrmServiceClient client = new CrmServiceClient("Url=https://revmortage.crm.dynamics.com; Username=isteiner@RevMortage.onmicrosoft.com; Password=9V5$m9xjm#*HDuHT; authtype=Office365");
            IOrganizationService service = (IOrganizationService)
            client.OrganizationWebProxyClient ?? (IOrganizationService)client.OrganizationServiceProxy;

            UpdateAttributeRequest mortgageAutoNumber = new UpdateAttributeRequest
            {
                EntityName = "mortage_mortgage",
                Attribute = new StringAttributeMetadata
                {
                    //Define the format of the attribute
                    AutoNumberFormat = "{DATETIMEUTC:yyyyMM}-{SEQNUM:6}",
                    LogicalName = "mortage_name",
                    SchemaName = "mortage_name",
                    DisplayName = new Label("Mortgage Number", 1033),
                    Description = new Label("System generated mortgage number for this mortgage.", 1033)
                }
            };
            var result = service.Execute(mortgageAutoNumber);

            Console.WriteLine(result.ResponseName);

            foreach (var item in result.Results)
            {
                Console.WriteLine($"Key: {item.Key} — Value: {item.Value}");
            }
        }
    }
}
