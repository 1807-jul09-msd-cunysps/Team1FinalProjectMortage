using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MortageAppApi.Models
{
    public class Case
    {
        public string mortgageid { get; set; }
        public string customer { get; set; }
        public string description { get; set; }
    }
}