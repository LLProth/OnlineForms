using OnlineForms.Models.SFN61579;
using System.Collections.Generic;

namespace OnlineForms.ViewModels
{
    public class SFN61579DisplayViewModel
    {
        public List<SFN61579Model> CharitableModel { get; set; }

        public SFN61579Model CharitableModels { get; set; }

        public List<SFN61579CharityModel> CharityInfoModel { get; set; }

        public SFN61579CharityModel CharityInfoModels { get; set; }
    }
}