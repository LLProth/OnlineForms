using OnlineForms.Helper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices.AccountManagement;
using System.Web.Http;



namespace OnlineForms.Controllers
{
    public class ValidationController : ApiController
    {
        // GET: api/Validation/5
        public List<String> Get(string query)
        {
            if (query == null)
            {
                query = "";
            }
            else
            {
                query = query.Trim();
            }

            List<string> finalEdit = new List<string>();
            List<string> names = new List<string>();


            using (PrincipalContext principalContext = new PrincipalContext(ContextType.Domain, "nd.gov", "OU=WSI, DC=nd,DC=gov"))
            using (UserPrincipal userPrincipal = new UserPrincipal(principalContext) { Enabled = true })
            using (PrincipalSearcher userSearcher = new PrincipalSearcher(userPrincipal))
            using (PrincipalSearchResult<Principal> results = userSearcher.FindAll())
            {
                foreach (Principal result in results)
                {
                    string firstLast = result.ToString();
                    string distinguished = result.DistinguishedName.ToString();
                    //if (distinguished.Contains("CN=WSI"))
                    //{ continue; }
                    //if (distinguished.Contains("CN=!"))
                    //{ continue; }
                    //if (distinguished.Contains("OU=Vendor-Contractor"))
                    //{ continue; }

                    int count = 0;
                    if (query != null)
                    {
                        // COMMENTED OUT THIS FOREACH STATEMENT SO THAT I COULD CHANGE THIS TO IF THE NAME CONTAINS THE QUERY
                        //foreach (char letters in query)
                        //{
                        if (firstLast.ToLower().Contains(query.ToLower()))
                        {
                            //count++;
                            //if (count == query.Length)
                            //{
                            if (!names.Contains(firstLast))
                            {
                                names.Add(result.DisplayName);
                            }

                            //}
                        }
                        else
                        {
                            break;
                        }
                        //}
                    }
                    //    }
                    names.Add("HR");
                    names.Add("ERGO");
                    names.Add("Help Desk");
                    names.Add("IS Tech");
                    names.Add("Facility Management");
                    int count2 = 0;
                    foreach (string name in names)
                    {
                        if (!finalEdit.Contains(name))
                        {
                            finalEdit.Add(name);
                            count2++;
                            if (count2 == 5)
                            {
                                break;
                            }
                        }

                    }
                }
                foreach (string name in finalEdit)
                { Debug.WriteLine(name); }
                // Sort the results
                finalEdit.Sort();
                return finalEdit;
            }

        }
    }
}
