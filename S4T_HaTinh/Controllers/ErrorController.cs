using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace S4T_HaTinh.Controllers
{
    public class ErrorController : Controller
    {
        public class NotFoundViewModel
        {
            public string RequestedUrl { get; set; }
            public string ReferrerUrl { get; set; }
        }
        // GET: Error
        public ActionResult NotFound()
        {
            return RedirectToAction("Index", "Home");
            ActionResult result;
            
            object model = Request.Url.PathAndQuery;

            if (!Request.IsAjaxRequest())
                result = View(model);
            else
                result = View();

            return result;
        }

        public ActionResult Error()
        {
            return RedirectToAction("Index", "Home");
            ActionResult result;

            object model = Request.Url.PathAndQuery;

            if (!Request.IsAjaxRequest())
                result = View(model);
            else
                result = View();

            return result;
        }
    }
}