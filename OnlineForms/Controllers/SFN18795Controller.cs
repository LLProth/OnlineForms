using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Web.Mvc;
using OnlineForms.ViewModels;
using OnlineForms.Models.SFN18795;
using OnlineForms.Models;
using OnlineForms.Helper;
using System.Data;
using FormCollection = System.Web.Mvc.FormCollection;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Web.WebPages;
using System.Configuration;
using System;

namespace OnlineForms.Controllers
{
    [Authorize]
    public class SFN18795Controller : Controller
    {
        private ModelDAL dal = new ModelDAL();
        private SFN18795ModelDal sfn18795Dal = new SFN18795ModelDal();
		
		private static PrincipalContext context = new PrincipalContext(ContextType.Domain);
        private static UserPrincipal u = new UserPrincipal(context);
		private string _filter = "";

		#region Content_Functions
		/*
         * Landing Page /SFN18795
         * List Users submissions or for Procurement officer list all submissions
        */
		public ActionResult Index(string sortOrder)
        {
            setViewbagDataReturnTitle();
			getConfigValues();
			// Get Table information for list
			sfn18795Dal = new SFN18795ModelDal();
            DataTable dtInfo = sfn18795Dal.GetSFN18795();

            SFN18795IndexViewModel lSFN18795 = new SFN18795IndexViewModel();
            lSFN18795.SFN18795s = SFN18795Model.GetSFN18795List(dtInfo);
            lSFN18795.RequisitionItems = SFN18795RequisitionItem.ConvertDataTableToRequisitionItemList(dtInfo);
            // Set default sort list by lastest submit date
            ViewBag.DateSubmitted = sortOrder == "date_desc" ? "date_asc" : "date_desc";
            switch (sortOrder)
            {
                default:
                    lSFN18795.SFN18795s = lSFN18795.SFN18795s.OrderByDescending(i => i.DateSubmitted).ToList();
                    break;
            }
            // Check if there is a total price 
            if (dtInfo.Rows.Count > 0)
            {
                ViewBag.TotalPrice = dtInfo.Rows[0]["REQ_ITEMS_TOTAL"].ToString();
            }

            ViewBag.sortOrder = sortOrder;

            return View("~/Views/Forms/SFN18795/Index.cshtml", lSFN18795);
        }

        /*
        * Form Creation Page /SFN18795/Create
        * User submits a SFN18795 form that is sent to appropiate supervisor for approval
        */
        public ActionResult Create()
        {
			getConfigValues();
			setViewbagDataReturnTitle();
			// Get Login user 
			UserPrincipal loggedInUser = UserPrincipal.FindByIdentity(context, User.Identity.Name);

            return View("~/Views/Forms/SFN18795/Create.cshtml");
        }

		/*
        * Form Approval Page /SFN18795/Approval/ID
        * Supervisor user and Directors will approve of deny request and send request to next approver when necessary
        * @param [string] id - ID number of the submitted form
        */
		public ActionResult Approval(int id)
		{
			getConfigValues();
			List<string> personelnames = new List<string>();
			bool isSupervisor = false;
			bool isDirector = false;
			bool isDepartment = false;
			bool isChief = false;
			bool isDirectorSupervisor = false;
			string employeeJobTitle;
			string employeeManager;
			string employeeManagerTitle = "";
			string employeeChief = "";
			string employeeDepartment = "";
			// Get form information by the ID number
			sfn18795Dal = new SFN18795ModelDal();
			DataTable dtInfo = sfn18795Dal.GetSFN18795InfoByID(id);
			// Set form information by the database information
			SFN18795DisplayViewModel vmSFN18795 = new SFN18795DisplayViewModel();
			vmSFN18795.RequisitionModel = SFN18795Model.ConvertDataTableToRequisition(dtInfo);
			vmSFN18795.RequisitionItems = SFN18795RequisitionItem.ConvertDataTableToRequisitionItemList(dtInfo);

			// Get the requesting Employee from the form and employee information
			UserPrincipal requestingEmployee = UserPrincipal.FindByIdentity(context, vmSFN18795.RequisitionModel.EmplID);
			employeeJobTitle = DirectoryHelper.getJobTitle(requestingEmployee);
			employeeChief = DirectoryHelper.findChief(requestingEmployee);
			employeeDepartment = DirectoryHelper.findDepartmentDirector(requestingEmployee);
			// Check if the person submitting the form is not the director them check for the manager. 
			if (employeeJobTitle != "Director")
			{
				employeeManager = DirectoryHelper.getManager(requestingEmployee);
			}
			else
			{
				employeeManager = "";
				employeeManagerTitle = "";
			}
			// If manager is not blank get set manager name and title 
			if (employeeManager != "")
			{
				UserPrincipal manager = UserPrincipal.FindByIdentity(context, employeeManager);
				employeeManager = manager.DisplayName;
				employeeManagerTitle = DirectoryHelper.getJobTitle(manager);
			}
			// Check if the manager is the director
			if (employeeManagerTitle.Equals("Director"))
			{
				isDirectorSupervisor = true;
			}
			// Set the user log in information
			UserPrincipal loggedInUser = UserPrincipal.FindByIdentity(context, User.Identity.Name);

			string jobtitle = setViewbagDataReturnTitle();
			// Check if the submitter if the COO and the login user is the director to be the supervisor
			if (employeeJobTitle == "Chief Operations Officer" && jobtitle == "Director")
			{
				isSupervisor = true;
			}
			// Check the requisition total and check who can approve the form base on price, and the submitter and the user that is logged in viewing the form
			if (vmSFN18795.RequisitionModel.ReqItemsTotal > 50000.00)
			{
				// Check if there is a a Department Director, if the Department Equals the Director Check for Chiefs or Director approval
				if (employeeDepartment.Equals("Director"))
				{
					// If submitter is COO the Director of Finance approves
					if (employeeJobTitle.Equals("Chief Operations Officer"))
					{
						if (jobtitle.Equals("Director of Finance"))
						{
							isChief = true;
						}
					}
					// Check is approver is the Chief of Employee services or Chief of Injury Services
					else if (jobtitle == employeeChief)
					{
						isChief = true;
					}
					// If the manager is the Director then the COO will approve first
					else if (jobtitle == "Chief Operations Officer" && isDirectorSupervisor == true)
					{
						isChief = true;
					}
					// Else the Director will approve all others
					else if (jobtitle == "Director")
					{
						isDirector = true;
						isChief = false;
					}
				}
				else
				{
					// Check if the submitter is the Director and follow approval process
					if (employeeJobTitle.Equals("Director"))
					{
						// First to approve should be the Director of Finance
						if (jobtitle == "Director of Finance")
						{
							isChief = true;
						}
						// If approved by Director of Finance then the COO will approve
						if (jobtitle.Equals("Chief Operations Officer") && vmSFN18795.RequisitionModel.ChiefSignature != "")
						{
							isChief = false;
							isDirector = true;
						}
					}
					else
					{
						// Employee with Depertment Director will first to the their Department Director then the Director will approve
						if (jobtitle == employeeDepartment)
						{
							isDepartment = true;
						}
						if (jobtitle == "Director" && vmSFN18795.RequisitionModel.DepartmentSignature != "")
						{
							isDirector = true;
							isDepartment = false;
						}
					}
				}
			}
			// Approvals between 5000 to 50000
			else if (vmSFN18795.RequisitionModel.ReqItemsTotal > 5000.00 && vmSFN18795.RequisitionModel.ReqItemsTotal <= 50000.00)
			{
				// First check if there is a department/division Director
				if (jobtitle == employeeDepartment)
				{
					if (jobtitle == "Director")
					{
						isChief = true;
					}
					else
					{
						isDepartment = true;
					}
				}
				// Check if the Department Director equals the Director or if the Department Director has already approved. Then the Division Chief approves.
				else if ((jobtitle == employeeChief && employeeDepartment.Equals("Director")) ||
					(jobtitle == employeeChief && vmSFN18795.RequisitionModel.DepartmentSignature != ""))
				{
					isChief = true;
					isDepartment = false;
				}
				// If the Submitter is the Director then the Director of finance will approve
				else if (employeeJobTitle == "Director" && jobtitle == "Director of Finance")
				{
					isChief = true;
					isDepartment = false;
				}
				// If the submitter is the COO then Director will approve
				else if (employeeJobTitle == "Chief Operations Officer" && jobtitle == "Director")
				{
					isChief = true;
					isDepartment = false;
				}
			}
			// Approvals between 500 to 5000
			else if (vmSFN18795.RequisitionModel.ReqItemsTotal > 500.00 && vmSFN18795.RequisitionModel.ReqItemsTotal <= 5000.00)
			{
				// First check if there is a department/division Director
				if (jobtitle == employeeDepartment)
				{
					isDepartment = true;
				}
				// Check if the Department Director equals the Director or if the Department Director has already approved. Then the Division Chief approves.
				else if (employeeJobTitle.Contains("Director") && employeeJobTitle != "Director" && jobtitle == employeeChief)
				{
					isDepartment = true;
				}
				// If the Submitter is the Director then the Director of finance will approve
				else if (employeeJobTitle == "Director" && jobtitle == "Director of Finance")
				{
					isDepartment = true;
				}
				// If the submitter is the COO then Director will approve
				else if (employeeJobTitle == "Chief Operations Officer" && jobtitle == "Director")
				{
					isDepartment = true;
				}
				// If the submitter is a Division Chief the COO will approve
				else if ((employeeJobTitle == "Chief of Employer Services" || employeeJobTitle == "Chief of Injury Services" || employeeJobTitle.Equals("General Counsel")
					|| employeeJobTitle.Equals("Director of Information Technology") || employeeJobTitle.Equals("Director of Strategic Operations") || employeeJobTitle.Equals("Facilities Manager")
					|| employeeJobTitle.Equals("Director of Finance")) && jobtitle == "Chief Operations Officer")
				{
					isDepartment = true;
				}
			}
			else if (vmSFN18795.RequisitionModel.ReqItemsTotal <= 500.00)
			{
				// If the Submitter is the Director then the Director of finance will approve
				if (employeeJobTitle == "Director" && jobtitle == "Director of Finance")
				{
					isSupervisor = true;
				}
				// If the submitter is a Division Chief the COO will approve
				else if ((employeeJobTitle == "Chief of Employer Services" || employeeJobTitle == "Chief of Injury Services") && jobtitle == "Chief Operations Officer")
				{
					isSupervisor = true;
				}
			}
			// Request under 500 Check if logged in user is supervisor of the submitter
			personelnames = DirectoryHelper.getPersonel(loggedInUser.SamAccountName);
			foreach (string person in personelnames)
			{
				string personName = DirectoryHelper.findName(person);

				if (personName == vmSFN18795.RequisitionModel.Name)
				{
					isSupervisor = true;
				}
			}

			ViewBag.isSupervisor = isSupervisor;
			ViewBag.isDepartment = isDepartment;
			ViewBag.isDirector = isDirector;
			ViewBag.isChief = isChief;
			ViewBag.TotalPrice = dtInfo.Rows[0]["REQ_ITEMS_TOTAL"].ToString();
			ViewBag.ID = id;

			return View("~/Views/Forms/SFN18795/Approval.cshtml", vmSFN18795);
		}

		/*
        * Details Page /SFN18795/Details/ID
        * Details page that can be viewed to check current status on approvals and print the page information
        * @param [string] id - ID number of the submitted form
        */
		public ActionResult Details(int id)
		{
			getConfigValues();
			setViewbagDataReturnTitle();
			string employeeDepartment;
			// Get form information by ID
			sfn18795Dal = new SFN18795ModelDal();
			DataTable dtInfo = sfn18795Dal.GetSFN18795InfoByID(id);
			// Set form information into different data models
			SFN18795DisplayViewModel vmSFN18795 = new SFN18795DisplayViewModel();
			vmSFN18795.RequisitionModel = SFN18795Model.ConvertDataTableToRequisition(dtInfo);
			vmSFN18795.RequisitionItems = SFN18795RequisitionItem.ConvertDataTableToRequisitionItemList(dtInfo);

			ViewBag.TotalPrice = dtInfo.Rows[0]["REQ_ITEMS_TOTAL"].ToString();
			ViewBag.ID = id;
			return View("~/Views/Forms/SFN18795/View.cshtml", vmSFN18795);
		}

		/*
        * Edit Page /SFN18795/Edit/ID
        * Edit page for forms that are saved but not completed, retrieves form information by ID number
        * @param [string] id - ID number of the submitted form
        */
		public ActionResult Edit(int id)
		{
			getConfigValues();
			setViewbagDataReturnTitle();
			// Get form information by ID
			sfn18795Dal = new SFN18795ModelDal();
			DataTable dtInfo = sfn18795Dal.GetSFN18795InfoByID(id);
			// Set form information into different data models
			SFN18795DisplayViewModel vmSFN18795 = new SFN18795DisplayViewModel();
			vmSFN18795.RequisitionModel = SFN18795Model.ConvertDataTableToRequisition(dtInfo);
			vmSFN18795.RequisitionItems = SFN18795RequisitionItem.ConvertDataTableToRequisitionItemList(dtInfo);

			ViewBag.TotalPrice = dtInfo.Rows[0]["REQ_ITEMS_TOTAL"].ToString();
			ViewBag.ID = id;
			return View("~/Views/Forms/SFN18795/Edit.cshtml", vmSFN18795);
		}

		/*
         * Myapprovals Page /SFN18795
         * List of forms that are waiting for User to approve
         * @param [String] sortorder - Defualt sortorder when pulled from the data base
        */
		public ActionResult MyApprovals(string sortOrder)
		{
			getConfigValues();
			setViewbagDataReturnTitle();
			// Get Table information for list
			sfn18795Dal = new SFN18795ModelDal();
			DataTable dtInfo = sfn18795Dal.GetSFN18795();
			// Set table information to data models
			SFN18795IndexViewModel lSFN18795 = new SFN18795IndexViewModel();
			lSFN18795.SFN18795s = SFN18795Model.GetSFN18795List(dtInfo);
			lSFN18795.RequisitionItems = SFN18795RequisitionItem.ConvertDataTableToRequisitionItemList(dtInfo);

			ViewBag.DateSubmitted = sortOrder == "date_desc" ? "date_asc" : "date_desc";
			// Check if there is a total price 
			if (dtInfo.Rows.Count > 0)
			{
				ViewBag.TotalPrice = dtInfo.Rows[0]["REQ_ITEMS_TOTAL"].ToString();
			}
			// Set default sort order
			switch (sortOrder)
			{
				default:
					lSFN18795.SFN18795s = lSFN18795.SFN18795s.OrderByDescending(i => i.DateSubmitted).ToList();
					break;
			}

			ViewBag.sortOrder = sortOrder;

			return View("~/Views/Forms/SFN18795/MyApprovals.cshtml", lSFN18795);
		}

		/*
         * Landing Page /SFN18795/ViewAllTeamMembers
         * List alls submissions from users team members
        */
		public ActionResult ViewAllTeamMembers(string sortOrder)
		{
			getConfigValues();
			setViewbagDataReturnTitle();
			List<string> personelnames = new List<string>();
			string[] isHierarchy;

			// Get Table information for list
			sfn18795Dal = new SFN18795ModelDal();
			DataTable dtInfo = sfn18795Dal.GetSFN18795();

			SFN18795IndexViewModel lSFN18795 = new SFN18795IndexViewModel();
			lSFN18795.SFN18795s = SFN18795Model.GetSFN18795List(dtInfo);
			lSFN18795.RequisitionItems = SFN18795RequisitionItem.ConvertDataTableToRequisitionItemList(dtInfo);
			// Set default sort list by lastest submit date
			ViewBag.DateSubmitted = sortOrder == "date_desc" ? "date_asc" : "date_desc";
			switch (sortOrder)
			{
				default:
					lSFN18795.SFN18795s = lSFN18795.SFN18795s.OrderByDescending(i => i.DateSubmitted).ToList();
					break;
			}
			// Check if there is a total price 
			if (dtInfo.Rows.Count > 0)
			{
				ViewBag.TotalPrice = dtInfo.Rows[0]["REQ_ITEMS_TOTAL"].ToString();
			}

			ViewBag.sortOrder = sortOrder;
			// Get User login information
			UserPrincipal loggedInUser = UserPrincipal.FindByIdentity(context, User.Identity.Name);
			string title = DirectoryHelper.getJobTitle(loggedInUser);
			personelnames = DirectoryHelper.getPersonel(loggedInUser.SamAccountName);
			if (title == "Director of Finance")
			{
				List<string> directors = DirectoryHelper.GetListOfAdUsersByProperty("nd", "Director", ADUserProperties.TITLE);
				isHierarchy = DirectoryHelper.inHierarchy(directors, loggedInUser.SamAccountName);
				string directorName = isHierarchy[0];
				foreach (string director in directors)
				{
					UserPrincipal directorPrincipal = UserPrincipal.FindByIdentity(context, director);
					if (directorPrincipal.DisplayName == directorName)
					{
						personelnames.Add(director);
					}
				}


			}
			ViewBag.teamMembers = personelnames;

			return View("~/Views/Forms/SFN18795/ViewAllTeamMembers.cshtml", lSFN18795);
		}

		#endregion

		#region Action_Functions
		/*
        * POST: Form Creation Page /SFN18795/Create
        * User submits a SFN18795 form that is sent to appropiate supervisor for approval
        * @param [FormCollection] collection - The information transfer from the form from the view
        * @param [string] command - submission value to determine action to take 
        */
		[HttpPost]
        public ActionResult Create(FormCollection collection, string command)
        {
			getConfigValues();
			// If command is Cancel return to index page
			if (command.Equals("Cancel"))
            {
                return RedirectToAction("Index");
            }

            string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + "/";
            string employee = collection["RequisitionModel.Name"];
            string employeetitle = "";
            string employeeEmail;
            string itEmployeeEmail = "";
            string[] approvalInfo;
            string approverEmail = "";
            string nextApproval = "";
            string itApprover = "";
            double totalPrice = 0.00;
			UserPrincipal requestingEmployee = UserPrincipal.FindByIdentity(context, User.Identity.Name);

			string requestingJobTitle = DirectoryHelper.getJobTitle(requestingEmployee);
            // If the total price is filled in parse double
            if (collection["RequisitionModel.TotalPrice"] != "")
            {
                totalPrice = double.Parse(collection["RequisitionModel.TotalPrice"]);
            }

            if (!ModelState.IsValid)
            {
                ModelState.Clear();

            }
            // When command is submit insert Form information to table and send notification email.
            if (command.Equals("Submit"))
            {
                bool formsub = true;
                // Insert information into the data table
                int sfn18795ID = sfn18795Dal.InsertSFN18795GetID(collection, formsub, nextApproval, User.Identity.Name);
                sfn18795Dal.InsertSFN18795ReqItems(collection, sfn18795ID, 0);
                // Get form information ID
                DataTable dtInfo = sfn18795Dal.GetSFN18795InfoByID(sfn18795ID);
                SFN18795DisplayViewModel vmSFN18795 = new SFN18795DisplayViewModel();
                vmSFN18795.RequisitionModel = SFN18795Model.ConvertDataTableToRequisition(dtInfo);string isITRequest = collection["RequisitionModel.SoftwareHardware"];
                // Check if IT request to determine if send first email to IT or first approval
                if (isITRequest.Equals("true"))
                {
					sendITRequestEmail(requestingJobTitle, collection["RequisitionModel.Name"], sfn18795ID, baseUrl);
                }
                else
                {
                    sendFirstApprovalEmail(totalPrice, requestingEmployee, collection["RequisitionModel.Name"], sfn18795ID, baseUrl);
                }
                return RedirectToAction("Index");
            }
            // When command is save insert form information into table
            else if (command.Equals("Save"))
            {
                bool formsub = false;
                // Insert information into the data table
                int sfn18795ID = sfn18795Dal.InsertSFN18795GetID(collection, formsub, nextApproval, User.Identity.Name);
                sfn18795Dal.InsertSFN18795ReqItems(collection, sfn18795ID, 0);

                return RedirectToAction("Edit", new { id = sfn18795ID });
            }
            return Content("error");
        }

        /*
         * Form Creation Page /SFN18795/Approval/id
         * Post approval to SFN52712_Approvals table and send emails notifing the next approver or if form has been approved
         * @param [int] id - form id number to pull information
         * @param [FormCollection] collection - The information transfer from the form from the view
         * @param [string] command - submission value to determine action to take 
         * @param [string] approver - approver value position
         * @param [string] denied - form denied email message to be sent to user
         * @param [string] revise - the revision message to be sent to email and to the database
         * @param [bool] ProcurementProcessing - check is form is being/been processed
        */
        [HttpPost]
        public ActionResult Approval(int id, FormCollection collection, string command, string approver, string denied, string revise, bool ProcurementProcessing)
        {
			getConfigValues();
			// If command is Cancel return to index page
			if (command.Equals("Cancel"))
            {
                return RedirectToAction("Index");
            }
            // Get form database information by form id
            sfn18795Dal = new SFN18795ModelDal();
            DataTable dtInfo = sfn18795Dal.GetSFN18795InfoByID(id);
            SFN18795DisplayViewModel vmSFN18795 = new SFN18795DisplayViewModel();
            vmSFN18795.RequisitionModel = SFN18795Model.ConvertDataTableToRequisition(dtInfo);
            // Set the requesting Employee by the employee id
            UserPrincipal requestingEmployee = UserPrincipal.FindByIdentity(context, vmSFN18795.RequisitionModel.EmplID);

            string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + "/";
            string employeeName = collection["RequisitionModel.Name"];
            string employeetitle;
            string employeeEmail;
            string itEmployeeEmail;
            string[] approvalInfo;
            string approverEmail = "";
            string approverName = "";
            string manager = "";
            string managerTitle = "";
            List<string> chief = new List<string>();
            string chiefName = "";
            string chiefTitle = "";
            string chiefEmail = "";
            string directorName = "";
            string directorEmail = "";
            string[] isHierarchy;
            double totalPrice = double.Parse(collection["RequisitionModel.TotalPrice"]);
            string from = "wsinoreply@nd.gov";
            string approvedSubject = "A Requisition Form requires your Approval";
            string declineSubject = "Your Requisition Form has been denied.";
            string reviseSubject = "Your Requisition Form needs to be revised";
            string approvedBody = "Please review the Requisition request submitted by " + collection["RequisitionModel.Name"] +
                " <a href=\"" + baseUrl + "SFN18795/Approval/" + id + "\">Requisition Form Link " + id + "</a>";
            string declinedBody = "Your Requisition form has been denied: " + denied + " <a href=\"" + baseUrl + "SFN18795/Approval/" + id + "\">Requisition Form Link " + id + "</a>";
            string reviseBody = "The Procurement Officer has requested your form be revised: " + revise + "Click here to revise and resubmit your form: <a href=\"" + baseUrl + "SFN18795/Edit/" + id + "\">Requisition Form Link " + id + "</a>";

            // Get Submitter information
            employeeEmail = DirectoryHelper.getEmail(requestingEmployee.SamAccountName);
            employeetitle = DirectoryHelper.getJobTitle(requestingEmployee);
            // Check if the submitter is the director if not, find the submitters manager
            if (employeetitle != "Director")
            {
                manager = DirectoryHelper.getManager(requestingEmployee);
                UserPrincipal employeeManager = UserPrincipal.FindByIdentity(context, manager);
                managerTitle = DirectoryHelper.getJobTitle(employeeManager);
            }

            if (!ModelState.IsValid)
            {
                ModelState.Clear();
            }

			int amountSection = getAmountSection(totalPrice);

			switch (command)
            {
                case "Revise":
					dal.SendEmail(employeeEmail, from, reviseSubject, reviseBody);
					sfn18795Dal.updateSFN18795Revise(id, revise);
					return RedirectToAction("MyApprovals");
                case "Process":
					sfn18795Dal.updateSFN18795ProcurementProcess(id, collection, ProcurementProcessing);
					return RedirectToAction("Index");
                case "Submit":
					sfn18795Dal.UpdateSFN18795Approval(id, collection, approver);
					// Check if form is denied or approve send appropiate email.
					if (!denied.IsEmpty())
					{
						dal.SendEmail(employeeEmail, from, declineSubject, declinedBody);
						sfn18795Dal.updateSFN18795Denied(id);
						return RedirectToAction("MyApprovals");
					}
					//Check if the current user is Approving IT request and find the first approver.
					else if (approver.Equals("itrep"))
					{
						if (employeetitle == "Technical Services Lead Specialist" && totalPrice <= 500)
						{
							collection["RequisitionModel.SupervisorSignature"] = collection["RequisitionModel.ITRepSignature"];
							collection["RequisitionModel.SupervisorSignatureDate"] = collection["RequisitionModel.ITRepSignatureDate"];
							sfn18795Dal.UpdateSFN18795Approval(id, collection, "supervisor");
							sendProcurementApprovedEmails(id, employeeName, employeeEmail);
						}
						else
						{
							// If the IT request is approve find the first person to approve after it request.
							approvalInfo = getFirstApproverEmail(totalPrice, requestingEmployee);
							approverEmail = approvalInfo[0];
							approverName = approvalInfo[1];

							dal.SendEmail(approverEmail, from, approvedSubject, approvedBody);
							sfn18795Dal.updateSFN18795WaitingApproval(id, approverName);
						}
					}
					else if (approver.Equals("procurement"))
					{
						sfn18795Dal.updateSFN18795ProcurementProcess(id, collection, ProcurementProcessing);
						return RedirectToAction("Details", new { id = id });
					}
					// Determine who needs to approve the form based on the amount section and employee title.
					switch (amountSection)
					{
						case 1:
						case 2:
							if (approver.Equals("supervisor") || approver.Equals("department"))
							{
								sendProcurementApprovedEmails(id, employeeName, employeeEmail);
							}
							break;
                        case 3:
							if (approver.Equals("department"))
							{
								chiefTitle = DirectoryHelper.findChief(requestingEmployee);
								chief = DirectoryHelper.GetListOfAdUsersByProperty("nd", chiefTitle, ADUserProperties.TITLE);
								isHierarchy = DirectoryHelper.inHierarchy(chief, requestingEmployee.SamAccountName);
								chiefName = isHierarchy[0];
								chiefEmail = isHierarchy[1];

								dal.SendEmail(chiefEmail, from, approvedSubject, approvedBody);
								sfn18795Dal.updateSFN18795WaitingApproval(id, chiefName);
							}
							else if (approver.Equals("chief"))
							{
								sendProcurementApprovedEmails(id, employeeName, employeeEmail);
							}
							break;
                        case 4:
							List<string> director = DirectoryHelper.GetListOfAdUsersByProperty("nd", "Director", ADUserProperties.TITLE);
							isHierarchy = DirectoryHelper.inHierarchy(director, requestingEmployee.SamAccountName);
							directorName = isHierarchy[0];
							directorEmail = isHierarchy[1];

							if (approver.Equals("department"))
							{
								dal.SendEmail(directorEmail, from, approvedSubject, approvedBody);
								sfn18795Dal.updateSFN18795WaitingApproval(id, directorName);
							}
							else if (approver.Equals("chief"))
							{
								if (employeetitle.Equals("Director"))
								{
									chief = DirectoryHelper.GetListOfAdUsersByProperty("nd", "Chief Operations Officer", ADUserProperties.TITLE);
									isHierarchy = DirectoryHelper.inHierarchy(chief, requestingEmployee.SamAccountName);
									chiefName = isHierarchy[0];
									chiefEmail = isHierarchy[1];

									dal.SendEmail(chiefEmail, from, approvedSubject, approvedBody);
									sfn18795Dal.updateSFN18795WaitingApproval(id, chiefName);
								}
								else
								{
									dal.SendEmail(directorEmail, from, approvedSubject, approvedBody);
									sfn18795Dal.updateSFN18795WaitingApproval(id, directorName);
								}
							}
							else if (approver.Equals("director"))
							{
								sendProcurementApprovedEmails(id, employeeName, employeeEmail);
							}
							break;
                    }
					return RedirectToAction("MyApprovals");
				default:
                    break;
            }
            return Content("error");
        }

		/*
        * POST: Form Edit Page /SFN18795/Edit/ID
        * User submits a SFN18795 form by ID, that is sent to appropiate supervisor for approval
        * @param [string] id - ID number of the submitted form
        * @param [FormCollection] collection - The information transfer from the form from the view
        * @param [string] command - submission value to determine action to take 
        */
		[HttpPost]
        public ActionResult Edit(int id, FormCollection collection, string command)
        {
			getConfigValues();
			// If cancel command is selected immediatly return to index page
			if (command.Equals("Cancel"))
            {
                return RedirectToAction("Index");
            }
            string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + "/";
            string employeeEmail;
            string[] approvalInfo;
            string approverEmail;
            string nextApproval = "";
            string itApproval = "";
            double totalPrice = double.Parse(collection["RequisitionModel.TotalPrice"]);
            // Pull data table information for form to check the item count and ammount
            DataTable dtInfo = sfn18795Dal.GetSFN18795InfoByID(id);
            SFN18795DisplayViewModel vmSFN18795 = new SFN18795DisplayViewModel();
            vmSFN18795.RequisitionModel = SFN18795Model.ConvertDataTableToRequisition(dtInfo);
            vmSFN18795.RequisitionItems = SFN18795RequisitionItem.ConvertDataTableToRequisitionItemList(dtInfo);
            int count = vmSFN18795.RequisitionItems.Count;
            string[] items = collection.GetValues("RequisitionItem.Quantity");
            // Get requesting employee information
            UserPrincipal requestingEmployee = UserPrincipal.FindByIdentity(context, User.Identity.Name);
            string requestingJobTitle = DirectoryHelper.getJobTitle(requestingEmployee);

			// If dates are blank auto set dates to defualt to avoid errors
			if (collection["RequisitionModel.EstimatedStartDate"] == "")
			{
				collection["RequisitionModel.EstimatedStartDate"] = "1111-01-01";
			}
			if (collection["RequisitionModel.EstimatedCompleteDate"] == "")
			{
				collection["RequisitionModel.EstimatedCompleteDate"] = "1111-01-01";
			}

			if (!ModelState.IsValid)
            {
                ModelState.Clear();

            }
            // Check command select to determine action, Submit will send emails save will only update database
            if (command.Equals("Submit"))
            {
                bool formsub = true;
                string isITRequest = collection["RequisitionModel.SoftwareHardware"];
				// Check if the request is an IT request and get IT personel information
				// Check if IT request to determine if send first email to IT or first approval
				if (isITRequest.Equals("true"))
                {
					itApproval = sendITRequestEmail(requestingJobTitle, collection["RequisitionModel.Name"], id, baseUrl);
					sfn18795Dal.UpdateSFN18795(collection, id, formsub, itApproval);
                }
                else
                {
					nextApproval = sendFirstApprovalEmail(totalPrice, requestingEmployee, collection["RequisitionModel.Name"], id, baseUrl);
					sfn18795Dal.UpdateSFN18795(collection, id, formsub, nextApproval);
                }
                // Check the item list length and if the count has changed and update the items
                if (items.Length < count)
                {
                    sfn18795Dal.DeleteSFN18795Items(id, count, items.Length);
                    sfn18795Dal.updateSFN18795ReqItems(id, collection, vmSFN18795, items.Length);

                }
                else
                {
                    sfn18795Dal.updateSFN18795ReqItems(id, collection, vmSFN18795, count);
                    sfn18795Dal.InsertSFN18795ReqItems(collection, id, count);
                }

                return RedirectToAction("Index");
            }
            else if (command.Equals("Save"))
            {
                bool formsub = false;
                string isITRequest = collection["RequisitionModel.SoftwareHardware"];
                // If form has been update save new form information
                if (isITRequest.Equals("true"))
                {
                    sfn18795Dal.UpdateSFN18795(collection, id, formsub, itApproval);
                }
                else
                {
                    sfn18795Dal.UpdateSFN18795(collection, id, formsub, nextApproval);
                }
				// Check the item list length and if the count has changed and update the items
				if (items.Length < count)
                {
                    sfn18795Dal.DeleteSFN18795Items(id, count, items.Length);
                    sfn18795Dal.updateSFN18795ReqItems(id, collection, vmSFN18795, items.Length);

                }
                else
                {
                    sfn18795Dal.updateSFN18795ReqItems(id, collection, vmSFN18795, count);
                    sfn18795Dal.InsertSFN18795ReqItems(collection, id, count);
                }

                return RedirectToAction("Edit", new { id = id });
            }
            return Content("error");
        }

		#endregion

		#region Support_Functions
		/*
        * Check if the user has any Team Members that report to them
        * @param [string] loggedInUser - users login user name
        */
		private static int checkUserPersonel(string loggedInUser)
        {
            List<string> personelnames = new List<string>();
            personelnames = DirectoryHelper.getPersonel(loggedInUser);
            return personelnames.Count;
        }

        private static int getAmountSection(double totalCost)
        {
			int amountSection = 1;

			// Check total cost to assign amount section for approvals.
			if (totalCost <= 500)
			{
				amountSection = 1;
			}
			else if (totalCost > 500 && totalCost <= 5000)
			{
				amountSection = 2;
			}
			else if (totalCost > 5000 && totalCost <= 50000)
			{
				amountSection = 3;
			}
			else if (totalCost > 50000)
			{
				amountSection = 4;
			}

			return amountSection;
		}

		/*
        *  Send email to the first person approval
        *  @param [string] employeeJobTitle - jobtitle of the requesting employee
        *  @param [string] employeeName - name of the requesting employee
        * @param [int] id - Form id number
        * @param [string] - the base URL of the send website link
        */
		private string sendFirstApprovalEmail(double totalPrice, UserPrincipal requestingEmployee, string employeeName, int id, string baseUrl )
        {
            string itApprover;
            string approverEmail;
            string nextApproval;
			string from = "wsinoreply@nd.gov";
			string subject = "A Requisition Form requires your Approval";
			string body = "Please review the Requisition request submitted by " + employeeName + " <a href=\"" + baseUrl + "SFN18795/Approval/" + id + "\">Requisition Form Link " + id + "</a>";
			
            // Getting the name and the email of the first person    
			string [] approvalInfo = getFirstApproverEmail(totalPrice, requestingEmployee);
			approverEmail = approvalInfo[0];
			nextApproval = approvalInfo[1];

			dal.SendEmail(approverEmail, from, subject, body);
			sfn18795Dal.updateSFN18795WaitingApproval(id, nextApproval);
            return nextApproval;
		}

		/*
        *  Send email to IT for IT request approval
        *  @param [string] employeeJobTitle - jobtitle of the requesting employee
        *  @param [string] employeeName - name of the requesting employee
        * @param [int] id - Form id number
        * @param [string] - the base URL of the send website link
        */
		private string sendITRequestEmail(string employeeJobTitle, string employeeName, int id, string baseUrl)
		{
			string itApprover;
			string itEmployeeEmail;
			string nextApproval;
			string from = "wsinoreply@nd.gov";
			string subject = "A Requisition Form requires your Approval";
			string body = "Please review the Requisition request submitted by " + employeeName + " <a href=\"" + baseUrl + "SFN18795/Approval/" + id + "\">Requisition Form Link " + id + "</a>";

			// Get IT Lead Specialist for IT request approval
			if (employeeJobTitle != "Technical Services Lead Specialist")
			{
				var technicalServeList = DirectoryHelper.GetListOfAdUsersByProperty("nd", "Technical Services Lead Specialist", ADUserProperties.TITLE);
				itApprover = technicalServeList[0];
			}
			else
			{
				var helpDeskSuperList = DirectoryHelper.GetListOfAdUsersByProperty("nd", "Application Services/Help Desk Supervisor", ADUserProperties.TITLE);
				itApprover = helpDeskSuperList[0];
			}

			UserPrincipal itEmployee = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, itApprover);
			itEmployeeEmail = itEmployee.EmailAddress;
			nextApproval = itEmployee.DisplayName;

			dal.SendEmail(itEmployeeEmail, from, subject, body);
			sfn18795Dal.updateSFN18795WaitingApproval(id, nextApproval);
			return nextApproval;
		}

		/*
        *  Send procurement email and email to employee that form has been approved
        * @param [int] id - Form id number
        */
		private void sendProcurementApprovedEmails(int id, string employeeName, string employeeEmail){
			string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + "/";
			// Get the Procurement Officer Information
			string procurementOfficer = DirectoryHelper.GetListOfAdUsersByProperty("nd", "Procurement Officer", ADUserProperties.TITLE, _filter)[0];
			string procurementOfficerName = DirectoryHelper.findName(procurementOfficer);
			string procurementOfficerEmail = DirectoryHelper.getEmail(procurementOfficer);

			string from = "wsinoreply@nd.gov";
			string approvedSubject = "A Requisition Form requires your Approval";
			string completeSubject = "Your Requisition Form has been Approved";

			string approvedBody = "Please review the Requisition request submitted by " + employeeName +
				" <a href=\"" + baseUrl + "SFN18795/Approval/" + id + "\">Requisition Form Link " + id + "</a>";
            string completeBody = "Your requisition request has been approved. " +
				" <a href=\"" + baseUrl + "/SFN18795/Details/" + id + "\">Requisition Form Link " + id + "</a>";

			dal.SendEmail(procurementOfficerEmail, from, approvedSubject, approvedBody);
			dal.SendEmail(employeeEmail, from, completeSubject, completeBody);
			sfn18795Dal.updateSFN18795WaitingApproval(id, procurementOfficerName);
			sfn18795Dal.updateSFN18795FormComplete(id);
		}

		/*
         * Sets the used Viewbag infomation that needs to be passed to each page
         */
		private string setViewbagDataReturnTitle()
		{
			List<string> personelnames = new List<string>();
			// Get form header information 
			ViewBag.FormInfo = dal.GetFormInfo("SFN18795");
			ViewBag.User = User.Identity.Name;

			// Get log in user's username
			UserPrincipal loggedInUser = UserPrincipal.FindByIdentity(context, User.Identity.Name);

			// Check is user is a manager
			personelnames = DirectoryHelper.getPersonel(loggedInUser.SamAccountName);
			string jobTitle = DirectoryHelper.getJobTitle(loggedInUser);

			ViewBag.UserName = loggedInUser.DisplayName;
			ViewBag.Email = loggedInUser.EmailAddress;
			ViewBag.Reportsto = personelnames;
			ViewBag.jobtitle = jobTitle;
			ViewBag.nonsuper = personelnames.Count;

			return jobTitle;
		}

		/*
        *  Get the first approval email based off of total and empoylee title
        * @param [double] totalCost - total amount of request requisisition
        * @param [UserPrincipal] employee - the submitting employee requesting requisisition
        */
		public string[] getFirstApproverEmail(double totalCost, UserPrincipal employee)
        {
			string approverEmail = "";
            string employeeTitle = DirectoryHelper.getJobTitle(employee);
			List<string> director;
			string chiefEmail = "";
            string chiefName = "";
            string departmentEmail = "";
            string departmentName = "";
            string[] isHierarchy;
			string[] approver = new string[2];

            // Get current COO email and name
			List<string> coo = DirectoryHelper.GetListOfAdUsersByProperty("nd", "Chief Operations Officer", ADUserProperties.TITLE);
			string[] iscooHierarchy = DirectoryHelper.inHierarchy(coo, employee.SamAccountName);
            string cooEmail = iscooHierarchy[1];
            string cooName = iscooHierarchy[0];

			int amountSection = getAmountSection(totalCost);

            // Determine who needs to approve the form based on the amount section and employee title.
            switch (amountSection)
            {
                case 1:
					if (employeeTitle.Equals("Chief of Employer Services") || employeeTitle.Equals("Chief of Injury Services") || employeeTitle.Equals("Director of Finance"))
					{
						approver[0] = cooEmail;
						approver[1] = cooName;
                        break;
					}
					else if(!employeeTitle.Equals("Chief of Employer Services") && !employeeTitle.Equals("Chief of Injury Services") && !employeeTitle.Equals("Director"))
					{
						string manager = DirectoryHelper.getManager(employee);
						approver[0] = DirectoryHelper.getEmail(manager);
						approver[1] = DirectoryHelper.findName(manager);
                        break;
					}
                    goto case 2;
				case 2:
				case 3:
					if (employeeTitle.Equals("Chief Operations Officer") || employeeTitle.Contains("Communications Director") || employeeTitle.Contains("Internal Audit Director") || employeeTitle.Contains("Decision Review Director")
					|| employeeTitle.Contains("Change Management Specialist") || employeeTitle.Contains("HR Director") || employeeTitle.Contains("Executive Assistant"))
					{
						int num = DirectoryHelper.GetListOfAdUsersByProperty("nd", "Director", ADUserProperties.TITLE).Count;
						director = DirectoryHelper.GetListOfAdUsersByProperty("nd", "Director", ADUserProperties.TITLE);
						string[] isdirectorHierarchy = DirectoryHelper.inHierarchy(director, employee.SamAccountName);
						string directorEmail = isdirectorHierarchy[1];
						string directorName = isdirectorHierarchy[0];

						approver[0] = directorEmail;
						approver[1] = directorName;
                        break;
					}
					if (employeeTitle.Equals("Chief of Employer Services") || employeeTitle.Equals("Chief of Injury Services") || employeeTitle.Equals("General Counsel") || employeeTitle.Equals("Director of Information Technology")
						 || employeeTitle.Equals("Director of Strategic Operations") || employeeTitle.Equals("Facilities Manager") || employeeTitle.Equals("Director of Finance"))
					{
						approver[0] = cooEmail;
						approver[1] = cooName;
                        break;
					}
					goto case 4;
				case 4:
					if (employeeTitle.Equals("Director") || employeeTitle.Equals("Chief Operations Officer"))
					{
						string finance = DirectoryHelper.GetListOfAdUsersByProperty("nd", "Director of Finance", ADUserProperties.TITLE, _filter)[0];
						string financeEmail = DirectoryHelper.getEmail(finance);
						string financeName = DirectoryHelper.findName(finance);

						approver[0] = financeEmail;
						approver[1] = financeName;
                        break;
					}
					else if (employeeTitle.Equals("Chief of Employer Services") || employeeTitle.Equals("Chief of Injury Services") || employeeTitle.Contains("Communications Director")
                        || employeeTitle.Contains("Internal Audit Director") || employeeTitle.Contains("Decision Review Director") || employeeTitle.Contains("Change Management Specialist")
                        || employeeTitle.Equals("Facilities Manager") || employeeTitle.Contains("HR Director") || employeeTitle.Contains("Executive Assistant") || employeeTitle.Contains("Director of Finance"))
					{
						approver[0] = cooEmail;
						approver[1] = cooName;
						break;
					}
					else if (employeeTitle.Contains("Director") || employeeTitle.Contains("Business Representative") || employeeTitle.Contains("Executive Support"))
					{
						string chiefTitle = DirectoryHelper.findChief(employee);
						List<string> chief = DirectoryHelper.GetListOfAdUsersByProperty("nd", chiefTitle, ADUserProperties.TITLE);
						isHierarchy = DirectoryHelper.inHierarchy(chief, employee.SamAccountName);
						chiefEmail = isHierarchy[1];
						chiefName = isHierarchy[0];

						approver[0] = chiefEmail;
						approver[1] = chiefName;
						break;
					}
					else
					{
						string departmentTitle = DirectoryHelper.findDepartmentDirector(employee);
						List<string> department = DirectoryHelper.GetListOfAdUsersByProperty("nd", departmentTitle, ADUserProperties.TITLE);
						isHierarchy = DirectoryHelper.inHierarchy(department, employee.SamAccountName);
						departmentEmail = isHierarchy[1];
						departmentName = isHierarchy[0];

						approver[0] = departmentEmail;
						approver[1] = departmentName;
						break;
					}
				default:
                    break;
            }

            return approver;
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
		#endregion

	}
}