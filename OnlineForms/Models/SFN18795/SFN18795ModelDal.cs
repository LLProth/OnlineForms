using Microsoft.Ajax.Utilities;
using OnlineForms.ViewModels;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using WSI.Utility.Database;

namespace OnlineForms.Models.SFN18795
{
    public class SFN18795ModelDal : ModelDAL
	{
        IDatabaseService dal;
        public SFN18795ModelDal()
        {
            dal = new DatabaseService("webformsdev");
        }

        public Dictionary<string, string> GetFormInfo(string path)
        {
            string prcName = "PKG_SFN18795.PRC_SELECT_WEBFORM_BY_PATH";

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

        public void DeleteSFN18795Items(int id, int oldCount, int newCount)
        {
            string prcName = "PKG_SFN18795.PRC_DELETE_SFN18795_ITEMS";
            for (int i = oldCount; i > newCount; i--)
            {
                string[] prcParamNames = { "a_id_number", "a_item_order" };
                object[] prcParams = { id, i };

                dal = new DatabaseService("webformsdev");
                dal.ExecuteProcedure(prcName, prcParamNames, prcParams);
            }
        }

        // SFN18795
        public int InsertSFN18795GetID(System.Web.Mvc.FormCollection collection, bool formsub, string nextApproval, string emplID)
        {
            string prcName = "PKG_SFN18795.PRC_INSRT_SFN18795";
            foreach (var key in collection.AllKeys)
            {
                if (collection[key].IsNullOrWhiteSpace() && key.Contains("Date"))
                {
                    collection[key] = "1111-01-01";
                }
                if (key == "RequisitionModel.TotalPrice" && collection[key].IsNullOrWhiteSpace())
                {
                    collection[key] = "0.00";
                }
            }

            object[] prcParams = {
                collection["RequisitionModel.Name"], //a_request_employee
                emplID,
                collection["RequisitionModel.Department"], //a_department_budget
                ConvertToOracleDate(collection["RequisitionModel.DateSubmitted"]), //a_request_date
                collection["RequisitionModel.Contractor"], //a_suggested_contractor
                ConvertBooleanToYesNo(Boolean.Parse(collection["RequisitionModel.SoftwareHardware"])), //a_software_hardware_request
                ConvertToOracleDate(collection["RequisitionModel.EstimatedStartDate"]), //a_estimated_start_date
                ConvertToOracleDate(collection["RequisitionModel.EstimatedCompleteDate"]), //a_estimated_complete_date
                Double.Parse(collection["RequisitionModel.TotalPrice"]), //a_req_items_total
                ConvertBooleanToYesNo(formsub),
                nextApproval
            };

            string[] prcParamNames = {
                "a_request_employee",
                "a_emplid",
                "a_department_budget",
                "a_request_date",
                "a_suggested_contractor",
                "a_software_hardware_request",
                "a_estimated_start_date",
                "a_estimated_complete_date",
                "a_req_items_total",
                "a_form_submitted",
                "a_waiting_approval"
            };

            DataTable procedureReturn = dal.ExecuteProcedureOutCursor(prcName, prcParamNames, prcParams, 1);

            int requisitionID = int.Parse(procedureReturn.Rows[0][0].ToString());

            return requisitionID;
        }

        public void InsertSFN18795ReqItems(System.Web.Mvc.FormCollection collection, int reqID, int start)
        {
            string prcName = "PKG_SFN18795.PRC_INSRT_SFN18795_REQUISITION_ITEMS";
            string[] reqItemsQuantities = collection.GetValues("RequisitionItem.Quantity");
            string[] reqItemsItemNums = collection.GetValues("RequisitionItem.ItemNumber");
            string[] reqItemsDescs = collection.GetValues("RequisitionItem.Description");
            string[] reqItemsPrices = collection.GetValues("RequisitionItem.Price");
            string[] reqItemsEstCosts = collection.GetValues("RequisitionItem.EstimatedCost");
            string order = "0";

            for (int i = start; i < reqItemsQuantities.Length; i++)
            {
                order = Convert.ToString(i + 1);
                if (reqItemsQuantities[i].IsNullOrWhiteSpace())
                {
                    reqItemsQuantities[i] = "0";
                }
                if (reqItemsPrices[i].IsNullOrWhiteSpace())
                {
                    reqItemsPrices[i] = "0.00";
                }
                if (reqItemsEstCosts[i].IsNullOrWhiteSpace())
                {
                    reqItemsEstCosts[i] = "0.00";
                }
                if (reqItemsDescs[i].IsNullOrWhiteSpace() || reqItemsDescs[i] == "")
                {
                    for (int x = 0; x < i + 1; x++)
                    {
                        reqItemsDescs[i] = reqItemsDescs[i] + " ";
                    }

                }
                object[] prcParams =
                {
                    reqID,
                    reqItemsQuantities[i],
                    reqItemsItemNums[i],
                    reqItemsDescs[i],
                    float.Parse(reqItemsPrices[i]),
                    float.Parse(reqItemsEstCosts[i]),
                    order

                };

                string[] prcParamNames =
                {
                    "a_id_number",
                    "a_item_quantity",
                    "a_item_number",
                    "a_item_description",
                    "a_item_price",
                    "a_item_estimated_cost",
                    "a_item_order"
                };

                dal = new DatabaseService("webformsdev");
                dal.ExecuteProcedure(prcName, prcParamNames, prcParams);
            }
        }

        public DataTable GetSFN18795InfoByID(int reqID)
        {
            string prcName = "WEBFORMS.PKG_SFN18795.PRC_SELECT_SFN18795_INFO";
            object[] prcParams = { reqID };
            string[] prcParamNames = { "a_id_number" };
            dal = new DatabaseService("webformsdev");
            DataTable dtFormInfo = dal.ExecuteProcedureOutCursor(prcName, prcParamNames, prcParams, 1);
            return dtFormInfo;
        }

        public DataTable GetSFN18795()
        {
            string sql = "SELECT * FROM WEBFORMS.SFN18795 JOIN WEBFORMS.SFN18795_REQUISITION_ITEMS ON SFN18795.ID_NUMBER=SFN18795_REQUISITION_ITEMS.ID_NUMBER " +
				"AND SFN18795_REQUISITION_ITEMS.ITEM_ORDER = '1'  ORDER BY SFN18795.CREATED_DATE DESC";
            string[] prcParams = { };
            string[] prcParamNames = { };
            //DataTable dtSFN18795s = dal.ExecuteProcedureOutCursor(sql, prcParamNames, prcParams, 1);
            DataTable dtSFN18795s = dal.ExecuteSelect(sql, prcParams, prcParamNames);
            return dtSFN18795s;
        }

        public void UpdateSFN18795(System.Web.Mvc.FormCollection collection, int id, bool formsub, string nextApproval)
        {
            string prcName = "WEBFORMS.PKG_SFN18795.PRC_UPDATE_SFN18795";
            object[] prcParams = {
                id,
                collection["RequisitionModel.Name"], //a_request_employee
                collection["RequisitionModel.Department"], //a_department_budget
                ConvertToOracleDate(collection["RequisitionModel.DateSubmitted"]), //a_request_date
                collection["RequisitionModel.Contractor"], //a_suggested_contractor
                ConvertBooleanToYesNo(Boolean.Parse(collection["RequisitionModel.SoftwareHardware"])), //a_software_hardware_request
                ConvertToOracleDate(collection["RequisitionModel.EstimatedStartDate"]), //a_estimated_start_date
                ConvertToOracleDate(collection["RequisitionModel.EstimatedCompleteDate"]), //a_estimated_complete_date
                Double.Parse(collection["RequisitionModel.TotalPrice"]), //a_req_items_total
                ConvertBooleanToYesNo(formsub),
                nextApproval
                };
            string[] prcParamNames =
                {
                "a_id_number",
                "a_request_employee",
                "a_department_budget",
                "a_request_date",
                "a_suggested_contractor",
                "a_software_hardware_request",
                "a_estimated_start_date",
                "a_estimated_complete_date",
                "a_req_items_total",
                "a_form_submitted",
                "a_waiting_approval"
                };
            dal = new DatabaseService("webformsdev");
            dal.ExecuteProcedure(prcName, prcParamNames, prcParams);

        }

        public void updateSFN18795Denied(int id)
        {
            string prcName = "PKG_SFN18795.PRC_UPDATE_SFN18795_DENIED";
            string[] pnames = { "a_id_number" };
            object[] pValues = { id };
            dal = new DatabaseService("webformsdev");
            dal.ExecuteProcedure(prcName, pnames, pValues);
        }

        public void updateSFN18795FormComplete(int id)
        {
            string prcName = "PKG_SFN18795.PRC_UPDATE_SFN18795_FORM_COMPLETED";
            string[] pnames = { "a_id_number" };
            object[] pValues = { id };
            dal = new DatabaseService("webformsdev");
            dal.ExecuteProcedure(prcName, pnames, pValues);
        }

        public void updateSFN18795ProcurementProcess(int id, System.Web.Mvc.FormCollection collection, bool ProcurementProcessing)
        {
            Debug.WriteLine(collection["RequisitionModel.ProcurementProcessing"]);
            string prcName = "PKG_SFN18795.PRC_UPDATE_SFN18795_PROCUREMENT_PROCESS";
            string[] pnames = { "a_id_number", "a_procurement_processing", "a_procurement_process_date", "a_requisition_id" };
            object[] pValues = { id,
                ConvertCheckboxBoolean(collection["RequisitionModel.ProcurementProcessing"]),
                ConvertToOracleDate(collection["RequisitionModel.ProcurementProcessedDate"]),
                collection["RequisitionModel.RequisitionID"]
            };
            dal = new DatabaseService("webformsdev");
            dal.ExecuteProcedure(prcName, pnames, pValues);
        }

        public void updateSFN18795Revise(int id, string reviseComment)
        {
            string prcName = "PKG_SFN18795.PRC_UPDATE_SFN18795_REVISE";
            string[] pnames = { "a_id_number", "a_revised_comment" };
            object[] pValues = { id , reviseComment };
            dal = new DatabaseService("webformsdev");
            dal.ExecuteProcedure(prcName, pnames, pValues);
        }

        public void updateSFN18795ReqItems(int reqID, System.Web.Mvc.FormCollection collection, SFN18795DisplayViewModel vmSFN18795, int reqCount)
        {
            string prcName = "PKG_SFN18795.PRC_UPDATE_SFN18795_REQUISITION_ITEMS";
            string[] reqItemsQuantities = collection.GetValues("RequisitionItem.Quantity");
            string[] reqItemsItemNums = collection.GetValues("RequisitionItem.ItemNumber");
            string[] reqItemsDescs = collection.GetValues("RequisitionItem.Description");
            string[] reqItemsPrices = collection.GetValues("RequisitionItem.Price");
            string[] reqItemsEstCosts = collection.GetValues("RequisitionItem.EstimatedCost");
            string currentOrder;
			string newOrder;

			if (reqCount > 0)
            {
                for (int i = 0; i < reqCount; i++)
                {
					currentOrder = vmSFN18795.RequisitionItems[i].Order;
					newOrder = (1+i).ToString();

					if (reqItemsQuantities[i].IsNullOrWhiteSpace())
                    {
                        reqItemsQuantities[i] = "0";
                    }
                    if (reqItemsPrices[i].IsNullOrWhiteSpace())
                    {
                        reqItemsPrices[i] = "0.00";
                    }
                    if (reqItemsEstCosts[i].IsNullOrWhiteSpace())
                    {
                        reqItemsEstCosts[i] = "0.00";
                    }
                    if (reqItemsDescs[i].IsNullOrWhiteSpace() || reqItemsDescs[i] == "")
                    {
                        for (int x = 0; x < i + 1; x++)
                        {
                            reqItemsDescs[i] = reqItemsDescs[i] + " ";
                        }

                    }
                    object[] prcParams =
                    {
                    reqID,
                    reqItemsQuantities[i],
                    reqItemsItemNums[i],
                    reqItemsDescs[i],
					float.Parse(reqItemsPrices[i]),
					float.Parse(reqItemsEstCosts[i]),
					currentOrder,
					newOrder
				};

                    string[] prcParamNames =
                    {
                    "a_id_number",
                    "a_item_quantity",
                    "a_item_number",
                    "a_item_description",
                    "a_item_price",
                    "a_item_estimated_cost",
                    "a_current_item_order",
					"a_new_item_order"
				};
                    dal = new DatabaseService("webformsdev");
                    dal.ExecuteProcedure(prcName, prcParamNames, prcParams);

                }
            }
        }

        public void UpdateSFN18795Approval(int id, System.Web.Mvc.FormCollection collection, string approver)
        {
            string prcName = "";
            object[] prcParams = new object[3];
            string[] prcParamNames = new string[3];

            switch (approver)
            {
                case "itrep":
                    prcName = "PKG_SFN18795.PRC_UPDATE_SFN18795_IT_APPROVAL";
                    prcParams = new object[] { id, collection["RequisitionModel.ITRepSignature"], ConvertToOracleDate(collection["RequisitionModel.ITRepSignatureDate"]) };
                    prcParamNames = new string[] { "a_id_number", "a_it_rep_signature", "a_it_rep_signature_date" };
                    break;
                case "supervisor":
                    prcName = "PKG_SFN18795.PRC_UPDATE_SFN18795_SUPERVISOR_APPROVAL";
                    prcParams = new object[] { id, collection["RequisitionModel.SupervisorSignature"], ConvertToOracleDate(collection["RequisitionModel.SupervisorSignatureDate"]) };
                    prcParamNames = new string[] { "a_id_number", "a_supervisor_signature", "a_supervisor_signature_date" };
                    break;
                case "department":
                    prcName = "PKG_SFN18795.PRC_UPDATE_SFN18795_DEPARTMENT_APPROVAL";
                    prcParams = new object[] { id, collection["RequisitionModel.DepartmentSignature"], ConvertToOracleDate(collection["RequisitionModel.DepartmentSignatureDate"]) };
                    prcParamNames = new string[] { "a_id_number", "a_department_signature", "a_department_signature_date" };
                    break;
                case "chief":
                    prcName = "PKG_SFN18795.PRC_UPDATE_SFN18795_CHIEF_APPROVAL";
                    prcParams = new object[] { id, collection["RequisitionModel.ChiefSignature"], ConvertToOracleDate(collection["RequisitionModel.ChiefSignatureDate"]) };
                    prcParamNames = new string[] { "a_id_number", "a_chief_signature", "a_chief_signature_date" };
                    break;
                case "director":
                    prcName = "PKG_SFN18795.PRC_UPDATE_SFN18795_DIRECTOR_APPROVAL";
                    prcParams = new object[] { id, collection["RequisitionModel.DirectorSignature"], ConvertToOracleDate(collection["RequisitionModel.DirectorSignatureDate"]) };
                    prcParamNames = new string[] { "a_id_number", "a_director_signature", "a_director_signature_date" };
                    break;
                case "procurement":
                    prcName = "PKG_SFN18795.PRC_UPDATE_SFN18795_PROCUREMENT_OFFICER_APPROVAL";
                    prcParams = new object[] { id, collection["RequisitionModel.ProcurementOfficerSignature"], ConvertToOracleDate(collection["RequisitionModel.ProcurementOfficerSignatureDate"]), collection["RequisitionModel.RequisitionID"] };
                    prcParamNames = new string[] { "a_id_number", "a_procurement_officer_signature", "a_procurement_officer_signature_date", "a_requisition_id" };
                    break;
            }
            dal = new DatabaseService("webformsdev");
            dal.ExecuteProcedure(prcName, prcParamNames, prcParams);
        }

        public void updateSFN18795WaitingApproval(int id, string nextapprover)
        {
            string prcName = "PKG_SFN18795.PRC_UPDATE_SFN18795_WAITING_APPROVAL";
            string[] pnames = { "a_waiting_approval", "a_id_number" };
            object[] pValues = { nextapprover, id };
            dal = new DatabaseService("webformsdev");
            dal.ExecuteProcedure(prcName, pnames, pValues);
        }
    }
}