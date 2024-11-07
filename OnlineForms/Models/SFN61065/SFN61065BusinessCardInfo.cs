using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace OnlineForms.Models.SFN61065
{
    public class SFN61065BusinessCardInfo
    {
        public int ID { get; set; }

        [Display(Name = "First name")]
        [Required(ErrorMessage = "First name is required")]
        public string FirstName { get; set; }

        [Display(Name = "Last name")]
        [Required(ErrorMessage = "Last name is required")]
        public string LastName { get; set; }

        [Display(Name = "Credentials (RN, MD, CPC, etc.)")]
        public string Credentials { get; set; }

        [Display(Name = "Title")]
        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; }

        [Display(Name = "Email")]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }

        [Display(Name = "Telephone number")]
        [Required(ErrorMessage = "Telephone number is required")]
        public string TelephoneNum { get; set; }

        [Display(Name = "Cell number")]
        public string CellNum { get; set; }

        [Display(Name = "Fax number")]
        [Required(ErrorMessage = "Fax number is required")]
        public string FaxNum { get; set; }

        [Display(Name = "Fax number")]
        public string FaxNumType { get; set; }
        public SFN61065BusinessCardInfo(string firstname, string lastname, string credentials, string title, string email, string telephoneNum, string cellNum, string faxNum)
        {
            FirstName = firstname;
            LastName = lastname;
            Credentials = credentials;
            Title = title;
            Email = email;
            TelephoneNum = telephoneNum;
            CellNum = cellNum;
            FaxNum = faxNum;
        }

        public static List<SFN61065BusinessCardInfo> ConvertDataTableToBusinessCardInfo(DataTable dtReq)
        {
            List<SFN61065BusinessCardInfo> businessCardInfo = new List<SFN61065BusinessCardInfo>();
            foreach (DataRow row in dtReq.Rows)
            {
                SFN61065BusinessCardInfo reqItem = new SFN61065BusinessCardInfo(
                    row["FIRST_NAME"].ToString(),
                    row["LAST_NAME"].ToString(),
                    row["CREDENTIALS"].ToString(),
                    row["TITLE"].ToString(),
                    row["EMAIL"].ToString(),
                    row["TELEPHONE_NUMBER"].ToString(),
                    row["CELL_NUMBER"].ToString(),
                    row["FAX_NUMBER"].ToString()
                );
                businessCardInfo.Add(reqItem);
            }
            return businessCardInfo;
        }
        public static SFN61065BusinessCardInfo ConvertDataTableToBusinessCardInfos(DataTable dtReq)
        {
            DataRow drReq = dtReq.Rows[0];
            SFN61065BusinessCardInfo req = new SFN61065BusinessCardInfo(
                drReq["FIRST_NAME"].ToString(),
                    drReq["LAST_NAME"].ToString(),
                    drReq["CREDENTIALS"].ToString(),
                    drReq["TITLE"].ToString(),
                    drReq["EMAIL"].ToString(),
                    drReq["TELEPHONE_NUMBER"].ToString(),
                    drReq["CELL_NUMBER"].ToString(),
                    drReq["FAX_NUMBER"].ToString()
            );
            return req;
        }
    }
}