using OnlineForms.Models.SFN54497;
using OnlineForms.Models.SFN61579;
using System.Collections.Generic;

namespace OnlineForms.ViewModels
{
    public class SFN54497DisplayViewModel
    {
        public List<SFN54497Model> StaffRequestModel { get; set; }
        public List<SFN54497Approval> Approval { get; set; }
        public SFN54497Model StaffRequestModels { get; set; }
        public SFN54497Approval Approvals { get; set; }
    }
}