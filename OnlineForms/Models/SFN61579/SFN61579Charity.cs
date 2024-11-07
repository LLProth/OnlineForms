using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using OnlineForms.Models.SFN61579;

namespace OnlineForms.Models.SFN61579
{
    public class SFN61579CharityModel
    {
        [Display(Name = "ID Number (to be assigned)")]
        public int? ID { get; set; }
        public string CharityWebsite { get; set; }

        [Display(Name = "Reviewed")]
        public bool Reviewed { get; set; }

        [Display(Name = "Address line 1")]
        [Required(ErrorMessage = "Address line 1 is required")]
        public string AddressLineOne { get; set; }

        [Display(Name = "Address line 2")]
        public string AddressLineTwo { get; set; }

        [Display(Name = "City")]
        [Required(ErrorMessage = "City is required")]
        public string City { get; set; }

        [Display(Name = "State")]
        [Required(ErrorMessage = "State must be selected from list")]
        public string State { get; set; }

        [Display(Name = "Zip code")]
        [Required(ErrorMessage = "Zip code is required")]
        public string ZipCode { get; set; }

        [Display(Name = "Charity website")]
        [Required(ErrorMessage = "Charity website is required")]
        public string Website { get; set; }

        [Display(Name = "Name of organization")]
        [Required(ErrorMessage = "Name of organization is required")]
        public string Organization { get; set; }

        [Display(Name = "Month of charitable event")]
        [Required(ErrorMessage = "Month of charitable event is Required")]
        public string Month { get; set; }

        [Display(Name = "Please tell us about the organization and your involvement with them")]
        [DataType(DataType.MultilineText)]
        [Required(ErrorMessage = "This field is required")]
        public string CharityDescription { get; set; }

        public SFN61579CharityModel() { }

        public SFN61579CharityModel(int id, string addressLineOne, string addressLineTwo, string city, string state, string zipCode, string website, string organization, string month,
            string charityDescription)
        {
            ID = id;
            AddressLineOne = addressLineOne;
            AddressLineTwo = addressLineTwo;
            City = city;
            State = state;
            ZipCode = zipCode;
            Website = website;
            Organization = organization;
            Month = month;
            CharityDescription = charityDescription;
            
        }
        public static List<SFN61579CharityModel> ConvertDataTableToCharitableEventInfo(DataTable dtReq)
        {
            List<SFN61579CharityModel> charitableeventinfo = new List<SFN61579CharityModel>();
            foreach (DataRow row in dtReq.Rows)
            {
                SFN61579CharityModel reqItem = new SFN61579CharityModel(
                    int.Parse(row["ID_NUMBER"].ToString()),
                    row["ADDRESS_LINE_ONE"].ToString(),
                    row["ADDRESS_LINE_TWO"].ToString(),
                    row["ADDRESS_CITY"].ToString(),
                    row["ADDRESS_STATE"].ToString(),
                    row["ZIP_CODE"].ToString(),
                    row["WEBSITE"].ToString(),
                    row["ORGANIZATION_NAME"].ToString(),
                    row["EVENT_MONTH"].ToString(),
                    row["CHARITY_DESCRIPTION"].ToString()
                );

                charitableeventinfo.Add(reqItem);
            }
            return charitableeventinfo;
        }
        public static SFN61579CharityModel ConvertDataTableToCharitableEventInfos(DataTable dtReq)
        {
            DataRow drReq = dtReq.Rows[0];
            SFN61579CharityModel req = new SFN61579CharityModel(
                int.Parse(drReq["ID_NUMBER"].ToString()),
                drReq["ADDRESS_LINE_ONE"].ToString(),
                drReq["ADDRESS_LINE_TWO"].ToString(),
                drReq["ADDRESS_CITY"].ToString(),
                drReq["ADDRESS_STATE"].ToString(),
                drReq["ZIP_CODE"].ToString(),
                drReq["WEBSITE"].ToString(),
                drReq["ORGANIZATION_NAME"].ToString(),
                drReq["EVENT_MONTH"].ToString(),
                drReq["CHARITY_DESCRIPTION"].ToString()

            );
           return req;
        }
    }
}