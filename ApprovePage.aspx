<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ApprovePage.aspx.cs" Inherits="ElectionManagementSystem.ApprovePage" %>

<!DOCTYPE html>
<html lang="en">
<head runat="server">
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Approve Candidates</title>
    <style>
        /* General Styling */
        body {
            font-family: Arial, sans-serif;
            background-color: #f4f4f9;
            margin: 0;
            padding: 0;
        }

        /* Navigation Bar */
        .navbar {
            background-color: #374151;
            padding: 10px 0;
            display: flex;
            justify-content: center;
            box-shadow: 0 2px 5px rgba(0, 0, 0, 0.2);
            margin-bottom: 20px;
        }

        .navbar a {
            color: white;
            text-decoration: none;
            padding: 10px 20px;
            margin: 0 10px;
            font-weight: bold;
            border-radius: 5px;
            transition: background-color 0.3s;
        }

        .navbar a:hover {
            background-color: #0056b3;
        }

        /* Container */
        .container {
            margin: 20px auto;
            max-width: 900px;
            background: #ffffff;
            border-radius: 10px;
            box-shadow: 0 2px 5px rgba(0, 0, 0, 0.1);
            padding: 20px;
        }

        h2 {
            text-align: center;
            color: #333;
            margin-bottom: 20px;
        }

        /* GridView Styling */
        .table {
            width: 100%;
            border-collapse: collapse;
            margin-top: 20px;
        }

        .table th, .table td {
            border: 1px solid #ddd;
            padding: 10px;
            text-align: left;
        }

        .table th {
            background-color: #007bff;
            color: white;
        }

        .action-buttons {
            display: flex;
            gap: 10px;
        }

        .action-button {
            background-color: #28a745;
            color: white;
            padding: 6px 12px;
            border: none;
            border-radius: 4px;
            cursor: pointer;
            transition: background-color 0.3s;
        }

        .action-button.reject {
            background-color: #dc3545;
        }

        .action-button:hover {
            opacity: 0.8;
        }
    </style>
</head>
<body>
    <!-- Navigation Bar -->
    <div class="navbar">
        <a href="AdminPage.aspx" class="dashboard-button">Home</a>
        <a href="ApproveCandidates.aspx" class="dashboard-button">Approve Candidates</a>
        <a href="ManageElection.aspx" class="dashboard-button">Manage Election</a>
        <a href="SendNotifications.aspx" class="dashboard-button">Send Notifications</a>
        <a href="ElectionResultsAdmin.aspx" class="dashboard-button">View Election Results</a>
    </div>

    <!-- Main Content -->
    <form id="form1" runat="server">
        <div class="container">
            <h2>Approve Candidates</h2>

            <!-- Election Name Label -->
            <asp:Label ID="lblElectionName" runat="server" Text="Election Details" Font-Bold="True" Font-Size="Large"></asp:Label>
            <br /><br />

            <!-- GridView for Candidates -->
            <asp:GridView ID="gvCandidates" runat="server" AutoGenerateColumns="False" CssClass="table" OnRowCommand="gvCandidates_RowCommand">
                <Columns>
                    <asp:BoundField DataField="CandidateName" HeaderText="Candidate Name" SortExpression="CandidateName" />
                    <asp:BoundField DataField="Slogan" HeaderText="Slogan" SortExpression="Slogan" />
                    <asp:TemplateField>
                        <ItemTemplate>
                            <div class="action-buttons">
                                <asp:Button ID="btnApprove" runat="server" Text="Approve" CommandName="Approve" CommandArgument='<%# Eval("CandidateId") %>' CssClass="action-button" OnClientClick="return confirm('Are you sure you want to approve this candidate?');" />
                                <asp:Button ID="btnReject" runat="server" Text="Reject" CommandName="Reject" CommandArgument='<%# Eval("CandidateId") %>' CssClass="action-button reject" OnClientClick="return confirm('Are you sure you want to reject this candidate?');" />
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </form>
</body>
</html>
