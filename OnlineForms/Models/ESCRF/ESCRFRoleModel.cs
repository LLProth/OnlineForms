using System.Collections.Generic;
using System.Data;
using System.Data.Entity;

namespace OnlineForms.Models.ESCRF
{
    public class ESCRFRoleModel
    {
        public string EmployeeName { get; set; }

        public string DepartmentAffiliation { get; set; }

        public string EmployeeEmail { get; set; }

        public ESCRFRoleModel() { }

        public ESCRFRoleModel(string employeeName, string departmentAffiliation, string employeeEmail)
        {
            EmployeeName = employeeName;
            DepartmentAffiliation = departmentAffiliation;
            EmployeeEmail = employeeEmail;
        }

        public static List<ESCRFRoleModel> GetRoleList(DataTable dtRole)
        {
            List<ESCRFRoleModel> Roles = new List<ESCRFRoleModel>();
            foreach (DataRow drRole in dtRole.Rows)
            {
                ESCRFRoleModel Role = new ESCRFRoleModel(
                    drRole["EMPLOYEE_NAME"].ToString(),
                    drRole["DEPARTMENT_AFFILIATION"].ToString(),
                    drRole["EMPLOYEE_EMAIL"].ToString()
                    );
                Roles.Add(Role);
            }
            return Roles;
        }
    }
    public class ESCRFRoleDBContext : DbContext
    {
        public DbSet<ESCRFRoleModel> Roles { get; set; }
    }
};