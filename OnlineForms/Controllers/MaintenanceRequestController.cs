using OnlineForms.Models;
using OnlineForms.ViewModels;
using System;
using System.Data;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Web.Mvc;

namespace OnlineForms.Controllers
{
    public class MaintenanceRequestController : Controller
    {
        MaintenanceRequestViewModel mrView = new MaintenanceRequestViewModel();

        // GET: Maintenance
        [Authorize]
        public ActionResult Index()
        {
            if (Session["MaintReq"] == null)
            {
                MaintenanceRequestViewModel mrView = new MaintenanceRequestViewModel();
            }
            else
            {
                mrView = (MaintenanceRequestViewModel)Session["MaintReq"];
            }
            MaintenanceRequestModelDAL dal = new MaintenanceRequestModelDAL();
            DataTable dataTable = dal.GetMaintenanceRequestInfo();
            mrView.MaintenanceRequests = MaintenanceRequestModel.GetMaintenanceRequestsList(dataTable);
            System.Web.HttpContext.Current.Request.LogonUserIdentity.Impersonate();
            PrincipalContext context = new PrincipalContext(ContextType.Domain);
            UserPrincipal u = new UserPrincipal(context);
            UserPrincipal user = UserPrincipal.FindByIdentity(context, User.Identity.Name);
            DirectoryEntry dirEntry = (DirectoryEntry)user.GetUnderlyingObject();
            ViewBag.Name = user.Name;
            string[] names = user.Name.Split();
            ViewBag.FirstName = names[1];
            ViewBag.PhoneNumber = user.VoiceTelephoneNumber;
            ViewBag.Email = user.EmailAddress;
            ViewBag.TodaysDate = DateTime.Now;
            ViewBag.CompletedDate = DateTime.MinValue;
            string title = dirEntry.Properties["Title"].Value.ToString();
            ViewBag.Job = title;
            //using (PrincipalContext context = new PrincipalContext(ContextType.Domain))
            //{
            //    using (UserPrincipal loggedInUser = UserPrincipal.FindByIdentity(context, Environment.UserName))
            //    {

            //        viewbag.name = loggedinuser.name;
            //        string[] names = loggedinuser.name.split();
            //        viewbag.firstname = names[1];
            //        viewbag.phonenumber = loggedinuser.voicetelephonenumber;
            //        viewbag.email = loggedinuser.emailaddress;

            //        viewbag.todaysdate = datetime.now;
            //        viewbag.completeddate = datetime.minvalue;
            //        directoryentry direntry = (directoryentry)loggedinuser.getunderlyingobject();
            //        string title = dirEntry.Properties["Title"].Value.ToString();
            //        Debug.WriteLine(title);
            //        ViewBag.Job = title;

            //    }
            //}
            Session["MaintReq"] = mrView;
            return View("~/Views/Forms/MaintenanceRequest/Index.cshtml", mrView);
        }

        public ActionResult MasterList()
        {
            if (Session["MaintReq"] == null)
            {
                MaintenanceRequestViewModel mrView = new MaintenanceRequestViewModel();
            }
            else
            {
                mrView = (MaintenanceRequestViewModel)Session["MaintReq"];
            }
            MaintenanceRequestModelDAL dal = new MaintenanceRequestModelDAL();
            DataTable dataTable = dal.GetMaintenanceRequestInfo();
            mrView.MaintenanceRequests = Models.MaintenanceRequestModel.GetMaintenanceRequestsList(dataTable);
            Session["MaintReq"] = mrView;
            return View("~/Views/Forms/MaintenanceRequest/MasterList.cshtml", mrView);
        }

        public ActionResult ViewMaintenanceRequest(int id)
        {
            if (Session["MaintReq"] == null)
            {
                MaintenanceRequestViewModel mrView = new MaintenanceRequestViewModel();
            }
            else
            {
                mrView = (MaintenanceRequestViewModel)Session["MaintReq"];
            }
            MaintenanceRequestModelDAL dal = new MaintenanceRequestModelDAL();
            DataTable dtMaint = dal.GetMaintenanceRequestInfoByID(id);
            mrView.MaintenanceRequest = MaintenanceRequestModel.ConvertDataTableToMaintenanceRequest(dtMaint);
            Session["MaintReq"] = mrView;
            return View("~/Views/Forms/MaintenanceRequest/ViewMaintenanceRequest.cshtml", mrView);
        }

        public ActionResult SubmitMaintenanceRequest(FormCollection collection)
        {
            MaintenanceRequestModelDAL dal = new MaintenanceRequestModelDAL();
            int MaintId = dal.InsertMaintenanceRequestGetID(collection);
            return RedirectToAction("Index");
        }

        public ActionResult UpdateMaintenanceRequest(int Id)
        {
            if (Session["MaintReq"] == null)
            {
                MaintenanceRequestViewModel mrView = new MaintenanceRequestViewModel();
            }
            else
            {
                mrView = (MaintenanceRequestViewModel)Session["MaintReq"];
            }
            DateTime now = DateTime.Now;
            mrView.MaintenanceRequest.CompletedDate = now;
            MaintenanceRequestModelDAL dal = new MaintenanceRequestModelDAL();
            dal.UpdateMaintenanceRequest(Id);
            Session["MaintReq"] = mrView;
            return ViewMaintenanceRequest(Id);

        }
    }
}