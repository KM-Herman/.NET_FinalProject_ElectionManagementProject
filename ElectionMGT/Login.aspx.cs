using System;
<<<<<<< HEAD
using System.Collections.Generic;
=======
>>>>>>> cbd02db6243c35730a7ac421f0eb3f9d9f3ba35c
using System.Configuration;
using System.Data.SqlClient;

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
<<<<<<< HEAD
                        u.UserId, u.RoleId, u.Password, u.Salt, r.RoleName
                    FROM 
                        Users u
                    JOIN 
                        [Role] r ON u.RoleId = r.RoleId
                    WHERE 
                        u.Email = @Email";
=======
                        UserId, Role, Password, Salt 
                    FROM 
                        Users 
                    WHERE 
                        Email = @Email";
>>>>>>> cbd02db6243c35730a7ac421f0eb3f9d9f3ba35c

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Email", email);

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    int userId = 0;
<<<<<<< HEAD
                    int roleId = 0;
                    string roleName = string.Empty;
=======
                    string role = string.Empty;
>>>>>>> cbd02db6243c35730a7ac421f0eb3f9d9f3ba35c
                    string storedPassword = string.Empty;
                    string salt = string.Empty;

                    if (reader.Read())
                    {
                        userId = Convert.ToInt32(reader["UserId"]);
<<<<<<< HEAD
                        roleId = Convert.ToInt32(reader["RoleId"]);
                        roleName = reader["RoleName"].ToString(); // Get the Role Name
=======
                        role = reader["Role"].ToString();
>>>>>>> cbd02db6243c35730a7ac421f0eb3f9d9f3ba35c
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
<<<<<<< HEAD
                            LoggingHelper.LogActivity(userId, "LOGIN_SUCCESS", $"User '{email}' logged in successfully.");
                            // Check if the user is a candidate
                            bool isCandidate = CheckIfCandidate(userId, conn);

                            List<string> userPermissions = GetUserPermissions(roleId, conn);
                            Session["Permissions"] = userPermissions;

                            // Successful login
                            Session["UserId"] = userId;
                            Session["Role"] = roleName;
=======
                            // Check if the user is a candidate
                            bool isCandidate = CheckIfCandidate(userId, conn);

                            // Successful login
                            Session["UserId"] = userId;
                            Session["Role"] = role;
>>>>>>> cbd02db6243c35730a7ac421f0eb3f9d9f3ba35c

                            // Redirect based on role
                            if (isCandidate)
                            {
                                Response.Redirect("CandidatePage.aspx");
                            }
<<<<<<< HEAD
                            else if (roleName == "Admin")
                            {
                                Response.Redirect("AdminPage.aspx");
                            }
                            else if (roleName == "Voter")
                            {
                                Response.Redirect("VoterPage.aspx");
                            }else{
                                Response.Redirect("DefaultPage.aspx");
                             }
                        }else{
                            LoggingHelper.LogActivity(userId, "LOGIN_FAILURE", $"Failed login attempt for existing user '{email}'.");

=======
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
>>>>>>> cbd02db6243c35730a7ac421f0eb3f9d9f3ba35c
                            lblErrorMessage.Text = "Invalid email or password.";
                            lblErrorMessage.ForeColor = System.Drawing.Color.Red;
                            lblErrorMessage.Visible = true;
                        }
<<<<<<< HEAD
                    }else{
                        LoggingHelper.LogActivity(null, "LOGIN_FAILURE", $"Failed login attempt '{email}'not found.");
=======
                    }
                    else
                    {
>>>>>>> cbd02db6243c35730a7ac421f0eb3f9d9f3ba35c
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
<<<<<<< HEAD


        private List<string> GetUserPermissions(int roleId, SqlConnection conn)
        {
            List<string> permissions = new List<string>();

            // The connection is already open when called from btnLogin_Click
            string query = @"
                SELECT 
                    P.Name 
                FROM 
                    RolePermission RP
                JOIN 
                    Permission P ON RP.PermissionId = P.PermissionId
                WHERE 
                    RP.RoleId = @RoleId";

            // Note: We create a new command, but use the existing open connection
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@RoleId", roleId);

                // We must use a separate DataReader because the main one was already closed.
                // Re-executing cmd.ExecuteReader() is fine on an open connection.
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        permissions.Add(reader["Name"].ToString());
                    }
                }
            }
            return permissions;
        }


=======
>>>>>>> cbd02db6243c35730a7ac421f0eb3f9d9f3ba35c
    }
}
