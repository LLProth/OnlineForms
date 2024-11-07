using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity;
using OnlineForms.Models;
using OnlineForms.Models.SFN18795;

namespace OnlineForms.Models.SFN18795
{
    public class SFN18795Model
    {
        [Key]
        [Display(Name = "ID Number (to be assigned)")]        
        public int ID { get; set; }

        [Display(Name = "ID Number (to be assigned)")]
        public string RequisitionID { get; set; }

        [Display(Name = "Requesting Employee Name")]
        [Required]
		[StringLength(100, ErrorMessage = "Employee Name can only be 100 characters.")]
		public string Name { get; set; }

        [Display(Name = "Employee Id")]
        public string EmplID { get; set; }

        [Display(Name = "Department/Budget")]
		[StringLength(100, ErrorMessage = "Employee Name can only be 100 characters.")]
		public string Department { get; set; }

        [Display(Name = "Date")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime DateSubmitted { get; set; }

        [Display(Name = "Suggested Contractor Name")]
		[StringLength(100, ErrorMessage = "Employee Name can only be 100 characters.")]
		public string Contractor { get; set; }

        [Display(Name = "Is this a software/hardware request?")]
        public bool SoftwareHardware { get; set; }

        [Display(Name = "Estimated Start Date")]
        public DateTime? EstimatedStartDate { get; set; }

        [Display(Name = "Estimated Completion Date")]
        public DateTime? EstimatedCompleteDate { get; set; }

        [Display(Name = "Total")]
        public float ReqItemsTotal { get; set; }

        [Display(Name = "Form is Submitted")]
        public bool FormSubmitted { get; set; }

        [Display(Name = "Form is Completed")]
        public bool FormCompleted { get; set; }

        [Display(Name = "Form is Denied")]
        public bool FormDenied { get; set; }

        [Display(Name = "Form is Revised")]
        public bool FormRevised { get; set; }

        [Display(Name = "Revised Reason")]
        public string RevisedComment { get; set; }

        [Display(Name = "Procurement is Processing")]
        public bool ProcurementProcessing { get; set; }

        [Display(Name = "Procurement Date")]
        [DataType(DataType.Date)]
        public DateTime ProcurementProcessedDate { get; set; }

        [Display(Name = "Procurement Officer Signature")]
        public string ProcurementOfficerSignature { get; set; }

        [Display(Name = "Procurement Officer Signature Date")]
        public DateTime ProcurementOfficerSignatureDate { get; set; }

        [Display(Name = "IT Rep Signature")]
        public string ITRepSignature { get; set; }

        [Display(Name = "IT Rep Signature Date")]
        public DateTime ITRepSignatureDate { get; set; }

        [Display(Name = "Supervisor Signature")]
        public string SupervisorSignature { get; set; }

        [Display(Name = "Supervisor  Signature Date")]
        public DateTime SupervisorSignatureDate { get; set; }

        [Display(Name = "Department Signature")]
        public string DepartmentSignature { get; set; }

        [Display(Name = "Department Signature Date")]
        public DateTime DepartmentSignatureDate { get; set; }

        [Display(Name = "Chief Signature")]
        public string ChiefSignature { get; set; }

        [Display(Name = "Chief  Signature Date")]
        public DateTime ChiefSignatureDate { get; set; }

        [Display(Name = "Director Signature")]
        public string DirectorSignature { get; set; }

        [Display(Name = "Director  Signature Date")]
        public DateTime DirectorSignatureDate { get; set; }

        [Display(Name = "Waiting Approval")]
        public string WaitingApproval { get; set; }

        [Display(Name = "Modified Date")]
        public DateTime ModifiedDate { get; set; }

        public SFN18795Model() { }

        public SFN18795Model(int id, string requisitionID, string name, string emplid, string department, DateTime dateSubmitted, string contractor, 
            bool softwareHardware, DateTime estimatedStartDate, DateTime estimatedCompleteDate, float total, string itRepSignature, DateTime iTRepSignatureDate,
            string supervisorSignature, DateTime supervisorSignatureDate, string departmentSignature, DateTime departmentSignatureDate, string chiefSignature, 
            DateTime chiefSignatureDate, string directorSignature, DateTime directorSignatureDate, string procurementOfficerSignature, DateTime procurementOfficerSignatureDate, 
            string waitingApproval, bool formSubmitted, bool formCompleted, bool formDenied, bool formRevised, string revisedComment, bool procurementProcessing, 
            DateTime procurementProcessedDate)
        {
            ID = id;
            RequisitionID = requisitionID;
            Name = name;
            EmplID = emplid;
            Department = department;
            DateSubmitted = dateSubmitted;
            Contractor = contractor;
            SoftwareHardware = softwareHardware;
            EstimatedStartDate = estimatedStartDate;
            EstimatedCompleteDate = estimatedCompleteDate;
            ReqItemsTotal= total;
            ITRepSignature = itRepSignature;
            ITRepSignatureDate = iTRepSignatureDate;
            SupervisorSignature = supervisorSignature;
            SupervisorSignatureDate = supervisorSignatureDate;
            DepartmentSignature = departmentSignature;
            DepartmentSignatureDate = departmentSignatureDate;
            ChiefSignature = chiefSignature;
            ChiefSignatureDate = chiefSignatureDate;
            DirectorSignature = directorSignature;
            DirectorSignatureDate = directorSignatureDate;
            ProcurementOfficerSignature = procurementOfficerSignature;
            ProcurementOfficerSignatureDate = procurementOfficerSignatureDate;
            WaitingApproval = waitingApproval;
            FormSubmitted = formSubmitted;
            FormCompleted = formCompleted;
            FormDenied = formDenied;
            FormRevised = formRevised;
			RevisedComment = revisedComment;
			ProcurementProcessing = procurementProcessing;
            ProcurementProcessedDate = procurementProcessedDate;
        }

        public static SFN18795Model ConvertDataTableToRequisition(DataTable dtReq)
        {
            DataRow drReq = dtReq.Rows[0];
            SFN18795Model req = new SFN18795Model(
                int.Parse(drReq["ID_NUMBER"].ToString()),
                drReq["REQUISITION_ID"].ToString(),
                drReq["REQUEST_EMPLOYEE"].ToString(),
                drReq["EMPLID"].ToString(),
                drReq["DEPARTMENT_BUDGET"].ToString(),
                DateTime.Parse(drReq["REQUEST_DATE"].ToString()),
                drReq["SUGGESTED_CONTRACTOR"].ToString(),
                (drReq["SOFTWARE_HARDWARE_REQUEST"].ToString() == "Y") ? true : false,
                DateTime.Parse(drReq["ESTIMATED_START_DATE"].ToString()),
                DateTime.Parse(drReq["ESTIMATED_COMPLETE_DATE"].ToString()),
                float.Parse(drReq["REQ_ITEMS_TOTAL"].ToString()),
                drReq["IT_REP_SIGNATURE"].ToString(),
                (drReq["IT_REP_SIGNATURE_DATE"].ToString() == "") ? DateTime.Parse("1/1/0001") : DateTime.Parse(drReq["IT_REP_SIGNATURE_DATE"].ToString()),
                drReq["SUPERVISOR_SIGNATURE"].ToString(),
                (drReq["SUPERVISOR_SIGNATURE_DATE"].ToString() == "") ? DateTime.Parse("1/1/0001") : DateTime.Parse(drReq["SUPERVISOR_SIGNATURE_DATE"].ToString()),
                drReq["DEPARTMENT_SIGNATURE"].ToString(),
                (drReq["DEPARTMENT_SIGNATURE_DATE"].ToString() == "") ? DateTime.Parse("1/1/0001") : DateTime.Parse(drReq["DEPARTMENT_SIGNATURE_DATE"].ToString()),
                drReq["CHIEF_SIGNATURE"].ToString(),
                (drReq["CHIEF_SIGNATURE_DATE"].ToString() == "") ? DateTime.Parse("1/1/0001") : DateTime.Parse(drReq["CHIEF_SIGNATURE_DATE"].ToString()),
                drReq["DIRECTOR_SIGNATURE"].ToString(),
                (drReq["DIRECTOR_SIGNATURE_DATE"].ToString() == "") ? DateTime.Parse("1/1/0001") : DateTime.Parse(drReq["DIRECTOR_SIGNATURE_DATE"].ToString()),
                drReq["PROCUREMENT_OFFICER_SIGNATURE"].ToString(),
                (drReq["PROCUREMENT_OFFICER_SIGNATURE_DATE"].ToString() == "") ? DateTime.Parse("1/1/0001") : DateTime.Parse(drReq["PROCUREMENT_OFFICER_SIGNATURE_DATE"].ToString()),
                drReq["WAITING_APPROVAL"].ToString(),
                (drReq["FORM_SUBMITTED"].ToString() == "Y") ? true : false,
                (drReq["FORM_COMPLETED"].ToString() == "Y") ? true : false,
                (drReq["FORM_DENIED"].ToString() == "Y") ? true : false,
				(drReq["FORM_REVISED"].ToString() == "Y") ? true : false,
				drReq["REVISED_COMMENT"].ToString(),
				(drReq["PROCUREMENT_PROCESSING"].ToString() == "Y") ? true : false,
                (drReq["PROCUREMENT_PROCESS_DATE"].ToString() == "") ? DateTime.Parse("1/1/0001") : DateTime.Parse(drReq["PROCUREMENT_PROCESS_DATE"].ToString())
            );
            return req;
        }

        public static List < SFN18795Model > GetSFN18795List(DataTable dlist)
        {
            List < SFN18795Model > SFN18795List = new List<SFN18795Model>();
            foreach (DataRow row in dlist.Rows)
            {
                SFN18795Model reqItem = new SFN18795Model(
                int.Parse(row["ID_NUMBER"].ToString()),
                row["REQUISITION_ID"].ToString(),
                row["REQUEST_EMPLOYEE"].ToString(),
                row["EMPLID"].ToString(),
                row["DEPARTMENT_BUDGET"].ToString(),
                DateTime.Parse(row["REQUEST_DATE"].ToString()),
                row["SUGGESTED_CONTRACTOR"].ToString(),
                (row["SOFTWARE_HARDWARE_REQUEST"].ToString() == "Y") ? true : false,
                DateTime.Parse(row["ESTIMATED_START_DATE"].ToString()),
                DateTime.Parse(row["ESTIMATED_COMPLETE_DATE"].ToString()),
                float.Parse(row["REQ_ITEMS_TOTAL"].ToString()),
                row["IT_REP_SIGNATURE"].ToString(),
                (row["IT_REP_SIGNATURE_DATE"].ToString() == "") ? DateTime.Parse("1/1/0001") : DateTime.Parse(row["IT_REP_SIGNATURE_DATE"].ToString()),
                row["SUPERVISOR_SIGNATURE"].ToString(),
                (row["SUPERVISOR_SIGNATURE_DATE"].ToString() == "") ? DateTime.Parse("1/1/0001") : DateTime.Parse(row["SUPERVISOR_SIGNATURE_DATE"].ToString()),
                row["DEPARTMENT_SIGNATURE"].ToString(),
                (row["DEPARTMENT_SIGNATURE_DATE"].ToString() == "") ? DateTime.Parse("1/1/0001") : DateTime.Parse(row["DEPARTMENT_SIGNATURE_DATE"].ToString()),
                row["CHIEF_SIGNATURE"].ToString(),
                (row["CHIEF_SIGNATURE_DATE"].ToString() == "") ? DateTime.Parse("1/1/0001") : DateTime.Parse(row["CHIEF_SIGNATURE_DATE"].ToString()),
                row["DIRECTOR_SIGNATURE"].ToString(),
                (row["DIRECTOR_SIGNATURE_DATE"].ToString() == "") ? DateTime.Parse("1/1/0001") : DateTime.Parse(row["DIRECTOR_SIGNATURE_DATE"].ToString()),
                row["PROCUREMENT_OFFICER_SIGNATURE"].ToString(),
                (row["PROCUREMENT_OFFICER_SIGNATURE_DATE"].ToString() == "") ? DateTime.Parse("1/1/0001") : DateTime.Parse(row["PROCUREMENT_OFFICER_SIGNATURE_DATE"].ToString()),
                row["WAITING_APPROVAL"].ToString(),
                (row["FORM_SUBMITTED"].ToString() == "Y") ? true : false,
                (row["FORM_COMPLETED"].ToString() == "Y") ? true : false,
                (row["FORM_DENIED"].ToString() == "Y") ? true : false,
			    (row["FORM_REVISED"].ToString() == "Y") ? true : false,
				row["REVISED_COMMENT"].ToString(),
				(row["PROCUREMENT_PROCESSING"].ToString() == "Y") ? true : false,
                (row["PROCUREMENT_PROCESS_DATE"].ToString() == "") ? DateTime.Parse("1/1/0001") : DateTime.Parse(row["PROCUREMENT_PROCESS_DATE"].ToString())
                );
                SFN18795List.Add(reqItem);
            }

            return SFN18795List;

        }

    }
    public class SFN18795DBContext : DbContext
    {
        public DbSet<SFN18795Model> SFN18795s { get; set; }
    }
}