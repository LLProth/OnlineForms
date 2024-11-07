using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity;

namespace OnlineForms.Models.ESCRF
{
    public class TerminationModel : IESCRF
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [MaxLength(207)]
        [Display(Name = "Employee Name")]
        public string EmployeeName { get; set; }
        [Required]
        [Display(Name = "Effective Date")]
        public DateTime EffectiveDate { get; set; }

        [Required]
        [MaxLength(207)]
        [Display(Name = "Current Supervisor")]
        public string CurrentSupervisor { get; set; }

        [Required]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Required]
        [Display(Name = "Last Date Worked")]
        public DateTime LastDateWorked { get; set; }

        [Required]
        [MaxLength(100)]
        public string Department { get; set; }

        [Required]
        [MaxLength(100)]
        [Display(Name = "Office Location")]
        public string OfficeLocation { get; set; }

        [Required]
        [Display(Name = "Transferring to Another State Agency")]
        public bool TransferringToAgency { get; set; }

        [Display(Name = "Modified By")]
        public string ModifiedBy { get; set; }

        [Display(Name = "Change Type")]
        public string ChangeType { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        public int TaskListID { get; set; }

        public string Status { get; set; }

        [MaxLength(2000)]
        public string Comments { get; set; }


        public string Name()
        {
            return EmployeeName;
        }

        public string NewSupervisor()
        {
            return "N/A";
        }

        DateTime IESCRF.CreatedDate()
        {
            return CreatedDate;
        }

        string IESCRF.CurrentSupervisor()
        {
            return CurrentSupervisor;
        }

        string IESCRF.ChangeType()
        {
            return ChangeType;
        }

        string IESCRF.ID()
        {
            return ID.ToString();
        }

        string IESCRF.ModifiedBy()
        {
            return ModifiedBy;
        }

        int IESCRF.TaskListID()
        {
            return TaskListID;
        }

        string IESCRF.Status()
        {
            return Status;
        }

        void IESCRF.CreateStatus()
        {
            this.Status = "Not Started";
            if (this.TaskListID == 0)
            {
                this.Status = "Not Started";
            }
            else if (this.TaskListID != 0)
            {
                ESCRFModelDal dal = new ESCRFModelDal();
                System.Data.DataTable dtTasklist = dal.GetTaskListInfoByID(this.TaskListID);
                OnlineForms.Models.ESCRF.TaskListModel taskList = OnlineForms.Models.ESCRF.TaskListModel.ConvertDataTableToTaskList(dtTasklist);
                if (taskList.FinishedDate != DateTime.MinValue)
                {
                    this.Status = "Completed";
                }
                else if (taskList.IsDeployed == false)
                {
                    this.Status = "Not Started";
                }
                else
                {
                    this.Status = "In Progress";
                }
            }

        }

        public TerminationModel() { }

        public TerminationModel(int id, string employeeName, DateTime effectiveDate, string currentSupervisor, string phoneNumber, DateTime lastDateWorked, string department, string officeLocation,
          bool transferringToAgency, string modifiedBy, string changeType, DateTime createdDate, int taskListID, string comments)

        {
            ID = id;
            EmployeeName = employeeName;
            EffectiveDate = effectiveDate;
            CurrentSupervisor = currentSupervisor;
            PhoneNumber = phoneNumber;
            Department = department;
            OfficeLocation = officeLocation;
            TransferringToAgency = transferringToAgency;
            ModifiedBy = modifiedBy;
            ChangeType = changeType;
            CreatedDate = createdDate;
            TaskListID = taskListID;
            LastDateWorked = lastDateWorked;
            Comments = comments;
        }

        public static TerminationModel ConvertDataTableToTermination(DataTable dtTerm)
        {
            DataRow drTerm = dtTerm.Rows[0];
            TerminationModel termination = new TerminationModel(
                int.Parse(drTerm["ID_NUMBER"].ToString()),
                drTerm["EMPLOYEE_NAME"].ToString(),
                DateTime.Parse(drTerm["EFFECTIVE_DATE"].ToString()),
                drTerm["CURRENT_SUPERVISOR"].ToString(),
                drTerm["PHONE_NUMBER"].ToString(),
                DateTime.Parse(drTerm["LAST_DAY_WORKED"].ToString()),
                drTerm["DEPARTMENT"].ToString(),
                drTerm["OFFICE_LOCATION"].ToString(),
                (drTerm["TRANSFERRING_TO_AGENCY"].ToString() == "Y") ? true : false,
                drTerm["MODIFIED_BY"].ToString(),
                drTerm["CHANGE_TYPE"].ToString(),
                DateTime.Parse(drTerm["CREATED_DATE"].ToString()),
                int.Parse(drTerm["TASK_LIST_ID"].ToString()),
                drTerm["COMMENTS"].ToString()
                );

            return termination;
        }

        public static List<IESCRF> GetTerminationList(DataTable TermList)
        {
            List<IESCRF> IESCRFLIST = new List<IESCRF>();
            foreach (DataRow TermRow in TermList.Rows)
            {
                TerminationModel termination = new TerminationModel(
                    int.Parse(TermRow["ID_NUMBER"].ToString()),
                    TermRow["EMPLOYEE_NAME"].ToString(),
                    DateTime.Parse(TermRow["EFFECTIVE_DATE"].ToString()),
                    TermRow["CURRENT_SUPERVISOR"].ToString(),
                    TermRow["PHONE_NUMBER"].ToString(),
                    DateTime.Parse(TermRow["LAST_DAY_WORKED"].ToString()),
                    TermRow["DEPARTMENT"].ToString(),
                    TermRow["OFFICE_LOCATION"].ToString(),
                    (TermRow["TRANSFERRING_TO_AGENCY"].ToString() == "Y") ? true : false,
                    TermRow["MODIFIED_BY"].ToString(),
                    TermRow["CHANGE_TYPE"].ToString(),
                    DateTime.Parse(TermRow["CREATED_DATE"].ToString()),
                    int.Parse(TermRow["TASK_LIST_ID"].ToString()),
                    TermRow["COMMENTS"].ToString()
                    );
                IESCRFLIST.Add(termination);
            }
            return IESCRFLIST;
        }
    }
    public class TerminationDBContext : DbContext
    {
        public DbSet<TerminationModel> Terminations { get; set; }
    }
};