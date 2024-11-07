using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace OnlineForms.Models.SFN54497
{
    public class SFN54497Model
    {
        public int ID { get; set; }

        [Display(Name = "Position title")]
        [Required(ErrorMessage = "Position title is required")]
        public string PositionTitle { get; set; }

        [Display(Name = "Department")]
        [Required(ErrorMessage = "Department must be selected from list")]
        public string Department { get; set; }

        [Display(Name = "Location")]
        [Required(ErrorMessage = "Location must be selected from list")]
        public string Location { get; set; }

        [Display(Name = "Type of hire")]
        [Required(ErrorMessage = "Type of hire must be selected from list")]
        public string TypeOfHire { get; set; }

        [Display(Name = "Number of openings")]
        [Required(ErrorMessage = "Number of openings is required")]
        public string NumberOfOpenings { get; set; }

        [Display(Name = "Salary range")]
        public string SalaryRange { get; set; }

        [Display(Name = "Position reports to")]
        [Required(ErrorMessage = "Position reports to must be selected from list")]
        public string PositionReportsTo { get; set; }

        [Display(Name = "Hours")]
        [Required(ErrorMessage = "Hours is required")]
        public string Hours { get; set; }

        [Display(Name = "Hours per week")]
        [Required(ErrorMessage = "Hours per week is required")]
        public string HoursPerWeek { get; set; }

        [Display(Name = "Desired start date")]
        [Required(ErrorMessage = "Desired start date is required")]
        public DateTime DesiredStartDate { get; set; }

        [Display(Name = "Projected end date")]
        [Required(ErrorMessage = "Projected end date is required")]
        public DateTime ProjectedEndDate { get; set; }

        [Display(Name = "Position justification")]
        [DataType(DataType.MultilineText)]
        [Required(ErrorMessage = "Positior justification is required")]
        public string PositionJustification { get; set; }

        [Display(Name = "Submitted by")]
        public string SubmittedBy { get; set; }

        [Display(Name = "Submit date")]
        public string SubmitDate { get; set; }

        [Display(Name = "Submitter comments")]
        [DataType(DataType.MultilineText)]
        public string SubmitterComments { get; set; }

        [Display(Name = "Waiting Approval")]
        public string WaitingApproval { get; set; }

        [Display(Name = "Form Submitted")]
        public bool FormSubmitted { get; set; }

        [Display(Name = "Form Approved")]
        public bool FormApproved { get; set; }

        [Display(Name = "Form Denied")]
        public bool FormDenied { get; set; }
        public SFN54497Model(int id, string positionTitle, string department, string location, string typeOfHire, string numberOfOpenings, string salaryRange, string positionReportsTo,
            string hours, string hoursPerWeek, DateTime desiredStartDate, DateTime projectedEndDate, string positionJustification, string submittedBy, string submitDate, 
            string submitterComments, string waitingApproval, bool formSubmitted, bool formApproved, bool formDenied)
        {
            ID = id;
            PositionTitle = positionTitle;
            Department = department;
            Location = location;
            TypeOfHire = typeOfHire;
            NumberOfOpenings = numberOfOpenings;
            SalaryRange = salaryRange;
            PositionReportsTo = positionReportsTo;
            Hours = hours;
            HoursPerWeek = hoursPerWeek;
            DesiredStartDate = desiredStartDate;
            ProjectedEndDate = projectedEndDate;
            PositionJustification = positionJustification;
            SubmittedBy = submittedBy;
            SubmitDate = submitDate;
            SubmitterComments = submitterComments;
            WaitingApproval = waitingApproval;
            FormSubmitted = formSubmitted;
            FormApproved = formApproved;
            FormDenied = formDenied;
        }
        public static List<SFN54497Model> ConvertDataTableToStaffRequest(DataTable dtReq)
        {
            List<SFN54497Model> staffRequest = new List<SFN54497Model>();
            foreach (DataRow row in dtReq.Rows)
            {
                SFN54497Model reqItem = new SFN54497Model(
                    int.Parse(row["ID_NUMBER"].ToString()),
                    row["POSITION_TITLE"].ToString(),
                    row["DEPARTMENT"].ToString(),
                    row["POSITION_LOCATION"].ToString(),
                    row["HIRE_TYPE"].ToString(),
                    row["NUM_OF_OPENINGS"].ToString(),
                    row["SALARY_RANGE"].ToString(),
                    row["POSITION_REPORTS_TO"].ToString(),
                    row["HOURS"].ToString(),
                    row["HOURS_PER_WEEK"].ToString(),
                    DateTime.Parse(row["DESIRED_START_DATE"].ToString()),
                    DateTime.Parse(row["PROJECTED_END_DATE"].ToString()),
                    row["POSITION_JUSTIFICATION"].ToString(),
                    row["SUBMITTED_BY"].ToString(),
                    row["SUBMIT_DATE"].ToString(),
                    row["SUBMITTER_COMMENTS"].ToString(),
                    row["WAITING_APPROVAL"].ToString(),
                    (row["FORM_SUBMITTED"].ToString() == "Y") ? true : false,
                    (row["FORM_APPROVED"].ToString() == "Y") ? true : false,
                    (row["FORM_DENIED"].ToString() == "Y") ? true : false
                );
                staffRequest.Add(reqItem);
            }
            return staffRequest;
        }
        public static SFN54497Model ConvertDataTableToStaffRequests(DataTable dtReq)
        {
            DataRow drReq = dtReq.Rows[0];
            SFN54497Model req = new SFN54497Model(
                int.Parse(drReq["ID_NUMBER"].ToString()),
                drReq["POSITION_TITLE"].ToString(),
                drReq["DEPARTMENT"].ToString(),
                drReq["POSITION_LOCATION"].ToString(),
                drReq["HIRE_TYPE"].ToString(),
                drReq["NUM_OF_OPENINGS"].ToString(),
                drReq["SALARY_RANGE"].ToString(),
                drReq["POSITION_REPORTS_TO"].ToString(),
                drReq["HOURS"].ToString(),
                drReq["HOURS_PER_WEEK"].ToString(),
                DateTime.Parse(drReq["DESIRED_START_DATE"].ToString()),
                DateTime.Parse(drReq["PROJECTED_END_DATE"].ToString()),
                drReq["POSITION_JUSTIFICATION"].ToString(),
                drReq["SUBMITTED_BY"].ToString(),
                drReq["SUBMIT_DATE"].ToString(),
                drReq["SUBMITTER_COMMENTS"].ToString(),
                drReq["WAITING_APPROVAL"].ToString(),
                (drReq["FORM_SUBMITTED"].ToString() == "Y") ? true : false,
                (drReq["FORM_APPROVED"].ToString() == "Y") ? true : false,
                (drReq["FORM_DENIED"].ToString() == "Y") ? true : false
            );
            return req;
        }
    }
}