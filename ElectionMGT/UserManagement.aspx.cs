using System;
using System.Collections.Generic; // Ensure this is present for PermissionHelper
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ElectionMGT
{
    public partial class UserManagement : System.Web.UI.Page
    {
        private readonly string connStr = ConfigurationManager.ConnectionStrings["MyDatabaseConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Role"] == null || Session["Role"].ToString() != "Admin")
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            bool canManageVoters = PermissionHelper.HasPermission("ManageVoters");
            pnlManageVoters.Visible = canManageVoters;
            lblNoVoterPermission.Visible = !canManageVoters;

            bool canManageRoles = PermissionHelper.HasPermission("ManageRoles");
            gvUserRoles.Visible = canManageRoles;
            lblNoRolePermission.Visible = !canManageRoles;

            bool canManageCandidates = PermissionHelper.HasPermission("ManageCandidates");
            gvCandidates.Visible = canManageCandidates;
            lblNoCandidatePermission.Visible = !canManageCandidates;

            if (!IsPostBack && canManageVoters)
            {
                if (canManageVoters)
                {
                    LoadVotersData();
                }

                if (canManageRoles)
                {
                    LoadUsersForRoleAssignment();
                }

                if (canManageCandidates)
                {
                    LoadCandidatesData();
                }
            }
        }

        private void LoadVotersData()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    // Query to select Users who are 'Voter' and join with Voter table 
                    // (Assuming you have a way to track voter registration/approval status, 
                    // though your table structure doesn't show a separate Voter table status.
                    // This query fetches all users who have the 'Voter' role for now.)
                    string query = @"
                        SELECT 
                            u.UserId, u.FirstName, u.LastName, u.Email 
                        FROM 
                            Users u
                        JOIN
                            [Role] r ON u.RoleId = r.RoleId
                        WHERE 
                            r.RoleName = 'Voter'
                        ORDER BY
                            u.LastName";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            gvVoters.DataSource = dt;
                            gvVoters.DataBind();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lblVoterMessage.Text = "Error loading voter data: " + ex.Message;
                lblVoterMessage.CssClass = "text-danger";
                LoggingHelper.LogActivity(Convert.ToInt32(Session["UserId"]), "ERROR", $"Failed to load voters: {ex.Message}");
            }
        }

         
        private DataTable LoadRoles()
        {
            DataTable dtRoles = new DataTable();
            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    string query = "SELECT RoleId, RoleName FROM [Role] ORDER BY RoleName";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dtRoles);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LoggingHelper.LogActivity(null, "DB_ERROR", $"Failed to load roles: {ex.Message}");
            }
            return dtRoles;
        }


        private void LoadCandidatesData()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    // Query to fetch candidate details: Candidate table data + User Name + Election Name
                    string query = @"
                SELECT 
                    c.CandidateId, c.UserId, c.ElectionId, c.Slogan, c.Manifesto, c.Approved,
                    u.FirstName, u.LastName, e.ElectionName
                FROM 
                    Candidate c
                JOIN 
                    Users u ON c.UserId = u.UserId
                JOIN
                    Election e ON c.ElectionId = e.ElectionId
                ORDER BY
                    c.Approved, e.ElectionName, u.LastName";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);

                            // Add a calculated column for full name display
                            dt.Columns.Add("FullName", typeof(string), "FirstName + ' ' + LastName");

                            gvCandidates.DataSource = dt;
                            gvCandidates.DataBind();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lblCandidateMessage.Text = "Error loading candidate data: " + ex.Message;
                lblCandidateMessage.CssClass = "text-danger";
                LoggingHelper.LogActivity(Convert.ToInt32(Session["UserId"]), "DB_ERROR", $"Failed to load candidates: {ex.Message}");
            }
        }

        private void LoadUsersForRoleAssignment()
        {
            try
            {
                DataTable dtRoles = LoadRoles();
                if (dtRoles.Rows.Count == 0) return;
                ViewState["AvailableRoles"] = dtRoles;

                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    string query = @"
                SELECT 
                    u.UserId, u.FirstName, u.LastName, u.Email, u.RoleId, r.RoleName
                FROM 
                    Users u
                JOIN 
                    [Role] r ON u.RoleId = r.RoleId
                ORDER BY
                    u.LastName";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable dtUsers = new DataTable();
                            da.Fill(dtUsers);

                            // Add a calculated column for full name display
                            dtUsers.Columns.Add("FullName", typeof(string), "FirstName + ' ' + LastName");

                            gvUserRoles.DataSource = dtUsers;
                            gvUserRoles.DataBind();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lblRoleMessage.Text = "Error loading user data for role assignment: " + ex.Message;
                lblRoleMessage.CssClass = "text-danger";
                LoggingHelper.LogActivity(Convert.ToInt32(Session["UserId"]), "DB_ERROR", $"Failed to load users for role assignment: {ex.Message}");
            }
        }


        protected void gvUserRoles_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "UpdateRole")
            {
                if (!PermissionHelper.HasPermission("ManageRoles"))
                {
                    lblRoleMessage.Text = "Permission denied for role update.";
                    lblRoleMessage.CssClass = "text-danger";
                    return;
                }

                int userId = Convert.ToInt32(e.CommandArgument);

                // Find the row where the command was executed
                GridViewRow row = (GridViewRow)((Control)e.CommandSource).NamingContainer;
                DropDownList ddlNewRole = (DropDownList)row.FindControl("ddlNewRole");

                if (ddlNewRole == null) return;

                int newRoleId = Convert.ToInt32(ddlNewRole.SelectedValue);
                string newRoleName = ddlNewRole.SelectedItem.Text;
                int adminId = Convert.ToInt32(Session["UserId"]);

                try
                {
                    using (SqlConnection conn = new SqlConnection(connStr))
                    {
                        string updateQuery = "UPDATE Users SET RoleId = @NewRoleId WHERE UserId = @UserId";

                        using (SqlCommand cmd = new SqlCommand(updateQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@NewRoleId", newRoleId);
                            cmd.Parameters.AddWithValue("@UserId", userId);

                            conn.Open();
                            cmd.ExecuteNonQuery();
                        }
                    }

                    // Log the activity
                    LoggingHelper.LogActivity(adminId, "ROLE_UPDATED", $"User ID {userId} role changed to '{newRoleName}'.");

                    lblRoleMessage.Text = $"Role updated successfully for User ID {userId} to **{newRoleName}**.";
                    lblRoleMessage.CssClass = "alert alert-success";

                    // Reload the grid to reflect changes
                    LoadUsersForRoleAssignment();
                }
                catch (Exception ex)
                {
                    lblRoleMessage.Text = $"Error updating role: {ex.Message}";
                    lblRoleMessage.CssClass = "text-danger";
                    LoggingHelper.LogActivity(adminId, "DB_ERROR", $"Failed to update role for User ID {userId}: {ex.Message}");
                }
            }
        }

        protected void gvCandidates_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int candidateId = Convert.ToInt32(e.CommandArgument);
            int adminId = Convert.ToInt32(Session["UserId"]);

            // Security check
            if (!PermissionHelper.HasPermission("ManageCandidates"))
            {
                lblCandidateMessage.Text = "Permission denied for this action.";
                lblCandidateMessage.CssClass = "text-danger";
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    if (e.CommandName == "ApproveCandidate")
                    {
                        string approveQuery = "UPDATE Candidate SET Approved = 1 WHERE CandidateId = @CandidateId";
                        using (SqlCommand cmd = new SqlCommand(approveQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@CandidateId", candidateId);
                            cmd.ExecuteNonQuery();
                        }

                        // Log the activity
                        LoggingHelper.LogActivity(adminId, "CANDIDATE_APPROVED", $"Candidate ID {candidateId} approved to run.");

                        lblCandidateMessage.Text = $"Candidate ID {candidateId} approved successfully.";
                        lblCandidateMessage.CssClass = "alert alert-success";
                    }
                    else if (e.CommandName == "DeleteCandidate")
                    {
                        // WARNING: Deleting a candidate requires cascading deletes or manual deletion 
                        // from the Votes table first due to foreign key constraints.

                        string deleteVotesQuery = "DELETE FROM Votes WHERE CandidateId = @CandidateId";
                        using (SqlCommand cmd = new SqlCommand(deleteVotesQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@CandidateId", candidateId);
                            cmd.ExecuteNonQuery();
                        }

                        string deleteCandidateQuery = "DELETE FROM Candidate WHERE CandidateId = @CandidateId";
                        using (SqlCommand cmd = new SqlCommand(deleteCandidateQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@CandidateId", candidateId);
                            cmd.ExecuteNonQuery();
                        }

                        LoggingHelper.LogActivity(adminId, "CANDIDATE_DELETED", $"Candidate ID {candidateId} deleted.");

                        lblCandidateMessage.Text = $"Candidate ID {candidateId} and associated votes deleted successfully.";
                        lblCandidateMessage.CssClass = "alert alert-success";
                    }
                }
                LoadCandidatesData();
            }
            catch (Exception ex)
            {
                lblCandidateMessage.Text = $"An error occurred during candidate action: {ex.Message}";
                lblCandidateMessage.CssClass = "text-danger";
                LoggingHelper.LogActivity(adminId, "ERROR", $"Candidate action failed for ID {candidateId}: {ex.Message}");
            }
        }

        protected void gvUserRoles_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // 1. Find the DropDownList control in the row
                DropDownList ddlNewRole = (DropDownList)e.Row.FindControl("ddlNewRole");

                if (ddlNewRole != null && ViewState["AvailableRoles"] is DataTable dtRoles)
                {

                    // 3. Bind the roles data to the DropDownList
                    ddlNewRole.DataSource = dtRoles;
                    ddlNewRole.DataTextField = "RoleName";
                    ddlNewRole.DataValueField = "RoleId";
                    ddlNewRole.DataBind();

                    // 4. Set the current role as the selected value
                    DataRowView dr = (DataRowView)e.Row.DataItem;
                    ddlNewRole.SelectedValue = dr["RoleId"].ToString();
                }
            }
        }

        protected void gvVoters_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (!PermissionHelper.HasPermission("ManageVoters"))
            {
                lblVoterMessage.Text = "Permission denied for this action.";
                lblVoterMessage.CssClass = "text-danger";
                return;
            }

            int userId = Convert.ToInt32(e.CommandArgument);

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    if (e.CommandName == "ApproveUser")
                    {

                        LoggingHelper.LogActivity(Convert.ToInt32(Session["UserId"]), "VOTER_APPROVED", $"Voter UserID {userId} approved.");

                        lblVoterMessage.Text = $"User ID {userId} approved successfully.";
                        lblVoterMessage.CssClass = "text-success";
                    }
                    else if (e.CommandName == "DeleteUser")
                    {
                        string deleteQuery = "DELETE FROM Users WHERE UserId = @UserId";

                        using (SqlCommand cmd = new SqlCommand(deleteQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@UserId", userId);
                            cmd.ExecuteNonQuery();
                        }

                        LoggingHelper.LogActivity(Convert.ToInt32(Session["UserId"]), "USER_DELETED", $"User UserID {userId} deleted.");

                        lblVoterMessage.Text = $"User ID {userId} and associated data deleted successfully.";
                        lblVoterMessage.CssClass = "text-success";
                    }
                }
                LoadVotersData();
            }
            catch (SqlException sqlex)
            {
                if (sqlex.Number == 547)
                {
                    lblVoterMessage.Text = $"Cannot delete User ID {userId}. Please ensure all related records (e.g., Votes, Candidates, Notifications) are deleted first (Foreign Key Constraint).";
                }
                else
                {
                    lblVoterMessage.Text = $"Database Error: {sqlex.Message}";
                }
                lblVoterMessage.CssClass = "text-danger";
            }
            catch (Exception ex)
            {
                lblVoterMessage.Text = $"An unexpected error occurred: {ex.Message}";
                lblVoterMessage.CssClass = "text-danger";
            }
        }
    }
}