<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CandidatePage.aspx.cs" Inherits="ElectionManagementSystem.CandidatePage" %>

<!DOCTYPE html>
<html lang="en">

<head runat="server">
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Voter Dashboard</title>
    <style>
        body {
            font-family: Arial, sans-serif;
            background-color: #f4f4f9;
            margin: 0;
            padding: 0;
            display: flex;
            justify-content: center;
            align-items: center;
            height: 100vh;
        }

        .dashboard-container {
            width: 400px;
            background-color: #fff;
            padding: 30px;
            border-radius: 8px;
            box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
            text-align: center;
        }

        .dashboard-title {
            font-size: 24px;
            font-weight: bold;
            margin-bottom: 20px;
            color: #333;
        }

        .dashboard-button {
            display: block;
            margin: 10px 0;
            padding: 12px;
            font-size: 16px;
            font-weight: bold;
            color: #fff;
            background-color: #007bff;
            border: none;
            border-radius: 4px;
            cursor: pointer;
            text-decoration: none;
        }

        .dashboard-button:hover {
            background-color: #0056b3;
        }

        .dashboard-button:focus {
            outline: none;
        }

        .logout-button {
            background-color: #dc3545;
            text-decoration: none;
        }

        .logout-button:hover {
            background-color: #c82333;
        }
    </style>
</head>

<body>
    <div class="dashboard-container">
        <h2 class="dashboard-title">Welcome Candidate</h2>

        <a href="VoteNow.aspx" class="dashboard-button">Vote Now</a>
        <a href="ViewMyVote.aspx" class="dashboard-button">View My Vote</a>
        <a href="UpdateProfile.aspx" class="dashboard-button">Update Profile</a>
        <a href="NominateYourself.aspx" class="dashboard-button">Nominate Yourself</a>
        <a href="ViewNotifications.aspx" class="dashboard-button">View Notifications</a>
        <a href="ElectionResults.aspx" class="dashboard-button">View Election Results</a>
        <a href="ManageCampaignDetails.aspx" class="dashboard-button">Manage Campaign Details</a>

        <br />
        <a href="Login.aspx" class="dashboard-button logout-button">Logout</a>
    </div>
</body>

</html>

