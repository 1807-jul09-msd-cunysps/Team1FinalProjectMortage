using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    public static class Utilities
    {
        const string key = "A secure key";
        const string salt = "A secure salt.";

        public static double GetRiskScore(string ssn)
        {
            throw new NotImplementedException();
        }

        public static int GetAndUpdateRiskScore(Entity contact)
        {
            // Set a default value for risk score in case something goes wrong
            // DO NOT DO THIS IN PRODUCTION IN REAL LIFE
            // @TODO: Update risk score instead of just accessing it
            int riskScore = 50;

            if ((int)contact.Attributes["mortage_riskscore"] >= 0)
            {
                riskScore = (int)contact.Attributes["mortage_riskscore"];
            }

            return riskScore;
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
    }
}
