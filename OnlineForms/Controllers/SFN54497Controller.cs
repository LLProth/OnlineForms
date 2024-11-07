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
using OnlineForms.Models.SFN54497;
using System.Net.Mail;
using System.Diagnostics;
using OnlineForms.Helper;
using System.Configuration;
//using PnP.Framework.Extensions;
using System.Web.Services;

namespace OnlineForms.Controllers
{
    public class SFN54497Controller : Controller
    {
        private ModelDALSFN54497 dal = new ModelDALSFN54497();

        private bool _superUser = false;
        // GET: Forms
        [Authorize]
        public ActionResult Index()
        {
            // Get form info from db
            ViewBag.FormInfo = dal.GetFormInfoSFN55497("SFN54497");
            dal = new ModelDALSFN54497();
            
            // Get form info
            DataTable dtInfo = dal.GetSFN54497Info();
            SFN54497DisplayViewModel vmSFN54497 = new SFN54497DisplayViewModel();
            vmSFN54497.StaffRequestModel = Models.SFN54497.SFN54497Model.ConvertDataTableToStaffRequest(dtInfo);

            // Create domain context
            PrincipalContext ctx = new PrincipalContext(ContextType.Domain);

            UserPrincipal u = new UserPrincipal(ctx);

            //UserPrincipal user = UserPrincipal.Current;
            UserPrincipal user = UserPrincipal.FindByIdentity(ctx, User.Identity.Name);

            // Get user's department
            DirectoryEntry dirEntry = (DirectoryEntry)user.GetUnderlyingObject();
            string company = dirEntry.Properties["Company"].Value.ToString();
            // Get user's job title
            string title = dirEntry.Properties["Title"].Value.ToString();

            // Set variables
            ViewBag.Username = user.DisplayName;
            ViewBag.Company = company;
            ViewBag.Title = title;

            if (title == "Human Resource Officer" || title == "HR Director")
            {
                ViewBag.Access = true;
            }

            // Get personel
            List<string> personelnames = new List<string>();
            personelnames = DirectoryHelper.getPersonel(user.SamAccountName);

            if (title == "Human Resource Officer" || title == "HR Director" || title.Contains("Director") || title.Contains("Chief"))
            {
                ViewBag.HasApprovals = true;
            }

            dal = new ModelDALSFN54497();
            return View("~/Views/Forms/SFN54497/Index.cshtml", vmSFN54497);
        }

        [Authorize]
        public ActionResult View(int id, FormCollection collection)
        {
            // Get form info from db
            ViewBag.FormInfo = dal.GetFormInfo("SFN54497");

            dal = new ModelDALSFN54497();

            // Get individual form info
            DataTable dtInfo = dal.GetSFN54497InfoByID(id);
            SFN54497DisplayViewModel vmSFN54497 = new SFN54497DisplayViewModel();
            vmSFN54497.StaffRequestModels = SFN54497Model.ConvertDataTableToStaffRequests(dtInfo);
            dal = new ModelDALSFN54497();
            vmSFN54497.Approvals = Models.SFN54497.SFN54497Approval.ConvertDataTableToApprovals(dtInfo);
            dal = new ModelDALSFN54497();
            ViewBag.Values = dal.GetSFN54497Values(id);
            ViewBag.ID = id;
            PrincipalContext ctx = new PrincipalContext(ContextType.Domain);

            UserPrincipal u = new UserPrincipal(ctx);

            //UserPrincipal user = UserPrincipal.Current;
            UserPrincipal user = UserPrincipal.FindByIdentity(ctx, User.Identity.Name);

            // Get user's title
            DirectoryEntry dirEntry = (DirectoryEntry)user.GetUnderlyingObject();
            string title = dirEntry.Properties["Title"].Value.ToString();
            string dept = dirEntry.Properties["Department"].Value.ToString();

            // Get requestor name from table
            dal = new ModelDALSFN54497();
            DataTable idNum = dal.GetSFN54497Requestor(id);
            string requestor = idNum.Rows[0][0].ToString();

            UserPrincipal findRequestor = UserPrincipal.FindByIdentity(ctx, requestor);
            DirectoryEntry requestorName = (DirectoryEntry)findRequestor.GetUnderlyingObject();
            string submitterDepartment = requestorName.Properties["Department"].Value.ToString();

            //// Get requestor's department director
            DataTable idNumber = dal.GetSFN54497Director(id);
            string director = idNumber.Rows[0][0].ToString();

            if (director != "")
            {
                
                UserPrincipal findDirector = UserPrincipal.FindByIdentity(ctx, director);
                DirectoryEntry directorName = (DirectoryEntry)findDirector.GetUnderlyingObject();
                if (directorName.Properties["title"].Value.ToString() != "Director")
                {
                    // Find requestor's manager in AD
                    string managerDistinguishedName = directorName.Properties["manager"][0].ToString();
                    UserPrincipal managerName = UserPrincipal.FindByIdentity(ctx, managerDistinguishedName);
                    DirectoryEntry chief = (DirectoryEntry)managerName.GetUnderlyingObject();

                    string chiefName = chief.Properties["displayname"][0].ToString();

                    ViewBag.Chief = chiefName;
                }
                
            }

            ViewBag.Username = user.DisplayName;

            // Determine whether user can approve form
            if (title.Contains("Director") && dept == submitterDepartment)
            {
                ViewBag.IsDepartmentDirector = true;
            }

            if (title.Contains("Chief") && ViewBag.Username == ViewBag.Chief)
            {
                ViewBag.IsDivisionChief = true;
            }

            if (dept.Contains("Human Resources"))
            {
                ViewBag.IsHumanResources = true;
            }

            if (title.Contains("Director of Finance"))
            {
                ViewBag.IsFinanceDirector = true;
            }

            if (title.Equals("Director"))
            {
                ViewBag.IsAgencyDirector = true;
            }

            if (title == "Human Resource Officer" || title == "HR Director" || title.Contains("Director") || title.Contains("Chief"))
            {
                ViewBag.HasApprovals = true;
            }

            return View("~/Views/Forms/SFN54497/View.cshtml", vmSFN54497);
        }

        public ActionResult Print(int id)
        {
            ViewBag.FormInfo = dal.GetFormInfo("SFN54497");

            dal = new ModelDALSFN54497();
            DataTable dtInfo = dal.GetSFN54497InfoByID(id);
            SFN54497DisplayViewModel vmSFN54497 = new SFN54497DisplayViewModel();
            vmSFN54497.StaffRequestModels = SFN54497Model.ConvertDataTableToStaffRequests(dtInfo);
            dal = new ModelDALSFN54497();
            vmSFN54497.Approvals = Models.SFN54497.SFN54497Approval.ConvertDataTableToApprovals(dtInfo);
            dal = new ModelDALSFN54497();

            // Get form values
            ViewBag.Values = dal.GetSFN54497Values(id);
            ViewBag.ID = id;
            PrincipalContext ctx = new PrincipalContext(ContextType.Domain);

            UserPrincipal u = new UserPrincipal(ctx);

            UserPrincipal user = UserPrincipal.FindByIdentity(ctx, User.Identity.Name);

            ViewBag.Username = user.DisplayName;
            return View("~/Views/Forms/SFN54497/Print.cshtml", vmSFN54497);
        }

        [Authorize]
        public ActionResult Create()
        {
            ViewBag.FormInfo = dal.GetFormInfo("SFN54497");

            // Create domain context
            PrincipalContext ctx = new PrincipalContext(ContextType.Domain);

            UserPrincipal u = new UserPrincipal(ctx);

            //UserPrincipal user = UserPrincipal.Current;
            UserPrincipal user = UserPrincipal.FindByIdentity(ctx, User.Identity.Name);

            ViewBag.Username = user.DisplayName;
            ViewBag.Email = user.EmailAddress;

            DirectoryEntry enTry = new DirectoryEntry("LDAP://OU=USERS,OU=WSI,DC=nd,DC=gov");
            DirectorySearcher mySearcher = new DirectorySearcher();
            SearchResultCollection results;

            mySearcher = new DirectorySearcher(enTry);
            
            results = mySearcher.FindAll();
            var departments = new List<string>();
            departments.Add("");

            //// Get list of all departments from AD
            //if (results.Count > 0)
            //{
            //    foreach (SearchResult searchResult in results)
            //    {
            //        if (searchResult.Properties.Contains("department"))
            //        {
            //            string department = searchResult.Properties["department"][0].ToString();
            //            if (!departments.Contains(department) && department.Contains("WSI") && department != "WSI-IS")
            //            {
            //                departments.Add(department);
            //            }
                        
            //        }
            //    }
            //}

            // Get user's title
            DirectoryEntry dirEntry = (DirectoryEntry)user.GetUnderlyingObject();
            string title = dirEntry.Properties["Title"].Value.ToString();


            List<ADInfo> list = DirectoryHelper.GetAllADForCompany();
            List<string> names = new List<string>();

            foreach (ADInfo ad in list)
            {
                names.Add(ad.DisplayName);
            }
            names.Sort();
            ViewBag.WSIPersonnel = names;

            // Sort lists

            // Get list of all users from AD
            //mySearcher = new DirectorySearcher(enTry);
            //var names = new List<string>();
            //names.Add("");
            //mySearcher.Filter = "(&(objectCategory=person)(objectClass=user)(memberOf=*)(company=Workforce Safety & Insurance)(manager=*))";
            //results = mySearcher.FindAll();


            //if (results.Count > 0)
            //{
            //    foreach (SearchResult searchResult in results)
            //    {

            //        string name = searchResult.Properties["displayname"][0].ToString();

            //        names.Add(name);
            //    }
            //}

            departments.Sort();
            //names.Sort();

            ViewBag.Departments = departments;
            //ViewBag.PositionReportsTo = names;

            //string[] namesArray = names.ToArray();
            //ViewBag.NamesArray = namesArray;

            return View("~/Views/Forms/SFN54497/Create.cshtml");
        }

        public ActionResult MyApprovals(FormCollection collection)
        {
            ViewBag.FormInfo = dal.GetFormInfo("SFN54497");
            dal = new ModelDALSFN54497();
            // ViewBag.CharitableInfo = dal.GetSFN61579Info("101");

            DataTable dtInfo = dal.GetSFN54497Info();
            SFN54497DisplayViewModel vmSFN54497 = new SFN54497DisplayViewModel();
            vmSFN54497.StaffRequestModel = Models.SFN54497.SFN54497Model.ConvertDataTableToStaffRequest(dtInfo);

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

            if (title == "Human Resource Officer" || title == "HR Director" || title.Contains("Director") || title.Contains("Chief"))
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
            dal = new ModelDALSFN54497();

            return View("~/Views/Forms/SFN54497/MyApprovals.cshtml", vmSFN54497);
        }

        // Determine whether user is superuser
        private void isSuperUser(UserPrincipal user)
        {
            dal = new ModelDALSFN54497();
            DataTable dtInfo = dal.GetSuperUsers("SFN 54497");
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
        public ActionResult SFN54497(FormCollection collection)
        {
            string environment = ConfigurationManager.AppSettings["Environment"];
            string waitingApproval = "";
            // Get form ID number
            dal.InsertSFN54497GetID(collection, waitingApproval);
            dal = new ModelDALSFN54497();
            DataTable idNum = dal.GetSFN54497InfoByInfo();
            string id = idNum.Rows[0][0].ToString();

            // Create domain context
            PrincipalContext ctx = new PrincipalContext(ContextType.Domain);

            UserPrincipal u = new UserPrincipal(ctx);

            //UserPrincipal user = UserPrincipal.Current;
            UserPrincipal user = UserPrincipal.FindByIdentity(ctx, User.Identity.Name);

            DirectoryEntry dirEntry = (DirectoryEntry)user.GetUnderlyingObject();
            // Get submitter's department
            string submitterDept = dirEntry.Properties["Department"].Value.ToString();
            // Get submitter's job title
            string submitterTitle = dirEntry.Properties["Title"].Value.ToString();
            // If submitter is a department director, this is used to get their manager (the division chief)
            string submitterManager = dirEntry.Properties["manager"].Value.ToString();
            UserPrincipal managerName = UserPrincipal.FindByIdentity(ctx, submitterManager);
            DirectoryEntry chief = (DirectoryEntry)managerName.GetUnderlyingObject();
            

            // Create directory searcher
            DirectoryEntry enTry = new DirectoryEntry("LDAP://OU=WSI,DC=nd,DC=gov");
            DirectorySearcher mySearcher = new DirectorySearcher();
            SearchResultCollection results;

            // Create list for to email addresses
            var toEmailList = new List<string>();

            // Get url path up to "SFN"
            string path = HttpContext.Request.Url.AbsoluteUri.ToString();
            List<string> host = new List<string>(path.Split(new string[] { "SFN" }, StringSplitOptions.None));

            // If submitter is the Finance Director...
            if (submitterTitle == "Director of Finance")
            {
                // Add Finance Director approval information to form
                collection["Approval.DirectorOfFinanceApproval"] = user.DisplayName;
                collection["Approval.DirectorOfFinanceApproveDate"] = DateTime.Today.ToString("MM/dd/yyyy");
                collection["Approval.DirectorOfFinanceApproved"] = "true";

                var waitingApprovalNames = new List<string>();

                // Get agency director email and add to the email list
                mySearcher = new DirectorySearcher(enTry);
                mySearcher.Filter = "(&(company=Workforce Safety & Insurance)(title=" + "Director" + "))";
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

                string combinedNames = string.Join("; ", waitingApprovalNames.ToArray());

                dal = new ModelDALSFN54497();
                dal.UpdateSFN54497WaitingApproval(int.Parse(id), combinedNames);

                // Add finance approval info
                dal = new ModelDALSFN54497();
                dal.InsertSFN54497ApprovalFinance(int.Parse(id), collection);
            } 
            
            // If submitter is Human Resource Officer...
            else if (submitterTitle == "Human Resource Officer" || submitterTitle == "HR Director")
            {
                // Add human resources approval info to form
                collection["Approval.HumanResourcesApproval"] = user.DisplayName;
                collection["Approval.HumanResourcesApproveDate"] = DateTime.Today.ToString("MM/dd/yyyy");
                collection["Approval.HR"] = "true";

                var waitingApprovalNames = new List<string>();

                // Add finance director email to email list
                mySearcher = new DirectorySearcher(enTry);
                mySearcher.Filter = "(&(company=Workforce Safety & Insurance)(title=" + "Director of Finance" + "))";
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

                string combinedNames = string.Join("; ", waitingApprovalNames.ToArray());

                dal = new ModelDALSFN54497();
                dal.UpdateSFN54497WaitingApproval(int.Parse(id), combinedNames);

                // Insert human resources approval info to db
                dal = new ModelDALSFN54497();
                dal.InsertSFN54497ApprovalHR(int.Parse(id), collection);
            } 
            
            // If submitter is Division Chief...
            else if (submitterTitle.Contains("Chief"))
            {
                // Add division chief approval info to form
                collection["Approval.DivisionChiefApproval"] = user.DisplayName;
                collection["Approval.DivisionChiefApproveDate"] = collection["StaffRequestModel.SubmitDate"];
                collection["Approval.DivisionChiefApproved"] = "true";

                var waitingApprovalNames = new List<string>();

                // Get human resources email and add to email list
                mySearcher = new DirectorySearcher(enTry);
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

                mySearcher.Filter = "(&(company=Workforce Safety & Insurance)(title=" + "HR Director" + "))";
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

                string combinedNames = string.Join("; ", waitingApprovalNames.ToArray());

                dal = new ModelDALSFN54497();
                dal.UpdateSFN54497WaitingApproval(int.Parse(id), combinedNames);

                // Insert division chief approval info to db
                dal = new ModelDALSFN54497();
                dal.InsertSFN54497ApprovalDivision(int.Parse(id), collection);
            } 
            
            // If submitter is department director...
            else if ((submitterTitle.Contains("Director") && !submitterTitle.Contains("HR") && !submitterTitle.Contains("Finance")) || submitterManager.Contains("Chief"))
            {
                // Add department director approval info to form
                collection["Approval.DepartmentDirectorApproval"] = user.DisplayName;
                collection["Approval.DepartmentDirectorApproveDate"] = DateTime.Today.ToString("MM/dd/yyyy");
                collection["Approval.DepartmentDirectorApproved"] = "true";

                // Get division chief email and add to email list
                if (chief.Properties["Title"][0].ToString().Contains("Chief"))
                {
                    string email = chief.Properties["mail"][0].ToString();

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

                dal = new ModelDALSFN54497();
                dal.UpdateSFN54497WaitingApproval(int.Parse(id), managerName.DisplayName);

                // Insert division chief approval info to db
                dal = new ModelDALSFN54497();
                dal.InsertSFN54497ApprovalDepartment(int.Parse(id), collection);

                //dal = new ModelDALSFN54497();
                //dal.UpdateSFN54497DepartmentDate(int.Parse(id));
            } 
            
            // If submitter is none of the above...
            else
            {
                // Get submitter's department director
                mySearcher = new DirectorySearcher(enTry);
                mySearcher.Filter = "(&(company=Workforce Safety & Insurance)(Department=" + submitterDept + "))";
                results = mySearcher.FindAll();

                var directorNames = new List<string>();
                // Get department director's email and add to email list
                foreach (SearchResult result in results)
                {
                    if (result.Properties["Title"][0].ToString().Contains("Director"))
                    {
                        string email = result.Properties["mail"][0].ToString();

                        if (environment.ToUpper() == "PROD")
                        {
                            if (email.Contains("wsi_") || email.Contains("wsitrain"))
                            {

                            }

                            else
                            {
                                toEmailList.Add(email);
                                directorNames.Add(result.Properties["displayname"][0].ToString());
                            }

                        }
                        else
                        {
                            toEmailList.Add(email);
                            directorNames.Add(result.Properties["displayname"][0].ToString());
                        }
                    }
                }

                string combinedNames = string.Join("; ", directorNames.ToArray());

                dal = new ModelDALSFN54497();
                dal.UpdateSFN54497WaitingApproval(int.Parse(id), combinedNames);

                dal = new ModelDALSFN54497();
                dal.InsertSFN54497ApprovalID(int.Parse(id));
            }

            // Send email notifying of form submission to correct party
            string[] to = { "hdaugaard@nd.gov" };
            string toAddress = string.Join("; ", toEmailList.ToArray());
            string from = "wsinoreply@nd.gov";
            string subject = "A Staff Request has been submitted";

            string body = "A request for Staff Request has been submitted by " + collection["StaffRequestModel.SubmittedBy"] + ". Click here to review the form. "
            + "<a href=\"" + host[0].ToString() + "SFN54497/View/" + id + "\">Staff Request Form Link " + "</a>";

            // Send email
            dal = new ModelDALSFN54497();
            dal.SendEmail(toAddress, from, subject, body); ;

            //Redirect back to form landing page
            return Redirect("~/SFN54497");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Approval(FormCollection collection, string Command)
        {
            string environment = ConfigurationManager.AppSettings["Environment"];
            // Get domain context
            PrincipalContext ctx = new PrincipalContext(ContextType.Domain);

            UserPrincipal u = new UserPrincipal(ctx);
            UserPrincipal user = UserPrincipal.FindByIdentity(ctx, User.Identity.Name);
            // Create directory searcher
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
            DataTable idNum = dal.GetSFN54497ID(collection);
            string id = idNum.Rows[0][0].ToString();

            // Set username variable
            ViewBag.Username = user.DisplayName;

            // Get requestor name from table
            dal = new ModelDALSFN54497();
            DataTable requestorValue = dal.GetSFN54497Requestor(int.Parse(id));
            string requestor = requestorValue.Rows[0][0].ToString();
            UserPrincipal findRequestor = UserPrincipal.FindByIdentity(ctx, requestor);
            DirectoryEntry requestorName = (DirectoryEntry)findRequestor.GetUnderlyingObject();
            // Get submitter's department
            string submitterDepartment = requestorName.Properties["Department"][0].ToString();
            ViewBag.RequestorEmail = findRequestor.EmailAddress;

            dal = new ModelDALSFN54497();

            // Get url from address bar up to form name
            string path = HttpContext.Request.Url.AbsoluteUri.ToString();
            List<string> host = new List<string>(path.Split(new string[] { "SFN" }, StringSplitOptions.None));

            var waitingApprovalNames = new List<string>();

            // Department Director Approval emails
            if (title.Contains("Director") && dept == submitterDepartment)
            {
                // If approved...
                if (Command == "Approve")
                {
                    // Set department director approval
                    collection["Approvals.DepartmentDirectorApproved"] = "true";
                    collection["Approvals.DepartmentDirectorApproval"] = user.DisplayName;
                    collection["Approvals.DepartmentDirectorApproveDate"] = DateTime.Today.ToString("MM/dd/yyyy");
                    // Create email address list
                    var toEmailList = new List<string>();

                    // Update department director approvals in db
                    dal = new ModelDALSFN54497();
                    dal.UpdateSFN54497DepartmentDirectorApproval(collection);

                    dal = new ModelDALSFN54497();
                    dal.UpdateSFN54497Approval(int.Parse(id));

                    // Find division chief using department director's name
                    
                    UserPrincipal directorName = UserPrincipal.FindByIdentity(ctx, collection["Approvals.DepartmentDirectorApproval"]); ;
                    DirectoryEntry director = (DirectoryEntry)directorName.GetUnderlyingObject();

                    if (!director.Properties["Title"][0].ToString().Equals("Director"))
                    {
                        string directorManager = director.Properties["manager"][0].ToString();


                        UserPrincipal managerName = UserPrincipal.FindByIdentity(ctx, directorManager);
                        DirectoryEntry chief = (DirectoryEntry)managerName.GetUnderlyingObject();

                        

                        // Add division chief's name to email list
                        if (chief.Properties["Title"][0].ToString().Contains("Chief"))
                        {
                            string email = chief.Properties["mail"][0].ToString();
                            if (environment.ToUpper() == "PROD")
                            {
                                if (email.Contains("wsi_") || email.Contains("wsitrain"))
                                {

                                }

                                else
                                {
                                    toEmailList.Add(email);
                                    waitingApprovalNames.Add(chief.Properties["displayname"][0].ToString());
                                }

                            }
                            else
                            {
                                toEmailList.Add(email);
                                waitingApprovalNames.Add(chief.Properties["displayname"][0].ToString());
                            }
                        } else if (chief.Properties["Title"][0].ToString().Equals("Director"))
                        {
                            string email = chief.Properties["mail"][0].ToString();
                            if (email.Contains("wsi_") || email.Contains("wsitrain"))
                            {

                            }

                            else
                            {
                                toEmailList.Add(email);
                                waitingApprovalNames.Add(chief.Properties["displayname"][0].ToString());
                            }
                        }
                    } 
                    
                    if (director.Properties["Title"][0].ToString().Equals("Director"))
                    {
                        // Create searcher for human resource officer
                        mySearcher = new DirectorySearcher(enTry);
                        mySearcher.Filter = "(&(company=Workforce Safety & Insurance)(|(title=" + "Human Resource Officer" + ")(title=" + "HR Director" + ")))";
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
                    }

                    string combinedNames = string.Join("; ", waitingApprovalNames.ToArray());

                    dal = new ModelDALSFN54497();
                    dal.UpdateSFN54497WaitingApproval(int.Parse(id), combinedNames);

                    // Email to division chief to notify of department director approval
                    string toAddress = string.Join("; ", toEmailList.ToArray());
                    string from = "wsinoreply@nd.gov";
                    string subject = "A Staff Request has been submitted";

                    string body = "A Staff Request has been submitted by " + collection["StaffRequestModels.SubmittedBy"] + ". Click here to view the form: "
                    + "<a href=\"" + host[0].ToString() + "SFN54497/View/" + id + "\">Staff Request Form Link " + "</a>";

                    // Send email
                    dal = new ModelDALSFN54497();
                    dal.SendEmail(toAddress, from, subject, body);
                }

                // If denied...
                else if (Command == "Deny")
                {
                    // Update department director approval in db
                    collection["Approvals.DepartmentDirectorApproved"] = "false";
                    collection["Approvals.DepartmentDirectorApproval"] = user.DisplayName;
                    collection["Approvals.DepartmentDirectorApproveDate"] = DateTime.Today.ToString("MM/dd/yyyy");
                    dal = new ModelDALSFN54497();
                    dal.UpdateSFN54497DepartmentDirectorApproval(collection);

                    dal = new ModelDALSFN54497();
                    dal.UpdateSFN54497WaitingApproval(int.Parse(id), "");

                    dal = new ModelDALSFN54497();
                    dal.UpdateSFN54497Denial(int.Parse(id));

                    // Create email address list
                    var toEmailList = new List<string>();
                    toEmailList.Add(ViewBag.RequestorEmail);

                    // Create searcher for human resource officer
                    mySearcher = new DirectorySearcher(enTry);
                    mySearcher.Filter = "(&(company=Workforce Safety & Insurance)(|(title=" + "Human Resource Officer" + ")(title=" + "HR Director" + ")))";
                    results = mySearcher.FindAll();

                    // Add human resource officer's email to email list
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

                    toEmailList.Add(ViewBag.RequestorEmail);

                    // Send email to requestor
                    string[] to = { ViewBag.RequestorEmail };
                    string toAddress = string.Join("; ", toEmailList.ToArray());
                    string from = "wsinoreply@nd.gov";
                    string subject = "A Staff Request was submitted";

                    string body = "A Staff Request was submitted by " + collection["StaffRequestModels.SubmittedBy"] + ". The submitter's Department Director has denied the request. Click here to review the form: "
                    + "<a href=\"" + host[0].ToString() + "SFN54497/View/" + id + "\">Staff Request Form Link " + "</a>";

                    // Send email
                    dal = new ModelDALSFN54497();
                    dal.SendEmail(toAddress, from, subject, body);
                }
            }

            // Division Chief Approval Emails
            if (title.Contains("Chief"))
            {
                // If approved...
                if (Command == "Approve")
                {
                    // Update approval in db
                    collection["Approvals.DivisionChiefApproved"] = "true";
                    collection["Approvals.DivisionChiefApproval"] = user.DisplayName;
                    collection["Approvals.DivisionChiefApproveDate"] = DateTime.Today.ToString("MM/dd/yyyy");
                    dal = new ModelDALSFN54497();
                    dal.UpdateSFN54497DivisionChiefApproval(collection);

                    dal = new ModelDALSFN54497();
                    dal.UpdateSFN54497Approval(int.Parse(id));

                    // Create email list
                    var toEmailList = new List<string>();

                    // Create searcher for human resource officer
                    mySearcher = new DirectorySearcher(enTry);
                    mySearcher.Filter = "(&(company=Workforce Safety & Insurance)(|(title=" + "Human Resource Officer" + ")(title=" + "HR Director" + ")))";
                    results = mySearcher.FindAll();

                    // Add human resource officer's email to email list
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

                    string combinedNames = string.Join("; ", waitingApprovalNames.ToArray());

                    dal = new ModelDALSFN54497();
                    dal.UpdateSFN54497WaitingApproval(int.Parse(id), combinedNames);

                    // Send email to human resource officer
                    string toAddress = string.Join("; ", toEmailList.ToArray());
                    string from = "wsinoreply@nd.gov";
                    string subject = "A Staff Request has been submitted";

                    string body = "A Staff Request has been submitted "
                     + "<a href=\"" + host[0].ToString() + "SFN54497/View/" + id + "\">Staff Request Form Link " + "</a>";

                    // Send email
                    dal = new ModelDALSFN54497();
                    dal.SendEmail(toAddress, from, subject, body);
                }
                // If denied...
                else if (Command == "Deny")
                {
                    // Update division chief approval in db
                    collection["Approvals.DivisionChiefApproved"] = "false";
                    collection["Approvals.DivisionChiefApproval"] = user.DisplayName;
                    collection["Approvals.DivisionChiefApproveDate"] = DateTime.Today.ToString("MM/dd/yyyy");
                    dal = new ModelDALSFN54497();
                    dal.UpdateSFN54497DivisionChiefApproval(collection);

                    dal = new ModelDALSFN54497();
                    dal.UpdateSFN54497WaitingApproval(int.Parse(id), "");

                    dal = new ModelDALSFN54497();
                    dal.UpdateSFN54497Denial(int.Parse(id));

                    // Create email address list
                    var toEmailList = new List<string>();
                    toEmailList.Add(ViewBag.RequestorEmail);

                    // Create searcher for human resource officer
                    mySearcher = new DirectorySearcher(enTry);
                    mySearcher.Filter = "(&(company=Workforce Safety & Insurance)(|(title=" + "Human Resource Officer" + ")(title=" + "HR Director" + ")))";
                    results = mySearcher.FindAll();

                    // Add human resource officer's email to email list
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

                    // Find division chief using department director's name
                    UserPrincipal directorName = UserPrincipal.FindByIdentity(ctx, collection["Approvals.DepartmentDirectorApproval"]); ;
                    toEmailList.Add(directorName.EmailAddress);

                    toEmailList.Add(ViewBag.RequestorEmail);

                    // Send email to requestor
                    string[] to = { ViewBag.RequestorEmail };
                    string toAddress = string.Join("; ", toEmailList.ToArray());
                    string from = "wsinoreply@nd.gov";
                    string subject = "A Staff Request was submitted";

                    // Email to requesting employee notifying them of business card proof
                    string body = "A Staff Request was submitted by " + collection["StaffRequestModels.SubmittedBy"] + ". The Division Chief has denied the request. Click here to review the form: "
                    + "<a href=\"" + host[0].ToString() + "SFN54497/View/" + id + "\">Staff Request Form Link " + "</a>";

                    // Send email
                    dal = new ModelDALSFN54497();
                    dal.SendEmail(toAddress, from, subject, body);
                }
            }

            // Set reviewed indicator to 'y'
            // When human resources sets approval...
            if (dept.Contains("Human Resources"))
            {
                // If approved...
                if (Command == "Approve")
                {
                    // Update human resources approvals in db
                    collection["Approvals.HumanResourcesApproved"] = "true";
                    collection["Approvals.HumanResourcesApproval"] = user.DisplayName;
                    collection["Approvals.HumanResourcesApproveDate"] = DateTime.Today.ToString("MM/dd/yyyy");
                    dal = new ModelDALSFN54497();
                    dal.UpdateSFN54497HumanResourcesApproval(collection);

                    dal = new ModelDALSFN54497();
                    dal.UpdateSFN54497Approval(int.Parse(id));

                    // Create email list
                    var toEmailList = new List<string>();

                    // Create searcher for finance director
                    mySearcher = new DirectorySearcher(enTry);
                    mySearcher.Filter = "(&(company=Workforce Safety & Insurance)(title=" + "Director of Finance" + "))";
                    results = mySearcher.FindAll();

                    // Add finance director's email to list
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

                    string combinedNames = string.Join("; ", waitingApprovalNames.ToArray());

                    dal = new ModelDALSFN54497();
                    dal.UpdateSFN54497WaitingApproval(int.Parse(id), combinedNames);

                    // Email to finance director
                    string toAddress = string.Join("; ", toEmailList.ToArray());
                    string from = "wsinoreply@nd.gov";
                    string subject = "A Staff Request has been submitted";

                   string body = "A Staff Request has been submitted "
                    + "<a href=\"" + host[0].ToString() + "SFN54497/View/" + id + "\">Staff Request Form Link " + "</a>";

                    // Send email
                    dal = new ModelDALSFN54497();
                    dal.SendEmail(toAddress, from, subject, body);
                }

                // If denied...
                else if (Command == "Deny")
                {
                    // Update human resources approval in db
                    collection["Approvals.HumanResourcesApproved"] = "false";
                    collection["Approvals.HumanResourcesApproval"] = user.DisplayName;
                    collection["Approvals.HumanResourcesApproveDate"] = DateTime.Today.ToString("MM/dd/yyyy");
                    dal = new ModelDALSFN54497();
                    dal.UpdateSFN54497HumanResourcesApproval(collection);

                    dal = new ModelDALSFN54497();
                    dal.UpdateSFN54497WaitingApproval(int.Parse(id), "");

                    dal = new ModelDALSFN54497();
                    dal.UpdateSFN54497Denial(int.Parse(id));

                    // Create email address list
                    var toEmailList = new List<string>();
                    toEmailList.Add(ViewBag.RequestorEmail);

                    // Create searcher for human resource officer
                    mySearcher = new DirectorySearcher(enTry);
                    mySearcher.Filter = "(&(company=Workforce Safety & Insurance)(|(title=" + "Human Resource Officer" + ")(title=" + "HR Director" + ")))";
                    results = mySearcher.FindAll();

                    // Add human resource officer's email to email list
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

                    // Find division chief using department director's name
                    if (collection["Approvals.DepartmentDirectorApproval"] != "")
                    {
                        UserPrincipal directorName = UserPrincipal.FindByIdentity(ctx, collection["Approvals.DepartmentDirectorApproval"]); ;
                        toEmailList.Add(directorName.EmailAddress);
                    }

                    if (collection["Approvals.DivisionChiefApproval"] != "")
                    {
                        UserPrincipal chiefName = UserPrincipal.FindByIdentity(ctx, collection["Approvals.DivisionChiefApproval"]);
                        toEmailList.Add(chiefName.EmailAddress);
                    }
                    
                    toEmailList.Add(ViewBag.RequestorEmail);

                    // Email to requesting employee
                    string[] to = { ViewBag.RequestorEmail };
                    string toAddress = string.Join("; ", toEmailList.ToArray());
                    string from = "wsinoreply@nd.gov";
                   string subject = "A Staff Request was submitted";

                    // Email to requesting employee notifying them of business card proof
                    string body = "A Staff Request was submitted by " + collection["StaffRequestModels.SubmittedBy"] + ". The Human Resources has denied the request. Click here to review the form: "
                    + "<a href=\"" + host[0].ToString() + "SFN54497/View/" + id + "\">Staff Request Form Link " + "</a>";

                    // Send email
                    dal = new ModelDALSFN54497();
                    dal.SendEmail(toAddress, from, subject, body);
                }
            }

            //// Set reviewed indicator to 'y'
            //// When finance director sets approval...
            if (ViewBag.Title == "Director of Finance")
            {
                // If approved
                if (Command == "Approve")
                {
                    // Update finance director approvals in db
                    collection["Approvals.DirectorOfFinanceApproved"] = "true";
                    collection["Approvals.DirectorOfFinanceApproval"] = user.DisplayName;
                    collection["Approvals.DirectorOfFinanceApproveDate"] = DateTime.Today.ToString("MM/dd/yyyy");
                    dal = new ModelDALSFN54497();
                    dal.UpdateSFN54497FinanceDirectorApproval(collection);

                    dal = new ModelDALSFN54497();
                    dal.UpdateSFN54497Approval(int.Parse(id));

                    // Create email list
                    var toEmailList = new List<string>();

                    // Create searcher for agency director
                    mySearcher = new DirectorySearcher(enTry);
                    mySearcher.Filter = "(&(company=Workforce Safety & Insurance)(title=" + "Director" + "))";
                    results = mySearcher.FindAll();

                    // Add agency director's email to list
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

                    string combinedNames = string.Join("; ", waitingApprovalNames.ToArray());

                    dal = new ModelDALSFN54497();
                    dal.UpdateSFN54497WaitingApproval(int.Parse(id), combinedNames);

                    // Email to agency director
                    string toAddress = string.Join("; ", toEmailList.ToArray());
                    string from = "wsinoreply@nd.gov";
                    string subject = "A Staff Request Form has been submitted";

                    string body = "A Staff Request has been submitted by " + collection["StaffRequestModels.SubmittedBy"] + ". Click here to view the form: "
                    + "<a href=\"" + host[0].ToString() + "SFN54497/View/" + id + "\">Staff Request Form Link " + "</a>";

                    // Send email
                    dal = new ModelDALSFN54497();
                    dal.SendEmail(toAddress, from, subject, body);
                }

                // If denied
                else if (Command == "Deny")
                {
                    // Update finance director approvals in db
                    collection["Approvals.DirectorOfFinanceApproved"] = "false";
                    collection["Approvals.DirecotrOfFinanceApproval"] = user.DisplayName;
                    collection["Approvals.DirectorOfFinanceApproveDate"] = DateTime.Today.ToString("MM/dd/yyyy");
                    dal = new ModelDALSFN54497();
                    dal.UpdateSFN54497FinanceDirectorApproval(collection);

                    dal = new ModelDALSFN54497();
                    dal.UpdateSFN54497WaitingApproval(int.Parse(id), "");

                    dal = new ModelDALSFN54497();
                    dal.UpdateSFN54497Denial(int.Parse(id));

                    // Create email address list
                    var toEmailList = new List<string>();
                    toEmailList.Add(ViewBag.RequestorEmail);

                    // Create searcher for human resource officer
                    mySearcher = new DirectorySearcher(enTry);
                    mySearcher.Filter = "(&(company=Workforce Safety & Insurance)(|(title=" + "Human Resource Officer" + ")(title=" + "HR Director" + ")))";
                    results = mySearcher.FindAll();

                    // Add human resource officer's email to email list
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

                    // Find division chief using department director's name
                    if (collection["Approvals.DepartmentDirectorApproval"] != "")
                    {
                        UserPrincipal directorName = UserPrincipal.FindByIdentity(ctx, collection["Approvals.DepartmentDirectorApproval"]); ;
                        toEmailList.Add(directorName.EmailAddress);
                    }

                    if (collection["Approvals.DivisionChiefApproval"] != "")
                    {
                        UserPrincipal chiefName = UserPrincipal.FindByIdentity(ctx, collection["Approvals.DivisionChiefApproval"]);
                        toEmailList.Add(chiefName.EmailAddress);
                    }

                    toEmailList.Add(ViewBag.RequestorEmail);

                    // Email to requestor
                    string[] to = { ViewBag.RequestorEmail };
                    string toAddress = string.Join("; ", toEmailList.ToArray());
                    string from = "wsinoreply@nd.gov";
                    string subject = "Your Staff Request was denied";

                    string body = "A Staff Request was submitted by " + collection["StaffRequestModels.SubmittedBy"] + ". The Director of Finance has denied the request. Click here to review the form: "
                    + "<a href=\"" + host[0].ToString() + "SFN54497/View/" + id + "\">Staff Request Form Link " + "</a>";

                    // Send email
                    dal = new ModelDALSFN54497();
                    dal.SendEmail(toAddress, from, subject, body);
                }
            }

            // Agency director approvals...
            else if (ViewBag.Title == "Director")
           {
                // If approved...
                if (Command == "Approve")
                {
                    // Update agency director approval in db
                    collection["Approvals.AgencyDirectorApproved"] = "true";
                    collection["Approvals.AgencyDirectorApproval"] = user.DisplayName;
                    collection["Approvals.AgencyDirectorApproveDate"] = DateTime.Today.ToString("MM/dd/yyyy");
                    dal.UpdateSFN54497AgencyDirectorApproval(collection);

                    dal = new ModelDALSFN54497();
                    dal.UpdateSFN54497WaitingApproval(int.Parse(id), "");

                    dal = new ModelDALSFN54497();
                    dal.UpdateSFN54497Approval(int.Parse(id));

                    // Email to requestor
                    string[] to = { ViewBag.RequestorEmail };
                    string toAddress = string.Join("; ", to.ToArray());
                    string from = "wsinoreply@nd.gov";
                    string subject = "A Staff Request Form has been approved";

                    string body = "The Agency Director has approved a Staff Request from " + collection["StaffRequestModels.SubmittedBy"] + ". Click here to view the request: " + "<a href=\"" + host[0].ToString() + "SFN54497/View/" + id + "\">Staff Request Form Link " + "</a>";

                    // Send email
                    dal = new ModelDALSFN54497();
                    dal.SendEmail(toAddress, from, subject, body);
                }

                // If denied...
                else if (Command == "Deny")
                {
                    // Update agancy director approvals in db
                    collection["Approvals.AgencyDirectorApproved"] = "false";
                    collection["Approvals.AgencyDirectorApproval"] = user.DisplayName;
                    collection["Approvals.AgencyDirectorApproveDate"] = DateTime.Today.ToString("MM/dd/yyyy");
                    dal = new ModelDALSFN54497();
                    dal.UpdateSFN54497AgencyDirectorApproval(collection);

                    dal = new ModelDALSFN54497();
                    dal.UpdateSFN54497WaitingApproval(int.Parse(id), "");

                    dal = new ModelDALSFN54497();
                    dal.UpdateSFN54497Denial(int.Parse(id));

                    // Create email address list
                    var toEmailList = new List<string>();
                    toEmailList.Add(ViewBag.RequestorEmail);

                    // Create searcher for human resource officer
                    mySearcher = new DirectorySearcher(enTry);
                    mySearcher.Filter = "(&(company=Workforce Safety & Insurance)(|(title=" + "Human Resource Officer" + ")(title=" + "HR Director" + ")))";
                    results = mySearcher.FindAll();

                    // Add human resource officer's email to email list
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

                    // Find division chief using department director's name
                    if (collection["Approvals.DepartmentDirectorApproval"] != "")
                    {
                        UserPrincipal directorName = UserPrincipal.FindByIdentity(ctx, collection["Approvals.DepartmentDirectorApproval"]); ;
                        toEmailList.Add(directorName.EmailAddress);
                    }

                    if (collection["Approvals.DivisionChiefApproval"] != "")
                    {
                        UserPrincipal chiefName = UserPrincipal.FindByIdentity(ctx, collection["Approvals.DivisionChiefApproval"]);
                        toEmailList.Add(chiefName.EmailAddress);
                    }

                    if (collection["Approvals.DirectorOfFinanceApproval"] != "")
                    {
                        UserPrincipal financeName = UserPrincipal.FindByIdentity(ctx, collection["Approvals.DirectorOfFinanceApproval"]);
                        toEmailList.Add(financeName.EmailAddress);
                    }

                    toEmailList.Add(ViewBag.RequestorEmail);

                    // Email to requestor
                    string[] to = { ViewBag.RequestorEmail };
                    string toAddress = string.Join("; ", toEmailList.ToArray());
                    string from = "wsinoreply@nd.gov";
                    string subject = "Your Staff Request was denied";

                    string body = "A Staff Request was submitted by " + collection["StaffRequestModels.SubmittedBy"] + ". The Agency Director has denied the request. Click here to review the form: "
                    + "<a href=\"" + host[0].ToString() + "SFN54497/View/" + id + "\">Staff Request Form Link " + "</a>";

                    // Send email
                    dal = new ModelDALSFN54497();
                    dal.SendEmail(toAddress, from, subject, body);
                }
            }
            // Redirect back to form landing page
            return Redirect("~/SFN54497");
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
    }
}