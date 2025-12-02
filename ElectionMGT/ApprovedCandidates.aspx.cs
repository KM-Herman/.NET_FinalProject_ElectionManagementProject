using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ElectionManagementSystem
{
    public partial class ApproveCandidates : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadElections();
            }
        }
        private void LoadElections()
        {
            string connStr = ConfigurationManager.ConnectionStrings["MyDatabaseConnection"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "SELECT ElectionId, ElectionName FROM Election WHERE Status = 'Pending'";
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                gvElections.DataSource = dt;
                gvElections.DataBind();
            }
        }

        protected void gvElections_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Vote")
            {
                int electionId = Convert.ToInt32(e.CommandArgument);
                Response.Redirect($"ApprovePage.aspx?ElectionId={electionId}");
            }
        }
    }
}
