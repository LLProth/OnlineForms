using Microsoft.Ajax.Utilities;
using OnlineForms.ViewModels;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using WSI.Utility.Database;

namespace OnlineForms.Models
{
    public class ModelDALSFN61065 : ModelDAL
    {
        public ModelDALSFN61065():base()
        {
            dal = new DatabaseService("webformsdev");
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
                {"WaitingApproval", dtFormInfo.Rows[0]["WAITING_APPROVAL"].ToString() },
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

        public int InsertSFN61065GetID(System.Web.Mvc.FormCollection collection, string waitingApproval)
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
                ConvertToOracleDate(collection["BusinessCardModel.SubmittedDate"]), //a_submitted_date
                waitingApproval
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
                "a_submitted_date",
                "a_waiting_approval"
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

        public void UpdateSFN61065ProofComments(System.Web.Mvc.FormCollection collection)
        {
            string prcName = "PKG_SFN61065.PRC_UPDATE_PROOF_COMMENTS_SFN61065";
            object[] prcParams = { collection["BusinessCardModels.ID"] };
            string[] prcParamNames = { "a_id_number" };

            dal = new DatabaseService("webformsdev");
            dal.ExecuteProcedure(prcName, prcParamNames, prcParams);
        }

            public void UpdateSFN61065ProofUploaded(int id)
        {
            string prcName = "PKG_SFN61065.PRC_UPDATE_PROOF_UPLOADED_SFN61065";
            object[] prcParams = { id };
            string[] prcParamNames = { "a_id_number" };

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

        public void UpdateSFN61065WaitingApproval(System.Web.Mvc.FormCollection collection, string waitingApproval)
        {
            string prcName = "PKG_SFN61065.PRC_UPDATE_WAITING_APPROVAL_SFN61065";
            object[] prcParams = { collection["BusinessCardModels.ID"], waitingApproval };
            string[] prcParamNames = { "a_id_number", "a_waiting_approval" };

            dal = new DatabaseService("webformsdev");
            dal.ExecuteProcedure(prcName, prcParamNames, prcParams);
        }

        private byte[] ReadFile(string sPath)
        {
            //Initialize byte array with a null value initially.
            byte[] data = null;

            //Use FileInfo object to get file size.
            FileInfo fInfo = new FileInfo(sPath);
            long numBytes = fInfo.Length;

            //Open FileStream to read file
            FileStream fStream = new FileStream(sPath, FileMode.Open, FileAccess.Read);

            //Use BinaryReader to read file stream into byte array.
            BinaryReader br = new BinaryReader(fStream);

            //When you use BinaryReader, you need to supply number of bytes to read from file.
            //In this case we want to read entire file. So supplying total number of bytes.
            data = br.ReadBytes((int)numBytes);
            return data;
        }

        public void UploadProof(int id, byte[] pdfFileName)
        {
            // read the file contents into a byte array
            byte[] blob = pdfFileName;

            // initialize oracle server connection
            using (OracleConnection con = new OracleConnection(base.ConnectionString))
            {
                // set insert query
                string qry = @"UPDATE SFN61065 SET PROOF = :pdfLob WHERE ID_NUMBER = " + id;

                using (OracleCommand cmd = new OracleCommand(qry, con))
                {
                    OracleParameter blobParameter = new OracleParameter();
                    blobParameter.OracleDbType = OracleDbType.Blob;
                    blobParameter.ParameterName = ":pdfLob";
                    blobParameter.Value = blob;

                    // We are passing Name and Blob byte data as Oracle parameters
                    cmd.Parameters.Add(blobParameter);

                    //Open connection and execute insert query
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }    
        }

        public DataTable GetSFN61065ProofByID(int reqID)
        {
            string prcName = "PKG_SFN61065.PRC_SELECT_PROOF";
            object[] prcParams = { reqID };
            string[] prcParamNames = { "a_id_number" };
            DataTable dtFormInfo = dal.ExecuteProcedureOutCursor(prcName, prcParamNames, prcParams, 1);

            return dtFormInfo;
        }
    }
}