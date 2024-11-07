using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity;

namespace OnlineForms.Models.ESCRF
{
    public class NewHireModel : IESCRF
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [MaxLength(100)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(100)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }


        [MaxLength(3)]
        [Display(Name = "Middle Initial")]
        public string MiddleInitial { get; set; }

        [Required]
        [MaxLength(100)]
        [Display(Name = "Job Title")]
        public string JobTitle { get; set; }

        [Required]
        [Display(Name = "Effective Date")]
        public DateTime EffectiveDate { get; set; }

        [Required]
        [MaxLength(207)]
        [Display(Name = "Supervisor")]
        public string CurrentSupervisor { get; set; }

        [Required]
        [Display(Name = "Employment Type")]
        public string EmployeeType { get; set; }

        [Required]
        [Display(Name = "Transferring from Govt Agency")]
        public bool TransferringFromAgency { get; set; }

        [Required]
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
            string fullName = string.Empty;
            if (MiddleInitial != null)
            {
                fullName = String.Format("{0}, {1} {2}", LastName, FirstName, MiddleInitial);

            }
            else
            {
                fullName = string.Format("{0}, {1}", LastName, FirstName);
            }

            return fullName;
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
                } else if (taskList.IsDeployed == false)
                {
                    this.Status = "Not Started";
                }
                else
                {
                    this.Status = "In Progress";
                }
            }

        }

        public NewHireModel() { }

        public NewHireModel(
            int id,
            string firstName,
            string middleInitial,
            string lastName,
            string jobTitle,
            DateTime effectiveDate,
            string currentSupervisor,
            string employeeType,
            bool transferringFromAgency,
            string flsaStatus,
            string comments,
            string modifiedBy,
            DateTime createdDate,
            string changeType,
            int taskListID
        )
        {
            ID = id;
            FirstName = firstName;
            MiddleInitial = middleInitial;
            LastName = lastName;
            JobTitle = jobTitle;
            EffectiveDate = effectiveDate;
            CurrentSupervisor = currentSupervisor;
            EmployeeType = employeeType;
            TransferringFromAgency = transferringFromAgency;
            FLSAStatus = flsaStatus;
            Comments = comments;
            ModifiedBy = modifiedBy;
            CreatedDate = createdDate;
            ChangeType = changeType;
            TaskListID = taskListID;
        }

        public static NewHireModel ConvertDataTableToNewHire(DataTable dtNewHire)
        {
            DataRow drNewHire = dtNewHire.Rows[0];
            NewHireModel newHire = new NewHireModel(
                int.Parse(drNewHire["ID_NUMBER"].ToString()),
                drNewHire["FIRST_NAME"].ToString(),
                drNewHire["MIDDLE_INITIAL"].ToString(),
                drNewHire["LAST_NAME"].ToString(),
                drNewHire["JOB_TITLE"].ToString(),
                DateTime.Parse(drNewHire["EFFECTIVE_DATE"].ToString()),
                drNewHire["CURRENT_SUPERVISOR"].ToString(),
                drNewHire["EMPLOYMENT_TYPE"].ToString(),
                (drNewHire["TRANSFERRING_FROM_AGENCY"].ToString() == "Y") ? true : false,
                drNewHire["FLSA_STATUS"].ToString(),
                drNewHire["COMMENTS"].ToString(),
                drNewHire["MODIFIED_BY"].ToString(),
                DateTime.Parse(drNewHire["CREATED_DATE"].ToString()),
                drNewHire["CHANGE_TYPE"].ToString(),
                int.Parse(drNewHire["TASK_LIST_ID"].ToString())
            );

            return newHire;
        }

        public static List<IESCRF> GetNewHireList(DataTable dtNewHire)
        {
            List<IESCRF> IESCRFList = new List<IESCRF>();
            foreach (DataRow NewHireRow in dtNewHire.Rows)
            {
                NewHireModel NewHire = new NewHireModel(
                    int.Parse(NewHireRow["ID_NUMBER"].ToString()),
                    NewHireRow["FIRST_NAME"].ToString(),
                    NewHireRow["MIDDLE_INITIAL"].ToString(),
                    NewHireRow["LAST_NAME"].ToString(),
                    NewHireRow["JOB_TITLE"].ToString(),
                    DateTime.Parse(NewHireRow["EFFECTIVE_DATE"].ToString()),
                    NewHireRow["CURRENT_SUPERVISOR"].ToString(),
                    NewHireRow["EMPLOYMENT_TYPE"].ToString(),
                    (NewHireRow["TRANSFERRING_FROM_AGENCY"].ToString() == "Y") ? true : false,
                    NewHireRow["FLSA_STATUS"].ToString(),
                    NewHireRow["COMMENTS"].ToString(),
                    NewHireRow["MODIFIED_BY"].ToString(),
                    DateTime.Parse(NewHireRow["CREATED_DATE"].ToString()),
                    NewHireRow["CHANGE_TYPE"].ToString(),
                    int.Parse(NewHireRow["TASK_LIST_ID"].ToString())
                );

                IESCRFList.Add(NewHire);
            }

            return IESCRFList;
        }
    }

    public class NewHireDBContext : DbContext
    {
        public DbSet<NewHireModel> NewHires { get; set; }
    }
};
