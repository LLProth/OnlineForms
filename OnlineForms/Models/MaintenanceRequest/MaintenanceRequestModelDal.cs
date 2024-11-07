using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WSI.Utility.Database;

namespace OnlineForms.Models
{
    public class MaintenanceRequestModelDAL
    {
        IDatabaseService dal;

        public MaintenanceRequestModelDAL()
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

        public int InsertMaintenanceRequestGetID(System.Web.Mvc.FormCollection collection)
        {
            dal = new DatabaseService("webformsdev");

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
            dal = new DatabaseService("webformsdev");
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
    }
}