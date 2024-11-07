using OnlineForms.Models.ESCRF;
using OnlineForms.Models.ESCRFViewModel;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;

namespace OnlineForms.ViewModels
{
    public class ESCRFViewModel
    {
        public string ChangeType { get; set; } = string.Empty;
        public IESCRF CreateHolder { get; set; }
        public List<IESCRF> IndexHolder { get; set; } = new List<IESCRF>() { };
        public List<TaskListItemModel> TaskListItemHolder { get; set; } = new List<TaskListItemModel>();
        public NewHireModel NewHire { get; set; } = new NewHireModel();
        public TerminationModel Termination { get; set; } = new TerminationModel();
        public ChangeModel Change { get; set; } = new ChangeModel();
        public NameModel Name { get; set; } = new NameModel();
        public ESCRFUIModel ESCRFUI { get; set; } = new ESCRFUIModel();
        public TaskListItemModel TaskListItem { get; set; }
        public TechnologyRequirementsModel TechRequirements { get; set; }
        public List<TaskListItemModel> TaskListItemList { get; set; }
        public ChangelistInfoModel ChangeListInfo { get; set; }
        public TaskListModel TaskList { get; set; }
        public List<DefaultTaskListItemModel> DefaultTaskListList = new List<DefaultTaskListItemModel> { };
        public List<Principal> EmployeeList = new List<Principal>();
        public List<WSITitleModel> wsiTitles = new List<WSITitleModel>();
        public List<string> titleStrings = new List<string>();
        public List<string> emailList = new List<string>();
        public EmployeeTechnologyModel EmployeeTechnology { get; set; }
        //This is for the dept affiliations of the logged in user
        public List<string> departments = new List<string>();
        public List<ESCRFRoleModel> Roles { get; set; } = new List<ESCRFRoleModel>();
        //for sorting the submitted forms on the main table 
        public string SortBy { get; set; } = string.Empty;
        public bool IsAscending { get; set; } = true;









    }
}