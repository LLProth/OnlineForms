using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OnlineForms.Models.SFN61065;
using OnlineForms.Models.SFN61579;

namespace OnlineForms.ViewModels
{
    public class SFN61065ViewModel
    {
        public SFN61065Model BusinessCardModel { get; set; }
        public SFN61065BusinessCardInfo BusinessCardInfo { get; set; }
        public SFN61065Approval Approval { get; set; }
    }
}