using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using OnlineForms.Models.SFN61579;

namespace OnlineForms.Models.SFN61579
{
    public class SFN61579Model
    {
        [Display(Name = "ID Number (to be assigned)")]
        public int? ID { get; set; }

        [Display(Name = "REQUESTING EMPLOYEE NAME")]
        [Required]
        public string Name { get; set; }

        [Display(Name = "Date")]
        public DateTime DateSubmitted { get; set; }

        [Display(Name = "Reviewed")]
        public bool Reviewed { get; set; }

        public SFN61579Model() { }

        public SFN61579Model(int id, string name, DateTime dateSubmitted, bool reviewed)
        {
            ID = id;
            Name = name;
            DateSubmitted = dateSubmitted;
            Reviewed = reviewed;
        }

        public static List<SFN61579Model> ConvertDataTableToCharitableEvent(DataTable dtReq)
        {
            List<SFN61579Model> charitableevent = new List<SFN61579Model>();
            foreach (DataRow row in dtReq.Rows)
            {
                SFN61579Model reqItem = new SFN61579Model(
                    int.Parse(row["ID_NUMBER"].ToString()),
                    row["REQUEST_EMPLOYEE"].ToString(),
                    DateTime.Parse(row["REQUEST_DATE"].ToString()),
                    (row["REVIEWED"].ToString() == "Y") ? true : false
                );

                charitableevent.Add(reqItem);
            }

            return charitableevent;
        }
        public static SFN61579Model ConvertDataTableToCharitableEvents(DataTable dtReq)
        {
            DataRow drReq = dtReq.Rows[0];
            SFN61579Model req = new SFN61579Model(
                int.Parse(drReq["ID_NUMBER"].ToString()),
                drReq["REQUEST_EMPLOYEE"].ToString(),
                DateTime.Parse(drReq["REQUEST_DATE"].ToString()),
                (drReq["REVIEWED"].ToString() == "Y") ? true : false

            );
           return req;
        }
    }
}