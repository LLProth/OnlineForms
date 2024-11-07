using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace OnlineForms.Models.ESCRFViewModel
{
    public class ChangelistInfoModel
    {
        public int ID { get; set; }

        public int ChangelistID { get; set; }

        public string ChangeType { get; set; }
        [Display(Name = "Change Type")]
        public string EmployeeName { get; set; }
        [Display(Name = "Employee Name")]

        public DateTime EffectiveDate { get; set; }

        public ChangelistInfoModel() { }

        public ChangelistInfoModel(int iD, int changelistID, string changeType, string employeeName, DateTime effectiveDate)
        {
            ID = iD;
            ChangelistID = changelistID;
            ChangeType = changeType;
            EmployeeName = employeeName;
            EffectiveDate = effectiveDate;
        }

        public static ChangelistInfoModel ConvertDataTableToChangelistInfoItem(DataTable dtTaskListItem)
        {
            DataRow drTaskListItem = dtTaskListItem.Rows[0];
            ChangelistInfoModel changelistInfoModel = new ChangelistInfoModel(
                int.Parse(drTaskListItem["ID_NUMBER"].ToString()),
                int.Parse(drTaskListItem["TASK_LIST_ID"].ToString()),
                drTaskListItem["CHANGE_TYPE"].ToString(),
                drTaskListItem["EMPLOYEE_NAME"].ToString(),
                DateTime.Parse(drTaskListItem["EFFECTIVE_DATE"].ToString())
                );
            return changelistInfoModel;
        }

        public static List<ChangelistInfoModel> GetChangelistInfoList(DataTable dtTaskListItem)
        {
            List<ChangelistInfoModel> taskListItemList = new List<ChangelistInfoModel>();
            foreach (DataRow taskListRow in dtTaskListItem.Rows)
            {
                ChangelistInfoModel taskListItem = new ChangelistInfoModel(
                   int.Parse(taskListRow["ID_NUMBER"].ToString()),
                int.Parse(taskListRow["TASK_LIST_ID"].ToString()),
                taskListRow["CHANGE_TYPE"].ToString(),
                taskListRow["EMPLOYEE_NAME"].ToString(),
                DateTime.Parse(taskListRow["EFFECTIVE_DATE"].ToString())
                    );
                taskListItemList.Add(taskListItem);
            }
            return taskListItemList;
        }
    }
}