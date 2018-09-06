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
using Newtonsoft.Json.Linq;

namespace UpdateAPR
{
    class Program
    {
        static void Main(string[] args)
        {
            // Get the CRM connection string and connect to the CRM Organization
            CrmServiceClient client = new CrmServiceClient("Url=https://revmortage.crm.dynamics.com; Username=isteiner@RevMortage.onmicrosoft.com; Password=9V5$m9xjm#*HDuHT; authtype=Office365");
            IOrganizationService service = (IOrganizationService)
            client.OrganizationWebProxyClient ?? (IOrganizationService)client.OrganizationServiceProxy;

            // Get the most recent APR
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://mortage.azurewebsites.net/api/apr");
            request.Method = "GET";

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string responseString = "";

            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                responseString = reader.ReadToEnd();
            }

            var res = JObject.Parse(responseString);
            var data = res["apr"].ToString();


            //Console.WriteLine(data);
            //Console.Read();

            // create a column set to define which attributes should be retrieved.
            var apr = service.Retrieve("mortage_configurations", Guid.Parse("9f8444f0-79b0-e811-a95b-000d3a3655ec"), new ColumnSet(
                 "mortage_value"
                 ));

            // update the mortgage value attribute.
            apr.Attributes["mortage_value"] = data;

            // update the configuration.
            service.Update(apr);

            Console.WriteLine(apr);
            Console.Read();
        }
    }
}


//ColumnSet attributes = service.Retrieve( (string "mortage_configurations", Guid("9F8444F0-79B0-E811-A95B-000D3A3655EC"), attributes);
//{ "name", "mortgage_value" });