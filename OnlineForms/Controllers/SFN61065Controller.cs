using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Management;
using System.Web.Mvc;
using OnlineForms.ViewModels;
using OnlineForms.Models;
using OnlineForms.Helper;
using System.Data;
using Microsoft.SharePoint.Client;
using Microsoft.VisualBasic;
using SP = Microsoft.SharePoint.Client;
using FormCollection = System.Web.Mvc.FormCollection;
using System.Security;
using System.Security.Principal;
using System.Net.Http;
using OnlineForms.Models.SFN61579;
using System.Configuration;
using OnlineForms.Helper;
using System.Web;
using System.IO;
using Microsoft.Office.SharePoint.Tools;
using OnlineForms.Logging;
using System.Runtime.Serialization.Formatters.Binary;

namespace OnlineForms.Controllers
{
    [Authorize]
    public class SFN61065Controller : Controller
    {
        private ModelDALSFN61065 dal = new ModelDALSFN61065();

        // GET: Forms
        public static LoggingService log = new LoggingService();
        //private bool _superUser = false;
        //private bool _multimediaSpecialist = false;
        //private bool _procurementOfficer = false;
        public ActionResult Index(FormCollection collection)
        {
            log.LogMessage("SFN61065 Index function started");
            ViewBag.FormInfo = dal.GetFormInfoSFN61065("SFN61065");
            dal = new ModelDALSFN61065();

            // Create domain context
            PrincipalContext ctx = new PrincipalContext(ContextType.Domain);

            UserPrincipal u = new UserPrincipal(ctx);

            UserPrincipal user = UserPrincipal.FindByIdentity(ctx, User.Identity.Name);

            // Get user's job title
            DirectoryEntry dirEntry = (DirectoryEntry)user.GetUnderlyingObject();
            string title = dirEntry.Properties["Title"].Value.ToString();

            // Get personel
            List<string> personelnames = new List<string>();
            personelnames = DirectoryHelper.getPersonel(user.SamAccountName);

            if (title == "Procurement Officer" || title == "Multimedia Specialist" || title == "Director of Finance" || personelnames.ToArray().Length > 0)
            {
                ViewBag.HasApprovals = true;
            }

            // Set superuser access
            if (title == "Procurement Officer" || title == "Multimedia Specialist")
            {
                ViewBag.Access = true;
            }

            ViewBag.Username = user.DisplayName;
            string company = dirEntry.Properties["Company"].Value.ToString();

            // Get form info
            DataTable dtInfo = dal.GetSFN61065Info();
            SFN61065DisplayViewModel vmSFN61065 = new SFN61065DisplayViewModel();
            vmSFN61065.BusinessCardModel = Models.SFN61065.SFN61065Model.ConvertDataTableToBusinessCardRequest(dtInfo);
            vmSFN61065.BusinessCardInfo = Models.SFN61065.SFN61065BusinessCardInfo.ConvertDataTableToBusinessCardInfo(dtInfo);

            dal = new ModelDALSFN61065();

            return View("~/Views/Forms/SFN61065/Index.cshtml", vmSFN61065);
        }
        
        public ActionResult MyApprovals(FormCollection collection)
        {
            ViewBag.FormInfo = dal.GetFormInfo("SFN61065");
            dal = new ModelDALSFN61065();
            // ViewBag.CharitableInfo = dal.GetSFN61579Info("101");

            DataTable dtInfo = dal.GetSFN61065Info();
            SFN61065DisplayViewModel vmSFN61065 = new SFN61065DisplayViewModel();
            vmSFN61065.BusinessCardModel = Models.SFN61065.SFN61065Model.ConvertDataTableToBusinessCardRequest(dtInfo);
            vmSFN61065.BusinessCardInfo = Models.SFN61065.SFN61065BusinessCardInfo.ConvertDataTableToBusinessCardInfo(dtInfo);

            // Create domain context
            PrincipalContext ctx = new PrincipalContext(ContextType.Domain);

            UserPrincipal u = new UserPrincipal(ctx);

            UserPrincipal user = UserPrincipal.FindByIdentity(ctx, User.Identity.Name);

            // Get user's title
            DirectoryEntry dirEntry = (DirectoryEntry)user.GetUnderlyingObject();
            string title = dirEntry.Properties["title"].Value.ToString();

            // Get personel
            List<string> personelnames = new List<string>();
            personelnames = DirectoryHelper.getPersonel(user.SamAccountName);

            if (title == "Procurement Officer" || title == "Multimedia Specialist" || title == "Director of Finance" || personelnames.ToArray().Length > 0)
            {
                ViewBag.HasApprovals = true;
            }

            ViewBag.Username = user.DisplayName;
            ViewBag.Title = title;

            string company = dirEntry.Properties["Company"].Value.ToString();
            if (company == "Workforce Safety & Insurance")
            {
                ViewBag.Company = true;
            }
            dal = new ModelDALSFN61065();

           return View("~/Views/Forms/SFN61065/MyApprovals.cshtml", vmSFN61065);
        }
        
        public ActionResult View(int id, FormCollection collection)
        {
            ViewBag.FormInfo = dal.GetFormInfo("SFN61065");

            // Get form info
            dal = new ModelDALSFN61065();
            DataTable dtInfo = dal.GetSFN61065InfoByID(id);
            SFN61065DisplayViewModel vmSFN61065 = new SFN61065DisplayViewModel();
            vmSFN61065.BusinessCardModels = Models.SFN61065.SFN61065Model.ConvertDataTableToBusinessCardRequests(dtInfo);
            dal = new ModelDALSFN61065();

            vmSFN61065.BusinessCardInfos = Models.SFN61065.SFN61065BusinessCardInfo.ConvertDataTableToBusinessCardInfos(dtInfo);
            dal = new ModelDALSFN61065();

            vmSFN61065.Approvals = Models.SFN61065.SFN61065Approval.ConvertDataTableToApprovals(dtInfo);
            dal = new ModelDALSFN61065();

            ViewBag.ID = id;

            // Create domain context
            PrincipalContext ctx = new PrincipalContext(ContextType.Domain, "nd.gov", "OU=WSI, DC=nd,DC=gov");

            UserPrincipal u = new UserPrincipal(ctx);

            UserPrincipal user = UserPrincipal.FindByIdentity(ctx, User.Identity.Name) ;

            // Get user's title
            DirectoryEntry dirEntry = (DirectoryEntry)user.GetUnderlyingObject();
            string title = dirEntry.Properties["Title"].Value.ToString();

            // If user's title is Multimedia Specialist, set IsCommunications to true
            if (title == "Multimedia Specialist")
            {
                ViewBag.IsCommunications = true;
            }

            // If user's title is Procurement Officer set IsProcurement to true
            if (title == "Procurement Officer")
            {
                ViewBag.IsProcurement = true;
            }

            // Get personel
            List<string> personelnames = new List<string>();
            personelnames = DirectoryHelper.getPersonel(user.SamAccountName);

            if (title == "Procurement Officer" || title == "Multimedia Specialist" || title == "Director of Finance" || personelnames.ToArray().Length > 0)
            {
                ViewBag.HasApprovals = true;
            }

            ViewBag.Username = user.DisplayName;

            // Get User's company
            string company = dirEntry.Properties["Company"].Value.ToString();
            if (company == "Workforce Safety & Insurance")
            {
                ViewBag.Company = true;
            }

            dal = new ModelDALSFN61065();
            // Get requestor name from table
           DataTable idNum = dal.GetSFN61065Requestor(id);
            string requestor = idNum.Rows[0][0].ToString();

            // Find requestor in AD
            //UserPrincipal findRequestor = UserPrincipal.FindByIdentity(ctx, requestor);
            //DirectoryEntry requestorName = (DirectoryEntry)findRequestor.GetUnderlyingObject();

            UserPrincipal requestorPrincipal = new UserPrincipal(ctx) { DisplayName = requestor };
            requestorPrincipal.Enabled = true;
            List<System.DirectoryServices.AccountManagement.Principal> results = new List<System.DirectoryServices.AccountManagement.Principal>();
            var searcher = new PrincipalSearcher(requestorPrincipal);
            results.AddRange(searcher.FindAll());
            UserPrincipal findRequestor = (UserPrincipal)results[0];
            DirectoryEntry enTry = new DirectoryEntry("LDAP://OU=WSI,DC=nd,DC=gov");
            DirectoryEntry requestorName = (DirectoryEntry)findRequestor.GetUnderlyingObject();

            if (requestorName == null)
            {

            }

            if (requestorName.Properties["title"].Value.ToString() != "Director")
            {
                // Find requestor's manager in AD
                string managerDistinguishedName = requestorName.Properties["manager"].Value.ToString();
                UserPrincipal manager = UserPrincipal.FindByIdentity(ctx, managerDistinguishedName);

                ViewBag.Manager = manager.DisplayName;
            }

            // If user's name is the same as the requestor's manager, CanApprove is set to true
            if (ViewBag.Username == ViewBag.Manager)
            {
                ViewBag.CanApprove = true;
            } else if (title == "Director of Finance" && requestorName.Properties["title"].Value.ToString() == "Director")
            {
                ViewBag.CanApprove = true;
            }

            // Determine which uploaded file goes with form
            string filename = "BusinessCardProof" + id + ".pdf";
            string serverpath = Server.MapPath("~/Uploads/");
            string filepath = serverpath + filename;

            if (System.IO.File.Exists(filepath))
            {
                ViewBag.FileExists = true;
            }

            // Set variable with user's job title
            ViewBag.JobTitle = title;

            return View("~/Views/Forms/SFN61065/View.cshtml", vmSFN61065);
        }

        public ActionResult Print(int id)
        {
            ViewBag.FormInfo = dal.GetFormInfo("SFN61065");

            dal = new ModelDALSFN61065();
            DataTable dtInfo = dal.GetSFN61065InfoByID(id);
            SFN61065DisplayViewModel vmSFN61065 = new SFN61065DisplayViewModel();
            vmSFN61065.BusinessCardModels = Models.SFN61065.SFN61065Model.ConvertDataTableToBusinessCardRequests(dtInfo);
            dal = new ModelDALSFN61065();

            vmSFN61065.BusinessCardInfos = Models.SFN61065.SFN61065BusinessCardInfo.ConvertDataTableToBusinessCardInfos(dtInfo);
            dal = new ModelDALSFN61065();

            vmSFN61065.Approvals = Models.SFN61065.SFN61065Approval.ConvertDataTableToApprovals(dtInfo);
            dal = new ModelDALSFN61065();

            // Get form values
            ViewBag.Values = dal.GetSFN61065Values(id);
            ViewBag.ID = id;
            PrincipalContext ctx = new PrincipalContext(ContextType.Domain);

            UserPrincipal u = new UserPrincipal(ctx);

            UserPrincipal user = UserPrincipal.FindByIdentity(ctx, User.Identity.Name);

            ViewBag.Username = user.DisplayName;
            return View("~/Views/Forms/SFN61065/Print.cshtml", vmSFN61065);
        }

        // Open proof PDF
        public FileResult Open(int id, DataTable dataTable)
        {

            dal = new ModelDALSFN61065();
            DataTable proof = dal.GetSFN61065ProofByID(id);
            DataRow row = proof.Rows[0];
           

            byte[] data = (byte[])row[0];


            
            return new FileContentResult(data, "application/pdf");
        }
        public ActionResult Create()
        {
            // Get form info from db
            ViewBag.FormInfo = dal.GetFormInfo("SFN61065");

            // Create domain context
            PrincipalContext ctx = new PrincipalContext(ContextType.Domain);

            UserPrincipal u = new UserPrincipal(ctx);

            //UserPrincipal user = UserPrincipal.Current;
            UserPrincipal user = UserPrincipal.FindByIdentity(ctx, User.Identity.Name);

            // Get user's department
            DirectoryEntry dirEntry = (DirectoryEntry)user.GetUnderlyingObject();
            string dept = dirEntry.Properties["Department"].Value.ToString();

            // Get user's title
            string title = dirEntry.Properties["Title"].Value.ToString();

            string company = dirEntry.Properties["Company"].Value.ToString();
            if (company == "Workforce Safety & Insurance")
            {
                ViewBag.Company = true;
            }

            // Get personel
            List<string> personelnames = new List<string>();
            personelnames = DirectoryHelper.getPersonel(user.SamAccountName);

            if (title == "Procurement Officer" || title == "Multimedia Specialist" || title == "Director of Finance" || personelnames.ToArray().Length > 0)
            {
                ViewBag.HasApprovals = true;
            }

            // Set variables with user's info
            ViewBag.Username = user.DisplayName;
            string name = user.DisplayName;
            ViewBag.Email = user.EmailAddress;
            ViewBag.Department = dept;
            ViewBag.Phone = user.VoiceTelephoneNumber;
            dal = new ModelDALSFN61065();

            // Get most recent form's info
            DataTable dtInfo = dal.GetSFN61065InfoByMostRecent(name);
            SFN61065ViewModel vmSFN61065 = new SFN61065ViewModel();

           if (dtInfo != null) {
                if(dtInfo.Rows.Count > 0)
                {
                    vmSFN61065.BusinessCardModel = Models.SFN61065.SFN61065Model.ConvertDataTableToBusinessCardRequests(dtInfo);
                    dal = new ModelDALSFN61065();
                    vmSFN61065.BusinessCardInfo = Models.SFN61065.SFN61065BusinessCardInfo.ConvertDataTableToBusinessCardInfos(dtInfo);
                    dal = new ModelDALSFN61065();
                    ViewBag.FirstName = dtInfo.Rows[0]["FIRST_NAME"].ToString();
                    ViewBag.LastName = dtInfo.Rows[0]["LAST_NAME"].ToString();
                    ViewBag.Credentials = dtInfo.Rows[0]["CREDENTIALS"].ToString();
                    ViewBag.RequestorTitle = dtInfo.Rows[0]["TITLE"].ToString();
                    ViewBag.CurrentEmail = dtInfo.Rows[0]["EMAIL"].ToString();
                    ViewBag.Telephone = dtInfo.Rows[0]["TELEPHONE_NUMBER"].ToString();
                    ViewBag.Cell = dtInfo.Rows[0]["CELL_NUMBER"].ToString();
                    ViewBag.Fax = dtInfo.Rows[0]["FAX_NUMBER"].ToString();
                }
                
            }
            return View("~/Views/Forms/SFN61065/Create.cshtml");
        }

        //private void isSuperUser(UserPrincipal user)
        //{
        //    dal = new ModelDALSFN61065();
        //    DataTable dtInfo = dal.GetSuperUsers("SFN 61065");
        //    foreach (DataRow dr in dtInfo.Rows)
        //    {
        //        string login = dr["SU_LOGIN"].ToString();
        //        string title = dr["SU_TITLE"].ToString();
        //        if (login.Equals(user.SamAccountName))
        //        {
        //            _superUser = true;
        //        }
        //        else if (title.Length > 0)
        //        {
        //            List<string> memberLogin = DirectoryHelper.GetListOfAdUsersByProperty("nd", title, ADUserProperties.TITLE);
        //            foreach (string member in memberLogin)
        //            {
        //                if (member.Equals(user.SamAccountName))
        //                {
        //                    _superUser = true;
        //                    break;
        //                }
        //            }
        //        }
        //        if (_superUser)
        //            break;
        //    }
        //    if (DirectoryHelper.getJobTitle(user).Equals("Multimedia Specialist"))
        //    {
        //        _multimediaSpecialist = true;
        //    }
        //    if(DirectoryHelper.getJobTitle(user).Equals("Procurement Officer"))
        //    {
        //        _procurementOfficer = true;
        //    }
        //    //if (DirectoryHelper.findDepartment(user).Equals("WSI-Human Resources"))
        //    //{
        //    //    _inHR = true;
        //    //}
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SFN61065(FormCollection collection)
        {
            string environment = ConfigurationManager.AppSettings["Environment"];
            // Create domain context
            PrincipalContext ctx = new PrincipalContext(ContextType.Domain);

            UserPrincipal u = new UserPrincipal(ctx);

            UserPrincipal user = UserPrincipal.FindByIdentity(ctx, User.Identity.Name);

            DirectoryEntry enTry = new DirectoryEntry("LDAP://OU=WSI,DC=nd,DC=gov");
            DirectorySearcher mySearcher = new DirectorySearcher();
            SearchResultCollection results;
            var toEmailList = new List<string>();
            var waitingApprovalNames = new List<string>();

            DirectoryEntry dirEntry = (DirectoryEntry)user.GetUnderlyingObject();

            // Insert info into db
            // Get user's title
            string title = dirEntry.Properties["Title"].Value.ToString();
            ViewBag.Title = title;
            if (title == "Director")
            {
                // Find users with the job title Procurement Officer
                    mySearcher = new DirectorySearcher(enTry);
                    mySearcher.Filter = "(&(company=Workforce Safety & Insurance)(title=" + "Director of Finance" + "))";
                    results = mySearcher.FindAll();

                    // Add Procurment Officer email to list
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
                                    waitingApprovalNames.Add(searchResult.Properties["displayname"][0].ToString());
                                }

                            }
                            else
                            {
                                toEmailList.Add(email);
                                waitingApprovalNames.Add(searchResult.Properties["displayname"][0].ToString());
                            }
                        }
                        }
                    }
            } else
            {
                // Find requestor's manager
                
                string managerDistinguishedName = dirEntry.Properties["manager"][0].ToString();
                UserPrincipal manager = UserPrincipal.FindByIdentity(ctx, managerDistinguishedName);
                string managerEmail = manager.EmailAddress;
                toEmailList.Add(managerEmail);
                waitingApprovalNames.Add(manager.DisplayName);
            }

            string combinedNames = string.Join("; ", waitingApprovalNames.ToArray());
            int sfn61065ID = dal.InsertSFN61065GetID(collection, combinedNames);

            dal.InsertSFN61065BusinessCardInfo(collection, sfn61065ID);

            // Get form id number
            dal = new ModelDALSFN61065();
            DataTable idNum = dal.GetSFN61065InfoByMaxID();

            string id = idNum.Rows[0][0].ToString();

            // Insert id into approvals table
            dal = new ModelDALSFN61065();
            dal.InsertSFN61065Approval(int.Parse(id));

            // Get form info
            ViewBag.FormInfo = dal.GetFormInfoSFN61065("SFN61065");

            // Get form url path
            string path = HttpContext.Request.Url.AbsoluteUri.ToString();
            List<string> host = new List<string>(path.Split(new string[] { "SFN" }, StringSplitOptions.None));

            // Email configurations
            // Email to communications to notify of supervisor approval

            string toAddress = string.Join("; ", toEmailList.ToArray()); ;
            string from = "wsinoreply@nd.gov";
            string subject = "A Business Card Request form has been submitted";

            // Email to supervisor for approval
            string body = "A Business Card Request form has been submitted by " + collection["BusinessCardModel.Name"] + " for your approval. Click here to view the form: " + "<a href=\""
                + host[0].ToString() + "SFN61065/View/" + id + "\">Business Card Request Link " + "</a>";

            // Send email
            dal = new ModelDALSFN61065();
            dal.SendEmail(toAddress, from, subject, body);

            return Redirect("~/SFN61065");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Upload(FormCollection collection, HttpPostedFileBase postedFile)
        {
            // Create domain context
            PrincipalContext ctx = new PrincipalContext(ContextType.Domain);

            UserPrincipal u = new UserPrincipal(ctx);

            UserPrincipal user = UserPrincipal.FindByIdentity(ctx, User.Identity.Name);

            // Get form id number
            DataTable idNum = dal.GetSFN61065ID(collection);
            string id = idNum.Rows[0][0].ToString();

            dal = new ModelDALSFN61065();
            // Get requestor name from table
            DataTable requestorValue = dal.GetSFN61065Requestor(int.Parse(id));
            string requestor = requestorValue.Rows[0][0].ToString();

            // Find requestor in AD
            //UserPrincipal findRequestor = UserPrincipal.FindByIdentity(ctx, requestor);
            //DirectoryEntry requestorName = (DirectoryEntry)findRequestor.GetUnderlyingObject();
            UserPrincipal requestorPrincipal = new UserPrincipal(ctx) { DisplayName = requestor };
            requestorPrincipal.Enabled = true;
            List<System.DirectoryServices.AccountManagement.Principal> requestorResults = new List<System.DirectoryServices.AccountManagement.Principal>();
            var searcher = new PrincipalSearcher(requestorPrincipal);
            requestorResults.AddRange(searcher.FindAll());
            UserPrincipal findRequestor = (UserPrincipal)requestorResults[0];
            DirectoryEntry requestorName = (DirectoryEntry)findRequestor.GetUnderlyingObject();

            ViewBag.RequestorEmail = findRequestor.EmailAddress;

            dal = new ModelDALSFN61065();
            
            if (postedFile.ContentLength > 0 && postedFile.ContentType == "application/pdf")
                {
                    string _FileName = Path.GetFileName(postedFile.FileName);
                    MemoryStream target = new MemoryStream();
                    postedFile.InputStream.CopyTo(target);
                    byte[] data = target.ToArray();
                //the field "data" goes into DB.

                dal.UploadProof(int.Parse(id), data);
                dal = new ModelDALSFN61065();
                dal.UpdateSFN61065ProofUploaded(int.Parse(id));
            }
            

            // Get form url path
            string path = HttpContext.Request.Url.AbsoluteUri.ToString();
            List<string> host = new List<string>(
                path.Split(new string[] { "SFN" }, StringSplitOptions.None));

            // Email to requestor about proof

            //  NEED TO PULL REQUESTOR EMAIL AND POSSIBLY MOVE INTO IF STATEMENT
            string[] to = { findRequestor.EmailAddress };
            string toAddress = string.Join("; ", to.ToArray());
            string from = "wsinoreply@nd.gov";
            string subject = "Communications has prepared a proof of your business card";

            string body = "Communications has prepared a proof of your business card. Click on the link to review and approve/decline the proof: "
            + "<a href=\"" + host[0].ToString() + "SFN61065/View/" + id + "\">Business Card Request Form Link" + "</a>";

            // Send email
            dal = new ModelDALSFN61065();
            dal.SendEmail(toAddress, from, subject, body);

            dal = new ModelDALSFN61065();
            dal.UpdateSFN61065WaitingApproval(collection, findRequestor.DisplayName);

            dal = new ModelDALSFN61065();
            dal.UpdateSFN61065ProofComments(collection);
            
            

            return Redirect("~/SFN61065");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Approval(FormCollection collection, string Command)
        {
            string environment = ConfigurationManager.AppSettings["Environment"];
            // Create domain context
            PrincipalContext ctx = new PrincipalContext(ContextType.Domain, "nd.gov", "OU=WSI, DC=nd,DC=gov");

            UserPrincipal u = new UserPrincipal(ctx);

            UserPrincipal user = UserPrincipal.FindByIdentity(ctx, User.Identity.Name);

            // Create directry searcher
            DirectoryEntry enTry = new DirectoryEntry("LDAP://OU=WSI,DC=nd,DC=gov");
            DirectorySearcher mySearcher = new DirectorySearcher();
            SearchResultCollection results;

            // Get user's department
            DirectoryEntry dirEntry = (DirectoryEntry)user.GetUnderlyingObject();
            string dept = dirEntry.Properties["Department"].Value.ToString();

            // Get user's title
            string title = dirEntry.Properties["Title"].Value.ToString();
            ViewBag.Title = title;

            // Get form id number
            DataTable idNum = dal.GetSFN61065ID(collection);
            string id = idNum.Rows[0][0].ToString();
            ViewBag.Username = user.DisplayName;

            dal = new ModelDALSFN61065();

            // Get requestor name from table
            DataTable requestorValue = dal.GetSFN61065Requestor(int.Parse(id));
            string requestor = requestorValue.Rows[0][0].ToString();

            // Find requestor in AD
            //UserPrincipal findRequestor = UserPrincipal.FindByIdentity(ctx, requestor);
            //DirectoryEntry requestorName = (DirectoryEntry)findRequestor.GetUnderlyingObject();

            UserPrincipal requestorPrincipal = new UserPrincipal(ctx) { DisplayName = requestor };
            requestorPrincipal.Enabled = true;
            List<System.DirectoryServices.AccountManagement.Principal> requestorResults = new List<System.DirectoryServices.AccountManagement.Principal>();
            var searcher = new PrincipalSearcher(requestorPrincipal);
            requestorResults.AddRange(searcher.FindAll());
            UserPrincipal findRequestor = (UserPrincipal)requestorResults[0];
            DirectoryEntry requestorName = (DirectoryEntry)findRequestor.GetUnderlyingObject();
            ViewBag.RequestorEmail = findRequestor.EmailAddress;

            UserPrincipal manager = findRequestor;
            
            if (requestorName.Properties["title"].Value.ToString() != "Director")
            {
                // Find requestor's manager in AD
                string managerDistinguishedName = requestorName.Properties["manager"][0].ToString();
                manager = UserPrincipal.FindByIdentity(ctx, managerDistinguishedName);
                if (ViewBag.Username == manager.DisplayName)
                {
                    ViewBag.IsManager = true;
                }
            }if (requestorName.Properties["title"].Value.ToString() == "Director" && title == "Director of Finance")
            {
                ViewBag.IsManager = true;
            }

            var waitingApprovalNames = new List<string>();

            // Get form url path
            string path = HttpContext.Request.Url.AbsoluteUri.ToString();
            List<string> host = new List<string>(
                path.Split(new string[] { "SFN" }, StringSplitOptions.None));

            // Supervisor approval emails
            if (ViewBag.IsManager == true)
            {
                if (Command == "Approve")
                {
                    // Set supervisor approval
                    collection["Approvals.SupervisorApproved"] = "true";
                    collection["Approvals.SupervisorApproval"] = user.DisplayName;
                    collection["Approvals.SupervisorApprovalDate"] = DateTime.Today.ToString("MM/dd/yyyy");
                    var toEmailList = new List<string>();

                    // Update approval table in db
                    dal = new ModelDALSFN61065();
                    dal.UpdateSFN61065SupervisorApproval(collection);

                    // Search for users with a title of Multimedia Specialist
                    //mySearcher = new DirectorySearcher(enTry);
                    //mySearcher.Filter = "(&(company=Workforce Safety & Insurance)(title=" + "Multimedia Specialist" + "))";
                    //results = mySearcher.FindAll();

                    //// Add Multimedia Specialist email to list
                    //if (results.Count > 0)
                    //{
                    //    foreach (SearchResult searchResult in results)
                    //    {
                    //        if (searchResult.Properties.Contains("mail"))
                    //        {
                    //            string email = searchResult.Properties["mail"][0].ToString();
                    //            if (environment.ToUpper() == "PROD")
                    //            {
                    //                if (email.Contains("wsi_") || email.Contains("wsitrain"))
                    //                {

                    //                }

                    //                else
                    //                {
                    //                    toEmailList.Add(email);
                    //                    waitingApprovalNames.Add(searchResult.Properties["displayname"][0].ToString());
                    //                }

                    //            }
                    //            else
                    //            {
                    //                toEmailList.Add(email);
                    //                waitingApprovalNames.Add(searchResult.Properties["displayname"][0].ToString());
                    //            }

                    //        }
                    //    }
                    //}
                    
                    // Email to communications to notify of supervisor approval

                    string toAddress = "lroberson@nd.gov";
                    string from = "wsinoreply@nd.gov";
                    string subject = "A Business Card Request form has been submitted";

                    string body = "A Business Card Request form has been submitted by " + collection["BusinessCardModels.Name"] + " and approved. Click here to view the form: "
                    + "<a href=\"" + host[0].ToString() + "SFN61065/View/" + id + "\">Business Card Request Form Link " + "</a>";

                    // Send email
                    dal = new ModelDALSFN61065();
                    dal.SendEmail(toAddress, from, subject, body);

                    List<string> communications = new List<string>(DirectoryHelper.GetListOfAdUsersByProperty("nd", "Multimedia Specialist", ADUserProperties.TITLE));
                    var communicationsNames = new List<string>();
                    foreach (string item in communications)
                    {

                        string multimediaSpecialistName = DirectoryHelper.findName(item);
                        if (!multimediaSpecialistName.Contains("WSI, Train"))
                        {
                            communicationsNames.Add(multimediaSpecialistName);
                        }
                        
                    }

                    string combinedNames = string.Join("; ", waitingApprovalNames.ToArray());
                    
                    dal = new ModelDALSFN61065();
                    dal.UpdateSFN61065WaitingApproval(collection, combinedNames);
                }

                else if (Command == "Deny")
                {
                    // Update approvals table in db
                    collection["Approvals.SupervisorApproved"] = "false";
                    collection["Approvals.SupervisorApproval"] = "Denied";
                    collection["Approvals.SupervisorApprovalDate"] = DateTime.Today.ToString("MM/dd/yyyy");
                    dal = new ModelDALSFN61065();
                    dal.UpdateSFN61065SupervisorApproval(collection);

                    // Email to user to notify of denial
                    string[] to = { ViewBag.RequestorEmail };
                    string toAddress = string.Join("; ", to.ToArray());
                    string from = "wsinoreply@nd.gov";
                    string subject = "Your supervisor has reviewed your business card request";

                    string body = "Your Business Card Request has been reviewed by your supervisor and was denied. "
                    + "<a href=\"" + host[0].ToString() + "SFN61065/View/" + id + "\">Business Card Request Form Link " + "</a>";

                    // Send email
                    dal = new ModelDALSFN61065();
                    dal.SendEmail(toAddress, from, subject, body);

                    dal = new ModelDALSFN61065();
                    dal.UpdateSFN61065WaitingApproval(collection, "");
                }
            }

            // When employee sets approval
            else if (user.DisplayName == collection["BusinessCardModels.Name"])
            {
                if (Command == "Approve")
                {
                    collection["Approvals.RequestingEmployeeApproval"] = user.DisplayName;
                    collection["Approvals.RequestingEmployeeApproved"] = "true";
                    collection["Approvals.RequestingEmployeeApprovalDate"] = DateTime.Today.ToString("MM/dd/yyyy");
                    collection["Approvals.ProofComments"] = "";
                    // Update approvals table in db
                    dal = new ModelDALSFN61065();
                    dal.UpdateSFN61065RequestingEmployeeApproval(collection);

                    // Create email list
                    var toEmailList = new List<string>();

                    // Find users with the job title Multimedia Specialist
                    //mySearcher = new DirectorySearcher(enTry);
                    //mySearcher.Filter = "(&(company=Workforce Safety & Insurance)(title=" + "Multimedia Specialist" + "))";
                    //results = mySearcher.FindAll();

                    //// Add Multimedia Specialist email to list
                    //if (results.Count > 0)
                    //{
                    //    foreach (SearchResult searchResult in results)
                    //    {
                    //        if (searchResult.Properties.Contains("mail"))
                    //        {
                    //            string email = searchResult.Properties["mail"][0].ToString();
                    //            if (environment.ToUpper() == "PROD")
                    //            {
                    //                if (email.Contains("wsi_") || email.Contains("wsitrain"))
                    //                {

                    //                }

                    //                else
                    //                {
                    //                    toEmailList.Add(email);
                    //                    waitingApprovalNames.Add(searchResult.Properties["displayname"][0].ToString());
                    //                }

                    //            }
                    //            else
                    //            {
                    //                toEmailList.Add(email);
                    //                waitingApprovalNames.Add(searchResult.Properties["displayname"][0].ToString());
                    //            }
                    //        }
                    //    }
                    //}

                    // Email to communications notifying them that the employee has accepted the business card proof
                    string toAddress = "lroberson@nd.gov";
                    string from = "wsinoreply@nd.gov";
                    string subject = collection["BusinessCardModels.Name"] + " has approved the business card proof";

                    string body = collection["BusinessCardModels.Name"] + " has approved the business card proof: "
                    + "<a href=\"" + host[0].ToString() + "SFN61065/View/" + id + "\">Business Card Request Form Link " + "</a>";

                    // Send email
                    dal = new ModelDALSFN61065();
                    dal.SendEmail(toAddress, from, subject, body);

                    List<string> communications = new List<string>(DirectoryHelper.GetListOfAdUsersByProperty("nd", "Multimedia Specialist", ADUserProperties.TITLE));
                    var communicationsNames = new List<string>();
                    foreach (string item in communications)
                    {
                        string multimediaSpecialistName = DirectoryHelper.findName(item);
                        if (!multimediaSpecialistName.Contains("WSI, Train"))
                        {
                            communicationsNames.Add(multimediaSpecialistName);
                        }
                        
                    }

                    string combinedNames = string.Join("; ", waitingApprovalNames.ToArray());

                    dal = new ModelDALSFN61065();
                    dal.UpdateSFN61065WaitingApproval(collection, combinedNames);
                }
                // If employee denies proof
                else if (Command == "Confirm")
                {
                    collection["Approvals.RequestingEmployeeApproval"] = "Denied";
                    collection["Approvals.RequestingEmployeeApproved"] = "false";
                    collection["Approvals.RequestingEmployeeApprovalDate"] = DateTime.Today.ToString("MM/dd/yyyy");
                    // Update approvals table in db
                    dal = new ModelDALSFN61065();
                    dal.UpdateSFN61065RequestingEmployeeApproval(collection);

                    // Create email list
                    var toEmailList = new List<string>();

                    // Find users with the job title Multimedia Specialist
                    //mySearcher = new DirectorySearcher(enTry);
                    //mySearcher.Filter = "(&(company=Workforce Safety & Insurance)(title=" + "Multimedia Specialist" + "))";
                    //results = mySearcher.FindAll();

                    //// Add Multimedia Specialist email to list
                    //if (results.Count > 0)
                    //{
                    //    foreach (SearchResult searchResult in results)
                    //    {
                    //        if (searchResult.Properties.Contains("mail"))
                    //        {
                    //            string email = searchResult.Properties["mail"][0].ToString();
                    //            if (environment.ToUpper() == "PROD")
                    //            {
                    //                if (email.Contains("wsi_") || email.Contains("wsitrain"))
                    //                {

                    //                }

                    //                else
                    //                {
                    //                    toEmailList.Add(email);
                    //                    waitingApprovalNames.Add(searchResult.Properties["displayname"][0].ToString());
                    //                }

                    //            }
                    //            else
                    //            {
                    //                toEmailList.Add(email);
                    //                waitingApprovalNames.Add(searchResult.Properties["displayname"][0].ToString());
                    //            }
                    //        }
                    //    }
                    //}

                    string toAddress = "lroberson@nd.gov";
                    string from = "wsinoreply@nd.gov";
                    string subject = collection["BusinessCardModels.Name"] + " has denied the business card proof.";

                    // Email to communications notifying them that the employee has denied the business card proof
                    string body = collection["BusinessCardModels.Name"] + " has denied the business card proof: " + collection["Approvals.ProofComments"] +
                    ". Click here to view the form: " + "<a href=\"" + host[0].ToString() + "SFN61065/View/" + id 
                    + "\">Business Card Request Form Link " + "</a>";

                    // Send email
                    dal = new ModelDALSFN61065();
                    dal.SendEmail(toAddress, from, subject, body);

                    List<string> communications = new List<string>(DirectoryHelper.GetListOfAdUsersByProperty("nd", "Multimedia Specialist", ADUserProperties.TITLE));
                    var communicationsNames = new List<string>();
                    foreach (string item in communications)
                    {
                        string multimediaSpecialistName = DirectoryHelper.findName(item);
                        communicationsNames.Add(multimediaSpecialistName);
                    }

                    string combinedNames = string.Join("; ", waitingApprovalNames.ToArray());

                    dal = new ModelDALSFN61065();
                    dal.UpdateSFN61065WaitingApproval(collection, combinedNames);
                }
            }

                // Set reviewed indicator to 'y'
                // When communications sets approval...
                if (ViewBag.Title == "Multimedia Specialist")
            {
                // If Multimedia Specialist approves
                if (Command == "Completed")
                {
                    // Update approvals table in db
                    collection["Approvals.CommunicationsApproved"] = "true";
                    collection["Approvals.CommunicationsApproval"] = user.DisplayName;
                    collection["Approvals.CommunicationsApprovalDate"] = DateTime.Today.ToString("MM/dd/yyyy");
                    dal = new ModelDALSFN61065();
                    dal.UpdateSFN61065CommunicationsApproval(collection);

                    // Create email list
                    var toEmailList = new List<string>();

                    // Find users with the job title Procurement Officer
                    mySearcher = new DirectorySearcher(enTry);
                    mySearcher.Filter = "(&(company=Workforce Safety & Insurance)(title=" + "Procurement Officer" + "))";
                    results = mySearcher.FindAll();

                    // Add Procurment Officer email to list
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

                    // SEND EMAIL TO PROCUREMENT AFTER LISA APPROVES, NO PROCUREMENT APPROVAL NEEDED

                    // Email variables
                    string toAddress = string.Join("; ", toEmailList.ToArray());
                    string from = "wsinoreply@nd.gov";
                    string subject = "A Business Card Request has been completed";

                    // Email to procurement notifying them of business card request...
                    string body = "A business card request has been completed and is ready to be ordered: "
                    + "<a href=\"" + host[0].ToString() + "SFN61065/View/" + id + "\">Business Card Request Form Link " + "</a>";

                    // Send email
                    dal = new ModelDALSFN61065();
                    dal.SendEmail(toAddress, from, subject, body);

                    
                    // Email to requesting employee notifying them that their business cards will be ordered
                    string[] toRequestor = { ViewBag.RequestorEmail };
                    string toAddressRequestor = string.Join("; ", toRequestor.ToArray());
                    string fromRequestor = "wsinoreply@nd.gov";
                    string subjectRequestor = "Your Business Card Request has been approved";

                    // Email to procurement notifying them of business card request...
                    string bodyRequestor = "Your Business Card Request has been approved. Your cards will be ordered.";

                    dal = new ModelDALSFN61065();
                    dal.SendEmail(toAddressRequestor, fromRequestor, subjectRequestor, bodyRequestor);

                    dal = new ModelDALSFN61065();
                    dal.UpdateSFN61065WaitingApproval(collection, "");
                } 
                // If Multimedia Specialist denies
                else if (Command == "Deny")
                {
                    collection["Approvals.CommunicationsApproved"] = "false";
                    collection["Approvals.CommunicationsApproval"] = "Denied";
                    dal = new ModelDALSFN61065();
                    dal.UpdateSFN61065CommunicationsApproval(collection);

                    // Email variables
                    string[] to = { ViewBag.RequestorEmail };
                    string toAddress = string.Join("; ", to.ToArray());
                    string from = "wsinoreply@nd.gov";
                    string subject = "Your business card request has been denied";

                    // Email to requesting employee notifying them of business card proof
                    string body = "Your business card request has been denied.";

                    // Send email
                    dal = new ModelDALSFN61065();
                    dal.SendEmail(toAddress, from, subject, body);

                    dal = new ModelDALSFN61065();
                    dal.UpdateSFN61065WaitingApproval(collection, "");
                }
            }
            
            else if (ViewBag.Title == "Procurement Officer")
            {
                if (Command == "Approve")
                {
                    collection["Approvals.FinanceApproved"] = "true";
                    dal.UpdateSFN61065FinanceApproval(collection);

                    // NEED TO GRAB REQUESTOR EMAIL
                    string[] to = { ViewBag.RequestorEmail };
                    string toAddress = string.Join("; ", to.ToArray());
                    string from = "wsinoreply@nd.gov";
                    string subject = "Your Business Card Request has been approved";

                    // Email to procurement notifying them of business card request...
                    string body = "Your Business Card Request has been approved. Your cards will be ordered by Procurement.";

                    // Send email
                    dal = new ModelDALSFN61065();
                    dal.SendEmail(toAddress, from, subject, body);
                }
            }
            // Redirect back to form landing page
            return Redirect("~/SFN61065");
        }

        //public async System.Threading.Tasks.Task<ActionResult> Testing()
        //{
        //    //string contentString = await TestSharePointConnectionAsync("https://ndgov.sharepoint.com/sites/WSI-FormsDevelopment");

        //    string contentString = await TestSharepointGet();

        //    return Content(contentString);
        //}

        //public static async System.Threading.Tasks.Task<string> TestSharePointConnectionAsync(string sharepointUrl)
        //{
        //    SecureString passWord = new SecureString();
        //    foreach (char c in "share#2014".ToCharArray()) passWord.AppendChar(c);
        //    using (var auth = new PnP.Framework.AuthenticationManager("0ba7030e-6a25-47ef-86fe-4b77b7cfd955", "workflow@nd.gov", passWord))
        //    {
        //        using (var ctx = await auth.GetContextAsync(sharepointUrl))
        //        {
        //            ctx.Load(ctx.Web);
        //            ctx.ExecuteQuery();
        //            return ctx.Web.Title;
        //        }
        //    }
        //}

        //public static async System.Threading.Tasks.Task<string> TestSharepointGet()
        //{
        //    string content = string.Empty;
        //    //string siteURL = "https://ndgov.sharepoint.com/sites/WSI-Forms/CentralFormLists/Lists/Department/AllItems.aspx";
        //    string siteURL = "https://ndgov.sharepoint.com/sites/WSI-Forms/CentralFormLists";

        //    SecureString pWord = new SecureString();
        //    foreach (char c in "share#2014".ToCharArray()) pWord.AppendChar(c);
        //    using (var auth = new PnP.Framework.AuthenticationManager("0ba7030e-6a25-47ef-86fe-4b77b7cfd955", "workflow@nd.gov", pWord))
        //    {
        //        using (var ctx = await auth.GetContextAsync(siteURL))
        //        {
        //            Web web = ctx.Web;
        //            ListCollection collList = web.Lists;
        //            ctx.Load(collList);
        //            ctx.ExecuteQuery();
        //            SP.List deptList = collList.GetByTitle("Departments");
        //            ctx.Load(deptList);
        //            ctx.ExecuteQuery();

        //            CamlQuery query = new CamlQuery();
        //            query.ViewXml = "<View />";
        //            SP.ListItemCollection items = deptList.GetItems(query);
        //            ctx.Load(items);
        //            ctx.ExecuteQuery();

        //            foreach (SP.ListItem item in items)
        //            {
        //                SP.FieldUserValue user = (SP.FieldUserValue)item["Division_x0020_Chief"];
        //                content += user.LookupValue + "<br />";
        //            }

        //            return content;
        //        }
        //    }
        //}

        public static string GetRequisitionManagerEmail(float totalCost)
        {
            string managerEmail = string.Empty;
            string managerProps = string.Empty;
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

                    for (int i = 0; i < managerSteps; i++)
                    {
                        using (UserPrincipal managerUser = UserPrincipal.FindByIdentity(context, IdentityType.DistinguishedName, managerDistinguishedName))
                        {
                            managerEmail = managerUser.EmailAddress;

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
            catch (Exception e)
            {
                return "Unable to get manager's email<br />Error:" + e.Message;
            }
        }
    }
}