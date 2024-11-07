using System.Collections.Generic;
using System.Data;

namespace OnlineForms.Models.ESCRFViewModel
{
    public class DefaultTaskListItemModel
    {
        public string Task { get; set; }

        public string Deputy { get; set; }

        public string ChangeType { get; set; }

        public int ID { get; set; }

        public string Dept { get; set; }

        public DefaultTaskListItemModel() { }

        public DefaultTaskListItemModel(string task, string deputy, string changeType, int iD, string dept)
        {

            Task = task;
            Deputy = deputy;
            ChangeType = changeType;
            ID = iD;
            Dept = dept;
        }

        public static DefaultTaskListItemModel ConvertDataTableToTaskListItem(DataTable dtTaskListItem)
        {
            DataRow drTaskListItem = dtTaskListItem.Rows[0];
            DefaultTaskListItemModel taskListItemModel = new DefaultTaskListItemModel(
                drTaskListItem["TASK"].ToString(),
                drTaskListItem["DEPUTY"].ToString(),
                drTaskListItem["CHANGETYPE"].ToString(),
                int.Parse(drTaskListItem["ID_NUMBER"].ToString()),
                drTaskListItem["DEPT"].ToString()
                );
            return taskListItemModel;
        }

        public static List<DefaultTaskListItemModel> GetTaskListItemList(DataTable dtTaskListItem)
        {
            List<DefaultTaskListItemModel> taskListItemList = new List<DefaultTaskListItemModel>();
            foreach (DataRow taskListRow in dtTaskListItem.Rows)
            {
                DefaultTaskListItemModel taskListItem = new DefaultTaskListItemModel(
                    taskListRow["TASK"].ToString(),
                    taskListRow["DEPUTY"].ToString(),
                    taskListRow["CHANGE_TYPE"].ToString(),
                    int.Parse(taskListRow["ID_NUMBER"].ToString()),
                    taskListRow["DEPT"].ToString()
                    );
                taskListItemList.Add(taskListItem);
            }
            return taskListItemList;
        }
    }
}