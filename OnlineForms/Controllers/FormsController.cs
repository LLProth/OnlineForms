using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Web.Mvc;
using System.Net.Mail;
using OnlineForms.ViewModels;
using OnlineForms.Models.SFN18795;
using OnlineForms.Models;
using OnlineForms.Helper;
using System.Data;
using Microsoft.SharePoint.Client;
using SP = Microsoft.SharePoint.Client;
using FormCollection = System.Web.Mvc.FormCollection;
using System.Security;
using System.Diagnostics;

namespace OnlineForms.Controllers
{
    public class FormsController : Controller
    {
        private SFN18795ModelDal dal = new SFN18795ModelDal();

        // GET: Forms
        public ActionResult Index()
        {
            return View();
        }

        
        public ActionResult SFN18795Display(int id)
        {
            ViewBag.FormInfo = dal.GetFormInfo("SFN18795");

            dal = new SFN18795ModelDal();
            DataTable dtInfo = dal.GetSFN18795InfoByID(id);
            SFN18795DisplayViewModel vmSFN18795 = new SFN18795DisplayViewModel();
            vmSFN18795.RequisitionModel = SFN18795Model.ConvertDataTableToRequisition(dtInfo);
            vmSFN18795.RequisitionItems = SFN18795RequisitionItem.ConvertDataTableToRequisitionItemList(dtInfo);

            ViewBag.TotalPrice = dtInfo.Rows[0]["REQ_ITEMS_TOTAL"].ToString();
            ViewBag.ID = id;
            
            return View("~/Views/Forms/SFN18795/View.cshtml", vmSFN18795);
        }

        public ActionResult SFN18795Print(int id)
        {
            ViewBag.FormInfo = dal.GetFormInfo("SFN18795");

            dal = new SFN18795ModelDal();
            DataTable dtInfo = dal.GetSFN18795InfoByID(id);
            SFN18795DisplayViewModel vmSFN18795 = new SFN18795DisplayViewModel();
            vmSFN18795.RequisitionModel = SFN18795Model.ConvertDataTableToRequisition(dtInfo);
            vmSFN18795.RequisitionItems = SFN18795RequisitionItem.ConvertDataTableToRequisitionItemList(dtInfo);

            ViewBag.TotalPrice = dtInfo.Rows[0]["REQ_ITEMS_TOTAL"].ToString();
            ViewBag.ID = id;

            return View("~/Views/Forms/SFN18795/Print.cshtml", vmSFN18795);
        }

        // POST: Forms/RequisitionForm
        //[HttpPost]
        //public ActionResult SFN18795(FormCollection collection)
        //{
        //    bool formsub = false;
        //    int sfn18795ID = dal.InsertSFN18795GetID(collection, formsub, );

        //    string content = GetRequisitionManagerEmail(float.Parse(collection["RequisitionModel.TotalPrice"]), sfn18795ID);

        //    //dal.InsertSFN18795ReqItems(collection, sfn18795ID);

        //    return Content(content);
        //}

        public async System.Threading.Tasks.Task<ActionResult> Testing()
        {
            //string contentString = await TestSharePointConnectionAsync("https://ndgov.sharepoint.com/sites/WSI-FormsDevelopment");

            string contentString = await TestSharepointGet();

            return Content(contentString);
        }

        public static async System.Threading.Tasks.Task<string> TestSharePointConnectionAsync(string sharepointUrl)
        {
            SecureString passWord = new SecureString();
            foreach (char c in "share#2014".ToCharArray()) passWord.AppendChar(c); 
            using (var auth = new PnP.Framework.AuthenticationManager("0ba7030e-6a25-47ef-86fe-4b77b7cfd955", "workflow@nd.gov", passWord))
            {
                using (var ctx = await auth.GetContextAsync(sharepointUrl))
                {
                    ctx.Load(ctx.Web);
                    ctx.ExecuteQuery();
                    return ctx.Web.Title;
                }
            }
        }

        public static async System.Threading.Tasks.Task<string> TestSharepointGet()
        {
            string content = string.Empty;
            //string siteURL = "https://ndgov.sharepoint.com/sites/WSI-Forms/CentralFormLists/Lists/Department/AllItems.aspx";
            string siteURL = "https://ndgov.sharepoint.com/sites/WSI-Forms/CentralFormLists";

            SecureString pWord = new SecureString();
            foreach (char c in "share#2014".ToCharArray()) pWord.AppendChar(c);
            using (var auth = new PnP.Framework.AuthenticationManager("0ba7030e-6a25-47ef-86fe-4b77b7cfd955", "workflow@nd.gov", pWord))
            {
                using (var ctx = await auth.GetContextAsync(siteURL))
                {
                    Web web = ctx.Web;
                    ListCollection collList = web.Lists;
                    ctx.Load(collList);
                    ctx.ExecuteQuery();
                    SP.List deptList = collList.GetByTitle("Departments");
                    ctx.Load(deptList);
                    ctx.ExecuteQuery();

                    CamlQuery query = new CamlQuery();
                    query.ViewXml = "<View />";
                    SP.ListItemCollection items = deptList.GetItems(query);
                    ctx.Load(items);
                    ctx.ExecuteQuery();

                    foreach (SP.ListItem item in items)
                    {
                        SP.FieldUserValue user = (SP.FieldUserValue)item["Division_x0020_Chief"];
                        content += user.LookupValue + "<br />";
                    }

                    return content;
                }
            }
        }

        public static string GetRequisitionManagerEmail(float totalCost, int id)
        {
            string managerEmail = string.Empty;
            string managerProps = string.Empty;
            string managerName = string.Empty;
            int managerSteps = 0;

            if (totalCost <= 200) { managerSteps = 1; }
            else if (totalCost <= 5000) { managerSteps = 2; }
            else if (totalCost <= 50000) { managerSteps = 3; }
            else if (totalCost > 50000) { managerSteps = 4; }

            try
            {
                using (PrincipalContext context = new PrincipalContext(ContextType.Domain))
                {
                    string managerDistinguishedName = string.Empty;

                    using (UserPrincipal loggedInUser = UserPrincipal.FindByIdentity(context, Environment.UserName))
                    {
                        DirectoryEntry loggedInEntry = (DirectoryEntry)loggedInUser.GetUnderlyingObject();
                        managerDistinguishedName = loggedInEntry.Properties["manager"][0].ToString();
                    }
                   
                    string[] uemail;
                    uemail = new string[1];
                    string[] ccmail = new string[0];

                    for (int i = 0; i < managerSteps; i++)
                    {
                        using (UserPrincipal managerUser = UserPrincipal.FindByIdentity(context, IdentityType.DistinguishedName, managerDistinguishedName))
                        {
                                                     
                            uemail[0] = "tdmonroe@nd.gov";
                            managerEmail = managerUser.EmailAddress;
                            managerName = managerUser.DisplayName;
                            string body = "Please review the Requisition request submitted by " + UserPrincipal.Current.DisplayName + " to be Approved by " + managerName + ":  " + managerEmail 
                                            + "/n <a href=\"https://localhost:44345/SFN18795/Edit/" + id + "\">Requisition Form Link " + id +"</a>";
                            //Email.SendEmail(uemail, ccmail, "Requisition Form Approval", body);
                            
                            Debug.WriteLine(managerDistinguishedName + ":  " + managerName +   " :"  +managerEmail + ":  " + body);
                            //If on the last manager no need to create a new directory entry
                            if (i != (managerSteps - 1))
                            {
                                DirectoryEntry currManagerEntry = (DirectoryEntry)managerUser.GetUnderlyingObject();
                                managerDistinguishedName = currManagerEntry.Properties["manager"][0].ToString();
                            }
                        }
                    }
                }

                return managerProps;
            }
            catch(Exception e)
            {
                return "Unable to get manager's email<br />Error:" + e.Message;
            }
        }

    }
}