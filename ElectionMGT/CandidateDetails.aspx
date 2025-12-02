<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CandidateDetails.aspx.cs" Inherits="ElectionManagementSystem.CandidateDetails" %>

<!DOCTYPE html>
<html lang="en">
<head runat="server">
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Candidate Details</title>
    <style>
        /* General Body Styling */
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

        /* Header Style */
        h1 {
            text-align: center;
            background-color: #4CAF50;
            color: white;
            padding: 15px;
            margin: 0;
        }

        /* Main Content Styling */
        .container {
            width: 60%;
            margin: 30px auto;
            background-color: white;
            padding: 20px;
            box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
            border-radius: 8px;
            text-align: center;
        }

        /* Candidate Details Styling */
        .candidate-details {
            margin-bottom: 20px;
            font-size: 18px;
            color: #333;
        }

        /* Button Styling */
        .vote-button {
            background-color: #4CAF50;
            color: white;
            border: none;
            padding: 10px 20px;
            cursor: pointer;
            border-radius: 4px;
            transition: background-color 0.3s;
        }

        .vote-button:hover {
            background-color: #45a049;
        }

        /* Message Styling */
        .message {
            margin-top: 20px;
            font-size: 18px;
            color: green;
        }

        /* Error Message Styling */
        .error {
            color: red;
        }
    </style>
</head>
<body>

    <!-- Navigation Bar -->
    <div class="navbar">
        <a href="VoterPage.aspx" class="dashboard-button">Home</a>
        <a href="VoteNow.aspx" class="dashboard-button">Vote Now</a>
        <a href="ViewMyVote.aspx" class="dashboard-button">View My Vote</a>
        <a href="UpdateProfile.aspx" class="dashboard-button">Update Profile</a>
        <a href="NominateYourself.aspx" class="dashboard-button">Nominate Yourself</a>
        <a href="ViewNotifications.aspx" class="dashboard-button">View Notifications</a>
        <a href="ElectionResults.aspx" class="dashboard-button">View Election Results</a>
    </div>

    <!-- Candidate Details Form -->
    <form id="form1" runat="server">
        <h1>Candidate Details</h1>
        <div class="container">
            <!-- Display Candidate Information -->
            <asp:Label ID="lblCandidateDetails" runat="server" CssClass="candidate-details" />

            <!-- Vote Button -->
            <asp:Button ID="btnVote" runat="server" Text="Vote" CssClass="vote-button" OnClick="Vote_Click" />

            <!-- Message Label -->
            <asp:Label ID="lblMessage" runat="server" CssClass="message" Visible="false" />
        </div>
    </form>

</body>
</html>
