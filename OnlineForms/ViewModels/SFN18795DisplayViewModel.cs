using OnlineForms.Models.SFN18795;
using System.Collections.Generic;

namespace OnlineForms.ViewModels
{
    public class SFN18795DisplayViewModel
    {
        public SFN18795Model RequisitionModel { get; set; }

        public SFN18795RequisitionItem RequisitionItem { get; set; }

        public List<SFN18795RequisitionItem> RequisitionItems { get; set; }
    }
}