using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.Controllers
{
    public class ErrorController : Controller
    {

        public ActionResult Index()
        {
            TempData["ErStat"] = "We do not know what happened";
            return RedirectToAction("Index", "Home");
        }
        public ActionResult NotFound()
        {
            TempData["ErStat"] = "The Page you are trying to access does not exist";
            return RedirectToAction("Index", "Home");
        }
        public ActionResult BadReq()
        {
            TempData["ErStat"] = "This isn't a proper way to access the page you are trying to reach";
            return RedirectToAction("Index", "Home");
        }
        public ActionResult NoLogin()
        {
            TempData["ErStat"] = "Provided credentials are not valid";
            return RedirectToAction("Login", "Home");
        }
    }
}