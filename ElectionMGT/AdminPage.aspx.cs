using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace ElectionMGT
{
    public partial class AdminPage : System.Web.UI.Page
    {
        // Connection string
        private readonly string connStr = ConfigurationManager.ConnectionStrings["MyDatabaseConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Security check: Ensure only Admin can access this page (RBAC/PBAC)
            if (Session["Role"] == null || Session["Role"].ToString() != "Admin")
            {
                // Simple role check for now; for production, use the PermissionHelper!
                Response.Redirect("Login.aspx");
            }

            if (!IsPostBack)
            {
                if (!PermissionHelper.HasPermission("ViewDashboard")) 
                { Response.Redirect("AccessDenied.aspx"); }

                LoadDashboardData();
            }
        }

        private void LoadDashboardData()
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                // 1. Load Summary Counts
                LoadSummaryCounts(conn);

                // 2. Load Election Status and Details
                LoadElectionStatus(conn);

                // 3. Load Recent Activity Log
                LoadActivityLog(conn);
            }
        }

        private void LoadSummaryCounts(SqlConnection conn)
        {
            // Query 1: Total Voters (Users with RoleName 'Voter')
            // Assumes RoleName is stored in the [Role] table
            string voterQuery = "SELECT COUNT(u.UserId) FROM [Users] u JOIN [Role] r ON u.RoleId = r.RoleId WHERE r.RoleName = 'Voter'";
            lblTotalVoters.Text = GetScalarValue(voterQuery, conn).ToString();

            // Query 2: Total Candidates
            string candidateQuery = "SELECT COUNT(*) FROM [Candidate]";
            lblTotalCandidates.Text = GetScalarValue(candidateQuery, conn).ToString();

            // Query 3: Unread Notifications (Assumes IsRead column added to Notifications)
            string notificationQuery = "SELECT COUNT(*) FROM [Notifications] WHERE IsRead = 0";
            lblUnreadNotifications.Text = GetScalarValue(notificationQuery, conn).ToString();
        }

        private void LoadElectionStatus(SqlConnection conn)
        {
            // Query: Get the latest Active or Upcoming Election
            string electionQuery = @"
                SELECT TOP 1 
                    e.ElectionName, e.Status, 
                    (SELECT COUNT(CandidateId) FROM Candidate c WHERE c.ElectionId = e.ElectionId) AS RunningCandidates,
                    (SELECT COUNT(VoteId) FROM Votes v WHERE v.ElectionId = e.ElectionId) AS TotalVotes
                FROM 
                    [Election] e
                ORDER BY 
                    CASE e.Status
                        WHEN 'Ongoing' THEN 1 
                        WHEN 'Upcoming' THEN 2 
                        ELSE 3 
                    END,
                    e.ElectionId DESC";

            using (SqlCommand cmd = new SqlCommand(electionQuery, conn))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        // Set the status card color/text
                        string status = reader["Status"].ToString();
                        lblElectionStatus.Text = status;

                        // Update UI for Election Details panel
                        lblCurrentElectionName.Text = reader["ElectionName"].ToString();
                        lblCurrentElectionStatus.Text = status;
                        lblRunningCandidatesCount.Text = reader["RunningCandidates"].ToString();
                        lblTotalVotesCast.Text = reader["TotalVotes"].ToString();

                        pnlElectionDetails.Visible = true;
                        lblNoElection.Visible = false;
                    }
                    else
                    {
                        lblElectionStatus.Text = "No Elections";
                        pnlElectionDetails.Visible = false;
                        lblNoElection.Visible = true;
                    }
                }
            }
        }

        private void LoadActivityLog(SqlConnection conn)
        {
            // Query: Get the last 10 activities, ordered by newest first
            string logQuery = "SELECT TOP 10 Timestamp, ActivityType, Details FROM [ActivityLog] ORDER BY Timestamp DESC";

            using (SqlCommand cmd = new SqlCommand(logQuery, conn))
            {
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    gvActivityLog.DataSource = dt;
                    gvActivityLog.DataBind();
                }
            }
        }

        // Generic method to execute a scalar query (used for counts)
        private int GetScalarValue(string query, SqlConnection conn)
        {
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                // Ensure the connection is not closed by the caller, only used temporarily here
                object result = cmd.ExecuteScalar();
                return result != DBNull.Value && result != null ? Convert.ToInt32(result) : 0;
            }
        }
    }
}