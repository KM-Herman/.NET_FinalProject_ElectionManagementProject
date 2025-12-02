<%@ Page Title="Dashboard Overview" Language="C#" MasterPageFile="~/AdminMaster.Master" AutoEventWireup="true" CodeBehind="AdminPage.aspx.cs" Inherits="ElectionMGT.AdminPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <h2 class="mb-4">📊 System Overview Dashboard</h2>

    <div class="row">
        <div class="col-lg-3 col-md-6 mb-4">
            <div class="card bg-primary text-white shadow">
                <div class="card-body">
                    <div class="h5 mb-0 font-weight-bold">Total Voters</div>
                    <asp:Label ID="lblTotalVoters" runat="server" Text="0" CssClass="display-4"></asp:Label>
                </div>
            </div>
        </div>
        
        <div class="col-lg-3 col-md-6 mb-4">
            <div class="card bg-success text-white shadow">
                <div class="card-body">
                    <div class="h5 mb-0 font-weight-bold">Total Candidates</div>
                    <asp:Label ID="lblTotalCandidates" runat="server" Text="0" CssClass="display-4"></asp:Label>
                </div>
            </div>
        </div>
        
        <div class="col-lg-3 col-md-6 mb-4">
            <div class="card bg-info text-white shadow">
                <div class="card-body">
                    <div class="h5 mb-0 font-weight-bold">Active Election</div>
                    <asp:Label ID="lblElectionStatus" runat="server" Text="N/A" CssClass="h3"></asp:Label>
                </div>
            </div>
        </div>

        <div class="col-lg-3 col-md-6 mb-4">
            <div class="card bg-warning text-dark shadow">
                <div class="card-body">
                    <div class="h5 mb-0 font-weight-bold">Unread Notifications</div>
                    <asp:Label ID="lblUnreadNotifications" runat="server" Text="0" CssClass="display-4"></asp:Label>
                </div>
            </div>
        </div>
    </div>
    
    <div class="row mt-4">
        <div class="col-lg-6 mb-4">
            <div class="card shadow">
                <div class="card-header bg-secondary text-white">
                    <h5 class="mb-0">🕒 Recent System Activity</h5>
                </div>
                <div class="card-body">
                    <asp:GridView ID="gvActivityLog" runat="server" AutoGenerateColumns="False" 
                        CssClass="table table-striped table-sm"
                        EmptyDataText="No recent activity found."
                        >
                        <Columns>
                            <asp:BoundField DataField="Timestamp" HeaderText="Time" DataFormatString="{0:MMM dd, hh:mm tt}" />
                            <asp:BoundField DataField="ActivityType" HeaderText="Type" />
                            <asp:BoundField DataField="Details" HeaderText="Details" />
                        </Columns>
                    </asp:GridView>
                </div>
            </div>
        </div>

        <div class="col-lg-6 mb-4">
            <div class="card shadow">
                <div class="card-header bg-dark text-white">
                    <h5 class="mb-0">🗳️ Current/Upcoming Election</h5>
                </div>
                <div class="card-body">
                    <asp:Panel ID="pnlElectionDetails" runat="server">
                        <p><strong>Name:</strong> <asp:Label ID="lblCurrentElectionName" runat="server"></asp:Label></p>
                        <p><strong>Status:</strong> <asp:Label ID="lblCurrentElectionStatus" runat="server"></asp:Label></p>
                        <p><strong>Running Candidates:</strong> <asp:Label ID="lblRunningCandidatesCount" runat="server"></asp:Label></p>
                        <p><strong>Total Votes Cast:</strong> <asp:Label ID="lblTotalVotesCast" runat="server"></asp:Label></p>
                    </asp:Panel>
                    <asp:Label ID="lblNoElection" runat="server" Text="No active or upcoming election found." Visible="false" CssClass="text-muted"></asp:Label>
                </div>
            </div>
        </div>
    </div>

</asp:Content>