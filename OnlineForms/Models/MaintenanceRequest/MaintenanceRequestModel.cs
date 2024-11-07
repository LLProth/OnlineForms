using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;

namespace OnlineForms.Models
{
    public class MaintenanceRequestModel
    {
        public int Id { get; set; }

        public string EmployeeName { get; set; }

        public string Email { get; set; }

        public string Agency { get; set; }

        public string Phonenumber { get; set; }

        public DateTime EnteredDate { get; set; }

        public DateTime CompletedDate { get; set; }

        public string Description { get; set; }

        public string Subject { get; set; }

        public MaintenanceRequestModel() { }

        public MaintenanceRequestModel(int id, string name, string email, string agency, string phonenumber, DateTime enteredDate, DateTime completedDated, string description, string subject)
        {
            Id = id;
            EmployeeName = name;
            Email = email;
            Agency = agency;
            Phonenumber = phonenumber;
            EnteredDate = enteredDate;
            CompletedDate = completedDated;
            Description = description;
            Subject = subject;

        }

        public static MaintenanceRequestModel ConvertDataTableToMaintenanceRequest(DataTable dtMaint)
        {
            DataRow drMaint = dtMaint.Rows[0];
            MaintenanceRequestModel maintenanceRequest = new MaintenanceRequestModel(
                int.Parse(drMaint["ID_NUMBER"].ToString()),
                drMaint["Employee_NAME"].ToString(),
                drMaint["EMAIL"].ToString(),
                drMaint["AGENCY"].ToString(),
                drMaint["PHONENUMBER"].ToString(),
                DateTime.Parse(drMaint["ENTERED_DATE"].ToString()),
                DateTime.Parse(drMaint["COMPLETED_DATE"].ToString()),
                drMaint["DESCRIPTION"].ToString(),
                drMaint["SUBJECT"].ToString()
                );
            return maintenanceRequest;
        }

        public static List<MaintenanceRequestModel> GetMaintenanceRequestsList(DataTable dtMaint)
        {
            List<MaintenanceRequestModel> maintenanceRequests = new List<MaintenanceRequestModel>();
            foreach (DataRow row in dtMaint.Rows)
            {
                MaintenanceRequestModel maintenanceRequest = new MaintenanceRequestModel(
                     int.Parse(row["ID_NUMBER"].ToString()),
                row["Employee_NAME"].ToString(),
                row["EMAIL"].ToString(),
                row["AGENCY"].ToString(),
                row["PHONENUMBER"].ToString(),
                DateTime.Parse(row["ENTERED_DATE"].ToString()),
                DateTime.Parse(row["COMPLETED_DATE"].ToString()),
                row["DESCRIPTION"].ToString(),
                row["SUBJECT"].ToString()
                    );
                maintenanceRequests.Add(maintenanceRequest);
            }
            return maintenanceRequests;
        }
        public class MaintenanceRequestDBContext : DbContext
        {
            public DbSet<MaintenanceRequestModel> MaintenanceRequests { get; set; }
        }
    }
};