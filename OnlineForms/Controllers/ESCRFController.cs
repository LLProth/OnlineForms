using OnlineForms.Models.ESCRF;
using OnlineForms.Models.ESCRFViewModel;
using OnlineForms.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using FormCollection = System.Web.Mvc.FormCollection;

namespace OnlineForms.Controllers
{
    [Authorize]
    public class ESCRFController : Controller
    {
        private ESCRFViewModel globalESCRF = new ESCRFViewModel();
        private ESCRFModelDal dal = new ESCRFModelDal();


        #region Index
        //function Renders Index page and all the tables 
        public ActionResult Index()
        {
            //Checking to see if a session varible exists to see if the session varible can be imported or needs to be created.
            if (Session["ESCRF"] != null)
            {
                globalESCRF = (ESCRFViewModel)Session["ESCRF"];
            }
            else
            {
                globalESCRF = new ESCRFViewModel();
            }
            //Getting titles from the db and storing it in the view model
            DataTable dtWSITitles = dal.GetWSITitlesInfo();
            globalESCRF.wsiTitles = WSITitleModel.GetWSITitleList(dtWSITitles);
            foreach (WSITitleModel wsiTitle in globalESCRF.wsiTitles)
            {
                string titles = wsiTitle.WSITitle.ToString();
                globalESCRF.titleStrings.Add(titles);
            }
            //telling the ui to render the full ui
            globalESCRF.ESCRFUI.MinimalUI = false;
            //Getting the logged in users identidy from AD
            System.Web.HttpContext.Current.Request.LogonUserIdentity.Impersonate();
            PrincipalContext context = new PrincipalContext(ContextType.Domain);
            UserPrincipal u = new UserPrincipal(context);
            UserPrincipal user = UserPrincipal.FindByIdentity(context, User.Identity.Name);
            DirectoryEntry dirEntry = (DirectoryEntry)user.GetUnderlyingObject();
            string title = dirEntry.Properties["Title"].Value.ToString();
            ViewBag.Name = user.Name;
            //Getting Dept Permissions from the DB
            DataTable dtDepartments = dal.GetDeptAfilliation(user.Name.ToString());
            List<ESCRFRoleModel> roles = ESCRFRoleModel.GetRoleList(dtDepartments);
            List<string> departments = new List<string>();
            if (roles.Count() != 0)
            {
                foreach (ESCRFRoleModel role in roles)
                {
                    if (role.EmployeeName == user.Name)
                    {
                        departments.Add(role.DepartmentAffiliation);
                    }
                }
            }
            globalESCRF.departments = departments;
            //Checking to see if the user has admin priveledges.
            ViewBag.isAdmin = false;
            //if (title == "Human Resource Officer" || Title == "HR Director")
            //if (title == "Software Engineer")
            //if (title == "HR Director I")
            if (title == "HR Director I" || title == "Human Resource Officer" || title == "HR Director" || title == "Learning and Development Coordinator" || title == "Software Engineer")
            {
                ViewBag.isAdmin = true;
            }

            if (ConfigurationManager.AppSettings["Environment"].ToUpper() == "PROD" && (title == "Business Analyst Supervisor" || title == "Application Services Supervisor" ||
                title == "Technical Services Lead Specialist" || user.DisplayName == "Wolf, Timothy J."))
            {
                ViewBag.ITSecurity = true;
            } else if (user.DisplayName == "Hall, Bradley W." || title == "Technical Services Lead Specialist" ||
                user.DisplayName == "Wolf, Timothy J." || user.DisplayName == "Kunz, Krisi L.")
            {
                ViewBag.ITSecurity = true;
            }

            PrincipalContext ctx = new PrincipalContext(ContextType.Domain);
            string deputy;
            bool isFinance = user.IsMemberOf(ctx, IdentityType.Name, "-Grp-WSI Finance");
            bool isHelpDesk = user.IsMemberOf(ctx, IdentityType.Name, "-Grp-WSI Help Desk");
            if (isFinance)
            {
                deputy = "Finance";
            }
            else if (isHelpDesk)
            {
                deputy = "Help Desk";
            }
            else if (title.ToLower() == "facilities manager" || title.ToLower() == "general trades maintenance worker")
            {
                deputy = "Facility Management";
            }
            else if (title.Contains("Technical Services"))
            {
                deputy = "IS Tech";
            }
            else
            {
                deputy = user.DisplayName;
            }

            var taskLists = new List<string>();

            DataTable taskListIds = dal.SelectDistinctTasklistIDByDeputy(deputy);
            foreach (DataRow row in taskListIds.Rows)
            {
                taskLists.Add(row["TASK_LIST_ID"].ToString());
            }

            ViewBag.HasTasks = taskLists;

            var completedITLists = new List<string>();

            //Retreiving all TaskListItems 
            DataTable dtTasks = dal.GetTaskListItemInfo();
            globalESCRF.TaskListItemHolder = TaskListItemModel.GetTaskListItemList(dtTasks);

            foreach (TaskListItemModel task in globalESCRF.TaskListItemHolder)
            {
                if (task.Deputy.ToLower() == "is tech" || task.Deputy.ToLower() == "help desk")
                {
                    if (task.Completed == false && task.NotApplicable == false)
                    {
                        completedITLists.Add(task.TaskListID.ToString());
                    }
                }
            }
            string itCompleted = string.Join("; ", completedITLists.ToArray());
            ViewBag.ITTasksComplete = itCompleted;
            //Checking each TaskListItem to see if it is relavant to the current user
            if (globalESCRF.TaskListItemHolder.Count() != 0)
            {

                foreach (TaskListItemModel taskListItem in globalESCRF.TaskListItemHolder.ToList())
                {
                    //Current TaskListItem is meant for the current user. Since it is relevant we do not need to check any other things
                    if (taskListItem.Deputy == ViewBag.Name)
                    {
                        continue;
                    }


                    if (globalESCRF.departments.Count() != 0)
                    {   //Since the TaskListItem is not equal to the User's name 
                        //Looping through all the department permissions that the user has against the deputy. 
                        foreach (string department in globalESCRF.departments)
                        {
                            if (department != taskListItem.Deputy.ToString())
                            {
                                globalESCRF.TaskListItemHolder.Remove(taskListItem);
                            }
                            //If one of the user's permissions does match then break the loop because it does not need to be removed
                            else
                            {
                                break;
                            }
                        };
                    }

                }
            }
            globalESCRF.IndexHolder.Count();
            //setting how many results would be on each page of the table
            globalESCRF.ESCRFUI.ResultsOnPage = 20;
            // Clearing UI Fields
            globalESCRF.IndexHolder.Clear();
            globalESCRF.ESCRFUI.FormsOnPagesArray.Clear();

            //Now that the data has been retreived we need to decide what data is shown
            //Sorting by type of form 
            if (globalESCRF.ESCRFUI.AllFormsToggle == true)
            {
                ESCRFModelDal newHireDal = new ESCRFModelDal();
                DataTable dtNewHire = newHireDal.GetESCRFNewHireInfo();
                if (dtNewHire != null)
                {
                    List<IESCRF> newHires = NewHireModel.GetNewHireList(dtNewHire);
                    globalESCRF.IndexHolder.AddRange(newHires);
                }

                ESCRFModelDal termDal = new ESCRFModelDal();
                DataTable dtTerm = termDal.GetESCRFTerminationInfo();
                if (dtTerm != null)
                {
                    List<IESCRF> terminations = TerminationModel.GetTerminationList(dtTerm);
                    globalESCRF.IndexHolder.AddRange(terminations);
                }

                ESCRFModelDal changeDal = new ESCRFModelDal();
                DataTable dtChange = changeDal.GetESCRFChangeInfo();
                if (dtChange != null)
                {
                    List<IESCRF> changes = ChangeModel.GetChangeList(dtChange);
                    globalESCRF.IndexHolder.AddRange(changes);
                }

                ESCRFModelDal nameDal = new ESCRFModelDal();
                DataTable dtName = nameDal.GetESCRFNameInfo();
                if (dtName != null)
                {
                    List<IESCRF> names = NameModel.GetNameList(dtName);
                    globalESCRF.IndexHolder.AddRange(names);
                }

            }
            if (globalESCRF.ESCRFUI.NewHireToggle == true)
            {
                ESCRFModelDal newHireDal = new ESCRFModelDal();
                DataTable dtNewHire = newHireDal.GetESCRFNewHireInfo();
                List<IESCRF> NewHire = NewHireModel.GetNewHireList(dtNewHire);
                globalESCRF.IndexHolder.AddRange(NewHire);
            }
            if (globalESCRF.ESCRFUI.TerminationToggle == true)
            {
                ESCRFModelDal termDal = new ESCRFModelDal();
                DataTable dtTerm = termDal.GetESCRFTerminationInfo();
                List<IESCRF> Termination = TerminationModel.GetTerminationList(dtTerm);
                globalESCRF.IndexHolder.AddRange(Termination);
            }
            if (globalESCRF.ESCRFUI.ChangeToggle == true)
            {
                ESCRFModelDal changeDal = new ESCRFModelDal();
                DataTable dtChange = changeDal.GetESCRFChangeInfo();
                List<IESCRF> Change = ChangeModel.GetChangeList(dtChange);
                globalESCRF.IndexHolder.AddRange(Change);
            }
            if (globalESCRF.ESCRFUI.NameToggle == true)
            {
                ESCRFModelDal nameDal = new ESCRFModelDal();
                DataTable dtName = nameDal.GetESCRFNameInfo();
                List<IESCRF> Name = NameModel.GetNameList(dtName);
                globalESCRF.IndexHolder.AddRange(Name);
            }
            //Now that type of form has been sorted lets further sort by what step in the workflow it is at
            List<IESCRF> tempHold = new List<IESCRF>();
            if (globalESCRF.ESCRFUI.AllStatusToggle == true)
            {
                foreach (IESCRF form in globalESCRF.IndexHolder.ToList())
                {
                    tempHold.Add(form);
                }
            }

            if (globalESCRF.ESCRFUI.NotDeployedToggle == true)
            {
                foreach (IESCRF form in globalESCRF.IndexHolder.ToList())
                {
                    if (form.TaskListID() == 0)
                    {
                        tempHold.Add(form);
                    }
                }
            }
            if (globalESCRF.ESCRFUI.InProgressToggle == true)
            {
                foreach (IESCRF form in globalESCRF.IndexHolder.ToList())
                {
                    if (form.TaskListID() != 0)
                    {
                        DateTime early = DateTime.MinValue;
                        DataTable dtTaskList = dal.GetTaskListInfoByID(form.TaskListID());
                        TaskListModel model = TaskListModel.ConvertDataTableToTaskList(dtTaskList);
                        if (model.FinishedDate == early)
                        {
                            tempHold.Add(form);
                        }
                    }
                }
            }
            if (globalESCRF.ESCRFUI.CompletedToggle == true)
            {
                foreach (IESCRF form in globalESCRF.IndexHolder.ToList())
                {
                    if (form.TaskListID() != 0)
                    {
                        DateTime early = DateTime.MinValue;
                        DataTable dtTaskList = dal.GetTaskListInfoByID(form.TaskListID());
                        TaskListModel model = TaskListModel.ConvertDataTableToTaskList(dtTaskList);
                        if (model.FinishedDate != early)
                        {
                            tempHold.Add(form);
                        }
                    }
                }
            }

            

            //Now That we have the forms we want to show lets calculate the status
            foreach (IESCRF form in globalESCRF.IndexHolder.ToList())
            {
                form.CreateStatus();
            }
            //Now that we have the forms we want to show now we need calculate what forms based on what page. ex: page 1 show forms 0-19 page 2 show forms 20-39 
            globalESCRF.IndexHolder = tempHold;
            //Calculating other UI values
            //FormsOnPagesArray is an array that represents how many records will be on each page.
            int count = 0;
            for (int i = 0; i < globalESCRF.IndexHolder.Count; i++)
            {
                if (globalESCRF.IndexHolder.Count == globalESCRF.ESCRFUI.ResultsOnPage)
                {
                    globalESCRF.ESCRFUI.FormsOnPagesArray.Add(globalESCRF.IndexHolder.Count);
                    count = 0;
                }
                count++;
            }
            if (count != 0)
            {
                globalESCRF.ESCRFUI.FormsOnPagesArray.Add(count);
            }
            //Calculating pages
            if (globalESCRF.IndexHolder.Count % globalESCRF.ESCRFUI.ResultsOnPage == 0)
            {
                globalESCRF.ESCRFUI.Pages = ((globalESCRF.IndexHolder.Count / (int)globalESCRF.ESCRFUI.ResultsOnPage) - 1);
            }
            else
            {
                double recordAmount = globalESCRF.IndexHolder.Count;
                double pagesBeforeRounding = recordAmount / globalESCRF.ESCRFUI.ResultsOnPage;
                double pagesRounded = Math.Ceiling(pagesBeforeRounding);
                globalESCRF.ESCRFUI.Pages = (int)pagesRounded - 1;
            }
            //Now that we have forms and calced the pages lets sort
            switch (globalESCRF.SortBy)
            {
                case "Change Type":
                    if (globalESCRF.IsAscending == true)
                    {
                        List<IESCRF> index = globalESCRF.IndexHolder.OrderBy(p => p.ChangeType()).ToList();
                        globalESCRF.IndexHolder = index;
                    }
                    if (globalESCRF.IsAscending == false)
                    {
                        List<IESCRF> index = globalESCRF.IndexHolder.OrderBy(p => p.ChangeType()).ToList();
                        index.Reverse();
                        globalESCRF.IndexHolder = index;
                    }
                    break;
                case "Employee Name":
                    if (globalESCRF.IsAscending == true)
                    {
                        List<IESCRF> index = globalESCRF.IndexHolder.OrderBy(p => p.Name()).ToList();
                        globalESCRF.IndexHolder = index;
                    }
                    if (globalESCRF.IsAscending == false)
                    {
                        List<IESCRF> index = globalESCRF.IndexHolder.OrderBy(p => p.Name()).ToList();
                        index.Reverse();
                        globalESCRF.IndexHolder = index;
                    }
                    break;
                case "Supervisor":
                    if (globalESCRF.IsAscending == true)
                    {
                        List<IESCRF> index = globalESCRF.IndexHolder.OrderBy(p => p.CurrentSupervisor()).ToList();
                        globalESCRF.IndexHolder = index;
                    }
                    if (globalESCRF.IsAscending == false)
                    {
                        List<IESCRF> index = globalESCRF.IndexHolder.OrderBy(p => p.CurrentSupervisor()).ToList();
                        index.Reverse();
                        globalESCRF.IndexHolder = index;
                    }
                    break;
                case "New Supervisor":
                    if (globalESCRF.IsAscending == true)
                    {
                        List<IESCRF> index = globalESCRF.IndexHolder.OrderBy(p => p.NewSupervisor()).ToList();
                        globalESCRF.IndexHolder = index;
                    }
                    if (globalESCRF.IsAscending == false)
                    {
                        List<IESCRF> index = globalESCRF.IndexHolder.OrderBy(p => p.NewSupervisor()).ToList();
                        index.Reverse();
                        globalESCRF.IndexHolder = index;
                    }
                    break;
                case "Modified By":
                    if (globalESCRF.IsAscending == true)
                    {
                        List<IESCRF> index = globalESCRF.IndexHolder.OrderBy(p => p.ModifiedBy()).ToList();
                        globalESCRF.IndexHolder = index;
                    }
                    if (globalESCRF.IsAscending == false)
                    {
                        List<IESCRF> index = globalESCRF.IndexHolder.OrderBy(p => p.ModifiedBy()).ToList();
                        index.Reverse();
                        globalESCRF.IndexHolder = index;
                    }
                    break;
                case "Submitted On":
                    if (globalESCRF.IsAscending == true)
                    {
                        List<IESCRF> index = globalESCRF.IndexHolder.OrderBy(p => p.CreatedDate()).ToList();
                        globalESCRF.IndexHolder = index;
                    }
                    if (globalESCRF.IsAscending == false)
                    {
                        List<IESCRF> index = globalESCRF.IndexHolder.OrderBy(p => p.CreatedDate()).ToList();
                        index.Reverse();
                        globalESCRF.IndexHolder = index;
                    }
                    break;
                case "Effective On":
                    if (globalESCRF.IsAscending == true)
                    {
                        List<IESCRF> index = globalESCRF.IndexHolder.OrderBy(p => p.EffectiveDate).ToList();
                        globalESCRF.IndexHolder = index;
                    }
                    if (globalESCRF.IsAscending == false)
                    {
                        List<IESCRF> index = globalESCRF.IndexHolder.OrderBy(p => p.EffectiveDate).ToList();
                        index.Reverse();
                        globalESCRF.IndexHolder = index;
                    }
                    break;
                case "Status":
                    if (globalESCRF.IsAscending == true)
                    {
                        List<IESCRF> index = globalESCRF.IndexHolder.OrderBy(p => p.Status()).ToList();
                        globalESCRF.IndexHolder = index;
                    }
                    if (globalESCRF.IsAscending == false)
                    {
                        List<IESCRF> index = globalESCRF.IndexHolder.OrderBy(p => p.ModifiedBy()).ToList();
                        index.Reverse();
                        globalESCRF.IndexHolder = index;
                    }
                    break;
            }

            Session["ESCRF"] = globalESCRF;
            return View("~/Views/Forms/ESCRF/Index.cshtml", globalESCRF); ;
        }

        //Routing off index
        // This action controls what page you go to when you select a new form.
        public ActionResult ESCRFRouter(FormCollection collection)
        {
            if (Session["User"] != null)
            {
                globalESCRF = (ESCRFViewModel)Session["User"];
            }
            else
            {
                globalESCRF = new ESCRFViewModel();
            }
            if (collection["ESCRFUI.ChangeTypes"] == "New Hire")
            {
                Session["User"] = globalESCRF;
                return RedirectToAction("NewHire", globalESCRF);
            }
            if (collection["ESCRFUI.ChangeTypes"] == "Termination")
            {
                Session["User"] = globalESCRF;
                return RedirectToAction("Termination");
            }
            if (collection["ESCRFUI.ChangeTypes"] == "Change In WSI")
            {
                Session["User"] = globalESCRF;
                return RedirectToAction("Change");
            }
            if (collection["ESCRFUI.ChangeTypes"] == "Name Change")
            {
                Session["User"] = globalESCRF;
                return RedirectToAction("Name");
            }
            else
            {
                Session["User"] = globalESCRF;
                return RedirectToAction("Index");
            }
        }

        public void UpdateStatus(IESCRF eSCRF)
        {
            eSCRF.CreateStatus();
        }
        #endregion

        #region New Hire
        //Used to create the NewHire Page
        public ActionResult NewHire()
        {
            //grabbing context
            if (Session["ESCRF"] == null)
            {
                globalESCRF = new ESCRFViewModel();
            }
            else
            {
                globalESCRF = (ESCRFViewModel)Session["ESCRF"];
            }

            // Determine which urls to use for the AD functions
            // These are determined by the Environment app setting in the web.config
            if (ConfigurationManager.AppSettings["Environment"].ToUpper() == "DEV")
            {
                ViewBag.Validation = "/api/Validation/?query=";
                ViewBag.Verify = "/api/verify/?query=";
                ViewBag.Retrieve = "/api/retrieve/?query=";
            }
            else if (ConfigurationManager.AppSettings["Environment"].ToUpper() == "TEST")
            {
                ViewBag.Validation = "https://itdwsinett2.netstaging.nd.gov/WSI/onlineforms/api/Validation/?query=";
                ViewBag.Verify = "https://itdwsinett2.netstaging.nd.gov/WSI/onlineforms/api/verify/?query=";
                ViewBag.Retrieve = "https://itdwsinett2.netstaging.nd.gov/WSI/onlineforms/api/retrieve/?query=";
            }
            else if (ConfigurationManager.AppSettings["Environment"].ToUpper() == "PROD")
            {
                ViewBag.Validation = "https://webforms.wsi.nd.gov/api/Validation/?query=";
                ViewBag.Verify = "https://webforms.wsi.nd.gov/api/verify/?query=";
                ViewBag.Retrieve = "https://webforms.wsi.nd.gov/api/retrieve/?query=";
            }

            //for the titles dropdown
            ViewBag.Titles = globalESCRF.titleStrings;
            //setting the UI
            globalESCRF.ESCRFUI.MinimalUI = true;
            globalESCRF.NewHire = new NewHireModel();
            PrincipalContext context = new PrincipalContext(ContextType.Domain);
            UserPrincipal user = UserPrincipal.FindByIdentity(context, User.Identity.Name);
            ViewBag.Name = user.Name;
            Session["ESCRF"] = globalESCRF;
            return View("~/Views/Forms/ESCRF/NewHire.cshtml", globalESCRF);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //Submitting the form to the DB and routes you back to the Index page
        public ActionResult SubmitNewHire(FormCollection collection)
        {
            if (Session["ESCRF"] == null)
            {
                globalESCRF = new ESCRFViewModel();
            }
            else
            {
                globalESCRF = (ESCRFViewModel)Session["ESCRF"];
            }
            int ESCRFNewHireID = dal.InsertESCRFNewHireGetID(collection);
            dal.InsertChangelistInfoNewHire(collection, ESCRFNewHireID);
            DataTable dtNewHire = dal.GetESCRFNewHireInfoByID(ESCRFNewHireID);
            globalESCRF.NewHire = NewHireModel.ConvertDataTableToNewHire(dtNewHire);
            Session["ESCRF"] = globalESCRF;
            return RedirectToAction("NewHireView", new { id = globalESCRF.NewHire.ID });
        }

        //Requires ID of thee form and returns a view of the completed form
        public ActionResult NewHireView(int id)
        {
            //grabbing context
            if (Session["ESCRF"] != null)
            {
                globalESCRF = (ESCRFViewModel)Session["ESCRF"];
            }
            else
            {
                globalESCRF = new ESCRFViewModel();
            }
            //setting UI 
            globalESCRF.ESCRFUI.MinimalUI = true;
            globalESCRF.ChangeType = "New Hire";
            //This Varible will controll whether the create list button is visible
            ViewBag.CreateListVisible = true;
            //preparing the a data structure just in case the tasklist has been deployed
            globalESCRF.TaskListItemHolder = new List<TaskListItemModel>();
            dal = new ESCRFModelDal();
            //grabbing the form data
            DataTable dtNewHire = dal.GetESCRFNewHireInfoByID(id);
            globalESCRF.NewHire = NewHireModel.ConvertDataTableToNewHire(dtNewHire);
            //If the TaskListId is not 0 means a taskList has been deployed and will need to be retreived
            //will need to be cleaned up
            if (globalESCRF.NewHire.TaskListID != 0)
            {
                DataTable dtTaskList = dal.GetTaskListInfoByID(globalESCRF.NewHire.TaskListID);
                globalESCRF.TaskList = TaskListModel.ConvertDataTableToTaskList(dtTaskList);
                if (globalESCRF.TaskList.IsDeployed == true)
                {
                    ViewBag.CreateListVisible = false;
                }
                if (globalESCRF.TaskList.CreatedDate != DateTime.MinValue)
                {
                    DataTable dtTaskListItems = dal.GetTaskListItemInfoByTaskListID(globalESCRF.TaskList.ID);
                    globalESCRF.TaskListItemHolder = TaskListItemModel.GetTaskListItemList(dtTaskListItems);
                }
            }
            Session["ESCRF"] = globalESCRF;
            return View("~/Views/Forms/ESCRF/NewHireView.cshtml", globalESCRF);
        }

        //Retreives the NewHire Form data that is stored in the ViewModel
        public ActionResult GetNewHireViewByContext()
        {
            //grabbing context
            if (Session["ESCRF"] != null)
            {
                globalESCRF = (ESCRFViewModel)Session["ESCRF"];
            }
            else
            {
                globalESCRF = new ESCRFViewModel();
            }
            //setting UI
            globalESCRF.ESCRFUI.MinimalUI = true;
            dal = new ESCRFModelDal();
            //loading the form data
            DataTable dtNewHire = dal.GetESCRFNewHireInfoByID(globalESCRF.NewHire.ID);
            globalESCRF.NewHire = NewHireModel.ConvertDataTableToNewHire(dtNewHire);
            //If the TaskListId is not 0 means a taskList has been deployed and will need to be retreived
            if (globalESCRF.NewHire.TaskListID != 0)
            {
                DataTable dtTaskList = dal.GetTaskListInfoByID(globalESCRF.NewHire.TaskListID);
                globalESCRF.TaskList = TaskListModel.ConvertDataTableToTaskList(dtTaskList);
                if (globalESCRF.TaskList.CreatedDate != DateTime.MinValue)
                {
                    DataTable dtTaskListItems = dal.GetTaskListItemInfoByTaskListID(globalESCRF.TaskList.ID);
                    globalESCRF.TaskListItemHolder = TaskListItemModel.GetTaskListItemList(dtTaskListItems);
                }
            }
            Session["ESCRF"] = globalESCRF;
            return View("NewHireView", globalESCRF);
        }
        //Requires ID of thee form and returns a view to edit the form
        public ActionResult NewHireEditView(int id)
        {
            //grabbing context
            if (Session["ESCRF"] != null)
            {
                globalESCRF = (ESCRFViewModel)Session["ESCRF"];
            }
            else
            {
                globalESCRF = new ESCRFViewModel();
            }
            //Need to set this for the cancel button 
            globalESCRF.ChangeType = "New Hire";
            //setting the UI
            globalESCRF.ESCRFUI.MinimalUI = true;
            //grabbing form data
            dal = new ESCRFModelDal();
            DataTable dtNewHire = dal.GetESCRFNewHireInfoByID(id);
            globalESCRF.NewHire = NewHireModel.ConvertDataTableToNewHire(dtNewHire);
            globalESCRF.CreateHolder = globalESCRF.NewHire;
            Session["ESCRF"] = globalESCRF;
            return View("~/Views/Forms/ESCRF/NewHireEditView.cshtml", globalESCRF);
        }
        //submitting the data of the edited form 
        public ActionResult UpdateNewHire(FormCollection collection)
        {
            //grabbing the context
            if (Session["ESCRF"] != null)
            {
                globalESCRF = (ESCRFViewModel)Session["ESCRF"];
            }
            else
            {
                globalESCRF = new ESCRFViewModel();
            }
            // Did this because form data is submitted
            int ID = Int32.Parse(collection[13]);
            //used to debug 
            //Debug.WriteLine(ID);
            //grabbing form data
            DataTable dtNewHire = dal.GetESCRFNewHireInfoByID(ID);
            globalESCRF.NewHire = NewHireModel.ConvertDataTableToNewHire(dtNewHire);
            Debug.WriteLine(globalESCRF.NewHire);
            dal.UpdateNewHireDates(collection);
            dtNewHire = dal.GetESCRFNewHireInfoByID(globalESCRF.NewHire.ID);
            globalESCRF.NewHire = NewHireModel.ConvertDataTableToNewHire(dtNewHire);
            Session["ESCRF"] = globalESCRF;
            return RedirectToAction("NewHireView", new { id = globalESCRF.NewHire.ID });
        }

        public ActionResult NewHirePrint(int id, int taskListId)
        {
            //grabbing the context
            if (Session["ESCRF"] != null)
            {
                globalESCRF = (ESCRFViewModel)Session["ESCRF"];
            }
            else
            {
                globalESCRF = new ESCRFViewModel();
            }
            ViewBag.ID = taskListId;
            //setting the UI
            globalESCRF.ESCRFUI.MinimalUI = true;
            //grabbing form data
            dal = new ESCRFModelDal();
            DataTable dtNewHire = dal.GetESCRFNewHireInfoByID(id);
            globalESCRF.NewHire = NewHireModel.ConvertDataTableToNewHire(dtNewHire);
            Session["ESCRF"] = globalESCRF;
            return View("~/Views/Forms/ESCRF/NewHirePrintView.cshtml", globalESCRF);
        }
        #endregion

        #region Termination
        public ActionResult Termination()
        {
            //grabbing the context
            if (Session["ESCRF"] == null)
            {
                globalESCRF = new ESCRFViewModel();
            }
            else
            {
                globalESCRF = (ESCRFViewModel)Session["ESCRF"];
            }

            // Determine which urls to use for the AD functions
            // These are determined by the Environment app setting in the web.config
            if (ConfigurationManager.AppSettings["Environment"].ToUpper() == "DEV")
            {
                ViewBag.Validation = "/api/Validation/?query=";
                ViewBag.Verify = "/api/verify/?query=";
                ViewBag.Retrieve = "/api/retrieve/?query=";
            }
            else if (ConfigurationManager.AppSettings["Environment"].ToUpper() == "TEST")
            {
                ViewBag.Validation = "https://itdwsinett2.netstaging.nd.gov/WSI/onlineforms/api/Validation/?query=";
                ViewBag.Verify = "https://itdwsinett2.netstaging.nd.gov/WSI/onlineforms/api/verify/?query=";
                ViewBag.Retrieve = "https://itdwsinett2.netstaging.nd.gov/WSI/onlineforms/api/retrieve/?query=";
            }
            else if (ConfigurationManager.AppSettings["Environment"].ToUpper() == "PROD")
            {
                ViewBag.Validation = "https://webforms.wsi.nd.gov/api/Validation/?query=";
                ViewBag.Verify = "https://webforms.wsi.nd.gov/api/verify/?query=";
                ViewBag.Retrieve = "https://webforms.wsi.nd.gov/api/retrieve/?query=";
            }

            //grabbing titles from dbs
            ViewBag.Titles = globalESCRF.titleStrings;
            //setting the ui
            globalESCRF.ESCRFUI.MinimalUI = true;
            globalESCRF.Termination = new TerminationModel();
            //grabbing info about the user from ad
            PrincipalContext context = new PrincipalContext(ContextType.Domain);
            UserPrincipal user = UserPrincipal.FindByIdentity(context, User.Identity.Name);
            ViewBag.Name = user.Name;
            Session["ESCRF"] = globalESCRF;
            return View("~/Views/Forms/ESCRF/Termination.cshtml", globalESCRF);
        }
        //Action used to submit Termination form
        public ActionResult SubmitTermination(FormCollection collection)
        {
            if (Session["ESCRF"] != null)
            {
                globalESCRF = (ESCRFViewModel)Session["ESCRF"];
            }
            else
            {
                globalESCRF = new ESCRFViewModel();
            }
            int ESCRFTerminationID = dal.InsertESCRFTerminationGetID(collection);
            dal.InsertChangelistInfoTermination(collection, ESCRFTerminationID);
            DataTable dtTerm = dal.GetESCRFTerminationInfoByID(ESCRFTerminationID);
            globalESCRF.Termination = TerminationModel.ConvertDataTableToTermination(dtTerm);
            Session["ESCRF"] = globalESCRF;
            return RedirectToAction("TerminationView", new { id = globalESCRF.Termination.ID });

        }

        //retreiving the completed form from the db
        public ActionResult TerminationView(int id)
        {
            //grabbing context
            if (Session["ESCRF"] != null)
            {
                globalESCRF = (ESCRFViewModel)Session["ESCRF"];
            }
            else
            {
                globalESCRF = new ESCRFViewModel();
            }
            globalESCRF.ChangeType = "Termination";
            //This Varible will controll whether the create list button is visible
            ViewBag.CreateListVisible = true;
            //setting the ui
            globalESCRF.ESCRFUI.MinimalUI = true;
            globalESCRF.TaskListItemHolder = new List<TaskListItemModel>();
            dal = new ESCRFModelDal();
            //grabbing the form info
            DataTable dtTermination = dal.GetESCRFTerminationInfoByID(id);
            globalESCRF.Termination = TerminationModel.ConvertDataTableToTermination(dtTermination);
            //if the form has a tasklist associated with it then grab the tasklist and the tasklistitems
            if (globalESCRF.Termination.TaskListID != 0)
            {
                DataTable dtTaskList = dal.GetTaskListInfoByID(globalESCRF.Termination.TaskListID);
                globalESCRF.TaskList = TaskListModel.ConvertDataTableToTaskList(dtTaskList);
                if (globalESCRF.TaskList.IsDeployed == true)
                {
                    ViewBag.CreateListVisible = false;
                }
                if (globalESCRF.TaskList.CreatedDate != DateTime.MinValue)
                {
                    DataTable dtTaskListItems = dal.GetTaskListItemInfoByTaskListID(globalESCRF.TaskList.ID);
                    globalESCRF.TaskListItemHolder = TaskListItemModel.GetTaskListItemList(dtTaskListItems);
                }
            }
            globalESCRF.CreateHolder = globalESCRF.Termination;
            Session["ESCRF"] = globalESCRF;
            return View("~/Views/Forms/ESCRF/TerminationView.cshtml", globalESCRF);
        }

        public ActionResult GetTerminationViewByContext()
        {
            //grabbing the context
            if (Session["ESCRF"] != null)
            {
                globalESCRF = (ESCRFViewModel)Session["ESCRF"];
            }
            else
            {
                globalESCRF = new ESCRFViewModel();
            }
            //setting the ui
            globalESCRF.ESCRFUI.MinimalUI = true;
            dal = new ESCRFModelDal();
            //grabbing the form info
            DataTable dtTerm = dal.GetESCRFTerminationInfoByID(globalESCRF.Termination.ID);
            globalESCRF.Termination = TerminationModel.ConvertDataTableToTermination(dtTerm);
            //if the form has a tasklist associated with it then retrieve the tasklist and the tasklistitems 
            if (globalESCRF.Termination.TaskListID != 0)
            {
                DataTable dtTaskList = dal.GetTaskListInfoByID(globalESCRF.Termination.TaskListID);
                globalESCRF.TaskList = TaskListModel.ConvertDataTableToTaskList(dtTaskList);
                if (globalESCRF.TaskList.CreatedDate != DateTime.MinValue)
                {
                    DataTable dtTaskListItems = dal.GetTaskListItemInfoByTaskListID(globalESCRF.TaskList.ID);
                    globalESCRF.TaskListItemHolder = TaskListItemModel.GetTaskListItemList(dtTaskListItems);
                }
            }
            Session["ESCRF"] = globalESCRF;
            return View("TerminationView", globalESCRF);
        }

        public ActionResult TerminationEditView(int id)
        {
            //grabbing context
            if (Session["ESCRF"] != null)
            {
                globalESCRF = (ESCRFViewModel)Session["ESCRF"];
            }
            else
            {
                globalESCRF = new ESCRFViewModel();
            }
            //used for cancel button
            globalESCRF.ChangeType = globalESCRF.Termination.ChangeType;
            //setting the ui
            globalESCRF.ESCRFUI.MinimalUI = true;
            dal = new ESCRFModelDal();
            //grabbing the form data
            DataTable dtTerm = dal.GetESCRFTerminationInfoByID(id);
            globalESCRF.Termination = TerminationModel.ConvertDataTableToTermination(dtTerm);
            Session["ESCRF"] = globalESCRF;
            return View("~/Views/Forms/ESCRF/TerminationEditView.cshtml", globalESCRF);
        }

        public ActionResult UpdateTermination(FormCollection collection)
        {
            //grabbing the context
            if (Session["ESCRF"] != null)
            {
                globalESCRF = (ESCRFViewModel)Session["ESCRF"];
            }
            else
            {
                globalESCRF = new ESCRFViewModel();
            }
            //need to do this because we are grabbing the id from the collection
            int ID = Int32.Parse(collection[12]);
            dal = new ESCRFModelDal();
            //retreiving the formdata
            DataTable dtTerm = dal.GetESCRFTerminationInfoByID(ID);
            globalESCRF.Termination = TerminationModel.ConvertDataTableToTermination(dtTerm);
            //updating the form data
            dal.UpdateTerminationDates(collection);
            //retrieving up to date form data
            dtTerm = dal.GetESCRFTerminationInfoByID(globalESCRF.Termination.ID);
            globalESCRF.Termination = TerminationModel.ConvertDataTableToTermination(dtTerm);
            Session["ESCRF"] = globalESCRF;
            return RedirectToAction("TerminationView", new { id = globalESCRF.Termination.ID });
        }

        public ActionResult TerminationPrintView(int id, int taskListId)
        {
            //grabbing the form data
            if (Session["ESCRF"] != null)
            {
                globalESCRF = (ESCRFViewModel)Session["ESCRF"];
            }
            else
            {
                globalESCRF = new ESCRFViewModel();
            }
            //setting the ui
            globalESCRF.ESCRFUI.MinimalUI = true;
            dal = new ESCRFModelDal();
            ViewBag.ID = taskListId;
            //retrieving the form data
            DataTable dtTerm = dal.GetESCRFTerminationInfoByID(id);
            globalESCRF.Termination = TerminationModel.ConvertDataTableToTermination(dtTerm);
            Session["ESCRF"] = globalESCRF;
            return View("~/Views/Forms/ESCRF/TerminationPrintView.cshtml", globalESCRF);
        }
        #endregion

        #region Change
        public ActionResult Change()
        {// grabbing context
            if (Session["ESCRF"] == null)
            {
                globalESCRF = new ESCRFViewModel();
            }
            else
            {
                globalESCRF = (ESCRFViewModel)Session["ESCRF"];
            }

            // Determine which urls to use for the AD functions
            // These are determined by the Environment app setting in the web.config
            if (ConfigurationManager.AppSettings["Environment"].ToUpper() == "DEV")
            {
                ViewBag.Validation = "/api/Validation/?query=";
                ViewBag.Verify = "/api/verify/?query=";
                ViewBag.Retrieve = "/api/retrieve/?query=";
            }
            else if (ConfigurationManager.AppSettings["Environment"].ToUpper() == "TEST")
            {
                ViewBag.Validation = "https://itdwsinett2.netstaging.nd.gov/WSI/onlineforms/api/Validation/?query=";
                ViewBag.Verify = "https://itdwsinett2.netstaging.nd.gov/WSI/onlineforms/api/verify/?query=";
                ViewBag.Retrieve = "https://itdwsinett2.netstaging.nd.gov/WSI/onlineforms/api/retrieve/?query=";
            }
            else if (ConfigurationManager.AppSettings["Environment"].ToUpper() == "PROD")
            {
                ViewBag.Validation = "https://webforms.wsi.nd.gov/api/Validation/?query=";
                ViewBag.Verify = "https://webforms.wsi.nd.gov/api/verify/?query=";
                ViewBag.Retrieve = "https://webforms.wsi.nd.gov/api/retrieve/?query=";
            }

            //getting the title strings from the db
            ViewBag.Titles = globalESCRF.titleStrings;
            //setting the ui
            globalESCRF.ESCRFUI.MinimalUI = true;
            globalESCRF.Change = new ChangeModel();
            //grabbing users info from ad
            PrincipalContext context = new PrincipalContext(ContextType.Domain);
            UserPrincipal user = UserPrincipal.FindByIdentity(context, User.Identity.Name);
            ViewBag.Name = user.Name;
            Session["ESCRF"] = globalESCRF;
            return View("~/Views/Forms/ESCRF/Change.cshtml", globalESCRF);
        }
        //for submitting the change form
        public ActionResult SubmitChange(FormCollection collection)
        {
            if (Session["ESCRF"] == null)
            {
                globalESCRF = new ESCRFViewModel();
            }
            else
            {
                globalESCRF = (ESCRFViewModel)Session["ESCRF"];
            }
            int ESCRFChangeID = dal.InsertESCRFChangeGetID(collection);
            dal.InsertChangelistInfoChange(collection, ESCRFChangeID);
            DataTable dtChange = dal.GetESCRFChangeInfoByID(ESCRFChangeID);
            globalESCRF.Change = ChangeModel.ConvertDataTableToChange(dtChange);
            Session["ESCRF"] = globalESCRF;
            return RedirectToAction("ChangeView", new { id = globalESCRF.Change.ID });
        }
        //
        public ActionResult ChangeView(int id)
        {
            //grabbing the context
            if (Session["ESCRF"] != null)
            {
                globalESCRF = (ESCRFViewModel)Session["ESCRF"];
            }
            else
            {
                globalESCRF = new ESCRFViewModel();
            }
            globalESCRF.ChangeType = "Change in WSI";
            //This Varible will controll whether the create list button is visible
            ViewBag.CreateListVisible = true;
            //setting the ui
            globalESCRF.ESCRFUI.MinimalUI = true;
            globalESCRF.TaskListItemHolder = new List<TaskListItemModel>();
            dal = new ESCRFModelDal();
            //retreiving the form data
            DataTable dtChange = dal.GetESCRFChangeInfoByID(id);
            globalESCRF.Change = ChangeModel.ConvertDataTableToChange(dtChange);
            //if the form data is associated with a list then retrieve the list from db and tasklistitems if they exist
            if (globalESCRF.Change.TaskListID != 0)
            {
                DataTable dtTaskList = dal.GetTaskListInfoByID(globalESCRF.Change.TaskListID);
                globalESCRF.TaskList = TaskListModel.ConvertDataTableToTaskList(dtTaskList);
                if (globalESCRF.TaskList.IsDeployed == true)
                {
                    ViewBag.CreateListVisible = false;
                }
                if (globalESCRF.TaskList.CreatedDate != DateTime.MinValue)
                {
                    DataTable dtTaskListItems = dal.GetTaskListItemInfoByTaskListID(globalESCRF.TaskList.ID);
                    globalESCRF.TaskListItemHolder = TaskListItemModel.GetTaskListItemList(dtTaskListItems);
                }
            }
            globalESCRF.CreateHolder = globalESCRF.Change;
            Session["ESCRF"] = globalESCRF;
            return View("~/Views/Forms/ESCRF/ChangeView.cshtml", globalESCRF);
        }
        //this changes from the ChangeView because it grabs the change from the viewmodel
        public ActionResult GetChangeViewByContext()
        {
            //grabbing the context 
            if (Session["ESCRF"] != null)
            {
                globalESCRF = (ESCRFViewModel)Session["ESCRF"];
            }
            else
            {
                globalESCRF = new ESCRFViewModel();
            }
            //getting the title strings from the db
            ViewBag.Titles = globalESCRF.titleStrings;
            //setting the ui
            globalESCRF.ESCRFUI.MinimalUI = true;
            dal = new ESCRFModelDal();
            //retriving form data
            DataTable dtChange = dal.GetESCRFChangeInfoByID(globalESCRF.Change.ID);
            globalESCRF.Change = ChangeModel.ConvertDataTableToChange(dtChange);
            //if the form data is associated with a list then retrieve the list from db and tasklistitems if they exist
            if (globalESCRF.Change.TaskListID != 0)
            {
                DataTable dtTaskList = dal.GetTaskListInfoByID(globalESCRF.Change.TaskListID);
                globalESCRF.TaskList = TaskListModel.ConvertDataTableToTaskList(dtTaskList);
                if (globalESCRF.TaskList.CreatedDate != DateTime.MinValue)
                {
                    DataTable dtTaskListItems = dal.GetTaskListItemInfoByTaskListID(globalESCRF.TaskList.ID);
                    globalESCRF.TaskListItemHolder = TaskListItemModel.GetTaskListItemList(dtTaskListItems);
                }
            }
            Session["ESCRF"] = globalESCRF;
            return View("ChangeView", globalESCRF);
        }

        public ActionResult ChangeEditView(int id)
        {
            //grabbing the context
            if (Session["ESCRF"] != null)
            {
                globalESCRF = (ESCRFViewModel)Session["ESCRF"];
            }
            else
            {
                globalESCRF = new ESCRFViewModel();
            }
            //this varible is used fro the cancel button
            globalESCRF.ChangeType = globalESCRF.Change.ChangeType;
            //setting the UI
            globalESCRF.ESCRFUI.MinimalUI = true;
            //retrieving the form data
            dal = new ESCRFModelDal();
            DataTable dtChange = dal.GetESCRFChangeInfoByID(id);
            globalESCRF.Change = ChangeModel.ConvertDataTableToChange(dtChange);
            Session["ESCRF"] = globalESCRF;
            return View("~/Views/Forms/ESCRF/ChangeEditView.cshtml", globalESCRF);
        }

        public ActionResult UpdateChange(FormCollection collection)
        {
            //grabbing the context
            if (Session["ESCRF"] != null)
            {
                globalESCRF = (ESCRFViewModel)Session["ESCRF"];
            }
            else
            {
                globalESCRF = new ESCRFViewModel();
            }

            dal = new ESCRFModelDal();
            //because we are using form data we need to pull the id out 
            int ID = Int32.Parse(collection[14]);
            dal = new ESCRFModelDal();
            //grabbing the form data from the db
            DataTable dtChange = dal.GetESCRFChangeInfoByID(ID);
            globalESCRF.Change = ChangeModel.ConvertDataTableToChange(dtChange);
            //updating the values on the record
            dal.UpdateChangeDates(collection);
            //updating the view model to get the up to date data
            dtChange = dal.GetESCRFChangeInfoByID(ID);
            globalESCRF.Change = ChangeModel.ConvertDataTableToChange(dtChange);
            Session["ESCRF"] = globalESCRF;
            return RedirectToAction("ChangeView", new { id = globalESCRF.Change.ID });

        }

        public ActionResult ChangePrintView(int id, int taskListId)
        {
            //grabbing the context
            if (Session["ESCRF"] != null)
            {
                globalESCRF = (ESCRFViewModel)Session["ESCRF"];
            }
            else
            {
                globalESCRF = new ESCRFViewModel();
            }
            //setting the ui
            globalESCRF.ESCRFUI.MinimalUI = true;
            //retrieving the form data 
            dal = new ESCRFModelDal();
            DataTable dtChange = dal.GetESCRFChangeInfoByID(id);
            globalESCRF.Change = ChangeModel.ConvertDataTableToChange(dtChange);
            Session["ESCRF"] = globalESCRF;
            ViewBag.ID = taskListId;
            return View("~/Views/Forms/ESCRF/ChangePrintView.cshtml", globalESCRF);
        }
        #endregion

        #region Name
        public ActionResult Name()
        {
            // grabbing the title strings from the db
            ViewBag.Titles = globalESCRF.titleStrings;
            //setting the ui
            globalESCRF.ESCRFUI.MinimalUI = true;

            globalESCRF.Name = new NameModel();

            // Determine which urls to use for the AD functions
            // These are determined by the Environment app setting in the web.config
            if (ConfigurationManager.AppSettings["Environment"].ToUpper() == "DEV")
            {
                ViewBag.Validation = "/api/Validation/?query=";
                ViewBag.Verify = "/api/verify/?query=";
                ViewBag.Retrieve = "/api/retrieve/?query=";
            }
            else if (ConfigurationManager.AppSettings["Environment"].ToUpper() == "TEST")
            {
                ViewBag.Validation = "https://itdwsinett2.netstaging.nd.gov/WSI/onlineforms/api/Validation/?query=";
                ViewBag.Verify = "https://itdwsinett2.netstaging.nd.gov/WSI/onlineforms/api/verify/?query=";
                ViewBag.Retrieve = "https://itdwsinett2.netstaging.nd.gov/WSI/onlineforms/api/retrieve/?query=";
            }
            else if (ConfigurationManager.AppSettings["Environment"].ToUpper() == "PROD")
            {
                ViewBag.Validation = "https://webforms.wsi.nd.gov/api/Validation/?query=";
                ViewBag.Verify = "https://webforms.wsi.nd.gov/api/verify/?query=";
                ViewBag.Retrieve = "https://webforms.wsi.nd.gov/api/retrieve/?query=";
            }

            //grabbing info about the user from ad
            PrincipalContext context = new PrincipalContext(ContextType.Domain);
            UserPrincipal user = UserPrincipal.FindByIdentity(context, User.Identity.Name);
            ViewBag.Name = user.Name;
            Session["ESCRF"] = globalESCRF;
            return View("~/Views/Forms/ESCRF/Name.cshtml", globalESCRF);
        }
        //used to submit the name form 
        public ActionResult SubmitName(FormCollection collection)
        {
            if (Session["ESCRF"] != null)
            {
                globalESCRF = (ESCRFViewModel)Session["ESCRF"];
            }
            else
            {
                globalESCRF = new ESCRFViewModel();
            }
            int ESCRFNameID = dal.InsertESCRFNameGetID(collection);
            dal.InsertChangelistInfoName(collection, ESCRFNameID);
            DataTable dtName = dal.GetESCRFNameInfoByID(ESCRFNameID);
            globalESCRF.Name = NameModel.ConvertDataTableToName(dtName);
            Session["ESCRF"] = globalESCRF;
            return RedirectToAction("NameView", new { id = globalESCRF.Name.ID });
        }

        public ActionResult NameView(int id)
        {
            //grabbing the context
            if (Session["ESCRF"] != null)
            {
                globalESCRF = (ESCRFViewModel)Session["ESCRF"];
            }
            else
            {
                globalESCRF = new ESCRFViewModel();
            }
            globalESCRF.ChangeType = "Name Change";
            //This Varible will controll whether the create list button is visible
            ViewBag.CreateListVisible = true;
            //setting the ui
            globalESCRF.ESCRFUI.MinimalUI = true;
            globalESCRF.TaskListItemHolder = new List<TaskListItemModel>();
            dal = new ESCRFModelDal();
            //retrieving the form data
            DataTable dtName = dal.GetESCRFNameInfoByID(id);
            globalESCRF.Name = NameModel.ConvertDataTableToName(dtName);
            //if a tasklist is associated with the form then retrieve the task list and tasklistitems
            if (globalESCRF.Name.TaskListID != 0)
            {
                DataTable dtTaskList = dal.GetTaskListInfoByID(globalESCRF.Name.TaskListID);
                globalESCRF.TaskList = TaskListModel.ConvertDataTableToTaskList(dtTaskList);
                if (globalESCRF.TaskList.IsDeployed == true)
                {
                    ViewBag.CreateListVisible = false;
                }
                if (globalESCRF.TaskList.CreatedDate != DateTime.MinValue)
                {
                    DataTable dtTaskListItems = dal.GetTaskListItemInfoByTaskListID(globalESCRF.TaskList.ID);
                    globalESCRF.TaskListItemHolder = TaskListItemModel.GetTaskListItemList(dtTaskListItems);
                }
            }
            globalESCRF.CreateHolder = globalESCRF.Name;
            Session["ESCRF"] = globalESCRF;
            return View("~/Views/Forms/ESCRF/NameView.cshtml", globalESCRF);
        }

        public ActionResult GetNameViewByContext()
        {
            //grabbing context
            if (Session["ESCRF"] != null)
            {
                globalESCRF = (ESCRFViewModel)Session["ESCRF"];
            }
            else
            {
                globalESCRF = new ESCRFViewModel();
            }
            //setting th eui
            globalESCRF.ESCRFUI.MinimalUI = true;
            dal = new ESCRFModelDal();
            //retrieving the form data
            DataTable dtName = dal.GetESCRFNameInfoByID(globalESCRF.Name.ID);
            globalESCRF.Name = NameModel.ConvertDataTableToName(dtName);
            //if a tasklist is associated with the form then grab the tasklist and taqsklist items
            if (globalESCRF.Name.TaskListID != 0)
            {
                DataTable dtTaskList = dal.GetTaskListInfoByID(globalESCRF.Name.TaskListID);
                globalESCRF.TaskList = TaskListModel.ConvertDataTableToTaskList(dtTaskList);
                if (globalESCRF.TaskList.CreatedDate != DateTime.MinValue)
                {
                    DataTable dtTaskListItems = dal.GetTaskListItemInfoByTaskListID(globalESCRF.TaskList.ID);
                    globalESCRF.TaskListItemHolder = TaskListItemModel.GetTaskListItemList(dtTaskListItems);
                }
            }
            Session["ESCRF"] = globalESCRF;
            return View("NameView", globalESCRF);
        }

        public ActionResult NameEditView(int id)
        {
            // grabbing context
            if (Session["ESCRF"] != null)
            {
                globalESCRF = (ESCRFViewModel)Session["ESCRF"];
            }
            else
            {
                globalESCRF = new ESCRFViewModel();
            }
            //setting this for the cancel button
            globalESCRF.ChangeType = globalESCRF.Name.ChangeType;
            //setting the ui
            globalESCRF.ESCRFUI.MinimalUI = true;
            dal = new ESCRFModelDal();
            //retrieving the form data
            DataTable dtName = dal.GetESCRFNameInfoByID(id);
            globalESCRF.Name = NameModel.ConvertDataTableToName(dtName);
            Session["ESCRF"] = globalESCRF;
            return View("~/Views/Forms/ESCRF/NameEditView.cshtml", globalESCRF);
        }

        public ActionResult UpdateName(FormCollection collection)
        {
            //grabbing the form data
            if (Session["ESCRF"] != null)
            {
                globalESCRF = (ESCRFViewModel)Session["ESCRF"];
            }
            else
            {
                globalESCRF = new ESCRFViewModel();
            }
            //Since the parameter is of FormCollection this is how we get the id
            int ID = int.Parse(collection[13]);
            //retrieving form data
            DataTable dtName = dal.GetESCRFNameInfoByID(ID);
            globalESCRF.Name = NameModel.ConvertDataTableToName(dtName);
            dal = new ESCRFModelDal();
            //updating the record
            dal.UpdateNameDates(collection);
            //retrieving the up to date form info
            dtName = dal.GetESCRFNameInfoByID(ID);
            globalESCRF.Name = NameModel.ConvertDataTableToName(dtName);
            Session["ESCRF"] = globalESCRF;
            return RedirectToAction("NameView", new { id = globalESCRF.Name.ID });

        }

        public ActionResult NamePrintView(int id, int taskListId)
        {
            //getting the context
            if (Session["ESCRF"] != null)
            {
                globalESCRF = (ESCRFViewModel)Session["ESCRF"];
            }
            else
            {
                globalESCRF = new ESCRFViewModel();
            }
            //setting the ui
            globalESCRF.ESCRFUI.MinimalUI = true;
            //retrieving the form data
            dal = new ESCRFModelDal();
            DataTable dtName = dal.GetESCRFNameInfoByID(id);
            globalESCRF.Name = NameModel.ConvertDataTableToName(dtName);
            Session["ESCRF"] = globalESCRF;
            ViewBag.ID = taskListId;
            return View("~/Views/Forms/ESCRF/NamePrintView.cshtml", globalESCRF);
        }
        #endregion

        #region Tasklist
        public ActionResult CreateList()
        {
            //grabbing contex
            if (Session["ESCRF"] != null)
            {
                globalESCRF = (ESCRFViewModel)Session["ESCRF"];
            }
            else
            {
                globalESCRF = new ESCRFViewModel();
            }

            // Determine which urls to use for the AD functions
            // These are determined by the Environment app setting in the web.config
            if (ConfigurationManager.AppSettings["Environment"].ToUpper() == "DEV")
            {
                ViewBag.Validation = "/api/Validation/?query=";
                ViewBag.Verify = "/api/verify/?query=";
                ViewBag.Retrieve = "/api/retrieve/?query=";
            }
            else if (ConfigurationManager.AppSettings["Environment"].ToUpper() == "TEST")
            {
                ViewBag.Validation = "https://itdwsinett2.netstaging.nd.gov/WSI/onlineforms/api/Validation/?query=";
                ViewBag.Verify = "https://itdwsinett2.netstaging.nd.gov/WSI/onlineforms/api/verify/?query=";
                ViewBag.Retrieve = "https://itdwsinett2.netstaging.nd.gov/WSI/onlineforms/api/retrieve/?query=";
            }
            else if (ConfigurationManager.AppSettings["Environment"].ToUpper() == "PROD")
            {
                ViewBag.Validation = "https://webforms.wsi.nd.gov/api/Validation/?query=";
                ViewBag.Verify = "https://webforms.wsi.nd.gov/api/verify/?query=";
                ViewBag.Retrieve = "https://webforms.wsi.nd.gov/api/retrieve/?query=";
            }

            //setting the UI
            globalESCRF.ESCRFUI.MinimalUI = true;
            globalESCRF.ESCRFUI.ResultsOnPage = 50;
            //retrieving all the tasklistitems
            ESCRFModelDal dAL = new ESCRFModelDal();
            globalESCRF.TaskListItem = new TaskListItemModel();
            DataTable taskListItems = dal.GetTaskListItemInfoByTaskListID(globalESCRF.TaskList.ID);
            globalESCRF.TaskListItemHolder = TaskListItemModel.GetTaskListItemList(taskListItems);
            //Organizing the list
            List<TaskListItemModel> tempList = globalESCRF.TaskListItemHolder.OrderBy(p => p.Deputy).ToList();
            globalESCRF.TaskListItemHolder = tempList;
            string url = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath;
            Session["ESCRF"] = globalESCRF;
            return View("~/Views/Forms/ESCRF/CreateList.cshtml", globalESCRF);
        }

        public ActionResult CreateNewHireTaskList(int id)
        {
            //grabbing the contex
            if (Session["ESCRF"] != null)
            {
                globalESCRF = (ESCRFViewModel)Session["ESCRF"];
            }
            else
            {
                globalESCRF = new ESCRFViewModel();
            }
            //grabbing the form data
            dal = new ESCRFModelDal();
            DataTable dtNewHire = dal.GetESCRFNewHireInfoByID(id);
            globalESCRF.NewHire = NewHireModel.ConvertDataTableToNewHire(dtNewHire);
            globalESCRF.CreateHolder = globalESCRF.NewHire;
            //if no tasklist is associated with the task list then create a tasklist and associate it
            if (globalESCRF.NewHire.TaskListID == 0)
            {
                int taskListID = dal.InsertTaskListGetID(globalESCRF.NewHire.ChangeType);
                dal.UpdateNewHireTaskListId(id, taskListID);
                dal.UpdateChangelistTaskListId(id, taskListID);
                dtNewHire = dal.GetESCRFNewHireInfoByID(id);
                globalESCRF.NewHire = NewHireModel.ConvertDataTableToNewHire(dtNewHire);
            }
            //retrieve tasklist and the tasklist items
            DataTable dtTaskList = dal.GetTaskListInfoByID(globalESCRF.NewHire.TaskListID);
            globalESCRF.TaskList = TaskListModel.ConvertDataTableToTaskList(dtTaskList);
            DataTable dtTaskListItem = dal.GetTaskListItemInfoByTaskListID(globalESCRF.TaskList.ID);
            globalESCRF.TaskListItemHolder = TaskListItemModel.GetTaskListItemList(dtTaskListItem);
            //if we have no tasklistitems on the tasklist then we will need to grab the default items from the db and add them to the tasklist  
            if (globalESCRF.TaskListItemHolder.Count == 0)
            {
                DataTable dtDefaultTasks = dal.GetDefaultByChangeType(globalESCRF.NewHire.ChangeType);
                globalESCRF.DefaultTaskListList = DefaultTaskListItemModel.GetTaskListItemList(dtDefaultTasks);
                foreach (DefaultTaskListItemModel defaultTask in globalESCRF.DefaultTaskListList)
                {
                    RetrieveController retrieve = new RetrieveController();
                    VerifyController verify = new VerifyController();
                    //if deputy is supervisor then we need to put in the supervisor name
                    if (defaultTask.Deputy == "SUPERVISOR")
                    {
                        string supervisorString = retrieve.Get(globalESCRF.NewHire.CurrentSupervisor.ToString());
                        List<string> supervisorList = supervisorString.Split('+').ToList();
                        dal.InsertTaskListItemGetIDManualWithEmail(defaultTask.Task, globalESCRF.NewHire.CurrentSupervisor, DateTime.MinValue.ToShortDateString(), globalESCRF.NewHire.TaskListID, defaultTask.Dept, supervisorList[5]);
                    }
                    else if (verify.Get(defaultTask.Deputy))
                    {
                        string supervisorString = retrieve.Get(globalESCRF.NewHire.CurrentSupervisor.ToString());
                        List<string> supervisorList = supervisorString.Split('+').ToList();
                        dal.InsertTaskListItemGetIDManualWithEmail(defaultTask.Task, defaultTask.Deputy, DateTime.MinValue.ToShortDateString(), globalESCRF.NewHire.TaskListID, defaultTask.Dept, supervisorList[5]);
                    }
                    else
                    {
                        dal.InsertTaskListItemGetIDManual(defaultTask.Task, defaultTask.Deputy, DateTime.MinValue.ToShortDateString(), globalESCRF.NewHire.TaskListID, defaultTask.Dept);
                    }
                }
            }
            Session["ESCRF"] = globalESCRF;
            return RedirectToAction("CreateList", globalESCRF);
        }

        public ActionResult CreateTerminationTaskList(int id)
        {
            //grabbing the context
            if (Session["ESCRF"] != null)
            {
                globalESCRF = (ESCRFViewModel)Session["ESCRF"];
            }
            else
            {
                globalESCRF = new ESCRFViewModel();
            }
            //grabbing the form data
            dal = new ESCRFModelDal();
            DataTable dtTerm = dal.GetESCRFTerminationInfoByID(id);
            globalESCRF.Termination = TerminationModel.ConvertDataTableToTermination(dtTerm);
            globalESCRF.CreateHolder = globalESCRF.Termination;
            //if no tasklist is associated with the form data then create a tasklist and associate it with the form data
            if (globalESCRF.Termination.TaskListID == 0)
            {
                int taskListID = dal.InsertTaskListGetID(globalESCRF.Termination.ChangeType);
                dal.UpdateTerminationTaskListId(id, taskListID);
                dal.UpdateChangelistTaskListId(id, taskListID);
                dtTerm = dal.GetESCRFTerminationInfoByID(id);
                globalESCRF.Termination = TerminationModel.ConvertDataTableToTermination(dtTerm);
            }
            //grabbing tasklist and tasklistitems
            DataTable dtTaskList = dal.GetTaskListInfoByID(globalESCRF.Termination.TaskListID);
            globalESCRF.TaskList = TaskListModel.ConvertDataTableToTaskList(dtTaskList);
            DataTable dtTaskListItem = dal.GetTaskListItemInfoByTaskListID(globalESCRF.TaskList.ID);
            globalESCRF.TaskListItemHolder = TaskListItemModel.GetTaskListItemList(dtTaskListItem);
            //if we have no tasklistitems on the tasklist then we will need to grab the default items from the db and add them to the tasklist 
            if (globalESCRF.TaskListItemHolder.Count == 0)
            {
                DataTable dtDefaultTasks = dal.GetDefaultByChangeType(globalESCRF.Termination.ChangeType);
                globalESCRF.DefaultTaskListList = DefaultTaskListItemModel.GetTaskListItemList(dtDefaultTasks);
                foreach (DefaultTaskListItemModel defaultTask in globalESCRF.DefaultTaskListList)
                {
                    VerifyController verify = new VerifyController();
                    RetrieveController retrieve = new RetrieveController();
                    //if deputy is supervisor then we need to put in the supervisor name
                    if (defaultTask.Deputy == "SUPERVISOR")
                    {
                        string supervisorString = retrieve.Get(globalESCRF.Termination.CurrentSupervisor.ToString());
                        List<string> supervisorList = supervisorString.Split('+').ToList();
                        dal.InsertTaskListItemGetIDManualWithEmail(defaultTask.Task, globalESCRF.Termination.CurrentSupervisor, DateTime.MinValue.ToShortDateString(), globalESCRF.Termination.TaskListID, defaultTask.Dept, supervisorList[5]);
                    }
                    else if (verify.Get(defaultTask.Deputy))
                    {
                        string supervisorString = retrieve.Get(globalESCRF.Termination.CurrentSupervisor.ToString());
                        List<string> supervisorList = supervisorString.Split('+').ToList();
                        dal.InsertTaskListItemGetIDManualWithEmail(defaultTask.Task, defaultTask.Deputy, DateTime.MinValue.ToShortDateString(), globalESCRF.Termination.TaskListID, defaultTask.Dept, supervisorList[5]);
                    }
                    else
                    {
                        dal.InsertTaskListItemGetIDManual(defaultTask.Task, defaultTask.Deputy, DateTime.MinValue.ToShortDateString(), globalESCRF.Termination.TaskListID, defaultTask.Dept);
                        DataTable dtEmployees = dal.GetEmployeesByDeptAfilliation(defaultTask.Deputy);
                        globalESCRF.Roles = ESCRFRoleModel.GetRoleList(dtEmployees);
                    }
                }
            }
            Session["ESCRF"] = globalESCRF;
            return RedirectToAction("CreateList", globalESCRF);
        }

        public ActionResult CreateChangeTaskList(int id)
        {
            //grabbing the context
            if (Session["ESCRF"] != null)
            {
                globalESCRF = (ESCRFViewModel)Session["ESCRF"];
            }
            else
            {
                globalESCRF = new ESCRFViewModel();
            }
            //retreiving the form data
            dal = new ESCRFModelDal();
            DataTable dtChange = dal.GetESCRFChangeInfoByID(id);
            globalESCRF.Change = ChangeModel.ConvertDataTableToChange(dtChange);
            globalESCRF.CreateHolder = globalESCRF.Change;
            //if no task list is associated with the form data then create the tasklist and associate it with the form 
            if (globalESCRF.Change.TaskListID == 0)
            {
                int taskListID = dal.InsertTaskListGetID(globalESCRF.Change.ChangeType);
                dal.UpdateChangeTaskListId(id, taskListID);
                dal.UpdateChangelistTaskListId(id, taskListID);
                dtChange = dal.GetESCRFChangeInfoByID(id);
                globalESCRF.Change = ChangeModel.ConvertDataTableToChange(dtChange);
            }
            //retrieving the tasklist and the tasklistitems
            DataTable dtTaskList = dal.GetTaskListInfoByID(globalESCRF.Change.TaskListID);
            globalESCRF.TaskList = TaskListModel.ConvertDataTableToTaskList(dtTaskList);
            DataTable dtTaskListItem = dal.GetTaskListItemInfoByTaskListID(globalESCRF.TaskList.ID);
            globalESCRF.TaskListItemHolder = TaskListItemModel.GetTaskListItemList(dtTaskListItem);
            //if we have no tasklistitems on the tasklist then we will need to grab the default items from the db and add them to the tasklist 
            if (globalESCRF.TaskListItemHolder.Count == 0)
            {
                DataTable dtDefaultTasks = dal.GetDefaultByChangeType(globalESCRF.Change.ChangeType);
                globalESCRF.DefaultTaskListList = DefaultTaskListItemModel.GetTaskListItemList(dtDefaultTasks);
                foreach (DefaultTaskListItemModel defaultTask in globalESCRF.DefaultTaskListList)
                {
                    VerifyController verify = new VerifyController();
                    RetrieveController retrieve = new RetrieveController();
                    //if deputy is supervisor then we need to put in the supervisor name 
                    if (defaultTask.Deputy == "SUPERVISOR")
                    {
                        // If the new supervisor box is checked then the supervisor tasks should be assigned to the new supervisor
                        if (globalESCRF.Change.IsNewSupervisor == true)
                        {
                            string supervisorString = retrieve.Get(globalESCRF.Change.NewSupervisor.ToString());
                            List<string> supervisorList = supervisorString.Split('+').ToList();
                            dal.InsertTaskListItemGetIDManualWithEmail(defaultTask.Task, globalESCRF.Change.NewSupervisor, DateTime.MinValue.ToShortDateString(), globalESCRF.Change.TaskListID, defaultTask.Dept, supervisorList[5]);

                        }
                        // If the new supervisor box is not checked then the supervisor tasks should be assigned to the current supervisor
                        else
                        {
                            string supervisorString = retrieve.Get(globalESCRF.Change.CurrentSupervisor.ToString());
                            List<string> supervisorList = supervisorString.Split('+').ToList();
                            dal.InsertTaskListItemGetIDManualWithEmail(defaultTask.Task, globalESCRF.Change.CurrentSupervisor, DateTime.MinValue.ToShortDateString(), globalESCRF.Change.TaskListID, defaultTask.Dept, supervisorList[5]);

                        }
                    }
                    else if (verify.Get(defaultTask.Deputy))
                    {
                        string supervisorString = retrieve.Get(globalESCRF.Change.CurrentSupervisor.ToString());
                        List<string> supervisorList = supervisorString.Split('+').ToList();
                        dal.InsertTaskListItemGetIDManualWithEmail(defaultTask.Task, defaultTask.Deputy, DateTime.MinValue.ToShortDateString(), globalESCRF.Change.TaskListID, defaultTask.Dept, supervisorList[5]);
                    }
                    else
                    {
                        dal.InsertTaskListItemGetIDManual(defaultTask.Task, defaultTask.Deputy, DateTime.MinValue.ToShortDateString(), globalESCRF.Change.TaskListID, defaultTask.Dept);
                        DataTable dtEmployees = dal.GetEmployeesByDeptAfilliation(defaultTask.Deputy);
                        globalESCRF.Roles = ESCRFRoleModel.GetRoleList(dtEmployees);
                    }
                }
            }
            Session["ESCRF"] = globalESCRF;
            return RedirectToAction("CreateList", globalESCRF);
        }

        public ActionResult CreateNameTaskList(int id)
        {
            //Grabbing the context 
            if (Session["ESCRF"] != null)
            {
                globalESCRF = (ESCRFViewModel)Session["ESCRF"];
            }
            else
            {
                globalESCRF = new ESCRFViewModel();
            }
            //retrieving the form data from the db
            dal = new ESCRFModelDal();
            DataTable dtName = dal.GetESCRFNameInfoByID(id);
            globalESCRF.Name = NameModel.ConvertDataTableToName(dtName);
            globalESCRF.CreateHolder = globalESCRF.Name;
            //if the form data is not associated with a tasklist then create a tasklist and associate it with the form data
            if (globalESCRF.Name.TaskListID == 0)
            {

                int taskListID = dal.InsertTaskListGetID(globalESCRF.Name.ChangeType);
                dal.UpdateNameTaskListId(id, taskListID);
                dal.UpdateChangelistTaskListId(id, taskListID);
                //Getting the up to date form data from the db
                dtName = dal.GetESCRFNameInfoByID(id);
                globalESCRF.Name = NameModel.ConvertDataTableToName(dtName);
            }
            //Retrieving the tasklist and the tasklist items
            DataTable dtTaskList = dal.GetTaskListInfoByID(globalESCRF.Name.TaskListID);
            globalESCRF.TaskList = TaskListModel.ConvertDataTableToTaskList(dtTaskList);
            DataTable dtTaskListItem = dal.GetTaskListItemInfoByTaskListID(globalESCRF.TaskList.ID);
            globalESCRF.TaskListItemHolder = TaskListItemModel.GetTaskListItemList(dtTaskListItem);
            //if we have no tasklistitems on the tasklist then we will need to grab the default items from the db and add them to the tasklist 
            if (globalESCRF.TaskListItemHolder.Count == 0)
            {
                DataTable dtDefaultTasks = dal.GetDefaultByChangeType(globalESCRF.Name.ChangeType);
                globalESCRF.DefaultTaskListList = DefaultTaskListItemModel.GetTaskListItemList(dtDefaultTasks);
                foreach (DefaultTaskListItemModel defaultTask in globalESCRF.DefaultTaskListList)
                {
                    VerifyController verify = new VerifyController();
                    RetrieveController retrieve = new RetrieveController();
                    if (defaultTask.Deputy == "SUPERVISOR")
                    {

                        string supervisorString = retrieve.Get(globalESCRF.Name.CurrentSupervisor.ToString());
                        List<string> supervisorList = supervisorString.Split('+').ToList();
                        dal.InsertTaskListItemGetIDManualWithEmail(defaultTask.Task, globalESCRF.Name.CurrentSupervisor, DateTime.MinValue.ToShortDateString(), globalESCRF.Name.TaskListID, defaultTask.Dept, supervisorList[5]);
                    }
                    else if (verify.Get(defaultTask.Deputy))
                    {
                        string supervisorString = retrieve.Get(globalESCRF.Name.CurrentSupervisor.ToString());
                        List<string> supervisorList = supervisorString.Split('+').ToList();
                        dal.InsertTaskListItemGetIDManualWithEmail(defaultTask.Task, defaultTask.Deputy, DateTime.MinValue.ToShortDateString(), globalESCRF.Name.TaskListID, defaultTask.Dept, supervisorList[5]);
                    }
                    else
                    {
                        dal.InsertTaskListItemGetIDManual(defaultTask.Task, defaultTask.Deputy, DateTime.MinValue.ToShortDateString(), globalESCRF.Name.TaskListID, defaultTask.Dept);
                        DataTable dtEmployees = dal.GetEmployeesByDeptAfilliation(defaultTask.Deputy);
                        globalESCRF.Roles = ESCRFRoleModel.GetRoleList(dtEmployees);
                    }

                }
            }
            Session["ESCRF"] = globalESCRF;
            return RedirectToAction("CreateList", globalESCRF);
        }

        public ActionResult DeployTaskList(int taskListID)
        {
            if (Session["ESCRF"] != null)
            {
                globalESCRF = (ESCRFViewModel)Session["ESCRF"];
            }
            else
            {
                globalESCRF = new ESCRFViewModel();
            }
            ESCRFModelDal dal = new ESCRFModelDal();
            //Lets grab the task list
            DataTable dtTaskList = dal.GetTaskListInfoByID(taskListID);
            globalESCRF.TaskList = TaskListModel.ConvertDataTableToTaskList(dtTaskList);
            //Retreive all TaskListItems that have a matching TaskListId
            DataTable dtTaskListItems = dal.GetTaskListItemInfoByTaskListID(taskListID);
            globalESCRF.TaskListItemHolder = TaskListItemModel.GetTaskListItemList(dtTaskListItems);
            //Get all roles from the DB
            DataTable dtRoles = dal.GetDeptAfilliationInfo();
            List<ESCRFRoleModel> Roles = ESCRFRoleModel.GetRoleList(dtRoles);
            List<string> beenEmailed = new List<string>();
            string path = HttpContext.Request.Url.AbsoluteUri.ToString();
            List<string> host = new List<string>(
                path.Split(new string[] { "ESCRF" }, StringSplitOptions.None));
            string from = string.Empty;
            string subject = string.Empty;
            string body = string.Empty;
            //going through all the tasklistItems in the tasklist.
            var emailList = new List<string>();
            //This is going to be the email address list creater. It will create the list of email addresses. Then we will use that list to and send an email to each address
            foreach (TaskListItemModel taskListItem in globalESCRF.TaskListItemHolder)
            {
                string task = taskListItem.Task;
                string deputy = taskListItem.Deputy.Trim();
                string email = taskListItem.Dep_Email.Trim();
                //if there is an email in the dep email then use that
                if (email.Length > 0)
                {
                    // Add deputy email to list if it's not already in there (so emails don't duplicate)
                    if (!emailList.Contains(email))
                    {
                        emailList.Add(email);
                    }
                }
                //no email in the record
                if (email.Length == 0)
                {
                    VerifyController verify = new VerifyController();
                    RetrieveController retrieve = new RetrieveController();
                    //if the deputy is a person then we will get that info from ad
                    if (verify.Get(deputy))
                    {
                        string deputyInfo = retrieve.Get(deputy);
                        List<string> infoList = deputyInfo.Split('+').ToList();
                        if (!emailList.Contains(infoList[5]))
                        {
                            emailList.Add(infoList[5]);
                        }
                    }
                    //no email on the record and not a person so a group 
                    else
                    {
                        foreach (ESCRFRoleModel role in Roles)
                        {
                            if (deputy == role.DepartmentAffiliation)
                            {
                                if (!emailList.Contains(role.EmployeeEmail))
                                {
                                    emailList.Add(role.EmployeeEmail);
                                }
                            }
                        }
                    }
                }
            }
            string to = string.Empty;
            foreach (string email in emailList)
            {
                to = to + email + ";";
            }
            if (globalESCRF.TaskList.ChangeType == "New Hire")
            {
                //grab the form info
                DataTable dtNewHire = dal.GetESCRFNewHireInfoByTaskListID(taskListID);
                globalESCRF.NewHire = NewHireModel.ConvertDataTableToNewHire(dtNewHire);
                from = "wsinoreply@nd.gov";
                subject = "Employee Status Change Request Submitted: New Hire " + globalESCRF.NewHire.Name().ToString();
                body =
                   "An employee status change request has been submitted with a request type of New Hire. " + System.Environment.NewLine +
                   "<a href=\"" + host[0].ToString() + "ESCRF/ChecklistTasks/" + taskListID + "\"> Checklist Link " + "</a>" + System.Environment.NewLine +
                   "Instructions: " + "<a href=\"" + "file://nd.gov/wsi/wsi-shared/SHAREPOINT%20DOCUMENTS/Human%20Resources/Checklist%20Education.pdf" + "</a>" + System.Environment.NewLine +
                   "Submitted Information: " + System.Environment.NewLine +
                   "Employee is Existing WSI User: Yes " + System.Environment.NewLine +
                   "Employee’s Name: " + globalESCRF.NewHire.Name().ToString() + System.Environment.NewLine +
                   "Job Title: " + globalESCRF.NewHire.JobTitle.ToString() + System.Environment.NewLine +
                   "Effective Date: " + globalESCRF.NewHire.EffectiveDate.ToShortDateString() + System.Environment.NewLine +
                   "Current Supervisor: " + globalESCRF.NewHire.CurrentSupervisor.ToString() + System.Environment.NewLine +
                   "Employee Type: " + globalESCRF.NewHire.FLSAStatus.ToString() + System.Environment.NewLine +
                   "Transferring from State Agency: " + globalESCRF.NewHire.TransferringFromAgency.ToString() + System.Environment.NewLine +
                   "Comments: " + globalESCRF.NewHire.Comments.ToString() + System.Environment.NewLine;
                dal.SendEmail(to, from, subject, body);
            }
            if (globalESCRF.TaskList.ChangeType == "Termination")
            {
                string title = string.Empty;
                VerifyController verify = new VerifyController();
                if (verify.Get(globalESCRF.Termination.Name()))
                {
                    RetrieveController retrieve = new RetrieveController();
                    string termInfoString = retrieve.Get(globalESCRF.Termination.Name());
                    List<string> termInfoList = termInfoString.Split('+').ToList();
                    title = termInfoList[6];
                }
                //grab the form info
                DataTable dtTerm = dal.GetESCRFTerminationInfoByTaskListID(taskListID);
                globalESCRF.Termination = TerminationModel.ConvertDataTableToTermination(dtTerm);
                from = "wsinoreply@nd.gov";
                subject = "Employee Status Change Request Submitted: Termination " + globalESCRF.Termination.Name().ToString();
                body =
                    "An employee status change request has been submitted with a request type of Termination." + System.Environment.NewLine +
                    "<a href=\"" + host[0].ToString() + "ESCRF/ChecklistTasks/" + taskListID + "\"> Checklist Link " + "</a>" + System.Environment.NewLine +
                    "Instructions: " + "<a href=\"" + "file://nd.gov/wsi/wsi-shared/SHAREPOINT%20DOCUMENTS/Human%20Resources/Checklist%20Education.pdf" + "</a>" + System.Environment.NewLine +
                    "Submitted Information: " + System.Environment.NewLine +
                    "Employee is Existing WSI User: Yes " + System.Environment.NewLine +
                    "Employee's Title: " + title + System.Environment.NewLine +
                    "Employee’s Name: " + globalESCRF.Termination.Name().ToString() + System.Environment.NewLine +
                    "Phone Number: " + globalESCRF.Termination.PhoneNumber.ToString() + System.Environment.NewLine +
                    "Office Location: " + globalESCRF.Termination.OfficeLocation.ToString() + System.Environment.NewLine +
                    "Effective Date: " + globalESCRF.Termination.EffectiveDate.ToShortDateString() + System.Environment.NewLine +
                    "Current Supervisor: " + globalESCRF.Termination.CurrentSupervisor.ToString() + System.Environment.NewLine +
                    "Last Day Worked: " + globalESCRF.Termination.LastDateWorked.ToShortDateString() + System.Environment.NewLine +
                    "Transferring To State Agency: " + globalESCRF.Termination.TransferringToAgency.ToString() + System.Environment.NewLine +
                    "Comments: " + globalESCRF.Termination.Comments.ToString();
                dal.SendEmail(to, from, subject, body);
            }
            if (globalESCRF.TaskList.ChangeType == "Change in WSI")
            {
                //grab the form info
                DataTable dtChange = dal.GetESCRFChangeInfoByTaskListID(taskListID);
                globalESCRF.Change = ChangeModel.ConvertDataTableToChange(dtChange);
                from = "wsinoreply@nd.gov";
                subject = "Employee Status Change Request Submitted: Change within WSI " + globalESCRF.Change.Name().ToString();
                body =
                    "An employee status change request has been submitted with a request type of Change within WSI." + System.Environment.NewLine +
                    "<a href=\"" + host[0].ToString() + "ESCRF/ChecklistTasks/" + taskListID + "\"> Checklist Link " + "</a>" + System.Environment.NewLine +
                    "Instructions: " + "<a href=\"" + "file://nd.gov/wsi/wsi-shared/SHAREPOINT%20DOCUMENTS/Human%20Resources/Checklist%20Education.pdf" + "</a>" + System.Environment.NewLine +
                    "Submitted Information: " + System.Environment.NewLine +
                    "Employee is Existing WSI User: Yes " + System.Environment.NewLine +
                    "Employee’s Name: " + globalESCRF.Change.Name().ToString() + System.Environment.NewLine +
                    "Job Title: " + globalESCRF.Change.PositionName.ToString() + System.Environment.NewLine +
                    "Office Location: " + globalESCRF.Change.OfficeLocation.ToString() + System.Environment.NewLine +
                    "Effective Date: " + globalESCRF.Change.EffectiveDate.ToShortDateString() + System.Environment.NewLine +
                    "Current Supervisor: " + globalESCRF.Change.CurrentSupervisor.ToString() + System.Environment.NewLine +
                    "New Supervisor: " + globalESCRF.Change.NewSupervisor.ToString() + System.Environment.NewLine +
                    "Comments: " + globalESCRF.Change.Comments.ToString() + System.Environment.NewLine;
                dal.SendEmail(to, from, subject, body);
            }
            if (globalESCRF.TaskList.ChangeType == "Name Change")
            {
                //This is so the Employee's name string can change depending if they have a middle initial or not.
                string employeeNameString = string.Empty;
                if (string.IsNullOrEmpty(globalESCRF.Name.MiddleInitial))
                {
                    employeeNameString = "Employee’s New Name: " + globalESCRF.Name.LastName.ToString() + ", " + globalESCRF.Name.FirstName.ToString() + System.Environment.NewLine;
                }
                else
                {
                    employeeNameString = "Employee’s New Name: " + globalESCRF.Name.LastName.ToString() + ", " + globalESCRF.Name.FirstName.ToString() + " " + globalESCRF.Name.MiddleInitial + "." + System.Environment.NewLine;
                }
                //grab the form info
                DataTable dtName = dal.GetESCRFNameInfoByTaskListID(taskListID);
                globalESCRF.Name = NameModel.ConvertDataTableToName(dtName);
                from = "wsinoreply@nd.gov";
                subject = "Employee Status Change Request Submitted: Name Change " + globalESCRF.Name.Name().ToString();
                body =
                    "An employee status change request has been submitted with a request type of Name Change." + System.Environment.NewLine +
                    "<a href=\"" + host[0].ToString() + "ESCRF/ChecklistTasks/" + taskListID + "\"> Checklist Link " + "</a>" + System.Environment.NewLine +
                    "Instructions: " + "<a href=\"" + "file://nd.gov/wsi/wsi-shared/SHAREPOINT%20DOCUMENTS/Human%20Resources/Checklist%20Education.pdf" + "</a>" + System.Environment.NewLine +
                    "Submitted Information: " + System.Environment.NewLine +
                    "Employee is Existing WSI User: Yes " + System.Environment.NewLine +
                    "Employee’s Name: " + globalESCRF.Name.Name().ToString() + System.Environment.NewLine +
                    employeeNameString +
                    "Phone Number: " + globalESCRF.Name.PhoneNumber.ToString() + System.Environment.NewLine +
                    "Office Location: " + globalESCRF.Name.OfficeLocation.ToString() + System.Environment.NewLine +
                    "Effective Date: " + globalESCRF.Name.EffectiveDate.ToShortDateString() + System.Environment.NewLine +
                    "Current Supervisor: " + globalESCRF.Name.CurrentSupervisor.ToString() + System.Environment.NewLine +
                    "Comments: " + globalESCRF.Name.Comments.ToString() + System.Environment.NewLine;
                //this to is for if the deputy is a real person and not a group
                dal.SendEmail(to, from, subject, body);
            }

            //Update the created date on the record
            dal.UpdateTaskListCreatedDate(taskListID);
            dal.UpdateTaskListDeployed(taskListID);
            dtTaskList = dal.GetTaskListInfoByID(taskListID);
            globalESCRF.TaskList = TaskListModel.ConvertDataTableToTaskList(dtTaskList);
            Debug.WriteLine(globalESCRF.TaskList.ID);
            Session["ESCRF"] = globalESCRF;
            return RedirectToAction("ChecklistTasks", new { id = globalESCRF.TaskList.ID });
        }
        //Gather Tasks from a specific TaskList and checks to see if they all have been completed. If they all have been completed then it will update the tasklist's finished date. 
        public void FinishList(int taskListID, string emailUrl)
        {
            ESCRFModelDal dal = new ESCRFModelDal();
            DataTable dtTaskList = dal.GetTaskListInfoByID(taskListID);
            globalESCRF.TaskList = TaskListModel.ConvertDataTableToTaskList(dtTaskList);
            DataTable dtTaskListItemList = dal.GetTaskListItemInfoByTaskListID(taskListID);
            globalESCRF.TaskListItemHolder = TaskListItemModel.GetTaskListItemList(dtTaskListItemList);
            DataTable dtChangeListInfo = dal.GetChangelistInfoByChangelistID(taskListID);
            globalESCRF.ChangeListInfo = ChangelistInfoModel.ConvertDataTableToChangelistInfoItem(dtChangeListInfo);
            bool isComplete = true;
            foreach (TaskListItemModel taskListItem in globalESCRF.TaskListItemHolder)
            {
                if (taskListItem.SignedOn == DateTime.MinValue)
                {
                    isComplete = false;
                }
            }

            // if the form is complete send a message to hr to let them know
            if (isComplete)
            {
                string employee = string.Empty;
                string changetype = globalESCRF.TaskList.ChangeType.ToString();
                if (changetype == "New Hire")
                {
                    employee = globalESCRF.NewHire.Name();
                }
                if (changetype == "Termination")
                {
                    employee = globalESCRF.Termination.Name();
                }
                if (changetype == "Change in WSI")
                {
                    employee = globalESCRF.Change.Name();
                }
                if (changetype == "Name Change")
                {
                    employee = globalESCRF.Name.Name();
                }

                dal.UpdateTaskListFinishedDate(globalESCRF.TaskList.ID);

                string environment = ConfigurationManager.AppSettings["Environment"];
                //Create email to superusers notifying of form submission
                //Create email list
                var toEmailList = new List<string>();

                // Find users with the job title Multimedia Specialist
                DirectoryEntry enTry = new DirectoryEntry("LDAP://OU=WSI,DC=nd,DC=gov");
                DirectorySearcher mySearcher = new DirectorySearcher();
                SearchResultCollection results;
                mySearcher = new DirectorySearcher(enTry);
                mySearcher.Filter = "(&(company=Workforce Safety & Insurance)(|(title=" + "Human Resource Officer" + ")(title=" + "HR Director" + ")(title=" + "Learning and Development Coordinator" + ")))";
                results = mySearcher.FindAll();

                // Add Technical Services Lead Specialist email to list
                if (results.Count > 0)
                {
                    foreach (System.DirectoryServices.SearchResult searchResult in results)
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

                string toAddress = string.Join("; ", toEmailList.ToArray());
                string from = "wsinoreply@nd.gov";
                string subject = "Employee Status Change Checklist is Complete for " + globalESCRF.ChangeListInfo.EmployeeName;
                string body = "An Employee Status Change Checklist is Complete for " + globalESCRF.ChangeListInfo.EmployeeName
            + "<a href=\"" + emailUrl + "ESCRF/ChecklistTasks/" + taskListID + "\">Technology Requirements Checklist Link " + "</a>";
                dal = new ESCRFModelDal();
                dal.SendEmail(toAddress, from, subject, body);
            }

        }
        #endregion

        #region Task

        public ActionResult TaskView(int ID)
        {
            //grabbing the context
            if (Session["ESCRF"] != null)
            {
                globalESCRF = (ESCRFViewModel)Session["ESCRF"];
            }
            else
            {
                globalESCRF = new ESCRFViewModel();
            }
            //setting the ui
            globalESCRF.ESCRFUI.MinimalUI = true;
            //retrieving the tasklistitem and form from db
            DataTable dtTaskListItem = dal.GetTaskListItemInfoByID(ID);
            globalESCRF.TaskListItem = TaskListItemModel.ConvertDataTableToTaskListItem(dtTaskListItem);
            DataTable dtTaskList = dal.GetTaskListInfoByID(globalESCRF.TaskListItem.TaskListID);
            globalESCRF.TaskList = TaskListModel.ConvertDataTableToTaskList(dtTaskList);
            //retrieving the form associated with the tasklistitem based on form type
            if (globalESCRF.TaskList.ChangeType == "New Hire")
            {
                DataTable dtNewHire = dal.GetESCRFNewHireInfoByTaskListID(globalESCRF.TaskList.ID);
                globalESCRF.NewHire = NewHireModel.ConvertDataTableToNewHire(dtNewHire);
                globalESCRF.CreateHolder = globalESCRF.NewHire;
            }
            if (globalESCRF.TaskList.ChangeType == "Termination")
            {
                DataTable dtTerm = dal.GetESCRFTerminationInfoByTaskListID(globalESCRF.TaskList.ID);
                globalESCRF.Termination = TerminationModel.ConvertDataTableToTermination(dtTerm);
                globalESCRF.CreateHolder = globalESCRF.Termination;
            }
            if (globalESCRF.TaskList.ChangeType == "Change in WSI")
            {
                DataTable dtChange = dal.GetESCRFChangeInfoByTaskListID(globalESCRF.TaskList.ID);
                globalESCRF.Change = ChangeModel.ConvertDataTableToChange(dtChange);
                globalESCRF.CreateHolder = globalESCRF.Change;
            }
            if (globalESCRF.TaskList.ChangeType == "Name Change")
            {
                DataTable dtName = dal.GetESCRFNameInfoByTaskListID(globalESCRF.TaskList.ID);
                globalESCRF.Name = NameModel.ConvertDataTableToName(dtName);
                globalESCRF.CreateHolder = globalESCRF.Name;
            }
            Session["ESCRF"] = globalESCRF;
            return View("~/Views/Forms/ESCRF/TaskView.cshtml", globalESCRF);
        }

        public ActionResult TaskEditView(int ID)
        {
            //grabbing the context
            if (Session["ESCRF"] != null)
            {
                globalESCRF = (ESCRFViewModel)Session["ESCRF"];
            }
            else
            {
                globalESCRF = new ESCRFViewModel();
            }

            // Determine which urls to use for the AD functions
            // These are determined by the Environment app setting in the web.config
            if (ConfigurationManager.AppSettings["Environment"].ToUpper() == "DEV")
            {
                ViewBag.Validation = "/api/Validation/?query=";
                ViewBag.Verify = "/api/verify/?query=";
                ViewBag.Retrieve = "/api/retrieve/?query=";
            }
            else if (ConfigurationManager.AppSettings["Environment"].ToUpper() == "TEST")
            {
                ViewBag.Validation = "https://itdwsinett2.netstaging.nd.gov/WSI/onlineforms/api/Validation/?query=";
                ViewBag.Verify = "https://itdwsinett2.netstaging.nd.gov/WSI/onlineforms/api/verify/?query=";
                ViewBag.Retrieve = "https://itdwsinett2.netstaging.nd.gov/WSI/onlineforms/api/retrieve/?query=";
            }
            else if (ConfigurationManager.AppSettings["Environment"].ToUpper() == "PROD")
            {
                ViewBag.Validation = "https://webforms.wsi.nd.gov/api/Validation/?query=";
                ViewBag.Verify = "https://webforms.wsi.nd.gov/api/verify/?query=";
                ViewBag.Retrieve = "https://webforms.wsi.nd.gov/api/retrieve/?query=";
            }

            //setting the ui
            globalESCRF.ESCRFUI.MinimalUI = true;
            //retrieving the tasklist and tasklistitem
            DataTable dtTaskListItem = dal.GetTaskListItemInfoByID(ID);
            globalESCRF.TaskListItem = TaskListItemModel.ConvertDataTableToTaskListItem(dtTaskListItem);
            DataTable dtTaskList = dal.GetTaskListInfoByID(int.Parse(dtTaskListItem.Rows[0]["TASK_LIST_ID"].ToString()));
            globalESCRF.TaskList = TaskListModel.ConvertDataTableToTaskList(dtTaskList);
            DataTable dtChangeType = dal.GetChangelistInfoByChangelistID(int.Parse(dtTaskListItem.Rows[0]["TASK_LIST_ID"].ToString()));
            globalESCRF.ChangeListInfo = ChangelistInfoModel.ConvertDataTableToChangelistInfoItem(dtChangeType);
            //retrieving the form associated with the tasklistitem based on form type
            if (globalESCRF.TaskList.ChangeType == "New Hire")
            {
                DataTable dtNewHire = dal.GetESCRFNewHireInfoByTaskListID(globalESCRF.TaskList.ID);
                globalESCRF.NewHire = NewHireModel.ConvertDataTableToNewHire(dtNewHire);
                globalESCRF.CreateHolder = globalESCRF.NewHire;
            }
            if (globalESCRF.TaskList.ChangeType == "Termination")
            {
                DataTable dtTerm = dal.GetESCRFTerminationInfoByTaskListID(globalESCRF.TaskList.ID);
                globalESCRF.Termination = TerminationModel.ConvertDataTableToTermination(dtTerm);
                globalESCRF.CreateHolder = globalESCRF.Termination;
            }
            if (globalESCRF.TaskList.ChangeType == "Change In WSI")
            {
                DataTable dtChange = dal.GetESCRFChangeInfoByTaskListID(globalESCRF.TaskList.ID);
                globalESCRF.Change = ChangeModel.ConvertDataTableToChange(dtChange);
                globalESCRF.CreateHolder = globalESCRF.Change;
            }
            if (globalESCRF.TaskList.ChangeType == "Name Change")
            {
                DataTable dtName = dal.GetESCRFNameInfoByTaskListID(globalESCRF.TaskList.ID);
                globalESCRF.Name = NameModel.ConvertDataTableToName(dtName);
                globalESCRF.CreateHolder = globalESCRF.Name;
            }
            Session["ESCRF"] = globalESCRF;
            return View("~/Views/Forms/ESCRF/TaskEditView.cshtml", globalESCRF);
        }
        //used in taskedit view for cancel
        public ActionResult ListRouter()
        {
            ESCRFModelDal dal = new ESCRFModelDal();

            //grabbing the context 
            if (Session["ESCRF"] != null)
            {
                globalESCRF = (ESCRFViewModel)Session["ESCRF"];
            }
            else
            {
                globalESCRF = new ESCRFViewModel();
            }

            //routes back to Create*Tasklist based on the form type
            if (globalESCRF.ChangeType == "New Hire")
            {
                if (globalESCRF.NewHire.TaskListID != 0)
                {
                    DataTable dtTaskList = dal.GetTaskListInfoByID(globalESCRF.NewHire.TaskListID);
                    globalESCRF.TaskList = TaskListModel.ConvertDataTableToTaskList(dtTaskList);
                    if (globalESCRF.TaskList.IsDeployed)
                    {
                        return ChecklistTasks(globalESCRF.TaskList.ID);
                    }
                }
                int id = globalESCRF.NewHire.ID;
                return CreateNewHireTaskList(id);
            }
            if (globalESCRF.ChangeType == "Termination")
            {
                if (globalESCRF.Termination.TaskListID != 0)
                {
                    DataTable dtTaskList = dal.GetTaskListInfoByID(globalESCRF.Termination.TaskListID);
                    globalESCRF.TaskList = TaskListModel.ConvertDataTableToTaskList(dtTaskList);
                    if (globalESCRF.TaskList.IsDeployed)
                    {
                        return ChecklistTasks(globalESCRF.TaskList.ID);
                    }
                }
                int id = globalESCRF.Termination.ID;
                return CreateTerminationTaskList(id);
            }
            if (globalESCRF.ChangeType == "Change in WSI")
            {
                if (globalESCRF.Change.TaskListID != 0)
                {
                    DataTable dtTaskList = dal.GetTaskListInfoByID(globalESCRF.Change.TaskListID);
                    globalESCRF.TaskList = TaskListModel.ConvertDataTableToTaskList(dtTaskList);
                    if (globalESCRF.TaskList.IsDeployed)
                    {
                        return ChecklistTasks(globalESCRF.TaskList.ID);
                    }
                }
                int id = globalESCRF.Change.ID;
                return CreateChangeTaskList(id);
            }
            if (globalESCRF.ChangeType == "Name Change")
            {
                if (globalESCRF.Name.TaskListID != 0)
                {
                    DataTable dtTaskList = dal.GetTaskListInfoByID(globalESCRF.Name.TaskListID);
                    globalESCRF.TaskList = TaskListModel.ConvertDataTableToTaskList(dtTaskList);
                    if (globalESCRF.TaskList.IsDeployed)
                    {
                        return ChecklistTasks(globalESCRF.TaskList.ID);
                    }
                }
                int id = globalESCRF.Name.ID;
                return CreateNameTaskList(id);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        public ActionResult CompleteTask(int id)
        {
            //grabbing the context
            if (Session["ESCRF"] != null)
            {
                globalESCRF = (ESCRFViewModel)Session["ESCRF"];
            }
            else
            {
                globalESCRF = new ESCRFViewModel();
            }
            //instantiating varibles
            string hrOfficerEmail = getEmailByTitle("Human Resource Officer");
            string hrDirectorEmail = getEmailByTitle("HR Director");
            string to = string.Empty;
            string from = string.Empty;
            string subject = string.Empty;
            string body = string.Empty;
            //grabbing information from AD about the user
            System.Web.HttpContext.Current.Request.LogonUserIdentity.Impersonate();
            PrincipalContext context = new PrincipalContext(ContextType.Domain);
            UserPrincipal u = new UserPrincipal(context);
            UserPrincipal user = UserPrincipal.FindByIdentity(context, User.Identity.Name);
            //updating the tasklistitem 
            dal.UpdateTaskListItem(id);
            //retrieving the most up to record of the tasklist item from the db
            DataTable dtTaskListItem = dal.GetTaskListItemInfoByID(id);
            globalESCRF.TaskListItem = TaskListItemModel.ConvertDataTableToTaskListItem(dtTaskListItem);
            DataTable dtTaskList = dal.GetTaskListInfoByID(globalESCRF.TaskListItem.TaskListID);
            globalESCRF.TaskList = TaskListModel.ConvertDataTableToTaskList(dtTaskList);
            string affectedEmployee = string.Empty;
            //getting the name of the affected employee depending on form type
            if (globalESCRF.TaskList.ChangeType == "New Hire")
            {
                DataTable dtNewHire = dal.GetESCRFNewHireInfoByTaskListID(globalESCRF.TaskList.ID);
                globalESCRF.NewHire = NewHireModel.ConvertDataTableToNewHire(dtNewHire);
                affectedEmployee = globalESCRF.NewHire.Name().ToString();
            }
            if (globalESCRF.TaskList.ChangeType == "Termination")
            {
                DataTable dtTerm = dal.GetESCRFTerminationInfoByTaskListID(globalESCRF.TaskList.ID);
                globalESCRF.Termination = TerminationModel.ConvertDataTableToTermination(dtTerm);
                affectedEmployee = globalESCRF.Termination.Name().ToString();
            }
            if (globalESCRF.TaskList.ChangeType == "Change in WSI")
            {
                DataTable dtChange = dal.GetESCRFChangeInfoByTaskListID(globalESCRF.TaskList.ID);
                globalESCRF.Change = ChangeModel.ConvertDataTableToChange(dtChange);
                affectedEmployee = globalESCRF.Change.Name().ToString();
            }
            if (globalESCRF.TaskList.ChangeType == "Name Change")
            {
                DataTable dtName = dal.GetESCRFNameInfoByTaskListID(globalESCRF.TaskList.ID);
                globalESCRF.Name = NameModel.ConvertDataTableToName(dtName);
                affectedEmployee = globalESCRF.Name.Name().ToString();
            }
            //VerifyController verifyController = new VerifyController();
            ////Deputy is not a SUPERVISOR which is a distinguished name
            //if (verifyController.Get(globalESCRF.TaskListItem.Deputy.ToString()) == false)
            //{
            //    DataTable dtEmployees = dal.GetEmployeesByDeptAfilliation(globalESCRF.TaskListItem.Deputy);
            //    globalESCRF.Roles = ESCRFRoleModel.GetRoleList(dtEmployees);
            //    foreach (ESCRFRoleModel employee in globalESCRF.Roles)
            //    {
            //        to = employee.EmployeeEmail.ToString();
            //        from = "wsinoreply@nd.gov";
            //        subject = "A task has been completed in the Employee Status Change Portal.";
            //        body = string.Format("Task: {0} /n Affected Employee: {1} /n Has been Completed By: {2}", globalESCRF.TaskListItem.Task, affectedEmployee, user.Name);
            //        dal.SendEmail(to, from, subject, body);
            //    }
            //}

            //to = hrOfficerEmail + ";" + hrDirectorEmail + ";";
            //from = "wsinoreply@nd.gov";
            //subject = "A task has been completed in the Employee Status Change Portal.";
            //body = string.Format("Task: {0} /n Affected Employee: {1} /n Has been Completed By: {2}", globalESCRF.TaskListItem.Task, affectedEmployee, user.Name);
            //dal.SendEmail(to, from, subject, body);

            //Checking to see if all items on the task list are complete.
            DataTable dtTaskListItemList = dal.GetTaskListItemInfoByTaskListID(globalESCRF.TaskListItem.TaskListID);
            globalESCRF.TaskListItemHolder = TaskListItemModel.GetTaskListItemList(dtTaskListItemList);
            bool isComplete = true;
            foreach (TaskListItemModel taskListItem in globalESCRF.TaskListItemHolder)
            {
                if (taskListItem.SignedOn == DateTime.MinValue)
                {
                    isComplete = false;
                }
            }
            // if the form is complete send a message to hr to let them know
            if (isComplete)
            {
                string employee = string.Empty;
                string changetype = globalESCRF.TaskList.ChangeType.ToString();
                if (changetype == "New Hire")
                {
                    employee = globalESCRF.NewHire.Name();
                }
                if (changetype == "Termination")
                {
                    employee = globalESCRF.Termination.Name();
                }
                if (changetype == "Change in WSI")
                {
                    employee = globalESCRF.Change.Name();
                }
                if (changetype == "Name Change")
                {
                    employee = globalESCRF.Name.Name();
                }
                dal.UpdateTaskListFinishedDate(globalESCRF.TaskList.ID);
                to = hrOfficerEmail + ";" + hrDirectorEmail + ";";
                from = "wsinoreply@nd.gov";
                subject = "Employee Status Change Portal: A task list has been completed.";
                body = string.Format("Employee: {0} /n Changetype: {1} /n This tasklist has been completed.", employee, changetype);
            }


            Session["ESCRF"] = globalESCRF;
            return RedirectToAction("Index", globalESCRF);
        }

        public ActionResult AddNewTaskListItem(FormCollection collection)
        {
            //grabbing the context
            if (Session["ESCRF"] != null)
            {
                globalESCRF = (ESCRFViewModel)Session["ESCRF"];
            }
            else
            {
                globalESCRF = new ESCRFViewModel();
            }
            //inserting the tasklistitem
            ESCRFModelDal dal = new ESCRFModelDal();
            VerifyController verify = new VerifyController();
            if (collection["TaskListItem.Deputy"] == "HR")
            {
                collection["TaskListItem.Dept"] = "HR";
            }
            else if (collection["TaskListItem.Deputy"] == "ERGO")
            {
                collection["TaskListItem.Dept"] = "ERGO";
            }
            else if (collection["TaskListItem.Deputy"] == "IS Tech")
            {
                collection["TaskListItem.Dept"] = "IS Tech";
            }
            else if (collection["TaskListItem.Deputy"] == "Facility Management")
            {
                collection["TaskListItem.Dept"] = "Facility Management";
            }
            else if (collection["TaskListItem.Deputy"] == "Help Desk")
            {
                collection["TaskListItem.Dept"] = "Help Desk";
            }
            else
            {
                collection["TaskListItem.Dept"] = "MISC";
            }
            int tasklistItemID = dal.InsertTaskListItemGetID(collection);
            Session["ESCRF"] = globalESCRF;
            return RedirectToAction("CreateList", globalESCRF);
        }

        public ActionResult EditTaskListItem(FormCollection collection)
        {
            //grabbing the context
            if (Session["ESCRF"] != null)
            {
                globalESCRF = (ESCRFViewModel)Session["ESCRF"];
            }
            else
            {
                globalESCRF = new ESCRFViewModel();
            }
            //updating the tasklistitem
            ESCRFModelDal dal = new ESCRFModelDal();
            dal.EditTaskListItem(collection);

            DataTable dtTaskListItem = dal.GetTaskListItemInfoByID(collection["TaskListItem.ID"].ToInt32());
            globalESCRF.TaskListItem = TaskListItemModel.ConvertDataTableToTaskListItem(dtTaskListItem);
            DataTable dtTaskList = dal.GetTaskListInfoByID(globalESCRF.TaskListItem.TaskListID);
            globalESCRF.TaskList = TaskListModel.ConvertDataTableToTaskList(dtTaskList);
            Session["ESCRF"] = globalESCRF;
            if (globalESCRF.TaskList.IsDeployed)
            {
                string path = HttpContext.Request.Url.AbsoluteUri.ToString();
                List<string> host = new List<string>(path.Split(new string[] { "ESCRF" }, StringSplitOptions.None));
                List<String> emailList = new List<string>();
                string from = string.Empty;
                string subject = string.Empty;
                string body = string.Empty;
                int taskListID = globalESCRF.TaskList.ID;
                //a new email will be sent
                //need to figure out who we are sending this email to
                VerifyController verify = new VerifyController();
                if (verify.Get(globalESCRF.TaskListItem.Deputy))
                {
                    RetrieveController retrieve = new RetrieveController();
                    string deputyInfo = retrieve.Get(globalESCRF.TaskListItem.Deputy);
                    List<string> deputyList = deputyInfo.Split('+').ToList();
                    string deputyEmail = deputyList[5];
                    emailList.Add(globalESCRF.TaskListItem.Dep_Email);
                    if (!emailList.Contains(deputyEmail))
                    {
                        emailList.Add(deputyEmail);
                    }
                }
                else
                {
                    DataTable dtRoles = dal.GetDeptAfilliationInfo();
                    List<ESCRFRoleModel> Roles = ESCRFRoleModel.GetRoleList(dtRoles);
                    foreach (ESCRFRoleModel role in Roles)
                    {
                        if (globalESCRF.TaskListItem.Deputy == role.DepartmentAffiliation)
                        {
                            if (!emailList.Contains(role.EmployeeEmail))
                            {
                                emailList.Add(role.EmployeeEmail);
                            }
                        }
                    }
                }
                string to = string.Empty;
                foreach (string email in emailList)
                {
                    to = to + email + ";";
                }
                foreach (string email in emailList)
                {
                    if (globalESCRF.TaskList.ChangeType == "New Hire")
                    {
                        //grab the form info
                        DataTable dtNewHire = dal.GetESCRFNewHireInfoByTaskListID(taskListID);
                        globalESCRF.NewHire = NewHireModel.ConvertDataTableToNewHire(dtNewHire);
                        from = "wsinoreply@nd.gov";
                        subject = "Employee Status Change Request Submitted: New Hire " + globalESCRF.NewHire.Name().ToString();
                        body =
                           "An employee status change request has been submitted with a request type of New Hire. " + System.Environment.NewLine +
                           "<a href=\"" + host[0].ToString() + "ESCRF/ChecklistTasks/" + taskListID + "\"> Checklist Link " + "</a>" + System.Environment.NewLine +
                           "Instructions: " + "<a href=\"" + "file://nd.gov/wsi/wsi-shared/SHAREPOINT%20DOCUMENTS/Human%20Resources/Checklist%20Education.pdf" + "</a>" + System.Environment.NewLine +
                           "Submitted Information: " + System.Environment.NewLine +
                           "Employee is Existing WSI User: Yes " + System.Environment.NewLine +
                           "Employee’s Name: " + globalESCRF.NewHire.Name().ToString() + System.Environment.NewLine +
                           "Job Title: " + globalESCRF.NewHire.JobTitle.ToString() + System.Environment.NewLine +
                           "Effective Date: " + globalESCRF.NewHire.EffectiveDate.ToShortDateString() + System.Environment.NewLine +
                           "Current Supervisor: " + globalESCRF.NewHire.CurrentSupervisor.ToString() + System.Environment.NewLine +
                           "Employee Type: " + globalESCRF.NewHire.FLSAStatus.ToString() + System.Environment.NewLine +
                           "Transferring from State Agency: " + globalESCRF.NewHire.TransferringFromAgency.ToString() + System.Environment.NewLine +
                           "Comments: " + globalESCRF.NewHire.Comments.ToString() + System.Environment.NewLine;
                        dal.SendEmail(to, from, subject, body);
                    }
                    if (globalESCRF.TaskList.ChangeType == "Termination")
                    {
                        string title = string.Empty;
                        if (verify.Get(globalESCRF.Termination.Name()))
                        {
                            RetrieveController retrieve = new RetrieveController();
                            string termInfoString = retrieve.Get(globalESCRF.Termination.Name());
                            List<string> termInfoList = termInfoString.Split('+').ToList();
                            title = termInfoList[6];
                        }
                        //grab the form info
                        DataTable dtTerm = dal.GetESCRFTerminationInfoByTaskListID(taskListID);
                        globalESCRF.Termination = TerminationModel.ConvertDataTableToTermination(dtTerm);
                        from = "wsinoreply@nd.gov";
                        subject = "Employee Status Change Request Submitted: Termination " + globalESCRF.Termination.Name().ToString();
                        body =
                            "An employee status change request has been submitted with a request type of Termination." + System.Environment.NewLine +
                            "<a href=\"" + host[0].ToString() + "ESCRF/ChecklistTasks/" + taskListID + "\"> Checklist Link " + "</a>" + System.Environment.NewLine +
                            "Instructions: " + "<a href=\"" + "file://nd.gov/wsi/wsi-shared/SHAREPOINT%20DOCUMENTS/Human%20Resources/Checklist%20Education.pdf" + "</a>" + System.Environment.NewLine +
                            "Submitted Information: " + System.Environment.NewLine +
                            "Employee is Existing WSI User: Yes " + System.Environment.NewLine +
                            "Employee’s Name: " + globalESCRF.Termination.Name().ToString() + System.Environment.NewLine +
                            "Employee's Title: " + title + System.Environment.NewLine +
                            "Phone Number: " + globalESCRF.Termination.PhoneNumber.ToString() + System.Environment.NewLine +
                            "Office Location: " + globalESCRF.Termination.OfficeLocation.ToString() + System.Environment.NewLine +
                            "Effective Date: " + globalESCRF.Termination.EffectiveDate.ToShortDateString() + System.Environment.NewLine +
                            "Current Supervisor: " + globalESCRF.Termination.CurrentSupervisor.ToString() + System.Environment.NewLine +
                            "Last Day Worked: " + globalESCRF.Termination.LastDateWorked.ToShortDateString() + System.Environment.NewLine +
                            "Transferring To State Agency: " + globalESCRF.Termination.TransferringToAgency.ToString() + System.Environment.NewLine +
                            "Comments: " + globalESCRF.Termination.Comments.ToString();
                        dal.SendEmail(to, from, subject, body);
                    }
                    if (globalESCRF.TaskList.ChangeType == "Change in WSI")
                    {
                        //grab the form info
                        DataTable dtChange = dal.GetESCRFChangeInfoByTaskListID(taskListID);
                        globalESCRF.Change = ChangeModel.ConvertDataTableToChange(dtChange);
                        from = "wsinoreply@nd.gov";
                        subject = "Employee Status Change Request Submitted: Change within WSI " + globalESCRF.Change.Name().ToString();
                        body =
                            "An employee status change request has been submitted with a request type of Change within WSI." + System.Environment.NewLine +
                            "<a href=\"" + host[0].ToString() + "ESCRF/ChecklistTasks/" + taskListID + "\"> Checklist Link " + "</a>" + System.Environment.NewLine +
                            "Instructions: " + "<a href=\"" + "file://nd.gov/wsi/wsi-shared/SHAREPOINT%20DOCUMENTS/Human%20Resources/Checklist%20Education.pdf" + "</a>" + System.Environment.NewLine +
                            "Submitted Information: " + System.Environment.NewLine +
                            "Employee is Existing WSI User: Yes " + System.Environment.NewLine +
                            "Employee’s Name: " + globalESCRF.Change.Name().ToString() + System.Environment.NewLine +
                            "Job Title: " + globalESCRF.Change.PositionName.ToString() + System.Environment.NewLine +
                            "Office Location: " + globalESCRF.Change.OfficeLocation.ToString() + System.Environment.NewLine +
                            "Effective Date: " + globalESCRF.Change.EffectiveDate.ToShortDateString() + System.Environment.NewLine +
                            "Current Supervisor: " + globalESCRF.Change.CurrentSupervisor.ToString() + System.Environment.NewLine +
                            "New Supervisor: " + globalESCRF.Change.NewSupervisor.ToString() + System.Environment.NewLine +
                            "Comments: " + globalESCRF.Change.Comments.ToString() + System.Environment.NewLine;
                        dal.SendEmail(to, from, subject, body);
                    }
                    if (globalESCRF.TaskList.ChangeType == "Name Change")
                    {
                        //This is so the Employee's name string can change depending if they have a middle initial or not.
                        string employeeNameString = string.Empty;
                        if (string.IsNullOrEmpty(globalESCRF.Name.MiddleInitial))
                        {
                            employeeNameString = "Employee’s New Name: " + globalESCRF.Name.LastName.ToString() + ", " + globalESCRF.Name.FirstName.ToString() + System.Environment.NewLine;
                        }
                        else
                        {
                            employeeNameString = "Employee’s New Name: " + globalESCRF.Name.LastName.ToString() + ", " + globalESCRF.Name.FirstName.ToString() + " " + globalESCRF.Name.MiddleInitial + "." + System.Environment.NewLine;
                        }
                        //grab the form info
                        DataTable dtName = dal.GetESCRFNameInfoByTaskListID(taskListID);
                        globalESCRF.Name = NameModel.ConvertDataTableToName(dtName);
                        from = "wsinoreply@nd.gov";
                        subject = "Employee Status Change Request Submitted: Name Change " + globalESCRF.Name.Name().ToString();
                        body =
                            "An employee status change request has been submitted with a request type of Name Change." + System.Environment.NewLine +
                            "<a href=\"" + host[0].ToString() + "ESCRF/ChecklistTasks/" + taskListID + "\"> Checklist Link " + "</a>" + System.Environment.NewLine +
                            "Instructions: " + "<a href=\"" + "file://nd.gov/wsi/wsi-shared/SHAREPOINT%20DOCUMENTS/Human%20Resources/Checklist%20Education.pdf" + "</a>" + System.Environment.NewLine +
                            "Submitted Information: " + System.Environment.NewLine +
                            "Employee is Existing WSI User: Yes " + System.Environment.NewLine +
                            "Employee’s Name: " + globalESCRF.Name.Name().ToString() + System.Environment.NewLine +
                            employeeNameString +
                            "Phone Number: " + globalESCRF.Name.PhoneNumber.ToString() + System.Environment.NewLine +
                            "Office Location: " + globalESCRF.Name.OfficeLocation.ToString() + System.Environment.NewLine +
                            "Effective Date: " + globalESCRF.Name.EffectiveDate.ToShortDateString() + System.Environment.NewLine +
                            "Current Supervisor: " + globalESCRF.Name.CurrentSupervisor.ToString() + System.Environment.NewLine +
                            "Comments: " + globalESCRF.Name.Comments.ToString() + System.Environment.NewLine;
                        //this to is for if the deputy is a real person and not a group
                        dal.SendEmail(to, from, subject, body);
                    }
                }
                return RedirectToAction("CheckListTasks", new { id = globalESCRF.TaskList.ID });

            }
            else
            {
                return RedirectToAction("CreateList", globalESCRF);
            }

            return RedirectToAction("CreateList", globalESCRF);
        }

        public ActionResult DeleteTaskListItem(int ID)
        {
            //grabbing the context
            if (Session["ESCRF"] != null)
            {
                globalESCRF = (ESCRFViewModel)Session["ESCRF"];
            }
            else
            {
                globalESCRF = new ESCRFViewModel();
            }
            //deleting the item from the db
            ESCRFModelDal dal = new ESCRFModelDal();
            dal.DeleteTaskListItemByID(ID);
            Session["ESCRF"] = globalESCRF;
            return RedirectToAction("CreateList", globalESCRF);
        }
        #endregion

        #region Checklist
        public ActionResult ChecklistTasks(int id)
        {

            dal = new ESCRFModelDal();
            // ViewBag.CharitableInfo = dal.GetSFN61579Info("101");

            // Create domain context
            PrincipalContext ctx = new PrincipalContext(ContextType.Domain);

            UserPrincipal u = new UserPrincipal(ctx);

            UserPrincipal user = UserPrincipal.FindByIdentity(ctx, User.Identity.Name);

            DirectoryEntry dirEntry = (DirectoryEntry)user.GetUnderlyingObject();
            string title = dirEntry.Properties["Title"].Value.ToString();

            if (title == "HR Director I" || title == "Human Resource Officer" || title == "HR Director" || title == "Learning and Development Coordinator"
                || title == "Software Engineer")
            {
                ViewBag.isAdmin = true;
            }
            DataTable dtTaskListItem = dal.GetTaskListItemInfoByTaskListID(id);
            ESCRFViewModel vmESCRF = new ESCRFViewModel();
            vmESCRF.ESCRFUI.MinimalUI = true;
            vmESCRF.TaskListItemList = TaskListItemModel.GetTaskListItemList(dtTaskListItem);
            DataTable dtChangeType = dal.GetChangelistInfoByChangelistID(id);
            vmESCRF.ChangeListInfo = ChangelistInfoModel.ConvertDataTableToChangelistInfoItem(dtChangeType);
            dal = new ESCRFModelDal();
            //vmESCRF.TaskListItem = TaskListItemModel.ConvertDataTableToTaskListItem(dtTaskListItem);

            foreach (TaskListItemModel item in vmESCRF.TaskListItemList)
            {
                if (vmESCRF.ChangeListInfo.ChangeType == "New Hire" && item.DefaultTaskID == "1" || 
                    vmESCRF.ChangeListInfo.ChangeType == "Change in WSI" && item.DefaultTaskID == "103" || 
                    vmESCRF.ChangeListInfo.ChangeType == "Termination" && item.DefaultTaskID == "132")
                {
                    ViewBag.TechRequirementsId = item.ID;
                    if (item.Completed == true)
                    {
                        ViewBag.TechRequirementsCompleted = "true";
                    } else
                    {
                        ViewBag.TechRequirmentsCompleted = "false";
                    }
                }
            }

            
            DataTable dtNewHire = dal.GetNewHireByTasklistID(id);
            if (dtNewHire.Rows.Count > 0)
            {
                vmESCRF.NewHire = NewHireModel.ConvertDataTableToNewHire(dtNewHire);
            }

            dal = new ESCRFModelDal();
            DataTable dtChange = dal.GetChangeByTasklistID(id);
            if (dtChange.Rows.Count > 0)
            {
                vmESCRF.Change = ChangeModel.ConvertDataTableToChange(dtChange);
            }

            dal = new ESCRFModelDal();
            DataTable dtNameChange = dal.GetNameChangeByTasklistID(id);
            if (dtNameChange.Rows.Count > 0)
            {
                vmESCRF.Name = NameModel.ConvertDataTableToName(dtNameChange);
            }

            dal = new ESCRFModelDal();
            DataTable dtTermination = dal.GetTerminationByTasklistID(id);
            if (dtTermination.Rows.Count > 0)
            {
                vmESCRF.Termination = TerminationModel.ConvertDataTableToTermination(dtTermination);
            }



            bool isFinance = user.IsMemberOf(ctx, IdentityType.Name, "-Grp-WSI Finance");
            if (isFinance)
            {
                ViewBag.Finance = true;
            }
            else
            {
                ViewBag.Finance = false;
            }

            bool isHelpDesk = user.IsMemberOf(ctx, IdentityType.Name, "-Grp-WSI Help Desk");
            if (isHelpDesk)
            {
                ViewBag.HelpDesk = true;
            }
            else
            {
                ViewBag.HelpDesk = false;
            }

            if (title.ToLower() == "facilities manager" || title.ToLower() == "general trades maintenance worker")
            {
                ViewBag.Facilities = true;
            }
            else
            {
                ViewBag.Facilities = false;
            }

            ViewBag.Username = user.DisplayName;

            // Give permission to select complete button
            if (title.Contains("Human Resource Officer") || title.Contains("HR Director") || title.Contains("Software Engineer")
                || title == "Learning and Development Coordinator")
            {
                ViewBag.Access = true;
            }

            // Set IT security group
            if (ConfigurationManager.AppSettings["Environment"].ToUpper() == "PROD" && (title == "Business Analyst Supervisor" || title == "Application Services Supervisor" ||
                title == "Technical Services Lead Specialist" || user.DisplayName == "Wolf, Timothy J." || title == "Software Engineer"))
            {
                ViewBag.ITSecurity = true;
            }
            else if (user.DisplayName == "Hall, Bradley W." || title == "Technical Services Lead Specialist" ||
              user.DisplayName == "Wolf, Timothy J." || user.DisplayName == "Kunz, Krisi L.")
            {
                ViewBag.ITSecurity = true;
            }

            // Give permission to select complete button
            if (title.Contains("Technical Services"))
            {
                ViewBag.TechServices = true;
            }

            vmESCRF.TaskListItemList = vmESCRF.TaskListItemList.OrderBy(p => p.DefaultTaskID).OrderBy(p => p.Deputy).ToList();


            return View("~/Views/Forms/ESCRF/ChecklistTasks.cshtml", vmESCRF);
        }

        public ActionResult TechnologyRequirements(int id)
        {
            ViewBag.ID = id;


            // Create domain context
            PrincipalContext ctx = new PrincipalContext(ContextType.Domain);

            UserPrincipal u = new UserPrincipal(ctx);

            //UserPrincipal user = UserPrincipal.Current;
            UserPrincipal user = UserPrincipal.FindByIdentity(ctx, User.Identity.Name);

            // Determine which urls to use for the AD functions
            // These are determined by the Environment app setting in the web.config
            if (ConfigurationManager.AppSettings["Environment"].ToUpper() == "DEV")
            {
                ViewBag.Validation = "/api/Validation/?query=";
                ViewBag.Verify = "/api/verify/?query=";
                ViewBag.Retrieve = "/api/retrieve/?query=";
            }
            else if (ConfigurationManager.AppSettings["Environment"].ToUpper() == "TEST")
            {
                ViewBag.Validation = "https://itdwsinett2.netstaging.nd.gov/WSI/onlineforms/api/Validation/?query=";
                ViewBag.Verify = "https://itdwsinett2.netstaging.nd.gov/WSI/onlineforms/api/verify/?query=";
                ViewBag.Retrieve = "https://itdwsinett2.netstaging.nd.gov/WSI/onlineforms/api/retrieve/?query=";
            }
            else if (ConfigurationManager.AppSettings["Environment"].ToUpper() == "PROD")
            {
                ViewBag.Validation = "https://webforms.wsi.nd.gov/api/Validation/?query=";
                ViewBag.Verify = "https://webforms.wsi.nd.gov/api/verify/?query=";
                ViewBag.Retrieve = "https://webforms.wsi.nd.gov/api/retrieve/?query=";
            }

            DataTable dtTaskListItem = dal.GetTaskListItemInfoByID(id);
            ESCRFViewModel vmESCRF = new ESCRFViewModel();
            vmESCRF.TaskListItem = TaskListItemModel.ConvertDataTableToTaskListItem(dtTaskListItem);

            DataTable dtChangeType = dal.GetChangelistInfoByChangelistID(int.Parse(dtTaskListItem.Rows[0]["TASK_LIST_ID"].ToString()));
            vmESCRF.ChangeListInfo = ChangelistInfoModel.ConvertDataTableToChangelistInfoItem(dtChangeType);



            return View("~/Views/Forms/ESCRF/TechnologyRequirements.cshtml", vmESCRF);
        }

        public ActionResult TechnologyRequirementsView(int id)
        {
            ViewBag.ID = id;
            dal = new ESCRFModelDal();
            // ViewBag.CharitableInfo = dal.GetSFN61579Info("101");
            ViewBag.TechServices = true;

            DataTable dtTaskListItem = dal.GetTechnologyRequirementsInfoByID(id);
            ESCRFViewModel vmESCRF = new ESCRFViewModel();
            DataTable dtTaskListItemInfo = dal.GetTaskListItemInfoByID(id);
            vmESCRF.TaskListItem = TaskListItemModel.ConvertDataTableToTaskListItem(dtTaskListItemInfo);
            vmESCRF.TechRequirements = TechnologyRequirementsModel.ConvertDataTableToTechRequirement(dtTaskListItem);
            DataTable dtChangeType = dal.GetChangelistInfoByChangelistID(int.Parse(dtTaskListItemInfo.Rows[0]["TASK_LIST_ID"].ToString()));
            vmESCRF.ChangeListInfo = ChangelistInfoModel.ConvertDataTableToChangelistInfoItem(dtChangeType);
            ViewBag.TaskListId = int.Parse(dtTaskListItemInfo.Rows[0]["TASK_LIST_ID"].ToString());
            //vmESCRF.TaskListItem = TaskListItemModel.ConvertDataTableToTaskListItem(dtTaskListItem);

            //DataTable dtChangeType = dal.GetChangelistInfoByChangelistID(id);
            //vmESCRF.ChangeListInfo = ChangelistInfoModel.GetChangelistInfoList(dtChangeType);

            // Create domain context
            PrincipalContext ctx = new PrincipalContext(ContextType.Domain);

            UserPrincipal u = new UserPrincipal(ctx);

            UserPrincipal user = UserPrincipal.FindByIdentity(ctx, User.Identity.Name);

            DirectoryEntry dirEntry = (DirectoryEntry)user.GetUnderlyingObject();
            string title = dirEntry.Properties["Title"].Value.ToString();
            string lastName = dirEntry.Properties["sn"].Value.ToString();

            // Give permission to select complete button
            if (title.Contains("Human Resource Officer") || title.Contains("HR Director") || title == "Learning and Development Coordinator")
            {
                ViewBag.Access = true;
            }

            // Set IT security group
            if (ConfigurationManager.AppSettings["Environment"].ToUpper() == "PROD" && (title == "Business Analyst Supervisor" || title == "Application Services Supervisor" ||
                title == "Technical Services Lead Specialist" || user.DisplayName == "Wolf, Timothy J." || title == "Software Engineer"))
            {
                ViewBag.ITSecurity = true;
            }
            else if (user.DisplayName == "Hall, Bradley W." || title == "Technical Services Lead Specialist" ||
              user.DisplayName == "Wolf, Timothy J." || user.DisplayName == "Kunz, Krisi L.")
            {
                ViewBag.ITSecurity = true;
            }

            return View("~/Views/Forms/ESCRF/TechnologyRequirementsView.cshtml", vmESCRF);
        }

        public ActionResult EmployeeTechnology(int id)
        {
            if (Session["ESCRF"] != null)
            {
                globalESCRF = (ESCRFViewModel)Session["ESCRF"];
            }
            else
            {
                globalESCRF = new ESCRFViewModel();
            }
            ViewBag.ID = id;

            // Create domain context
            PrincipalContext ctx = new PrincipalContext(ContextType.Domain);

            UserPrincipal u = new UserPrincipal(ctx);

            //UserPrincipal user = UserPrincipal.Current;
            UserPrincipal user = UserPrincipal.FindByIdentity(ctx, User.Identity.Name);

            // Determine which urls to use for the AD functions
            // These are determined by the Environment app setting in the web.config
            if (ConfigurationManager.AppSettings["Environment"].ToUpper() == "DEV")
            {
                ViewBag.Validation = "/api/Validation/?query=";
                ViewBag.Verify = "/api/verify/?query=";
                ViewBag.Retrieve = "/api/retrieve/?query=";
            }
            else if (ConfigurationManager.AppSettings["Environment"].ToUpper() == "TEST")
            {
                ViewBag.Validation = "https://itdwsinett2.netstaging.nd.gov/WSI/onlineforms/api/Validation/?query=";
                ViewBag.Verify = "https://itdwsinett2.netstaging.nd.gov/WSI/onlineforms/api/verify/?query=";
                ViewBag.Retrieve = "https://itdwsinett2.netstaging.nd.gov/WSI/onlineforms/api/retrieve/?query=";
            }
            else if (ConfigurationManager.AppSettings["Environment"].ToUpper() == "PROD")
            {
                ViewBag.Validation = "https://webforms.wsi.nd.gov/api/Validation/?query=";
                ViewBag.Verify = "https://webforms.wsi.nd.gov/api/verify/?query=";
                ViewBag.Retrieve = "https://webforms.wsi.nd.gov/api/verify/?query=";
            }
            globalESCRF.EmployeeTechnology = null;
            DataTable dtTaskListItem = dal.GetTaskListItemInfoByID(id);
            globalESCRF.TaskListItem = TaskListItemModel.ConvertDataTableToTaskListItem(dtTaskListItem);

            DataTable dtChangeType = dal.GetChangelistInfoByChangelistID(int.Parse(dtTaskListItem.Rows[0]["TASK_LIST_ID"].ToString()));
            globalESCRF.ChangeListInfo = ChangelistInfoModel.ConvertDataTableToChangelistInfoItem(dtChangeType);
            Session["ESCRF"] = globalESCRF;
            return View("~/Views/Forms/ESCRF/EmployeeTechnology.cshtml", globalESCRF);
        }

        public ActionResult EmployeeTechnologyView(int id)
        {
            if (Session["ESCRF"] != null)
            {
                globalESCRF = (ESCRFViewModel)Session["ESCRF"];
            }
            else
            {
                globalESCRF = new ESCRFViewModel();
            }
            ViewBag.ID = id;



            DataTable dtTaskListItem = dal.GetTaskListItemInfoByID(id);
            globalESCRF.TaskListItem = TaskListItemModel.ConvertDataTableToTaskListItem(dtTaskListItem);
            ViewBag.TaskListId = int.Parse(dtTaskListItem.Rows[0]["TASK_LIST_ID"].ToString());

            DataTable dtChangeType = dal.GetChangelistInfoByChangelistID(int.Parse(dtTaskListItem.Rows[0]["TASK_LIST_ID"].ToString()));
            globalESCRF.ChangeListInfo = ChangelistInfoModel.ConvertDataTableToChangelistInfoItem(dtChangeType);
            DataTable dtEmployeeTechnology = dal.GetEmployeeTechnologyInfoByID(id);
            globalESCRF.EmployeeTechnology = EmployeeTechnologyModel.ConvertDataTableToEmployeeTechnology(dtEmployeeTechnology);
            Session["ESCRF"] = globalESCRF;
            return View("~/Views/Forms/ESCRF/EmployeeTechnologyView.cshtml", globalESCRF);
        }

        public ActionResult SubmitEmployeeTechnology(FormCollection collection)
        {
            if (Session["ESCRF"] != null)
            {
                globalESCRF = (ESCRFViewModel)Session["ESCRF"];
            }
            else
            {
                globalESCRF = new ESCRFViewModel();
            }
            string environment = ConfigurationManager.AppSettings["Environment"];
            // Create domain context
            PrincipalContext ctx = new PrincipalContext(ContextType.Domain);

            UserPrincipal u = new UserPrincipal(ctx);

            UserPrincipal user = UserPrincipal.FindByIdentity(ctx, User.Identity.Name);

            string name = user.DisplayName;
            string date = DateTime.Today.ToString("MM/dd/yyyy");
            //inserting the form data
            dal.InsertEmployeeTechnology(collection);
            //Now lets complete the TaskListItem
            dal.UpdateESCRFEmployeeTechnologyCompleted(globalESCRF.TaskListItem.ID, name, date);
            Session["ESCRF"] = globalESCRF;

            //Send Emails to the respective people
            List<string> emails = new List<string>();
            string businessAnalystSupervisorEmail = getEmailByTitle("Business Analyst Supervisor");
            emails.Add(businessAnalystSupervisorEmail);
            string applicationServicesSupervisorEmail = getEmailByTitle("Application Services Supervisor");
            emails.Add(applicationServicesSupervisorEmail);
            DataTable ITEmails = dal.GetEmployeesByDeptAfilliation("IS Tech");
            globalESCRF.Roles = ESCRFRoleModel.GetRoleList(ITEmails);
            //add the emails from IS Tech
            foreach (ESCRFRoleModel role in globalESCRF.Roles)
            {
                emails.Add(role.EmployeeEmail);
            }
            
            string semicolon = ";";
            foreach (string email in emails)
            {
                string emailPlusSemicolon = email + semicolon;
                emailPlusSemicolon += email;
            }
            // Put the emails into a string for the email procedure -HD
            string to = string.Join("; ", emails.ToArray());
            string subject = "IT Termination Requirements have been submitted for: " + globalESCRF.Termination.Name();
            string path = HttpContext.Request.Url.AbsoluteUri.ToString();
            List<string> host = new List<string>(
                path.Split(new string[] { "ESCRF" }, StringSplitOptions.None));
            string taskListID = globalESCRF.TaskListItem.TaskListID.ToString();
            // Note from Haley: Added the information from the form to the email body per ticket 21832
            string body = "IT Termination Requirements have been submitted for: " + globalESCRF.Termination.Name() + System.Environment.NewLine +
                "Link: " + "<a href=\"" + host[0].ToString() + "ESCRF/EmployeeTechnologyView/" + taskListID + "\"> Checklist Link " + "</a>" + System.Environment.NewLine + "<br><br>" +
                "<b>Who should their work be transferred to?:</b> " + collection["EmployeeTechnology.TransferringTo"] + "<br><br>" +
                "<b>Can the employee be removed from all programs they currently have access to?:</b> " + collection["EmployeeTechnology.RemoveEmployee"] + "<br><br>" +
                "<b>Is this employee being replaced?:</b> " + collection["EmployeeTechnology.Replaced"] + "<br><br>" +
                "<b>Comments:</b> " + collection["EmployeeTechnology.Comments"];
            // Added the email procedure -HD
            dal.SendEmail(to, "wsinoreply@nd.gov", subject, body);
            return RedirectToAction("CheckListTasks", new { id = globalESCRF.TaskListItem.TaskListID });

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateChecklist(FormCollection collection, string Command)
        {

            // Create domain context
            PrincipalContext ctx = new PrincipalContext(ContextType.Domain);

            UserPrincipal u = new UserPrincipal(ctx);

            UserPrincipal user = UserPrincipal.FindByIdentity(ctx, User.Identity.Name);

            DataTable dtTaskListItem = dal.GetTaskListItemInfoByID(int.Parse(collection["TaskListItem.ID"]));
            ESCRFViewModel vmESCRF = new ESCRFViewModel();
            vmESCRF.TaskListItemList = TaskListItemModel.GetTaskListItemList(dtTaskListItem);
            dal = new ESCRFModelDal();


            if (Command == "Complete")
            {
                collection["TaskListItem.Completed"] = "true";
                collection["TaskListItem.NotApplicable"] = "false";
            }
            else if (Command == "N/A")
            {
                collection["TaskListItem.Completed"] = "false";
                collection["TaskListItem.NotApplicable"] = "true";
            }


            collection["TaskListItem.CompletedBy"] = user.DisplayName;
            collection["TaskListItem.SignedOn"] = DateTime.Today.ToString("yyyy-MM-dd");

            dal.UpdateESCRFCompleted(collection);
            //Now that an action has been taken on this task we need to check to see if all items in the list have been completed
            ESCRFController eSCRF = new ESCRFController();
            string path = HttpContext.Request.Url.AbsoluteUri.ToString();
            List<string> host = new List<string>(
                path.Split(new string[] { "ESCRF" }, StringSplitOptions.None));
            eSCRF.FinishList(int.Parse(collection["TaskListItem.TaskListID"]), host[0].ToString());
            //Redirect back to form landing page
            return RedirectToAction("CheckListTasks", new { id = int.Parse(collection["TaskListItem.TaskListID"]) });

        }

        public ActionResult UpdateTechnologyRequirements(FormCollection collection)
        {
            // Create domain context
            PrincipalContext ctx = new PrincipalContext(ContextType.Domain);

            UserPrincipal u = new UserPrincipal(ctx);

            UserPrincipal user = UserPrincipal.FindByIdentity(ctx, User.Identity.Name);

            string name = user.DisplayName;
            string date = DateTime.Today.ToString("MM/dd/yyyy");

            collection["TechRequirements.NoChange"] = setTrueFalse(collection["TechRequirements.NoChange"]).ToString();
            collection["TechRequirements.CMS"] = setTrueFalse(collection["TechRequirements.CMS"]).ToString();
            collection["TechRequirements.InfoPath"] = setTrueFalse(collection["TechRequirements.InfoPath"]).ToString();
            collection["TechRequirements.MicrosoftReports"] = setTrueFalse(collection["TechRequirements.MicrosoftReports"]).ToString();
            collection["TechRequirements.RecManager"] = setTrueFalse(collection["TechRequirements.RecManager"]).ToString();
            collection["TechRequirements.GreatPlains"] = setTrueFalse(collection["TechRequirements.GreatPlains"]).ToString();
            collection["TechRequirements.AccountingUtility"] = setTrueFalse(collection["TechRequirements.AccountingUtility"]).ToString();
            collection["TechRequirements.ITWorks"] = setTrueFalse(collection["TechRequirements.ITWorks"]).ToString();
            collection["TechRequirements.FileToFilenet"] = setTrueFalse(collection["TechRequirements.FileToFilenet"]).ToString();
            collection["TechRequirements.Indexing"] = setTrueFalse(collection["TechRequirements.Indexing"]).ToString();
            collection["TechRequirements.Verifier"] = setTrueFalse(collection["TechRequirements.Verifier"]).ToString();
            collection["TechRequirements.SecOfState"] = setTrueFalse(collection["TechRequirements.SecOfState"]).ToString();
            collection["TechRequirements.DOT"] = setTrueFalse(collection["TechRequirements.DOT"]).ToString();
            collection["TechRequirements.JobService"] = setTrueFalse(collection["TechRequirements.JobService"]).ToString();
            collection["TechRequirements.CAPS"] = setTrueFalse(collection["TechRequirements.CAPS"]).ToString();
            collection["TechRequirements.Legal"] = setTrueFalse(collection["TechRequirements.Legal"]).ToString();
            collection["TechRequirements.MyWSI"] = setTrueFalse(collection["TechRequirements.MyWSI"]).ToString();

            if (collection["TechRequirements.EmployeeMoving"] == null)
            {
                collection["TechRequirements.EmployeeMoving"] = "false";
            }

            if (collection["TechRequirements.CurrentPhone"] == null)
            {
                collection["TechRequirements.CurrentPhone"] = "false";
            }

            string environment = ConfigurationManager.AppSettings["Environment"];

            dal.InsertTechnologyRequirements(collection);
            dal = new ESCRFModelDal();
            dal.UpdateESCRFTechRequirementsCompleted(collection, name, date);

            // Create email to superusers notifying of form submission
            // Create email list
            var toEmailList = new List<string>();

            // Find users with the job title Multimedia Specialist
            DirectoryEntry enTry = new DirectoryEntry("LDAP://OU=WSI,DC=nd,DC=gov");
            DirectorySearcher mySearcher = new DirectorySearcher();
            SearchResultCollection results;
            mySearcher = new DirectorySearcher(enTry);
            mySearcher.Filter = "(&(company=Workforce Safety & Insurance)(title=" + "Technical Services Lead Specialist" + "))";
            results = mySearcher.FindAll();

            // Add Technical Services Lead Specialist email to list
            if (results.Count > 0)
            {
                foreach (System.DirectoryServices.SearchResult searchResult in results)
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

            mySearcher.Filter = "(&(company=Workforce Safety & Insurance)(title=" + "Business Analyst Supervisor" + "))";
            results = mySearcher.FindAll();

            // Add Business Analyst Supervisor email to list
            if (results.Count > 0)
            {
                foreach (System.DirectoryServices.SearchResult searchResult in results)
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
                            

                        }

                    }
                }
            }

            mySearcher.Filter = "(&(company=Workforce Safety & Insurance)(title=" + "Application Services Supervisor" + "))";
            results = mySearcher.FindAll();

            // Add Business Analyst Supervisor email to list
            if (results.Count > 0)
            {
                foreach (System.DirectoryServices.SearchResult searchResult in results)
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

            if (ConfigurationManager.AppSettings["Environment"].ToUpper() == "PROD")
            {
                mySearcher.Filter = "(&(company=Workforce Safety & Insurance)(title=" + "Employer Services Business Representative" + "))";
                results = mySearcher.FindAll();

                // Add Business Analyst Supervisor email to list
                if (results.Count > 0)
                {
                    foreach (System.DirectoryServices.SearchResult searchResult in results)
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
                mySearcher.Filter = "(&(company=Workforce Safety & Insurance)(title=" + "Claims Unit Supervisor" + "))";
                results = mySearcher.FindAll();

                // Add Business Analyst Supervisor email to list
                if (results.Count > 0)
                {
                    foreach (System.DirectoryServices.SearchResult searchResult in results)
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
            } else
            {
                toEmailList.Add("bhall@nd.gov");
                toEmailList.Add("klkunz@nd.gov");
            }

            // Adding Tim Wolf to the recipient list. Doing it this way as there are other people with the same job title
            toEmailList.Add("tiwolf@nd.gov");

            string path = HttpContext.Request.Url.AbsoluteUri.ToString();
            List<string> host = new List<string>(
                path.Split(new string[] { "ESCRF" }, StringSplitOptions.None));
            string toAddress = string.Join("; ", toEmailList.ToArray());
            string from = "wsinoreply@nd.gov";
            string subject = "A Technology Requirements Checklist Has Been Completed for " + collection["ChangelistInfo.EmployeeName"];
            // Email body contains the form info per IT security request
            string body = "A Technology Requirements checklist has been completed. Please click on the link below to access the checklist. "
            + "<a href=\"" + host[0].ToString() + "ESCRF/TechnologyRequirementsView/" + int.Parse(collection["TaskListItem.ID"]) + "\">Technology Requirements Checklist Link " + "</a><br><br>" +
            "<b>Is the employee moving to a new location?:</b> " + setYesNo(collection["TechRequirements.EmployeeMoving"]) +
            "<br><br><b>New Location:</b> " + collection["TechRequirements.NewLocation"] +
            "<br><br><b>Email distribution groups will be the same as:</b> " + collection["TechRequirements.EmailGroups"] +
            "<br><br><b>CMS/CAPS Field Security will be the same as:</b> " + collection["TechRequirements.FieldSecurity"] +
            "<br><br><b>Select all of the applications to which employee needs access:</b> <br>" +
            "<b>No change:</b> " + setYesNo(collection["TechRequirements.NoChange"]) + "<br>" +
            "<b>CMS:</b> " + setYesNo(collection["TechRequirements.CMS"]) + " <br>" +
            "<b>InfoPath:</b> " + setYesNo(collection["TechRequirements.InfoPath"]) + "<br>" +
            "<b>Microsoft Reports:</b> " + setYesNo(collection["TechRequirements.MicrosoftReports"]) + "<br>" +
            "<b>Rec. Manager:</b> " + setYesNo(collection["TechRequirements.RecManager"]) + "<br>" +
            "<b>Great Plains:</b> " + setYesNo(collection["TechRequirements.GreatPlains"]) + "<br>" +
            "<b>Accounting Utility:</b> " + setYesNo(collection["TechRequirements.AccountingUtility"]) + "<br>" +
            "<b>IT Works:</b> " + setYesNo(collection["TechRequirements.ITWorks"]) + "<br>" +
            "<b>File to Filenet:</b> " + setYesNo(collection["TechRequirements.FileToFilenet"]) + "<br>" +
            "<b>Indexing:</b> " + setYesNo(collection["TechRequirements.Indexing"]) + "<br>" +
            "<b>Verifier:</b> " + setYesNo(collection["TechRequirements.Verifier"]) + "<br>" +
            "<b>Sec of State:</b> " + setYesNo(collection["TechRequirements.SecOfState"]) + " <br>" +
            "<b>DOT:</b> " + setYesNo(collection["TechRequirements.DOT"]) + "<br>" +
            "<b>Job Service:</b> " + setYesNo(collection["TechRequirements.JobService"]) + "<br>" +
            "<b>CAPS:</b> " +setYesNo(collection["TechRequirements.CAPS"]) + "<br>" +
            "<b>Legal/Rehab:</b> " + setYesNo(collection["TechRequirements.Legal"]) + "<br>" +
            "<b>myWSI:</b> " + setYesNo(collection["TechRequirements.MyWSI"]) + "<br>" +
            "<b>Other:</b> " + collection["TechRequirements.Other"] + "<br><br>" +
            "<b>Who should their work queue be transferred to?:</b> " + collection["TechRequirements.WorkQueue"] + "<br><br>" +
            "<b>Are they keeping their current phone number?:</b> " + setYesNo(collection["TechRequirements.CurrentPhone"]) + "<br><br>" +
            "<b>Does the employee need to use the Call Recording System?:</b> " + setYesNo(collection["TechRequirements.CallRecording"]) + "<br><br>" +
            "<b>Does their electronic signature need to change?:</b> " + setYesNo(collection["TechRequirements.ElectronicSignature"]) + "<br><br>" +
            "<b>New Signature:</b> " + collection["TechRequirements.NewSignature"];

            // Send email

            dal.SendEmail(toAddress, from, subject, body);
            //Redirect back to form landing page
            return RedirectToAction("CheckListTasks", new { id = int.Parse(collection["TaskListItem.TaskListID"]) });
        }

        public ActionResult TechnologyRequirementsCompleted(FormCollection collection)
        {
            string environment = ConfigurationManager.AppSettings["Environment"];
            // Create domain context
            PrincipalContext ctx = new PrincipalContext(ContextType.Domain);

            UserPrincipal u = new UserPrincipal(ctx);

            UserPrincipal user = UserPrincipal.FindByIdentity(ctx, User.Identity.Name);

            string name = user.DisplayName;
            string date = DateTime.Today.ToString("MM/dd/yyyy");


            dal.UpdateESCRFTechRequirementsCompletedByIS(collection, name, date);

            //Redirect back to form landing page
            return RedirectToAction("CheckListTasks", new { id = int.Parse(collection["TaskListItem.TaskListID"]) });
        }

        private bool setTrueFalse(string set)
        {
            if (set == "false")
            {
                return false;
            }
            else
            {
                return true;
            }

        }

        private string setYesNo(string set)
        {
            if (set.ToLower() == "true")
            {
                return "Yes";
            } else {
                return "No";
            }
        }
        #endregion

        #region Delete Functions
        // Started to make delete functions, disregard and delete if we do something different
        // SQL procedures in here are in the ModelDAL for the time being
        public ActionResult NewHireDelete(FormCollection collection, int id, string taskListId)
        {

            dal.DeleteESCRFNewHire(id, taskListId);

            return RedirectToAction("Index");
        }

        public ActionResult ChangeDelete(FormCollection collection, int id, string taskListId)
        {
            dal.DeleteESCRFChange(id, taskListId);

            return RedirectToAction("Index");
        }

        public ActionResult NameDelete(FormCollection collection, int id, string taskListId)
        {
            dal.DeleteESCRFName(id, taskListId);

            return RedirectToAction("Index");
        }

        public ActionResult TerminationDelete(FormCollection collection, int id, string taskListId)
        {
            dal.DeleteESCRFTermination(id, taskListId);

            return RedirectToAction("Index");
        }
        #endregion

        // Actions in this region are used in pages that are used in multple other regions
        #region Common
        //Used to cancal editing the form
        public ActionResult BackToView()
        {
            //grabbing the context
            if (Session["ESCRF"] != null)
            {
                globalESCRF = (ESCRFViewModel)Session["ESCRF"];
            }
            else
            {
                globalESCRF = new ESCRFViewModel();
            }
            //redirecting based on form debt
            if (globalESCRF.ChangeType == "New Hire")
            {
                Session["ESCRF"] = globalESCRF;
                return RedirectToAction("NewHireView", new { id = globalESCRF.NewHire.ID });
            }
            if (globalESCRF.ChangeType == "Termination")
            {
                Session["ESCRF"] = globalESCRF;
                return RedirectToAction("TerminationView", new { id = globalESCRF.Termination.ID });
            }
            if (globalESCRF.ChangeType == "Change in WSI")
            {
                Session["ESCRF"] = globalESCRF;
                return RedirectToAction("ChangeView", new { id = globalESCRF.Change.ID });
            }
            if (globalESCRF.ChangeType == "Name Change")
            {
                Session["ESCRF"] = globalESCRF;
                return RedirectToAction("NameView", new { id = globalESCRF.Name.ID });
            }
            else
            {
                Session["ESCRF"] = globalESCRF;
                return RedirectToAction("Index");
            }
        }

        public string getEmailByTitle(string title)
        {

            //Console.WriteLine(title);
            if (title == null)
            {
                title = "";
            }
            else
            {
                title = title.Trim();
            }

            string email = string.Empty;
            //grabs all people from ad
            using (PrincipalContext principalContext = new PrincipalContext(ContextType.Domain, "nd.gov", "OU=WSI, DC=nd,DC=gov"))
            using (UserPrincipal userPrincipal = new UserPrincipal(principalContext) { Enabled = true })
            using (PrincipalSearcher userSearcher = new PrincipalSearcher(userPrincipal))
            using (PrincipalSearchResult<Principal> results = userSearcher.FindAll())
            {
                foreach (Principal result in results)
                {
                    //filtering out ad profiles that 
                    string firstLast = result.ToString();
                    string distinguished = result.DistinguishedName.ToString();
                    if (distinguished.Contains("CN=WSI"))
                    { continue; }
                    if (distinguished.Contains("CN=!"))
                    { continue; }
                    if (distinguished.Contains("OU=Vendor-Contractor"))
                    { continue; }
                    //looptitle is of the ad profile in this iteration of the loop
                    string loopTitle = string.Empty;
                    DirectoryEntry userInEntry = (DirectoryEntry)result.GetUnderlyingObject();
                    //some profile dont have titles this is so the action does not break
                    try
                    {
                        loopTitle = userInEntry.Properties["title"].Value.ToString();
                    }
                    catch
                    {
                        loopTitle = "N/A";
                    }
                    //if the title of the ad profile matches the parameter then we grab the email
                    if (loopTitle == title)
                    {
                        email = userInEntry.Properties["mail"].Value.ToString();
                        break;
                    }

                }
                if (email == string.Empty)
                {
                    email = "";
                }
                return email;
            }
        }
        #endregion

        #region UIControls
        public ActionResult AllFormsToggled()
        {
            //grabbing the context
            if (Session["ESCRF"] != null)
            {
                globalESCRF = (ESCRFViewModel)Session["ESCRF"];
            }
            else
            {
                globalESCRF = new ESCRFViewModel();
            }
            if (globalESCRF.ESCRFUI.AllFormsToggle == false)
            {
                globalESCRF.ESCRFUI.AllFormsToggle = true;
                globalESCRF.ESCRFUI.TerminationToggle = false;
                globalESCRF.ESCRFUI.NewHireToggle = false;
                globalESCRF.ESCRFUI.ChangeToggle = false;
                globalESCRF.ESCRFUI.NameToggle = false;
                globalESCRF.ESCRFUI.FormsOnPagesArray.Clear();
                globalESCRF.ESCRFUI.PageCount = 0;
            }
            Session["ESCRF"] = globalESCRF;
            return RedirectToAction("Index");
        }

        public ActionResult NewHireToggled()
        {
            if (Session["ESCRF"] != null)
            {
                globalESCRF = (ESCRFViewModel)Session["ESCRF"];
            }
            else
            {
                globalESCRF = new ESCRFViewModel();
            }
            if (globalESCRF.ESCRFUI.NewHireToggle == false)
            {
                globalESCRF.ESCRFUI.NewHireToggle = true;
                globalESCRF.ESCRFUI.AllFormsToggle = false;
                globalESCRF.ESCRFUI.TerminationToggle = false;
                globalESCRF.ESCRFUI.ChangeToggle = false;
                globalESCRF.ESCRFUI.NameToggle = false;
                globalESCRF.ESCRFUI.PageCount = 0;
            }

            Session["ESCRF"] = globalESCRF;
            return RedirectToAction("Index");
        }

        public ActionResult TerminationToggled()
        {
            if (Session["ESCRF"] != null)
            {
                globalESCRF = (ESCRFViewModel)Session["ESCRF"];
            }
            else
            {
                globalESCRF = new ESCRFViewModel();
            }
            if (globalESCRF.ESCRFUI.TerminationToggle == false)
            {
                globalESCRF.ESCRFUI.TerminationToggle = true;
                globalESCRF.ESCRFUI.AllFormsToggle = false;
                globalESCRF.ESCRFUI.NewHireToggle = false;
                globalESCRF.ESCRFUI.ChangeToggle = false;
                globalESCRF.ESCRFUI.NameToggle = false;
                globalESCRF.ESCRFUI.PageCount = 0;
            }

            Session["ESCRF"] = globalESCRF;
            return RedirectToAction("Index");
        }

        public ActionResult ChangeToggled()
        {
            if (Session["ESCRF"] != null)
            {
                globalESCRF = (ESCRFViewModel)Session["ESCRF"];
            }
            else
            {
                globalESCRF = new ESCRFViewModel();
            }
            if (globalESCRF.ESCRFUI.ChangeToggle == false)
            {
                globalESCRF.ESCRFUI.ChangeToggle = true;
                globalESCRF.ESCRFUI.AllFormsToggle = false;
                globalESCRF.ESCRFUI.NewHireToggle = false;
                globalESCRF.ESCRFUI.TerminationToggle = false;
                globalESCRF.ESCRFUI.NameToggle = false;
                globalESCRF.ESCRFUI.PageCount = 0;
            }

            Session["ESCRF"] = globalESCRF;
            return RedirectToAction("Index");
        }

        public ActionResult NameToggled()
        {
            if (Session["ESCRF"] != null)
            {
                globalESCRF = (ESCRFViewModel)Session["ESCRF"];
            }
            else
            {
                globalESCRF = new ESCRFViewModel();
            }
            if (globalESCRF.ESCRFUI.NameToggle == false)
            {
                globalESCRF.ESCRFUI.NameToggle = true;
                globalESCRF.ESCRFUI.AllFormsToggle = false;
                globalESCRF.ESCRFUI.NewHireToggle = false;
                globalESCRF.ESCRFUI.TerminationToggle = false;
                globalESCRF.ESCRFUI.ChangeToggle = false;
                globalESCRF.ESCRFUI.PageCount = 0;
            }

            Session["ESCRF"] = globalESCRF;
            return RedirectToAction("Index");
        }

        public ActionResult AllStatusToggled()
        {
            if (Session["ESCRF"] != null)
            {
                globalESCRF = (ESCRFViewModel)Session["ESCRF"];
            }
            else
            {
                globalESCRF = new ESCRFViewModel();
            }
            if (globalESCRF.ESCRFUI.AllStatusToggle == false)
            {
                globalESCRF.ESCRFUI.AllStatusToggle = true;
                globalESCRF.ESCRFUI.InProgressToggle = false;
                globalESCRF.ESCRFUI.NotDeployedToggle = false;
                globalESCRF.ESCRFUI.CompletedToggle = false;
                globalESCRF.ESCRFUI.FormsOnPagesArray.Clear();
                globalESCRF.ESCRFUI.PageCount = 0;
            }
            Session["ESCRF"] = globalESCRF;
            return RedirectToAction("Index");
        }

        public ActionResult NotDeployedToggled()
        {
            if (Session["ESCRF"] != null)
            {
                globalESCRF = (ESCRFViewModel)Session["ESCRF"];
            }
            else
            {
                globalESCRF = new ESCRFViewModel();
            }
            if (globalESCRF.ESCRFUI.NotDeployedToggle == false)
            {
                globalESCRF.ESCRFUI.NotDeployedToggle = true;
                globalESCRF.ESCRFUI.AllStatusToggle = false;
                globalESCRF.ESCRFUI.PageCount = 0;
            }
            else
            {
                globalESCRF.ESCRFUI.NotDeployedToggle = false;
                globalESCRF.ESCRFUI.PageCount = 0;
            }
            if (globalESCRF.ESCRFUI.NotDeployedToggle == true && globalESCRF.ESCRFUI.InProgressToggle == true && globalESCRF.ESCRFUI.CompletedToggle == true)
            {
                globalESCRF.ESCRFUI.NotDeployedToggle = false;
                globalESCRF.ESCRFUI.InProgressToggle = false;
                globalESCRF.ESCRFUI.CompletedToggle = false;
                globalESCRF.ESCRFUI.AllStatusToggle = true;
            }
            if (globalESCRF.ESCRFUI.NotDeployedToggle == false && globalESCRF.ESCRFUI.InProgressToggle == false && globalESCRF.ESCRFUI.CompletedToggle == false)
            {
                globalESCRF.ESCRFUI.AllStatusToggle = true;

            }
            Session["ESCRF"] = globalESCRF;
            return RedirectToAction("Index");
        }

        public ActionResult InProgressToggled()
        {
            if (Session["ESCRF"] != null)
            {
                globalESCRF = (ESCRFViewModel)Session["ESCRF"];
            }
            else
            {
                globalESCRF = new ESCRFViewModel();
            }
            if (globalESCRF.ESCRFUI.InProgressToggle == false)
            {
                globalESCRF.ESCRFUI.InProgressToggle = true;
                globalESCRF.ESCRFUI.AllStatusToggle = false;
                globalESCRF.ESCRFUI.PageCount = 0;
            }
            else
            {
                globalESCRF.ESCRFUI.InProgressToggle = false;
                globalESCRF.ESCRFUI.PageCount = 0;
            }
            if (globalESCRF.ESCRFUI.NotDeployedToggle == true && globalESCRF.ESCRFUI.InProgressToggle == true && globalESCRF.ESCRFUI.CompletedToggle == true)
            {
                globalESCRF.ESCRFUI.NotDeployedToggle = false;
                globalESCRF.ESCRFUI.InProgressToggle = false;
                globalESCRF.ESCRFUI.CompletedToggle = false;
                globalESCRF.ESCRFUI.AllStatusToggle = true;
            }
            if (globalESCRF.ESCRFUI.NotDeployedToggle == false && globalESCRF.ESCRFUI.InProgressToggle == false && globalESCRF.ESCRFUI.CompletedToggle == false)
            {
                globalESCRF.ESCRFUI.AllStatusToggle = true;

            }
            Session["ESCRF"] = globalESCRF;
            return RedirectToAction("Index");
        }

        public ActionResult CompletedToggled()
        {
            if (Session["ESCRF"] != null)
            {
                globalESCRF = (ESCRFViewModel)Session["ESCRF"];
            }
            else
            {
                globalESCRF = new ESCRFViewModel();
            }
            if (globalESCRF.ESCRFUI.CompletedToggle == false)
            {
                globalESCRF.ESCRFUI.CompletedToggle = true;
                globalESCRF.ESCRFUI.AllStatusToggle = false;
                globalESCRF.ESCRFUI.PageCount = 0;
            }
            else
            {
                globalESCRF.ESCRFUI.CompletedToggle = false;
                globalESCRF.ESCRFUI.PageCount = 0;
            }
            if (globalESCRF.ESCRFUI.NotDeployedToggle == true && globalESCRF.ESCRFUI.InProgressToggle == true && globalESCRF.ESCRFUI.CompletedToggle == true)
            {
                globalESCRF.ESCRFUI.NotDeployedToggle = false;
                globalESCRF.ESCRFUI.InProgressToggle = false;
                globalESCRF.ESCRFUI.CompletedToggle = false;
                globalESCRF.ESCRFUI.AllStatusToggle = true;
            }
            if (globalESCRF.ESCRFUI.NotDeployedToggle == false && globalESCRF.ESCRFUI.InProgressToggle == false && globalESCRF.ESCRFUI.CompletedToggle == false)
            {
                globalESCRF.ESCRFUI.AllStatusToggle = true;

            }
            Session["ESCRF"] = globalESCRF;
            return RedirectToAction("Index");
        }

        public ActionResult Previous()
        {
            if (Session["ESCRF"] != null)
            {
                globalESCRF = (ESCRFViewModel)Session["ESCRF"];
            }
            else
            {
                globalESCRF = new ESCRFViewModel();
            }
            if (globalESCRF.ESCRFUI.PageCount > 0)
            {
                globalESCRF.ESCRFUI.PageCount--;
            }
            Session["ESCRF"] = globalESCRF;
            return RedirectToAction("Index");
        }
        public ActionResult Next()
        {
            if (Session["ESCRF"] != null)
            {
                globalESCRF = (ESCRFViewModel)Session["ESCRF"];
            }
            else
            {
                globalESCRF = new ESCRFViewModel();
            }
            if (globalESCRF.ESCRFUI.PageCount < globalESCRF.ESCRFUI.Pages)
            {
                globalESCRF.ESCRFUI.PageCount++;
            }
            Session["ESCRF"] = globalESCRF;
            return RedirectToAction("Index");

        }
        //this is for the the submitted forms tables and sorting them 

        //this will control the sorting of the Change Type Sort
        public ActionResult ChangeTypeSort()
        {
            //grabbing the context
            if (Session["ESCRF"] != null)
            {
                globalESCRF = (ESCRFViewModel)Session["ESCRF"];
            }
            else
            {
                globalESCRF = new ESCRFViewModel();
            }
            // If sort by is not ChangeType then that means we are switching from sorting another column so we want to default to ascending. 
            if (globalESCRF.SortBy != "Change Type")
            {
                globalESCRF.IsAscending = false;
            }
            globalESCRF.SortBy = "Change Type";
            //if the changetype is already selected then clicking on the button is to change the sort order 
            if (globalESCRF.SortBy == "Change Type")
            {
                if (globalESCRF.IsAscending == true)
                {
                    globalESCRF.IsAscending = false;
                }
                else
                {
                    globalESCRF.IsAscending = true;
                }
            }
            Session["ESCRF"] = globalESCRF;
            return RedirectToAction("Index");
        }

        public ActionResult EmployeeNameSort()
        {
            //grabbing the context
            if (Session["ESCRF"] != null)
            {
                globalESCRF = (ESCRFViewModel)Session["ESCRF"];
            }
            else
            {
                globalESCRF = new ESCRFViewModel();
            }
            // If sort by is not ChangeType then that means we are switching from sorting another column so we want to default to ascending. 
            if (globalESCRF.SortBy != "Employee Name")
            {
                globalESCRF.IsAscending = false;
            }
            globalESCRF.SortBy = "Employee Name";
            //if the changetype is already selected then clicking on the button is to change the sort order 
            if (globalESCRF.SortBy == "Employee Name")
            {
                if (globalESCRF.IsAscending == true)
                {
                    globalESCRF.IsAscending = false;
                }
                else
                {
                    globalESCRF.IsAscending = true;
                }
            }
            Session["ESCRF"] = globalESCRF;
            return RedirectToAction("Index");
        }

        public ActionResult SupervisorSort()
        {
            //grabbing the context
            if (Session["ESCRF"] != null)
            {
                globalESCRF = (ESCRFViewModel)Session["ESCRF"];
            }
            else
            {
                globalESCRF = new ESCRFViewModel();
            }
            // If sort by is not ChangeType then that means we are switching from sorting another column so we want to default to ascending. 
            if (globalESCRF.SortBy != "Supervisor")
            {
                globalESCRF.IsAscending = false;
            }
            globalESCRF.SortBy = "Supervisor";
            //if the changetype is already selected then clicking on the button is to change the sort order 
            if (globalESCRF.SortBy == "Supervisor")
            {
                if (globalESCRF.IsAscending == true)
                {
                    globalESCRF.IsAscending = false;
                }
                else
                {
                    globalESCRF.IsAscending = true;
                }
            }
            Session["ESCRF"] = globalESCRF;
            return RedirectToAction("Index");
        }

        public ActionResult NewSupervisorSort()
        {
            //grabbing the context
            if (Session["ESCRF"] != null)
            {
                globalESCRF = (ESCRFViewModel)Session["ESCRF"];
            }
            else
            {
                globalESCRF = new ESCRFViewModel();
            }
            // If sort by is not ChangeType then that means we are switching from sorting another column so we want to default to ascending. 
            if (globalESCRF.SortBy != "New Supervisor")
            {
                globalESCRF.IsAscending = false;
            }
            globalESCRF.SortBy = "New Supervisor";
            //if the changetype is already selected then clicking on the button is to change the sort order 
            if (globalESCRF.SortBy == "New Supervisor")
            {
                if (globalESCRF.IsAscending == true)
                {
                    globalESCRF.IsAscending = false;
                }
                else
                {
                    globalESCRF.IsAscending = true;
                }
            }
            Session["ESCRF"] = globalESCRF;
            return RedirectToAction("Index");
        }

        public ActionResult ModifiedBySort()
        {
            //grabbing the context
            if (Session["ESCRF"] != null)
            {
                globalESCRF = (ESCRFViewModel)Session["ESCRF"];
            }
            else
            {
                globalESCRF = new ESCRFViewModel();
            }
            // If sort by is not ChangeType then that means we are switching from sorting another column so we want to default to ascending. 
            if (globalESCRF.SortBy != "Modified By")
            {
                globalESCRF.IsAscending = false;
            }
            globalESCRF.SortBy = "Modified By";
            //if the changetype is already selected then clicking on the button is to change the sort order 
            if (globalESCRF.SortBy == "Modified By")
            {
                if (globalESCRF.IsAscending == true)
                {
                    globalESCRF.IsAscending = false;
                }
                else
                {
                    globalESCRF.IsAscending = true;
                }
            }
            Session["ESCRF"] = globalESCRF;
            return RedirectToAction("Index");
        }

        public ActionResult SubmittedOnSort()
        {
            //grabbing the context
            if (Session["ESCRF"] != null)
            {
                globalESCRF = (ESCRFViewModel)Session["ESCRF"];
            }
            else
            {
                globalESCRF = new ESCRFViewModel();
            }
            // If sort by is not ChangeType then that means we are switching from sorting another column so we want to default to ascending. 
            if (globalESCRF.SortBy != "Submitted On")
            {
                globalESCRF.IsAscending = false;
            }
            globalESCRF.SortBy = "Submitted On";
            //if the changetype is already selected then clicking on the button is to change the sort order 
            if (globalESCRF.SortBy == "Submitted On")
            {
                if (globalESCRF.IsAscending == true)
                {
                    globalESCRF.IsAscending = false;
                }
                else
                {
                    globalESCRF.IsAscending = true;
                }
            }
            Session["ESCRF"] = globalESCRF;
            return RedirectToAction("Index");
        }

        public ActionResult EffectiveOnSort()
        {
            //grabbing the context
            if (Session["ESCRF"] != null)
            {
                globalESCRF = (ESCRFViewModel)Session["ESCRF"];
            }
            else
            {
                globalESCRF = new ESCRFViewModel();
            }
            // If sort by is not ChangeType then that means we are switching from sorting another column so we want to default to ascending. 
            if (globalESCRF.SortBy != "Effective On")
            {
                globalESCRF.IsAscending = false;
            }
            globalESCRF.SortBy = "Effective On";
            //if the changetype is already selected then clicking on the button is to change the sort order 
            if (globalESCRF.SortBy == "Effective On")
            {
                if (globalESCRF.IsAscending == true)
                {
                    globalESCRF.IsAscending = false;
                }
                else
                {
                    globalESCRF.IsAscending = true;
                }
            }
            Session["ESCRF"] = globalESCRF;
            return RedirectToAction("Index");
        }

        public ActionResult StatusSort()
        {
            //grabbing the context
            if (Session["ESCRF"] != null)
            {
                globalESCRF = (ESCRFViewModel)Session["ESCRF"];
            }
            else
            {
                globalESCRF = new ESCRFViewModel();
            }
            // If sort by is not ChangeType then that means we are switching from sorting another column so we want to default to ascending. 
            if (globalESCRF.SortBy != "Status")
            {
                globalESCRF.IsAscending = false;
            }
            globalESCRF.SortBy = "Status";
            //if the changetype is already selected then clicking on the button is to change the sort order 
            if (globalESCRF.SortBy == "Status")
            {
                if (globalESCRF.IsAscending == true)
                {
                    globalESCRF.IsAscending = false;
                }
                else
                {
                    globalESCRF.IsAscending = true;
                }
            }
            Session["ESCRF"] = globalESCRF;
            return RedirectToAction("Index");
        }
        #endregion
    }
}
