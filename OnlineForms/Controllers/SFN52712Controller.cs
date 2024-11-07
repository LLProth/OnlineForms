using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineForms.ViewModels;
using OnlineForms.Models.SFN52712;
using OnlineForms.Models;
using OnlineForms.Helper;
using System.DirectoryServices.AccountManagement;
using System.Data;
using System.Web.WebPages;

namespace OnlineForms.Controllers
{
    [Authorize]
    public class SFN52712Controller : Controller
    {
        private ModelDAL dal = new ModelDAL();
        private SFN52712ModelDal sfn52712dal = new SFN52712ModelDal();

        private static PrincipalContext context = new PrincipalContext(ContextType.Domain);
        private static UserPrincipal u = new UserPrincipal(context);

		#region Content_Functions
		/*
         * Landing Page /SFN52712
         * List Users submissions or for Procurement officer list all submissions
        */
		public ActionResult Index()
		{
			// Set default sort order
			string sortOrder;
			ViewBag.DateSubmitted = sortOrder = "date_desc";

			// Set model get whole list of SFN52712 for database 
			sfn52712dal = new SFN52712ModelDal();
			DataTable dtInfo = sfn52712dal.GetSFN52712();
			SFN52712IndexViewmodel lsSFN52712 = new SFN52712IndexViewmodel();
			lsSFN52712.SFN52712s = SFN52712Model.GetSFN52712List(dtInfo);
			lsSFN52712.SFN52712Destinations = SFN52712DestinationModel.ConvertDataTableDestinationList(dtInfo);
			// Set loading page data
			setViewbagDataReturnTitle();

			// Set initial order of database information
			switch (sortOrder)
			{
				default:
					lsSFN52712.SFN52712s = lsSFN52712.SFN52712s.OrderByDescending(i => i.CreationDate).ToList();
					break;
			}

			ViewBag.sortOrder = sortOrder;
			return View("~/Views/Forms/SFN52712/Index.cshtml", lsSFN52712);
		}

		/*
         * Form Creation Page /SFN52712/Create
         * User submits a SFN52712 form that is sent to their supervisors for approval
        */
		public ActionResult Create()
		{
			// Set loading page data
			setViewbagDataReturnTitle();
			setSelectListValues();

			return View("~/Views/Forms/SFN52712/Create.cshtml");
		}

		/*
         * Form Creation Page /SFN52712/Edit/id
         * Pulls form information based on the Id number
         * User submits or saves a SFN52712 form that is sent to their supervisors for approval
         * @param [int] id - form id number to pull information
        */
		public ActionResult Edit(int id)
		{
			// Get form flight information by the id number
			DataTable dtInfo = sfn52712dal.GetSFN52712ById(id);
			DataTable flightInfo = sfn52712dal.GetSFN52712FlightInfo(id);
			// Check if flight information exist
			if (flightInfo.Rows.Count > 0)
			{
				ViewBag.FlightExist = true;
			}
			else
			{
				ViewBag.FlightExist = false;
			}
			// Check if method of travel is other and pull and seperate other comment information
			if (dtInfo.Rows[0]["METHOD_OF_TRAVEL"].ToString().Contains("Other (Explain)"))
			{
				ViewBag.OtherMethodofTravel = dtInfo.Rows[0]["METHOD_OF_TRAVEL"].ToString().Replace("Other (Explain) ", "");
				dtInfo.Rows[0]["METHOD_OF_TRAVEL"] = "Other (Explain)";
			}
			// Get form information by the id number
			SFN52712ViewModel lsSFN52712 = new SFN52712ViewModel();
			lsSFN52712.TravelAuthoriztionModel = SFN52712Model.GetSFN52712(dtInfo);
			lsSFN52712.SFN52712Destinations = SFN52712DestinationModel.ConvertDataTableDestinationList(dtInfo);
			if (flightInfo.Rows.Count > 0)
			{
				lsSFN52712.FlightInfo = SFN52712FlightMethod.ConvertDataTableFlightMethod(flightInfo);
			}
			// Set loading page data
			setViewbagDataReturnTitle();
			setSelectListValues();

			return View("~/Views/Forms/SFN52712/Edit.cshtml", lsSFN52712);
		}

		/*
         * Form Creation Page /SFN52712/Approval/id
         * Post approval to SFN52712_Approvals table and send emails notifing the next approver or if form has been approved
         * @param [int] id - form id number to pull information
        */
		public ActionResult Approval(int id)
		{
			ViewBag.ID = id;
			bool isSupervisor = false;
			bool isDirector = false;
			bool isDepartment = false;
			bool isChief = false;
			bool isDirectorSupervisor = false;
			bool isFinanceDirector = false;
			string employeeJobTitle;
			string employeeManager;
			string employeeManagerTitle = "";
			string employeeChief = "";
			string employeeDepartment = "";

			// Get form flight information by the id number
			DataTable dtInfo = sfn52712dal.GetSFN52712ById(id);
			DataTable flightInfo = sfn52712dal.GetSFN52712FlightInfo(id);
			DataTable approvalInfo = sfn52712dal.GetSFN52712Approvals(id);

			SFN52712ViewModel lsSFN52712 = new SFN52712ViewModel();
			lsSFN52712.TravelAuthoriztionModel = SFN52712Model.GetSFN52712(dtInfo);
			lsSFN52712.SFN52712Destinations = SFN52712DestinationModel.ConvertDataTableDestinationList(dtInfo);
			// Check if there is flight information 
			if (flightInfo.Rows.Count > 0)
			{
				ViewBag.FlightExist = true;
				lsSFN52712.FlightInfo = SFN52712FlightMethod.ConvertDataTableFlightMethod(flightInfo);
			}
			else
			{
				ViewBag.FlightExist = false;
			}
			// Check if Method of travel is other split explanation and Other (Explain) selection
			if (dtInfo.Rows[0]["METHOD_OF_TRAVEL"].ToString().Contains("Other (Explain)"))
			{
				ViewBag.OtherMethodofTravel = dtInfo.Rows[0]["METHOD_OF_TRAVEL"].ToString().Replace("Other (Explain) ", "");
				dtInfo.Rows[0]["METHOD_OF_TRAVEL"] = "Other (Explain)";
			}
			// Get form Approval information
			if (approvalInfo.Rows.Count > 0)
			{
				lsSFN52712.SFN52712Approvals = SFN52712Approval.ConvertDataTableApproval(approvalInfo);
			}
			// Get requesting Employee information and supervisor heirarchy
			UserPrincipal requestingEmployee = UserPrincipal.FindByIdentity(context, lsSFN52712.TravelAuthoriztionModel.Emplid);
			employeeJobTitle = DirectoryHelper.getJobTitle(requestingEmployee);
			employeeChief = DirectoryHelper.findChief(requestingEmployee);
			employeeDepartment = DirectoryHelper.findDepartmentDirector(requestingEmployee);
			// Check if the requesting Employees is the Director if not check for employee manager.
			if (employeeJobTitle != "Director")
			{
				employeeManager = DirectoryHelper.getManager(requestingEmployee);
			}
			else
			{
				employeeManager = "";
				employeeManagerTitle = "";
			}

			if (employeeManager != "")
			{
				UserPrincipal manager = UserPrincipal.FindByIdentity(context, employeeManager);
				employeeManagerTitle = DirectoryHelper.getJobTitle(manager);
				ViewBag.SuperVisor = employeeManagerTitle;
			}
			// Set loading page data and get jobtitle
			string jobTitle = setViewbagDataReturnTitle();

			// Check if current user can sign off for the form
			if (jobTitle == employeeManagerTitle)
			{
				isSupervisor = true;
			}
			if (jobTitle == employeeDepartment)
			{
				isSupervisor = false;
				isDepartment = true;
			}
			if (jobTitle == employeeChief)
			{
				isSupervisor = false;
				isChief = true;
			}
			if (employeeJobTitle.Equals("Director") && jobTitle.Contains("Director of Finance"))
			{
				isFinanceDirector = true;
			}
			if (jobTitle.Equals("Director") && !employeeJobTitle.Equals("Director"))
			{
				isSupervisor = false;
				isDepartment = false;
				isDirector = true;
			}

			ViewBag.isSupervisor = isSupervisor;
			ViewBag.isDepartment = isDepartment;
			ViewBag.isDirector = isDirector;
			ViewBag.isChief = isChief;
			ViewBag.isFinanceDirector = isFinanceDirector;

			return View("~/Views/Forms/SFN52712/Approval.cshtml", lsSFN52712);
		}

		/*
         * Form Creation Page /SFN52712/MyApprovals
         * Get list of all forms need my approval 
         * @param [string] sortorder - default sortorder of list
        */
		public ActionResult MyApprovals(string sortOrder)
		{
			// Get table information
			DataTable dtInfo = sfn52712dal.GetSFN52712();

			SFN52712IndexViewmodel lsSFN52712 = new SFN52712IndexViewmodel();
			lsSFN52712.SFN52712s = SFN52712Model.GetSFN52712List(dtInfo);
			lsSFN52712.SFN52712Destinations = SFN52712DestinationModel.ConvertDataTableDestinationList(dtInfo);
			lsSFN52712.SFN52712Approvals = SFN52712Approval.ConvertDataTableApprovalList(dtInfo);
			// Set default sorting
			ViewBag.DateSubmitted = sortOrder == "date_desc" ? "date_asc" : "date_desc";
			// Set loading page data
			setViewbagDataReturnTitle();

			return View("~/Views/Forms/SFN52712/MyApprovals.cshtml", lsSFN52712);
		}

		/*
		  * Form Creation Page /SFN52712/Detials/id
		  * Get form information from SFN52712 tables and Print the form for internal use
		  * @param [string] id - form id number to pull information
		 */
		public ActionResult Details(int id)
		{
			// Get table flight and approval information
			DataTable dtInfo = sfn52712dal.GetSFN52712ById(id);
			DataTable flightInfo = sfn52712dal.GetSFN52712FlightInfo(id);
			DataTable approvalInfo = sfn52712dal.GetSFN52712Approvals(id);
			// Check if there is any flight informoation for the form and to display the flight info.
			if (flightInfo.Rows.Count > 0)
			{
				ViewBag.FlightExist = true;
			}
			else
			{
				ViewBag.FlightExist = false;
			}
			// Check the method of travel is other, set method of travel to other and split explanation from selection
			if (dtInfo.Rows[0]["METHOD_OF_TRAVEL"].ToString().Contains("Other (Explain)"))
			{
				ViewBag.OtherMethodofTravel = dtInfo.Rows[0]["METHOD_OF_TRAVEL"].ToString().Replace("Other (Explain) ", "");
				dtInfo.Rows[0]["METHOD_OF_TRAVEL"] = "Other (Explain)";
			}
			// Set form information from table information
			SFN52712ViewModel lsSFN52712 = new SFN52712ViewModel();
			lsSFN52712.TravelAuthoriztionModel = SFN52712Model.GetSFN52712(dtInfo);
			lsSFN52712.SFN52712Destinations = SFN52712DestinationModel.ConvertDataTableDestinationList(dtInfo);
			if (approvalInfo.Rows.Count > 0)
			{
				lsSFN52712.SFN52712Approvals = SFN52712Approval.ConvertDataTableApproval(approvalInfo);
			}
			if (flightInfo.Rows.Count > 0)
			{
				lsSFN52712.FlightInfo = SFN52712FlightMethod.ConvertDataTableFlightMethod(flightInfo);
			}
			// Set loading page data
			setViewbagDataReturnTitle();

			return View("~/Views/Forms/SFN52712/Details.cshtml", lsSFN52712);
		}

		#endregion

		#region Action_Functions
		/*
         * Form Creation Page /SFN52712/Create
         * Post form information to the SFN52712 table and send emails notifing supervisors of need approval
         * @param [FormCollection] collection - The information transfer from the form from the view
         * @param [string] command - submission value to determine action to take 
        */
		[HttpPost]
        public ActionResult Create(FormCollection collection, string command)
        {
            // If command is cancel immediately return to Index page
			if (command == "Cancel")
			{
				return RedirectToAction("Index");
			}
            string employeeManager = "";
            string managerEmail = "";
            string managerName = "";
            string jobTitle = "";
            string financeDirector = DirectoryHelper.GetListOfAdUsersByProperty("nd", "Director of Finance", ADUserProperties.TITLE)[1];
			// Check if method of travel is set to other
			if (collection["TravelAuthoriztionModel.MethodOfTravel"].Contains("Other"))
            {
                collection["TravelAuthoriztionModel.MethodOfTravel"] = "Other (Explain) " + collection["OtherTravel"];
            }
			// Get log in user's username
			UserPrincipal loggedInUser = UserPrincipal.FindByIdentity(context, User.Identity.Name);

			jobTitle = DirectoryHelper.getJobTitle(loggedInUser);
            // Check if user is the Director, which determines who will then be the first to approve the form.
            if (jobTitle.Equals("Director"))
            {
                managerName = DirectoryHelper.findName(financeDirector);
                managerEmail = DirectoryHelper.getEmail(financeDirector);
            }
            else
            {
                employeeManager = DirectoryHelper.getManager(loggedInUser);
                managerEmail = DirectoryHelper.getEmail(employeeManager);
                managerName = DirectoryHelper.findName(employeeManager);
            }

            if (!ModelState.IsValid)
            {
                ModelState.Clear();
                return View();

            }
			
			if (command == "Submit")
            {
                bool formsub = true;
                // Insert into form information into the database
                int sfn52712Id = sfn52712dal.InsertSFN52712GetID(collection, formsub, managerName, User.Identity.Name);
                sfn52712dal.InsertSFN52712TravelerSignature(collection, sfn52712Id);
                sfn52712dal.InsertSFN52712Destination(collection, sfn52712Id, 0);
                // Check if method of travel is Commercial Air then insert the information into the database
                if (collection["TravelAuthoriztionModel.MethodOfTravel"].Contains("Commercial Air"))
                {
                    sfn52712dal.InsertSFN52712FlightInfo(collection, sfn52712Id);
                }

                sendNextApprovalEmail(managerEmail, sfn52712Id);

                return RedirectToAction("Index");
            }
            else if (command == "Save")
            {
                bool formsub = false;
				// Insert into form information into the database
				int sfn52712Id = sfn52712dal.InsertSFN52712GetID(collection, formsub, managerName, User.Identity.Name);
                sfn52712dal.InsertSFN52712Destination(collection, sfn52712Id, 0);
				// Check if method of travel is Commercial Air then insert the information into the database
				if (collection["TravelAuthoriztionModel.MethodOfTravel"].Contains("Commercial Air"))
                {
                    sfn52712dal.InsertSFN52712FlightInfo(collection, sfn52712Id);
                }

                return RedirectToAction("Edit", new { id = sfn52712Id });
            }
            return Content("error");
        }		

		/*
         * Form Creation Page /SFN52712/Edit/id
         * Get information from SFN52712 and Post form information to the SFN52712 table and send emails notifing supervisors of need approval
         * @param [int] id - form id number to pull information
         * @param [FormCollection] collection - The information transfer from the form from the view
         * @param [string] command - submission value to determine action to take 
        */
		[HttpPost]
        public ActionResult Edit(int id, FormCollection collection, string command)
        {
			// If command is cancel immediately return to Index page
			if (command == "Cancel")
			{
				return RedirectToAction("Index");
			}
            string employeeManager = "";
            string managerEmail = "";
            string managerName = "";
            string jobTitle = "";
            string financeDirector = DirectoryHelper.GetListOfAdUsersByProperty("nd", "Director of Finance", ADUserProperties.TITLE)[1];
			// Check if method of travel is set to other
			if (collection["TravelAuthoriztionModel.MethodOfTravel"].Contains("Other"))
            {
                collection["TravelAuthoriztionModel.MethodOfTravel"] = "Other (Explain) " + collection["OtherTravel"];
            }
			// Get log in user's username
			UserPrincipal loggedInUser = UserPrincipal.FindByIdentity(context, User.Identity.Name);
            jobTitle = DirectoryHelper.getJobTitle(loggedInUser);
			// Check if user is the Director, which determines who will then be the first to approve the form.
			if (jobTitle.Equals("Director"))
            {
                managerName = DirectoryHelper.findName(financeDirector);
                managerEmail = DirectoryHelper.getEmail(financeDirector);
            }
            else
            {
                employeeManager = DirectoryHelper.getManager(loggedInUser);
                managerEmail = DirectoryHelper.getEmail(employeeManager);
                managerName = DirectoryHelper.findName(employeeManager);
            }

            // Get database information to see how many rows. 
            DataTable dtInfo = sfn52712dal.GetSFN52712ById(id);
            DataTable flightInfo = sfn52712dal.GetSFN52712FlightInfo(id);

            SFN52712ViewModel lsSFN52712 = new SFN52712ViewModel();
            lsSFN52712.TravelAuthoriztionModel = SFN52712Model.GetSFN52712(dtInfo);
            lsSFN52712.SFN52712Destinations = SFN52712DestinationModel.ConvertDataTableDestinationList(dtInfo);

            int count = lsSFN52712.SFN52712Destinations.Count;
            if (command == "Submit")
            {
                bool formsub = true;
                // Update and insert the form informatoin into the database base on the ID number
                sfn52712dal.updateSFN52712ByID(collection, formsub, id, managerName);
                sfn52712dal.InsertSFN52712TravelerSignature(collection, id);
                // Check desitination list and add or delete if more or less then previously saved.
                if (collection.GetValues("SFN52712Destination.City").Length < count)
                {

                    sfn52712dal.DeleteSFN52712Destinations(id, count, collection.GetValues("SFN52712Destination.City").Length);
                    sfn52712dal.UpdateSFN52712DestinationById(collection, id, lsSFN52712, collection.GetValues("SFN52712Destination.City").Length);
                }
                else
                {
                    sfn52712dal.UpdateSFN52712DestinationById(collection, id, lsSFN52712, count);
                    sfn52712dal.InsertSFN52712Destination(collection, id, count);
                }
                // Check if method of travel is Commercial Air and either insert or update 
                if (collection["TravelAuthoriztionModel.MethodOfTravel"].Contains("Commercial Air"))
                {
                    if (flightInfo.Rows.Count > 0)
                    {
                        sfn52712dal.UpdateSFN52712FlightInfoById(collection, id);
                    }
                    else
                    {
                        sfn52712dal.InsertSFN52712FlightInfo(collection, id);
                    }
                }

				sendNextApprovalEmail(managerEmail, id);

				return RedirectToAction("Index");
            }
            else if (command == "Save")
            {
                bool formsub = false;
				// Update and insert the form informatoin into the database base on the ID number
				sfn52712dal.updateSFN52712ByID(collection, formsub, id, managerName);
                if (collection.GetValues("SFN52712Destination.City").Length < count)
                {
                    sfn52712dal.DeleteSFN52712Destinations(id, count, collection.GetValues("SFN52712Destination.City").Length);
                    sfn52712dal.UpdateSFN52712DestinationById(collection, id, lsSFN52712, collection.GetValues("SFN52712Destination.City").Length);
                }
                else
                {
                    sfn52712dal.UpdateSFN52712DestinationById(collection, id, lsSFN52712, count);
                    sfn52712dal.InsertSFN52712Destination(collection, id, count);
                }
				// Check if method of travel is Commercial Air then insert the information into the database
				if (collection["TravelAuthoriztionModel.MethodOfTravel"].Contains("Commercial Air"))
                {
                    if (flightInfo.Rows.Count > 0)
                    {
                        sfn52712dal.UpdateSFN52712FlightInfoById(collection, id);
                    }
                    else
                    {
                        sfn52712dal.InsertSFN52712FlightInfo(collection, id);
                    }
                }

                return RedirectToAction("Edit");
            }
            return Content("error");
        }

		/*
         * Form Creation Page /SFN52712/Approval/id
         * Post approval to SFN52712_Approvals table and send emails notifing the next approver or if form has been approved
         * @param [int] id - form id number to pull information
         * @param [FormCollection] collection - The information transfer from the form from the view
         * @param [string] command - submission value to determine action to take 
         * @param [string] approver - approver value position
         * @param [string] denied - form denied email message to be sent to user
         * @param [bool] ProcurementProcessing - check is form is being/been processed
        */
		[HttpPost]
        public ActionResult Approval(int id, FormCollection collection, string command, string approver, string denied, bool ProcurementProcessing)
        {
            // If submission is canceled return to MyApprovals page. 
            if (command.Equals("Cancel"))
			{
				return RedirectToAction("MyApprovals");
			}
            // Get form information from ID
			DataTable dtInfo = sfn52712dal.GetSFN52712ById(id);
            DataTable approvalInfo = sfn52712dal.GetSFN52712Approvals(id);

            SFN52712ViewModel lsSFN52712 = new SFN52712ViewModel();
            lsSFN52712.TravelAuthoriztionModel = SFN52712Model.GetSFN52712(dtInfo);
            if (approvalInfo.Rows.Count > 0)
            {
                lsSFN52712.SFN52712Approvals = SFN52712Approval.ConvertDataTableApproval(approvalInfo);
            }

            string jobtitle = "";
            string manager = "";
            string managerTitle = "";
            string employeeManager = "";
			string employeeEmail = lsSFN52712.TravelAuthoriztionModel.Email;
            string employeeName = lsSFN52712.TravelAuthoriztionModel.Name;
			string nextapproverEmail = "";
            string nextApproverName = "";
            string nextapproverTitle = "";

            // If Signature is being submitted send emails and update database 
            if (command.Equals("Submit"))
            {
				UserPrincipal requestingEmployee = UserPrincipal.FindByIdentity(context, lsSFN52712.TravelAuthoriztionModel.Emplid);

				jobtitle = DirectoryHelper.getJobTitle(requestingEmployee);
				manager = DirectoryHelper.getManager(requestingEmployee);
				string userid = DirectoryHelper.getUserID(requestingEmployee);
                				
                // Check if requsting user is the director if not will find manager and next approver
				if (jobtitle != "Director")
				{
					UserPrincipal managerUser = UserPrincipal.FindByIdentity(context, manager);
					manager = managerUser.DisplayName;
					managerTitle = DirectoryHelper.getJobTitle(managerUser);

					UserPrincipal loggedInUser = UserPrincipal.FindByIdentity(context, User.Identity.Name);
					string nextapprover = DirectoryHelper.getManager(loggedInUser);
					string title = DirectoryHelper.getJobTitle(loggedInUser);

                    // Check if logged in user is Director if not will find next approver for form.
					if (title != "Director")
					{
						UserPrincipal nextapproverUser = UserPrincipal.FindByIdentity(context, nextapprover);
						nextapproverTitle = DirectoryHelper.getJobTitle(nextapproverUser);
						// If next person to approve is the Chief Operations Officer skip to the Director
						if (nextapproverTitle.Equals("Chief Operations Officer"))
						{
							nextapprover = DirectoryHelper.findDirector(loggedInUser)[1];
						}
						nextapproverEmail = DirectoryHelper.getEmail(nextapprover);
						nextApproverName = DirectoryHelper.findName(nextapprover);
					}
				}
                // Check which position is approving the form in order to check who the form needs to go to next
				if (approver == "supervisor")
                {
                    sfn52712dal.UpdateSFN52712SupervisorSignature(collection, id, collection["SFN52712Approvals.SupervisorSignature"], collection["SFN52712Approvals.SupervisorSignatureDate"], nextApproverName);
                }
                else if (approver == "department")
                {
                    sfn52712dal.UpdateSFN52712DepartmentSignature(collection, id, collection["SFN52712Approvals.DepartmentBudgetManagerSignature"], collection["SFN52712Approvals.DepartmentBudgetManagerSignatureDate"], nextApproverName);
                    // If the supervisor position was not filled becuase there is no supervisor insert the Department Director name into the supervisor as well
                    if (managerTitle.Contains("Director") && lsSFN52712.SFN52712Approvals.SupervisorSignature == "")
                    {
                        sfn52712dal.UpdateSFN52712SupervisorSignature(collection, id, collection["SFN52712Approvals.DepartmentBudgetManagerSignature"], collection["SFN52712Approvals.DepartmentBudgetManagerSignatureDate"], nextApproverName);
                    }
                }
                else if (approver == "chief")
                {
                    sfn52712dal.UpdateSFN52712ChiefSignature(collection, id, collection["SFN52712Approvals.DivisionChiefSignature"], collection["SFN52712Approvals.DivisionChiefSignatureDate"], nextApproverName);
					// If the supervisor position was not filled becuase there is no Department Director insert the Department Director name into the supervisor, and Department as well
					if (managerTitle.Contains("Chief") && lsSFN52712.SFN52712Approvals.SupervisorSignature == "")
                    {
                        sfn52712dal.UpdateSFN52712SupervisorSignature(collection, id, collection["SFN52712Approvals.DivisionChiefSignature"], collection["SFN52712Approvals.DivisionChiefSignatureDate"], nextApproverName);
                        sfn52712dal.UpdateSFN52712DepartmentSignature(collection, id, collection["SFN52712Approvals.DivisionChiefSignature"], collection["SFN52712Approvals.DivisionChiefSignatureDate"], nextApproverName);
                    }
                }
                else if (approver == "financedirector")
                {
                    sfn52712dal.UpdateSFN52712DirectorofFinanceSignature(collection, id);
                    sfn52712dal.UpdateSFN52712DirectorSignature(collection, id, collection["SFN52712Approvals.PersonTravelingSignature"], collection["SFN52712Approvals.PersonTravelingSignatureDate"]);
					sfn52712dal.UpdateSFN52712ApprovedbyId(id);
					// If form is not denied sends emails to super users
					if (!collection["SFN52712Approvals.PersonTravelingSignature"].Equals("Denied"))
                    {
                        sendCompletedEmail(employeeEmail, employeeName, id);
                    }
                }
                else if (approver == "director")
                {
                    sfn52712dal.UpdateSFN52712DirectorSignature(collection, id, collection["SFN52712Approvals.DirectorSignature"], collection["SFN52712Approvals.DirectorSignatureDate"]);
					sfn52712dal.UpdateSFN52712ApprovedbyId(id);
					// If the supervisor position was not filled becuase there is no chief insert the Department Director name into the supervisor, Department adn chief as well
					if (managerTitle.Equals("Director") && lsSFN52712.SFN52712Approvals.SupervisorSignature == "")
                    {
                        sfn52712dal.UpdateSFN52712SupervisorSignature(collection, id, collection["SFN52712Approvals.DirectorSignature"], collection["SFN52712Approvals.DirectorSignatureDate"], nextApproverName);
                        sfn52712dal.UpdateSFN52712DepartmentSignature(collection, id, collection["SFN52712Approvals.DirectorSignature"], collection["SFN52712Approvals.DirectorSignatureDate"], nextApproverName);
                        sfn52712dal.UpdateSFN52712ChiefSignature(collection, id, collection["SFN52712Approvals.DirectorSignature"], collection["SFN52712Approvals.DirectorSignatureDate"], nextApproverName);
                    }
					// If form is not denied sends emails to super users
					if (!collection["SFN52712Approvals.DirectorSignature"].Equals("Denied"))
                    {
                        sendCompletedEmail(employeeEmail, employeeName, id);
					}
                }
				// Check if form is denied or approve send appropiate email.
				if (denied.IsEmpty())
				{
					sendNextApprovalEmail(nextapproverEmail, id);
				}
				else
				{
					sfn52712dal.UpdateSFN52712DeniedbyId(id);
                    sendDeniedEmail(employeeEmail, denied, id);
				}

				return RedirectToAction("MyApprovals");
            }
            // Save command for super users can make notes before marking complete
            else if (command.Equals("Save"))
            {
                sfn52712dal.UpdateSFN52712ProcurementProcessById(collection, id, ProcurementProcessing);
                return RedirectToAction("Approval", id);
            }
            // Complete command for super users when form has been approve and is complete 
            else if (command.Equals("Complete"))
            {
                sfn52712dal.UpdateSFN52712ProcurementProcessById(collection, id, ProcurementProcessing);
                sfn52712dal.UpdateSFN52712CompletebyId(id);
                return RedirectToAction("MyApprovals");
            }

            return Content("Error");
        }
		#endregion

		#region Support_Functions
		/*
		  * Check if the user has any persnel under them
		  * @param [string] loggedInUser - users login user name
		 */
		private static int checkUserPersonel(string loggedInUser)
        {
			List<string> personelnames = new List<string>();
			personelnames = DirectoryHelper.getPersonel(loggedInUser);
			return personelnames.Count;
		}

		/*
         * Send email to the employee informing why it was denied
         */
		private void sendDeniedEmail(string requestingEmployeeEmail, string denied, int id)
		{
			string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + "/";
			string from = "wsinoreply@nd.gov";
			string deniedSubject = "An Authorization For Out Of State Travel Form has been denied";
			string deniedBody = "Your Authorization for Out of State Travel form has been denied: " + denied +
								  "< a href =\"" + baseUrl + "SFN52712/Approval/" + id + "\">Authorization for Out of State Travel Form Link</a>";

			// Send email to the manager
			dal.SendEmail(requestingEmployeeEmail, from, deniedSubject, deniedBody);
		}

		/*
         * Send email to the HR, Procurement, Executive Assistant and Employee that the form is complete and ready for review
         */
		private void sendCompletedEmail(string employeeEmail, string employeeName, int id) 
        {
			string hrDirector = DirectoryHelper.GetListOfAdUsersByProperty("nd", "HR Director", ADUserProperties.TITLE)[0];
			string hrDirectorEmail = DirectoryHelper.getEmail(hrDirector);

			string executiveAssitant = DirectoryHelper.GetListOfAdUsersByProperty("nd", "Executive Assistant", ADUserProperties.TITLE)[1];
			string executiveAssitantEmail = DirectoryHelper.getEmail(executiveAssitant);

			string procurementOfficer = DirectoryHelper.GetListOfAdUsersByProperty("nd", "Procurement Officer", ADUserProperties.TITLE)[0];
			string procurementOfficerEmail = DirectoryHelper.getEmail(procurementOfficer);

			string emails = hrDirectorEmail + "; " + executiveAssitantEmail + "; " + procurementOfficerEmail + "; " + employeeEmail;
			string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + "/";
			string from = "wsinoreply@nd.gov";
			string subject = employeeName + " - Authorization For Out Of State Travel Form - Approved";
			string body = "An authorization for Out of State Travel has been approved for " + employeeName + ". Please click here to access the form: " +
									 " < a href =\"" + baseUrl + "SFN52712/Approval/" + id + "\">Authorization for Out of State Travel Form Link</a>";

			// Send email to the manager
			dal.SendEmail(emails, from, subject, body);
		}

		/*
         * Send email to the next manager that needs to approve the form
         */
		private void sendNextApprovalEmail(string managerEmail, int id)
		{
			string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + "/";
			string from = "wsinoreply@nd.gov";
			string subject = "An Authorization For Out Of State Travel Form requires your Approval";
			string body = "An Authorization for Out of State Travel form has been submitted. Please click on the link below to access the form. " +
									 " < a href =\"" + baseUrl + "SFN52712/Approval/" + id + "\">Authorization for Out of State Travel Form Link</a>";

			// Send email to the manager
			dal.SendEmail(managerEmail, from, subject, body);
		}

		/*
         * Sets the used Viewbag infomation that needs to be passed to each page
         */
		private string setViewbagDataReturnTitle()
        {
			List<string> personelnames = new List<string>();
			// Get form header information 
			ViewBag.FormInfo = dal.GetFormInfo("SFN52712");
			ViewBag.User = User.Identity.Name;

			// Get log in user's username
			UserPrincipal loggedInUser = UserPrincipal.FindByIdentity(context, User.Identity.Name);
			// Check is user is a manager
			personelnames = DirectoryHelper.getPersonel(loggedInUser.SamAccountName);
			string jobTitle = DirectoryHelper.getJobTitle(loggedInUser);

			ViewBag.UserName = loggedInUser.DisplayName;
			ViewBag.Email = loggedInUser.EmailAddress;
			ViewBag.Reportsto = personelnames;
            ViewBag.jobTitle = jobTitle;
			ViewBag.nonsuper = personelnames.Count;

            return jobTitle;
		}

		/*
         * Set Departments and States dropdown list from database tables
         */
		private void setSelectListValues()
        {

			// Set Departments and States dropdown list from database tables
			ViewBag.Departments = ToSelectList(dal.getDepartments(), "Department", "Department");
			ViewBag.States = ToSelectList(dal.getStates(), "STATE_ID", "STATE_DESCRIPTION");
			ViewBag.MethodofTravel = new SelectList(
				new List<SelectListItem> {
					new SelectListItem { Value = "", Text =  ""},
					new SelectListItem { Value = "State Vehicle", Text =  "State Vehicle"},
					new SelectListItem { Value = "Personal Vehicle", Text =  "Personal Vehicle" },
					new SelectListItem { Value = "Commercial Air", Text =  "Commercial Air" },
					new SelectListItem { Value = "Other (Explain)", Text =  "Other (Explain)" }
				}, "Value", "Text");
			ViewBag.SeatPreference = new SelectList(
							new List<SelectListItem> {
					new SelectListItem { Value = "", Text =  ""},
					new SelectListItem { Value = "Window", Text =  "Window"},
					new SelectListItem { Value = "Aisle", Text =  "Aisle" },
					new SelectListItem { Value = "No Preference", Text =  "No Preference" }
							}, "Value", "Text");
		}

		/*
		  * Check if the user has any persnel under them
		  * @param [DataTable] tabel - the table that will be use to be set up for select list
		  * @param [string] valueField - match the value of the selection
		  * @param [string] textField -  the text for the value of the selection
		 */
		public static SelectList ToSelectList(DataTable table, string valueField, string textField)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            //Set each field value and match is with the textvalue in the select list with what is in the table.
            foreach (DataRow row in table.Rows)
            {
                list.Add(new SelectListItem()
                {
                    Text = row[textField].ToString(),
                    Value = row[valueField].ToString()
                });
            }
            return new SelectList(list, "Value", "Text");
        }
		#endregion
	}
}