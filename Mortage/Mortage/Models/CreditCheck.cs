using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;

namespace Mortage
{
    /** 
     * This class is used to check the credit based on SSN
     * and then return a number(Risk Score) on a scale of (1-100)
     * with 100 being the highest
     * **/
    public class CreditCheck
    {
        public int riskScore { get; set; }
    }
}