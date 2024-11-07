using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using OnlineForms.Models.SFN52712;

namespace OnlineForms.ViewModels
{
    public class SFN52712ViewModel
    {
        public SFN52712Model TravelAuthoriztionModel { get; set; }
        public SFN52712DestinationModel SFN52712Destination { get; set; }
        public List<SFN52712DestinationModel> SFN52712Destinations { get; set; }
        public SFN52712FlightMethod FlightInfo { get; set; }
        public SFN52712Approval SFN52712Approvals { get; set; }
    }

    public class SFN52712IndexViewmodel
    {
        public List<SFN52712Model> SFN52712s { get; set; }
        public List<SFN52712DestinationModel> SFN52712Destinations { get; set; }
        public List<SFN52712Approval> SFN52712Approvals { get; set; }
	}

}