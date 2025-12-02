using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;

namespace ElectionMGT
{

    public static class LoggingHelper
    {
        private static readonly string connStr = ConfigurationManager.ConnectionStrings["MyDatabaseConnection"].ConnectionString;
        public static void LogActivity(int? userId, string activityType, string details)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    string logQuery = @"
                        INSERT INTO [ActivityLog] (UserId, ActivityType, Details, Timestamp)
                        VALUES (@UserId, @ActivityType, @Details, GETDATE())";

                    using (SqlCommand logCmd = new SqlCommand(logQuery, conn))
                    {
                        // Handle nullable UserId parameter
                        if (userId.HasValue && userId.Value > 0)
                        {
                            logCmd.Parameters.AddWithValue("@UserId", userId.Value);
                        }
                        else
                        {
                            logCmd.Parameters.AddWithValue("@UserId", DBNull.Value);
                        }

                        logCmd.Parameters.AddWithValue("@ActivityType", activityType);
                        logCmd.Parameters.AddWithValue("@Details", details);

                        conn.Open();
                        logCmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"CRITICAL ERROR: Failed to log activity to database. Details: {ex.Message}");
            }
        }
    }
}