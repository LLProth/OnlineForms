using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineForms.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {            
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult TestHeader()
        {
            IDictionary<string, string> formInfo = new Dictionary<string, string>()
            {
                {"FormName", "REQUEST TO CHANGE MEDICAL PROVIDER"},
                {"Division", "TEST DIVISION"},
                {"Info", "SFN 16830 (09/2019)"}
            };

            ViewBag.test = "hello world";

            ViewBag.FormInfo = formInfo;
            return View();
        }
    }
}