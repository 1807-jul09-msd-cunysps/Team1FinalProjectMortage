using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace UpdateExchangeRate
{
    class Program
    {
        static void Main(string[] args)
        {
            CrmServiceClient client = new CrmServiceClient("Url=https://revmortage.crm.dynamics.com; Username=isteiner@RevMortage.onmicrosoft.com; Password=9V5$m9xjm#*HDuHT; authtype=Office365");
            IOrganizationService service = (IOrganizationService)
            client.OrganizationWebProxyClient ?? (IOrganizationService)client.OrganizationServiceProxy;

            // Get the most recent exchange rate
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://data.fixer.io/api/latest?access_key=290214052ac1a4686e0322204fe25f66&base=EUR&symbols=USD,CAD");
            request.Method = "GET";

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string responseString = "";

            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                responseString = reader.ReadToEnd();
            }

            var responseObject = JObject.Parse(responseString);

            decimal usd = Decimal.Parse((string)responseObject["rates"]["USD"]);
            decimal cad = Decimal.Parse((string)responseObject["rates"]["CAD"]);

            // The exchange rate from USD as the base to Canadian Maple Money
            decimal exchangeRate = cad / usd;

            // Update the exchange rate in Dynamics
            var mapleMoney = service.Retrieve("transactioncurrency", new Guid("098ECA1F-82B0-E811-A95B-000D3A3AB886"), new ColumnSet(
                "exchangerate"
                ));

            mapleMoney.Attributes["exchangerate"] = exchangeRate;

            service.Update(mapleMoney);

            Console.WriteLine($"Canadian exchange rate updated to {mapleMoney.Attributes["exchangerate"]}");
        }
    }
}
