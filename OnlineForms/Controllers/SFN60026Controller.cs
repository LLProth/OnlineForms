using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Management;
using System.Web.Mvc;
using OnlineForms.ViewModels;
using OnlineForms.Models.SFN18795;
using OnlineForms.Models;
using System.Data;
using Microsoft.SharePoint.Client;
using Microsoft.VisualBasic;
using SP = Microsoft.SharePoint.Client;
using FormCollection = System.Web.Mvc.FormCollection;
using System.Security;
using System.Security.Principal;
using System.Net.Http;
using OnlineForms.Models.SFN60026;
using System.Configuration;
using OnlineForms.Helper;
using System.Web;
using System.IO;
using Newtonsoft.Json;
using OnlineForms.Logging;

namespace OnlineForms.Controllers
{
    [Authorize]
    public class SFN60026Controller : Controller
    {
        private ModelDALSFN60026 dal = new ModelDALSFN60026();
        private bool _superUser = false;
        private bool _inHR = false;
        private string _filter = "";
        public static LoggingService log = new LoggingService();

        #region Content_Functions
        //[Authorize]
        public ActionResult Index()
        {
            log.LogMessage("SFN60026 Index function started");//: SessionID = " + HttpContext.Session.SessionID);
            log.LogMessage("SFN60026 Caling getConfigValues()");
            
            getConfigValues();
            log.LogMessage("SFN60026 Calling GetFormInfo()");
            ViewBag.FormInfo = dal.GetFormInfo("SFN60026");
            dal = new ModelDALSFN60026();
            log.LogMessage(string.Format("SFN60026 DAL Connection String: {0}", dal.ConnectionString));
            // ViewBag.CharitableInfo = dal.GetSFN61579Info("101");
            // Create domain context
            PrincipalContext ctx = new PrincipalContext(ContextType.Domain);

            UserPrincipal u = new UserPrincipal(ctx);

            UserPrincipal user = UserPrincipal.FindByIdentity(ctx, User.Identity.Name);
            log.LogMessage(string.Format("SFN60026 User Name: {0}", User.Identity.Name));

            log.LogMessage("SFN60026 User: " + User.Identity.Name);
            DirectoryEntry dirEntry = (DirectoryEntry)user.GetUnderlyingObject();
            string title = dirEntry.Properties["Title"].Value.ToString();


            SFN60026DisplayViewModel vmSFN60026 = new SFN60026DisplayViewModel();
            isSuperUser(user);
            log.LogMessage("SFN60026 Is SuperUser:" + _superUser.ToString());
            if (_superUser)
            {
                log.LogMessage("SFN60026 Calling GetSFN60026AllInfo()");
                DataTable dtInfo = dal.GetSFN60026AllInfo();
                log.LogMessage("SFN60026 Calling GetListSFN60026() - data table");
                vmSFN60026.BonusRecommendationModels = SFN60026Model.GetListSFN60026(dtInfo);
            }
            else
            {
                log.LogMessage("SFN60026 Calling GetSFN60026Info() - " + user.DisplayName);
                DataTable dtInfo = dal.GetSFN60026Info(user.DisplayName);
                log.LogMessage("SFN60026 Calling GetListSFN60026() - data table and " + user.DisplayName);
                vmSFN60026.BonusRecommendationModels = SFN60026Model.GetListSFN60026(dtInfo,user.DisplayName);
            }
            log.LogMessage(string.Format("SFN60026 Filling View Bag - Super User:{0}, InHR:{1}, Display Name:{2}, Company:{3}", _superUser,_inHR, user.DisplayName, dirEntry.Properties["Company"].Value.ToString()));
            ViewBag.Access = _superUser;
            ViewBag.InHR = _inHR;
            ViewBag.Username = user.DisplayName;
            string company = dirEntry.Properties["Company"].Value.ToString();


            dal = new ModelDALSFN60026();


            if (company.Equals("Workforce Safety & Insurance"))
            {
                log.LogMessage("SFN60026 SFN60026 Index function completed");
                return View("~/Views/Forms/SFN60026/Index.cshtml", vmSFN60026);
            } else
            {
                return new HttpUnauthorizedResult();
            }            
        }
        public ActionResult Create(FormCollection collection = null)
        {
            getConfigValues();
            ViewBag.FormInfo = dal.GetFormInfo("SFN60026");

            // Create domain context
            PrincipalContext ctx = new PrincipalContext(ContextType.Domain);

            UserPrincipal u = new UserPrincipal(ctx);

            //UserPrincipal user = UserPrincipal.Current;
            UserPrincipal user = UserPrincipal.FindByIdentity(ctx, User.Identity.Name);

            // Get user's position
            DirectoryEntry dirEntry = (DirectoryEntry)user.GetUnderlyingObject();
            string position = dirEntry.Properties["Title"].Value.ToString();

            string company = dirEntry.Properties["Company"].Value.ToString();
            if (company == "Workforce Safety & Insurance")
            {
                ViewBag.Company = true;
            }


            ViewBag.Username = user.DisplayName;
            ViewBag.SubPos = position;
            ViewBag.SubDate = DateTime.Today.ToString("MM/dd/yyyy");

            //create list for drop down list
            List<ADInfo> list = DirectoryHelper.GetAllADForCompany();
            List<string> names = new List<string>();
            names.Add("[Enter Nominee Name]");
            foreach (ADInfo ad in list)
            {
                names.Add(ad.DisplayName);
            }
            names.Sort();
            ViewBag.WSIPersonnel = names;

            //create json string with all details to fill in position and department
            string json = JsonConvert.SerializeObject(list);
            ViewBag.WSIList = json;

            SFN60026DisplayViewModel vmSFN60026 = new SFN60026DisplayViewModel();
            vmSFN60026.BonusRecommendationModel = new SFN60026Model();
            fillInCollectionValues(vmSFN60026.BonusRecommendationModel, collection);
            return View("~/Views/Forms/SFN60026/Create.cshtml", vmSFN60026);
        }
        public ActionResult Edit(int id, FormCollection collection = null)
        {
            getConfigValues();
            dal = new ModelDALSFN60026();
            ViewBag.FormInfo = dal.GetFormInfo("SFN60026");

            SFN60026DisplayViewModel vmSFN60026 = new SFN60026DisplayViewModel();
            dal = new ModelDALSFN60026();
            DataTable dtInfo = dal.GetSFN60026InfoByID(id);
            vmSFN60026.BonusRecommendationModel = Models.SFN60026.SFN60026Model.ConvertDataTableToSFN60026(dtInfo);
            //ViewBag.Values = dtInfo;
            if(collection.Count >= 0)
                fillInCollectionValues(vmSFN60026.BonusRecommendationModel, collection);

            ViewBag.ID = id;

            //create list for drop down list
            List<ADInfo> list = DirectoryHelper.GetAllADForCompany();
            List<string> names = new List<string>();
            names.Add("[Enter Nominee Name]");
            foreach (ADInfo ad in list)
            {
                names.Add(ad.DisplayName);
            }
            names.Sort();
            ViewBag.WSIPersonnel = names;

            //create json string with all details to fill in position and department
            string json = JsonConvert.SerializeObject(list);
            ViewBag.WSIList = json;

            return View("~/Views/Forms/SFN60026/Edit.cshtml", vmSFN60026);
        }
        public ActionResult View(int id, FormCollection collection)
        {
            getConfigValues();
            ViewBag.FormInfo = dal.GetFormInfo("SFN60026");

            SFN60026DisplayViewModel vmSFN60026 = new SFN60026DisplayViewModel();
            dal = new ModelDALSFN60026();
            DataTable dtInfo = dal.GetSFN60026InfoByID(id);
            vmSFN60026.BonusRecommendationModel = Models.SFN60026.SFN60026Model.ConvertDataTableToSFN60026(dtInfo);
            //ViewBag.Values = dtInfo;
            fillInCollectionValues(vmSFN60026.BonusRecommendationModel, collection);

            ViewBag.ID = id;

            // Create domain context
            PrincipalContext ctx = new PrincipalContext(ContextType.Domain);

            UserPrincipal u = new UserPrincipal(ctx);

            UserPrincipal user = UserPrincipal.FindByIdentity(ctx, User.Identity.Name);


            // Validate user can view form
            bool access = validateUser(user, vmSFN60026.BonusRecommendationModel);
            // Get User's company
            DirectoryEntry dirEntry = (DirectoryEntry)user.GetUnderlyingObject();
            string company = dirEntry.Properties["Company"].Value.ToString();
            ViewBag.Username = user.DisplayName;
            ViewBag.SuperUser = _superUser;
            ViewBag.InHR = _inHR;
            if (_superUser || (company.Equals("Workforce Safety & Insurance") && access))
            {
                ViewBag.Company = true;
            }
            else
            {
                //return new HttpUnauthorizedResult();
                return View("~/Views/Forms/SFN60026/Unauthorized.cshtml", vmSFN60026);
            }
            if (vmSFN60026.BonusRecommendationModel.CurrentStatus == 9)
            {
                findDeniedStep(vmSFN60026.BonusRecommendationModel);
                return View("~/Views/Forms/SFN60026/Denied.cshtml", vmSFN60026);
            }
            else
            {
                return View("~/Views/Forms/SFN60026/View.cshtml", vmSFN60026);
            }
        }
        //[Authorize]
        public ActionResult Print(int id, FormCollection collection = null)
        {
            ViewBag.FormInfo = dal.GetFormInfo("SFN60026");

            dal = new ModelDALSFN60026();
            DataTable dtInfo = dal.GetSFN60026InfoByID(id);
            SFN60026DisplayViewModel vmSFN60026 = new SFN60026DisplayViewModel();
            vmSFN60026.BonusRecommendationModel = Models.SFN60026.SFN60026Model.ConvertDataTableToSFN60026(dtInfo);
            dal = new ModelDALSFN60026();

            // Get form values
            if (collection.Count >= 0)
            {
                fillInCollectionValues(vmSFN60026.BonusRecommendationModel, collection);
            }
            ViewBag.ID = id;
            PrincipalContext ctx = new PrincipalContext(ContextType.Domain);

            UserPrincipal u = new UserPrincipal(ctx);

            UserPrincipal user = UserPrincipal.FindByIdentity(ctx, User.Identity.Name);

            ViewBag.Username = user.DisplayName;
            return View("~/Views/Forms/SFN60026/Print.cshtml", vmSFN60026);
        }

        #endregion

        #region Action_Functions
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SFN60026(FormCollection collection, string command)
        {
            getConfigValues();
            try
            {
                if (command.Equals("Submit"))
                {
                    if (!validateModel(collection))
                    {
                        return Create(collection);
                    }
                    PrincipalContext ctx = new PrincipalContext(ContextType.Domain);

                    UserPrincipal user = UserPrincipal.FindByIdentity(ctx, User.Identity.Name);
                    collection.Set("BonusRecommendationModel.SubmitterName", user.DisplayName);
                    collection.Set("BonusRecommendationModel.ModifiedBy", user.DisplayName);
                    collection.Set("BonusRecommendationModel.FormSubmitted", "Y");
                    collection.Set("BonusRecommendationModel.SupervisorEndorsement", "N");
                    int stage = 1;
                    collection.Set("BonusRecommendationModel.CurrentStatus", stage.ToString());
                    fillApprovalHierarchy(collection, stage);
                    collection.Set("BonusRecommendationModel.SubmitterDate", DateTime.Today.ToString("yyyy-MM-dd"));
                    int sfn60026ID = dal.InsertSFN60026GetID(collection);
                    //In case we are skipping some steps.
                    dal.UpdateSFN60026(collection, sfn60026ID);

                    dal = new ModelDALSFN60026();
                    DataTable idNum = dal.GetSFN60026InfoByMaxID();

                    string id = idNum.Rows[0][0].ToString();
                    log.LogMessage(string.Format("SFN60026 Submit Stage: {0}", stage));
                    log.LogMessage(string.Format("SFN60026 Submit SupervisorSignature: {0}", collection["BonusRecommendationModel.SupervisorSignature"]));
                    log.LogMessage(string.Format("SFN60026 Submit NomineeName: {0}", collection["BonusRecommendationModel.NomineeName"]));
                    sendNextApprovalEmail(collection, int.Parse(id));

                    return Redirect("~/SFN60026");
                }
                else if (command.Equals("Save"))
                {
                    PrincipalContext ctx = new PrincipalContext(ContextType.Domain);

                    UserPrincipal user = UserPrincipal.FindByIdentity(ctx, User.Identity.Name);
                    collection.Set("BonusRecommendationModel.ModifiedBy", user.DisplayName);

                    collection.Set("BonusRecommendationModel.FormSubmitted", "N");
                    collection.Set("BonusRecommendationModel.CurrentStatus", "0");
                    collection.Set("BonusRecommendationModel.SupervisorEndorsement", "N");
                    int sfn60026ID = dal.InsertSFN60026GetID(collection);

                    return Edit(sfn60026ID,collection);
                }
                else if (command.Equals("Close") || command.Equals("Cancel"))
                {
                    return Redirect("~/SFN60026");
                }
                else
                    return View("~/Views/Shared/Error.cshtml");
            }
            catch(Exception ex)
            {
                ViewBag.Header = ex.Message;
                ViewBag.Message = "";
                return View("~/Views/Forms/SFN60026/Error.cshtml");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SFN60026Edit(FormCollection collection, string command)
        {
            getConfigValues();
            if (command.Equals("Submit"))
            {
                int sfn60026ID = int.Parse(collection["BonusRecommendationModel.ID"]);
                if (!validateModel(collection))
                {
                    return Edit(sfn60026ID, collection);
                }
                PrincipalContext ctx = new PrincipalContext(ContextType.Domain);

//                UserPrincipal u = new UserPrincipal(ctx);

                UserPrincipal user = UserPrincipal.FindByIdentity(ctx, User.Identity.Name);
                collection.Set("BonusRecommendationModel.SubmitterName", user.DisplayName);
                collection.Set("BonusRecommendationModel.ModifiedBy", user.DisplayName);
                collection.Set("BonusRecommendationModel.FormSubmitted", "Y");
                collection.Set("BonusRecommendationModel.SupervisorEndorsement", "N");
                int stage = 1;
                collection.Set("BonusRecommendationModel.CurrentStatus", stage.ToString());
                fillApprovalHierarchy(collection, stage);
                sendNextApprovalEmail(collection, sfn60026ID);
                dal = new ModelDALSFN60026();
                collection.Set("BonusRecommendationModel.SubmitterDate",  DateTime.Today.ToString("yyyy-MM-dd"));
                dal.UpdateSFN60026Init(collection, sfn60026ID);
                //In case we are skipping some steps.
                dal.UpdateSFN60026(collection, sfn60026ID);

                return Redirect("~/SFN60026");
            }
            else if (command.Equals("Save"))
            {
                PrincipalContext ctx = new PrincipalContext(ContextType.Domain);

                UserPrincipal user = UserPrincipal.FindByIdentity(ctx, User.Identity.Name);
                collection.Set("BonusRecommendationModel.ModifiedBy", user.DisplayName);

                int sfn60026ID = int.Parse(collection["BonusRecommendationModel.ID"]);
                collection.Set("BonusRecommendationModel.FormSubmitted", "N");
                collection.Set("BonusRecommendationModel.CurrentStatus", "0");
                collection.Set("BonusRecommendationModel.SupervisorEndorsement", "N");
                dal = new ModelDALSFN60026();
                dal.UpdateSFN60026Init(collection, sfn60026ID);

                return Edit(sfn60026ID, collection);
            }
            else if (command.Equals("Close") || command.Equals("Cancel"))
            {
                return Redirect("~/SFN60026");
            }
            else if (command.Equals("PDF"))
            {
                System.IO.FileStream b = new FileStream("c:\\temp\\elf.pdf", FileMode.Open);

                MemoryStream target = new MemoryStream();
                b.CopyTo(target);
                byte[] data = target.ToArray();

                return new FileContentResult(data, "application/pdf");
            }
            else
                return View("~/Views/Shared/Error.cshtml");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Approve(FormCollection collection, string command)
        {
            getConfigValues();
            if (command.Equals("Approve") || (command.Equals("Save") && collection["BonusRecommendationModel.CurrentStatus"] == "5" && collection["BonusRecommendationModel.CommitteeApproval"] == "Yes"))
            {
                if (!validateModel(collection))
                {
                    int id = int.Parse(collection["BonusRecommendationModel.ID"]);
                    return View(id,collection);
                }
                else
                {
                    PrincipalContext ctx = new PrincipalContext(ContextType.Domain);

                    UserPrincipal user = UserPrincipal.FindByIdentity(ctx, User.Identity.Name);
                    collection.Set("BonusRecommendationModel.ModifiedBy", user.DisplayName);
                    int sfn60026ID = int.Parse(collection["BonusRecommendationModel.ID"]);
                    int stage = checkApproval(collection);
                    collection.Set("BonusRecommendationModel.CurrentStatus", stage.ToString());
                    fillApprovalHierarchy(collection, stage);
                    dal = new ModelDALSFN60026();
                    sendNextApprovalEmail(collection, sfn60026ID);

                    dal.UpdateSFN60026(collection, sfn60026ID);
                    //dal.ApproveSFN60026(collection, sfn60026ID, stage);
                    return Redirect("~/SFN60026");
                }
            }
            else if (command.Equals("Deny") || (command.Equals("Save") && collection["BonusRecommendationModel.CurrentStatus"] == "5" && collection["BonusRecommendationModel.CommitteeApproval"] == "No" ))
            {
                //If Current Status == 5 and got here, the user clicked "Save" with selecting "No" for the Committee Approval.
                // As a double check, we will only check to make sure the Approved Amount field is blank.
                // This is the only validation needed here.
                if (command.Equals("Save") && collection["BonusRecommendationModel.CurrentStatus"] == "5" && collection["BonusRecommendationModel.CommitteeApproval"] == "No")
                {
                    if (!validateModel(collection))
                    {
                        int id = int.Parse(collection["BonusRecommendationModel.ID"]);
                        return View(id, collection);
                    }
                }
                PrincipalContext ctx = new PrincipalContext(ContextType.Domain);

                UserPrincipal user = UserPrincipal.FindByIdentity(ctx, User.Identity.Name);
                collection.Set("BonusRecommendationModel.ModifiedBy", user.DisplayName);
                int sfn60026ID = int.Parse(collection["BonusRecommendationModel.ID"]);
                collection.Set("BonusRecommendationModel.CurrentStatus", "9");
                collection.Set("BonusRecommendationModel.FormDenied", "Y");

                dal.DenySFN60026(collection, sfn60026ID);
                sendDeniedEmail(collection, sfn60026ID);

                return Redirect("~/SFN60026");
            }
            else if (command.Equals("Save"))
            {
                //If Current Status == 5 and got here, the user clicked "Save" but did not choose to approve or not.
                // This is the only validation needed here.
                if (collection["BonusRecommendationModel.CurrentStatus"] == "5")
                {
                    if (!validateModel(collection))
                    {
                        int id = int.Parse(collection["BonusRecommendationModel.ID"]);
                        return View(id, collection);
                    }
                }

                PrincipalContext ctx = new PrincipalContext(ContextType.Domain);

                UserPrincipal user = UserPrincipal.FindByIdentity(ctx, User.Identity.Name);
                collection.Set("BonusRecommendationModel.ModifiedBy", user.DisplayName);
                switch (collection["BonusRecommendationModel.CurrentStatus"])
                {
                    case "4":
                        dal.UpdateHRSFN60026(collection);
                        break;
                    case "5":
                        dal.UpdateCommitteeSFN60026(collection);
                        break;
                    default:
                        break;
                }

                return Redirect("~/SFN60026");
            }
            else if (command.Equals("Close") || command.Equals("Cancel"))
            {
                return Redirect("~/SFN60026");
            }
            else
                return View("~/Views/Shared/Error.cshtml");
        }
        #endregion

        #region Support_Functions
        private int checkApproval(FormCollection collection)
        {
            int retVal = int.Parse(collection["BonusRecommendationModel.CurrentStatus"]);
            dal = new ModelDALSFN60026();
            //Increase stage
            ++retVal;
            switch (retVal)
            {
                case 2:
                    collection.Set("BonusRecommendationModel.SupervisorSignatureDate", DateTime.Today.ToString("yyyy-MM-dd"));
                    collection.Set("BonusRecommendationModel.SupervisorEndorsement", "Y");
                    break;
                case 3:
                    collection.Set("BonusRecommendationModel.DepartmentSignatureDate", DateTime.Today.ToString("yyyy-MM-dd"));
                    collection.Set("BonusRecommendationModel.DepartmentEndorsement", "Y");
                    break;
                case 4:
                    collection.Set("BonusRecommendationModel.ChiefSignatureDate", DateTime.Today.ToString("yyyy-MM-dd"));
                    break;
                case 5:
                    collection.Set("BonusRecommendationModel.HRRepresentativeDate", DateTime.Today.ToString("yyyy-MM-dd"));
                    collection.Set("BonusRecommendationModel.HRRepresentative", collection["BonusRecommendationModel.ModifiedBy"]);
                    dal.UpdateHRSFN60026(collection);
                    break;
                case 6:
                    collection.Set("BonusRecommendationModel.CommitteeApprovalDate", DateTime.Today.ToString("yyyy-MM-dd"));
                    dal.UpdateCommitteeSFN60026(collection);
                    break;
                case 7:
                    collection.Set("BonusRecommendationModel.ChiefEndorsementDate", DateTime.Today.ToString("yyyy-MM-dd"));
                    break;
                case 8:
                    collection.Set("BonusRecommendationModel.AgencyDirectorEndorsementDate", DateTime.Today.ToString("yyyy-MM-dd"));
                    break;
                case 9:
                    collection.Set("BonusRecommendationModel.FormComplete", "Y");
                    break;
            }
            return retVal;
        }
        private void fillApprovalHierarchy(FormCollection collection, int stage)
        {
            //Create domain context
            PrincipalContext ctx = new PrincipalContext(ContextType.Domain);

            UserPrincipal u = new UserPrincipal(ctx) { DisplayName = collection["BonusRecommendationModel.NomineeName"] }; 
            u.Enabled = true;
            List<System.DirectoryServices.AccountManagement.Principal> results = new List<System.DirectoryServices.AccountManagement.Principal>();
            var searcher = new PrincipalSearcher(u);
            results.AddRange(searcher.FindAll());
            UserPrincipal user = (UserPrincipal) results[0];
            switch (stage)
            {
                case 1:
                    collection.Set("BonusRecommendationModel.SupervisorEndorsement", "N");
                    collection.Set("BonusRecommendationModel.DepartmentEndorsement", "N");
                    string manager = DirectoryHelper.getManager(user);
                    string managerName = DirectoryHelper.findName(manager);
                    collection.Set("BonusRecommendationModel.SupervisorSignature", managerName);
                    if(managerName.Equals(collection["BonusRecommendationModel.SubmitterName"]))
                    {
                        collection.Set("BonusRecommendationModel.SupervisorSignatureDate", DateTime.Today.ToString("MM/dd/yyyy"));
                        collection.Set("BonusRecommendationModel.SupervisorEndorsement", "Y");
                        collection.Set("BonusRecommendationModel.CurrentStatus", "2");
                        string departmentTitleA = findDepartmentDirectorSFN60026(user);
                        string directorA = FilterList("nd", departmentTitleA, ADUserProperties.TITLE,_filter)[0];
                        string directorNameA = DirectoryHelper.findName(directorA);
                        collection.Set("BonusRecommendationModel.DepartmentSignature", directorNameA);
                        if(directorNameA.Equals(collection["BonusRecommendationModel.SubmitterName"]) || directorNameA.Equals(collection["BonusRecommendationModel.NomineeName"]))
                        {
                            if (directorNameA.Equals(collection["BonusRecommendationModel.NomineeName"]))
                            {
                                collection.Set("BonusRecommendationModel.DepartmentSignature", "Skipped - person cannot approve for themselves");
                            }
                            collection.Set("BonusRecommendationModel.DepartmentSignatureDate", DateTime.Today.ToString("MM/dd/yyyy"));
                            collection.Set("BonusRecommendationModel.DepartmentEndorsement", "Y");
                            collection.Set("BonusRecommendationModel.CurrentStatus", "3");
                            string chiefTitleA = DirectoryHelper.findChief(user);
                            string chiefA = FilterList("nd", chiefTitleA, ADUserProperties.TITLE, _filter)[0];
                            string chiefDeptA = DirectoryHelper.findName(chiefA);
                            collection.Set("BonusRecommendationModel.ChiefSignature", chiefDeptA);
                            if (chiefDeptA.Equals(collection["BonusRecommendationModel.SubmitterName"]))
                            {
                                collection.Set("BonusRecommendationModel.ChiefSignatureDate", DateTime.Today.ToString("MM/dd/yyyy"));
                                collection.Set("BonusRecommendationModel.CurrentStatus", "4");
                                collection.Set("BonusRecommendationModel.HRRepresentative", findHRNames());
                            }
                        }
                    }
                    break;

                case 2:
                    collection.Set("BonusRecommendationModel.SupervisorEndorsement", "Y");
                    collection.Set("BonusRecommendationModel.DepartmentEndorsement", "N");
                    string departmentTitle = findDepartmentDirectorSFN60026(user);
                    string director = FilterList("nd", departmentTitle, ADUserProperties.TITLE, _filter)[0];
                    string directorName = DirectoryHelper.findName(director);
                    collection.Set("BonusRecommendationModel.DepartmentSignature", directorName);
                    if (directorName.Equals(collection["BonusRecommendationModel.SubmitterName"]) || directorName.Equals(collection["BonusRecommendationModel.NomineeName"])
                        || directorName.Equals(collection["BonusRecommendationModel.SupervisorSignature"]))
                    {
                        if (directorName.Equals(collection["BonusRecommendationModel.NomineeName"]))
                        {
                            collection.Set("BonusRecommendationModel.DepartmentSignature", "Skipped - person cannot approve for themselves");
                        }
                        collection.Set("BonusRecommendationModel.DepartmentSignatureDate", DateTime.Today.ToString("MM/dd/yyyy"));
                        collection.Set("BonusRecommendationModel.DepartmentEndorsement", "Y");
                        collection.Set("BonusRecommendationModel.CurrentStatus", "3");
                        string chiefTitleA = findChiefSFN60026(user);
                        string chiefA = FilterList("nd", chiefTitleA, ADUserProperties.TITLE, _filter)[0];
                        string chiefDeptA = DirectoryHelper.findName(chiefA);
                        collection.Set("BonusRecommendationModel.ChiefSignature", chiefDeptA);
                        if (chiefDeptA.Equals(collection["BonusRecommendationModel.SubmitterName"]) || chiefDeptA.Equals(collection["BonusRecommendationModel.NomineeName"])
                            || chiefDeptA.Equals(collection["BonusRecommendationModel.SupervisorSignature"]))
                        {
                            collection.Set("BonusRecommendationModel.ChiefSignatureDate", DateTime.Today.ToString("MM/dd/yyyy"));
                            collection.Set("BonusRecommendationModel.CurrentStatus", "4");
                            collection.Set("BonusRecommendationModel.HRRepresentative", findHRNames());
                        }
                    }
                    break;

                case 3:
                    collection.Set("BonusRecommendationModel.SupervisorEndorsement", "Y");
                    collection.Set("BonusRecommendationModel.DepartmentEndorsement", "Y");
                    string chiefTitle = DirectoryHelper.findChief(user);
                    string chief = FilterList("nd", chiefTitle, ADUserProperties.TITLE, _filter)[0];
                    string chiefDept = DirectoryHelper.findName(chief);
                    collection.Set("BonusRecommendationModel.ChiefSignature", chiefDept);
                    if (chiefDept.Equals(collection["BonusRecommendationModel.SubmitterName"]) || chiefDept.Equals(collection["BonusRecommendationModel.NomineeName"])
                            || chiefDept.Equals(collection["BonusRecommendationModel.SupervisorSignature"]) || chiefDept.Equals(collection["BonusRecommendationModel.DepartmentSignature"]))
                    {
                        collection.Set("BonusRecommendationModel.ChiefSignatureDate", DateTime.Today.ToString("MM/dd/yyyy"));
                        collection.Set("BonusRecommendationModel.CurrentStatus", "4");
                        collection.Set("BonusRecommendationModel.HRRepresentative", findHRNames());
                    }
                    break;

                case 4:
                    collection.Set("BonusRecommendationModel.SupervisorEndorsement", "Y");
                    collection.Set("BonusRecommendationModel.DepartmentEndorsement", "Y");
                    //TODO: figure out how to pull HR Super users group
                    string HRnames = findHRNames();
                    collection.Set("BonusRecommendationModel.HRRepresentative", HRnames);
                    break;

                case 5:
                    collection.Set("BonusRecommendationModel.SupervisorEndorsement", "Y");
                    collection.Set("BonusRecommendationModel.DepartmentEndorsement", "Y");
                    //TODO: figure out how to pull Committee emails
                    string memberLogin = FilterList("nd", "Director", ADUserProperties.TITLE, _filter)[0];
                    string committeeNames = DirectoryHelper.findName(memberLogin);
                    memberLogin = FilterList("nd", "Chief Operations Officer", ADUserProperties.TITLE, _filter)[0];
                    committeeNames += ";" + DirectoryHelper.findName(memberLogin);
                    memberLogin = FilterList("nd", "General Counsel", ADUserProperties.TITLE, _filter)[0];
                    committeeNames += ";" + DirectoryHelper.findName(memberLogin);
                    memberLogin = FilterList("nd", "Chief of Injury Services", ADUserProperties.TITLE, _filter)[0];
                    committeeNames += ";" + DirectoryHelper.findName(memberLogin);
                    memberLogin = FilterList("nd", "Chief of Employer Services", ADUserProperties.TITLE, _filter)[0];
                    committeeNames += ";" + DirectoryHelper.findName(memberLogin);
                    memberLogin = FilterList("nd", "HR Director", ADUserProperties.TITLE, _filter)[0];
                    committeeNames += ";" + DirectoryHelper.findName(memberLogin);
                    collection.Set("BonusRecommendationModel.CommitteeApprovalList", committeeNames);
                    break;

                case 6:
                    collection.Set("BonusRecommendationModel.SupervisorEndorsement", "Y");
                    collection.Set("BonusRecommendationModel.DepartmentEndorsement", "Y");
                    chiefTitle = DirectoryHelper.findChief(user);
                    chief = FilterList("nd", chiefTitle, ADUserProperties.TITLE, _filter)[0];
                    chiefDept = DirectoryHelper.findName(chief);
                    collection.Set("BonusRecommendationModel.ChiefEndorsement", chiefDept);
                    break;

                case 7:
                    collection.Set("BonusRecommendationModel.SupervisorEndorsement", "Y");
                    collection.Set("BonusRecommendationModel.DepartmentEndorsement", "Y");
                    string wsiHead = FilterList("nd", "Director", ADUserProperties.TITLE, _filter)[0];
                    string WSIDirector = DirectoryHelper.findName(wsiHead);
                    collection.Set("BonusRecommendationModel.AgencyDirectorEndorsement", WSIDirector);
                    if (WSIDirector.Equals(collection["BonusRecommendationModel.ChiefEndorsement"]))
                    {
                        collection.Set("BonusRecommendationModel.AgencyDirectorEndorsementDate", DateTime.Today.ToString("MM/dd/yyyy"));
                        collection.Set("BonusRecommendationModel.CurrentStatus", "8");
                        //TODO: figure out how to pull HR Super users group
                        HRnames = findHRNames();
                        collection.Set("BonusRecommendationModel.HRRepresentative", HRnames);
                    }
                    break;

                case 8:
                    collection.Set("BonusRecommendationModel.SupervisorEndorsement", "Y");
                    collection.Set("BonusRecommendationModel.DepartmentEndorsement", "Y");
                    //TODO: figure out how to pull HR Super users group
                    HRnames = findHRNames();
                    collection.Set("BonusRecommendationModel.HRRepresentative", HRnames);
                    break;
            }
        }
        private string findHRNames()
        {

            List<string> list = FilterList("nd", "WSI-Human Resources", ADUserProperties.DEPARTMENT, _filter);
            string HRnames = "";
            foreach (string HR in list)
            {
                if (HR != "")
                {
                    HRnames += DirectoryHelper.findName(HR) + ";";
                }
            }
            return HRnames;
        }

        private string getEmails(string[] list)
        {
            PrincipalContext ctx = new PrincipalContext(ContextType.Domain);
            string emails = "";
            string names = "";
            foreach (string HR in list)
            {
                //if (!HR.StartsWith("wsi"))
                {
                    names = HR; // DirectoryHelper.findName(HR);
                    if (!names.Equals(""))
                    {
                        try
                        {
                            UserPrincipal personUP = UserPrincipal.FindByIdentity(ctx, names);


                            emails += personUP.EmailAddress + ";";
                            log.LogMessage(string.Format("SFN60026 getEmails: Found email {0} ", emails));
                        }
                        catch (Exception ex)
                        {
                            log.LogMessage(string.Format("SFN60026 getEmails: Could not find {0} ", names));
                            log.LogMessage(string.Format("SFN60026 getEmails: Trying Searching by Display Name"));
                            Console.WriteLine("Could not find " + names);
                            UserPrincipal testPrin = new UserPrincipal(ctx);
                            testPrin.DisplayName = names;
                            PrincipalSearcher srch = new PrincipalSearcher(testPrin);
                            UserPrincipal user = srch.FindOne() as UserPrincipal;
                            if(user != null)
                            {
                                emails = user.EmailAddress + ";";
                            }
                            else
                            {
                                log.LogMessage(string.Format("SFN60026 getEmails: Failed both searches for {0}", names));
                                sendErrorEmail(names);
                            }
                        }
                    }
                }
            }
            return emails;
        }

        private bool validateModel(FormCollection collection)
        {
            bool retVal = true;
            
            if (collection["BonusRecommendationModel.NomineeName"] == "[Enter Nominee Name]")
            {
                ModelState.AddModelError("BonusRecommendationModel.NomineeName", "Please choose a Nominee for the bonus.");
                retVal = false;
            }
            if (collection["BonusRecommendationModel.Justification"] == "")
            {
                ModelState.AddModelError("BonusRecommendationModel.Justification", "Please enter a justification for the bonus.");
                retVal = false;
            }
            if (collection["BonusRecommendationModel.LastPerformanceScore"] == "")
            {
                ModelState.AddModelError("BonusRecommendationModel.LastPerformanceScore", "Please enter the Last Performance Score.");
                retVal = false;
            }
            if (collection["BonusRecommendationModel.CurrentStatus"] == "5")
            {
                if (collection["BonusRecommendationModel.CommitteeApproval"] == "Yes" && collection["BonusRecommendationModel.CommitteeApprovalAmount"] == "")
                {
                    ModelState.AddModelError("BonusRecommendationModel.CommitteeApprovalAmount", "Please enter the Approved Bonus Amount.");
                    retVal = false;
                }
                if (collection["BonusRecommendationModel.CommitteeApproval"] == "Blank")
                {
                    ModelState.AddModelError("BonusRecommendationModel.CommitteeApproval", "Please choose the Approval Decision.");
                    retVal = false;
                }
                if (collection["BonusRecommendationModel.CommitteeApproval"] == "No" && collection["BonusRecommendationModel.CommitteeApprovalAmount"] != "")
                {
                    ModelState.AddModelError("BonusRecommendationModel.CommitteeApprovalAmount", "When denying bonus, Approved Bonus Amount needs to be blank.");
                    retVal = false;
                }
            }
            //check to see if we need to validate this
            if (collection["BonusRecommendationModel.CurrentStatus"] == "4")
            {
                //if ((collection["BonusRecommendationModel.LastBonusDate"] != null && collection["BonusRecommendationModel.LastBonusDate"] != "") 
                //    && collection["BonusRecommendationModel.LastBonusAmount"] == "")
                //{
                //    ModelState.AddModelError("BonusRecommendationModel.LastBonusAmount", "Please enter the Last Bonus Amount.");
                //    retVal = false;
                //}
                if (!collection["BonusRecommendationModel.FullTime"].Contains("true") && !collection["BonusRecommendationModel.PartTime"].Contains("true")
                && !collection["BonusRecommendationModel.ProbationaryEmployee"].Contains("true") && !collection["BonusRecommendationModel.Temporary"].Contains("true"))
                {
                    ModelState.AddModelError("BonusRecommendationModel.ProbationaryEmployee", "Please select at least one Employee Type.");
                    retVal = false;
                }
                if (collection["BonusRecommendationModel.StateEmployeeOneYear"] != "true")
                {
                    ModelState.AddModelError("BonusRecommendationModel.StateEmployeeOneYear", "Needs to be 'Yes' to approve form.");
                    retVal = false;
                }
                if (collection["BonusRecommendationModel.MeetsRequirements"] != "true")
                {
                    ModelState.AddModelError("BonusRecommendationModel.MeetsRequirements", "Needs to be 'Yes' to approve form.");
                    retVal = false;
                }
            }
            // Check the Dates
            DateTime startDate = DateTime.Parse(collection["BonusRecommendationModel.AccomplishmentStartDate"].ToString());
            DateTime endDate = DateTime.Parse(collection["BonusRecommendationModel.AccomplishmentEndDate"].ToString());
            if(startDate.CompareTo(endDate) > 0)
            {
                ModelState.AddModelError("BonusRecommendationModel.AccomplishmentEndDate", "End Date needs to be the same or after the Start Date.");
                retVal = false;
            }
            return retVal;
        }

        private void isSuperUser(UserPrincipal user)
        {
            dal = new ModelDALSFN60026();
            DataTable dtInfo = dal.GetSuperUsers("SFN60026");
            foreach(DataRow dr in dtInfo.Rows)
            {
                string login = dr["SU_LOGIN"].ToString();
                string title = dr["SU_TITLE"].ToString();
                if (login.Equals(user.SamAccountName))
                {
                    _superUser = true;
                }
                else if(title.Length > 0)
                {
                    List<string> memberLogin = FilterList("nd", title, ADUserProperties.TITLE, _filter);
                    foreach (string member in memberLogin)
                    {
                        if(member.Equals(user.SamAccountName))
                        {
                            _superUser = true;
                            break;
                        }
                    }
                }
                if (_superUser)
                    break;
            }
            _inHR = isInHR(user);
        }
        private bool isInHR(UserPrincipal user)
        {
            bool retVal = false;
            List<string> list = FilterList("nd", "WSI-Human Resources", ADUserProperties.DEPARTMENT, _filter);
            if(list.Contains(user.SamAccountName))
            {
                retVal = true;
            }
            return retVal;
        }
        private void sendDeniedEmail(FormCollection collection, int id)
        {
            // Get form url path
            string path = HttpContext.Request.Url.AbsoluteUri.ToString();
            List<string> host = new List<string>(path.Split(new string[] { "SFN" }, StringSplitOptions.None));

            List<string> toList = new List<string>();
            string endText = "";
            string emails = "";
            // Decide who denied the form and who gets the email
            if (collection["BonusRecommendationModel.HRRepresentative"] != "")
            {
                //If HR denys the form, change the wording and send to Sup, Dir, and Chief
                endText = "Review by Human Resources found " + collection["BonusRecommendationModel.NomineeName"] + " is not eligible to receive a Performance Bonus at this time.";
                toList.Add(collection["BonusRecommendationModel.SupervisorSignature"]);
                toList.Add(collection["BonusRecommendationModel.DepartmentSignature"]);
                toList.Add(collection["BonusRecommendationModel.ChiefSignature"]);
                emails = getEmails(toList.ToArray());
            }
            else
            {
                string names = findHRNames();
                emails = getEmails(names.Split(';'));
                if (collection["BonusRecommendationModel.ChiefSignature"] != "")
                    endText = "The nominee's Division Chief has denied the request. Click here to review the form: " + "<a href=\"" + host[0].ToString() + "SFN60026/View/" + id + "\">Performance Bonus Recommendation Form Link " + "</a>";
                else if (collection["BonusRecommendationModel.DepartmentSignature"] != "")
                    endText = "The nominee's Department Director has denied the request. Click here to review the form: " + "<a href=\"" + host[0].ToString() + "SFN60026/View/" + id + "\">Performance Bonus Recommendation Form Link " + "</a>";
                else if (collection["BonusRecommendationModel.SupervisorSignature"] != "")
                    endText = "The nominee's supervisor has denied the request. Click here to review the form: " + "<a href=\"" + host[0].ToString() + "SFN60026/View/" + id + "\">Performance Bonus Recommendation Form Link " + "</a>";
            }
            // Email configurations
            // Email to communications to notify of supervisor approval
            // string toAddress = string.Join("; ", toList.ToArray());
            string toAddress = emails;
            string from = "wsinoreply@nd.gov";
            string subject = collection["BonusRecommendationModel.NomineeName"] + " - A Performance Bonus Recommendation has been denied";

            // Email to supervisor for approval
            string body = "A Performance Bonus Nomination was submitted for " + collection["BonusRecommendationModel.NomineeName"] + ". " + endText;

            // Send email
            dal = new ModelDALSFN60026();
            dal.SendEmail(toAddress, from, subject, body);
        }

        private void sendErrorEmail(string name)
        {
            log.LogMessage(string.Format("SFN60026 sendErrorEmail"));
            // Send email
            dal = new ModelDALSFN60026();
            var results = dal.SendEmail(ConfigurationManager.AppSettings["AdminEmail"], ConfigurationManager.AppSettings["SystemEmail"], "Error SFN60026 - Could not find " + name, "System tried to find " + name + " but nothing was found for that name.");
            log.LogMessage(string.Format("SFN60026 Results From Error Email"));
            foreach (var item in results)
            {
                log.LogMessage(string.Format("SFN60026 {0} - {1}", item.Key, item.Value));
            }
        }

        private void sendEmail(string name, int id, string recipient)
        {
            int idx = 0;
            log.LogMessage(string.Format("SFN60026 sendEmail: {0} ", ++idx));
            // Get form url path
            string path = HttpContext.Request.Url.AbsoluteUri.ToString();
            log.LogMessage(string.Format("SFN60026 sendEmail: {0} ", ++idx));
            List<string> host = new List<string>(path.Split(new string[] { "SFN" }, StringSplitOptions.None));
            log.LogMessage(string.Format("SFN60026 sendEmail: {0} ", ++idx));

            List<string> toList = new List<string>();
            log.LogMessage(string.Format("SFN60026 sendEmail: {0} ", ++idx));
            string[] names = recipient.Split(';');
            log.LogMessage(string.Format("SFN60026 sendEmail: {0} ", ++idx));

            // Email configurations
            // Email to communications to notify of supervisor approval
            // string toAddress = string.Join("; ", toList.ToArray());
            string toAddress = getEmails(names);
            log.LogMessage(string.Format("SFN60026 sendEmail: {0} ", ++idx));
            string from = "wsinoreply@nd.gov";
            log.LogMessage(string.Format("SFN60026 sendEmail: {0} ", ++idx));
            string subject = name + " - A Performance Bonus Nomination for has been submitted";
            log.LogMessage(string.Format("SFN60026 sendEmail: {0} ", ++idx));

            // Email to supervisor for approval
            string body = "A Performance Bonus Nomination has been submitted for " + name + ". Click here to review the nomination: "
                + "<a href=\"" + host[0].ToString() + "SFN60026/View/" + id + "\">Performance Bonus Recommendation Form Link " + "</a>";

            log.LogMessage(string.Format("SFN60026 Sending Email To - names: {0} ", names));
            log.LogMessage(string.Format("SFN60026 Sending Email To - toAddress: {0} ", toAddress));
            log.LogMessage(string.Format("SFN60026 Sending Email From: {0} ", from));
            log.LogMessage(string.Format("SFN60026 Sending Email Subject: {0} ", subject));
            log.LogMessage(string.Format("SFN60026 Sending Email body: {0} ", body));

            // Send email
            dal = new ModelDALSFN60026();
            var results = dal.SendEmail(toAddress, from, subject, body);
            log.LogMessage(string.Format("SFN60026 Results From Email"));
            foreach (var item in results)
            {
                log.LogMessage(string.Format("SFN60026 {0} - {1}", item.Key, item.Value));
            }

        }
        private void sendCommitteeEmail(string name, int id, string recipient)
        {
            int idx = 0;
            log.LogMessage(string.Format("SFN60026 sendEmail: {0} ", ++idx));
            // Get form url path
            string path = HttpContext.Request.Url.AbsoluteUri.ToString();
            log.LogMessage(string.Format("SFN60026 sendEmail: {0} ", ++idx));
            List<string> host = new List<string>(path.Split(new string[] { "SFN" }, StringSplitOptions.None));
            log.LogMessage(string.Format("SFN60026 sendEmail: {0} ", ++idx));

            List<string> toList = new List<string>();
            log.LogMessage(string.Format("SFN60026 sendEmail: {0} ", ++idx));
            string[] names = recipient.Split(';');
            log.LogMessage(string.Format("SFN60026 sendEmail: {0} ", ++idx));

            // Email configurations
            // Email to communications to notify of supervisor approval
            // string toAddress = string.Join("; ", toList.ToArray());
            string toAddress = getEmails(names);
            log.LogMessage(string.Format("SFN60026 sendEmail: {0} ", ++idx));
            string from = "wsinoreply@nd.gov";
            log.LogMessage(string.Format("SFN60026 sendEmail: {0} ", ++idx));
            string subject = name + " - A Performance Bonus Nomination Ready for Committee Review";
            log.LogMessage(string.Format("SFN60026 sendEmail: {0} ", ++idx));

            // Email to supervisor for approval
            string body = "A Performance Bonus Nomination for " + name + ". is ready for review by the Performance Bonus Nomination Committee. " +
                "Click here to view the nomination prior to the Committee meeting: "
                + "<a href=\"" + host[0].ToString() + "SFN60026/View/" + id + "\">Performance Bonus Recommendation Form Link " + "</a>";

            log.LogMessage(string.Format("SFN60026 Sending Email To - names: {0} ", names));
            log.LogMessage(string.Format("SFN60026 Sending Email To - toAddress: {0} ", toAddress));
            log.LogMessage(string.Format("SFN60026 Sending Email From: {0} ", from));
            log.LogMessage(string.Format("SFN60026 Sending Email Subject: {0} ", subject));
            log.LogMessage(string.Format("SFN60026 Sending Email body: {0} ", body));

            // Send email
            dal = new ModelDALSFN60026();
            var results = dal.SendEmail(toAddress, from, subject, body);
            log.LogMessage(string.Format("SFN60026 Results From Email"));
            foreach (var item in results)
            {
                log.LogMessage(string.Format("SFN60026 {0} - {1}", item.Key, item.Value));
            }
        }
        private void sendEndorsementEmail(string name, int id, string recipient)
        {
            // Get form url path
            string path = HttpContext.Request.Url.AbsoluteUri.ToString();
            List<string> host = new List<string>(path.Split(new string[] { "SFN" }, StringSplitOptions.None));

            //Create domain context
            PrincipalContext ctx = new PrincipalContext(ContextType.Domain);

            UserPrincipal recipientUP = UserPrincipal.FindByIdentity(ctx, recipient);

            string recipientEmail = recipientUP.EmailAddress;


            // Email configurations
            // Email to communications to notify of supervisor approval
            string[] to = { recipientEmail };
            string toAddress = string.Join("; ", to.ToArray());
            string from = "wsinoreply@nd.gov";
            string subject = name + " - A Performance Bonus Recommendation for has been approved";

            // Email to supervisor for approval
            string body = "A Performance Bonus Nomination has been approved by the Performance Bonus Nomination Committee for " + name + ". Click here to complete your final endorsement: "
                + "<a href=\"" + host[0].ToString() + "SFN60026/View/" + id + "\">Performance Bonus Recommendation Form Link " + "</a>";

            // Send email
            dal = new ModelDALSFN60026();
            dal.SendEmail(toAddress, from, subject, body);
        }
        private void sendFinalEmail(string name, int id, string recipient)
        {
            // Get form url path
            string path = HttpContext.Request.Url.AbsoluteUri.ToString();
            List<string> host = new List<string>(path.Split(new string[] { "SFN" }, StringSplitOptions.None));

            List<string> toList = new List<string>();
            string[] names = recipient.Split(';');

            ////Create domain context
            //PrincipalContext ctx = new PrincipalContext(ContextType.Domain);
            //foreach (string person in names)
            //{
            //    if (!person.Equals(""))
            //    {
            //        try
            //        {
            //            UserPrincipal personUP = UserPrincipal.FindByIdentity(ctx, person);


            //            if(personUP.EmailAddress != null && personUP.EmailAddress != "" )
            //                toList.Add(personUP.EmailAddress);
            //        }
            //        catch (Exception ex)
            //        {
            //            Console.WriteLine("Could not find " + person);
            //        }
            //    }
            //}
            // Email configurations
            // Email to communications to notify of supervisor approval
            // string toAddress = string.Join("; ", toList.ToArray());
            string toAddress = getEmails(names);
            string from = "wsinoreply@nd.gov";
            string subject = name + " - Final Endorsement of a Performance Bonus Recommendation";

            // Email to supervisor for approval
            string body = "Final endorsement of the Performance Bonus Nomination for " + name + " has been completed. Click here to view the form: "
                + "<a href=\"" + host[0].ToString() + "SFN60026/View/" + id + "\">Performance Bonus Recommendation Form Link " + "</a>";

            // Send email
            dal = new ModelDALSFN60026();
            dal.SendEmail(toAddress, from, subject, body);
        }
        private void sendNextApprovalEmail(FormCollection collection, int id)
        {

            int stage = int.Parse(collection["BonusRecommendationModel.CurrentStatus"]);
            log.LogMessage(string.Format("SFN60026 sendNextApprovalEmail Stage: {0}", stage));
            log.LogMessage(string.Format("SFN60026 sendNextApprovalEmail SupervisorSignature: {0}", collection["BonusRecommendationModel.SupervisorSignature"]));
            log.LogMessage(string.Format("SFN60026 sendNextApprovalEmail NomineeName: {0}", collection["BonusRecommendationModel.NomineeName"]));
            switch (stage)
            {
                case 1:
                    sendEmail(collection["BonusRecommendationModel.NomineeName"], id, collection["BonusRecommendationModel.SupervisorSignature"]);
                    break;
                case 2:
                    sendEmail(collection["BonusRecommendationModel.NomineeName"], id, collection["BonusRecommendationModel.DepartmentSignature"]);
                    break;
                case 3:
                    sendEmail(collection["BonusRecommendationModel.NomineeName"], id, collection["BonusRecommendationModel.ChiefSignature"]);
                    break;
                case 4:
                    sendEmail(collection["BonusRecommendationModel.NomineeName"], id, collection["BonusRecommendationModel.HRRepresentative"]);
                    break;
                case 5:
                    sendCommitteeEmail(collection["BonusRecommendationModel.NomineeName"], id, collection["BonusRecommendationModel.CommitteeApprovalList"]);
                    break;
                case 6:
                    sendEndorsementEmail(collection["BonusRecommendationModel.NomineeName"], id, collection["BonusRecommendationModel.ChiefEndorsement"]);
                    break;
                case 7:
                    sendEndorsementEmail(collection["BonusRecommendationModel.NomineeName"], id, collection["BonusRecommendationModel.AgencyDirectorEndorsement"]);
                    break;
                case 8:
                    string HRnames = findHRNames();
                    sendFinalEmail(collection["BonusRecommendationModel.NomineeName"], id, HRnames);
                    break;
            }
        }
        private bool validateUser(UserPrincipal user,SFN60026Model model)
        {
            bool retVal = false;
            switch(model.CurrentStatus)
            {
                case 1:
                    retVal = DirectoryHelper.findName(user.SamAccountName) == model.SupervisorSignature && model.SupervisorSignatureDate.CompareTo(DateTime.Parse("1/1/2000")) <= 0;
                    break;
                case 2:
                    retVal = DirectoryHelper.findName(user.SamAccountName) == model.DepartmentSignature && model.DepartmentSignatureDate.CompareTo(DateTime.Parse("1/1/2000")) <= 0;
                    break;
                case 3:
                    retVal = DirectoryHelper.findName(user.SamAccountName) == model.ChiefSignature && model.ChiefSignatureDate.CompareTo(DateTime.Parse("1/1/2000")) <= 0;
                    break;
                case 4:
                    retVal = model.HRRepresentative.Contains(DirectoryHelper.findName(user.SamAccountName)) && model.HRRepresentativeDate.CompareTo(DateTime.Parse("1/1/2000")) <= 0;
                    break;
                case 5:
                    retVal = model.CommitteeApprovalList.Contains(DirectoryHelper.findName(user.SamAccountName)) && model.CommitteeApprovalDate.CompareTo(DateTime.Parse("1/1/2000")) <= 0;
                    break;
                case 6:
                    retVal = DirectoryHelper.findName(user.SamAccountName) == model.ChiefEndorsement && model.ChiefEndorsementDate.CompareTo(DateTime.Parse("1/1/2000")) <= 0;
                    break;
                case 7:
                    retVal = DirectoryHelper.findName(user.SamAccountName) == model.AgencyDirectorEndorsement && model.AgencyDirectorEndorsementDate.CompareTo(DateTime.Parse("1/1/2000")) <= 0;
                    break;
                default: //Status 0, 8, or 9
                    break;
            }
            isSuperUser(user);
            return retVal;
        }
        public static string findDepartmentDirectorSFN60026(UserPrincipal employee)
        {
            string department = "";
            string managerTitle = DirectoryHelper.getJobTitle(employee);
            if (managerTitle.Equals("Director"))
            {
                department = "no Department Director";
                return department;
            }
            string manager = DirectoryHelper.getManager(employee);
            using (PrincipalContext context = new PrincipalContext(ContextType.Domain))
            {
                using (UserPrincipal managerUser = UserPrincipal.FindByIdentity(context, manager))
                {
                    manager = managerUser.DisplayName;
                    managerTitle = DirectoryHelper.getJobTitle(managerUser);

                    if (managerTitle.Contains("Director") || managerTitle.Equals("General Counsel"))
                    {
                        department = managerTitle;
                        return department;
                    }
                    else if (managerTitle.Contains("Chief"))
                    {
                        department = managerTitle;
                    }
                    else
                    {
                        department = findDepartmentDirectorSFN60026(managerUser);
                    }
                }
            }
            return department;
        }
        public static string findChiefSFN60026(UserPrincipal employee)
        {
            string chief = "";
            string managerTitle = DirectoryHelper.getJobTitle(employee);
            if (managerTitle.Equals("Director"))
            {
                return "Chief Operations Officer";
            }
            string manager = DirectoryHelper.getManager(employee);

            using (PrincipalContext context = new PrincipalContext(ContextType.Domain))
            {
                using (UserPrincipal managerUser = UserPrincipal.FindByIdentity(context, manager))
                {
                    manager = managerUser.DisplayName;
                    managerTitle = DirectoryHelper.getJobTitle(managerUser);

                    if (managerTitle.Contains("Chief") || managerTitle.Equals("Director"))
                    {
                        chief = managerTitle;
                        return chief;
                    }
                    else
                    {
                        chief = findChiefSFN60026(managerUser);
                    }
                }
            }
            return chief;
        }
        private void findDeniedStep(SFN60026Model model)
        {
            if (model.SupervisorEndorsement == false)
            {
                ViewBag.deniedStep = 1;
                ViewBag.deniedBy = model.SupervisorSignature;
                ViewBag.deniedDate = model.SupervisorSignatureDate.ToString("MM/dd/yyyy");
            }
            else if (model.DepartmentEndorsement == false)
            {
                ViewBag.deniedStep = 2;
                ViewBag.deniedBy = model.DepartmentSignature;
                ViewBag.deniedDate = model.DepartmentSignatureDate.ToString("MM/dd/yyyy");
            }
            else if (model.HRRepresentative == "")
            {
                ViewBag.deniedStep = 3;
                ViewBag.deniedBy = model.ChiefSignature;
                ViewBag.deniedDate = model.ChiefSignatureDate.ToString("MM/dd/yyyy");
            }
            else if (model.CommitteeApprovalList == "")
            {
                ViewBag.deniedStep = 4;
                ViewBag.deniedBy = "Human Resources";
                ViewBag.deniedDate = model.HRRepresentativeDate.ToString("MM/dd/yyyy");
            }
            else if ((model.CommitteeApproval == SFN60026Model.Approval.No)|| (model.CommitteeApproval == SFN60026Model.Approval.Blank))
            {
                ViewBag.deniedStep = 5;
                ViewBag.deniedBy = "the Bonus Committee";
                ViewBag.deniedDate = model.CommitteeApprovalDate.ToString("MM/dd/yyyy");
            }
            //else if ()
            //{ }
            //else if ()
            //{ }
        }

        private void fillInCollectionValues(SFN60026Model model, FormCollection col)
        {
            //for each item in the colleciton
            foreach(string key in col.AllKeys)
            {
                if (key.StartsWith("BonusRecommendationModel"))
                {
                    string[] parts = key.Split('.');
                    string justKey = parts[1];
                    //get value
                    string value = col[key].ToString();
                    //used to check values from Checkboxes
                    bool boolCheck = value.Contains("true");
                    switch (justKey)
                    {
                        case "NomineeName":
                            if (value != model.NomineeName)
                                model.NomineeName = value;
                            break;
                        case "NomineeDepartment":
                            if (value != model.NomineeDepartment)
                                model.NomineeDepartment = value;
                            break;
                        case "NomineePosition":
                            if (value != model.NomineePosition)
                                model.NomineePosition = value;
                            break;
                        case "AccomplishmentStartDate":
                            if (value != model.AccomplishmentStartDate.ToString("yyyy-MM-dd"))
                                model.AccomplishmentStartDate = DateTime.Parse(value);
                            break;
                        case "AccomplishmentEndDate":
                            if (value != model.AccomplishmentEndDate.ToString("yyyy-MM-dd"))
                                model.AccomplishmentEndDate = DateTime.Parse(value);
                            break;
                        case "Justification":
                            if (value != model.Justification)
                                model.Justification = value;
                            break;
                        case "StateEmployeeOneYear":
                            if (value != model.StateEmployeeOneYear.ToString().ToLower())
                                model.StateEmployeeOneYear = bool.Parse(value);
                            break;
                        case "ProbationaryEmployee":
                            if (boolCheck != model.ProbationaryEmployee)
                                model.ProbationaryEmployee = boolCheck;
                            break;
                        case "FullTime":
                            if (boolCheck != model.FullTime)
                                model.FullTime = boolCheck;
                            break;
                        case "PartTime":
                            if (boolCheck != model.PartTime)
                                model.PartTime = boolCheck;
                            break;
                        case "Temporary":
                            if (boolCheck != model.Temporary)
                                model.Temporary = boolCheck;
                            break;
                        case "LastBonusDate":
                            DateTime dateCol;
                            if (value.Equals("")) //model.LastBonusDate.HasValue)
                            {
                                dateCol = DateTime.Parse("2000-01-01");
                            }
                            else
                            {
                                dateCol = DateTime.Parse(value);
                            }
                            DateTime dateMod = DateTime.Parse("2000-01-01");
                            if (model.LastBonusDate.HasValue)
                                dateMod = model.LastBonusDate.Value;
                            if(!dateCol.Equals(dateMod))
                                    model.LastBonusDate = dateCol;
                            break;
                        case "LastBonusAmount":
                            if (value != model.LastBonusAmount)
                                model.LastBonusAmount = value;
                            break;
                        case "LastPerformanceScore":
                            if (value != model.LastPerformanceScore)
                                model.LastPerformanceScore = value;
                            break;
                        case "HRAction":
                            if (value != model.HRAction.ToString().ToLower())
                                model.HRAction = bool.Parse(value);
                            break;
                        case "MeetsRequirements":
                            if (value != model.MeetsRequirements.ToString().ToLower())
                                model.MeetsRequirements = bool.Parse(value);
                            break;
                        case "CommitteeApproval":
                            if (value != model.CommitteeApproval.ToString())                                
                                model.CommitteeApproval = (value == "Yes") ? SFN60026Model.Approval.Yes : (value == "Blank") ? SFN60026Model.Approval.Blank : SFN60026Model.Approval.No;
                            break;
                        case "CommitteeApprovalAmount":
                            if (value != model.CommitteeApprovalAmount)
                                model.CommitteeApprovalAmount = value;
                            break;
                        case "Comments":
                            if (value != model.Comments)
                                model.Comments = value;
                            break;
                        default:
                            break;
                    }
                }
            }
        }
        private void getConfigValues()
        {
            try
            {
                string tmp = ConfigurationManager.AppSettings["CurrentFilter"];
                _filter = tmp ?? "";
            }
            catch (Exception ex)
            {
                _filter = "";
            }
        }

        private static List<string> FilterList(string domainName, string groupName, string property, string filter = "")
        {
            List<string> list;
            List<string> delete = new List<string>();
            string environment = ConfigurationManager.AppSettings["CurrentEnvironment"];
            if (environment.ToUpper() == "PROD")
            {
                list = DirectoryHelper.GetListOfAdUsersByProperty(domainName, groupName, property);
                foreach (string str in list)
                {
                    if (str.Contains("wsi_") || str.Contains("wsitrain") || str.Contains("wsibadge"))
                        delete.Add(str); 
                }
                foreach(string str in delete)
                {
                    list.Remove(str);
                }
            }
            else
            {
               list = DirectoryHelper.GetListOfAdUsersByProperty(domainName, groupName, property, "InfoPath");
            }
            return list;
        }

        #endregion
    }
}