using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using OnlineForms.Models.SFN61065;

namespace OnlineForms.Models.SFN61065
{
    public class SFN61065Model
    {
        [Display(Name = "ID Number (to be assigned)")]
        public int? ID { get; set; }

        [Display(Name = "Requesting employee name")]
        [Required]
        public string Name { get; set; }

        [Display(Name = "Date")]
        public DateTime DateSubmitted { get; set; }

        [Display(Name = "Department")]
        public string Department { get; set; }

        [Display(Name = "Phone")]
        public string Phone { get; set; }

        [Display(Name = "Request type")]
        [Required(ErrorMessage = "Request type is required")]
        public string RequestType { get; set; }

        [Display(Name = "Number of cards")]
        [Required(ErrorMessage = "Number of cards is required")]
        public string NumOfCards { get; set; }

        [Display(Name = "Additional comments")]
        [DataType(DataType.MultilineText)]
        public string AdditionalComments { get; set; }

        [Display(Name = "Submitted by")]
        public string SubmittedBy { get; set; }

        [Display(Name = "Submitted date")]
        public DateTime SubmittedDate { get; set; }

        [Display(Name = "Waiting Approval")]
        public string WaitingApproval { get; set; }

        [Display(Name = "Supervisor approval")]
        public string SupervisorApproval { get; set; }

        [Display(Name = "Supervisor approval date")]
        public string SupervisorApprovalDate { get; set; }

        [Display(Name = "Finance approval")]
        public string FinanceApproval { get; set; }

        [Display(Name = "Finance approval date")]
        public string FinanceApprovalDate { get; set; }

        [Display(Name = "Communications approval")]
        public string CommunicationsApproval { get; set; }

        [Display(Name = "Communications approval date")]
        public string CommunicationsApprovalDate { get; set; }

        [Display(Name = "Requesting employee approval")]
        public string RequestingEmployeeApproval { get; set; }

        [Display(Name = "Requesting employee approval date")]
        public string RequestingEmployeeApprovalDate { get; set; }

        [Display(Name = "Supervisor approved")]
        public bool SupervisorApproved { get; set; }

        [Display(Name = "Finance approved")]
        public bool FinanceApproved { get; set; }

        [Display(Name = "Communications approved")]
        public bool CommunicationsApproved { get; set; }

        [Display(Name = "Requesting employee approved")]
        public bool RequestingEmployeeApproved { get; set; }

        [Display(Name = "Proof comments")]
        [DataType(DataType.MultilineText)]
        public string ProofComments { get; set; }

        public SFN61065Model() { }

        public SFN61065Model(int id, string name, DateTime dateSubmitted, string department, string phone, string requestType, string numOfCards, string addtionalComments, string submittedBy, DateTime submittedDate, string waitingApproval)
        {
            ID = id;
            Name = name;
            DateSubmitted = dateSubmitted;
            Department = department;
            Phone = phone;
            RequestType = requestType;
            NumOfCards = numOfCards;
            AdditionalComments = addtionalComments;
            SubmittedBy = submittedBy;
            SubmittedDate = submittedDate;
            WaitingApproval = waitingApproval;
        }

        public static List<SFN61065Model> ConvertDataTableToBusinessCardRequest(DataTable dtReq)
        {
            List<SFN61065Model> businesscard = new List<SFN61065Model>();
            foreach (DataRow row in dtReq.Rows)
            {
                SFN61065Model reqItem = new SFN61065Model(
                    int.Parse(row["ID_NUMBER"].ToString()),
                    row["REQUESTOR_NAME"].ToString(),
                    DateTime.Parse(row["REQUEST_DATE"].ToString()),
                    row["DEPARTMENT"].ToString(),
                    row["REQUESTOR_PHONE"].ToString(),
                    row["REQUEST_TYPE"].ToString(),
                    row["NUMBER_OF_CARDS"].ToString(),
                    row["ADDITIONAL_COMMENTS"].ToString(),
                    row["SUBMITTED_BY"].ToString(),
                    DateTime.Parse(row["SUBMIT_DATE"].ToString()),
                    row["WAITING_APPROVAL"].ToString()
                );

                businesscard.Add(reqItem);
            }
            return businesscard;
        }

        public static SFN61065Model ConvertDataTableToBusinessCardRequests(DataTable dtReq)
        {
            DataRow drReq = dtReq.Rows[0];
            SFN61065Model req = new SFN61065Model(
                int.Parse(drReq["ID_NUMBER"].ToString()),
                    drReq["REQUESTOR_NAME"].ToString(),
                    DateTime.Parse(drReq["REQUEST_DATE"].ToString()),
                    drReq["DEPARTMENT"].ToString(),
                    drReq["REQUESTOR_PHONE"].ToString(),
                    drReq["REQUEST_TYPE"].ToString(),
                    drReq["NUMBER_OF_CARDS"].ToString(),
                    drReq["ADDITIONAL_COMMENTS"].ToString(),
                    drReq["SUBMITTED_BY"].ToString(),
                    DateTime.Parse(drReq["SUBMIT_DATE"].ToString()),
                    drReq["WAITING_APPROVAL"].ToString()
            );
            return req;
        }
    }
}