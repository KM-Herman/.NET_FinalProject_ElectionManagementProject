using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace ElectionMGT
{
    public partial class ElectionManagement : System.Web.UI.Page
    {
        private readonly string connStr = ConfigurationManager.ConnectionStrings["MyDatabaseConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!PermissionHelper.HasPermission("ManageElections"))
            {
                Response.Redirect("~/AccessDenied.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadElections();
            }
        }

        /// <summary>
        /// Loads all elections into the GridView.
        /// </summary>
        private void LoadElections()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    string query = "SELECT ElectionId, ElectionName, Status FROM [Election] ORDER BY ElectionId DESC";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            gvElections.DataSource = dt;
                            gvElections.DataBind();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lblListMessage.Text = "Error loading elections: " + ex.Message;
                lblListMessage.CssClass = "text-danger";
                LoggingHelper.LogActivity(Convert.ToInt32(Session["UserId"]), "DB_ERROR", $"Failed to load elections: {ex.Message}");
            }
        }
        public string GetStatusCss(object status)
        {
            string statusStr = status.ToString();
            string cssClass = "badge badge-secondary";

            switch (statusStr)
            {
                case "Ongoing":
                    cssClass = "badge badge-success";
                    break;
                case "Upcoming":
                    cssClass = "badge badge-info";
                    break;
                case "Finished":
                    cssClass = "badge badge-danger";
                    break;
            }

            return cssClass;
        }

        protected void btnCreateElection_Click(object sender, EventArgs e)
        {
            if (!PermissionHelper.HasPermission("ManageElections"))
            {
                lblCreateMessage.Text = "Permission denied to create elections.";
                lblCreateMessage.CssClass = "text-danger";
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    string name = txtElectionName.Text.Trim();
                    string query = "INSERT INTO [Election] (ElectionName, Status) VALUES (@Name, 'Upcoming')";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Name", name);
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

                LoggingHelper.LogActivity(Convert.ToInt32(Session["UserId"]), "ELECTION_CREATED", $"New election '{txtElectionName.Text}' created.");
                lblCreateMessage.Text = "Election created successfully!";
                lblCreateMessage.CssClass = "alert alert-success";
                txtElectionName.Text = string.Empty; // Clear form

                LoadElections(); // Refresh grid
            }
            catch (Exception ex)
            {
                lblCreateMessage.Text = "Error creating election: " + ex.Message;
                lblCreateMessage.CssClass = "text-danger";
                LoggingHelper.LogActivity(Convert.ToInt32(Session["UserId"]), "DB_ERROR", $"Failed to create election: {ex.Message}");
            }
        }

        protected void gvElections_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (!PermissionHelper.HasPermission("ManageElections"))
            {
                lblListMessage.Text = "Permission denied for this action.";
                lblListMessage.CssClass = "text-danger";
                return;
            }

            int electionId = Convert.ToInt32(e.CommandArgument);
            string newStatus = string.Empty;
            string logType = string.Empty;
            string successMessage = string.Empty;
            string updateQuery = "UPDATE [Election] SET Status = @Status WHERE ElectionId = @ElectionId";
            int adminId = Convert.ToInt32(Session["UserId"]);

            switch (e.CommandName)
            {
                case "SetOngoing":
                    newStatus = "Ongoing";
                    logType = "ELECTION_STATUS_OPENED";
                    successMessage = "Election status set to **Ongoing** (Open).";
                    break;

                case "SetUpcoming":
                    newStatus = "Upcoming";
                    logType = "ELECTION_STATUS_PENDING";
                    successMessage = "Election status set to **Upcoming** (Pending).";
                    break;

                case "SetFinished":
                    newStatus = "Finished";
                    logType = "ELECTION_STATUS_CLOSED";
                    successMessage = "Election status set to **Finished** (Closed).";
                    break;

                case "DeleteElection":
                    DeleteElection(electionId);
                    return;

                default:
                    return;
            }

            if (!string.IsNullOrEmpty(newStatus))
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(connStr))
                    using (SqlCommand cmd = new SqlCommand(updateQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@Status", newStatus);
                        cmd.Parameters.AddWithValue("@ElectionId", electionId);
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }

                    LoggingHelper.LogActivity(adminId, logType, $"Election ID {electionId} status manually set to {newStatus}.");
                    lblListMessage.Text = successMessage;
                    lblListMessage.CssClass = "alert alert-success";
                    LoadElections();
                }
                catch (Exception ex)
                {
                    lblListMessage.Text = $"Error updating election status: {ex.Message}";
                    lblListMessage.CssClass = "text-danger";
                    LoggingHelper.LogActivity(adminId, "DB_ERROR", $"Failed to update election status for ID {electionId}: {ex.Message}");
                }
            }
        }
        private void DeleteElection(int electionId)
        {
            int adminId = Convert.ToInt32(Session["UserId"]);

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand("DELETE FROM Votes WHERE ElectionId = @Eid", conn))
                    {
                        cmd.Parameters.AddWithValue("@Eid", electionId);
                        cmd.ExecuteNonQuery();
                    }

                    using (SqlCommand cmd = new SqlCommand("DELETE FROM Candidate WHERE ElectionId = @Eid", conn))
                    {
                        cmd.Parameters.AddWithValue("@Eid", electionId);
                        cmd.ExecuteNonQuery();
                    }

                    using (SqlCommand cmd = new SqlCommand("DELETE FROM Election WHERE ElectionId = @Eid", conn))
                    {
                        cmd.Parameters.AddWithValue("@Eid", electionId);
                        cmd.ExecuteNonQuery();
                    }
                }

                LoggingHelper.LogActivity(adminId, "ELECTION_DELETED", $"Election ID {electionId} deleted successfully.");
                lblListMessage.Text = "Election and all related data deleted successfully.";
                lblListMessage.CssClass = "alert alert-success";
                LoadElections();
            }
            catch (Exception ex)
            {
                lblListMessage.Text = $"Error deleting election: {ex.Message}";
                lblListMessage.CssClass = "text-danger";
                LoggingHelper.LogActivity(adminId, "DB_ERROR", $"Failed to delete election ID {electionId}: {ex.Message}");
            }
        }
    }
}