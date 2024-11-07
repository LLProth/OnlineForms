using System;
using System.Web;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Diagnostics;
using OnlineForms.Helper;
using System.Collections.Generic;

namespace OnlineForms.Helper
{
    public class DirectoryHelper
    {
        public static string getEmail(string userID)
        {
            string userEmail = "";
            using (PrincipalContext context = new PrincipalContext(ContextType.Domain))
            {
                using (UserPrincipal user = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, userID))
                {
                    userEmail = user.EmailAddress;
                }
            }

                return userEmail;
        }

        public static string getManager(UserPrincipal user)
        {
            string usertitle = getJobTitle(user);
            string manager;
            string managerDistinguishedName = string.Empty;
            try
            {
                if (usertitle.Equals("Director"))
                {
                    manager = user.SamAccountName;
                }
                else
                {
                    using (PrincipalContext context = new PrincipalContext(ContextType.Domain))
                    {
                        DirectoryEntry loggedInEntry = (DirectoryEntry)user.GetUnderlyingObject();
                        managerDistinguishedName = loggedInEntry.Properties["manager"][0].ToString();

                        using (UserPrincipal managerUser = UserPrincipal.FindByIdentity(context, IdentityType.DistinguishedName, managerDistinguishedName))
                        {
                            manager = managerUser.SamAccountName;
                        }
                    }
                }
                return manager;
            }
            catch (Exception e)
            {
                throw new Exception("Unable to get manager Error: " + e.Message);
            }            
        }

        public static string findName(string userID)
        {
            string name = "";
            using (PrincipalContext context = new PrincipalContext(ContextType.Domain))
            {
                using (UserPrincipal user = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, userID))
                {
                    name = user.DisplayName;
                }
            }
            return name;
        }

        public static UserPrincipal GetUserPrincipal(string userId)
        {
            UserPrincipal user;
            using (PrincipalContext context = new PrincipalContext(ContextType.Domain))
            {
                user = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, userId);
            }
            return user;
        }

        public static string findChief(UserPrincipal employee)
        {
            string chief = "";
            string managerTitle = getJobTitle(employee);
            if (managerTitle.Equals("Director"))
            {
                return "Chief Operations Officer";
            }
            string manager = getManager(employee);
            
            using (PrincipalContext context = new PrincipalContext(ContextType.Domain))
            {
                using (UserPrincipal managerUser = UserPrincipal.FindByIdentity(context, manager))
                {
                    manager = managerUser.DisplayName;
                    managerTitle = getJobTitle(managerUser);

                    if (managerTitle.Contains("Chief"))
                    {
                        chief = managerTitle;
                        return chief;
                    }
                    else
                    {
                       chief = findChief(managerUser);
                    }
                }
            }
            return chief;
        }

        public static string findDepartmentDirector(UserPrincipal employee)
        {
            string department = "";
            string managerTitle = getJobTitle(employee);
            if (managerTitle.Equals("Director"))
            {
                department = "no Department Director";
                return department;
            }
            string manager = getManager(employee);
            using (PrincipalContext context = new PrincipalContext(ContextType.Domain))
            {
                using (UserPrincipal managerUser = UserPrincipal.FindByIdentity(context, manager))
                {
                    manager = managerUser.DisplayName;
                    managerTitle = getJobTitle(managerUser);

                   if ((managerTitle.Contains("Director") && !managerTitle.Equals("Director of Legal Services") && !managerTitle.Equals("SIU Director")) || managerTitle.Contains("General Counsel"))
                    {
                        department = managerTitle;
                        return department;
                    }
                    else
                    {
                        department = findDepartmentDirector(managerUser);
                    }
                }
            }
            return department;
        }

        public static string[] findDirector(UserPrincipal employee)
        {
            string[] director = new string[2];
            string managerTitle = "";
            string manager = getManager(employee);
            using (PrincipalContext context = new PrincipalContext(ContextType.Domain))
            {
                using (UserPrincipal managerUser = UserPrincipal.FindByIdentity(context, manager))
                {
                    manager = managerUser.DisplayName;
                    managerTitle = getJobTitle(managerUser);
                    if (managerTitle.Equals("Director"))
                    {
                        director[0] = manager;
                        director[1] = managerUser.SamAccountName;
                        return director;
                    }
                    else
                    {
                        director = findDirector(managerUser);
                    }
                }
            }

            return director;
        }

        public static List<string> getPersonel(string userID)
        {
            List<string> personelnames = new List<string>();

            using (PrincipalContext context = new PrincipalContext(ContextType.Domain))
            {
                using (UserPrincipal user = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, userID))
                {
                    DirectoryEntry ManagerEntry = (DirectoryEntry)user.GetUnderlyingObject();
                    for (var i = 0; i < ManagerEntry.Properties["directReports"].Count; i++)
                    {
                        using (UserPrincipal emp = UserPrincipal.FindByIdentity(context, IdentityType.DistinguishedName, ManagerEntry.Properties["directReports"][i].ToString()))
                        {
                            personelnames.Add(emp.SamAccountName);
                        }
                    }
                }
            }
            return personelnames;
        }

        public static string getJobTitle(UserPrincipal user)
        {
            string jobTitle = "";
            DirectoryEntry userInEntry = (DirectoryEntry)user.GetUnderlyingObject();
            PropertyValueCollection titles = userInEntry.Properties["title"];

            Debug.WriteLine(userInEntry.Properties["title"]);
            jobTitle = userInEntry.Properties["title"][0].ToString();

            return jobTitle;
        }

        public static string getUserID(UserPrincipal user)
        {
            string userId = "";
            DirectoryEntry userInEntry = (DirectoryEntry)user.GetUnderlyingObject();
            userId = userInEntry.Properties["sAMAccountName"][0].ToString();

            return userId;
        }

        public static List<string> GetListOfAdUsersByProperty(string domainName, string groupName, string property, string filter = "")
        {
            DirectoryEntry entry = new DirectoryEntry("LDAP://OU=WSI,DC=" + domainName + ",DC=gov");
            DirectorySearcher search = new DirectorySearcher(entry);
            List<string> results = new List<string>();
            string query = "(&(objectCategory=person)(objectClass=user)(memberOf=*))";
            search.Filter = query;
            search.PropertiesToLoad.Add(property);
            search.PropertiesToLoad.Add("sAMAccountName");

            System.DirectoryServices.SearchResultCollection mySearchResultColl = search.FindAll();
            foreach (SearchResult result in mySearchResultColl)
            {
                foreach (string prop in result.Properties[property])
                {
                    //if(result.Path.Contains("CN=WSI"))
                    if (prop.Equals(groupName) && !result.Path.Contains("Disabled Accounts") && result.Path.Contains(filter))
                    {
                        results.Add(result.Properties["sAMAccountName"][0].ToString());
                    }
                }
            }

            return results;
        }
        public static List<ADInfo> GetAllADForCompany()
        {
            DirectoryEntry entry = new DirectoryEntry("LDAP://OU=WSI,DC=nd,DC=gov");
            DirectorySearcher search = new DirectorySearcher(entry);
            List<ADInfo> results = new List<ADInfo>();
            string query = "(&(objectCategory=person)(objectClass=user)(memberOf=*))";
            search.Filter = query;
            search.PropertiesToLoad.Add(ADUserProperties.COMPANY);
            search.PropertiesToLoad.Add(ADUserProperties.DEPARTMENT);
            search.PropertiesToLoad.Add(ADUserProperties.DISPLAYNAME);
            search.PropertiesToLoad.Add(ADUserProperties.TITLE);
            search.PropertiesToLoad.Add("sAMAccountName");

            System.DirectoryServices.SearchResultCollection mySearchResultColl = search.FindAll();
            foreach (SearchResult result in mySearchResultColl)
            {
                foreach (string prop in result.Properties[ADUserProperties.COMPANY])
                {
                    if (prop.Equals(ADUserProperties.WSIName) && result.Properties[ADUserProperties.TITLE].Count > 0)
                    {
                        ADInfo ad = new ADInfo();
                        ad.Username = result.Properties["sAMAccountName"][0].ToString(); ;
                        ad.DisplayName = result.Properties[ADUserProperties.DISPLAYNAME][0].ToString();
                        if (result.Properties[ADUserProperties.TITLE].Count > 0)
                            ad.Title = result.Properties[ADUserProperties.TITLE][0].ToString();
                        else
                            ad.Title = "";
                        if (result.Properties[ADUserProperties.DEPARTMENT].Count > 0)
                            ad.Department = result.Properties[ADUserProperties.DEPARTMENT][0].ToString();
                        else
                            ad.Department = "";
                        results.Add(ad);
                    }
                }
            }

            return results;
        }

        public static string[] inHierarchy(List<string> managernames, string employee)
        {
            bool results = false;
            PrincipalContext context = new PrincipalContext(ContextType.Domain);
            UserPrincipal u = new UserPrincipal(context);
            UserPrincipal employeeCheck = UserPrincipal.FindByIdentity(context, employee);
            string employeeTitle = getJobTitle(employeeCheck);
            string nextManager = "";
            string manager2 = "";

            string[] manager = new string[2];

            if(employeeTitle == "Director")
			{
                foreach (string name in managernames)
                {
                    UserPrincipal directorcheck = UserPrincipal.FindByIdentity(context, name);
                    manager2 = getManager(directorcheck);

                    if (manager2 == employee)
                    {
                        manager[0] = findName(name);
                        manager[1] = getEmail(name);
                        results = true;
                        break;
                    }
                }
            }
			else
            {
				UserPrincipal mangercheck = UserPrincipal.FindByIdentity(context, employee);
				nextManager = getManager(mangercheck);
                

                foreach (string name in managernames)
                { 
						results = personelHierarchyIteration(nextManager, name);
                        if (results == true)
                        {
                            manager[0] = findName(name);
                            manager[1] = getEmail(name);
                            break;
                        }
				}
			}
            return manager;
        }

        public static bool personelHierarchyIteration(string nextManager, string name)
        {
            bool results = false;
            string manager = "";
            PrincipalContext context = new PrincipalContext(ContextType.Domain);
            UserPrincipal u = new UserPrincipal(context);
            UserPrincipal nextmanagerPrincipal = UserPrincipal.FindByIdentity(context, nextManager);

            if(nextmanagerPrincipal.SamAccountName == name)
            {
				results = true;
			}
			if (results == false && getJobTitle(nextmanagerPrincipal) != "Director" )
            {
                nextManager = getManager(nextmanagerPrincipal);
				results = personelHierarchyIteration(nextManager, name);
			}          

			return results;
        }
	}
    public class ADInfo
    {
        public string Username;
        public string DisplayName;
        public string Title;
        public string Department;
    }
}
