using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using OnlineForms.Models.SFN54497;

namespace OnlineForms.Models.SFN54497
{
    public class SFN54497Approval
    {
        public int ID { get; set; }

        [Display(Name = "Department Director approval")]
        public string DepartmentDirectorApproval { get; set; }

        [Display(Name = "Department Director approved?")]
        public bool DepartmentDirectorApproved { get; set; }

        [Display(Name = "Department Director approval date")]
        public string DepartmentDirectorApproveDate { get; set; }

        [Display(Name = "Department Director comments")]
        [DataType(DataType.MultilineText)]
        public string DepartmentDirectorComments { get; set; }

        [Display(Name = "Division Chief approval")]
        public string DivisionChiefApproval { get; set; }

        [Display(Name = "Division Chief approved?")]
        public bool DivisionChiefApproved { get; set; }
        
        [Display(Name = "Division Chief approval date")]
        public string DivisionChiefApproveDate { get; set; }

        [Display(Name = "Division Chief comments")]
        [DataType(DataType.MultilineText)]
        public string DivisionChiefComments { get; set; }

        [Display(Name = "Human Resources approval")]
        public string HumanResourcesApproval { get; set; }

        [Display(Name = "Human resources approved?")]
        public bool HumanResourcesApproved { get; set; }

        [Display(Name = "Human Resources approval date")]
        public string HumanResourcesApproveDate { get; set; }

        [Display(Name = "Human Resources comments")]
        [DataType(DataType.MultilineText)]
        public string HumanResourcesComments { get; set; }

        [Display(Name = "Director of Finance approval")]
        public string DirectorOfFinanceApproval { get; set; }

        [Display(Name = "Director of Finance approved?")]
        public bool DirectorOfFinanceApproved { get; set; }

        [Display(Name = "Director of Finance approval date")]
        public string DirectorOfFinanceApproveDate { get; set; }

        [Display(Name = "Director of Finance comments")]
        [DataType(DataType.MultilineText)]
        public string DirectorOfFinanceComments { get; set; }

        [Display(Name = "Agency Director approval")]
        public string AgencyDirectorApproval { get; set; }

        [Display(Name = "Agency Director approved?")]
        public bool AgencyDirectorApproved { get; set; }

        [Display(Name = "Agency Director approval date")]
        public string AgencyDirectorApproveDate { get; set; }

        [Display(Name = "Agency Director comments")]
        [DataType(DataType.MultilineText)]
        public string AgencyDirectorComments { get; set; }
        public SFN54497Approval() { }
        public SFN54497Approval(
            int id, string departmentDirectorApproval, bool departmentDirector, string departmentDirectorApproveDate, string departmentDirectorComments, string divisionChiefApproval, bool divisionChief, string divisionChiefApproveDate, string divisionChiefComments,
            string humanResourcesApproval, bool hr, string humanResourcesApproveDate, string humanResourcesComments, string directorOfFinanceApproval, bool directorOfFinance, string directorOfFinanceApproveDate, string directorOfFinanceComments,
            string agencyDirectorApproval, bool agencyDirector, string agencyDirectorApproveDate, string agencyDirectorComments)
        {
            ID = id; 
            DepartmentDirectorApproval = departmentDirectorApproval;
            DepartmentDirectorApproved = departmentDirector;
            DepartmentDirectorApproveDate = departmentDirectorApproveDate;
            DepartmentDirectorComments = departmentDirectorComments;
            DivisionChiefApproval = divisionChiefApproval;
            DivisionChiefApproved = divisionChief;
            DivisionChiefApproveDate = divisionChiefApproveDate;
            DivisionChiefComments = divisionChiefComments;
            HumanResourcesApproval = humanResourcesApproval;
            HumanResourcesApproved = hr;
            HumanResourcesApproveDate = humanResourcesApproveDate;
            HumanResourcesComments = humanResourcesComments;
            DirectorOfFinanceApproval = directorOfFinanceApproval;
            DirectorOfFinanceApproved = directorOfFinance;
            DirectorOfFinanceApproveDate = directorOfFinanceApproveDate;
            DirectorOfFinanceComments = directorOfFinanceComments;
            AgencyDirectorApproval = agencyDirectorApproval;
            AgencyDirectorApproved = agencyDirector;
            AgencyDirectorApproveDate = agencyDirectorApproveDate;
            AgencyDirectorComments = agencyDirectorComments;
        }
        public static List<SFN54497Approval> ConvertDataTableToApproval(DataTable dtReq)
        {
            List<SFN54497Approval> approval = new List<SFN54497Approval>();
            foreach (DataRow row in dtReq.Rows)
            {
                SFN54497Approval reqItem = new SFN54497Approval(
                int.Parse(row["ID_NUMBER"].ToString()),
                row["DEPARTMENT_DIRECTOR_APPROVAL"].ToString(),
                (row["DEPARTMENT_DIRECTOR_APPROVED"].ToString() == "Y") ? true : false,
                row["DEPARTMENT_DIRECTOR_APPROVE_DATE"].ToString(),
                row["DEPARTMENT_DIRECTOR_COMMENTS"].ToString(),
                row["DIVISION_CHIEF_APPROVAL"].ToString(),
                (row["DIVISION_CHIEF_APPROVED"].ToString() == "Y") ? true : false,
                row["DIVISION_CHIEF_APPROVE_DATE"].ToString(),
                row["DIVISION_CHIEF_COMMENTS"].ToString(),
                row["HUMAN_RESOURCES_APPROVAL"].ToString(),
                (row["HUMAN_RESOURCES_APPROVED"].ToString() == "Y") ? true : false,
                row["HUMAN_RESOURCES_APPROVE_DATE"].ToString(),
                row["HUMAN_RESOURCES_COMMENTS"].ToString(),
                row["FINANCE_DIRECTOR_APPROVAL"].ToString(),
                (row["FINANCE_DIRECTOR_APPROVED"].ToString() == "Y") ? true : false,
                row["FINANCE_DIRECTOR_APPROVE_DATE"].ToString(),
                row["FINANCE_DIRECTOR_COMMENTS"].ToString(),
                row["AGENCY_DIRECTOR_APPROVAL"].ToString(),
                (row["AGENCY_DIRECTOR_APPROVED"].ToString() == "Y") ? true : false,
                row["AGENCY_DIRECTOR_APPROVE_DATE"].ToString(),
                row["AGENCY_DIRECTOR_COMMENTS"].ToString()
                );
                approval.Add(reqItem);
            }
            return approval;
        }
        public static SFN54497Approval ConvertDataTableToApprovals(DataTable dtReq)
        {
            DataRow drReq = dtReq.Rows[0];
            SFN54497Approval req = new SFN54497Approval(
                int.Parse(drReq["ID_NUMBER"].ToString()),
                drReq["DEPARTMENT_DIRECTOR_APPROVAL"].ToString(),
                (drReq["DEPARTMENT_DIRECTOR_APPROVED"].ToString() == "Y") ? true : false,
                drReq["DEPARTMENT_DIRECTOR_APPROVE_DATE"].ToString(),
                drReq["DEPARTMENT_DIRECTOR_COMMENTS"].ToString(),
                drReq["DIVISION_CHIEF_APPROVAL"].ToString(),
                (drReq["DIVISION_CHIEF_APPROVED"].ToString() == "Y") ? true : false,
                drReq["DIVISION_CHIEF_APPROVE_DATE"].ToString(),
                drReq["DIVISION_CHIEF_COMMENTS"].ToString(),
                drReq["HUMAN_RESOURCES_APPROVAL"].ToString(),
                (drReq["HUMAN_RESOURCES_APPROVED"].ToString() == "Y") ? true : false,
                drReq["HUMAN_RESOURCES_APPROVE_DATE"].ToString(),
                drReq["HUMAN_RESOURCES_COMMENTS"].ToString(),
                drReq["FINANCE_DIRECTOR_APPROVAL"].ToString(),
                (drReq["FINANCE_DIRECTOR_APPROVED"].ToString() == "Y") ? true : false,
                drReq["FINANCE_DIRECTOR_APPROVE_DATE"].ToString(),
                drReq["FINANCE_DIRECTOR_COMMENTS"].ToString(),
                drReq["AGENCY_DIRECTOR_APPROVAL"].ToString(),
                (drReq["AGENCY_DIRECTOR_APPROVED"].ToString() == "Y") ? true : false,
                drReq["AGENCY_DIRECTOR_APPROVE_DATE"].ToString(),
                drReq["AGENCY_DIRECTOR_COMMENTS"].ToString()
            );
            return req;
        }
    }
}