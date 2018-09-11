using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MortageAppApi.Models
{
    //[Serializable]
    public class Application
    {
        public string ssn { get; set; }
        public string currency { get; set; }
        public string mortgageTitle { get; set; }
        public string amount { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string address1 { get; set; }
        public string address2 { get; set; }
        public string cityOrTown { get; set; }
        public string stateOrProvince { get; set; }
        public string zip { get; set; }
        public string country { get; set; }
    }
}