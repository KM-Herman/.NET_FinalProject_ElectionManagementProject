<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ApproveCandidates.aspx.cs" Inherits="ElectionManagementSystem.ApproveCandidates" %>

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
            max-width: 800px;
            background: #ffffff;
            border-radius: 10px;
            box-shadow: 0 2px 5px rgba(0, 0, 0, 0.1);
            padding: 20px;
        }

        h1 {
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

        .vote-button {
            background-color: #28a745;
            color: white;
            padding: 8px 16px;
            border: none;
            border-radius: 4px;
            cursor: pointer;
            transition: background-color 0.3s;
        }

        .vote-button:hover {
            background-color: #218838;
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
            <h1>Approve Candidates</h1>
            <asp:GridView ID="gvElections" runat="server" AutoGenerateColumns="False" CssClass="table" OnRowCommand="gvElections_RowCommand">
                <Columns>
                    <asp:BoundField DataField="ElectionName" HeaderText="Election Name" />
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:Button ID="btnVote" runat="server" Text="Approve" CommandName="Vote" CommandArgument='<%# Eval("ElectionId") %>' CssClass="vote-button" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </form>
</body>
</html>
