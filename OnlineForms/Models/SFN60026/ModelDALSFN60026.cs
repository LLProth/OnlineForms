using Microsoft.Ajax.Utilities;
using OnlineForms.ViewModels;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using WSI.Utility.Database;
using OnlineForms.Logging;

namespace OnlineForms.Models
{
    public class ModelDALSFN60026 : ModelDAL
    {
        public static LoggingService log = new LoggingService();
        public ModelDALSFN60026():base()
        {
            log.LogMessage("SFN60026 ModelDALSFN60026 - Connection initiated");
        }

        public DataTable GetSFN60026InfoByID(int reqID)
        {
            string sql = " SELECT * FROM WEBFORMS.SFN60026 WHERE ID_NUMBER = :a_id_number";
            object[] prcParams = { reqID };
            string[] prcParamNames = { "a_id_number" };
            base.dal = new DatabaseService("webformsdev");
            DataTable dt = dal.ExecuteSelect(sql, prcParamNames, prcParams);
            return dt;
        }

        public DataTable GetSFN60026Info(string name)
        {
            string sql = "SELECT * FROM (SELECT * FROM WEBFORMS.SFN60026 WHERE SUBMIT_NAME = :a_name UNION " +
                "SELECT * FROM WEBFORMS.SFN60026 WHERE SUPERVISOR_SIGNATURE = :a_name UNION " +
                "SELECT * FROM WEBFORMS.SFN60026 WHERE DIRECTOR_SIGNATURE = :a_name  UNION " +
                "SELECT * FROM WEBFORMS.SFN60026 WHERE CHIEF_SIGNATURE = :a_name  UNION " +
                "SELECT * FROM WEBFORMS.SFN60026 WHERE COMMITTEE_APPROVAL_LIST LIKE '%:a_name%' UNION " +
                "SELECT * FROM WEBFORMS.SFN60026 WHERE HR_SIGNATURE LIKE '%:a_name%' UNION " +
                "SELECT * FROM WEBFORMS.SFN60026 WHERE CHIEF_ENDORSEMENT = :a_name UNION " +
                "SELECT * FROM WEBFORMS.SFN60026 WHERE AGENCY_DIRECTOR_ENDORSEMENT = :a_name) " +
                "ORDER BY MODIFIED_DATE DESC";
            string[] prcParams = { name };
            string[] prcParamNames = { "a_name" };
            DataTable dt = dal.ExecuteSelect(sql, prcParamNames, prcParams);
            return dt;
        }

        public DataTable GetSFN60026AllInfo()
        {
            string sql = "SELECT * FROM WEBFORMS.SFN60026 ORDER BY MODIFIED_DATE DESC";
            string[] prcParams = { };
            string[] prcParamNames = { };
            DataTable dt = dal.ExecuteSelect(sql, prcParamNames, prcParams);
            return dt;
        }

        public DataTable GetSFN60026InfoByMaxID()
        {
            string sql = "SELECT id_number FROM WEBFORMS.SFN60026 WHERE SFN60026.ID_NUMBER = (SELECT MAX(id_number) FROM WEBFORMS.SFN60026)";
            string[] prcParams = { };
            string[] prcParamNames = { };
            DataTable dt = dal.ExecuteSelect(sql, prcParams, prcParamNames);

            return dt;
        }
        public int InsertSFN60026GetID(System.Web.Mvc.FormCollection collection)
        {
            string prcName = "PKG_SFN60026.PRC_INSRT_SFN60026";
            string[] prcParams = {
                collection["BonusRecommendationModel.NomineeName"], //a_nominee_name
                collection["BonusRecommendationModel.NomineePosition"], //a_nominee_position
                collection["BonusRecommendationModel.NomineeDepartment"], //a_nominee_department
                ConvertToOracleDate(collection["BonusRecommendationModel.AccomplishmentStartDate"]), //a_start_date
                ConvertToOracleDate(collection["BonusRecommendationModel.AccomplishmentEndDate"]), //a_end_date
                collection["BonusRecommendationModel.Justification"], //a_justification
                collection["BonusRecommendationModel.SubmitterName"], //a_submitter_name
                collection["BonusRecommendationModel.SubmitterPosition"], //a_submitter_position
                (collection["BonusRecommendationModel.SubmitterDate"] == null)? null: ConvertToOracleDate(collection["BonusRecommendationModel.SubmitterDate"]), //a_submitted_date
                collection["BonusRecommendationModel.FormSubmitted"], //a_form_submitted
                collection["BonusRecommendationModel.SupervisorSignature"],
                (collection["BonusRecommendationModel.SupervisorEndorsement"].Contains("Y"))?"Y":"N",
                (collection["BonusRecommendationModel.SupervisorSignatureDate"] == null)? null: ConvertToOracleDate(collection["BonusRecommendationModel.SupervisorSignatureDate"]),
                collection["BonusRecommendationModel.DepartmentSignature"],
                collection["BonusRecommendationModel.ChiefSignature"],
                collection["BonusRecommendationModel.HRRepresentative"],
                collection["BonusRecommendationModel.CommitteeApprovalList"],
                collection["BonusRecommendationModel.AgencyDirectorEndorsement"],
                collection["BonusRecommendationModel.CurrentStatus"],
                collection["BonusRecommendationModel.ModifiedBy"]

        };

            string[] prcParamNames = {
                "a_nominee_name",
                "a_nominee_position",
                "a_nominee_department",
                "a_start_date",
                "a_end_date",
                "a_justification",
                "a_submitter_name",
                "a_submitter_position",
                "a_submitted_date",
                "a_form_submitted",
                "a_sup_sig",
                "a_sup_end",
                "a_sup_sig_date",
                "a_dir_sig",
                "a_chief_sig",
                "a_hr_sig",
                "a_com_app_list",
                "a_wsi_dir_sig",
                "a_curr_status",
                "a_modified_by"
            };

            DataTable procedureReturn = dal.ExecuteProcedureOutCursor(prcName, prcParamNames, prcParams, 1);


            int bonusFormId = int.Parse(procedureReturn.Rows[0][0].ToString());

            return bonusFormId;
        }
        public void UpdateSFN60026(System.Web.Mvc.FormCollection collection, int id)
        {
            string prcName = "PKG_SFN60026.PRC_UPDATE_SFN60026";
            string[] prcParams = {
                id.ToString(), //a_id_number
                collection["BonusRecommendationModel.NomineeName"], //a_nominee_name
                collection["BonusRecommendationModel.NomineePosition"], //a_nominee_position
                collection["BonusRecommendationModel.NomineeDepartment"], //a_nominee_department
                ConvertToOracleDate(collection["BonusRecommendationModel.AccomplishmentStartDate"]), //a_start_date
                ConvertToOracleDate(collection["BonusRecommendationModel.AccomplishmentEndDate"]), //a_end_date
                collection["BonusRecommendationModel.Justification"], //a_justification
                collection["BonusRecommendationModel.SubmitterName"], //a_submitter_name
                collection["BonusRecommendationModel.SubmitterPosition"], //a_submitter_position
                (collection["BonusRecommendationModel.SubmitterDate"] == null)? null: ConvertToOracleDate(collection["BonusRecommendationModel.SubmitterDate"]), //a_submitted_date
                (collection["BonusRecommendationModel.FormSubmitted"].Contains("Y"))?"Y":"N", //a_form_submitted
                collection["BonusRecommendationModel.SupervisorSignature"],
                (collection["BonusRecommendationModel.SupervisorSignatureDate"] == null)? null: ConvertToOracleDate(collection["BonusRecommendationModel.SupervisorSignatureDate"]),
                (collection["BonusRecommendationModel.SupervisorEndorsement"].Contains("Y"))?"Y":"N",
                collection["BonusRecommendationModel.DepartmentSignature"],
                (collection["BonusRecommendationModel.DepartmentSignatureDate"] == null)? null: ConvertToOracleDate(collection["BonusRecommendationModel.DepartmentSignatureDate"]),
                (collection["BonusRecommendationModel.DepartmentEndorsement"].Contains("Y"))?"Y":"N",
                collection["BonusRecommendationModel.ChiefSignature"],
                (collection["BonusRecommendationModel.ChiefSignatureDate"] == null)? null: ConvertToOracleDate(collection["BonusRecommendationModel.ChiefSignatureDate"]),
                collection["BonusRecommendationModel.HRRepresentative"],
                (collection["BonusRecommendationModel.HRRepresentativeDate"] == null)? null: ConvertToOracleDate(collection["BonusRecommendationModel.HRRepresentativeDate"]),
                collection["BonusRecommendationModel.CommitteeApprovalList"],
                (collection["BonusRecommendationModel.CommitteeApprovalDate"] == null)? null: ConvertToOracleDate(collection["BonusRecommendationModel.CommitteeApprovalDate"]),
                collection["BonusRecommendationModel.ChiefEndorsement"],
                (collection["BonusRecommendationModel.ChiefEndorsementDate"] == null)? null: ConvertToOracleDate(collection["BonusRecommendationModel.ChiefEndorsementDate"]),
                collection["BonusRecommendationModel.AgencyDirectorEndorsement"],
                (collection["BonusRecommendationModel.AgencyDirectorEndorsementDate"] == null)? null: ConvertToOracleDate(collection["BonusRecommendationModel.AgencyDirectorEndorsementDate"]),
                collection["BonusRecommendationModel.CurrentStatus"],
                collection["BonusRecommendationModel.ModifiedBy"]
          };

            string[] prcParamNames = {
                "a_id_number",
                "a_nominee_name",
                "a_nominee_position",
                "a_nominee_department",
                "a_start_date",
                "a_end_date",
                "a_justification",
                "a_submitter_name",
                "a_submitter_position",
                "a_submitted_date",
                "a_form_submitted",
                "a_sup_sig",
                "a_sup_sig_date",
                "a_sup_end",
                "a_dir_sig",
                "a_dir_sig_date",
                "a_dir_end",
                "a_chief_sig",
                "a_chief_sig_date",
                "a_hr_sig",
                "a_hr_sig_date",
                "a_com_list",
                "a_com_list_date",
                "a_chief_endorse",
                "a_chief_endorse_date",
                "a_wsi_dir_sig",
                "a_wsi_dir_sig_date",
                "a_curr_status",
                "a_modified_by"
           };

            dal.ExecuteProcedure(prcName, prcParamNames, prcParams);
        }
        public void UpdateSFN60026Init(System.Web.Mvc.FormCollection collection, int id)
        {
            string prcName = "PKG_SFN60026.PRC_UPDATE_SFN60026_INIT";
            string[] prcParams = {
                id.ToString(), //a_id_number
                collection["BonusRecommendationModel.NomineeName"], //a_nominee_name
                collection["BonusRecommendationModel.NomineePosition"], //a_nominee_position
                collection["BonusRecommendationModel.NomineeDepartment"], //a_nominee_department
                ConvertToOracleDate(collection["BonusRecommendationModel.AccomplishmentStartDate"]), //a_start_date
                ConvertToOracleDate(collection["BonusRecommendationModel.AccomplishmentEndDate"]), //a_end_date
                collection["BonusRecommendationModel.Justification"], //a_justification
                collection["BonusRecommendationModel.SubmitterName"], //a_submitter_name
                collection["BonusRecommendationModel.SubmitterPosition"], //a_submitter_position
                (collection["BonusRecommendationModel.SubmitterDate"] == null)? null: ConvertToOracleDate(collection["BonusRecommendationModel.SubmitterDate"]), //a_submitted_date
                (collection["BonusRecommendationModel.FormSubmitted"] == "Y")? "Y": "N", //a_form_submitted
                collection["BonusRecommendationModel.SupervisorSignature"],
                (collection["BonusRecommendationModel.SupervisorEndorsement"].Contains("Y"))?"Y":"N",
                (collection["BonusRecommendationModel.SupervisorSignatureDate"] == null)? null: ConvertToOracleDate(collection["BonusRecommendationModel.SupervisorSignatureDate"]),
                collection["BonusRecommendationModel.DepartmentSignature"],
                collection["BonusRecommendationModel.ChiefSignature"],
                collection["BonusRecommendationModel.HRRepresentative"],
                collection["BonusRecommendationModel.CommitteeApprovalList"],
                collection["BonusRecommendationModel.AgencyDirectorEndorsement"],
                collection["BonusRecommendationModel.CurrentStatus"],
                collection["BonusRecommendationModel.ModifiedBy"]

        };

            string[] prcParamNames = {
                "a_id_number",
                "a_nominee_name",
                "a_nominee_position",
                "a_nominee_department",
                "a_start_date",
                "a_end_date",
                "a_justification",
                "a_submitter_name",
                "a_submitter_position",
                "a_submitted_date",
                "a_form_submitted",
                "a_sup_sig",
                "a_sup_end",
                "a_sup_sig_date",
                "a_dir_sig",
                "a_chief_sig",
                "a_hr_sig",
                "a_com_app_list",
                "a_wsi_dir_sig",
                "a_curr_status",
                "a_modified_by"
            };

            dal.ExecuteProcedure(prcName, prcParamNames, prcParams);
        }
        public void UpdateHRSFN60026(System.Web.Mvc.FormCollection collection)
        {
            string prcName = "PKG_SFN60026.PRC_UPDATE_HR_SFN60026";
            string[] prcParams = {
                collection["BonusRecommendationModel.ID"],
                (collection["BonusRecommendationModel.StateEmployeeOneYear"] == "true")?"Y":"N",
                (collection["BonusRecommendationModel.ProbationaryEmployee"].Contains("true"))?"Y":"N",
                (collection["BonusRecommendationModel.FullTime"].Contains("true"))?"Y":"N",
                (collection["BonusRecommendationModel.PartTime"].Contains("true"))?"Y":"N",
                (collection["BonusRecommendationModel.Temporary"].Contains("true"))?"Y":"N",
                (collection["BonusRecommendationModel.LastBonusDate"].Equals(""))? ConvertToOracleDate("01/01/2000"):ConvertToOracleDate(collection["BonusRecommendationModel.LastBonusDate"]),
                collection["BonusRecommendationModel.LastBonusAmount"],
                collection["BonusRecommendationModel.LastPerformanceScore"],
                (collection["BonusRecommendationModel.HRAction"] == "true")?"Y":"N",
                (collection["BonusRecommendationModel.MeetsRequirements"] == "true")?"Y":"N",
                collection["BonusRecommendationModel.ModifiedBy"]
            };

            string[] prcParamNames = {
                "a_id_number",
                "a_state_oneyr",
                "a_probationary",
                "a_full_time",
                "a_part_time",
                "a_temporary",
                "a_last_bonus_date",
                "a_last_bonus_amount",
                "a_last_performance_score",
                "a_hr_action",
                "a_meets_requirements",
                "a_modified_by"
            };

            dal.ExecuteProcedure(prcName, prcParamNames, prcParams);
        }
        public void UpdateCommitteeSFN60026(System.Web.Mvc.FormCollection collection)
        {
            string prcName = "PKG_SFN60026.PRC_UPDATE_COMMITTEE_SFN60026";
            string[] prcParams = {
                collection["BonusRecommendationModel.ID"],
                (collection["BonusRecommendationModel.CommitteeApproval"] == "Yes")?"Y":(collection["BonusRecommendationModel.CommitteeApproval"] == "No") ? "N": "",
                collection["BonusRecommendationModel.CommitteeApprovalAmount"],
                collection["BonusRecommendationModel.Comments"],
                collection["BonusRecommendationModel.ModifiedBy"]
            };

            string[] prcParamNames = {
                "a_id_number",
                "a_committee_approval",
                "a_committee_approval_amount",
                "a_comments",
                "a_modified_by"
            };

            dal.ExecuteProcedure(prcName, prcParamNames, prcParams);
        }
        public bool ApproveSFN60026(System.Web.Mvc.FormCollection collection, int id, int stage)
        {
            bool retVal = false;
            string dbField = "";
            string dbValue = "";
            string nextField = "";
            string nextValue = "";
            string thirdField = "";
            string thirdValue = "";
            switch (stage)
            {
                case 2:
                    dbField = "SUPERVISOR_SIGNATURE_DATE";
                    dbValue = collection["BonusRecommendationModel.SupervisorSignatureDate"];
                    nextField = "DIRECTOR_SIGNATURE";
                    nextValue = collection["BonusRecommendationModel.DepartmentSignature"];
                    thirdField = "SUPERVISOR_ENDORSEMENT";
                    thirdValue = collection["BonusRecommendationModel.SupervisorEndorsement"];
                    break;
                case 3:
                    dbField = "DIRECTOR_SIGNATURE_DATE";
                    dbValue = collection["BonusRecommendationModel.DepartmentSignatureDate"];
                    nextField = "CHIEF_SIGNATURE";
                    nextValue = collection["BonusRecommendationModel.ChiefSignature"];
                    thirdField = "DIRECTOR_ENDORSEMENT";
                    thirdValue = collection["BonusRecommendationModel.DepartmentEndorsement"];
                    break;
                case 4:
                    dbField = "CHIEF_SIGNATURE_DATE";
                    dbValue = collection["BonusRecommendationModel.ChiefSignatureDate"];
                    nextField = "HR_SIGNATURE";
                    nextValue = collection["BonusRecommendationModel.HRRepresentative"];
                    break;
                case 5:
                    dbField = "HR_SIGNATURE_DATE";
                    dbValue = collection["BonusRecommendationModel.HRRepresentativeDate"];
                    nextField = "COMMITTEE_APPROVAL_LIST";
                    nextValue = collection["BonusRecommendationModel.CommitteeApprovalList"];
                    break;
                case 6:
                    dbField = "COMMITTEE_APPROVAL_DATE";
                    dbValue = collection["BonusRecommendationModel.CommitteeApprovalDate"];
                    nextField = "CHIEF_ENDORSEMENT";
                    nextValue = collection["BonusRecommendationModel.ChiefEndorsement"];
                    break;
                case 7:
                    dbField = "CHIEF_ENDORSEMENT_DATE";
                    dbValue = collection["BonusRecommendationModel.ChiefEndorsementDate"];
                    nextField = "AGENCY_DIRECTOR_ENDORSEMENT";
                    nextValue = collection["BonusRecommendationModel.AgencyDirectorEndorsement"]; ;
                    break;
                case 8:
                    dbField = "AGENCY_DIRECTOR_ENDORSEMENT_DATE";
                    dbValue = collection["BonusRecommendationModel.AgencyDirectorEndorsementDate"];
                    nextField = "";
                    nextValue = "";
                    break;
            }
            string sql = "UPDATE WEBFORMS.SFN60026 SET id_number = " + id.ToString() + ", " + dbField + " = TO_DATE('" + dbValue + "', 'yyyy-mm-dd'), " + ((!nextField.Equals(""))?nextField + " = '" + nextValue + "',":"") + ((!thirdField.Equals("")) ? thirdField + " = '" + thirdValue + "'," : "") + " CURRENT_STATUS = '" + stage + "'  WHERE id_number = " + id.ToString();            
            retVal = dal.ExecuteUpdate(sql);
            return retVal;
        }
        public bool DenySFN60026(System.Web.Mvc.FormCollection collection, int id)
        {
            bool retVal = false;
            string dbField = "";
            string dateInsertString = "";
            if (!collection.AllKeys.Contains("BonusRecommendationModel.SupervisorSignatureDate"))
            {
                dateInsertString = "SUPERVISOR_SIGNATURE_DATE = TO_DATE('" + DateTime.Today.ToString("yyyy-MM-dd") + "','yyyy-mm-dd'), SUPERVISOR_ENDORSEMENT = 'N', ";
            }
            else if (!collection.AllKeys.Contains("BonusRecommendationModel.DepartmentSignatureDate"))
            {
                dateInsertString = "DIRECTOR_SIGNATURE_DATE = TO_DATE('" + DateTime.Today.ToString("yyyy-MM-dd") + "','yyyy-mm-dd'), DIRECTOR_ENDORSEMENT = 'N', ";
            }
            else if (!collection.AllKeys.Contains("BonusRecommendationModel.ChiefSignatureDate"))
            {
                dbField = "CHIEF_SIGNATURE_DATE";
            }
            else if (!collection.AllKeys.Contains("BonusRecommendationModel.HRRepresentativeDate"))
            {
                dbField = "HR_SIGNATURE_DATE";
            }
            else if (!collection.AllKeys.Contains("BonusRecommendationModel.CommitteeApprovalDate"))
            {
                dateInsertString = "COMMITTEE_APPROVAL_DATE = TO_DATE('" + DateTime.Today.ToString("yyyy-MM-dd") + "','yyyy-mm-dd'), ELIGIBILITY_COMMITTEE_APPROVAL = 'N', ";
            }
            else if (!collection.AllKeys.Contains("BonusRecommendationModel.ChiefEndorsementDate"))
            {
                dbField = "CHIEF_ENDORSEMENT_DATE";
            }
            else if (!collection.AllKeys.Contains("BonusRecommendationModel.AgencyDirectorEndorsementDate"))
            {
                dbField = "AGENCY_DIRECTOR_ENDORSEMENT_DATE";
            }
            if (dbField != "")
                dateInsertString = dbField + " = TO_DATE('" + DateTime.Today.ToString("yyyy-MM-dd") + "','yyyy-mm-dd'), ";

            string sql = "UPDATE WEBFORMS.SFN60026 SET id_number = " + id.ToString() + ", FORM_DENIED = 'Y', " + dateInsertString +
                "CURRENT_STATUS = '9', MODIFIED_BY = '" + collection["BonusRecommendationModel.ModifiedBy"] + "'  WHERE id_number = " + id.ToString();
            retVal = dal.ExecuteUpdate(sql);
            return retVal;
        }
    }
}