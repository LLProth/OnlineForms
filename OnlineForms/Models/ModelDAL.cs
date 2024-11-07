using Microsoft.Ajax.Utilities;
using OnlineForms.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using WSI.Utility.Database;

namespace OnlineForms.Models
{
    public class ModelDAL
    {
        protected IDatabaseService dal;
        private string _connectionString;
        public ModelDAL()
        {
            dal = new DatabaseService("webformsdev");
            _connectionString = dal.ConnectionString;
        }
        public string ConnectionString { get { return _connectionString; } set { _connectionString = value; } }

        public Dictionary<string, string> GetFormInfo(string path)
        {
            string prcName = "PKG_WEBFORMS.prc_select_webform_by_path";

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

        public DataTable getDepartments()
        {
            string prcName = "PKG_WEBFORMS.PRC_GET_DEPARTMENTS";

            object[] prcParams = { };
            string[] prcParamNames = { };

            dal = new DatabaseService("webformsdev");
            DataTable dtdepartments = dal.ExecuteProcedureOutCursor(prcName, prcParamNames, prcParams, 1);

            return dtdepartments;
        }

        public DataTable getStates()
        {
            string prcName = "PKG_WEBFORMS.PRC_GET_STATES";

            object[] prcParams = { };
            string[] prcParamNames = { };

            dal = new DatabaseService("webformsdev");
            DataTable dtdepartments = dal.ExecuteProcedureOutCursor(prcName, prcParamNames, prcParams, 1);

            return dtdepartments;
        }

        public Dictionary<string, object> SendEmail(string recipient, string from, string subject, string body)
        {
            string prcName = "MED.PRC_EMAIL";
            object[] prcParams = { recipient, from, subject, body };
            string[] prcParamNames = { "a_recipient", "a_from", "a_subject", "a_body" };

            dal = new DatabaseService("webformsdev");
            var results = dal.ExecuteProcedure(prcName, prcParamNames, prcParams);

            return results;
        }

        public static List<T> ConvertToList<T>(DataTable dt)
        {
            var columnNames = dt.Columns.Cast<DataColumn>().Select(c => c.ColumnName.ToLower()).ToList();
            var properties = typeof(T).GetProperties();
            return dt.AsEnumerable().Select(row =>
                {
                    var objT = Activator.CreateInstance<T>();
                    foreach (var property in properties)
                    {
                        if (columnNames.Contains(property.Name.ToLower()))
                        {
                            try
                            {
                                property.SetValue(objT, row[property.Name]);
                            }
                            catch (Exception) { }
                        }
                    }
                    return objT;
                }).ToList();
        }

        // SFN61065

        public Dictionary<string, string> GetFormInfoSFN61065(string path)
        {
            string prcName = "PKG_SFN61065.PRC_SELECT_WEBFORM_BY_PATH";

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
        public Dictionary<string, string> GetSFN61065Values(int id)
        {
            string prcName = "PKG_SFN61065.PRC_SELECT_SFN61065_INFO_BY_ID";

            object[] prcParams = { id };
            string[] prcParamNames = { "a_id_number" };

            DataTable dtFormInfo = dal.ExecuteProcedureOutCursor(prcName, prcParamNames, prcParams, 1);

            Dictionary<string, string> dictFormInfo = new Dictionary<string, string>()
            {
                {"Name", dtFormInfo.Rows[0]["REQUESTOR_NAME"].ToString()},
                {"RequestDate", dtFormInfo.Rows[0]["REQUEST_DATE"].ToString()},
                {"Department", dtFormInfo.Rows[0]["DEPARTMENT"].ToString()},
                {"RequestorPhone", dtFormInfo.Rows[0]["REQUESTOR_PHONE"].ToString()},
                {"RequestType", dtFormInfo.Rows[0]["REQUEST_TYPE"].ToString()},
                {"NumOfCards", dtFormInfo.Rows[0]["NUMBER_OF_CARDS"].ToString()},
                {"AdditionalComments", dtFormInfo.Rows[0]["ADDITIONAL_COMMENTS"].ToString()},
                {"SubmittedBy", dtFormInfo.Rows[0]["SUBMITTED_BY"].ToString()},
                {"SubmitDate", dtFormInfo.Rows[0]["SUBMIT_DATE"].ToString()},
                {"SupervisorApproval", dtFormInfo.Rows[0]["SUPERVISOR_APPROVAL"].ToString()},
                {"SupervisorApproveDate", dtFormInfo.Rows[0]["SUPERVISOR_APPROVE_DATE"].ToString()},
                {"FinanceApproval", dtFormInfo.Rows[0]["FINANCE_APPROVAL"].ToString()},
                {"FinanceApproveDate", dtFormInfo.Rows[0]["FINANCE_APPROVE_DATE"].ToString()},
                {"CommunicationsApproval", dtFormInfo.Rows[0]["COMMUNICATIONS_APPROVAL"].ToString()},
                {"CommunicationsApproveDate", dtFormInfo.Rows[0]["COMMUNICATIONS_APPROVE_DATE"].ToString()},
                {"FirstName", dtFormInfo.Rows[0]["FIRST_NAME"].ToString()},
                {"LastName", dtFormInfo.Rows[0]["LAST_NAME"].ToString()},
                {"Credentials", dtFormInfo.Rows[0]["CREDENTIALS"].ToString()},
                {"Title", dtFormInfo.Rows[0]["TITLE"].ToString()},
                {"Email", dtFormInfo.Rows[0]["EMAIL"].ToString()},
                {"TelephoneNumber", dtFormInfo.Rows[0]["TELEPHONE_NUMBER"].ToString()},
                {"CellNumber", dtFormInfo.Rows[0]["CELL_NUMBER"].ToString()},
                {"FaxNumber", dtFormInfo.Rows[0]["FAX_NUMBER"].ToString()},
                {"ProofComments", dtFormInfo.Rows[0]["PROOF_COMMENTS"].ToString() }
            };

            return dictFormInfo;
        }

        public DataTable GetSFN61065Info()
        {
            string prcName = "PKG_SFN61065.PRC_SELECT_SFN61065_INFO";
            object[] prcParams = { };
            string[] prcParamNames = { };
            DataTable dtFormInfo = dal.ExecuteProcedureOutCursor(prcName, prcParamNames, prcParams, 1);

            return dtFormInfo;
        }

        public DataTable GetSFN61065InfoByID(int reqID)
        {
            string prcName = "PKG_SFN61065.PRC_SELECT_SFN61065_INFO_BY_ID";
            object[] prcParams = { reqID };
            string[] prcParamNames = { "a_id_number" };
            DataTable dtFormInfo = dal.ExecuteProcedureOutCursor(prcName, prcParamNames, prcParams, 1);

            return dtFormInfo;
        }

        public DataTable GetSFN61065InfoByMaxID()
        {
            string prcName = "PKG_SFN61065.PRC_SELECT_SFN61065_INFO_BY_MAX_ID";
            object[] prcParams = { };
            string[] prcParamNames = { };
            DataTable dtFormInfo = dal.ExecuteProcedureOutCursor(prcName, prcParamNames, prcParams, 1);

            return dtFormInfo;
        }

        public DataTable GetSFN61065ID(System.Web.Mvc.FormCollection collection)
        {
            string prcName = "PKG_SFN61065.PRC_SELECT_SFN61065_ID";
            object[] prcParams = { collection["BusinessCardModels.ID"] };
            string[] prcParamNames = { "a_id_number" };
            DataTable dtFormInfo = dal.ExecuteProcedureOutCursor(prcName, prcParamNames, prcParams, 1);

            return dtFormInfo;
        }

        public DataTable GetSFN61065Requestor(int id)
        {
            string prcName = "PKG_SFN61065.PRC_SELECT_SFN61065_REQUESTOR";
            object[] prcParams = { id };
            string[] prcParamNames = { "a_id_number" };
            DataTable dtFormInfo = dal.ExecuteProcedureOutCursor(prcName, prcParamNames, prcParams, 1);

            return dtFormInfo;
        }

        public DataTable GetSFN61065InfoByMostRecent(string name)
        {
            string prcName = "PKG_SFN61065.PRC_SELECT_SFN61065_INFO_BY_MOST_RECENT";
            object[] prcParams = { name };
            string[] prcParamNames = { "a_requestor_name" };
            DataTable dtFormInfo = dal.ExecuteProcedureOutCursor(prcName, prcParamNames, prcParams, 1);

            return dtFormInfo;
        }

        public int InsertSFN61065GetID(System.Web.Mvc.FormCollection collection)
        {
            string prcName = "PKG_SFN61065.PRC_INSRT_SFN61065";

            object[] prcParams = {
                collection["BusinessCardModel.Name"], //a_requestor_name
                ConvertToOracleDate(collection["BusinessCardModel.DateSubmitted"]), //a_request_date
                collection["BusinessCardModel.Department"], //a_department
                collection["BusinessCardModel.Phone"], //a_requestor_phone
                collection["BusinessCardModel.RequestType"], //a_request_type
                collection["BusinessCardModel.NumOfCards"], //a_number_of_cards
                collection["BusinessCardModel.AdditionalComments"], //a_additional_comments
                collection["BusinessCardModel.SubmittedBy"], //a_submitted_by
                ConvertToOracleDate(collection["BusinessCardModel.SubmittedDate"]) //a_submitted_date
                //collection["BusinessCardModel.SupervisorApproval"],
                //ConvertToOracleDate(collection["BusinessCardModel.SupervisorApprovalDate"]),
                //collection["BusinessCardModel.FinanceApproval"],
                //ConvertToOracleDate(collection["BusinessCardModel.FinanceApprovalDate"]),
                //collection["BusinessCardModel.CommunicationsApproval"],
                //ConvertToOracleDate(collection["BusinessCardModel.CommunicationsApprovalDate"])
            };

            string[] prcParamNames = {
                "a_requestor_name",
                "a_request_date",
                "a_department",
                "a_requestor_phone",
                "a_request_type",
                "a_number_of_cards",
                "a_additional_comments",
                "a_submitted_by",
                "a_submitted_date"
                //"a_supervisor_approval",
                //"a_supervisor_approve_date",
                //"a_finance_approval",
                //"a_finance_approve_date",
                //"a_communications_approval",
                //"a_communications_approve_date"
            };

            DataTable procedureReturn = dal.ExecuteProcedureOutCursor(prcName, prcParamNames, prcParams, 1);

            int businessCardID = int.Parse(procedureReturn.Rows[0][0].ToString());

            return businessCardID;
        }

        public void InsertSFN61065BusinessCardInfo(System.Web.Mvc.FormCollection collection, int reqID)
        {
            string prcName = "PKG_SFN61065.PRC_INSRT_SFN61065_BUSINESS_CARD_INFO";


            object[] prcParams =
                {
                    reqID,
                    collection["BusinessCardInfo.FirstName"],
                    collection["BusinessCardInfo.LastName"],
                    collection["BusinessCardInfo.Credentials"],
                    collection["BusinessCardInfo.Title"],
                    collection["BusinessCardInfo.Email"],
                    collection["BusinessCardInfo.TelephoneNum"],
                    collection["BusinessCardInfo.CellNum"],
                    collection["BusinessCardInfo.FaxNum"]
                };

            string[] prcParamNames =
            {
                    "a_id_number",
                    "a_first_name",
                    "a_last_name",
                    "a_credentials",
                    "a_title",
                    "a_email",
                    "a_telephone_number",
                    "a_cell_number",
                    "a_fax_number"
                };

            dal = new DatabaseService("webformsdev");
            dal.ExecuteProcedure(prcName, prcParamNames, prcParams);

        }

        public void InsertSFN61065Approval(int reqID)
        {
            string prcName = "PKG_SFN61065.PRC_INSRT_SFN61065_APPROVAL";

            object[] prcParams =
            {
                reqID
            };

            string[] prcParamNames = { "a_id_number" };

            dal = new DatabaseService("webformsdev");
            dal.ExecuteProcedure(prcName, prcParamNames, prcParams);
        }

        public void UpdateSFN61065SupervisorApproval(System.Web.Mvc.FormCollection collection)
        {
            string prcName = "PKG_SFN61065.PRC_UPDATE_SUPERVISOR_APPROVAL_SFN61065";
            object[] prcParams = { collection["BusinessCardModels.ID"], ConvertBooleanToYesNo(Boolean.Parse(collection["Approvals.SupervisorApproved"])), collection["Approvals.SupervisorApproval"], collection["Approvals.SupervisorApprovalDate"] };
            string[] prcParamNames = { "a_id_number", "a_supervisor_approved", "a_supervisor_approval", "a_supervisor_approve_date" };

            dal = new DatabaseService("webformsdev");
            dal.ExecuteProcedure(prcName, prcParamNames, prcParams);

        }

        public void UpdateSFN61065FinanceApproval(System.Web.Mvc.FormCollection collection)
        {
            string prcName = "PKG_SFN61065.PRC_UPDATE_FINANCE_APPROVAL_SFN61065";
            object[] prcParams = { collection["BusinessCardModels.ID"], ConvertBooleanToYesNo(Boolean.Parse(collection["Approvals.FinanceApproved"])), collection["Approvals.FinanceApproval"], collection["Approvals.FinanceApprovalDate"] };
            string[] prcParamNames = { "a_id_number", "a_finance_approved", "a_finance_approval", "a_finance_approve_date" };

            dal = new DatabaseService("webformsdev");
            dal.ExecuteProcedure(prcName, prcParamNames, prcParams);

        }

        public void UpdateSFN61065CommunicationsApproval(System.Web.Mvc.FormCollection collection)
        {
            string prcName = "PKG_SFN61065.PRC_UPDATE_COMMUNICATIONS_APPROVAL_SFN61065";
            object[] prcParams = { collection["BusinessCardModels.ID"], ConvertBooleanToYesNo(Boolean.Parse(collection["Approvals.CommunicationsApproved"])), collection["Approvals.CommunicationsApproval"], collection["Approvals.CommunicationsApprovalDate"] };
            string[] prcParamNames = { "a_id_number", "a_communications_approved", "a_communications_approval", "a_communications_approve_date" };

            dal = new DatabaseService("webformsdev");
            dal.ExecuteProcedure(prcName, prcParamNames, prcParams);

        }

        public void UpdateSFN61065RequestingEmployeeApproval(System.Web.Mvc.FormCollection collection)
        {
            string prcName = "PKG_SFN61065.PRC_UPDATE_REQUESTING_EMPLOYEE_APPROVAL_SFN61065";
            object[] prcParams = { collection["BusinessCardModels.ID"], ConvertBooleanToYesNo(Boolean.Parse(collection["Approvals.RequestingEmployeeApproved"])), collection["Approvals.RequestingEmployeeApproval"], collection["Approvals.RequestingEmployeeApprovalDate"], collection["Approvals.ProofComments"] };
            string[] prcParamNames = { "a_id_number", "a_requesting_employee_approved", "a_requesting_employee_approval", "a_requesting_employee_approve_date", "a_proof_comments" };

            dal = new DatabaseService("webformsdev");
            dal.ExecuteProcedure(prcName, prcParamNames, prcParams);

        }

        public void UpdateSFN61065Proof(int id, string _path)
        {
            string prcName = "PKG_SFN61065.PRC_UPDATE_REQUESTING_EMPLOYEE_APPROVAL_SFN61065";
            object[] prcParams = { id, _path };
            string[] prcParamNames = { "a_id_number", "a_proof" };

            dal = new DatabaseService("webformsdev");
            dal.ExecuteProcedure(prcName, prcParamNames, prcParams);

        }
        // SFN61579

        public Dictionary<string, string> GetFormInfoSFN61579(string path)
        {
            string prcName = "PKG_SFN61579.PRC_SELECT_WEBFORM_BY_PATH";

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
        public Dictionary<string, string> GetSFN61579Values(int id)
        {
            string prcName = "PKG_SFN61579.PRC_SELECT_SFN61579_INFO_BY_ID";

            object[] prcParams = { id };
            string[] prcParamNames = { "a_id_number" };

            DataTable dtFormInfo = dal.ExecuteProcedureOutCursor(prcName, prcParamNames, prcParams, 1);

            Dictionary<string, string> dictFormInfo = new Dictionary<string, string>()
            {
                {"Name", dtFormInfo.Rows[0]["REQUEST_EMPLOYEE"].ToString()},
                {"Organization", dtFormInfo.Rows[0]["ORGANIZATION_NAME"].ToString()},
                {"Date", dtFormInfo.Rows[0]["REQUEST_DATE"].ToString()},
                {"Email", dtFormInfo.Rows[0]["EMAIL_ADDRESS"].ToString()},
                {"EventMonth", dtFormInfo.Rows[0]["EVENT_MONTH"].ToString()},
                {"CharityDescription", dtFormInfo.Rows[0]["CHARITY_DESCRIPTION"].ToString()},
                {"Reviewed", dtFormInfo.Rows[0]["REVIEWED"].ToString() }
            };

            return dictFormInfo;
        }

        public DataTable GetSFN61579Info()
        {
            string prcName = "PKG_SFN61579.PRC_SELECT_SFN61579_INFO";
            object[] prcParams = { };
            string[] prcParamNames = { };
            DataTable dtFormInfo = dal.ExecuteProcedureOutCursor(prcName, prcParamNames, prcParams, 1);

            return dtFormInfo;
        }

        public DataTable GetSFN61579InfoByID(int reqID)
        {
            string prcName = "PKG_SFN61579.PRC_SELECT_SFN61579_INFO_BY_ID";
            object[] prcParams = { reqID };
            string[] prcParamNames = { "a_id_number" };
            DataTable dtFormInfo = dal.ExecuteProcedureOutCursor(prcName, prcParamNames, prcParams, 1);

            return dtFormInfo;
        }

        public DataTable GetSFN61579InfoByInfo()
        {
            string prcName = "PKG_SFN61579.PRC_SELECT_SFN61579_INFO_BY_MAX_ID";
            object[] prcParams = { };
            string[] prcParamNames = { };
            DataTable dtFormInfo = dal.ExecuteProcedureOutCursor(prcName, prcParamNames, prcParams, 1);

            return dtFormInfo;
        }

        public void UpdateSFN61579Reviewed(System.Web.Mvc.FormCollection collection)
        {
            string prcName = "PKG_SFN61579.PRC_UPDATE_REVIEWED_SFN61579";
            object[] prcParams = { collection["CharitableModels.ID"] };
            string[] prcParamNames = { "a_id_number" };

            dal = new DatabaseService("webformsdev");
            dal.ExecuteProcedure(prcName, prcParamNames, prcParams);

        }

        public int InsertSFN61579GetID(System.Web.Mvc.FormCollection collection)
        {
            string prcName = "PKG_SFN61579.PRC_INSRT_SFN61579";

            object[] prcParams = {
                collection["CharitableModel.Name"],
                collection["CharitableModel.Email"],
                ConvertToOracleDate(collection["CharitableModel.DateSubmitted"]),
                collection["CharitableModel.Organization"],
                collection["CharitableModel.Month"],
                collection["CharitableModel.CharityDescription"],
                collection["CharitableModel.Reviewed"]
            };

            string[] prcParamNames = {
                "a_request_employee",
                "a_email_address",
                "a_request_date",
                "a_organization_name",
                "a_event_month",
                "a_charity_description",
                "a_reviewed"
            };


            dal = new DatabaseService("webformsdev");
            DataTable procedureReturn = dal.ExecuteProcedureOutCursor(prcName, prcParamNames, prcParams, 1);
            int charitableID = int.Parse(procedureReturn.Rows[0][0].ToString());

            return charitableID;

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
        public int InsertSFN54497GetID(System.Web.Mvc.FormCollection collection)
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
                collection["StaffRequestModel.SubmitterComments"]

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
                "a_submitter_comments"

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
                collection["StaffRequestModel.SubmittedDate"]
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
                collection["StaffRequestModel.SubmittedDate"]
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
                collection["StaffRequestModel.SubmittedDate"]

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
                collection["StaffRequestModel.SubmittedDate"]
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
                {"DesiredStartDate", dtFormInfo.Rows[0]["DESIRED_START_DATE"].ToString()},
                {"ProjectedEndDate", dtFormInfo.Rows[0]["PROJECTED_END_DATE"].ToString()},
                {"PositionJustification", dtFormInfo.Rows[0]["POSITION_JUSTIFICATION"].ToString()},
                {"SubmitterComments", dtFormInfo.Rows[0]["SUBMITTER_COMMENTS"].ToString()},
                {"SubmittedBy", dtFormInfo.Rows[0]["SUBMITTED_BY"].ToString()},
                {"SubmitDate", dtFormInfo.Rows[0]["SUBMIT_DATE"].ToString()},
                {"HoursPerWeek", dtFormInfo.Rows[0]["HOURS_PER_WEEK"].ToString()},
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
            object[] prcParams = { collection["StaffRequestModels.ID"], ConvertBooleanToYesNo(Boolean.Parse(collection["Approvals.DivisionChiefApproved"])), collection["Approvals.Division_ChiefApproval"], collection["Approvals.DivisionChiefApproveDate"], collection["Approvals.DivisionChiefComments"] };
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
            object[] prcParams = { collection["StaffRequestModels.ID"], ConvertBooleanToYesNo(Boolean.Parse(collection["Approvals.DirectorOfFinanceApproved"])), collection["Approvals.DirectorOfFinanceApproval"], collection["Approvals.DirectorOfFinanceApprovalDate"], collection["Approvals.DirectorOfFinanceComments"] };
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

        // ESCRF
        public int InsertESCRFNewHireGetID(System.Web.Mvc.FormCollection collection)
        {

            string prcName = "PKG_ESCRF.PRC_INSRT_ESCRF_NEW_HIRE";

            object[] prcParams =
        {
                collection["NewHireView.Firstname"],
                collection["NewHireView.Lastname"],
                collection["NewHireView.MiddleInitial"],
                collection["NewHireView.JobTitle"],
                ConvertToOracleDate(collection["NewHireView.EffectiveDate"]),
                collection["NewHireView.CurrentSupervisor"],
                collection["NewHireView.EmployeeType"],
                ConvertCheckboxBoolean((collection["NewHireView.TransferringFromAgency"])),
                collection["NewHireView.FLSAStatus"],
                collection["NewHireView.Comments"],
                collection["NewHireView.ModifiedBy"],
                ConvertToOracleDate(collection["NewHireView.CreatedDate"]),
                collection["NewHireView.ChangeType"],
                ConvertBooleanToYesNo(Boolean.Parse(collection["NewHireView.Reviewed"])),
                ConvertToOracleDate(collection["NewHireView.ReviewedOn"]),
                ConvertBooleanToYesNo(Boolean.Parse(collection["NewHireView.IsComplete"])),
                ConvertBooleanToYesNo(Boolean.Parse(collection["NewHireView.InProgress"])),
            };

            string[] prcParamNames =
        {
                "a_first_name",
                "a_last_name",
                "a_middle_initial",
                "a_job_title",
                "a_effective_date",
                "a_current_supervisor",
                "a_employment_type",
                "a_transferring_from_agency",
                "a_flsa_status",
                "a_comments",
                "a_modified_by",
                "a_created_date",
                "a_change_type",
                "a_reviewed",
                "a_reviewed_on",
                "a_is_complete",
                "a_in_progress"
            };

            DataTable procedureReturn = dal.ExecuteProcedureOutCursor(prcName, prcParamNames, prcParams, 1);

            int requisitionID = int.Parse(procedureReturn.Rows[0][0].ToString());

            return requisitionID;
        }

        public DataTable GetESCRFNewHireInfo()
        {
            string prcName = "PKG_ESCRF.PRC_SELECT_ESCRF_NEW_HIRE_INFO";
            object[] prcParams = { };
            string[] prcParamNames = { };
            DataTable dtFormInfo = dal.ExecuteProcedureOutCursor(prcName, prcParamNames, prcParams, 1);

            return dtFormInfo;
        }

        public DataTable GetESCRFNewHireInfoByID(int reqID)
        {
            string prcName = "PKG_ESCRF.PRC_SELECT_ESCRF_NEW_HIRE_INFO_BY_ID";
            object[] prcParams = { reqID };
            string[] prcParamNames = { "a_id_number" };
            DataTable dtFormInfo = dal.ExecuteProcedureOutCursor(prcName, prcParamNames, prcParams, 1);

            return dtFormInfo;
        }

        public void UpdateESCRFNewHire(int reqID)
        {
            string prcName = "PKG_ESCRF.PRC_UPDATE_REVIEWED_ESCRF_NEW_HIRE";
            object[] prcParams = { reqID };
            string[] prcParamNames = { "a_id_number" };

            dal = new DatabaseService("webformsdev");
            dal.ExecuteProcedure(prcName, prcParamNames, prcParams);

        }

        public int InsertESCRFTerminationGetID(System.Web.Mvc.FormCollection collection)
        {
            string prcName = "PKG_ESCRF.PRC_INSRT_ESCRF_TERMINATION";
            object[] prcParams =
            {

                collection["TerminationView.PhoneNumber"],
                collection["TerminationView.Department"],
                collection["TerminationView.OfficeLocation"],
                 collection["TerminationView.ModifiedBy"],
                ConvertToOracleDate(collection["TerminationView.CreatedDate"]),
                collection["TerminationView.ChangeType"],
                ConvertBooleanToYesNo(Boolean.Parse(collection["TerminationView.Reviewed"])),
                ConvertToOracleDate(collection["TerminationView.ReviewedOn"]),
                ConvertBooleanToYesNo(Boolean.Parse(collection["TerminationView.IsComplete"])),
                ConvertBooleanToYesNo(Boolean.Parse(collection["TerminationView.InProgress"])),
                collection["TerminationView.EmployeeName"]
            };

            string[] prcParamNames =
            {
                "a_phone_number",
                "a_department",
                "a_office_location",
                 "a_modified_by",
                "a_created_date",
                "a_change_type",
                "a_reviewed",
                "a_reviewed_on",
                "a_is_complete",
                "a_in_progress",
                "a_employee_name"
            };

            DataTable procedureReturn = dal.ExecuteProcedureOutCursor(prcName, prcParamNames, prcParams, 1);

            int requisitionID = int.Parse(procedureReturn.Rows[0][0].ToString());

            return requisitionID;
        }

        public DataTable GetESCRFTerminationInfo()
        {
            string prcName = "PKG_ESCRF.PRC_SELECT_ESCRF_TERMINATION_INFO";
            object[] prcParams = { };
            string[] prcParamNames = { };
            DataTable dtFormInfo = dal.ExecuteProcedureOutCursor(prcName, prcParamNames, prcParams, 1);

            return dtFormInfo;
        }


        public DataTable GetESCRFTerminationInfoByID(int reqID)
        {
            string prcName = "PKG_ESCRF.PRC_SELECT_ESCRF_Termination_INFO_BY_ID";
            object[] prcParams = { reqID };
            string[] prcParamNames = { "a_id_number" };
            DataTable dtFormInfo = dal.ExecuteProcedureOutCursor(prcName, prcParamNames, prcParams, 1);

            return dtFormInfo;
        }

        public void UpdateESCRFTermination(int reqID)
        {
            string prcName = "PKG_ESCRF.PRC_UPDATE_REVIEWED_ESCRF_TERMINATION";
            object[] prcParams = { reqID };
            string[] prcParamNames = { "a_id_number" };

            dal = new DatabaseService("webformsdev");
            dal.ExecuteProcedure(prcName, prcParamNames, prcParams);

        }

        public int InsertESCRFChangeGetID(System.Web.Mvc.FormCollection collection)
        {
            Console.WriteLine(collection.ToString());
            string prcName = "PKG_ESCRF.PRC_INSRT_ESCRF_CHANGE";
            object[] prcParams =
            {
                collection["Change.EmployeeName"],
                collection["Change.PhoneNumber"],
                collection["Change.Department"],
                collection["Change.OfficeLocation"],
                ConvertToOracleDate(collection["Change.EffectiveDate"]),
                collection["Change.CurrentSupervisor"],
                ConvertCheckboxBoolean(collection["Change.IsNewPosition"]),
                collection["Change.PositionName"],
                ConvertCheckboxBoolean(collection["Change.IsNewSupervisor"]),
                collection["Change.NewSupervisor"],
                ConvertCheckboxBoolean(collection["Change.IsTempToFte"]),
                collection["Change.FLSAStatus"],
                collection["Change.Comments"],
                collection["Change.ModifiedBy"],
                ConvertToOracleDate(collection["Change.CreatedDate"]),
                collection["Change.ChangeType"],
                ConvertBooleanToYesNo(Boolean.Parse(collection["Change.Reviewed"])),
                ConvertToOracleDate(collection["Change.ReviewedOn"]),
                ConvertBooleanToYesNo(Boolean.Parse(collection["Change.IsComplete"])),
                ConvertBooleanToYesNo(Boolean.Parse(collection["Change.InProgress"])),
                int.Parse(collection["Change.ListID"].ToString())
            };

            string[] prcParamNames =
            {
                "a_employee_name",
                "a_phone_number",
                "a_department",
                "a_office_location",
                "a_effective_date",
                "a_current_supervisor",
                "a_is_new_position",
                "a_position_name",
                "a_is_new_supervisor",
                "a_new_supervisor",
                "a_temp_to_fte",
                "a_flsa_status",
                "a_comments",
                "a_modified_by",
                "a_created_date",
                "a_change_type",
                "a_reviewed",
                "a_reviewed_on",
                "a_is_complete",
                "a_in_progress",
                "a_list_id"
            };

            DataTable procedureReturn = dal.ExecuteProcedureOutCursor(prcName, prcParamNames, prcParams, 1);

            int requisitionID = int.Parse(procedureReturn.Rows[0][0].ToString());

            return requisitionID;
        }

        public DataTable GetESCRFChangeInfo()
        {
            string prcName = "PKG_ESCRF.PRC_SELECT_ESCRF_CHANGE_INFO";
            object[] prcParams = { };
            string[] prcParamNames = { };
            DataTable dtFormInfo = dal.ExecuteProcedureOutCursor(prcName, prcParamNames, prcParams, 1);

            return dtFormInfo;
        }

        public DataTable GetESCRFChangeInfoByID(int reqID)
        {
            string prcName = "PKG_ESCRF.PRC_SELECT_ESCRF_CHANGE_INFO_BY_ID";
            object[] prcParams = { reqID };
            string[] prcParamNames = { "a_id_number" };
            DataTable dtFormInfo = dal.ExecuteProcedureOutCursor(prcName, prcParamNames, prcParams, 1);

            return dtFormInfo;
        }

        public void UpdateESCRFChange(int reqID)
        {
            string prcName = "PKG_ESCRF.PRC_UPDATE_REVIEWED_ESCRF_CHANGE";
            object[] prcParams = { reqID };
            string[] prcParamNames = { "a_id_number" };

            dal = new DatabaseService("webformsdev");
            dal.ExecuteProcedure(prcName, prcParamNames, prcParams);
        }

        public int InsertESCRFNameGetID(System.Web.Mvc.FormCollection collection)
        {

            string prcName = "PKG_ESCRF.PRC_INSRT_ESCRF_NAME";

            object[] prcParams =
            {
                collection["Name.EmployeeName"],
                collection["Name.PhoneNumber"],
                collection["Name.Department"],
                collection["Name.OfficeLocation"],
                ConvertToOracleDate(collection["Name.EffectiveDate"]),
                collection["Name.CurrentSupervisor"],
                collection["Name.Firstname"],
                collection["Name.Lastname"],
                collection["Name.MiddleInitial"],
                collection["Name.Comments"],
                collection["Name.ModifiedBy"],
                ConvertToOracleDate(collection["Name.CreatedDate"]),
                collection["Name.ChangeType"],
                ConvertBooleanToYesNo(Boolean.Parse(collection["Name.Reviewed"])),
                ConvertToOracleDate(collection["Name.ReviewedOn"]),
                ConvertBooleanToYesNo(Boolean.Parse(collection["Name.IsComplete"])),
                ConvertBooleanToYesNo(Boolean.Parse(collection["Name.InProgress"])),
                int.Parse(collection["Name.ListID"].ToString())
            };

            string[] prcParamNames =
            {
                "a_employee_name",
                "a_phone_number",
                "a_department",
                "a_office_location",
                "a_effective_date",
                "a_current_supervisor",
                "a_first_name",
                "a_last_name",
                "a_middle_initial",
                "a_comments",
                "a_modified_by",
                "a_created_date",
                "a_change_type",
                "a_reviewed",
                "a_reviewed_on",
                "a_is_complete",
                "a_in_progress",
                "a_list_id"
            };
            DataTable procedureReturn = dal.ExecuteProcedureOutCursor(prcName, prcParamNames, prcParams, 1);

            int requisitionID = int.Parse(procedureReturn.Rows[0][0].ToString());

            return requisitionID;
        }

        public DataTable GetESCRFNameInfo()
        {
            string prcName = "PKG_ESCRF.PRC_SELECT_ESCRF_NAME_INFO";
            object[] prcParams = { };
            string[] prcParamNames = { };
            DataTable dtFormInfo = dal.ExecuteProcedureOutCursor(prcName, prcParamNames, prcParams, 1);

            return dtFormInfo;
        }

        public DataTable GetESCRFNameInfoByID(int reqID)
        {
            string prcName = "PKG_ESCRF.PRC_SELECT_ESCRF_NAME_INFO_BY_ID";
            object[] prcParams = { reqID };
            string[] prcParamNames = { "a_id_number" };
            DataTable dtFormInfo = dal.ExecuteProcedureOutCursor(prcName, prcParamNames, prcParams, 1);

            return dtFormInfo;
        }

        public void UpdateESCRFName(int reqID)
        {
            string prcName = "PKG_ESCRF.PRC_UPDATE_REVIEWED_ESCRF_NAME";
            object[] prcParams = { reqID };
            string[] prcParamNames = { "a_id_number" };

            dal = new DatabaseService("webformsdev");
            dal.ExecuteProcedure(prcName, prcParamNames, prcParams);
        }

        public int InsertMaintenanceRequestGetID(System.Web.Mvc.FormCollection collection)
        {
            string prcName = "PKG_MAINTENANCE_REQUESTS.PRC_INSRT_MAINTENANCE_REQUEST";
            object[] prcParams =
            {
                collection["MaintenanceRequest.EmployeeName"],
                collection["MaintenanceRequest.Email"],
                collection["MaintenanceRequest.Agency"],
                collection["MaintenanceRequest.Phonenumber"],
                ConvertToOracleDate(collection["MaintenanceRequest.EnteredDate"]),
                ConvertToOracleDate(collection["MaintenanceRequest.CompletedDate"]),
                collection["MaintenanceRequest.Description"],
                collection["MaintenanceRequest.Subject"]
            };

            string[] prcParamNames =
            {
                "a_employee_name",
                "a_email",
                "a_agency",
                "a_phonenumber",
                "a_entered_date",
                "a_completed_date",
                "a_description",
                "a_subject"

            };

            DataTable procedureReturn = dal.ExecuteProcedureOutCursor(prcName, prcParamNames, prcParams, 1);

            int requisitionID = int.Parse(procedureReturn.Rows[0][0].ToString());

            return requisitionID;
        }

        public DataTable GetMaintenanceRequestInfo()
        {
            string prcName = "PKG_MAINTENANCE_REQUESTS.PRC_SELECT_MAINTENANCE_REQUEST_INFO";
            object[] prcParams = { };
            string[] prcParamNames = { };
            DataTable dtFormInfo = dal.ExecuteProcedureOutCursor(prcName, prcParamNames, prcParams, 1);

            return dtFormInfo;

        }

        public DataTable GetMaintenanceRequestInfoByID(int reqID)
        {
            string prcName = "PKG_MAINTENANCE_REQUESTS.PRC_SELECT_MAINTENANCE_REQUEST_INFO_BY_ID";
            object[] prcParams = { reqID };
            string[] prcParamNames = { "a_id_number" };
            DataTable dtFormInfo = dal.ExecuteProcedureOutCursor(prcName, prcParamNames, prcParams, 1);

            return dtFormInfo;
        }

        public void UpdateMaintenanceRequest(int reqID)
        {

            string prcName = "PKG_MAINTENANCE_REQUESTS.PRC_UPDATE_MAINTENANCE_REQUESTS";
            object[] prcParams = { reqID, DateTime.Now.ToShortDateString() };
            string[] prcParamNames = { "a_id_number", "a_completed_date" };
            dal = new DatabaseService("webformsdev");
            dal.ExecuteProcedure(prcName, prcParamNames, prcParams);
        }

        public static string ConvertBooleanToYesNo(bool boolParam)
        {
            string boolChar = (boolParam) ? "Y" : "N";
            return boolChar;
        }

        public static string ConvertCheckboxBoolean(string stringparam)
        {

            string boolLetter;
            if (stringparam == "true,false")
            {
                boolLetter = "Y";

            }
            else
            {
                boolLetter = "N";

            }
            return boolLetter;

        }


        public static string ConvertToOracleDate(string dateParam)
        {
            DateTime date = DateTime.Parse(dateParam);
            return date.ToShortDateString();
        }
        public DataTable GetSuperUsers(string form)
        {
            string sql = "SELECT * FROM WEBFORMS.FORM_SUPERUSERS WHERE SU_FORM_ID = :form";
            string[] prcParams = { form };
            string[] prcParamNames = { ":form" };
            DataTable dt = dal.ExecuteSelect(sql, prcParamNames, prcParams);

            return dt;
        }
    }
}