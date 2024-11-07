using Microsoft.Ajax.Utilities;
using OnlineForms.ViewModels;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using WSI.Utility.Database;

namespace OnlineForms.Models.SFN52712
{
    public class SFN52712ModelDal
    {
        IDatabaseService dal;
        public SFN52712ModelDal()
        {
            dal = new DatabaseService("webformsdev");
        }

        public void DeleteSFN52712Destinations(int id, int oldCount, int newCount)
        {
            string prcName = "PKG_SFN52712.PRC_DELETE_SFN52712_DESTINATION";            
            for(int i = oldCount; i > newCount; i--)
            {
                string[] prcParamNames = { "a_id_number", "a_destinations_order" };
                object[] prcParams = { id, i };

                dal = new DatabaseService("webformsdev");
                dal.ExecuteProcedure(prcName, prcParamNames, prcParams);
            }
        }

        public DataTable GetSFN52712()
        {
            string prcName = "PKG_SFN52712.PRC_GET_SFN52712";
            string[] prcParamNames = { };
            object[] prcParams = { };

            DataTable dtFormInfo = dal.ExecuteProcedureOutCursor(prcName, prcParamNames, prcParams, 1);

            return dtFormInfo;
        }

        public DataTable GetSFN52712ById(int id)
        {
            string prcName = "PKG_SFN52712.PRC_GET_SFN52712_BY_ID";
            string[] prcParamNames = { "a_id_number" };
            object[] prcParams = { id };

            dal = new DatabaseService("webformsdev");
            DataTable dtFormInfo = dal.ExecuteProcedureOutCursor(prcName, prcParamNames, prcParams, 1);

            return dtFormInfo;
        }

        public DataTable GetSFN52712FlightInfo(int id)
        {
            string prcName = "PKG_SFN52712.PRC_GET_SFN52712_FLIGHT_INFO";
            string[] prcParamNames = { "a_id_number" };
            object[] prcParams = { id };

            dal = new DatabaseService("webformsdev");
            DataTable dtFormInfo = dal.ExecuteProcedureOutCursor(prcName, prcParamNames, prcParams, 1);

            return dtFormInfo;
        }

        public DataTable GetSFN52712Approvals(int id)
        {
            string prcName = "PKG_SFN52712.PRC_GET_SFN52712_APPROVALS";
            string[] prcParamNames = { "a_id_number" };
            object[] prcParams = { id };

            dal = new DatabaseService("webformsdev");
            DataTable dtFormInfo = dal.ExecuteProcedureOutCursor(prcName, prcParamNames, prcParams, 1);

            return dtFormInfo;
        }

        public int InsertSFN52712GetID(System.Web.Mvc.FormCollection collection, bool formsub, string managerName, string emplId)
        {
            string prcName = "PKG_SFN52712.PRC_INSERT_SFN52712";

            foreach (var key in collection.AllKeys)
            {
                if (collection[key].IsNullOrWhiteSpace() && key.Contains("Date"))
                {
                    collection[key] = "1111-01-01";
                }
                if (key == "TravelAuthoriztionModel.EstimatedTotalCost" && collection[key].IsNullOrWhiteSpace())
                {
                    collection[key] = "0.00";
                }
            }

            string[] prcParamNames =
            {
                "a_deparment_budget",
                "a_name",
                "a_emplid",
                "a_title",
				"a_email",
				"a_method_of_travel",
                "a_preferred_departure_date",
                "a_event_start_date",
                "a_event_start_time",
                "a_event_end_date",
                "a_event_end_time",
                "a_preferred_return_date",
                "a_include_vacation_days",
                "a_purpose_of_trip",
                "a_number_of_persons",
                "a_estimated_total_cost",
                "a_transportation",
                "a_per_diem",
                "a_lodging",
                "a_registration",
                "a_rental_car_taxi",
                "a_comments",
                "a_form_submitted",
                "a_waiting_approval"
            };

            object[] prcParams =
            {
                collection["TravelAuthoriztionModel.DepartmentBudget"],
                collection["TravelAuthoriztionModel.Name"],
                emplId,
                collection["TravelAuthoriztionModel.Title"],
				collection["TravelAuthoriztionModel.Email"],
				collection["TravelAuthoriztionModel.MethodOfTravel"],
                ConvertToOracleDate(collection["TravelAuthoriztionModel.PreferredDepartureDate"]),
                ConvertToOracleDate(collection["TravelAuthoriztionModel.EventStartDate"]),
                collection["TravelAuthoriztionModel.EventStartTime"],
                ConvertToOracleDate(collection["TravelAuthoriztionModel.EventEndDate"]),
                collection["TravelAuthoriztionModel.EventEndTime"],
                ConvertToOracleDate(collection["TravelAuthoriztionModel.PreferredReturnDate"]),
                ConvertBooleanToYesNo(Boolean.Parse(collection["TravelAuthoriztionModel.IncludeVacationDays"])),
                collection["TravelAuthoriztionModel.PurposeOfTrip"],
                collection["TravelAuthoriztionModel.NumberOfPersons"],
                collection["TravelAuthoriztionModel.EstimatedTotalCost"],
                collection["TravelAuthoriztionModel.Transportation"],
                collection["TravelAuthoriztionModel.PerDiem"],
                collection["TravelAuthoriztionModel.Lodging"],
                collection["TravelAuthoriztionModel.Registration"],
                collection["TravelAuthoriztionModel.RentalCarTaxi"],
                collection["TravelAuthoriztionModel.Comments"],
                ConvertBooleanToYesNo(formsub),
                managerName
            };
            
            DataTable procedureReturn = dal.ExecuteProcedureOutCursor(prcName, prcParamNames, prcParams, 1);

            int requisitionID = int.Parse(procedureReturn.Rows[0][0].ToString());

            return requisitionID;
        }

        public void InsertSFN52712Destination(System.Web.Mvc.FormCollection collection, int id, int start)
        {
            string prcName = "PKG_SFN52712.PRC_INSERT_SFN52712_Destination";
            string[] cities = collection.GetValues("SFN52712Destination.City");
            string[] states = collection.GetValues("SFN52712Destination.State");
            string order = "1";
            for (int i = start; i < cities.Length; i++)
            {
                order = Convert.ToString(i + 1);
                object[] prcParams =
                {
                    id,
                    cities[i],
                    states[i],
                    order
                };

                string[] prcParamNames =
                {
                    "a_id_number",
                    "a_city",
                    "a_state", 
                    "a_destinations_order"
                };

                dal = new DatabaseService("webformsdev");
                dal.ExecuteProcedure(prcName, prcParamNames, prcParams);
            }
        }

        public void InsertSFN52712FlightInfo(System.Web.Mvc.FormCollection collection, int id)
        {
            string prcName = "PKG_SFN52712.PRC_INSERT_SFN52712_FLIGHT_INFO";

            object[] prcParams =
            {
                id,
                ConvertBooleanToYesNo(Boolean.Parse(collection["FlightInfo.EmpBookFlight"])),
                collection["FlightInfo.FreqFlierNumber"],
                collection["FlightInfo.GovernemtIdName"],
                ConvertToOracleDate(collection["FlightInfo.DateofBirth"]),
                collection["FlightInfo.TravelContactNumber"],
                collection["FlightInfo.SeatPreference"]
            };

            string[] prcParamNames =
            {
                "a_id_number",
                "a_employe_book_flight",
                "a_frequent_flier_number",
                "a_government_id_name",
                "a_date_of_birth",
                "a_travel_contact_number",
                "a_seat_preference"
            };

            dal = new DatabaseService("webformsdev");
            dal.ExecuteProcedure(prcName, prcParamNames, prcParams);

        }

        public void InsertSFN52712TravelerSignature(System.Web.Mvc.FormCollection collection, int id)
        {
            string prcName = "PKG_SFN52712.PRC_INSERT_SFN52712_TRAVELER_SIGNATURE";

            object[] prcParams =
            {
                id,
                collection["TravelAuthoriztionModel.Name"],
                ConvertToOracleDate( DateTime.Today.ToString("yyyy-MM-dd"))
            };

            string[] prcParamNames =
            {
                "a_id_number",
                "a_person_travel_signature",
                "a_person_travel_signature_date"
            };

            dal = new DatabaseService("webformsdev");
            dal.ExecuteProcedure(prcName, prcParamNames, prcParams);
        }

        public void UpdateSFN52712SupervisorSignature(System.Web.Mvc.FormCollection collection, int id, string signature, string date, string nextApproverName)
        {
            string prcName = "PKG_SFN52712.PRC_UPDATE_SFN52712_SUPERVISOR_SIGNATURE";
            object[] prcParams = { id, signature, ConvertToOracleDate(date), nextApproverName };
            string[] prcParamNames =  
            {
                "a_id_number",
                "a_supervisor_signature",
                "a_supervisor_signature_date",
                "a_waiting_approval"
            };

            dal = new DatabaseService("webformsdev");
            dal.ExecuteProcedure(prcName, prcParamNames, prcParams);
        }

        public void UpdateSFN52712DepartmentSignature(System.Web.Mvc.FormCollection collection, int id, string signature, string date, string nextApproverName)
        {
            string prcName = "PKG_SFN52712.PRC_UPDATE_SFN52712_DEPARTMENT_SIGNATURE";
            object[] prcParams = { id, signature, ConvertToOracleDate(date), nextApproverName };
            string[] prcParamNames =
            {
                "a_id_number",
                "a_department_budget_manager_signature",
                "a_department_budget_manager_signature_date",
                "a_waiting_approval"
            };

            dal = new DatabaseService("webformsdev");
            dal.ExecuteProcedure(prcName, prcParamNames, prcParams);
        }

        public void UpdateSFN52712ChiefSignature(System.Web.Mvc.FormCollection collection, int id, string signature, string date, string nextApproverName)
        {
            string prcName = "PKG_SFN52712.PRC_UPDATE_SFN52712_CHIEF_SIGNATURE";
            object[] prcParams = { id, signature, ConvertToOracleDate(date), nextApproverName };
            string[] prcParamNames =
            {
                "a_id_number",
                "a_division_chief_signature",
                "a_division_chief_signature_date",
                "a_waiting_approval"
            };

            dal = new DatabaseService("webformsdev");
            dal.ExecuteProcedure(prcName, prcParamNames, prcParams);
        }

        public void UpdateSFN52712DirectorofFinanceSignature(System.Web.Mvc.FormCollection collection, int id)
        {
            string prcName = "PKG_SFN52712.PRC_UPDATE_SFN52712_DIRECTOR_OF_FINANCE_SIGNATURE";
            object[] prcParams = { id, collection["SFN52712Approvals.DirectorofFinanceSignature"], ConvertToOracleDate(collection["SFN52712Approvals.DirectorofFinanceSignatureDate"]) };
            string[] prcParamNames =
            {
                "a_id_number",
                "a_director_of_finance_signature",
                "a_director_of_finance_signature_date"
            };

            dal = new DatabaseService("webformsdev");
            dal.ExecuteProcedure(prcName, prcParamNames, prcParams);
        }

        public void UpdateSFN52712DirectorSignature(System.Web.Mvc.FormCollection collection, int id, string signature, string date)
        {
            string prcName = "PKG_SFN52712.PRC_UPDATE_SFN52712_DIRECTOR_SIGNATURE";
            object[] prcParams = { id, signature, ConvertToOracleDate(date) };
            string[] prcParamNames =
            {
                "a_id_number",
                "a_director_signature",
                "a_director_signature_date"
            };

            dal = new DatabaseService("webformsdev");
            dal.ExecuteProcedure(prcName, prcParamNames, prcParams);
        }

        public void updateSFN52712ByID(System.Web.Mvc.FormCollection collection, bool formsub, int id, string managerName)
        {
            string prcName = "PKG_SFN52712.PRC_UPDATE_SFN52712";

            foreach (var key in collection.AllKeys)
            {
                if (collection[key].IsNullOrWhiteSpace() && key.Contains("Date"))
                {
                    collection[key] = "1111-01-01";
                }
                if (key == "TravelAuthoriztionModel.EstimatedTotalCost" && collection[key].IsNullOrWhiteSpace())
                {
                    collection[key] = "0.00";
                }
            }

            string[] prcParamNames =
            {
                "a_id_number",
                "a_deparment_budget",
                "a_name",
                "a_title",
				"a_email",
				"a_method_of_travel",
                "a_preferred_departure_date",
                "a_event_start_date",
                "a_event_start_time",
                "a_event_end_date",
                "a_event_end_time",
                "a_preferred_return_date",
                "a_include_vacation_days",
                "a_purpose_of_trip",
                "a_number_of_persons",
                "a_estimated_total_cost",
                "a_transportation",
                "a_per_diem",
                "a_lodging",
                "a_registration",
                "a_rental_car_taxi",
                "a_comments",
                "a_waiting_approval",
                "a_form_submitted"
            };

            object[] prcParams =
            {
                id,
                collection["TravelAuthoriztionModel.DepartmentBudget"],
                collection["TravelAuthoriztionModel.Name"],
                collection["TravelAuthoriztionModel.Title"],
				collection["TravelAuthoriztionModel.Email"],
				collection["TravelAuthoriztionModel.MethodOfTravel"],
                ConvertToOracleDate(collection["TravelAuthoriztionModel.PreferredDepartureDate"]),
                ConvertToOracleDate(collection["TravelAuthoriztionModel.EventStartDate"]),
                collection["TravelAuthoriztionModel.EventStartTime"],
                ConvertToOracleDate(collection["TravelAuthoriztionModel.EventEndDate"]),
                collection["TravelAuthoriztionModel.EventEndTime"],
                ConvertToOracleDate(collection["TravelAuthoriztionModel.PreferredReturnDate"]),
                ConvertBooleanToYesNo(Boolean.Parse(collection["TravelAuthoriztionModel.IncludeVacationDays"])),
                collection["TravelAuthoriztionModel.PurposeOfTrip"],
                collection["TravelAuthoriztionModel.NumberOfPersons"],
                collection["TravelAuthoriztionModel.EstimatedTotalCost"],
                collection["TravelAuthoriztionModel.Transportation"],
                collection["TravelAuthoriztionModel.PerDiem"],
                collection["TravelAuthoriztionModel.Lodging"],
                collection["TravelAuthoriztionModel.Registration"],
                collection["TravelAuthoriztionModel.RentalCarTaxi"],
                collection["TravelAuthoriztionModel.Comments"],
                managerName,
                ConvertBooleanToYesNo(formsub)
                
            };
            dal = new DatabaseService("webformsdev");
            dal.ExecuteProcedure(prcName, prcParamNames, prcParams);
        }

        public void UpdateSFN52712DestinationById(System.Web.Mvc.FormCollection collection, int id, SFN52712ViewModel lsSFN52712, int destCount)
        {
            string prcName = "PKG_SFN52712.PRC_UPDATE_SFN52712_Destination";
            string[] cities = collection.GetValues("SFN52712Destination.City");
            string[] states = collection.GetValues("SFN52712Destination.State");
            string order;

            if (destCount > 0)
            {
                for (int i = 0; i < destCount; i++)
                {
                    order = lsSFN52712.SFN52712Destinations[i].DestinationOrder;
                    object[] prcParams =
                    {
                    id,
                    cities[i],
                    states[i],
                    order
                };

                    string[] prcParamNames =
                    {
                    "a_id_number",
                    "a_city",
                    "a_state",
                    "a_destinations_order"
                };

                    dal = new DatabaseService("webformsdev");
                    dal.ExecuteProcedure(prcName, prcParamNames, prcParams);
                }
            }
        }

        public void UpdateSFN52712FlightInfoById(System.Web.Mvc.FormCollection collection, int id)
        {
            string prcName = "PKG_SFN52712.PRC_UPDATE_SFN52712_FLIGHT_INFO_BY_ID";

            object[] prcParams =
            {
                id,
                ConvertBooleanToYesNo(Boolean.Parse(collection["FlightInfo.EmpBookFlight"])),
                collection["FlightInfo.FreqFlierNumber"],
                collection["FlightInfo.GovernemtIdName"],
                ConvertToOracleDate(collection["FlightInfo.DateofBirth"]),
                collection["FlightInfo.TravelContactNumber"],
                collection["FlightInfo.SeatPreference"]
            };

            string[] prcParamNames =
            {
                "a_id_number",
                "a_employe_book_flight",
                "a_frequent_flier_number",
                "a_government_id_name",
                "a_date_of_birth",
                "a_travel_contact_number",
                "a_seat_preference"
            };

            dal = new DatabaseService("webformsdev");
            dal.ExecuteProcedure(prcName, prcParamNames, prcParams);
        }

        public void UpdateSFN52712ProcurementProcessById(System.Web.Mvc.FormCollection collection, int id, bool ProcurementProcessing)
        {
            string prcName = "PKG_SFN52712.PRC_UPDATE_SFN52712_PROCUREMENT_PROCESS_BY_ID";

            object[] prcParams =
            {
                id,
                ConvertCheckboxBoolean(collection["TravelAuthoriztionModel.ProcurementProcessing"]),
                ConvertToOracleDate(collection["TravelAuthoriztionModel.ProcurementProcessedDate"]),
                collection["TravelAuthoriztionModel.Comments"]
            };

            string[] prcParamNames =
            {
                "a_id_number",
                "a_procurement_processing",
                "a_procurement_process_date",
                "a_comments"
            };

            dal = new DatabaseService("webformsdev");
            dal.ExecuteProcedure(prcName, prcParamNames, prcParams);
        }

        public void UpdateSFN52712ApprovedbyId(int id)
        {
            string prcName = "PKG_SFN52712.PRC_UPDATE_SFN52712_APPROVED_BY_ID";

            object[] prcParams =
            {
                id
            };

            string[] prcParamNames =
            {
                "a_id_number"
            };

            dal = new DatabaseService("webformsdev");
            dal.ExecuteProcedure(prcName, prcParamNames, prcParams);
        }

        public void UpdateSFN52712CompletebyId(int id)
        {
            string prcName = "PKG_SFN52712.PRC_UPDATE_SFN52712_COMPLETE_BY_ID";

            object[] prcParams =
            {
                id
            };

            string[] prcParamNames =
            {
                "a_id_number"
            };

            dal = new DatabaseService("webformsdev");
            dal.ExecuteProcedure(prcName, prcParamNames, prcParams);
        }

        public void UpdateSFN52712DeniedbyId(int id)
        {
            string prcName = "PKG_SFN52712.PRC_UPDATE_SFN52712_DENIED_BY_ID";

            object[] prcParams =
            {
                id
            };

            string[] prcParamNames =
            {
                "a_id_number"
            };

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