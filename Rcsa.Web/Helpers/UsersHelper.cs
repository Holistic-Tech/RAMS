using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rcsa.Web.Helpers
{
    public class UsersHelper
    {
        /// <summary>
        /// Company Was Converted into A User Role.
        /// </summary>
        /// <returns></returns>
        public static bool isUserACompany()
        {
            string[] roles = System.Web.Security.Roles.GetRolesForUser();
            foreach (string role in  roles)
            {
                if(role == "User")
                {
                    return true;
                }
            }
            return false;
        }

        public static bool isUserAManager()
        {
            string[] roles = System.Web.Security.Roles.GetRolesForUser();
            foreach (string role in roles)
            {
                if (role == "Manager")
                {
                    return true;
                }
            }
            return false;
        }
    }
}