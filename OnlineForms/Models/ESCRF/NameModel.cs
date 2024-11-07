using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity;

namespace OnlineForms.Models.ESCRF
{
    public class NameModel : IESCRF
    {

        [Key]
        public int ID { get; set; }

        [Required]
        [MaxLength(210)]
        [Display(Name = "Employee Name")]
        public string EmployeeName { get; set; }

        [Required]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Required]
        [MaxLength(100)]
        public string Department { get; set; }

        [Required]
        [MaxLength(100)]
        [Display(Name = "Office Location")]
        public string OfficeLocation { get; set; }

        [Required]
        [Display(Name = "Effective Date")]
        public DateTime EffectiveDate { get; set; }

        [Required]
        [MaxLength(100)]
        [Display(Name = "Current Supervisor")]
        public string CurrentSupervisor { get; set; }

        [Required]
        [MaxLength(50)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }


        [MaxLength(1)]
        [Display(Name = "Middle Initial")]
        public string MiddleInitial { get; set; }

        [MaxLength(1000)]
        public string Comments { get; set; }

        [Display(Name = "Modified By")]
        public string ModifiedBy { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Change Type")]
        public string ChangeType { get; set; }

        public int TaskListID { get; set; }

        public string Status { get; set; }

        public string Name()
        {
            return EmployeeName;
        }

        string IESCRF.ChangeType()
        {
            return ChangeType;
        }

        DateTime IESCRF.CreatedDate()
        {
            return CreatedDate;
        }

        string IESCRF.CurrentSupervisor()
        {
            return CurrentSupervisor;
        }

        string IESCRF.ID()
        {
            return ID.ToString();
        }

        string IESCRF.ModifiedBy()
        {
            return ModifiedBy;
        }

        string IESCRF.NewSupervisor()
        {
            return "N/A";
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
        public NameModel() { }

        public NameModel(int iD, string employeeName, string phoneNumber, string department, string officeLocation, DateTime effectiveDate, string currentSupervisor, string firstName, string middleInitial, string lastName, string comments, string modifiedBy, DateTime createdDate, string changeType, int taskListID)
        {
            ID = iD;
            EmployeeName = employeeName;
            PhoneNumber = phoneNumber;
            Department = department;
            OfficeLocation = officeLocation;
            EffectiveDate = effectiveDate;
            CurrentSupervisor = currentSupervisor;
            FirstName = firstName;
            MiddleInitial = middleInitial;
            LastName = lastName;
            Comments = comments;
            ModifiedBy = modifiedBy;
            CreatedDate = createdDate;
            ChangeType = changeType;
            TaskListID = taskListID;
        }

        public static NameModel ConvertDataTableToName(DataTable dtName)
        {
            DataRow drName = dtName.Rows[0];
            NameModel name = new NameModel(
                 int.Parse(drName["ID_NUMBER"].ToString()),
                drName["EMPLOYEE_NAME"].ToString(),
                drName["PHONE_NUMBER"].ToString(),
                drName["DEPARTMENT"].ToString(),
                drName["OFFICE_LOCATION"].ToString(),
                DateTime.Parse(drName["EFFECTIVE_DATE"].ToString()),
                drName["CURRENT_SUPERVISOR"].ToString(),
                drName["FIRST_NAME"].ToString(),
                drName["MIDDLE_INITIAL"].ToString(),
                drName["LAST_NAME"].ToString(),
                drName["COMMENTS"].ToString(),
                drName["MODIFIED_BY"].ToString(),
                DateTime.Parse(drName["CREATED_DATE"].ToString()),
                drName["CHANGE_TYPE"].ToString(),
                int.Parse(drName["TASK_LIST_ID"].ToString())
                );
            return name;
        }

        public static List<IESCRF> GetNameList(DataTable dtName)
        {
            List<IESCRF> IESCRFList = new List<IESCRF>();
            foreach (DataRow nameRow in dtName.Rows)
            {
                NameModel name = new NameModel(
                int.Parse(nameRow["ID_NUMBER"].ToString()),
                nameRow["EMPLOYEE_NAME"].ToString(),
                nameRow["PHONE_NUMBER"].ToString(),
                nameRow["DEPARTMENT"].ToString(),
                nameRow["OFFICE_LOCATION"].ToString(),
                DateTime.Parse(nameRow["EFFECTIVE_DATE"].ToString()),
                nameRow["CURRENT_SUPERVISOR"].ToString(),
                 nameRow["FIRST_NAME"].ToString(),
                 nameRow["MIDDLE_INITIAL"].ToString(),
                 nameRow["LAST_NAME"].ToString(),
                 nameRow["COMMENTS"].ToString(),
                 nameRow["MODIFIED_BY"].ToString(),
                 DateTime.Parse(nameRow["CREATED_DATE"].ToString()),
                 nameRow["CHANGE_TYPE"].ToString(),
                 int.Parse(nameRow["TASK_LIST_ID"].ToString())
                 );
                IESCRFList.Add(name);
            }
            return IESCRFList;
        }
    }
    public class NameDBContext : DbContext
    {
        public DbSet<NameModel> Names { get; set; }
    }
}