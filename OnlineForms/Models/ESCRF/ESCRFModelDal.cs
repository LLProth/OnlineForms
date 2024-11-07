using System;
using System.Data;
using WSI.Utility.Database;


namespace OnlineForms.Models.ESCRF
{
    public class ESCRFModelDal
    {
        IDatabaseService dal;

        public ESCRFModelDal()
        {
            dal = new DatabaseService("webformsdev");
        }


        public void SendEmail(string recipient, string from, string subject, string body)
        {
            string prcName = "MED.PRC_EMAIL";
            object[] prcParams = { recipient, from, subject, body };
            string[] prcParamNames = { "a_recipient", "a_from", "a_subject", "a_body" };

            dal = new DatabaseService("webformsdev");
            dal.ExecuteProcedure(prcName, prcParamNames, prcParams);
        }

        // ESCRF
        public int InsertESCRFNewHireGetID(System.Web.Mvc.FormCollection collection)
        {

            string prcName = "PKG_ESCRF.PRC_INSRT_ESCRF_NEW_HIRE";

            object[] prcParams =
        {
                collection["NewHire.FirstName"],
                collection["NewHire.LastName"],
                collection["NewHire.MiddleInitial"],
                collection["NewHire.JobTitle"],
                ConvertToOracleDate(collection["NewHire.EffectiveDate"]),
                collection["NewHire.CurrentSupervisor"],
                collection["NewHire.EmployeeType"],
                ConvertCheckboxBoolean((collection["NewHire.TransferringFromAgency"])),
                collection["NewHire.FLSAStatus"],
                collection["NewHire.Comments"],
                collection["NewHire.ModifiedBy"],
                ConvertToOracleDate(collection["NewHire.CreatedDate"]),
                collection["NewHire.ChangeType"],
                int.Parse(collection["NewHire.TaskListID"]).ToString(),
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
                "a_task_list_id",
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

        public DataTable GetESCRFNewHireInfoByTaskListID(int taskListID)
        {
            string prcName = "PKG_ESCRF.PRC_SELECT_ESCRF_NEW_HIRE_INFO_BY_TASK_LIST_ID";
            object[] prcParams = { taskListID };
            string[] prcParamNames = { "a_task_list_id" };
            DataTable dtFormInfo = dal.ExecuteProcedureOutCursor(prcName, prcParamNames, prcParams, 1);

            return dtFormInfo;
        }

        public void UpdateNewHireDates(System.Web.Mvc.FormCollection collection)
        {
            string prcname = "pkg_escrf.prc_update_escrf_new_hire_dates";
            object[] prcparams =
            {
                collection["NewHire.id"],
                collection["NewHire.EffectiveDate"],
                collection["NewHire.FirstName"],
                collection["NewHire.LastName"],
                collection["NewHire.MiddleInitial"],
                collection["NewHire.EmployeeType"],
                collection["NewHire.FLSAStatus"],
                ConvertCheckboxBoolean((collection["NewHire.TransferringFromAgency"]))
            };
            string[] prcparamnames =
            {
                "a_id_number",
                "a_effective_date",
                "a_first_name",
                "a_last_name",
                "a_middle_initial",
                "a_employment_type",
                "a_flsa_status",
                "a_transferring_from_agency"
            };
            dal.ExecuteProcedure(prcname, prcparamnames, prcparams);
        }

        public void UpdateNewHireTaskListId(int reqID, int taskListID)
        {
            string prcName = "PKG_ESCRF.PRC_UPDATE_ESCRF_NEW_HIRE_TASK_LIST_ID";
            object[] prcParams = { reqID, taskListID };
            string[] prcParamNames = { "a_id_number", "a_task_list_id" };
            dal.ExecuteProcedure(prcName, prcParamNames, prcParams);
        }

        public int InsertESCRFTerminationGetID(System.Web.Mvc.FormCollection collection)
        {
            string prcName = "PKG_ESCRF.PRC_INSRT_ESCRF_TERMINATION";
            object[] prcParams =
            {
                collection["Termination.EmployeeName"],
                ConvertToOracleDate(collection["Termination.EffectiveDate"]),
                collection["Termination.CurrentSupervisor"],
                collection["Termination.PhoneNumber"],
                ConvertToOracleDate(collection["Termination.LastDateWorked"]),
                collection["Termination.Department"],
                collection["Termination.OfficeLocation"],
                ConvertCheckboxBoolean((collection["Termination.TransferringToAgency"])),
                collection["Termination.ModifiedBy"],
                ConvertToOracleDate(collection["Termination.CreatedDate"]),
                collection["Termination.ChangeType"],
                int.Parse(collection["Termination.TaskListID"]).ToString(),
                collection["Termination.Comments"]
            };

            string[] prcParamNames =
            {
                "a_employee_name",
                "a_effective_date",
                "a_current_supervisor",
                "a_phone_number",
                "a_last_day_worked",
                "a_department",
                "a_office_location",
                "a_transferring_to_agency",
                "a_modified_by",
                "a_created_date",
                "a_change_type",
                "a_task_list_id",
                "a_comments"
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

        public DataTable GetESCRFTerminationInfoByTaskListID(int taskListID)
        {
            string prcName = "PKG_ESCRF.PRC_SELECT_ESCRF_TERMINATION_INFO_BY_TASK_LIST_ID";
            object[] prcParams = { taskListID };
            string[] prcParamNames = { "a_task_list_id" };
            DataTable dtFormInfo = dal.ExecuteProcedureOutCursor(prcName, prcParamNames, prcParams, 1);

            return dtFormInfo;
        }

        public void UpdateTerminationDates(System.Web.Mvc.FormCollection collection)
        {
            string prcName = "PKG_ESCRF.PRC_UPDATE_ESCRF_TERMINATION_DATES";
            object[] prcParams = {
            collection["Termination.ID"],
            ConvertToOracleDate(collection["Termination.EffectiveDate"]),
            ConvertToOracleDate(collection["Termination.LastDateWorked"]),
            ConvertCheckboxBoolean(collection["Termination.TransferringToAgency"]),
            };
            string[] prcParamNames = { "a_id_number", "a_effective_date", "a_last_date_worked", "a_transferring_to_agency" };
            dal.ExecuteProcedure(prcName, prcParamNames, prcParams);
        }

        public void UpdateTerminationTaskListId(int reqID, int taskListID)
        {
            string prcName = "PKG_ESCRF.PRC_UPDATE_ESCRF_TERMINATION_TASK_LIST_ID";
            object[] prcParams = { reqID, taskListID };
            string[] prcParamNames = { "a_id_number", "a_task_list_id" };
            dal.ExecuteProcedure(prcName, prcParamNames, prcParams);
        }

        public int InsertESCRFChangeGetID(System.Web.Mvc.FormCollection collection)
        {
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
                int.Parse(collection["Change.TaskListID"].ToString()),
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
                "a_task_list_id",
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

        public DataTable GetESCRFChangeInfoByTaskListID(int taskListID)
        {
            string prcName = "PKG_ESCRF.PRC_SELECT_ESCRF_CHANGE_INFO_BY_TASK_LIST_ID";
            object[] prcParams = { taskListID };
            string[] prcParamNames = { "a_task_list_id" };
            DataTable dtFormInfo = dal.ExecuteProcedureOutCursor(prcName, prcParamNames, prcParams, 1);

            return dtFormInfo;
        }

        public void UpdateChangeDates(System.Web.Mvc.FormCollection collection)
        {
            string prcName = "PKG_ESCRF.PRC_UPDATE_ESCRF_CHANGE_DATES";
            object[] prcParams = {
            collection["Change.ID"],
            ConvertToOracleDate(collection["Change.EffectiveDate"]),
            collection["Change.FLSAStatus"]
            };
            string[] prcParamNames = { "a_id_number", "a_effective_date", "a_flsa_status" };
            dal.ExecuteProcedure(prcName, prcParamNames, prcParams);
        }

        public void UpdateChangeTaskListId(int reqID, int taskListID)
        {
            string prcName = "PKG_ESCRF.PRC_UPDATE_ESCRF_CHANGE_TASK_LIST_ID";
            object[] prcParams = { reqID, taskListID };
            string[] prcParamNames = { "a_id_number", "a_task_list_id" };
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
                int.Parse(collection["Name.TaskListID"].ToString()),
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
                "a_task_list_id",
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

        public void UpdateNameTaskListId(int reqID, int taskListID)
        {
            string prcName = "PKG_ESCRF.PRC_UPDATE_ESCRF_NAME_TASK_LIST_ID";
            object[] prcParams = { reqID, taskListID };
            string[] prcParamNames = { "a_id_number", "a_task_list_id" };
            dal.ExecuteProcedure(prcName, prcParamNames, prcParams);
        }

        public DataTable GetESCRFNameInfoByTaskListID(int taskListID)
        {
            string prcName = "PKG_ESCRF.PRC_SELECT_ESCRF_NAME_INFO_BY_TASK_LIST_ID";
            object[] prcParams = { taskListID };
            string[] prcParamNames = { "a_task_list_id" };
            DataTable dtFormInfo = dal.ExecuteProcedureOutCursor(prcName, prcParamNames, prcParams, 1);

            return dtFormInfo;
        }

        public void UpdateNameDates(System.Web.Mvc.FormCollection collection)
        {
            string prcName = "PKG_ESCRF.PRC_UPDATE_ESCRF_NAME_DATES";
            object[] prcParams = {
            collection["Name.ID"],
            ConvertToOracleDate(collection["Name.EffectiveDate"]),
            collection["Name.FirstName"],
            collection["Name.LastName"],
            collection["Name.MiddleInitial"]
            };
            string[] prcParamNames = { "a_id_number", "a_effective_date", "a_first_name", "a_last_name", "a_middle_initial" };
            dal.ExecuteProcedure(prcName, prcParamNames, prcParams);
        }

        public int InsertTaskListGetID(string changeType)
        {
            string prcName = "PKG_ESCRF.PRC_INSRT_ESCRF_TASK_LIST";
            object[] prcParams =
            {
            DateTime.MinValue.ToShortDateString(),
            DateTime.MinValue.ToShortDateString(),
            changeType,
            "n"

            };

            string[] prcParamNames =
            {
                "a_created_date",
                "a_finished_date",
                "a_change_type",
                "is_deployed"

            };

            DataTable procedureReturn = dal.ExecuteProcedureOutCursor(prcName, prcParamNames, prcParams, 1);

            int requisitionID = int.Parse(procedureReturn.Rows[0][0].ToString());

            return requisitionID;

        }

        public DataTable GetTaskListInfo()
        {
            string prcName = "PKG_ESCRF.PRC_SELECT_ESCRF_TASK_LIST_INFO";
            object[] prcParams = { };
            string[] prcParamNames = { };
            DataTable dtFormInfo = dal.ExecuteProcedureOutCursor(prcName, prcParamNames, prcParams, 1);

            return dtFormInfo;
        }

        public DataTable GetTaskListInfoByID(int reqID)
        {
            string prcName = "PKG_ESCRF.PRC_SELECT_ESCRF_TASK_LIST_INFO_BY_ID";
            object[] prcParams = { reqID };
            string[] prcParamNames = { "a_id_number" };
            DataTable dtFormInfo = dal.ExecuteProcedureOutCursor(prcName, prcParamNames, prcParams, 1);

            return dtFormInfo;
        }

        public void UpdateTaskListCreatedDate(int reqID)
        {
            DateTime now = DateTime.Now;
            string prcName = "PKG_ESCRF.PRC_UPDATE_ESCRF_TASK_LIST_CREATED_DATE";
            object[] prcParams = { reqID, ConvertToOracleDate(now.ToShortDateString()) };
            string[] prcParamNames = { "a_id_number", "a_created_date" };
            dal.ExecuteProcedure(prcName, prcParamNames, prcParams);
        }

        public void UpdateTaskListFinishedDate(int reqID)
        {
            DateTime now = DateTime.Now;
            string prcName = "PKG_ESCRF.PRC_UPDATE_ESCRF_TASK_LIST_FINISHED_DATE";
            object[] prcParams = { reqID, ConvertToOracleDate(now.ToShortDateString()) };
            string[] prcParamNames = { "a_id_number", "a_finished_date" };
            dal.ExecuteProcedure(prcName, prcParamNames, prcParams);
        }

        public void UpdateTaskListDeployed(int reqID)
        {
            string prcName = "PKG_ESCRF.PRC_UPDATE_ESCRF_TASK_LIST_DEPLOYED";
            object[] prcParams = { reqID, "Y" };
            string[] prcParamNames = { "a_id_number", "a_is_deployed" };
            dal.ExecuteProcedure(prcName, prcParamNames, prcParams);
        }

        //Used when adding a custom task
        public int InsertTaskListItemGetID(System.Web.Mvc.FormCollection collection)
        {
            string prcName = "PKG_ESCRF.PRC_INSRT_ESCRF_TASK_LIST_ITEM";
            object[] prcParams =
            {
                collection["TaskListItem.Task"],
                collection["TaskListItem.Deputy"],
                ConvertToOracleDate(collection["TaskListItem.SignedOn"]),
                collection["TaskListItem.TaskListID"],
                collection["TaskListItem.Dept"]
            };

            string[] prcParamNames =
            {
                "a_task",
                "a_deputy",
                "a_signed_on",
                "a_task_list_id",
                "a_dept"
            };

            DataTable procedureReturn = dal.ExecuteProcedureOutCursor(prcName, prcParamNames, prcParams, 1);

            int requisitionID = int.Parse(procedureReturn.Rows[0][0].ToString());

            return requisitionID;

        }
        //used when populating the tasklist with default tasks where a group is the deputy Example: ERGO, HR
        public int InsertTaskListItemGetIDManual(string a_task, string a_deputy, string a_signed_on, int a_task_list_id, string a_dept)
        {
            string prcName = "PKG_ESCRF.PRC_INSRT_ESCRF_TASK_LIST_ITEM";
            object[] prcParams =
            {
                a_task,
                a_deputy,
                a_signed_on,
                a_task_list_id,
                a_dept,
            };

            string[] prcParamNames =
            {
                "a_task",
                "a_deputy",
                "a_signed_on",
                "a_task_list_id",
                "a_dept",
            };

            DataTable procedureReturn = dal.ExecuteProcedureOutCursor(prcName, prcParamNames, prcParams, 1);

            int requisitionID = int.Parse(procedureReturn.Rows[0][0].ToString());

            return requisitionID;
        }
        ////used when populating the tasklist with default tasks where a person is a deputy example: Holland, Jordan M., Hall, Brad W.
        public int InsertTaskListItemGetIDManualWithEmail(string a_task, string a_deputy, string a_signed_on, int a_task_list_id, string a_dept, string a_dep_email)
        {
            string prcName = "PKG_ESCRF.PRC_INSRT_ESCRF_TASK_LIST_ITEM_WITH_EMAIL";
            object[] prcParams =
            {
                a_task,
                a_deputy,
                a_signed_on,
                a_task_list_id,
                a_dept,
                a_dep_email
            };

            string[] prcParamNames =
            {
                "a_task",
                "a_deputy",
                "a_signed_on",
                "a_task_list_id",
                "a_dept",
                "a_dep_email",
            };

            DataTable procedureReturn = dal.ExecuteProcedureOutCursor(prcName, prcParamNames, prcParams, 1);

            int requisitionID = int.Parse(procedureReturn.Rows[0][0].ToString());

            return requisitionID;
        }

        public DataTable GetTaskListItemInfo()
        {
            string prcName = "PKG_ESCRF.PRC_SELECT_ESCRF_TASK_LIST_ITEM_INFO";
            object[] prcParams = { };
            string[] prcParamNames = { };
            DataTable dtFormInfo = dal.ExecuteProcedureOutCursor(prcName, prcParamNames, prcParams, 1);

            return dtFormInfo;
        }

        public DataTable GetTaskListItemInfoByID(int reqID)
        {
            string prcName = "PKG_ESCRF.PRC_SELECT_ESCRF_TASK_LIST_ITEM_INFO_BY_ID";
            object[] prcParams = { reqID };
            string[] prcParamNames = { "a_id_number" };
            DataTable dtFormInfo = dal.ExecuteProcedureOutCursor(prcName, prcParamNames, prcParams, 1);

            return dtFormInfo;
        }

        public DataTable GetTaskListItemInfoByTaskListID(int taskListID)
        {
            string prcName = "PKG_ESCRF.PRC_SELECT_ESCRF_TASK_LIST_ITEM_INFO_BY_TASK_LIST_ID";
            object[] prcParams = { taskListID };
            string[] prcParamNames = { "a_task_list_id" };
            DataTable dtFormInfo = dal.ExecuteProcedureOutCursor(prcName, prcParamNames, prcParams, 1);

            return dtFormInfo;
        }

        public DataTable GetTaskListItemInfoByDeputy(string deputy)
        {
            string prcName = "PKG_ESCRF.PRC_SELECT_ESCRF_TASK_LIST_ITEM_INFO_BY_DEPUTY";
            string[] prcParams = { deputy };
            string[] prcParamNames = { "a_deputy" };
            DataTable dtFormInfo = dal.ExecuteProcedureOutCursor(prcName, prcParams, prcParamNames, 1);

            return dtFormInfo;
        }

        public void UpdateTaskListItem(int reqID)
        {
            DateTime now = DateTime.Now;
            string prcName = "PKG_ESCRF.PRC_UPDATE_ESCRF_TASK_LIST_ITEM";
            object[] prcParams = { reqID, ConvertToOracleDate(now.ToShortDateString()) };
            string[] prcParamNames = { "a_id_number", "a_signed_on" };
            dal.ExecuteProcedure(prcName, prcParamNames, prcParams);
        }

        public void UpdateTaskListItemWithCompleted(int reqID)
        {
            DateTime now = DateTime.Now;
            string completed = "Y";
            string prcName = "PKG_ESCRF.PRC_UPDATE_ESCRF_TASK_LIST_ITEM_WITH_COMPLETED";
            object[] prcParams = { reqID, ConvertToOracleDate(now.ToShortDateString()), completed };
            string[] prcParamNames = { "a_id_number", "a_signed_on", "a_completed" };
            dal.ExecuteProcedure(prcName, prcParamNames, prcParams);
        }

        public void EditTaskListItem(System.Web.Mvc.FormCollection collection)
        {
            string prcName = "PKG_ESCRF.PRC_EDIT_ESCRF_TASK_LIST_ITEM";
            object[] prcParams =
            {
                collection["TaskListItem.ID"],
                collection["TaskListItem.Task"],
                collection["TaskListItem.Deputy"],
            };
            string[] prcParamNames =
            {
                "a_id_number",
                 "a_task",
                "a_deputy"
            };
            dal.ExecuteProcedure(prcName, prcParamNames, prcParams);

        }

        public void DeleteTaskListItemByID(int reqID)
        {
            string prcName = "PKG_ESCRF.PRC_DELETE_ESCRF_TASK_LIST_ITEM_BY_ID";
            object[] prcParams = { reqID };
            string[] prcParamNames = { "a_id_number" };
            dal.ExecuteProcedure(prcName, prcParamNames, prcParams);
        }

        public DataTable GetDefaultByChangeType(string changeType)
        {
            string prcName = "PKG_ESCRF.PRC_SELECT_ESCRF_DEFAULT_INFO_CHANGETYPE";
            string[] prcParams = { changeType };
            string[] prcParamNames = { "a_change_type" };
            DataTable dtDefaultInfo = dal.ExecuteProcedureOutCursor(prcName, prcParamNames, prcParams, 1);

            return dtDefaultInfo;
        }

        public DataTable GetWSITitlesInfo()
        {
            string prcName = "PKG_ESCRF.PRC_SELECT_WSI_TITLES";
            object[] prcParams = { };
            string[] prcParamNames = { };
            DataTable dtTitleInfo = dal.ExecuteProcedureOutCursor(prcName, prcParamNames, prcParams, 1);

            return dtTitleInfo;
        }

        public DataTable GetDeptAfilliationInfo()
        {
            string prcname = "PKG_ESCRF.PRC_SELECT_ESCRF_ROLES_INFO";
            object[] prcparams = { };
            string[] prcparamnames = { };
            DataTable dtroles = dal.ExecuteProcedureOutCursor(prcname, prcparamnames, prcparams, 1);

            return dtroles;
        }

        public DataTable GetDeptAfilliation(string employeeName)
        {
            string prcName = "PKG_ESCRF.PRC_SELECT_ESCRF_ROLES";
            object[] prcParams = { employeeName };
            string[] prcParamNames = { "a_employee_name" };
            DataTable dtRoles = dal.ExecuteProcedureOutCursor(prcName, prcParamNames, prcParams, 1);

            return dtRoles;
        }

        public DataTable GetEmployeesByDeptAfilliation(string deptAfilliation)
        {
            string prcName = "PKG_ESCRF.PRC_SELECT_ESCRF_EMPLOYEE_BY_ROLE";
            object[] prcParams = { deptAfilliation };
            string[] prcParamNames = { "a_department_affiliation" };
            DataTable dtEmployee = dal.ExecuteProcedureOutCursor(prcName, prcParamNames, prcParams, 1);

            return dtEmployee;
        }

        public DataTable GetChangelistInfoByChangelistID(int id)
        {
            string prcName = "PKG_ESCRF.PRC_GET_ESCRF_CHANGELIST_INFO_BY_CHANGELIST_ID";
            object[] prcParams = { id };
            string[] prcParamNames = { "a_changelist_id" };
            DataTable dtChangelistInfo = dal.ExecuteProcedureOutCursor(prcName, prcParamNames, prcParams, 1);

            return dtChangelistInfo;
        }

        public DataTable GetTechnologyRequirementsInfoByID(int id)
        {
            string prcName = "PKG_ESCRF.PRC_GET_ESCRF_TECHNOLOGY_REQUIREMENTS_BY_ID";
            object[] prcParams = { id };
            string[] prcParamNames = { "a_id_number" };
            DataTable dtChangelistInfo = dal.ExecuteProcedureOutCursor(prcName, prcParamNames, prcParams, 1);

            return dtChangelistInfo;
        }

        public void UpdateESCRFCompleted(System.Web.Mvc.FormCollection collection)
        {
            string prcName = "PKG_ESCRF.PRC_COMPLETE_ESCRF_TASK";
            object[] prcParams = { collection["TaskListItem.ID"], collection["TaskListItem.CompletedBy"], ConvertToOracleDate(collection["TaskListItem.SignedOn"]), collection["TaskListItem.Comments"], ConvertBooleanToYesNo(Boolean.Parse(collection["TaskListItem.Completed"])), ConvertBooleanToYesNo(Boolean.Parse(collection["TaskListItem.NotApplicable"])) };
            string[] prcParamNames = { "a_id_number", "a_completed_by", "a_signed_on", "a_comments", "a_completed", "a_not_applicable" };

            dal = new DatabaseService("webformsdev");
            dal.ExecuteProcedure(prcName, prcParamNames, prcParams);
        }

        public void UpdateESCRFTechRequirementsCompleted(System.Web.Mvc.FormCollection collection, string name, string date)
        {
            string prcName = "PKG_ESCRF.PRC_COMPLETE_TECH_REQUIREMENT_TASK";
            object[] prcParams = { collection["TechRequirements.ID"], name, date };
            string[] prcParamNames = { "a_id_number", "a_name", "a_date" };

            dal = new DatabaseService("webformsdev");
            dal.ExecuteProcedure(prcName, prcParamNames, prcParams);
        }

        public void UpdateESCRFTechRequirementsCompletedByIS(System.Web.Mvc.FormCollection collection, string name, string date)
        {
            string prcName = "PKG_ESCRF.PRC_COMPLETE_TECH_REQUIREMENT_TASK_IS";
            object[] prcParams = { collection["TechRequirements.ID"], name, date };
            string[] prcParamNames = { "a_id_number", "a_name", "a_date" };

            dal = new DatabaseService("webformsdev");
            dal.ExecuteProcedure(prcName, prcParamNames, prcParams);
        }

        public void InsertTechnologyRequirements(System.Web.Mvc.FormCollection collection)
        {
            string prcName = "PKG_ESCRF.PRC_INSRT_TECH_REQUIREMENTS";

            object[] prcParams =
                {
                    collection["TechRequirements.ID"],
                    ConvertBooleanToYesNo(Boolean.Parse(collection["TechRequirements.EmployeeMoving"])),
                    collection["TechRequirements.EmailGroups"],
                    collection["TechRequirements.FieldSecurity"],
                    ConvertBooleanToYesNo(Boolean.Parse(collection["TechRequirements.NoChange"])),
                    ConvertBooleanToYesNo(Boolean.Parse(collection["TechRequirements.CMS"])),
                    ConvertBooleanToYesNo(Boolean.Parse(collection["TechRequirements.InfoPath"])),
                    ConvertBooleanToYesNo(Boolean.Parse(collection["TechRequirements.MicrosoftReports"])),
                    ConvertBooleanToYesNo(Boolean.Parse(collection["TechRequirements.RecManager"])),
                    ConvertBooleanToYesNo(Boolean.Parse(collection["TechRequirements.GreatPlains"])),
                    ConvertBooleanToYesNo(Boolean.Parse(collection["TechRequirements.AccountingUtility"])),
                    ConvertBooleanToYesNo(Boolean.Parse(collection["TechRequirements.ITWorks"])),
                    ConvertBooleanToYesNo(Boolean.Parse(collection["TechRequirements.FileToFilenet"])),
                    ConvertBooleanToYesNo(Boolean.Parse(collection["TechRequirements.Indexing"])),
                    ConvertBooleanToYesNo(Boolean.Parse(collection["TechRequirements.Verifier"])),
                    ConvertBooleanToYesNo(Boolean.Parse(collection["TechRequirements.SecOfState"])),
                    ConvertBooleanToYesNo(Boolean.Parse(collection["TechRequirements.DOT"])),
                    ConvertBooleanToYesNo(Boolean.Parse(collection["TechRequirements.JobService"])),
                    ConvertBooleanToYesNo(Boolean.Parse(collection["TechRequirements.CAPS"])),
                    ConvertBooleanToYesNo(Boolean.Parse(collection["TechRequirements.Legal"])),
                    ConvertBooleanToYesNo(Boolean.Parse(collection["TechRequirements.MyWSI"])),
                    collection["TechRequirements.Other"],
                    collection["TechRequirements.WorkQueue"],
                    ConvertBooleanToYesNo(Boolean.Parse(collection["TechRequirements.CurrentPhone"])),
                    ConvertBooleanToYesNo(Boolean.Parse(collection["TechRequirements.CallRecording"])),
                    ConvertBooleanToYesNo(Boolean.Parse(collection["TechRequirements.ElectronicSignature"])),
                    collection["TechRequirements.NewLocation"],
                    collection["TechRequirements.NewSignature"]
                };

            string[] prcParamNames =
            {
                    "a_id_number",
                    "a_employee_moving",
                    "a_email_groups",
                    "a_field_security",
                    "a_no_change",
                    "a_cms",
                    "a_info_path",
                    "a_microsoft_reports",
                    "a_recommendation_manager",
                    "a_great_plains",
                    "a_accounting_utility",
                    "a_it_works",
                    "a_file_to_filenet",
                    "a_indexing",
                    "a_verifier",
                    "a_sec_of_state",
                    "a_dot",
                    "a_job_service",
                    "a_caps",
                    "a_legal",
                    "a_mywsi",
                    "a_other",
                    "a_work_queue",
                    "a_current_phone",
                    "a_call_recording",
                    "a_electronic_signature",
                    "a_new_location",
                    "a_new_signature"

                };

            dal = new DatabaseService("webformsdev");
            dal.ExecuteProcedure(prcName, prcParamNames, prcParams);
        }

        public void InsertEmployeeTechnology(System.Web.Mvc.FormCollection collection)
        {
            string prcName = "PKG_ESCRF.PRC_INSRT_EMPLOYEE_TECHNOLOGY";

            object[] prcParams =
                {
                    collection["EmployeeTechnology.ID"],
                    ConvertCheckboxBoolean(collection["EmployeeTechnology.NoAccess"]),
                    ConvertCheckboxBoolean(collection["EmployeeTechnology.Agreement"]),
                    collection["EmployeeTechnology.TransferringTo"],
                    collection["EmployeeTechnology.RemoveEmployee"],
                    collection["EmployeeTechnology.Replaced"],
                    collection["EmployeeTechnology.Comments"],
                };

            string[] prcParamNames =
            {
                    "a_id_number",
                    "a_retain",
                    "a_agreement",
                    "a_transferring_to",
                    "a_remove_employee",
                    "a_replaced",
                    "a_comments"
                };

            dal = new DatabaseService("webformsdev");
            dal.ExecuteProcedure(prcName, prcParamNames, prcParams);
        }

        public void UpdateESCRFEmployeeTechnologyCompleted(int reqID, string name, string date)
        {
            string prcName = "PKG_ESCRF.PRC_COMPLETE_EMPLOYEE_TECHNOLOGY";
            object[] prcParams = { reqID, name, date };
            string[] prcParamNames = { "a_id_number", "a_name", "a_date" };

            dal = new DatabaseService("webformsdev");
            dal.ExecuteProcedure(prcName, prcParamNames, prcParams);
        }

        public DataTable GetEmployeeTechnologyInfoByID(int id)
        {
            string prcName = "PKG_ESCRF.PRC_GET_EMPLOYEE_TECHNOLOGY_BY_ID";
            object[] prcParams = { id };
            string[] prcParamNames = { "a_id_number" };
            DataTable dtChangelistInfo = dal.ExecuteProcedureOutCursor(prcName, prcParamNames, prcParams, 1);

            return dtChangelistInfo;
        }

        public void DeleteESCRFNewHire(int id, string taskListId)
        {
            string prcName = "PKG_ESCRF.PRC_DELETE_NEW_HIRE";
            object[] prcParams = { id, taskListId };
            string[] prcParamNames = { "a_id_number", "a_task_list_id" };

            dal = new DatabaseService("webformsdev");
            dal.ExecuteProcedure(prcName, prcParamNames, prcParams);
        }

        public void DeleteESCRFChange(int id, string taskListId)
        {
            string prcName = "PKG_ESCRF.PRC_DELETE_CHANGE";
            object[] prcParams = { id, taskListId };
            string[] prcParamNames = { "a_id_number", "a_task_list_id" };

            dal = new DatabaseService("webformsdev");
            dal.ExecuteProcedure(prcName, prcParamNames, prcParams);
        }

        public void DeleteESCRFName(int id, string taskListId)
        {
            string prcName = "PKG_ESCRF.PRC_DELETE_NAME";
            object[] prcParams = { id, taskListId };
            string[] prcParamNames = { "a_id_number", "a_task_list_id" };

            dal = new DatabaseService("webformsdev");
            dal.ExecuteProcedure(prcName, prcParamNames, prcParams);
        }

        public void DeleteESCRFTermination(int id, string taskListId)
        {
            string prcName = "PKG_ESCRF.PRC_DELETE_TERMINATION";
            object[] prcParams = { id, taskListId };
            string[] prcParamNames = { "a_id_number", "a_task_list_id" };

            dal = new DatabaseService("webformsdev");
            dal.ExecuteProcedure(prcName, prcParamNames, prcParams);
        }

        public void UpdateChangelistTaskListId(int reqID, int taskListID)
        {
            string prcName = "PKG_ESCRF.PRC_UPDATE_ESCRF_CHANGELIST_TASK_LIST_ID";
            object[] prcParams = { reqID, taskListID };
            string[] prcParamNames = { "a_id_number", "a_task_list_id" };
            dal.ExecuteProcedure(prcName, prcParamNames, prcParams);
        }

        public void InsertChangelistInfoNewHire(System.Web.Mvc.FormCollection collection, int reqID)
        {
            string prcName = "PKG_ESCRF.PRC_INSRT_CHANGELIST_INFO";

            object[] prcParams =
                {
                    reqID,
                    int.Parse(collection["NewHire.TaskListID"]).ToString(),
                    collection["NewHire.ChangeType"],
                    collection["NewHire.LastName"] + ", " + collection["NewHire.FirstName"] + " " + collection["NewHire.MiddleInitial"],
                    ConvertToOracleDate(collection["NewHire.EffectiveDate"])
                };

            string[] prcParamNames =
            {
                    "a_id_number",
                    "a_task_list_id",
                    "a_change_type",
                    "a_employee_name",
                    "a_effective_date"
                };

            dal = new DatabaseService("webformsdev");
            dal.ExecuteProcedure(prcName, prcParamNames, prcParams);
        }

        public void InsertChangelistInfoChange(System.Web.Mvc.FormCollection collection, int reqID)
        {
            string prcName = "PKG_ESCRF.PRC_INSRT_CHANGELIST_INFO";

            object[] prcParams =
                {
                    reqID,
                    int.Parse(collection["Change.TaskListID"]).ToString(),
                    collection["Change.ChangeType"],
                    collection["Change.EmployeeName"],
                    ConvertToOracleDate(collection["Change.EffectiveDate"])
                };

            string[] prcParamNames =
            {
                    "a_id_number",
                    "a_task_list_id",
                    "a_change_type",
                    "a_employee_name",
                    "a_effective_date"
                };

            dal = new DatabaseService("webformsdev");
            dal.ExecuteProcedure(prcName, prcParamNames, prcParams);
        }

        public void InsertChangelistInfoName(System.Web.Mvc.FormCollection collection, int reqID)
        {
            string prcName = "PKG_ESCRF.PRC_INSRT_CHANGELIST_INFO";

            object[] prcParams =
                {
                    reqID,
                    int.Parse(collection["Name.TaskListID"]).ToString(),
                    collection["Name.ChangeType"],
                    collection["Name.EmployeeName"],
                    ConvertToOracleDate(collection["Name.EffectiveDate"])
                };

            string[] prcParamNames =
            {
                    "a_id_number",
                    "a_task_list_id",
                    "a_change_type",
                    "a_employee_name",
                    "a_effective_date"
                };

            dal = new DatabaseService("webformsdev");
            dal.ExecuteProcedure(prcName, prcParamNames, prcParams);
        }

        public void InsertChangelistInfoTermination(System.Web.Mvc.FormCollection collection, int reqID)
        {
            string prcName = "PKG_ESCRF.PRC_INSRT_CHANGELIST_INFO";

            object[] prcParams =
                {
                    reqID,
                    int.Parse(collection["Termination.TaskListID"]).ToString(),
                    collection["Termination.ChangeType"],
                    collection["Termination.EmployeeName"],
                    ConvertToOracleDate(collection["Termination.EffectiveDate"])
                };

            string[] prcParamNames =
            {
                    "a_id_number",
                    "a_task_list_id",
                    "a_change_type",
                    "a_employee_name",
                    "a_effective_date"
                };

            dal = new DatabaseService("webformsdev");
            dal.ExecuteProcedure(prcName, prcParamNames, prcParams);
        }

        public DataTable GetTaskListByDeputy(string name)
        {
            string prcName = "PKG_ESCRF.PRC_SELECT_ESCRF_TASK_LIST_ITEM_INFO_BY_DEPUTY";
            object[] prcParams = { name };
            string[] prcParamNames = { "a_name" };
            DataTable dtFormInfo = dal.ExecuteProcedureOutCursor(prcName, prcParamNames, prcParams, 1);

            return dtFormInfo;
        }

        public DataTable GetNewHireByTasklistID(int id)
        {
            string prcName = "PKG_ESCRF.PRC_GET_ESCRF_NEW_HIRE_BY_TASK_LIST_ID";
            object[] prcParams = { id };
            string[] prcParamNames = { "a_task_list_id" };
            DataTable dtFormInfo = dal.ExecuteProcedureOutCursor(prcName, prcParamNames, prcParams, 1);

            return dtFormInfo;
        }

        public DataTable GetChangeByTasklistID(int id)
        {
            string prcName = "PKG_ESCRF.PRC_GET_ESCRF_CHANGE_BY_TASK_LIST_ID";
            object[] prcParams = { id };
            string[] prcParamNames = { "a_task_list_id" };
            DataTable dtFormInfo = dal.ExecuteProcedureOutCursor(prcName, prcParamNames, prcParams, 1);

            return dtFormInfo;
        }

        public DataTable GetNameChangeByTasklistID(int id)
        {
            string prcName = "PKG_ESCRF.PRC_GET_ESCRF_NAME_CHANGE_BY_TASK_LIST_ID";
            object[] prcParams = { id };
            string[] prcParamNames = { "a_task_list_id" };
            DataTable dtFormInfo = dal.ExecuteProcedureOutCursor(prcName, prcParamNames, prcParams, 1);

            return dtFormInfo;
        }

        public DataTable GetTerminationByTasklistID(int id)
        {
            string prcName = "PKG_ESCRF.PRC_GET_ESCRF_TERMINATION_BY_TASK_LIST_ID";
            object[] prcParams = { id };
            string[] prcParamNames = { "a_task_list_id" };
            DataTable dtFormInfo = dal.ExecuteProcedureOutCursor(prcName, prcParamNames, prcParams, 1);

            return dtFormInfo;
        }

        public DataTable SelectDistinctTasklistIDByDeputy(string deputy)
        {
            string prcName = "PKG_ESCRF.PRC_SELECT_DISTINCT_TASK_LIST_ID_BY_DEPUTY";
            object[] prcParams = { deputy };
            string[] prcParamNames = { "a_deputy" };
            DataTable dtFormInfo = dal.ExecuteProcedureOutCursor(prcName, prcParamNames, prcParams, 1);

            return dtFormInfo;
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
    }
}