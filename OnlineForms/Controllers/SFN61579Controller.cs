using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Management;
using System.Web.Mvc;
using OnlineForms.ViewModels;
using OnlineForms.Models;
using System.Data;
using Microsoft.SharePoint.Client;
using Microsoft.VisualBasic;
using SP = Microsoft.SharePoint.Client;
using FormCollection = System.Web.Mvc.FormCollection;
using System.Security;
using System.Security.Principal;
using System.Net.Http;
using OnlineForms.Models.SFN61579;
using System.Net.Mail;
using System.Diagnostics;
using OnlineForms.Helper;
using System.Configuration;

namespace OnlineForms.Controllers
{
    public class SFN61579Controller : Controller
    {
        private ModelDALSFN61579 dal = new ModelDALSFN61579();

        private bool _superUser = false;

        // GET: Forms
        [Authorize]
        public ActionResult Index()
        {
            // Get form info from db
            ViewBag.FormInfo = dal.GetFormInfoSFN61579("SFN61579");
            dal = new ModelDALSFN61579();
            
            // Get form info
            DataTable dtInfo = dal.GetSFN61579Info();
            SFN61579DisplayViewModel vmSFN61579 = new SFN61579DisplayViewModel();
            vmSFN61579.CharitableModel = Models.SFN61579.SFN61579Model.ConvertDataTableToCharitableEvent(dtInfo);
            dal = new ModelDALSFN61579();
            vmSFN61579.CharityInfoModel = Models.SFN61579.SFN61579CharityModel.ConvertDataTableToCharitableEventInfo(dtInfo);
            dal = new ModelDALSFN61579();

            // Create domain context
            PrincipalContext ctx = new PrincipalContext(ContextType.Domain);

            UserPrincipal u = new UserPrincipal(ctx);

            UserPrincipal user = UserPrincipal.FindByIdentity(ctx, User.Identity.Name);

            // isSuperUser(user);

            // Get user's department
            DirectoryEntry dirEntry = (DirectoryEntry)user.GetUnderlyingObject();
            string company = dirEntry.Properties["Company"].Value.ToString();
            // Get user's job title
            string title = dirEntry.Properties["Title"].Value.ToString();

            // Set variables
            ViewBag.Username = user.DisplayName;
            ViewBag.Company = company;
            ViewBag.Title = title;

            // Set superusers for the form
            if (title == "Human Resource Officer" || title == "Director of Strategic Operations" || user.DisplayName == "Wood, Margaret" || isInGroup(dirEntry, "-Grp-WSI Servant Leadership Ambassador"))
            {
                ViewBag.Access = true;
            }
            
            dal = new ModelDALSFN61579();
            return View("~/Views/Forms/SFN61579/Index.cshtml", vmSFN61579);
        }

        [Authorize]
        public ActionResult View(int id)
        {
            // Get form info from db
            ViewBag.FormInfo = dal.GetFormInfo("SFN61579");

            dal = new ModelDALSFN61579();

            // Get form info
            DataTable dtInfo = dal.GetSFN61579InfoByID(id);
            SFN61579DisplayViewModel vmSFN61579 = new SFN61579DisplayViewModel();
            vmSFN61579.CharitableModels = SFN61579Model.ConvertDataTableToCharitableEvents(dtInfo);
            dal = new ModelDALSFN61579();

            vmSFN61579.CharityInfoModels = Models.SFN61579.SFN61579CharityModel.ConvertDataTableToCharitableEventInfos(dtInfo);
            dal = new ModelDALSFN61579();

            // Get form values
            ViewBag.Values = dal.GetSFN61579Values(id);
            ViewBag.ID = id;

            // Create domain context
            PrincipalContext ctx = new PrincipalContext(ContextType.Domain);

            UserPrincipal u = new UserPrincipal(ctx);

            UserPrincipal user = UserPrincipal.FindByIdentity(ctx, User.Identity.Name);

            // Get user's job title
            DirectoryEntry dirEntry = (DirectoryEntry)user.GetUnderlyingObject();
            string title = dirEntry.Properties["Title"].Value.ToString();

            // Set superusers for the form
            if (title == "Human Resource Officer" || title == "Director of Strategic Operations" || user.DisplayName == "Wood, Margaret")
            {
                ViewBag.Access = true;
            }

            // ViewBag.Access = _superUser;

            // Create variable with user's display name
            ViewBag.Username = user.DisplayName;
            return View("~/Views/Forms/SFN61579/View.cshtml", vmSFN61579);
        }

        [Authorize]
        public ActionResult Print(int id)
        {
            ViewBag.FormInfo = dal.GetFormInfo("SFN61579");

            dal = new ModelDALSFN61579();
            DataTable dtInfo = dal.GetSFN61579InfoByID(id);
            SFN61579DisplayViewModel vmSFN61579 = new SFN61579DisplayViewModel();
            vmSFN61579.CharitableModels = SFN61579Model.ConvertDataTableToCharitableEvents(dtInfo);
            dal = new ModelDALSFN61579();
            vmSFN61579.CharityInfoModels = Models.SFN61579.SFN61579CharityModel.ConvertDataTableToCharitableEventInfos(dtInfo);
            dal = new ModelDALSFN61579();

            // Get form values
            ViewBag.Values = dal.GetSFN61579Values(id);
            ViewBag.ID = id;
            PrincipalContext ctx = new PrincipalContext(ContextType.Domain);

            UserPrincipal u = new UserPrincipal(ctx);

            UserPrincipal user = UserPrincipal.FindByIdentity(ctx, User.Identity.Name);

            ViewBag.Username = user.DisplayName;
            return View("~/Views/Forms/SFN61579/Print.cshtml", vmSFN61579);
        }

        [Authorize]
        public ActionResult Create()
        {
            // Get form info from db
            ViewBag.FormInfo = dal.GetFormInfo("SFN61579");

            // Create domain context
            PrincipalContext ctx = new PrincipalContext(ContextType.Domain);

            UserPrincipal u = new UserPrincipal(ctx);

            UserPrincipal user = UserPrincipal.FindByIdentity(ctx, User.Identity.Name);

            // Create variables with user info
            ViewBag.Username = user.DisplayName;
            ViewBag.Email = user.EmailAddress;

            // ViewBag.UserName = UserPrincipal.Current.DisplayName;
            return View("~/Views/Forms/SFN61579/Create.cshtml");
        }

        private void isSuperUser(UserPrincipal user)
        {
            dal = new ModelDALSFN61579();
            DataTable dtInfo = dal.GetSuperUsers("SFN 61579");
            foreach (DataRow dr in dtInfo.Rows)
            {
                string login = dr["SU_LOGIN"].ToString();
                string title = dr["SU_TITLE"].ToString();
                if (login.Equals(user.SamAccountName))
                {
                    _superUser = true;
                }
                else if (title.Length > 0)
                {
                    List<string> memberLogin = DirectoryHelper.GetListOfAdUsersByProperty("nd", title, ADUserProperties.TITLE);
                    foreach (string member in memberLogin)
                    {
                        if (member.Equals(user.SamAccountName))
                        {
                            _superUser = true;
                            break;
                        }
                    }
                }
                if (_superUser)
                    break;
            }
            //if (DirectoryHelper.findDepartment(user).Equals("WSI-Human Resources"))
            //{
            //    _inHR = true;
            //}
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SFN61579(FormCollection collection)
        {
            // Insert form info into db
            int sfn61579ID = dal.InsertSFN61579GetID(collection);

            dal.InsertSFN61579CharityInfo(collection, sfn61579ID);

            dal = new ModelDALSFN61579();

            // Get form info
            DataTable idNum = dal.GetSFN61579InfoByInfo();

            string environment = ConfigurationManager.AppSettings["Environment"];

            // Create directory searcher
            DirectorySearcher mySearcher = new DirectorySearcher();
            SearchResultCollection results;
            
            // Get email address of Director of Strategic Operations
            mySearcher.Filter = "(&(company=Workforce Safety & Insurance)(title=" + "Director of Strategic Operations" + "))";
            results = mySearcher.FindAll();

            var toEmailList = new List<string>();

            if (results.Count > 0)
            {
                foreach (SearchResult searchResult in results)
                {
                    if (searchResult.Properties.Contains("mail"))
                    {
                        string email = searchResult.Properties["mail"][0].ToString();
                        if (environment.ToUpper() == "PROD")
                        {
                            if (email.Contains("wsi_") || email.Contains("wsitrain"))
                            {

                            }

                            else
                            {
                                toEmailList.Add(email);
                                
                            }

                        }
                        else
                        {
                            toEmailList.Add(email);
                            
                        }

                    }
                }
            }

            // Get email address of Human Resource Officer
            mySearcher.Filter = "(&(company=Workforce Safety & Insurance)(title=" + "Human Resource Officer" + "))";
            results = mySearcher.FindAll();

            if (results.Count > 0)
            {
                foreach (SearchResult searchResult in results)
                {
                    if (searchResult.Properties.Contains("mail"))
                    {
                        string email = searchResult.Properties["mail"][0].ToString();
                        if (environment.ToUpper() == "PROD")
                        {
                            if (email.Contains("wsi_") || email.Contains("wsitrain"))
                            {

                            }

                            else
                            {
                                toEmailList.Add(email);
                            }

                        }
                        else
                        {
                            toEmailList.Add(email);
                        }
                    }
                }
            }
            
            // Get form url
            string path = HttpContext.Request.Url.AbsoluteUri.ToString();
            List<string> host = new List<string>(path.Split(new string[] { "SFN" }, StringSplitOptions.None));

            // Add to email list
            toEmailList.Add("mwood@nd.gov");

            // Get form id number
            string id = idNum.Rows[0][0].ToString();

            // Create email to superusers notifying of form submission
            string[] to = { "rpmaddock@nd.gov", "mwood@nd.gov", "dosmond@nd.gov" };
            string toAddress = string.Join("; ", toEmailList.ToArray());
            string from = "wsinoreply@nd.gov";
            string subject = "A Request for Charitable Event form has been submitted";

            string body = "A request for Charitable Event form has been submitted for " + collection["CharityInfoModel.Organization"] + ". Please click on the link below to access the form. "
            + "<a href=\"" + host[0].ToString() + "SFN61579/View/" + id + "\">Charitable Event Form Link " + "</a>";

            // Send email
            dal = new ModelDALSFN61579();
            dal.SendEmail(toAddress, from, subject, body); ;

            //Redirect back to form landing page
            return Redirect("~/SFN61579");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Reviewed(FormCollection collection)
        {
            dal = new ModelDALSFN61579();
            // Set reviewed indicator to 'y'
            dal.UpdateSFN61579Reviewed(collection);

            // Redirect back to form landing page
            return Redirect("~/SFN61579");
        }
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
        private bool isInGroup(DirectoryEntry user,string groupName)
        {
            Object[] userProp = (Object[])user.Properties[ADUserProperties.MEMBEROF].Value;
            foreach(Object ob in userProp)
            if (ob.ToString().Contains(groupName))
            {
                return true;
            }
            return false;
        }
    }
}