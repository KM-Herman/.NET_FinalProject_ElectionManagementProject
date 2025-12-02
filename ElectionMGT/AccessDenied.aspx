<%@ Page Title="Access Denied" Language="C#" AutoEventWireup="true" CodeBehind="AccessDenied.aspx.cs" Inherits="ElectionMGT.AccessDenied" %>

<!DOCTYPE html>
<html lang="en">
<head runat="server">
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>403 - Access Denied</title>
    <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css" />
    <style>
        /* Custom styling for the page */
        body {
            background-color: #f8f9fa; /* Light gray background */
            display: flex;
            justify-content: center;
            align-items: center;
            min-height: 100vh;
            text-align: center;
        }
        .error-container {
            max-width: 600px;
            padding: 40px;
            background-color: #fff;
            border-radius: 10px;
            box-shadow: 0 4px 15px rgba(0, 0, 0, 0.1);
        }
        .error-code {
            font-size: 8rem;
            font-weight: 700;
            color: #dc3545; /* Bootstrap Danger color */
            line-height: 1;
            margin-bottom: 0;
        }
        .error-icon {
            font-size: 3rem;
            color: #dc3545;
        }
        .sub-heading {
            color: #6c757d;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="error-container">
            <div class="error-icon mb-3">
                <span style="font-size: 3rem;">🔒</span>
            </div>
            
            <h1 class="error-code">403</h1>
            <h2 class="sub-heading mb-4">ACCESS DENIED - FORBIDDEN</h2>

            <p class="lead">
                You do not have the required **permissions** or **role** to view this page.
                This incident has been logged for security review.
            </p>

            <hr class="my-4">

            <p class="text-muted">
                If you believe this is an error, please contact your system administrator.
            </p>
            
            <asp:HyperLink ID="hlDashboard" runat="server" 
                Text="Go to Dashboard" NavigateUrl="~/AdminPage.aspx" 
                CssClass="btn btn-primary btn-lg mr-2" />

            <asp:HyperLink ID="hlLogout" runat="server" 
                Text="Log In" NavigateUrl="~/Login.aspx" 
                CssClass="btn btn-outline-secondary btn-lg" />

            <asp:Label ID="lblUserMessage" runat="server" CssClass="mt-3 d-block text-danger font-weight-bold" />
        </div>
    </form>
</body>
</html>