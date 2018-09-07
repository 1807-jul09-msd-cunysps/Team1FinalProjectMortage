using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mortage.Controllers
{
    public class MortgageController : Controller
    {
        // GET: Mortgage
        public ActionResult List()
        {
            return View();
        }

        public ActionResult Apply()
        {
            return View();
        }
    }
}