using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;

namespace OnlineForms.Models.ESCRF
{
    public class TaskListModel
    {
        public int ID { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime FinishedDate { get; set; }

        public string ChangeType { get; set; }

        public bool IsDeployed { get; set; }

        public TaskListModel() { }

        public TaskListModel(int iD, DateTime createdDate, DateTime finishedDate, string changeType, bool isDeployed)
        {
            ID = iD;
            CreatedDate = createdDate;
            FinishedDate = finishedDate;
            ChangeType = changeType;
            IsDeployed = isDeployed;
        }

        public static TaskListModel ConvertDataTableToTaskList(DataTable dtTaskList)
        {
            DataRow drTaskList = dtTaskList.Rows[0];
            TaskListModel taskList = new TaskListModel(
                int.Parse(drTaskList["ID_NUMBER"].ToString()),
                DateTime.Parse(drTaskList["CREATED_DATE"].ToString()),
                DateTime.Parse(drTaskList["FINISHED_DATE"].ToString()),
                drTaskList["CHANGE_TYPE"].ToString(),
                (drTaskList["IS_DEPLOYED"].ToString() == "Y") ? true : false

                );

            return taskList;
        }

        public static List<TaskListModel> GetTaskListList(DataTable dtTaskList)
        {
            List<TaskListModel> taskListList = new List<TaskListModel>();
            foreach (DataRow taskListRow in dtTaskList.Rows)
            {
                TaskListModel taskList = new TaskListModel(
                int.Parse(taskListRow["ID_NUMBER"].ToString()),
                DateTime.Parse(taskListRow["CREATED_DATE"].ToString()),
                DateTime.Parse(taskListRow["FINISHED_DATE"].ToString()),
                taskListRow["CHANGE_TYPE"].ToString(),
                (taskListRow["IS_DEPLOYED"].ToString() == "Y") ? true : false

                    );
                taskListList.Add(taskList);
            }
            return taskListList;
        }
    }
    public class TaskListDBContext : DbContext
    {
        public DbSet<TaskListModel> TaskLists { get; set; }
    }
}

