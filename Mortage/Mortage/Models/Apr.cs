using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mortage.Models
{
    /** 
     * For every 24 hours, Base APR synchronizes and gets latest value from web service.  
     * **/
    public class Apr
    {
        public int apr{ get; set; }
    }
}