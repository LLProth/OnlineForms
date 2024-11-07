using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity;
using OnlineForms.Models;
using OnlineForms.Models.SFN52712;
using OnlineForms.Controllers;
using OnlineForms.CustomValidation;

namespace OnlineForms.Models.SFN52712
{
    public class SFN52712Model : IValidatableObject
    {
        [Key]
        [Display(Name = "ID Number")]
        public int ID { get; set; }

        [Display(Name = "Department Budget")]
        [Required]
        public string DepartmentBudget { get; set; }

        [Display(Name = "Person Traveling")]
        public string Name { get; set; }

        [Display(Name = "Employee ID")]
        public string Emplid { get; set; }

        [Display(Name = "Title")]
        public string Title { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Method of Travel")]
        [Required]
		[StringLength(500, ErrorMessage = "Method of Travel can only be 500 characters.")]
		public string MethodOfTravel { get; set; }

        [Display(Name = "Preferred Departure Date")]
        [Required]
        [BeforeTodayAttributes(ErrorMessage = "Prefferred Departure date must be after today.")]
        public DateTime PreferredDepartureDate { get; set; }

        [Display(Name = "Event Start Date")]
        [DateOrderAttributes("PreferredDepartureDate", true, ErrorMessage = "Event Start date can't be before Prefferred Departure date.")]
        [BeforeTodayAttributes(ErrorMessage = "Event Start date must be after today.")]
		[DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
		[Required]
        public DateTime EventStartDate { get; set; }

        [Display(Name = "Start Time")]
        [DataType(DataType.Time)]
        public string EventStartTime { get; set; }
        
        [Display(Name = "Event End Date")]
        [DateOrderAttributes("EventStartDate", true, ErrorMessage = "Event End date can't be before Event Start date.")]
        [BeforeTodayAttributes(ErrorMessage = "Event End date must be after today.")]
		[DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
		[Required]
        public DateTime EventEndDate { get; set; }

        [Display(Name = "End Time")]
        [DataType(DataType.Time)]
        public string EventEndTime { get; set; }

        [Display(Name = "Preferred Return Date")]
        [DateOrderAttributes("EventEndDate", true, ErrorMessage = "Prefferred return date can't be before Event End date.")]
        [BeforeTodayAttributes(ErrorMessage = "Prefferred return date must be after today.")]
        public DateTime PreferredReturnDate { get; set; }

        [Display(Name = "Does the trip include vacation days?")]
        public bool? IncludeVacationDays { get; set; }

        [Display(Name = "Name of meeting/Purpose of trip (include conference website, do not abbreviate):")]
		[StringLength(1000, ErrorMessage = "Name of meeting/Purpose of trip can only be 1000 characters.")]
		public string PurposeOfTrip { get; set; }

        [Display(Name = "Number of Persons")]
		[StringLength(1000, ErrorMessage = "Number of Persons can only be 1000 characters.")]
		public string NumberOfPersons { get; set; }

        [Display(Name = "Estimated Cost of Trip")]
		[Required]
		public string EstimatedTotalCost { get; set; }

        [Display(Name = "Transportation")]
		[Required(ErrorMessage = "Transportation price is required, if none enter 0.00")]
		public string Transportation { get; set; }

        [Display(Name = "Per Diem")]
		[Required(ErrorMessage = "Per Diem price is required, if none enter 0.00")]
		public string PerDiem { get; set; }

        [Display(Name = "Lodging")]
		[Required(ErrorMessage = "Lodging price is required, if none enter 0.00")]
		public string Lodging { get; set; }

        [Display(Name = "Registration")]
		[Required(ErrorMessage = "Registration price is required, if none enter 0.00")]
		public string Registration { get; set; }

        [Display(Name = "Rental Car/Taxi")]
		[Required(ErrorMessage = "Rental Car/Taxi price is required, if none enter 0.00")]
		public string RentalCarTaxi { get; set; }

        [Display(Name = "Comments")]
		[StringLength(2000, ErrorMessage = "Number of Persons can only be 2000 characters.")]
		public string Comments { get; set; }

        [Display(Name = "Form is Submitted")]
        public bool FormSubmitted { get; set; }

        [Display(Name = "Form is Completed")]
        public bool FormCompleted { get; set; }

        [Display(Name = "Form is Denied")]
        public bool FormDenied { get; set; }

        [Display(Name = "Form is Approved")]
        public bool FormApproved { get; set; }

        [Display(Name = "Procurement is Processing")]
        public bool ProcurementProcessing { get; set; }

        [Display(Name = "Procurement Date")]
        [DataType(DataType.Date)]
        public DateTime ProcurementProcessedDate { get; set; }

        [Display(Name = "Waiting Approval")]
        public string WaitingApproval { get; set; }

        public DateTime ModifiedDate { get; set; }

		[DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
		public DateTime CreationDate { get; set; }


        public SFN52712Model(int id, string departmentBudget, string name, string emplid, string title, string email, string methodOfTravel, DateTime preferredDepartureDate,
                DateTime eventStartDate, string eventStartTime, DateTime eventEndDate, string eventEndTime, DateTime preferredReturnDate, bool includeVacationDays, string purposeOfTrip,
                string numberOfPersons, string estimatedTotalCost, string transportation, string perDiem, string lodging, string registration, string rentalCarTaxi, string comments,
                bool formSubmitted, bool formCompleted, bool formDenied, bool formApproved, bool procurementProcessing, DateTime procurementDate, string waitingApproval, DateTime modifiedDate,
                DateTime creationDate)
        {
            ID = id;
            DepartmentBudget = departmentBudget;
            Name = name;
            Emplid = emplid;
            Title = title;
            Email = email;
            MethodOfTravel = methodOfTravel;
            PreferredDepartureDate = preferredDepartureDate;
            EventStartDate = eventStartDate;
            EventStartTime = eventStartTime;
            EventEndDate = eventEndDate;
            EventEndTime = eventEndTime;
            PreferredReturnDate = preferredReturnDate;
            IncludeVacationDays = includeVacationDays;
            PurposeOfTrip = purposeOfTrip;
            NumberOfPersons = numberOfPersons;
            EstimatedTotalCost = estimatedTotalCost;
            Transportation = transportation;
            PerDiem = perDiem;
            Lodging = lodging;
            Registration = registration;
            RentalCarTaxi = rentalCarTaxi;
            Comments = comments;
            FormSubmitted = formSubmitted;
            FormCompleted = formCompleted;
            FormDenied = formDenied;
            FormApproved = formApproved;
            ProcurementProcessing = procurementProcessing;
            ProcurementProcessedDate = procurementDate;
            WaitingApproval = waitingApproval;
            ModifiedDate = modifiedDate;
            CreationDate = creationDate;
        }

        public static SFN52712Model GetSFN52712(DataTable dtReq)
        {
            DataRow drReq = dtReq.Rows[0];
            SFN52712Model req = new SFN52712Model(
                int.Parse(drReq["ID_NUMBER"].ToString()),
                drReq["DEPARTMENT_BUDGET"].ToString(),
                drReq["NAME"].ToString(),
                drReq["EMPLID"].ToString(),
                drReq["TITLE"].ToString(),
                drReq["EMAIL"].ToString(),
                drReq["METHOD_OF_TRAVEL"].ToString(),
                (drReq["PREFERRED_DEPARTURE_DATE"].ToString() == "") ? DateTime.Parse("1/1/0001") : DateTime.Parse(drReq["PREFERRED_DEPARTURE_DATE"].ToString()),
                (drReq["EVENT_START_DATE"].ToString() == "") ? DateTime.Parse("1/1/0001") : DateTime.Parse(drReq["EVENT_START_DATE"].ToString()),
                drReq["EVENT_START_TIME"].ToString(),
                (drReq["EVENT_END_DATE"].ToString() == "") ? DateTime.Parse("1/1/0001") : DateTime.Parse(drReq["EVENT_END_DATE"].ToString()),
                drReq["EVENT_END_TIME"].ToString(),
                (drReq["PREFERRED_RETURN_DATE"].ToString() == "") ? DateTime.Parse("1/1/0001") : DateTime.Parse(drReq["PREFERRED_RETURN_DATE"].ToString()),
                (drReq["INCLUDE_VACATION_DAYS"].ToString() == "Y") ? true : false,
                drReq["PURPOSE_OF_TRIP"].ToString(),
                drReq["NUMBER_OF_PERSONS"].ToString(),
                drReq["ESTIMATED_TOTAL_COST"].ToString(),
                drReq["TRANSPORTATION"].ToString(),
                drReq["PER_DIEM"].ToString(),
                drReq["LODGING"].ToString(),
                drReq["REGISTRATION"].ToString(),
                drReq["RENTAL_CAR_TAXI"].ToString(),
                drReq["COMMENTS"].ToString(),
                (drReq["FORM_SUBMITTED"].ToString() == "Y") ? true : false,
                (drReq["FORM_COMPLETED"].ToString() == "Y") ? true : false,
                (drReq["FORM_DENIED"].ToString() == "Y") ? true : false,
                (drReq["FORM_APPROVED"].ToString() == "Y") ? true : false,
                (drReq["PROCUREMENT_PROCESSING"].ToString() == "Y") ? true : false,
                (drReq["PROCUREMENT_PROCESS_DATE"].ToString() == "") ? DateTime.Parse("1/1/0001") : DateTime.Parse(drReq["PROCUREMENT_PROCESS_DATE"].ToString()),
                drReq["WAITING_APPROVAL"].ToString(),
                (drReq["MODIFIED_DATE"].ToString() == "") ? DateTime.Parse("1/1/0001") : DateTime.Parse(drReq["MODIFIED_DATE"].ToString()),
                (drReq["CREATION_DATE"].ToString() == "") ? DateTime.Parse("1/1/0001") : DateTime.Parse(drReq["CREATION_DATE"].ToString())
                );
            return req;
        }

        public static List < SFN52712Model > GetSFN52712List(DataTable dlist)
        {
            List<SFN52712Model> SFN52712List = new List<SFN52712Model>();
            foreach (DataRow row in dlist.Rows)
            {
                SFN52712Model item = new SFN52712Model(
                    int.Parse(row["ID_NUMBER"].ToString()),
                    row["DEPARTMENT_BUDGET"].ToString(),
                    row["NAME"].ToString(),
                    row["EMPLID"].ToString(),
                    row["TITLE"].ToString(),
                    row["EMAIL"].ToString(),
                    row["METHOD_OF_TRAVEL"].ToString(),
                    DateTime.Parse(row["PREFERRED_DEPARTURE_DATE"].ToString()),
                    DateTime.Parse(row["EVENT_START_DATE"].ToString()),
                    row["EVENT_START_TIME"].ToString(),
                    DateTime.Parse(row["EVENT_END_DATE"].ToString()),
                    row["EVENT_END_TIME"].ToString(),
                    DateTime.Parse(row["PREFERRED_RETURN_DATE"].ToString()),
                    (row["INCLUDE_VACATION_DAYS"].ToString() == "Y") ? true : false,
                    row["PURPOSE_OF_TRIP"].ToString(),
                    row["NUMBER_OF_PERSONS"].ToString(),
                    row["ESTIMATED_TOTAL_COST"].ToString(),
                    row["TRANSPORTATION"].ToString(),
                    row["PER_DIEM"].ToString(),
                    row["LODGING"].ToString(),
                    row["REGISTRATION"].ToString(),
                    row["RENTAL_CAR_TAXI"].ToString(),
                    row["COMMENTS"].ToString(),
                    (row["FORM_SUBMITTED"].ToString() == "Y") ? true : false,
                    (row["FORM_COMPLETED"].ToString() == "Y") ? true : false,
                    (row["FORM_DENIED"].ToString() == "Y") ? true : false,
                    (row["FORM_APPROVED"].ToString() == "Y") ? true : false,
                    (row["PROCUREMENT_PROCESSING"].ToString() == "Y") ? true : false,
                    (row["PROCUREMENT_PROCESS_DATE"].ToString() == "") ? DateTime.Parse("1/1/0001") : DateTime.Parse(row["PROCUREMENT_PROCESS_DATE"].ToString()),
                    row["WAITING_APPROVAL"].ToString(),
                    (row["MODIFIED_DATE"].ToString() == "") ? DateTime.Parse("1/1/0001") : DateTime.Parse(row["MODIFIED_DATE"].ToString()),
                    (row["CREATION_DATE"].ToString() == "") ? DateTime.Parse("1/1/0001") : DateTime.Parse(row["CREATION_DATE"].ToString())
                );

                SFN52712List.Add(item);
            }
            return SFN52712List;
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {

            if(PreferredDepartureDate < DateTime.Now)
            {
                yield return new ValidationResult(
                    errorMessage: "Preferred Departure date must be before Event Start Date",
                    memberNames: new[] { "PreferredDepartureDate", "EventStartDate" }
                );
            }
        }
    }

    public class SFN52712DestinationModel
    {
        [Display(Name = "ID Number (to be assigned)")]
        public int ID { get; set; }
        [Required]
        [Display(Name = "City")]
        public string City { get; set; }
        [Required]
        [Display(Name = "State")]
        public string State { get; set; }
        
        [Display(Name = "Destination Order")]
        public string DestinationOrder { get; set; }

        public SFN52712DestinationModel(int id, string city, string state, string destinationOrder)
        {
            ID = id;
            City = city;
            State = state;
            DestinationOrder = destinationOrder;
        }

        public static List<SFN52712DestinationModel> ConvertDataTableDestinationList(DataTable dlist)
        {
            List<SFN52712DestinationModel> SFN52712DestinationList = new List<SFN52712DestinationModel>();
            foreach (DataRow drReq in dlist.Rows)
            {
                SFN52712DestinationModel location = new SFN52712DestinationModel(
                    int.Parse(drReq["ID_NUMBER"].ToString()),
                    drReq["CITY"].ToString(),
                    drReq["STATE"].ToString(),
                    drReq["DESTINATIONS_ORDER"].ToString()
                );
                SFN52712DestinationList.Add(location);
            }
            return SFN52712DestinationList;
        }
    }

    public class SFN52712Approval
    {
        [Key]
        [Display(Name = "ID Number")]
        public int ID { get; set; }

        [Display(Name = "Signature of Person Traveling")]
        public string PersonTravelingSignature { get; set; }

        [Display(Name = "Signature of Person Traveling Date")]
        public DateTime PersonTravelingSignatureDate { get; set; }

        [Display(Name = "Supervisor Signature")]
        public string SupervisorSignature { get; set; }

        [Display(Name = "Supervisor Signature Date")]
        public DateTime SupervisorSignatureDate { get; set; }

        [Display(Name = "Department Budget Manager Signature")]
        public string DepartmentBudgetManagerSignature { get; set; }

        [Display(Name = "Department Budget Manager Signature Date")]
        public DateTime DepartmentBudgetManagerSignatureDate { get; set; }

        [Display(Name = "Division Chief Signature")]
        public string DivisionChiefSignature { get; set; }

        [Display(Name = "Division Chief Signature Date")]
        public DateTime DivisionChiefSignatureDate { get; set; }

        [Display(Name = "Director of Finance Signature")]
        public string DirectorofFinanceSignature { get; set; }

        [Display(Name = "Director of Finance Signature Date")]
        public DateTime DirectorofFinanceSignatureDate { get; set; }

        [Display(Name = "Director Signature")]
        public string DirectorSignature { get; set; }

        [Display(Name = "Director Signature Date")]
        public DateTime DirectorSignatureDate { get; set; }

        public SFN52712Approval(int id, string personTravelingSignature, DateTime personTravelingSignatureDate, string supervisorSignature, DateTime supervisorSignatureDate, string departmentBudgetManagerSignature,
            DateTime departmentBudgetManagerSignatureDate, string divisionChiefSignature, DateTime divisionChiefSignatureDate, string directorofFinanceSignature, DateTime directorofFinanceSignatureDate, string directorSignature, DateTime directorSignatureDate)
        {
            ID = id;
            PersonTravelingSignature = personTravelingSignature;
            PersonTravelingSignatureDate = personTravelingSignatureDate;
            SupervisorSignature = supervisorSignature;
            SupervisorSignatureDate = supervisorSignatureDate;
            DepartmentBudgetManagerSignature = departmentBudgetManagerSignature;
            DepartmentBudgetManagerSignatureDate = departmentBudgetManagerSignatureDate;
            DivisionChiefSignature = divisionChiefSignature;
            DivisionChiefSignatureDate = divisionChiefSignatureDate;
            DirectorofFinanceSignature = directorofFinanceSignature;
            DirectorofFinanceSignatureDate = directorofFinanceSignatureDate;
            DirectorSignature = directorSignature;
            DirectorSignatureDate = directorSignatureDate;
        }

        public static SFN52712Approval ConvertDataTableApproval(DataTable dtReq)
        {
            DataRow drReq = dtReq.Rows[0];
            SFN52712Approval req = new SFN52712Approval(
                int.Parse(drReq["ID_NUMBER"].ToString()),
                drReq["PERSON_TRAVEL_SIGNATURE"].ToString(),
                (drReq["PERSON_TRAVEL_SIGNATURE_DATE"].ToString() == "") ? DateTime.Parse("1/1/0001") : DateTime.Parse(drReq["PERSON_TRAVEL_SIGNATURE_DATE"].ToString()),
                drReq["SUPERVISOR_SIGNATURE"].ToString(),
                (drReq["SUPERVISOR_SIGNATURE_DATE"].ToString() == "") ? DateTime.Parse("1/1/0001") : DateTime.Parse(drReq["SUPERVISOR_SIGNATURE_DATE"].ToString()),
                drReq["DEPARTMENT_BUDGET_MANAGER_SIGNATURE"].ToString(),
                (drReq["DEPARTMENT_BUDGET_MANAGER_SIGNATURE_DATE"].ToString() == "") ? DateTime.Parse("1/1/0001") : DateTime.Parse(drReq["DEPARTMENT_BUDGET_MANAGER_SIGNATURE_DATE"].ToString()),
                drReq["DIVISION_CHIEF_SIGNATURE"].ToString(),
                (drReq["DIVISION_CHIEF_SIGNATURE_DATE"].ToString() == "") ? DateTime.Parse("1/1/0001") : DateTime.Parse(drReq["DIVISION_CHIEF_SIGNATURE_DATE"].ToString()),
                drReq["DIRECTOR_OF_FINANCE_SIGNATURE"].ToString(),
                (drReq["DIRECTOR_OF_FINANCE_SIGNATURE_DATE"].ToString() == "") ? DateTime.Parse("1/1/0001") : DateTime.Parse(drReq["DIRECTOR_SIGNATURE_DATE"].ToString()),
                drReq["DIRECTOR_SIGNATURE"].ToString(),
                (drReq["DIRECTOR_SIGNATURE_DATE"].ToString() == "") ? DateTime.Parse("1/1/0001") : DateTime.Parse(drReq["DIRECTOR_SIGNATURE_DATE"].ToString())
                );
            return req;
        }

		public static List<SFN52712Approval> ConvertDataTableApprovalList(DataTable dlist)
		{
			List<SFN52712Approval> SFN52712ApprovalList = new List<SFN52712Approval>();
            foreach (DataRow row in dlist.Rows)
            {
				SFN52712Approval item = new SFN52712Approval(
                int.Parse(row["ID_NUMBER"].ToString()),
				row["PERSON_TRAVEL_SIGNATURE"].ToString(),
                (row["PERSON_TRAVEL_SIGNATURE_DATE"].ToString() == "") ? DateTime.Parse("1/1/0001") : DateTime.Parse(row["PERSON_TRAVEL_SIGNATURE_DATE"].ToString()),
				row["SUPERVISOR_SIGNATURE"].ToString(),
                (row["SUPERVISOR_SIGNATURE_DATE"].ToString() == "") ? DateTime.Parse("1/1/0001") : DateTime.Parse(row["SUPERVISOR_SIGNATURE_DATE"].ToString()),
                row["DEPARTMENT_BUDGET_MANAGER_SIGNATURE"].ToString(),
                (row["DEPARTMENT_BUDGET_MANAGER_SIGNATURE_DATE"].ToString() == "") ? DateTime.Parse("1/1/0001") : DateTime.Parse(row["DEPARTMENT_BUDGET_MANAGER_SIGNATURE_DATE"].ToString()),
                row["DIVISION_CHIEF_SIGNATURE"].ToString(),
                (row["DIVISION_CHIEF_SIGNATURE_DATE"].ToString() == "") ? DateTime.Parse("1/1/0001") : DateTime.Parse(row["DIVISION_CHIEF_SIGNATURE_DATE"].ToString()),
                row["DIRECTOR_OF_FINANCE_SIGNATURE"].ToString(),
                (row["DIRECTOR_OF_FINANCE_SIGNATURE_DATE"].ToString() == "") ? DateTime.Parse("1/1/0001") : DateTime.Parse(row["DIRECTOR_SIGNATURE_DATE"].ToString()),
                row["DIRECTOR_SIGNATURE"].ToString(),
                (row["DIRECTOR_SIGNATURE_DATE"].ToString() == "") ? DateTime.Parse("1/1/0001") : DateTime.Parse(row["DIRECTOR_SIGNATURE_DATE"].ToString())
                );
				SFN52712ApprovalList.Add(item);
			}
			return SFN52712ApprovalList;
		}
	}

    public class SFN52712FlightMethod
    {
        [Key]
        [Display(Name = "ID Number")]
        public int ID { get; set; }
        
        [Display(Name = "Employee to Book Flights?")]
        public bool? EmpBookFlight { get; set; }

        [Display(Name = "Frequent Flier Number:")]
        public string FreqFlierNumber { get; set; }

        [Display(Name = "Name on Government ID:")]
        public string GovernemtIdName { get; set; }

        [Display(Name = "DOB:")]
        public DateTime DateofBirth { get; set; }

        [Display(Name = "Cell Phone Number while travelling:")]
        public string TravelContactNumber { get; set; }

        [Display(Name = "Seat Preference:")]
        public string SeatPreference { get; set; }

        public SFN52712FlightMethod(int id, bool empBookFlight, string freqFlierNumber, string governmentidname, DateTime dateofbirth, string travelcontactnumber, string seatpreference)
        {
            ID = id;
            EmpBookFlight = empBookFlight;
            FreqFlierNumber = freqFlierNumber;
            GovernemtIdName = governmentidname;
            DateofBirth = dateofbirth;
            TravelContactNumber = travelcontactnumber;
            SeatPreference = seatpreference;
        }

        public static SFN52712FlightMethod ConvertDataTableFlightMethod(DataTable dtReq)
        {
            DataRow drReq = dtReq.Rows[0];
            SFN52712FlightMethod req = new SFN52712FlightMethod(
                int.Parse(drReq["ID_NUMBER"].ToString()),
                (drReq["EMPLOYEE_BOOK_FLIGHT"].ToString() == "Y") ? true : false,
                drReq["FREQUENT_FLIER_NUMBER"].ToString(),
                drReq["GOVERNMENT_ID_NAME"].ToString(),
                DateTime.Parse(drReq["DATE_OF_BIRTH"].ToString()),
                drReq["TRAVEL_CONTACT_NUMBER"].ToString(),
                drReq["SEAT_PREFERENCE"].ToString()
                );
            return req;
        }
    }

}