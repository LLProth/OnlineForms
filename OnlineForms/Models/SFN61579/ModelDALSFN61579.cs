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
    public class ModelDALSFN61579 : ModelDAL
    {
        public ModelDALSFN61579():base()
        {
            dal = new DatabaseService("webformsdev");
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
                {"Date", dtFormInfo.Rows[0]["REQUEST_DATE"].ToString()},
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
            object[] prcParams = {  };
            string[] prcParamNames = {  };
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
                ConvertToOracleDate(collection["CharitableModel.DateSubmitted"]),
                collection["CharitableModel.Reviewed"]
            };

            string[] prcParamNames = {
                "a_request_employee",
                "a_request_date",
                "a_reviewed"
            };

            dal = new DatabaseService("webformsdev");
            DataTable procedureReturn = dal.ExecuteProcedureOutCursor(prcName, prcParamNames, prcParams, 1);
            int charitableID = int.Parse(procedureReturn.Rows[0][0].ToString());

            return charitableID;
        }
        public void InsertSFN61579CharityInfo(System.Web.Mvc.FormCollection collection, int reqID)
        {
            string prcName = "PKG_SFN61579.PRC_INSRT_SFN61579_CHARITY_INFO";

            object[] prcParams =
                {
                    reqID,
                    collection["CharityInfoModel.AddressLineOne"],
                    collection["CharityInfoModel.AddressLineTwo"],
                    collection["CharityInfoModel.City"],
                    collection["CharityInfoModel.State"],
                    collection["CharityInfoModel.ZipCode"],
                    collection["CharityInfoModel.Website"],
                    collection["CharityInfoModel.Organization"],
                    collection["CharityInfoModel.Month"],
                    collection["CharityInfoModel.CharityDescription"]
                };

            string[] prcParamNames =
            {
                    "a_id_number",
                    "a_address_line_one",
                    "a_address_line_two",
                    "a_city",
                    "a_state",
                    "a_zip_code",
                    "a_website",
                    "a_organization_name",
                    "a_event_month",
                    "a_charity_description"

                };

            dal = new DatabaseService("webformsdev");
            dal.ExecuteProcedure(prcName, prcParamNames, prcParams);
        }
    }
}