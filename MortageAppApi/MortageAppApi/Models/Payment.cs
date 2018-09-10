using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MortageAppApi.Models
{
    public class Payment
    {
        public string payAmount { get; set; }
        public string mortgageID { get; set; }
    }
}