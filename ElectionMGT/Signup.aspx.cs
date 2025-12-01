using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Net.Mail;
using System.Net;

namespace ElectionMGT
{
    public partial class Signup : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            lblMessage.Visible = false;
        }

        protected void btnSignup_Click(object sender, EventArgs e)
        {
            string firstName = txtFirstName.Text.Trim();
            string lastName = txtLastName.Text.Trim();
            string email = txtEmail.Text.Trim();
            string phoneNumber = txtPhoneNumber.Text.Trim();
            string password = txtPassword.Text.Trim();
            string confirmPassword = txtConfirmPassword.Text.Trim();

            // Validate password match
            if (password != confirmPassword)
            {
                lblMessage.Text = "Passwords do not match!";
                lblMessage.CssClass = "alert-danger";
                lblMessage.Visible = true;
                return;
            }

            string salt = GenerateSalt();
            string hashedPassword = HashPassword(password, salt);

            // Use connection string from Web.config
            string connectionString = ConfigurationManager.ConnectionStrings["MyDatabaseConnection"].ConnectionString;
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("CreateUser", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        // Add parameters for the stored procedure
                        cmd.Parameters.AddWithValue("@FirstName", firstName);
                        cmd.Parameters.AddWithValue("@LastName", lastName);
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@Password", hashedPassword);
                        cmd.Parameters.AddWithValue("@Salt", salt);
                        cmd.Parameters.AddWithValue("@PhoneNumber", phoneNumber);

                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

                // Send confirmation email
                SendConfirmationEmail(firstName, email);

                // Redirect to login page after successful signup
                Response.Redirect("Login.aspx");
            }
            catch (Exception ex)
            {
                // Handle exception (e.g., log error or show message)
                Response.Write($"<script>alert('An error occurred: {ex.Message}');</script>");
            }
        }

        private void SendConfirmationEmail(string firstName, string email)
        {
            try
            {
                // Email configuration from Web.config
                string smtpHost = "smtp.gmail.com";
                int smtpPort = 587; // TLS port
                string senderEmail = ConfigurationManager.AppSettings["SenderEmail"];
                string senderPassword = ConfigurationManager.AppSettings["SenderPassword"];

                // Create SMTP client with explicit security configuration
                using (SmtpClient smtpClient = new SmtpClient(smtpHost, smtpPort))
                {
                    // Explicit security settings
                    smtpClient.UseDefaultCredentials = false;
                    smtpClient.EnableSsl = true;
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

                    // Create network credentials
                    smtpClient.Credentials = new NetworkCredential(senderEmail, senderPassword);

                    // Prepare mail message
                    MailMessage mailMessage = new MailMessage
                    {
                        From = new MailAddress(senderEmail, "Election Management System"),
                        Subject = "Signup Confirmation",
                        Body = $@"Dear {firstName},

Thank you for signing up for the Election Management System!

Your account has been successfully created. You can now log in using your email address.

Best regards,
Election Management Team",
                        IsBodyHtml = false
                    };
                    mailMessage.To.Add(email);

                    // Send the email
                    smtpClient.Send(mailMessage);
                }
            }
            catch (Exception ex)
            {
                // Detailed error logging
                string errorMessage = $"Email sending failed: {ex.Message}";

                // Include inner exception details if available
                if (ex.InnerException != null)
                {
                    errorMessage += $"\nInner Exception: {ex.InnerException.Message}";
                }

                // Log or display the error
                Response.Write($"<script>alert('{errorMessage}');</script>");

                // Optional: You might want to log this error to a file or database
                System.Diagnostics.Debug.WriteLine(errorMessage);
            }
        }

        private string GenerateSalt()
        {
            // Replace with your logic for generating a unique salt
            return Guid.NewGuid().ToString("N");
        }

        private string HashPassword(string password, string salt)
        {
            // Replace with actual hashing logic
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var saltedPassword = password + salt;
                var bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(saltedPassword));
                return Convert.ToBase64String(bytes);
            }
        }
    }
}