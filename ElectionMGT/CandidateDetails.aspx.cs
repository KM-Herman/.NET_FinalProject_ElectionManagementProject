using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ElectionManagementSystem
{
    public partial class CandidateDetails : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null)
            {
                Response.Redirect("Login.aspx");
            }

            if (!IsPostBack)
            {
                int candidateId = Convert.ToInt32(Request.QueryString["CandidateId"]);
                LoadCandidateDetails(candidateId);
            }
        }
        private void LoadCandidateDetails(int candidateId)
        {
            string connStr = ConfigurationManager.ConnectionStrings["MyDatabaseConnection"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = @"
                SELECT 
                    U.FirstName + ' ' + U.LastName AS CandidateName,
                    C.Slogan,
                    C.Manifesto
                FROM Candidate C
                INNER JOIN Users U ON C.UserId = U.UserId
                WHERE C.CandidateId = @CandidateId";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@CandidateId", candidateId);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    lblCandidateDetails.Text = $@"
                        <h2>{reader["CandidateName"]}</h2>
                        <p>{reader["Slogan"]}</p>
                        <p>{reader["Manifesto"]}</p>";
                }
            }
        }

        protected void Vote_Click(object sender, EventArgs e)
        {
            int candidateId = Convert.ToInt32(Request.QueryString["CandidateId"]);
            int electionId = Convert.ToInt32(Request.QueryString["ElectionId"]);
            int userId = Convert.ToInt32(Session["UserId"]);

            string connStr = ConfigurationManager.ConnectionStrings["MyDatabaseConnection"].ConnectionString;

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    // Check if the user has already voted in this election
                    string checkVoteQuery = @"
                SELECT COUNT(*)
                FROM Votes
                WHERE UserId = @UserId AND ElectionId = @ElectionId";

                    using (SqlCommand checkCmd = new SqlCommand(checkVoteQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@UserId", userId);
                        checkCmd.Parameters.AddWithValue("@ElectionId", electionId);

                        int voteCount = (int)checkCmd.ExecuteScalar();
                        if (voteCount > 0)
                        {
                            lblMessage.CssClass = "error";
                            lblMessage.Text = "You have already voted in this election. You cannot vote again.";
                            lblMessage.Visible = true;
                            return;
                        }
                    }

                    // Insert the vote into the Votes table
                    string insertVoteQuery = @"
                INSERT INTO Votes (UserId, CandidateId, ElectionId, VoteTime)
                VALUES (@UserId, @CandidateId, @ElectionId, @VoteTime)";

                    using (SqlCommand insertCmd = new SqlCommand(insertVoteQuery, conn))
                    {
                        insertCmd.Parameters.AddWithValue("@UserId", userId);
                        insertCmd.Parameters.AddWithValue("@CandidateId", candidateId);
                        insertCmd.Parameters.AddWithValue("@ElectionId", electionId);
                        insertCmd.Parameters.AddWithValue("@VoteTime", DateTime.Now);

                        insertCmd.ExecuteNonQuery();
                    }
                }

                lblMessage.CssClass = "message";
                lblMessage.Text = "Your vote has been successfully cast!";
            }
            catch (Exception ex)
            {
                lblMessage.CssClass = "error";
                lblMessage.Text = "Error while casting vote: " + ex.Message;
            }

            lblMessage.Visible = true;
        }


    }
}
