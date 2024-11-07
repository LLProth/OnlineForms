using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using OnlineForms.Helper;
using System.Web.Mvc;

namespace OnlineForms.Models.SFN60026
{
    public class SFN60026Model : IEquatable<SFN60026Model>, IComparable<SFN60026Model>
    {
        [Key]
        [Display(Name = "ID Number (to be assigned)")]
        public int ID { get; set; }

        [Display(Name = "Nominee's Name")]
        public string NomineeName { get; set; }

        [Display(Name = "Position (to be filled by system)")]
        public string NomineePosition { get; set; }

        [Display(Name = "Department (to be filled by system)")]
        public string NomineeDepartment { get; set; }

        [Display(Name = "Start Date")]
        public DateTime AccomplishmentStartDate { get; set; }

        [Display(Name = "End Date")]
        public DateTime AccomplishmentEndDate { get; set; }

        [Display(Name = "Justification for Employee's Performance Bonus:")]
        public string Justification { get; set; }

        [Display(Name = "Has the nominee held a position in State Government for at least one year?")]
        public bool StateEmployeeOneYear { get; set; }

        [Display(Name = "Probationary Employee")]
        public bool ProbationaryEmployee { get; set; }

        [Display(Name = "Full Time")]
        public bool FullTime { get; set; }

        [Display(Name = "Part Time")]
        public bool PartTime { get; set; }

        [Display(Name = "Temporary")]
        public bool Temporary { get; set; }

        [Display(Name = "Date of Last Performance Bonus (if applicable)")]
        public DateTime? LastBonusDate { get; set; }

        [Display(Name = "Last Bonus Amount")]
        public string LastBonusAmount { get; set; }

        [Display(Name = "Score On Last Performance Appraisal")]
        public string LastPerformanceScore { get; set; }

        [Display(Name = "Pending or Disciplinary Action")]
        public bool HRAction { get; set; }

        [Display(Name = "Nominee meets the Eligibility Requirements")]
        public bool MeetsRequirements { get; set; }

        [Display(Name = "Committee Approval")]
        public Approval CommitteeApproval { get; set; }

        [Display(Name = "Amount Approved (not to exceed $1,500)")]
        public string CommitteeApprovalAmount { get; set; }

        [Display(Name = "Comments")]
        public string Comments { get; set; }

        [Display(Name = "Submitted By")]
        public string SubmitterName { get; set; }

        [Display(Name = "Submitter Position")]
        public string SubmitterPosition { get; set; }

        [Display(Name = "Date Submitted")]
        public DateTime SubmitterDate { get; set; }

        [Display(Name = "Supervisor Signature")]
        public string SupervisorSignature { get; set; }

        [Display(Name = "Supervisor Signature Date")]
        public DateTime SupervisorSignatureDate { get; set; }

        [Display(Name = "Supervisor Approval")]
        public bool SupervisorEndorsement { get; set; }

        [Display(Name = "Department Director Signature")]
        public string DepartmentSignature { get; set; }

        [Display(Name = "Department Director Approval")]
        public bool DepartmentEndorsement { get; set; }

        [Display(Name = "Department Director Signature Date")]
        public DateTime DepartmentSignatureDate { get; set; }

        [Display(Name = "Division Chief Review")]
        public string ChiefSignature { get; set; }

        [Display(Name = "Division Chief Review Date")]
        public DateTime ChiefSignatureDate { get; set; }

        [Display(Name = "Human Resources Representative")]
        public string HRRepresentative { get; set; }

        [Display(Name = "Human Resources Representative Date")]
        public DateTime HRRepresentativeDate { get; set; }

        [Display(Name = "Committee Approval List")]
        public string CommitteeApprovalList { get; set; }

        [Display(Name = "Committee Approval Date")]
        public DateTime CommitteeApprovalDate { get; set; }

        [Display(Name = "Division Chief Endorsement")]
        public string ChiefEndorsement { get; set; }

        [Display(Name = "Division Chief Endorsement Date")]
        public DateTime ChiefEndorsementDate { get; set; }

        [Display(Name = "Agency Director Endorsement")]
        public string AgencyDirectorEndorsement { get; set; }

        [Display(Name = "Agency Director Endorsement Date")]
        public DateTime AgencyDirectorEndorsementDate { get; set; }

        [Display(Name = "Form is Submitted")]
        public bool FormSubmitted { get; set; }

        [Display(Name = "Form is Completed")]
        public bool FormCompleted { get; set; }

        [Display(Name = "Form is Denied")]
        public bool FormDenied { get; set; }

        public string FormName { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public int CurrentStatus { get; set; }

        //These are used to show highest status that user has access to on this record
        //   and determine if they have completed that step.
        public int DisplayStatus { get; set; }
        public bool StepCompleted { get; set; }
        public enum Approval
        {
            Yes,
            No,
            Blank
        };


        public string[] Stages = new string[] { 
            "Not Submitted", //CurrentStatus = 0
            "Submitted",  //CurrentStatus = 1
            "Supervisor Approved",  //CurrentStatus = 2
            "Director Approved",  //CurrentStatus = 3
            "Chief Approved",  //CurrentStatus = 4
            "HR Approved",  //CurrentStatus = 5
            "Committee Approved",  //CurrentStatus = 6
            "Chief Endorseed",  //CurrentStatus = 7
            "Agency Director Approved",  //CurrentStatus = 8
            "Complete"  //CurrentStatus = 9
        };
        public override string ToString()
        {
            return base.ToString();
        }
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            SFN60026Model objAsModel = obj as SFN60026Model;
            if (objAsModel == null) return false;
            else return base.Equals(objAsModel);
        }
        // Default comparer for Part type.
        public int CompareTo(SFN60026Model comparePart)
        {
            // A null value means that this object is greater.
            if (comparePart == null)
                return 1;
            else
            {
                if (this.DisplayStatus.CompareTo(comparePart.ID) == 0)
                {
                    return this.ID.CompareTo(comparePart.ID);
                }
                else
                {
                    return this.DisplayStatus.CompareTo(comparePart.ID);
                }
            }
        }
        public override int GetHashCode()
        {
            return ID;
        }
        public bool Equals(SFN60026Model other)
        {
            if (other == null) return false;
            return (this.ID.Equals(other.ID));
        }

        public SFN60026Model(int id, string name, string position, string department, DateTime stDate, DateTime endDate, string justification, bool stateEmployeeOneYr,
                bool probation, bool fullTime, bool partTime, bool temporary, DateTime lastDate, string lastBonusAmt, string score, bool pendOrDiscipline, bool meetsReqs, Approval approval, string approvalAmt, string comments,
                string submitter, string subPosition, DateTime submittedDate, bool supEndorsement, string supSignature, DateTime supSigDate, bool dptDirEndorsement, string dptDirSignature, DateTime dptDirSigDate,
                string chiefSignature, DateTime chiefSigDate, string HRSignature, DateTime HRSigDate, string ComAppList, DateTime ComAppDate, string chiefEndorsement, DateTime chiefEndorsementDate, string agencyDirEndorsement, DateTime agencyDirEndorsementDate,
                bool formSubmitted, bool formCompleted, bool formDenied, DateTime modifiedDate, string modifiedBy, int currStatus)
        {
            ID = id;
            NomineeName = name;
            NomineePosition = position;
            NomineeDepartment = department;
            AccomplishmentStartDate = stDate;
            AccomplishmentEndDate = endDate;
            Justification = justification;
            StateEmployeeOneYear = stateEmployeeOneYr;
            ProbationaryEmployee = probation;
            FullTime = fullTime;
            PartTime = partTime;
            Temporary = temporary;
            LastBonusDate = lastDate;
            LastBonusAmount = lastBonusAmt;
            LastPerformanceScore = score;
            HRAction = pendOrDiscipline;
            MeetsRequirements = meetsReqs;
            CommitteeApproval = approval;
            CommitteeApprovalAmount = approvalAmt;
            Comments = comments;
            SubmitterName = submitter;
            SubmitterPosition = subPosition;
            SubmitterDate = submittedDate;
            SupervisorSignature = supSignature;
            SupervisorSignatureDate = supSigDate;
            SupervisorEndorsement = supEndorsement;
            DepartmentEndorsement = dptDirEndorsement;
            DepartmentSignature = dptDirSignature;
            DepartmentSignatureDate = dptDirSigDate;
            ChiefSignature = chiefSignature;
            ChiefSignatureDate = chiefSigDate;
            HRRepresentative = HRSignature;
            HRRepresentativeDate = HRSigDate;
            CommitteeApprovalList = ComAppList;
            CommitteeApprovalDate = ComAppDate;
            ChiefEndorsement = chiefEndorsement;
            ChiefEndorsementDate = chiefEndorsementDate;
            AgencyDirectorEndorsement = agencyDirEndorsement;
            AgencyDirectorEndorsementDate = agencyDirEndorsementDate;
            FormSubmitted = formSubmitted;
            FormCompleted = formCompleted;
            FormDenied = formDenied;
            ModifiedBy = modifiedBy;
            ModifiedDate = modifiedDate;
            CurrentStatus = currStatus;
        }
        public SFN60026Model()
        {
            ID = -1;
            NomineeName = "";
            NomineePosition = "";
            NomineeDepartment = "";
            AccomplishmentStartDate = DateTime.Today;
            AccomplishmentEndDate = DateTime.Today;
            Justification = "";
            //StateEmployeeOneYear = false;
            //ProbationaryEmployee = false;
            //FullTime = false;
            //PartTime = false;
            //Temporary = false;
            //LastBonusDate = DateTime.Parse("1/1/2000");
            //LastBonusAmount = "";
            //LastPerformanceScore = "";
            //HRAction = false;
            //MeetsRequirements = false;
            //CommitteeApproval = false;
            //CommitteeApprovalAmount = "";
            //Comments = "";
            //SubmitterName = "";
            //SubmitterPosition = "";
            //SubmitterDate = DateTime.Parse("1/1/2000");
            //SupervisorSignature = "";
            //SupervisorSignatureDate = DateTime.Parse("1/1/2000");
            //SupervisorEndorsement = false;
            //DepartmentEndorsement = false;
            //DepartmentSignature = "";
            //DepartmentSignatureDate = DateTime.Parse("1/1/2000");
            //ChiefSignature = "";
            //ChiefSignatureDate = DateTime.Parse("1/1/2000");
            //HRRepresentative = "";
            //HRRepresentativeDate = DateTime.Parse("1/1/2000");
            //CommitteeApprovalList = "";
            //CommitteeApprovalDate = DateTime.Parse("1/1/2000");
            //ChiefEndorsement = "";
            //ChiefEndorsementDate = DateTime.Parse("1/1/2000");
            //AgencyDirectorEndorsement = "";
            //AgencyDirectorEndorsementDate = DateTime.Parse("1/1/2000");
            //FormSubmitted = false;
            //FormCompleted = false;
            //FormDenied = false;
            //ModifiedBy = "";
            //CurrentStatus = 0;
        }
        public static SFN60026Model ConvertDataTableToSFN60026(DataTable dtReq)
        {
            DataRow drReq = dtReq.Rows[0];
            SFN60026Model model = new SFN60026Model(
                int.Parse(drReq["ID_NUMBER"].ToString()),
                drReq["NOMINEE_NAME"].ToString(),
                drReq["NOMINEE_POSITION"].ToString(),
                drReq["NOMINEE_DEPARTMENT"].ToString(),
                (drReq["JUSTIFICATION_START_DATE"].ToString() == "") ? DateTime.Parse("1/1/2000") : DateTime.Parse(drReq["JUSTIFICATION_START_DATE"].ToString()),
                (drReq["JUSTIFICATION_END_DATE"].ToString() == "") ? DateTime.Parse("1/1/2000") : DateTime.Parse(drReq["JUSTIFICATION_END_DATE"].ToString()),
                drReq["JUSTIFICATION_DESCRIPTION"].ToString(),
                (drReq["ELIGIBILITY_POS_YR"].ToString() == "Y"),
                (drReq["ELIGIBILITY_PROBATIONARY"].ToString() == "Y"),
                (drReq["ELIGIBILITY_FULLTIME"].ToString() == "Y"),
                (drReq["ELIGIBILITY_PARTTIME"].ToString() == "Y"),
                (drReq["ELIGIBILITY_TEMPORARY"].ToString() == "Y"),
                (drReq["ELIGIBILITY_LAST_BONUS_DATE"].ToString() == "") ? DateTime.Parse("1/1/2000") : DateTime.Parse(drReq["ELIGIBILITY_LAST_BONUS_DATE"].ToString()),
                drReq["ELIGIBILITY_LAST_BONUS_AMOUNT"].ToString(),
                drReq["ELIGIBILITY_LAST_APPRAISAL_SCORE"].ToString(),
                (drReq["ELIGIBILITY_PENDING_DISCIPLINE"].ToString() == "Y"),
                (drReq["ELIGIBILITY_MEETS_REQS"].ToString() == "Y"),
                (drReq["ELIGIBILITY_COMMITTEE_APPROVAL"].ToString() == "Y")? Approval.Yes: (drReq["ELIGIBILITY_COMMITTEE_APPROVAL"].ToString() == "") ? Approval.Blank: Approval.No,
                drReq["ELIGIBILITY_APPROVAL_AMOUNT"].ToString(),
                drReq["ELIGIBILITY_DISAPPROVE_COMMENTS"].ToString(),
                drReq["SUBMIT_NAME"].ToString(),
                drReq["SUBMIT_POSITION"].ToString(),
                (drReq["SUBMIT_DATE"].ToString() == "") ? DateTime.Parse("1/1/2000") : DateTime.Parse(drReq["SUBMIT_DATE"].ToString()),
                (drReq["SUPERVISOR_ENDORSEMENT"].ToString() == "Y"),
                drReq["SUPERVISOR_SIGNATURE"].ToString(),
                (drReq["SUPERVISOR_SIGNATURE_DATE"].ToString() == "") ? DateTime.Parse("1/1/2000") : DateTime.Parse(drReq["SUPERVISOR_SIGNATURE_DATE"].ToString()),
                (drReq["DIRECTOR_ENDORSEMENT"].ToString() == "Y"),
                drReq["DIRECTOR_SIGNATURE"].ToString(),
                (drReq["DIRECTOR_SIGNATURE_DATE"].ToString() == "") ? DateTime.Parse("1/1/2000") : DateTime.Parse(drReq["DIRECTOR_SIGNATURE_DATE"].ToString()),
                drReq["CHIEF_SIGNATURE"].ToString(),
                (drReq["CHIEF_SIGNATURE_DATE"].ToString() == "") ? DateTime.Parse("1/1/2000") : DateTime.Parse(drReq["CHIEF_SIGNATURE_DATE"].ToString()),
                drReq["HR_SIGNATURE"].ToString(),
                (drReq["HR_SIGNATURE_DATE"].ToString() == "") ? DateTime.Parse("1/1/2000") : DateTime.Parse(drReq["HR_SIGNATURE_DATE"].ToString()),
                drReq["COMMITTEE_APPROVAL_LIST"].ToString(),
                (drReq["COMMITTEE_APPROVAL_DATE"].ToString() == "") ? DateTime.Parse("1/1/2000") : DateTime.Parse(drReq["COMMITTEE_APPROVAL_DATE"].ToString()),
                drReq["CHIEF_ENDORSEMENT"].ToString(),
                (drReq["CHIEF_ENDORSEMENT_DATE"].ToString() == "") ? DateTime.Parse("1/1/2000") : DateTime.Parse(drReq["CHIEF_ENDORSEMENT_DATE"].ToString()),
                drReq["AGENCY_DIRECTOR_ENDORSEMENT"].ToString(),
                (drReq["AGENCY_DIRECTOR_ENDORSEMENT_DATE"].ToString() == "") ? DateTime.Parse("1/1/2000") : DateTime.Parse(drReq["AGENCY_DIRECTOR_ENDORSEMENT_DATE"].ToString()),
                (drReq["FORM_SUBMITTED"].ToString() == "Y"),
                (drReq["FORM_COMPLETED"].ToString() == "Y"),
                (drReq["FORM_DENIED"].ToString() == "Y"),
                (drReq["MODIFIED_DATE"].ToString() == "") ? DateTime.Parse("1/1/2000") : DateTime.Parse(drReq["MODIFIED_DATE"].ToString()),
                drReq["MODIFIED_BY"].ToString(),
                int.Parse(drReq["CURRENT_STATUS"].ToString())
                );

            return model;
        }
        public static List<SFN60026Model> GetListSFN60026(DataTable dtReq, string user = "")
        {
            List<SFN60026Model> list = new List<SFN60026Model>();
            foreach (DataRow drReq in dtReq.Rows)
            {
                SFN60026Model model = new SFN60026Model(
                    int.Parse(drReq["ID_NUMBER"].ToString()),
                    drReq["NOMINEE_NAME"].ToString(),
                    drReq["NOMINEE_POSITION"].ToString(),
                    drReq["NOMINEE_DEPARTMENT"].ToString(),
                    (drReq["JUSTIFICATION_START_DATE"].ToString() == "") ? DateTime.Parse("1/1/2000") : DateTime.Parse(drReq["JUSTIFICATION_START_DATE"].ToString()),
                    (drReq["JUSTIFICATION_END_DATE"].ToString() == "") ? DateTime.Parse("1/1/2000") : DateTime.Parse(drReq["JUSTIFICATION_END_DATE"].ToString()),
                    drReq["JUSTIFICATION_DESCRIPTION"].ToString(),
                    (drReq["ELIGIBILITY_POS_YR"].ToString() == "Y"),
                    (drReq["ELIGIBILITY_PROBATIONARY"].ToString() == "Y"),
                    (drReq["ELIGIBILITY_FULLTIME"].ToString() == "Y"),
                    (drReq["ELIGIBILITY_PARTTIME"].ToString() == "Y"),
                    (drReq["ELIGIBILITY_TEMPORARY"].ToString() == "Y"),
                    (drReq["ELIGIBILITY_LAST_BONUS_DATE"].ToString() == "") ? DateTime.Parse("1/1/2000") : DateTime.Parse(drReq["ELIGIBILITY_LAST_BONUS_DATE"].ToString()),
                    drReq["ELIGIBILITY_LAST_BONUS_AMOUNT"].ToString(),
                    drReq["ELIGIBILITY_LAST_APPRAISAL_SCORE"].ToString(),
                    (drReq["ELIGIBILITY_PENDING_DISCIPLINE"].ToString() == "Y"),
                    (drReq["ELIGIBILITY_MEETS_REQS"].ToString() == "Y"),
                    (drReq["ELIGIBILITY_COMMITTEE_APPROVAL"].ToString() == "Y") ? Approval.Yes : (drReq["ELIGIBILITY_COMMITTEE_APPROVAL"].ToString() == "") ? Approval.Blank : Approval.No,
                    drReq["ELIGIBILITY_APPROVAL_AMOUNT"].ToString(),
                    drReq["ELIGIBILITY_DISAPPROVE_COMMENTS"].ToString(),
                    drReq["SUBMIT_NAME"].ToString(),
                    drReq["SUBMIT_POSITION"].ToString(),
                    (drReq["SUBMIT_DATE"].ToString() == "") ? DateTime.Parse("1/1/2000") : DateTime.Parse(drReq["SUBMIT_DATE"].ToString()),
                    (drReq["SUPERVISOR_ENDORSEMENT"].ToString() == "Y"),
                    drReq["SUPERVISOR_SIGNATURE"].ToString(),
                    (drReq["SUPERVISOR_SIGNATURE_DATE"].ToString() == "") ? DateTime.Parse("1/1/2000") : DateTime.Parse(drReq["SUPERVISOR_SIGNATURE_DATE"].ToString()),
                    (drReq["DIRECTOR_ENDORSEMENT"].ToString() == "Y"),
                    drReq["DIRECTOR_SIGNATURE"].ToString(),
                    (drReq["DIRECTOR_SIGNATURE_DATE"].ToString() == "") ? DateTime.Parse("1/1/2000") : DateTime.Parse(drReq["DIRECTOR_SIGNATURE_DATE"].ToString()),
                    drReq["CHIEF_SIGNATURE"].ToString(),
                    (drReq["CHIEF_SIGNATURE_DATE"].ToString() == "") ? DateTime.Parse("1/1/2000") : DateTime.Parse(drReq["CHIEF_SIGNATURE_DATE"].ToString()),
                    drReq["HR_SIGNATURE"].ToString(),
                    (drReq["HR_SIGNATURE_DATE"].ToString() == "") ? DateTime.Parse("1/1/2000") : DateTime.Parse(drReq["HR_SIGNATURE_DATE"].ToString()),
                    drReq["COMMITTEE_APPROVAL_LIST"].ToString(),
                    (drReq["COMMITTEE_APPROVAL_DATE"].ToString() == "") ? DateTime.Parse("1/1/2000") : DateTime.Parse(drReq["COMMITTEE_APPROVAL_DATE"].ToString()),
                    drReq["CHIEF_ENDORSEMENT"].ToString(),
                    (drReq["CHIEF_ENDORSEMENT_DATE"].ToString() == "") ? DateTime.Parse("1/1/2000") : DateTime.Parse(drReq["CHIEF_ENDORSEMENT_DATE"].ToString()),
                    drReq["AGENCY_DIRECTOR_ENDORSEMENT"].ToString(),
                    (drReq["AGENCY_DIRECTOR_ENDORSEMENT_DATE"].ToString() == "") ? DateTime.Parse("1/1/2000") : DateTime.Parse(drReq["AGENCY_DIRECTOR_ENDORSEMENT_DATE"].ToString()),
                    (drReq["FORM_SUBMITTED"].ToString() == "Y"),
                    (drReq["FORM_COMPLETED"].ToString() == "Y"),
                    (drReq["FORM_DENIED"].ToString() == "Y"),                
                    (drReq["MODIFIED_DATE"].ToString() == "") ? DateTime.Parse("1/1/2000") : DateTime.Parse(drReq["MODIFIED_DATE"].ToString()),
                    drReq["MODIFIED_BY"].ToString(),
                    int.Parse(drReq["CURRENT_STATUS"].ToString())
                    );
                model.determineDisplayStatus(user); 
                list.Add(model);
            }
            //System.Comparison<SFN60026Model> comp = (SFN60026Model one, SFN60026Model other) => {//Sort sorts from lowest to highest
            //    if (one.SubmitterDate > other.SubmitterDate)
            //    {
            //        return 1;
            //    }
            //    else if (one.SubmitterDate < other.SubmitterDate)
            //    {
            //        return -1;
            //    }
            //    else
            //    {
            //        return string.Compare(one.NomineeName, other.NomineeName);
            //    }
            //}; 
            //list.Sort(comp);
            return list;
        }
        private void determineDisplayStatus(string user)
        {
            //DisplayStatus is the highest status in which the current user is mentioned.
            //StepCompleted is the flag to determine if the DisplayStatus step is completed or not
            //These two fields are used in the INDEX view when the user is not a SuperUser.

            // Set Display status to Current Status
            DisplayStatus = CurrentStatus;
            StepCompleted = false;
            // View for a regular user
            if(user.Length > 0)
            {
                // Start at the Current Status and work backwords
                bool foundHighest = false;
                for(++DisplayStatus; DisplayStatus >= 0; --DisplayStatus)
                {
                    switch(DisplayStatus)
                    {
                        case 8:
                            if (AgencyDirectorEndorsement.Contains(user))
                            {
                                foundHighest = true;
                                StepCompleted = (AgencyDirectorEndorsementDate.CompareTo(DateTime.Parse("1/1/2000")) > 0);
                            }
                            break;
                        case 7:
                            if (ChiefEndorsement.Contains(user))
                            {
                                foundHighest = true;
                                StepCompleted = (ChiefEndorsementDate.CompareTo(DateTime.Parse("1/1/2000")) > 0);
                            }
                            break;
                        case 6:
                            if (CommitteeApprovalList.Contains(user))
                            {
                                foundHighest = true;
                                StepCompleted = (CommitteeApprovalDate.CompareTo(DateTime.Parse("1/1/2000")) > 0);
                            }
                            break;
                        case 5:
                            if (HRRepresentative.Contains(user))
                            {
                                foundHighest = true;
                                StepCompleted = (HRRepresentativeDate.CompareTo(DateTime.Parse("1/1/2000")) > 0);
                            }
                            break;
                        case 4:
                            if (ChiefSignature.Contains(user))
                            {
                                foundHighest = true;
                                StepCompleted = (ChiefSignatureDate.CompareTo(DateTime.Parse("1/1/2000")) > 0);
                            }
                            break;
                        case 3:
                            if (DepartmentSignature.Contains(user))
                            {
                                foundHighest = true;
                                StepCompleted = (DepartmentSignatureDate.CompareTo(DateTime.Parse("1/1/2000")) > 0);
                            }
                            break;
                        case 2:
                            if (SupervisorSignature.Contains(user))
                            {
                                foundHighest = true;
                                StepCompleted = (SupervisorSignatureDate.CompareTo(DateTime.Parse("1/1/2000")) > 0);
                            }
                            break;
                        case 1:
                            if (SubmitterName.Contains(user) && FormSubmitted)
                            {
                                foundHighest = true;
                                StepCompleted = (SubmitterDate.CompareTo(DateTime.Parse("1/1/2000")) > 0);
                            }
                            break;
                        case 0:
                            if (SubmitterName.Contains(user))
                            {
                                foundHighest = true;
                                StepCompleted = false;
                            }
                            break;
                    }
                    if(foundHighest)
                    {
                        if (FormDenied)
                            StepCompleted = true;
                        DisplayStatus = (DisplayStatus > CurrentStatus) ? CurrentStatus : DisplayStatus;
                        break;
                    }
                }
            }
        }

        private string pullValue(object s)
        {
            return s == null ? "" : s.ToString();
        }
    }
}