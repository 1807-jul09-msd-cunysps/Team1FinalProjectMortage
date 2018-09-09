using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MortageAppApi.Models
{
    public class Apr
    {
        /** 
    * For every 24 hours, Base APR synchronizes and gets latest value from web service.  
    * **/
            public double apr { get; set; }
    }
}