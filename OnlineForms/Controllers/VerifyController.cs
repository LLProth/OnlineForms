using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Web.Http;

namespace OnlineForms.Controllers
{
    public class VerifyController : ApiController
    {
        // GET api/<controller>/5
        public bool Get(string query)
        {
            if (query == null)
            {
                query = " ";
            }
            else
            {
                query = query.Trim();
            }
            using (PrincipalContext principalContext = new PrincipalContext(ContextType.Domain, "nd.gov", "OU=WSI, DC=nd,DC=gov"))
            using (UserPrincipal userPrincipal = new UserPrincipal(principalContext) { Enabled = true })
            using (PrincipalSearcher userSearcher = new PrincipalSearcher(userPrincipal))
            using (PrincipalSearchResult<Principal> results = userSearcher.FindAll())
            {
                foreach (Principal result in results)
                {
                    DirectoryEntry dirEntry = (DirectoryEntry)result.GetUnderlyingObject();
                    string firstLast = result.ToString();
                    string distinguished = result.DistinguishedName.ToString();

                    //if (distinguished.Contains("CN=WSI"))
                    //{ continue; }
                    //if (distinguished.Contains("CN=!"))
                    //{ continue; }
                    //if (distinguished.Contains("OU=Vendor-Contractor"))
                    //{ continue; }
                    if (query == firstLast)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}