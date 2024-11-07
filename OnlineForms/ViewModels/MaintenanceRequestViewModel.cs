using OnlineForms.Models;
using System.Collections.Generic;
namespace OnlineForms.ViewModels
{
    public class MaintenanceRequestViewModel
    {
        public MaintenanceRequestModel MaintenanceRequest { get; set; }
        public List<MaintenanceRequestModel> MaintenanceRequests { get; set; } = new List<MaintenanceRequestModel>();
    }
};