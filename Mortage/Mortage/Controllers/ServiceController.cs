using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mortage.Controllers
{
    public class ServiceController : Controller
    {
        // GET: Service
        public ActionResult Case()
        {
            return View();
        }

        public ActionResult Docs()
        {
            return View();
        }
    }
}