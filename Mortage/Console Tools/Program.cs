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
    class Program
    {
        static void Main(string[] args)
        {
            CrmServiceClient client = new CrmServiceClient("Url=https://revmortage.crm.dynamics.com; Username=isteiner@RevMortage.onmicrosoft.com; Password=9V5$m9xjm#*HDuHT; authtype=Office365");
            IOrganizationService service = (IOrganizationService)
            client.OrganizationWebProxyClient ?? (IOrganizationService)client.OrganizationServiceProxy;

            CreateAttributeRequest mortgageAutoNumber = new CreateAttributeRequest
            {
                EntityName = "mortage_mortgage",
                Attribute = new StringAttributeMetadata
                {
                    //Define the format of the attribute
                    AutoNumberFormat = "-{RANDSTRING:10}-{DATETIMEUTC:yyyyMM}{SEQNUM:6}",
                    LogicalName = "mortage_mortgageautonumber",
                    SchemaName = "mortage_mortgageautonumber",
                    RequiredLevel = new AttributeRequiredLevelManagedProperty(AttributeRequiredLevel.ApplicationRequired),
                    MaxLength = 100, // The MaxLength defined for the string attribute must be greater than the length of the AutoNumberFormat value, that is, it should be able to fit in the generated value.
                    DisplayName = new Label("Mortgage Auto Number", 1033),
                    Description = new Label("Auto mortgage number for this mortgage.", 1033)
                }
            };
            var result = service.Execute(mortgageAutoNumber);

            Console.WriteLine(result.ResponseName);

            foreach (var item in result.Results)
            {
                Console.WriteLine($"Key: {item.Key} — Value: {item.Value}");
            }

            Console.Read();
        }
    }
}
