using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using System.Net.Mail;
using System.Net;

namespace ElectionMGT
{
    public partial class NotificationManagement : System.Web.UI.Page
    {
        private readonly string connStr = ConfigurationManager.ConnectionStrings["MyDatabaseConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!PermissionHelper.HasPermission("SendNotifications"))
            {
                Response.Redirect("~/AccessDenied.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadNotificationHistory();
            }
        }

        private void LoadNotificationHistory()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    string query = "SELECT TOP 50 NotificationId, Subject, Message, SentDate FROM [Notifications] ORDER BY SentDate DESC";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            gvNotifications.DataSource = dt;
                            gvNotifications.DataBind();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lblHistoryMessage.Text = "Error loading history: " + ex.Message;
                lblHistoryMessage.CssClass = "text-danger";
                LoggingHelper.LogActivity(Convert.ToInt32(Session["UserId"]), "DB_ERROR", $"Failed to load notification history: {ex.Message}");
            }
        }

        protected void btnSendNotification_Click(object sender, EventArgs e)
        {
            if (!PermissionHelper.HasPermission("SendNotifications"))
            {
                lblSendStatus.Text = "Permission denied to send notifications.";
                lblSendStatus.CssClass = "text-danger";
                return;
            }

            if (!Page.IsValid) return;

            string recipientGroup = ddlRecipientGroup.SelectedValue;
            string subject = txtSubject.Text.Trim();
            string body = txtMessageBody.Text.Trim();
            int adminId = Convert.ToInt32(Session["UserId"]);

            try
            {
                DataTable recipients = GetRecipientEmails(recipientGroup);

                if (recipients.Rows.Count == 0)
                {
                    lblSendStatus.Text = $"No users found in the selected group: {recipientGroup}.";
                    lblSendStatus.CssClass = "alert alert-warning";
                    return;
                }

                // 1. Save to Database (Central Notification Record)
                SaveNotificationToDatabase(adminId, subject, body);

                // 2. Send Emails (Optional - requires SMTP config)
                // Note: Sending mass emails synchronously like this is slow. Use an async worker/queue in production.

                /* foreach (DataRow row in recipients.Rows)
                {
                    string email = row["Email"].ToString();
                    string firstName = row["FirstName"].ToString();
                    SendEmail(email, firstName, subject, body);
                    // Optionally, log each individual send attempt if needed.
                }
                */

                // Logging and UI update
                LoggingHelper.LogActivity(adminId, "NOTIFICATION_SENT", $"Notification '{subject}' sent to {recipients.Rows.Count} users in the {recipientGroup} group.");
                lblSendStatus.Text = $"Notification sent to {recipients.Rows.Count} users successfully! (Email sending may require separate setup).";
                lblSendStatus.CssClass = "alert alert-success";

                // Clear and reload
                txtSubject.Text = string.Empty;
                txtMessageBody.Text = string.Empty;
                LoadNotificationHistory();
            }
            catch (Exception ex)
            {
                lblSendStatus.Text = $"An error occurred: {ex.Message}";
                lblSendStatus.CssClass = "text-danger";
                LoggingHelper.LogActivity(adminId, "ERROR", $"Notification send failure to {recipientGroup}: {ex.Message}");
            }
        }


        private DataTable GetRecipientEmails(string group)
        {
            string roleName = group;
            string query = "";

            if (group == "AllUsers")
            {
                query = "SELECT UserId, FirstName, Email FROM Users";
            }
            else
            {
                query = @"
            SELECT u.UserId, u.FirstName, u.Email 
            FROM Users u
            JOIN [Role] r ON u.RoleId = r.RoleId
            WHERE r.RoleName = @RoleName";
            }

            DataTable dtRecipients = new DataTable();
            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    if (group != "AllUsers")
                    {
                        cmd.Parameters.AddWithValue("@RoleName", roleName);
                    }
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dtRecipients);
                    }
                }
            }
            catch (Exception ex)
            {
                LoggingHelper.LogActivity(null, "DB_ERROR", $"Failed to retrieve recipient list for group {group}: {ex.Message}");
            }
            return dtRecipients;
        }

        private void SaveNotificationToDatabase(int userId, string subject, string body)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    string query = "INSERT INTO [Notifications] (UserId, Subject, Message, SentDate, IsRead) VALUES (@UserId, @Subject, @Message, GETDATE(), 0)";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        cmd.Parameters.AddWithValue("@Subject", subject);
                        cmd.Parameters.AddWithValue("@Message", body);
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                LoggingHelper.LogActivity(userId, "DB_ERROR", $"Failed to save notification to DB: {ex.Message}");
                throw; // Re-throw to inform the calling method of failure
            }
        }

        /* /// <summary>
        /// Sends an email (Requires correct SMTP configuration in Web.config).
        /// </summary>
        private void SendEmail(string recipientEmail, string recipientName, string subject, string body)
        {
            try
            {
                // Use SMTP configuration from your Web.config or app settings (similar to your Signup.aspx.cs)
                string smtpHost = ConfigurationManager.AppSettings["SmtpHost"];
                int smtpPort = Convert.ToInt32(ConfigurationManager.AppSettings["SmtpPort"]);
                string senderEmail = ConfigurationManager.AppSettings["SenderEmail"];
                string senderPassword = ConfigurationManager.AppSettings["SenderPassword"];

                using (SmtpClient smtpClient = new SmtpClient(smtpHost, smtpPort))
                {
                    smtpClient.UseDefaultCredentials = false;
                    smtpClient.EnableSsl = true;
                    smtpClient.Credentials = new NetworkCredential(senderEmail, senderPassword);

                    MailMessage mailMessage = new MailMessage
                    {
                        From = new MailAddress(senderEmail, "Election Management System Admin"),
                        Subject = subject,
                        Body = $"Dear {recipientName},\n\n{body}\n\nBest regards,\nElection Management Team",
                        IsBodyHtml = false
                    };
                    mailMessage.To.Add(recipientEmail);
                    smtpClient.Send(mailMessage);
                }
            }
            catch (Exception ex)
            {
                // Log error without stopping the entire notification process
                LoggingHelper.LogActivity(null, "EMAIL_SEND_FAIL", $"Failed to send email to {recipientEmail}. Subject: {subject}. Error: {ex.Message}");
            }
        }
        */

        // ... (btnSendNotification_Click implementation below) ...

        // ... (Helper methods for sending email/saving to DB) ...
    }
}