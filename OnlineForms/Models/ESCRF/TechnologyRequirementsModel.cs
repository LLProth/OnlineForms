using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace OnlineForms.Models.ESCRF
{
    public class TechnologyRequirementsModel
    {
        public int ID { get; set; }

        [Display(Name = "Is the employee moving to a new location?")]
        [Required(ErrorMessage = "This field is required")]
        public bool EmployeeMoving { get; set; }

        [Display(Name = "New Location")]
        public string NewLocation { get; set; }

        [Display(Name = "Email distribution groups will be the same as:")]
        [Required(ErrorMessage = "This field is required")]
        public string EmailGroups { get; set; }

        [Display(Name = "CMS/CAPS Field Security will be the same as:")]
        [Required(ErrorMessage = "This field is required")]
        public string FieldSecurity { get; set; }

        [Display(Name = "No change")]
        [Required(ErrorMessage = "This field is required")]
        public bool NoChange { get; set; }

        [Display(Name = "CMS")]
        [Required(ErrorMessage = "This field is required")]
        public bool CMS { get; set; }

        [Display(Name = "InfoPath")]
        [Required(ErrorMessage = "This field is required")]
        public bool InfoPath { get; set; }

        [Display(Name = "Microsoft Reports")]
        [Required(ErrorMessage = "This field is required")]
        public bool MicrosoftReports { get; set; }

        [Display(Name = "Rec. Manager")]
        [Required(ErrorMessage = "This field is required")]
        public bool RecManager { get; set; }

        [Display(Name = "Great Plains")]
        [Required(ErrorMessage = "This field is required")]
        public bool GreatPlains { get; set; }

        [Display(Name = "Accounting Utility")]
        [Required(ErrorMessage = "This field is required")]
        public bool AccountingUtility { get; set; }

        [Display(Name = "IT Works")]
        [Required(ErrorMessage = "This field is required")]
        public bool ITWorks { get; set; }

        [Display(Name = "File to Filenet")]
        [Required(ErrorMessage = "This field is required")]
        public bool FileToFilenet { get; set; }

        [Display(Name = "Indexing")]
        [Required(ErrorMessage = "This field is required")]
        public bool Indexing { get; set; }

        [Display(Name = "Verifier")]
        [Required(ErrorMessage = "This field is required")]
        public bool Verifier { get; set; }

        [Display(Name = "Sec of State")]
        [Required(ErrorMessage = "This field is required")]
        public bool SecOfState { get; set; }

        [Display(Name = "DOT")]
        [Required(ErrorMessage = "This field is required")]
        public bool DOT { get; set; }

        [Display(Name = "Job Service")]
        [Required(ErrorMessage = "This field is required")]
        public bool JobService { get; set; }

        [Display(Name = "CAPS")]
        [Required(ErrorMessage = "This field is required")]
        public bool CAPS { get; set; }

        [Display(Name = "Legal/Rehab")]
        [Required(ErrorMessage = "This field is required")]
        public bool Legal { get; set; }

        [Display(Name = "myWSI")]
        [Required(ErrorMessage = "This field is required")]
        public bool MyWsi { get; set; }

        [Display(Name = "Other")]
        public string Other { get; set; }

        [Display(Name = "Who should their work queue be transferred to?")]
        public string WorkQueue { get; set; }

        [Display(Name = "Are they keeping their current phone number?")]
        [Required(ErrorMessage = "This field is required")]
        public bool CurrentPhone { get; set; }

        [Display(Name = "Does the employee need to use the Call Recording System?")]
        [Required(ErrorMessage = "This field is required")]
        public bool CallRecording { get; set; }

        [Display(Name = "Does their electronic signature need to change?")]
        [Required(ErrorMessage = "This field is required")]
        public bool ElectronicSignature { get; set; }

        [Display(Name = "New signature")]
        public string NewSignature { get; set; }

        [Display(Name = "Comments/Special Instructions:")]
        [DataType(DataType.MultilineText)]
        public string Comments { get; set; }

        public TechnologyRequirementsModel(int id, bool employeeMoving, string newLocation, string emailGroups, string fieldSecurity, bool noChange,
            bool cms, bool infoPath, bool microsoftReports, bool recManager, bool greatPlains, bool accountingUtility,
            bool itWorks, bool fileToFilenet, bool indexing, bool verifier, bool secOfState, bool dot, bool jobService,
            bool caps, bool legal, bool myWsi, string other, string workQueue, bool currentPhone, bool callRecording,
            bool electronicSignature, string newSignature, string comments)
        {
            ID = id;
            EmployeeMoving = employeeMoving;
            NewLocation = newLocation;
            EmailGroups = emailGroups;
            FieldSecurity = fieldSecurity;
            NoChange = noChange;
            CMS = cms;
            InfoPath = infoPath;
            MicrosoftReports = microsoftReports;
            RecManager = recManager;
            GreatPlains = greatPlains;
            AccountingUtility = accountingUtility;
            ITWorks = itWorks;
            FileToFilenet = fileToFilenet;
            Indexing = indexing;
            Verifier = verifier;
            SecOfState = secOfState;
            DOT = dot;
            JobService = jobService;
            CAPS = caps;
            Legal = legal;
            MyWsi = myWsi;
            Other = other;
            WorkQueue = workQueue;
            CurrentPhone = currentPhone;
            CallRecording = callRecording;
            ElectronicSignature = electronicSignature;
            NewSignature = newSignature;
            Comments = comments;
        }

        public static List<TechnologyRequirementsModel> ConvertDataTableTechRequirements(DataTable dtReq)
        {
            List<TechnologyRequirementsModel> approval = new List<TechnologyRequirementsModel>();
            foreach (DataRow row in dtReq.Rows)
            {
                TechnologyRequirementsModel reqItem = new TechnologyRequirementsModel(
                int.Parse(row["ID_NUMBER"].ToString()),
                (row["EMPLOYEE_MOVING"].ToString() == "Y") ? true : false,
                row["NEW_LOCATION"].ToString(),
                row["EMAIL_GROUPS"].ToString(),
                row["FIELD_SECURITY"].ToString(),
                (row["NO_CHANGE"].ToString() == "Y") ? true : false,
                (row["CMS"].ToString() == "Y") ? true : false,
                (row["INFOPATH"].ToString() == "Y") ? true : false,
                (row["MICROSOFT_REPORTS"].ToString() == "Y") ? true : false,
                (row["RECOMMENDATION_MANAGER"].ToString() == "Y") ? true : false,
                (row["GREAT_PLAINS"].ToString() == "Y") ? true : false,
                (row["ACCOUNTING_UTILITY"].ToString() == "Y") ? true : false,
                (row["IT_WORKS"].ToString() == "Y") ? true : false,
                (row["FILE_TO_FILENET"].ToString() == "Y") ? true : false,
                (row["INDEXING_APP"].ToString() == "Y") ? true : false,
                (row["VERIFIER_APP"].ToString() == "Y") ? true : false,
                (row["SEC_OF_STATE"].ToString() == "Y") ? true : false,
                (row["DOT"].ToString() == "Y") ? true : false,
                (row["JOB_SERVICE"].ToString() == "Y") ? true : false,
                (row["CAPS"].ToString() == "Y") ? true : false,
                (row["LEGAL"].ToString() == "Y") ? true : false,
                (row["MYWSI"].ToString() == "Y") ? true : false,
                row["OTHER_APP"].ToString(),
                row["WORK_QUEUE"].ToString(),
                (row["CURRENT_PHONE"].ToString() == "Y") ? true : false,
                (row["CALL_RECORDING"].ToString() == "Y") ? true : false,
                (row["ELECTRONIC_SIGNATURE_CHANGE"].ToString() == "Y") ? true : false,
                row["NEW_SIGNATURE"].ToString(),
                row["ADDITIONAL_COMMENTS"].ToString()
                );
                approval.Add(reqItem);
            }
            return approval;
        }
        public static TechnologyRequirementsModel ConvertDataTableToTechRequirement(DataTable dtReq)
        {
            DataRow drReq = dtReq.Rows[0];
            TechnologyRequirementsModel req = new TechnologyRequirementsModel(
                int.Parse(drReq["ID_NUMBER"].ToString()),
                (drReq["EMPLOYEE_MOVING"].ToString() == "Y") ? true : false,
                drReq["NEW_LOCATION"].ToString(),
                drReq["EMAIL_GROUPS"].ToString(),
                drReq["FIELD_SECURITY"].ToString(),
                (drReq["NO_CHANGE"].ToString() == "Y") ? true : false,
                (drReq["CMS"].ToString() == "Y") ? true : false,
                (drReq["INFOPATH"].ToString() == "Y") ? true : false,
                (drReq["MICROSOFT_REPORTS"].ToString() == "Y") ? true : false,
                (drReq["RECOMMENDATION_MANAGER"].ToString() == "Y") ? true : false,
                (drReq["GREAT_PLAINS"].ToString() == "Y") ? true : false,
                (drReq["ACCOUNTING_UTILITY"].ToString() == "Y") ? true : false,
                (drReq["IT_WORKS"].ToString() == "Y") ? true : false,
                (drReq["FILE_TO_FILENET"].ToString() == "Y") ? true : false,
                (drReq["INDEXING_APP"].ToString() == "Y") ? true : false,
                (drReq["VERIFIER_APP"].ToString() == "Y") ? true : false,
                (drReq["SEC_OF_STATE"].ToString() == "Y") ? true : false,
                (drReq["DOT"].ToString() == "Y") ? true : false,
                (drReq["JOB_SERVICE"].ToString() == "Y") ? true : false,
                (drReq["CAPS"].ToString() == "Y") ? true : false,
                (drReq["LEGAL"].ToString() == "Y") ? true : false,
                (drReq["MYWSI"].ToString() == "Y") ? true : false,
                drReq["OTHER_APP"].ToString(),
                drReq["WORK_QUEUE"].ToString(),
                (drReq["CURRENT_PHONE"].ToString() == "Y") ? true : false,
                (drReq["CALL_RECORDING"].ToString() == "Y") ? true : false,
                (drReq["ELECTRONIC_SIGNATURE_CHANGE"].ToString() == "Y") ? true : false,
                drReq["NEW_SIGNATURE"].ToString(),
                drReq["ADDITIONAL_COMMENTS"].ToString()
            );
            return req;
        }

    }
}