using System.Collections.Generic;
using System.Data;
using System.Data.Entity;

namespace OnlineForms.Models.ESCRF
{
    public class WSITitleModel
    {
        public int TitleID { get; set; }

        public string WSITitle { get; set; }


        public WSITitleModel() { }

        public WSITitleModel(int titleID, string wsiTitle)
        {
            TitleID = titleID;
            WSITitle = wsiTitle;
        }

        public static WSITitleModel ConvertDataTableToWSITitle(DataTable dtTitle)
        {

            DataRow drWSITitle = dtTitle.Rows[0];
            WSITitleModel wsiTitle = new WSITitleModel(
                int.Parse(drWSITitle["TITLE_ID"].ToString()),
                drWSITitle["WSI_TITLE"].ToString()
                );

            return wsiTitle;
        }

        public static List<WSITitleModel> GetWSITitleList(DataTable dtTitle)
        {
            List<WSITitleModel> wsiTitleList = new List<WSITitleModel>();
            foreach (DataRow wsiTitleRow in dtTitle.Rows)
            {
                WSITitleModel wsiTitle = new WSITitleModel(
                int.Parse(wsiTitleRow["TITLE_ID"].ToString()),
                wsiTitleRow["WSI_TITLE"].ToString()
                    );
                wsiTitleList.Add(wsiTitle);
            }
            return wsiTitleList;
        }
    }
    public class WSITitleDBContext : DbContext
    {
        public DbSet<WSITitleModel> wsiTitles { get; set; }
    }
}

