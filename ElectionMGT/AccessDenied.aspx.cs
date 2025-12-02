using System;
using System.Web;
using System.Web.UI;

namespace ElectionMGT
{
    public partial class AccessDenied : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string requestedUrl = Request.QueryString["url"];

            if (Session["UserId"] != null)
            {
                int userId = Convert.ToInt32(Session["UserId"]);
                string role = Session["Role"]?.ToString() ?? "N/A";

                string details = $"Unauthorized access attempt by User ID {userId} ({role}). Target: {requestedUrl}";
                LoggingHelper.LogActivity(userId, "ACCESS_DENIED", details);

                lblUserMessage.Text = $"Your current role is **{role}**. You cannot access {requestedUrl}.";
            }
            else
            {
                string details = $"Unauthorized access attempt by Anonymous user. Target: {requestedUrl}";
                LoggingHelper.LogActivity(null, "ACCESS_DENIED_UKNOWN", details);

                lblUserMessage.Text = "Please log in to access system resources.";
                // Redirect anonymous users to login page after a message, or simply show the page.
                hlDashboard.Visible = false;
            }

            if (Session["Role"]?.ToString() == "Admin")
            {
                hlDashboard.NavigateUrl = "~/AdminPage.aspx";
            }
            else if (Session["Role"]?.ToString() == "Voter")
            {
                hlDashboard.NavigateUrl = "~/VoterPage.aspx";
            }
            else if (Session["Role"]?.ToString() == "Candidate")
            {
                hlDashboard.NavigateUrl = "~/CandidatePage.aspx";
            }
        }
    }
}