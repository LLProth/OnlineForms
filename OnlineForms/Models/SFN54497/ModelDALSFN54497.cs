using Microsoft.Ajax.Utilities;
using OnlineForms.ViewModels;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using WSI.Utility.Database;

namespace OnlineForms.Models
{
    public class ModelDALSFN54497 : ModelDAL
    {
        public ModelDALSFN54497() : base()
        {
            dal = new DatabaseService("webformsdev");
        }

        //SFN54497
        public Dictionary<string, string> GetFormInfoSFN55497(string path)
        {
            string prcName = "PKG_SFN54497.PRC_SELECT_WEBFORM_BY_PATH";

            object[] prcParams = { path };
            string[] prcParamNames = { "a_path" };

            DataTable dtFormInfo = dal.ExecuteProcedureOutCursor(prcName, prcParamNames, prcParams, 1);

            Dictionary<string, string> dictFormInfo = new Dictionary<string, string>()
            {
                {"FormName", dtFormInfo.Rows[0]["SFN_TITLE"].ToString()},
                {"Division", dtFormInfo.Rows[0]["SFN_DIVISION"].ToString()},
                {"Info", dtFormInfo.Rows[0]["SFN"].ToString() + " " + dtFormInfo.Rows[0]["SFN_REVISION_DATE"].ToString()}
            };
            return dictFormInfo;
        }
        public int InsertSFN54497GetID(System.Web.Mvc.FormCollection collection, string waitingApproval)
        {
            string prcName = "PKG_SFN54497.PRC_INSRT_SFN54497";

            object[] prcParams = {
                collection["StaffRequestModel.PositionTitle"], //a_requestor_name
                collection["StaffRequestModel.Department"], //a_department
                collection["StaffRequestModel.Location"], //a_requestor_phone
                collection["StaffRequestModel.TypeOfHire"], //a_request_type
                collection["StaffRequestModel.NumberOfOpenings"], //a_number_of_cards
                collection["StaffRequestModel.SalaryRange"], //a_additional_comments
                collection["StaffRequestModel.PositionReportsTo"], //a_submitted_by
                collection["StaffRequestModel.Hours"],
                collection["StaffRequestModel.HoursPerWeek"],
                ConvertToOracleDate(collection["StaffRequestModel.DesiredStartDate"]), //a_request_date
                ConvertToOracleDate(collection["StaffRequestModel.ProjectedEndDate"]),
                collection["StaffRequestModel.PositionJustification"],
                collection["StaffRequestModel.SubmittedBy"],
                collection["StaffRequestModel.SubmitDate"], //a_submitted_date
                collection["StaffRequestModel.SubmitterComments"],
                waitingApproval
            };

            string[] prcParamNames = {
                "a_position_title",
                "a_department",
                "a_location",
                "a_type_of_hire",
                "a_number_of_openings",
                "a_salary_range",
                "a_position_reports_to",
                "a_hours",
                "a_hours_per_week",
                "a_desired_start_date",
                "a_projected_end_date",
                "a_position_justification",
                "a_submitted_by",
                "a_submitted_date",
                "a_submitter_comments",
                "a_waiting_approval"
            };

            dal = new DatabaseService("webformsdev");
            DataTable procedureReturn = dal.ExecuteProcedureOutCursor(prcName, prcParamNames, prcParams, 1);
            int staffRequestID = int.Parse(procedureReturn.Rows[0][0].ToString());

            return staffRequestID;
        }

        public void InsertSFN54497ApprovalDepartment(int reqID, System.Web.Mvc.FormCollection collection)
        {
            string prcName = "PKG_SFN54497.PRC_INSRT_SFN54497_APPROVAL_DEPARTMENT";

            object[] prcParams =
            {
                reqID,
                collection["Approval.DepartmentDirectorApproval"],
                ConvertBooleanToYesNo(Boolean.Parse(collection["Approval.DepartmentDirectorApproved"])),
                ConvertToOracleDate(DateTime.Today.ToString("MM/dd/yyyy"))
            };

            string[] prcParamNames = { "a_id_number", "a_department_director_approval", "a_department_director_approved", "a_department_director_approve_date" };

            dal = new DatabaseService("webformsdev");
            dal.ExecuteProcedure(prcName, prcParamNames, prcParams);
        }

        public void InsertSFN54497ApprovalDivision(int reqID, System.Web.Mvc.FormCollection collection)
        {
            string prcName = "PKG_SFN54497.PRC_INSRT_SFN54497_APPROVAL_DIVISION";

            object[] prcParams =
            {
                reqID,
                collection["Approval.DivisionChiefApproval"],
                ConvertBooleanToYesNo(Boolean.Parse(collection["Approval.DivisionChiefApproved"])),
                ConvertToOracleDate(DateTime.Today.ToString("MM/dd/yyyy"))
            };

            string[] prcParamNames = { "a_id_number", "a_division_chief_approval", "a_division_chief_approved", "a_division_chief_approve_date" };

            dal = new DatabaseService("webformsdev");
            dal.ExecuteProcedure(prcName, prcParamNames, prcParams);
        }

        public void InsertSFN54497ApprovalHR(int reqID, System.Web.Mvc.FormCollection collection)
        {
            string prcName = "PKG_SFN54497.PRC_INSRT_SFN54497_APPROVAL_HR";

            object[] prcParams =
            {
                reqID,
                collection["Approval.HumanResourcesApproval"],
                ConvertBooleanToYesNo(Boolean.Parse(collection["Approval.HR"])),
                ConvertToOracleDate(DateTime.Today.ToString("MM/dd/yyyy"))

            };

            string[] prcParamNames = { "a_id_number", "a_human_resources_approval", "a_human_resources_approved", "a_human_resources_approve_date" };

            dal = new DatabaseService("webformsdev");
            dal.ExecuteProcedure(prcName, prcParamNames, prcParams);
        }

        public void InsertSFN54497ApprovalFinance(int reqID, System.Web.Mvc.FormCollection collection)
        {
            string prcName = "PKG_SFN54497.PRC_INSRT_SFN54497_APPROVAL_FINANCE";

            object[] prcParams =
            {
                reqID,
                collection["Approval.DirectorOfFinanceApproval"],
                ConvertBooleanToYesNo(Boolean.Parse(collection["Approval.DirectorOfFinanceApproved"])),
                ConvertToOracleDate(DateTime.Today.ToString("MM/dd/yyyy"))
            };

            string[] prcParamNames = { "a_id_number", "a_finance_director_approval", "a_finance_director_approved", "a_finance_director_approve_date" };

            dal = new DatabaseService("webformsdev");
            dal.ExecuteProcedure(prcName, prcParamNames, prcParams);
        }

        public void InsertSFN54497ApprovalID(int reqID)
        {
            string prcName = "PKG_SFN54497.PRC_INSRT_SFN54497_APPROVAL_ID";

            object[] prcParams =
            {
                reqID
            };

            string[] prcParamNames = { "a_id_number" };

            dal = new DatabaseService("webformsdev");
            dal.ExecuteProcedure(prcName, prcParamNames, prcParams);
        }

        public DataTable GetSFN54497ID(System.Web.Mvc.FormCollection collection)
        {
            string prcName = "PKG_SFN54497.PRC_SELECT_SFN54497_ID";
            object[] prcParams = { collection["StaffRequestModels.ID"] };
            string[] prcParamNames = { "a_id_number" };
            DataTable dtFormInfo = dal.ExecuteProcedureOutCursor(prcName, prcParamNames, prcParams, 1);

            return dtFormInfo;
        }

        public DataTable GetSFN54497Info()
        {
            string prcName = "PKG_SFN54497.PRC_SELECT_SFN54497_INFO";
            object[] prcParams = { };
            string[] prcParamNames = { };
            DataTable dtFormInfo = dal.ExecuteProcedureOutCursor(prcName, prcParamNames, prcParams, 1);

            return dtFormInfo;
        }

        public DataTable GetSFN54497InfoByInfo()
        {
            string prcName = "PKG_SFN54497.PRC_SELECT_SFN54497_INFO_BY_MAX_ID";
            object[] prcParams = { };
            string[] prcParamNames = { };
            DataTable dtFormInfo = dal.ExecuteProcedureOutCursor(prcName, prcParamNames, prcParams, 1);

            return dtFormInfo;
        }

        public DataTable GetSFN54497InfoByID(int reqID)
        {
            string prcName = "PKG_SFN54497.PRC_SELECT_SFN54497_INFO_BY_ID";
            object[] prcParams = { reqID };
            string[] prcParamNames = { "a_id_number" };
            DataTable dtFormInfo = dal.ExecuteProcedureOutCursor(prcName, prcParamNames, prcParams, 1);

            return dtFormInfo;
        }

        public Dictionary<string, string> GetSFN54497Values(int id)
        {
            string prcName = "PKG_SFN54497.PRC_SELECT_SFN54497_INFO_BY_ID";

            object[] prcParams = { id };
            string[] prcParamNames = { "a_id_number" };

            DataTable dtFormInfo = dal.ExecuteProcedureOutCursor(prcName, prcParamNames, prcParams, 1);

            Dictionary<string, string> dictFormInfo = new Dictionary<string, string>()
            {
                {"PositionTitle", dtFormInfo.Rows[0]["POSITION_TITLE"].ToString()},
                {"Department", dtFormInfo.Rows[0]["DEPARTMENT"].ToString()},
                {"Location", dtFormInfo.Rows[0]["POSITION_LOCATION"].ToString()},
                {"HireType", dtFormInfo.Rows[0]["HIRE_TYPE"].ToString()},
                {"NumberOfOpenings", dtFormInfo.Rows[0]["NUM_OF_OPENINGS"].ToString()},
                {"Salary_Range", dtFormInfo.Rows[0]["SALARY_RANGE"].ToString()},
                {"PositionReportsTo", dtFormInfo.Rows[0]["POSITION_REPORTS_TO"].ToString()},
                {"Hours", dtFormInfo.Rows[0]["HOURS"].ToString()},
                {"HoursPerWeek", dtFormInfo.Rows[0]["HOURS_PER_WEEK"].ToString()},
                {"DesiredStartDate", dtFormInfo.Rows[0]["DESIRED_START_DATE"].ToString()},
                {"ProjectedEndDate", dtFormInfo.Rows[0]["PROJECTED_END_DATE"].ToString()},
                {"PositionJustification", dtFormInfo.Rows[0]["POSITION_JUSTIFICATION"].ToString()},
                {"SubmitterComments", dtFormInfo.Rows[0]["SUBMITTER_COMMENTS"].ToString()},
                {"SubmittedBy", dtFormInfo.Rows[0]["SUBMITTED_BY"].ToString()},
                {"SubmitDate", dtFormInfo.Rows[0]["SUBMIT_DATE"].ToString()},
                {"WaitingApproval", dtFormInfo.Rows[0]["WAITING_APPROVAL"].ToString() },
                {"DepartmentDirectorApproval", dtFormInfo.Rows[0]["DEPARTMENT_DIRECTOR_APPROVAL"].ToString()},
                {"DepartmentDirectorApproved", dtFormInfo.Rows[0]["DEPARTMENT_DIRECTOR_APPROVED"].ToString()},
                {"DepartmentDirectorApproveDate", dtFormInfo.Rows[0]["DEPARTMENT_DIRECTOR_APPROVE_DATE"].ToString()},
                {"DepartmentDirectorComments", dtFormInfo.Rows[0]["DEPARTMENT_DIRECTOR_COMMENTS"].ToString()},
                {"DivisionChiefApproval", dtFormInfo.Rows[0]["DIVISION_CHIEF_APPROVAL"].ToString()},
                {"DivisionChiefApproved", dtFormInfo.Rows[0]["DIVISION_CHIEF_APPROVED"].ToString()},
                {"DivisionChiefApproveDate", dtFormInfo.Rows[0]["DIVISION_CHIEF_APPROVE_DATE"].ToString()},
                {"DivisionChiefComments", dtFormInfo.Rows[0]["DIVISION_CHIEF_COMMENTS"].ToString()},
                {"HumanResourcesApproval", dtFormInfo.Rows[0]["HUMAN_RESOURCES_APPROVAL"].ToString() },
                {"HumanResourcesApproved", dtFormInfo.Rows[0]["HUMAN_RESOURCES_APPROVED"].ToString()},
                {"HumanResourcesApproveDate", dtFormInfo.Rows[0]["HUMAN_RESOURCES_APPROVE_DATE"].ToString()},
                {"HumanResourcesComments", dtFormInfo.Rows[0]["HUMAN_RESOURCES_COMMENTS"].ToString()},
                {"FinanceDirectorApproval", dtFormInfo.Rows[0]["FINANCE_DIRECTOR_APPROVAL"].ToString()},
                {"FinanceDirectorApproved", dtFormInfo.Rows[0]["FINANCE_DIRECTOR_APPROVED"].ToString()},
                {"FinanceDirectorApproveDate", dtFormInfo.Rows[0]["FINANCE_DIRECTOR_APPROVE_DATE"].ToString()},
                {"FinanceDirectorComments", dtFormInfo.Rows[0]["FINANCE_DIRECTOR_COMMENTS"].ToString()},
                {"AgencyDirectorApproval", dtFormInfo.Rows[0]["AGENCY_DIRECTOR_APPROVAL"].ToString()},
                {"AgencyDirectorApproved", dtFormInfo.Rows[0]["AGENCY_DIRECTOR_APPROVED"].ToString()},
                {"AgencyDirectorApproveDate", dtFormInfo.Rows[0]["AGENCY_DIRECTOR_APPROVE_DATE"].ToString()},
                {"AgencyDirectorComments", dtFormInfo.Rows[0]["AGENCY_DIRECTOR_COMMENTS"].ToString()}

            };

            return dictFormInfo;
        }

        public DataTable GetSFN54497Requestor(int id)
        {
            string prcName = "PKG_SFN54497.PRC_SELECT_SFN54497_REQUESTOR";
            object[] prcParams = { id };
            string[] prcParamNames = { "a_id_number" };
            DataTable dtFormInfo = dal.ExecuteProcedureOutCursor(prcName, prcParamNames, prcParams, 1);

            return dtFormInfo;
        }

        public DataTable GetSFN54497Director(int id)
        {
            string prcName = "PKG_SFN54497.PRC_SELECT_SFN54497_DIRECTOR";
            object[] prcParams = { id };
            string[] prcParamNames = { "a_id_number" };
            DataTable dtFormInfo = dal.ExecuteProcedureOutCursor(prcName, prcParamNames, prcParams, 1);

            return dtFormInfo;
        }

        public void UpdateSFN54497DepartmentDirectorApproval(System.Web.Mvc.FormCollection collection)
        {
            string prcName = "PKG_SFN54497.PRC_UPDATE_DEPARTMENT_DIRECTOR_APPROVAL_SFN54497";
            object[] prcParams = { collection["StaffRequestModels.ID"], ConvertBooleanToYesNo(Boolean.Parse(collection["Approvals.DepartmentDirectorApproved"])), collection["Approvals.DepartmentDirectorApproval"], collection["Approvals.DepartmentDirectorApproveDate"], collection["Approvals.DepartmentDirectorComments"] };
            string[] prcParamNames = { "a_id_number", "a_department_director_approved", "a_department_director_approval", "a_department_director_approve_date", "a_department_director_comments" };

            dal = new DatabaseService("webformsdev");
            dal.ExecuteProcedure(prcName, prcParamNames, prcParams);
        }

        public void UpdateSFN54497DivisionChiefApproval(System.Web.Mvc.FormCollection collection)
        {
            string prcName = "PKG_SFN54497.PRC_UPDATE_DIVISION_CHIEF_APPROVAL_SFN54497";
            object[] prcParams = { collection["StaffRequestModels.ID"], ConvertBooleanToYesNo(Boolean.Parse(collection["Approvals.DivisionChiefApproved"])), collection["Approvals.DivisionChiefApproval"], collection["Approvals.DivisionChiefApproveDate"], collection["Approvals.DivisionChiefComments"] };
            string[] prcParamNames = { "a_id_number", "a_division_chief_approved", "a_division_chief_approval", "a_division_chief_approve_date", "a_division_chief_comments" };

            dal = new DatabaseService("webformsdev");
            dal.ExecuteProcedure(prcName, prcParamNames, prcParams);
        }

        public void UpdateSFN54497HumanResourcesApproval(System.Web.Mvc.FormCollection collection)
        {
            string prcName = "PKG_SFN54497.PRC_UPDATE_HUMAN_RESOURCES_APPROVAL_SFN54497";
            object[] prcParams = { collection["StaffRequestModels.ID"], ConvertBooleanToYesNo(Boolean.Parse(collection["Approvals.HumanResourcesApproved"])), collection["Approvals.HumanResourcesApproval"], collection["Approvals.HumanResourcesApproveDate"], collection["Approvals.HumanResourcesComments"] };
            string[] prcParamNames = { "a_id_number", "a_human_resources_approved", "a_human_resources_approval", "a_human_resources_approve_date", "a_human_resources_comments" };

            dal = new DatabaseService("webformsdev");
            dal.ExecuteProcedure(prcName, prcParamNames, prcParams);
        }

        public void UpdateSFN54497FinanceDirectorApproval(System.Web.Mvc.FormCollection collection)
        {
            string prcName = "PKG_SFN54497.PRC_UPDATE_FINANCE_DIRECTOR_APPROVAL_SFN54497";
            object[] prcParams = { collection["StaffRequestModels.ID"], ConvertBooleanToYesNo(Boolean.Parse(collection["Approvals.DirectorOfFinanceApproved"])), collection["Approvals.DirectorOfFinanceApproval"], collection["Approvals.DirectorOfFinanceApproveDate"], collection["Approvals.DirectorOfFinanceComments"] };
            string[] prcParamNames = { "a_id_number", "a_finance_director_approved", "a_finance_director_approval", "a_finance_director_approve_date", "a_finance_director_comments" };

            dal = new DatabaseService("webformsdev");
            dal.ExecuteProcedure(prcName, prcParamNames, prcParams);
        }

        public void UpdateSFN54497AgencyDirectorApproval(System.Web.Mvc.FormCollection collection)
        {
            string prcName = "PKG_SFN54497.PRC_UPDATE_AGENCY_DIRECTOR_APPROVAL_SFN54497";
            object[] prcParams = { collection["StaffRequestModels.ID"], ConvertBooleanToYesNo(Boolean.Parse(collection["Approvals.AgencyDirectorApproved"])), collection["Approvals.AgencyDirectorApproval"], collection["Approvals.AgencyDirectorApproveDate"], collection["Approvals.AgencyDirectorComments"] };
            string[] prcParamNames = { "a_id_number", "a_agency_director_approved", "a_agency_director_approval", "a_agency_director_approve_date", "a_agency_director_comments" };

            dal = new DatabaseService("webformsdev");
            dal.ExecuteProcedure(prcName, prcParamNames, prcParams);
        }

        public void UpdateSFN54497WaitingApproval(int id, string waitingApproval)
        {
            string prcName = "PKG_SFN54497.PRC_UPDATE_WAITING_APPROVAL_SFN54497";
            object[] prcParams = { id, waitingApproval };
            string[] prcParamNames = { "a_id_number", "a_waiting_approval" };

            dal = new DatabaseService("webformsdev");
            dal.ExecuteProcedure(prcName, prcParamNames, prcParams);
        }

        public void UpdateSFN54497Approval(int id)
        {
            string prcName = "PKG_SFN54497.PRC_UPDATE_FORM_APPROVAL_SFN54497";
            object[] prcParams = { id };
            string[] prcParamNames = { "a_id_number" };

            dal = new DatabaseService("webformsdev");
            dal.ExecuteProcedure(prcName, prcParamNames, prcParams);
        }

        public void UpdateSFN54497Denial(int id)
        {
            string prcName = "PKG_SFN54497.PRC_UPDATE_FORM_DENIAL_SFN54497";
            object[] prcParams = { id };
            string[] prcParamNames = { "a_id_number" };

            dal = new DatabaseService("webformsdev");
            dal.ExecuteProcedure(prcName, prcParamNames, prcParams);
        }
    }
}