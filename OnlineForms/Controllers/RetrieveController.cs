using System.Diagnostics;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Web.Http;

namespace OnlineForms.Controllers
{
    public class RetrieveController : ApiController
    {
        // GET api/Retrieve/5
        public string Get(string query)
        {
            if (query == null)
            {
                query = "";
            }
            else
            {
                query = query.Trim();
            }
            Debug.WriteLine("+++++++++++++++++");
            Debug.WriteLine(query);
            string name = string.Empty;
            string phone = string.Empty;
            string dept = string.Empty;
            string location = string.Empty;
            string output = string.Empty;
            string manager = string.Empty;
            string email = string.Empty;
            string title = string.Empty;
            string managerDistinguishedName = string.Empty;
            using (PrincipalContext principalContext = new PrincipalContext(ContextType.Domain, "nd.gov", "OU=WSI, DC=nd,DC=gov"))
            using (UserPrincipal userPrincipal = new UserPrincipal(principalContext) { Enabled = true, DisplayName = query })
            using (PrincipalSearcher userSearcher = new PrincipalSearcher(userPrincipal))
            using (PrincipalSearchResult<Principal> results = userSearcher.FindAll())
            {
                foreach (Principal result in results)
                {
                    DirectoryEntry dirEntry = (DirectoryEntry)result.GetUnderlyingObject();

                    // HD - Commented this out since I added DisplayName to the userPrincipal. We don't need to check this again
                    //name = dirEntry.Properties["displayName"].Value.ToString();

                    //if (query == name)
                    //{
                        try
                        {
                            name = dirEntry.Properties["name"].Value.ToString();
                        }
                        catch { name = "No Name in AD"; };
                        try
                        {
                            phone = dirEntry.Properties["telephoneNumber"].Value.ToString();
                        }
                        catch { phone = "(000)000-0000"; };
                        try
                        {
                            dept = dirEntry.Properties["department"].Value.ToString();
                        }
                        catch { dept = "No Dept in AD"; };
                        try
                        {
                            location = dirEntry.Properties["physicalDeliveryOfficeName"].Value.ToString();
                        }
                        catch { location = "No location in AD"; };
                        try
                        {
                            // Creating new variable for distinguished name
                            managerDistinguishedName = dirEntry.Properties["manager"].Value.ToString();
                            // Grabbing display name so that we can accurately grab information later on
                            UserPrincipal managerName = UserPrincipal.FindByIdentity(principalContext, managerDistinguishedName);
                            manager = managerName.DisplayName;
                        }
                        catch { manager = "No Manager in AD"; }
                        try
                        {
                            email = dirEntry.Properties["mail"].Value.ToString();
                        }
                        // if you edit catch string need to email generation in controller
                        catch { email = "No Email in AD"; }
                        try
                        {
                            title = dirEntry.Properties["title"].Value.ToString();
                        }
                        catch { title = "No Title in AD"; }
                        // Added managerDistinguishedName to the end of this since manager is now using display name and distinguished name seems to be required in some cases
                        output = name + "+" + phone + "+" + dept + "+" + location + "+" + manager + "+" + email + "+" + title + "+" + managerDistinguishedName;
                        return (output);
                    }



                
            }

            output = "-----------------";
            return (output);
        }
    }
}