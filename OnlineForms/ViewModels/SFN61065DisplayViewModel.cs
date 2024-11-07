using OnlineForms.Models.SFN61065;
using OnlineForms.Models.SFN61579;
using System.Collections.Generic;

namespace OnlineForms.ViewModels
{
    public class SFN61065DisplayViewModel
    {
        public List<SFN61065Model> BusinessCardModel { get; set; }
        public List<SFN61065BusinessCardInfo> BusinessCardInfo { get; set; }
        public List<SFN61065Approval> Approval { get; set; }

        public SFN61065Model BusinessCardModels { get; set; }
        public SFN61065BusinessCardInfo BusinessCardInfos { get; set; }
        public SFN61065Approval Approvals { get; set; }
    }
}