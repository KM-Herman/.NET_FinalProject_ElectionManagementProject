<%@ Page Title="Notification Management" Language="C#" MasterPageFile="~/AdminMaster.Master" AutoEventWireup="true" CodeBehind="NotificationManagement.aspx.cs" Inherits="ElectionMGT.NotificationManagement" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <h2 class="mb-4">📧 Notification Management</h2>
    
    <div class="row">
        <div class="col-lg-5 mb-4">
            <div class="card shadow">
                <div class="card-header bg-primary text-white">
                    <h5 class="mb-0">Compose New Message</h5>
                </div>
                <div class="card-body">
                    <asp:Label ID="lblSendStatus" runat="server" CssClass="mb-3"></asp:Label>
                    
                    <div class="form-group">
                        <asp:Label runat="server" Text="Recipient Group"></asp:Label>
                        <asp:DropDownList ID="ddlRecipientGroup" runat="server" CssClass="form-control">
                            <asp:ListItem Value="AllUsers" Text="All Registered Users"></asp:ListItem>
                            <asp:ListItem Value="Voters" Text="All Voters"></asp:ListItem>
                            <asp:ListItem Value="Candidates" Text="All Candidates"></asp:ListItem>
                            <asp:ListItem Value="Admins" Text="All Admins"></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    
                    <div class="form-group">
                        <asp:Label runat="server" Text="Subject"></asp:Label>
                        <asp:TextBox ID="txtSubject" runat="server" CssClass="form-control"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvSubject" runat="server" ControlToValidate="txtSubject" ErrorMessage="Subject is required." ForeColor="Red"></asp:RequiredFieldValidator>
                    </div>

                    <div class="form-group">
                        <asp:Label runat="server" Text="Message"></asp:Label>
                        <asp:TextBox ID="txtMessageBody" runat="server" TextMode="MultiLine" Rows="5" CssClass="form-control"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvMessageBody" runat="server" ControlToValidate="txtMessageBody" ErrorMessage="Message body is required." ForeColor="Red"></asp:RequiredFieldValidator>
                    </div>

                    <asp:Button ID="btnSendNotification" runat="server" Text="Send Notification" 
                        CssClass="btn btn-primary btn-block mt-3" OnClick="btnSendNotification_Click" />
                </div>
            </div>
        </div>

        <div class="col-lg-7 mb-4">
            <div class="card shadow">
                <div class="card-header bg-secondary text-white">
                    <h5 class="mb-0">Notification History</h5>
                </div>
                <div class="card-body">
                    <asp:Label ID="lblHistoryMessage" runat="server" CssClass="mb-3"></asp:Label>
                    
                    <asp:GridView ID="gvNotifications" runat="server" AutoGenerateColumns="False" 
                        CssClass="table table-bordered table-striped table-sm" DataKeyNames="NotificationId"
                        EmptyDataText="No notifications have been sent yet."
                        >
                        <Columns>
                            <asp:BoundField DataField="NotificationId" HeaderText="ID" ItemStyle-Width="50px"/>
                            <asp:BoundField DataField="Subject" HeaderText="Subject" />
                            <asp:BoundField DataField="Message" HeaderText="Preview" ItemStyle-Width="40%" />
                            <asp:BoundField DataField="SentDate" HeaderText="Sent On" DataFormatString="{0:MMM dd, yyyy HH:mm}" />
                        </Columns>
                    </asp:GridView>
                </div>
            </div>
        </div>
    </div>

</asp:Content>