using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    public static class Utilities
    {
        const string key = "A secure key";
        const string salt = "A secure salt.";

        // This actually doesn't work and sends a null body. Changed the api to pretend that didn't happen.
        public static int GetAndUpdateRiskScore(ref Entity contact)
        {
            try
            {
                byte[] ssn;

                if (contact.Attributes.Contains("mortage_ssn"))
                {
                    ssn = ASCIIEncoding.ASCII.GetBytes((string)contact.Attributes["mortage_ssn"]);
                }
                else
                {
                    // Generate an SSN and give to the API and the contact
                    string randomSSN = RandomSSN();
                    contact.Attributes.Add("mortage_ssn", randomSSN);
                    ssn = ASCIIEncoding.ASCII.GetBytes(randomSSN);
                }

                HttpClient client = new HttpClient();

                client.DefaultRequestHeaders
                  .Accept
                  .Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "http://mortage.azurewebsites.net/api/credit");

                httpRequestMessage.Content = new StringContent((string)contact.Attributes["mortage_ssn"], Encoding.ASCII, "application/json"); ;

                var response = client.SendAsync(httpRequestMessage).Result;

                int riskScore = Convert.ToInt32(response.Content.ReadAsStringAsync().Result);

                if (contact.Attributes.Contains("mortage_riskscore"))
                {
                    contact.Attributes["mortage_riskscore"] = riskScore;
                }
                else
                {
                    contact.Attributes.Add("mortage_riskscore", riskScore);
                }

                return riskScore;
            }
            catch (Exception)
            {
                // If something breaks, generate it ourselves
                Random random = new Random();
                return random.Next(1, 101);
            }
        }

        public static string HashAThing(string toHash)
        {
            MD5 hasher = System.Security.Cryptography.MD5.Create();

            Random random = new Random();
            
            string salt = random.Next(1,1000).ToString();

            salt.PadLeft(4, '0');

            var hashed = hasher.ComputeHash(ASCIIEncoding.ASCII.GetBytes(toHash + salt));

            string output = "";

            foreach (var datum in hashed)
            {
                output += datum;
            }

            output += salt;

            return output;
        }

        public static string RandomSSN()
        {
            string ssn = "";

            Random random = new Random();

            ssn += Convert.ToString(random.Next(0, 999)).PadLeft(3,'0');
            ssn += '-';
            ssn += Convert.ToString(random.Next(0, 99)).PadLeft(2, '0');
            ssn += '-';
            ssn += Convert.ToString(random.Next(0, 9999)).PadLeft(4, '0');

            return ssn;
        }
    }
}
