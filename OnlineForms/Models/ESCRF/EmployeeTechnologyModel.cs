using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity;

namespace OnlineForms.Models.ESCRF
{
    public class EmployeeTechnologyModel
    {
        public int ID { get; set; }

        [Display(Name = "I do not need access to this information. All of this information will be deleted and will NOT be retrievable.")]
        public bool NoAccess { get; set; }

        [Display(Name = "I agree to the terms described above.")]
        public bool Agreement { get; set; }
        [Required]
        [Display(Name = "Who should their work be transfered to?(If this is a claims adjuster please complete the Caseload Move Form)")]
        public string TransferringTo { get; set; }
        [Required]
        [Display(Name = "Can the employee be removed from all programs they currently have access to?")]
        public string RemoveEmployee { get; set; }
        [Required]
        [Display(Name = "Is this employee being replaced?")]
        public string Replaced { get; set; }
        [MaxLength(800)]
        public string Comments { get; set; }

        public EmployeeTechnologyModel(int id, bool noAccess, bool agreement, string transferringTo, string removeEmployee, string replaced, string comments)
        {
            ID = id;
            NoAccess = noAccess;
            Agreement = agreement;
            TransferringTo = transferringTo;
            RemoveEmployee = removeEmployee;
            Replaced = replaced;
            Comments = comments;
        }

        public static EmployeeTechnologyModel ConvertDataTableToEmployeeTechnology(DataTable dtEmployeeTechnology)
        {
            DataRow drEmployeeTechnology = dtEmployeeTechnology.Rows[0];
            EmployeeTechnologyModel employeeTechnology = new EmployeeTechnologyModel(
                int.Parse(drEmployeeTechnology["ID_NUMBER"].ToString()),
                (drEmployeeTechnology["RETAIN"].ToString() == "Y") ? true : false,
                (drEmployeeTechnology["AGREEMENT"].ToString() == "Y") ? true : false,
                drEmployeeTechnology["TRANSFERRING_TO"].ToString(),
                drEmployeeTechnology["REMOVE_EMPLOYEE"].ToString(),
                drEmployeeTechnology["REPLACED"].ToString(),
                drEmployeeTechnology["COMMENTS"].ToString()
                );

            return employeeTechnology;
        }

    }
    public class EmployeeTechnologyDBContext : DbContext
    {
        public DbSet<EmployeeTechnologyModel> EmployeeTechnologies { get; set; }
    }
}
