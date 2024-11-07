using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity;

namespace OnlineForms.Models.ESCRF
{
    public class ChangeModel : IESCRF
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
        [Display(Name = "New Position")]
        public bool IsNewPosition { get; set; }

        [MaxLength(100)]
        [Display(Name = "Position Name")]
        public string PositionName { get; set; }

        [Required]
        [Display(Name = "New Supervisor")]
        public bool IsNewSupervisor { get; set; }

        [MaxLength(100)]
        [Display(Name = "New Supervisor")]
        public string NewSupervisor { get; set; }

        [Required]
        [Display(Name = "Temp to FTE")]
        public bool IsTempToFte { get; set; }

        [Required]
        [MaxLength(25)]
        [Display(Name = "FLSA Status")]
        public string FLSAStatus { get; set; }


        [MaxLength(2000)]
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
            return NewSupervisor;
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

        public ChangeModel() { }

        public ChangeModel(int id, string employeeName, string phoneNumber, string department, string officeLocation, DateTime effectiveDate, string currentSupervisor, bool isNewPosition, string positionName, bool isNewSupervisor, string newSupervisor, bool isTempToFTE, string fLSAStatus, string comments, string modifiedBy, DateTime createdDate, string changeType, int taskListID)
        {
            ID = id;
            EmployeeName = employeeName;
            PhoneNumber = phoneNumber;
            Department = department;
            OfficeLocation = officeLocation;
            EffectiveDate = effectiveDate;
            CurrentSupervisor = currentSupervisor;
            IsNewPosition = isNewPosition;
            PositionName = positionName;
            IsNewSupervisor = isNewSupervisor;
            NewSupervisor = newSupervisor;
            IsTempToFte = isTempToFTE;
            FLSAStatus = fLSAStatus;
            Comments = comments;
            ModifiedBy = modifiedBy;
            CreatedDate = createdDate;
            ChangeType = changeType;
            TaskListID = taskListID;
        }

        public static ChangeModel ConvertDataTableToChange(DataTable dtChange)
        {
            DataRow drChange = dtChange.Rows[0];
            ChangeModel change = new ChangeModel(
                int.Parse(drChange["ID_NUMBER"].ToString()),
                drChange["EMPLOYEE_NAME"].ToString(),
                drChange["PHONE_NUMBER"].ToString(),
                drChange["DEPARTMENT"].ToString(),
                drChange["OFFICE_LOCATION"].ToString(),
                DateTime.Parse(drChange["EFFECTIVE_DATE"].ToString()),
                drChange["CURRENT_SUPERVISOR"].ToString(),
                (drChange["IS_NEW_POSITION"].ToString() == "Y") ? true : false,
                drChange["POSITION_NAME"].ToString(),
                (drChange["IS_NEW_SUPERVISOR"].ToString() == "Y") ? true : false,
                drChange["NEW_SUPERVISOR"].ToString(),
                (drChange["TEMP_TO_FTE"].ToString() == "Y") ? true : false,
                drChange["FLSA_STATUS"].ToString(),
                drChange["COMMENTS"].ToString(),
                drChange["MODIFIED_BY"].ToString(),
                DateTime.Parse(drChange["CREATED_DATE"].ToString()),
                drChange["CHANGE_TYPE"].ToString(),
                int.Parse(drChange["TASK_LIST_ID"].ToString())
                );

            return change;
        }

        public static List<IESCRF> GetChangeList(DataTable dtChange)
        {
            List<IESCRF> IESCRFList = new List<IESCRF>();
            foreach (DataRow changeRow in dtChange.Rows)
            {
                ChangeModel change = new ChangeModel(
               int.Parse(changeRow["ID_NUMBER"].ToString()),
               changeRow["EMPLOYEE_NAME"].ToString(),
               changeRow["PHONE_NUMBER"].ToString(),
               changeRow["DEPARTMENT"].ToString(),
               changeRow["OFFICE_LOCATION"].ToString(),
               DateTime.Parse(changeRow["EFFECTIVE_DATE"].ToString()),
               changeRow["CURRENT_SUPERVISOR"].ToString(),
               (changeRow["IS_NEW_POSITION"].ToString() == "Y") ? true : false,
               changeRow["POSITION_NAME"].ToString(),
               (changeRow["IS_NEW_SUPERVISOR"].ToString() == "Y") ? true : false,
               changeRow["NEW_SUPERVISOR"].ToString(),
               (changeRow["TEMP_TO_FTE"].ToString() == "Y") ? true : false,
               changeRow["FLSA_STATUS"].ToString(),
               changeRow["COMMENTS"].ToString(),
               changeRow["MODIFIED_BY"].ToString(),
               DateTime.Parse(changeRow["CREATED_DATE"].ToString()),
               changeRow["CHANGE_TYPE"].ToString(),
               int.Parse(changeRow["TASK_LIST_ID"].ToString())
               );

                IESCRFList.Add(change);
            }
            return IESCRFList;
        }


    }
    public class ChangeDBContext : DbContext
    {
        public DbSet<ChangeModel> Changes { get; set; }
    }
}