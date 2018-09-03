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

            //Money totalAmount = new Money(100000);
            //decimal apr = 0.1M + 0.05M;
            //int mortageTerm = 144;
            //decimal periodicInterest = (apr / 12);

            //Money monthlyPayment = new Money(
            //    (decimal)
            //        (
            //        (double)(totalAmount.Value * periodicInterest) /
            //            (
            //                1 - Math.Pow((double)(1 + periodicInterest), -(double)mortageTerm)
            //            )
            //        )
            //    );

            //Console.WriteLine($"Total amount: {totalAmount}");
            //Console.WriteLine($"APR: {apr}");
            //Console.WriteLine($"Periodic interest: {periodicInterest}");
            //Console.WriteLine($"Total term in months: {mortageTerm}");
            //Console.WriteLine($"Monthly payment:{monthlyPayment.Value}");

            Console.Read();
        }
    }
}
