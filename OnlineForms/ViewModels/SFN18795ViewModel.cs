using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OnlineForms.Models.SFN18795;

namespace OnlineForms.ViewModels
{
    public class SFN18795ViewModel
    {
        public SFN18795Model RequisitionModel { get; set; }
        public SFN18795RequisitionItem RequisitionItem { get; set; }
    }

    public class SFN18795IndexViewModel
    {
        public List<SFN18795Model> SFN18795s { get; set; }
        public List<SFN18795RequisitionItem> RequisitionItems { get; set; }
    }
}