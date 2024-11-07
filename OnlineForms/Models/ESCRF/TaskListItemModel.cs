using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace OnlineForms.Models.ESCRFViewModel
{
    public class TaskListItemModel
    {
        public int ID { get; set; }

        public string Task { get; set; }
        [Display(Name = "Assigned To")]
        public string Deputy { get; set; }

        public DateTime SignedOn { get; set; }

        public int TaskListID { get; set; }

        [DataType(DataType.MultilineText)]
        public string Comments { get; set; }


        public bool Completed { get; set; }

        public bool NotApplicable { get; set; }

        public string CompletedBy { get; set; }

        public string DefaultTaskID { get; set; }

        public string Dept { get; set; }

        [Display(Name = "Department")]
        public string DepartmentName { get; set; }

        public string Dep_Email { get; set; }

        public TaskListItemModel() { }

        public TaskListItemModel(int iD, string task, string deputy, DateTime signedOn, int taskListID, string comments, bool completed, bool notApplicable, string completedBy, string defaultTaskId, string dept, string dep_Email)
        {
            ID = iD;
            Task = task;
            Deputy = deputy;
            SignedOn = signedOn;
            TaskListID = taskListID;
            Comments = comments;
            Completed = completed;
            NotApplicable = notApplicable;
            CompletedBy = completedBy;
            DefaultTaskID = defaultTaskId;
            Dept = dept;
            Dep_Email = dep_Email;
        }

        public static TaskListItemModel ConvertDataTableToTaskListItem(DataTable dtTaskListItem)
        {
            DataRow drTaskListItem = dtTaskListItem.Rows[0];
            TaskListItemModel taskListItemModel = new TaskListItemModel(
                int.Parse(drTaskListItem["ID_NUMBER"].ToString()),
                drTaskListItem["TASK"].ToString(),
                drTaskListItem["DEPUTY"].ToString(),
                DateTime.Parse(drTaskListItem["SIGNED_ON"].ToString()),
                int.Parse(drTaskListItem["TASK_LIST_ID"].ToString()),
                drTaskListItem["COMMENTS"].ToString(),
                (drTaskListItem["COMPLETED"].ToString() == "Y") ? true : false,
                (drTaskListItem["NOT_APPLICABLE"].ToString() == "Y") ? true : false,
                drTaskListItem["COMPLETED_BY"].ToString(),
                drTaskListItem["DEFAULT_TASK_ID"].ToString(),
                drTaskListItem["DEPT"].ToString(),
                drTaskListItem["DEP_EMAIL"].ToString()
                );
            return taskListItemModel;
        }

        public static List<TaskListItemModel> GetTaskListItemList(DataTable dtTaskListItem)
        {
            List<TaskListItemModel> taskListItemList = new List<TaskListItemModel>();
            foreach (DataRow taskListRow in dtTaskListItem.Rows)
            {
                TaskListItemModel taskListItem = new TaskListItemModel(
                    int.Parse(taskListRow["ID_NUMBER"].ToString()),
                    taskListRow["TASK"].ToString(),
                    taskListRow["DEPUTY"].ToString(),
                    DateTime.Parse(taskListRow["SIGNED_ON"].ToString()),
                    int.Parse(taskListRow["TASK_LIST_ID"].ToString()),
                    taskListRow["COMMENTS"].ToString(),
                    (taskListRow["COMPLETED"].ToString() == "Y") ? true : false,
                    (taskListRow["NOT_APPLICABLE"].ToString() == "Y") ? true : false,
                    taskListRow["COMPLETED_BY"].ToString(),
                    taskListRow["DEFAULT_TASK_ID"].ToString(),
                    taskListRow["DEPT"].ToString(),
                    taskListRow["DEP_EMAIL"].ToString()
                    );
                taskListItemList.Add(taskListItem);
            }
            return taskListItemList;
        }
    }
}