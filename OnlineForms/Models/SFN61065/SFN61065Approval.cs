using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using OnlineForms.Models.SFN61065;

namespace OnlineForms.Models.SFN61065
{
    public class SFN61065Approval
    {
        public int ID { get; set; }

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

        [Display(Name = "Proof uploaded")]
        public bool ProofUploaded { get; set; }

        [Display(Name = "Proof comments")]
        [DataType(DataType.MultilineText)]
        public string ProofComments { get; set; }

        public SFN61065Approval() { }
        public SFN61065Approval(
            string supervisorApproval, string supervisorApprovalDate, string financeApproval, string financeApprovalDate, string communicationsApproval, string communicationsApprovalDate, string requestingEmployeeApproval,
            string requestingEmployeeApprovalDate, bool supervisorApproved, bool financeApproved, bool communicationsApproved, bool requestingEmployeeApproved, string proofComments, bool proofUploaded)
        {
            SupervisorApproval = supervisorApproval;
            SupervisorApprovalDate = supervisorApprovalDate;
            FinanceApproval = financeApproval;
            FinanceApprovalDate = financeApprovalDate;
            CommunicationsApproval = communicationsApproval;
            CommunicationsApprovalDate = communicationsApprovalDate;
            RequestingEmployeeApproval = requestingEmployeeApproval;
            RequestingEmployeeApprovalDate = requestingEmployeeApprovalDate;
            SupervisorApproved = supervisorApproved;
            FinanceApproved = financeApproved;
            CommunicationsApproved = communicationsApproved;
            RequestingEmployeeApproved = requestingEmployeeApproved;
            ProofComments = proofComments;
            ProofUploaded = proofUploaded;
        }
        public static List<SFN61065Approval> ConvertDataTableToApproval(DataTable dtReq)
        {
            List<SFN61065Approval> approval = new List<SFN61065Approval>();
            foreach (DataRow row in dtReq.Rows)
            {
                SFN61065Approval reqItem = new SFN61065Approval(
                row["SUPERVISOR_APPROVAL"].ToString(),
                row["SUPERVISOR_APPROVE_DATE"].ToString(),
                row["FINANCE_APPROVAL"].ToString(),
                row["FINANCE_APPROVE_DATE"].ToString(),
                row["COMMUNICATIONS_APPROVAL"].ToString(),
                row["COMMUNICATIONS_APPROVE_DATE"].ToString(),
                row["REQUESTING_EMPLOYEE_APPROVAL"].ToString(),
                row["REQUESTING_EMPLOYEE_APPROVE_DATE"].ToString(),
                (row["SUPERVISOR_APPROVED"].ToString() == "Y") ? true : false,
                (row["FINANCE_APPROVED"].ToString() == "Y") ? true : false,
                (row["COMMUNICATIONS_APPROVED"].ToString() == "Y") ? true : false,
                (row["REQUESTING_EMPLOYEE_APPROVED"].ToString() == "Y") ? true : false,
                 row["PROOF_COMMENTS"].ToString(),
                 (row["PROOF_UPLOADED"].ToString() == "Y") ? true : false
                );
                approval.Add(reqItem);
            }
            return approval;
        }
        public static SFN61065Approval ConvertDataTableToApprovals(DataTable dtReq)
        {
            DataRow drReq = dtReq.Rows[0];
            SFN61065Approval req = new SFN61065Approval(
                
                drReq["SUPERVISOR_APPROVAL"].ToString(),
                drReq["SUPERVISOR_APPROVE_DATE"].ToString(),
                drReq["FINANCE_APPROVAL"].ToString(),
                drReq["FINANCE_APPROVE_DATE"].ToString(),
                drReq["COMMUNICATIONS_APPROVAL"].ToString(),
                drReq["COMMUNICATIONS_APPROVE_DATE"].ToString(),
                drReq["REQUESTOR_APPROVAL"].ToString(),
                drReq["REQUESTOR_APPROVE_DATE"].ToString(),
                (drReq["SUPERVISOR_APPROVED"].ToString() == "Y") ? true : false,
                (drReq["FINANCE_APPROVED"].ToString() == "Y") ? true : false,
                (drReq["COMMUNICATIONS_APPROVED"].ToString() == "Y") ? true : false,
                (drReq["REQUESTOR_APPROVED"].ToString() == "Y") ? true : false,
                 drReq["PROOF_COMMENTS"].ToString(),
                (drReq["PROOF_UPLOADED"].ToString() == "Y") ? true : false
            );
            return req;
        }
    }
}