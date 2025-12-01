using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web;

namespace ElectionMGT
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            lblErrorMessage.Visible = false;
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text.Trim();
            string connStr = ConfigurationManager.ConnectionStrings["MyDatabaseConnection"].ConnectionString;

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    string query = @"
                    SELECT 
                        UserId, Role, Password, Salt 
                    FROM 
                        Users 
                    WHERE 
                        Email = @Email";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Email", email);

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    int userId = 0;
                    string role = string.Empty;
                    string storedPassword = string.Empty;
                    string salt = string.Empty;

                    if (reader.Read())
                    {
                        userId = Convert.ToInt32(reader["UserId"]);
                        role = reader["Role"].ToString();
                        storedPassword = reader["Password"].ToString();
                        salt = reader["Salt"].ToString();
                    }
                    reader.Close(); // Ensure the reader is closed

                    if (userId > 0)
                    {
                        // Hash the entered password and compare with the stored password
                        string enteredPasswordHash = HashPassword(password, salt);
                        if (storedPassword == enteredPasswordHash)
                        {
                            // Check if the user is a candidate
                            bool isCandidate = CheckIfCandidate(userId, conn);

                            // Successful login
                            Session["UserId"] = userId;
                            Session["Role"] = role;

                            // Redirect based on role
                            if (isCandidate)
                            {
                                Response.Redirect("CandidatePage.aspx");
                            }
                            else if (role == "Admin")
                            {
                                Response.Redirect("AdminPage.aspx");
                            }
                            else if (role == "Voter")
                            {
                                Response.Redirect("VoterPage.aspx");
                            }
                        }
                        else
                        {
                            lblErrorMessage.Text = "Invalid email or password.";
                            lblErrorMessage.ForeColor = System.Drawing.Color.Red;
                            lblErrorMessage.Visible = true;
                        }
                    }
                    else
                    {
                        lblErrorMessage.Text = "Invalid email or password.";
                        lblErrorMessage.ForeColor = System.Drawing.Color.Red;
                        lblErrorMessage.Visible = true;
                    }
                }
            }
            catch (Exception ex)
            {
                // Display exception details on the page for debugging
                lblErrorMessage.Text = $"An error occurred during login: {ex.Message}";
                lblErrorMessage.ForeColor = System.Drawing.Color.Red;
                lblErrorMessage.Visible = true;

                // Optionally, log the stack trace or more details in a log file
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        private bool CheckIfCandidate(int userId, SqlConnection conn)
        {
            string candidateQuery = "SELECT COUNT(*) FROM Candidate WHERE UserId = @UserId";
            using (SqlCommand cmd = new SqlCommand(candidateQuery, conn))
            {
                cmd.Parameters.AddWithValue("@UserId", userId);
                int candidateCount = Convert.ToInt32(cmd.ExecuteScalar());
                return candidateCount > 0; // If the count is greater than 0, the user is a candidate
            }
        }

        private string HashPassword(string password, string salt)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var saltedPassword = password + salt;
                var bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(saltedPassword));
                return Convert.ToBase64String(bytes);
            }
        }
    }
}
