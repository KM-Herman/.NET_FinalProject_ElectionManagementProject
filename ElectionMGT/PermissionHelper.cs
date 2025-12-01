using System.Collections.Generic;
using System.Web;

namespace ElectionMGT
{
    public static class PermissionHelper
    {
        public static bool HasPermission(string requiredPermission)
        {
            if (HttpContext.Current == null || HttpContext.Current.Session["Permissions"] == null)
            {
                return false;
            }

            // Retrieve the list of permissions from the session.
            var permissions = HttpContext.Current.Session["Permissions"] as List<string>;

            // Check for null or empty list
            if (permissions == null || permissions.Count == 0)
            {
                return false;
            }
            return permissions.Contains(requiredPermission);
        }

        public static bool IsAdmin()
        {
            // This relies on the RoleName being stored in the session during login.
            return HttpContext.Current != null &&
                   HttpContext.Current.Session["Role"] != null &&
                   HttpContext.Current.Session["Role"].ToString() == "Admin";
        }
    }
}