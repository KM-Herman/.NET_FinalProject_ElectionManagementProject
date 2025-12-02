<%@ Page Title="User & Role Management" Language="C#" MasterPageFile="~/AdminMaster.Master" AutoEventWireup="true" CodeBehind="UserManagement.aspx.cs" Inherits="ElectionMGT.UserManagement" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    </asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <h2 class="mb-4">👤 User & Role Management</h2>

    <ul class="nav nav-tabs" id="userManagementTabs" role="tablist">
        <li class="nav-item">
            <a class="nav-link active" id="voter-tab" data-toggle="tab" href="#voterManagement" role="tab" aria-controls="voterManagement" aria-selected="true">Voter Management</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" id="candidate-tab" data-toggle="tab" href="#candidateManagement" role="tab" aria-controls="candidateManagement" aria-selected="false">Candidate Management</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" id="role-tab" data-toggle="tab" href="#roleAssignment" role="tab" aria-controls="roleAssignment" aria-selected="false">Role Assignment</a>
        </li>
    </ul>

    <div class="tab-content border border-top-0 p-3 bg-white">
        
        <div class="tab-pane fade show active" id="voterManagement" role="tabpanel" aria-labelledby="voter-tab">
            <h4 class="mt-2">Registered Voters</h4>
            
            <asp:Panel ID="pnlManageVoters" runat="server">
                <asp:Label ID="lblVoterMessage" runat="server" CssClass="text-success"></asp:Label>
                <asp:GridView ID="gvVoters" runat="server" AutoGenerateColumns="False" 
                    CssClass="table table-bordered table-striped" DataKeyNames="UserId"
                    OnRowCommand="gvVoters_RowCommand" EmptyDataText="No registered voters found.">
                    <Columns>
                        <asp:BoundField DataField="FirstName" HeaderText="First Name" />
                        <asp:BoundField DataField="LastName" HeaderText="Last Name" />
                        <asp:BoundField DataField="Email" HeaderText="Email" />
                        <asp:TemplateField HeaderText="Actions">
                            <ItemTemplate>
                                <asp:LinkButton ID="btnApproveVoter" runat="server" CommandName="ApproveUser" CommandArgument='<%# Eval("UserId") %>' CssClass="btn btn-sm btn-outline-success mr-2">Approve</asp:LinkButton>
                                <asp:LinkButton ID="btnDeleteVoter" runat="server" CommandName="DeleteUser" CommandArgument='<%# Eval("UserId") %>' OnClientClick="return confirm('Are you sure you want to delete this user?');" CssClass="btn btn-sm btn-outline-danger">Delete</asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </asp:Panel>
            <asp:Label ID="lblNoVoterPermission" runat="server" Visible="false" CssClass="alert alert-danger" Text="You do not have permission to manage voters (ManageVoters)."></asp:Label>

        </div>

        <div class="tab-pane fade" id="candidateManagement" role="tabpanel" aria-labelledby="candidate-tab">
         <h4 class="mt-2">Registered Candidates</h4>
         <p class="text-muted">Review and approve candidate applications for specific elections.</p>
    
        <asp:Label ID="lblCandidateMessage" runat="server" CssClass="text-success mb-3"></asp:Label>
    
        <asp:GridView ID="gvCandidates" runat="server" AutoGenerateColumns="False" 
            CssClass="table table-bordered table-striped" DataKeyNames="CandidateId"
            OnRowCommand="gvCandidates_RowCommand" EmptyDataText="No candidates registered."
        >
        <Columns>
            <asp:BoundField DataField="CandidateId" HeaderText="ID" />
            <asp:BoundField DataField="FullName" HeaderText="Name" />
            <asp:BoundField DataField="ElectionName" HeaderText="Election" />
            <asp:BoundField DataField="Slogan" HeaderText="Slogan" />
            
            <asp:TemplateField HeaderText="Status">
                <ItemTemplate>
                    <asp:Label ID="lblApprovedStatus" runat="server" 
                        Text='<%# Convert.ToBoolean(Eval("Approved")) ? "Approved" : "Pending" %>' 
                        CssClass='<%# Convert.ToBoolean(Eval("Approved")) ? "badge badge-success" : "badge badge-warning" %>'
                        />
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Action">
                <ItemTemplate>
                    <asp:LinkButton ID="btnApproveCandidate" runat="server" 
                        CommandName="ApproveCandidate" 
                        CommandArgument='<%# Eval("CandidateId") %>' 
                        Visible='<%# !Convert.ToBoolean(Eval("Approved")) %>'
                        CssClass="btn btn-sm btn-outline-success mr-2">Approve</asp:LinkButton>
                        
                    <asp:LinkButton ID="btnDeleteCandidate" runat="server" 
                        CommandName="DeleteCandidate" 
                        CommandArgument='<%# Eval("CandidateId") %>' 
                        OnClientClick="return confirm('WARNING: Deleting this candidate will also remove their votes and association with the election. Proceed?');" 
                        CssClass="btn btn-sm btn-outline-danger">Delete</asp:LinkButton>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
    <asp:Label ID="lblNoCandidatePermission" runat="server" Visible="false" CssClass="alert alert-danger" Text="You do not have permission to manage voters (ManageVoters)."></asp:Label>

</div>

        <div class="tab-pane fade" id="roleAssignment" role="tabpanel" aria-labelledby="role-tab">
            <h4 class="mt-2">Assign User Roles</h4>
            <p class="text-muted">Change the role of any registered user below. Changing a role also updates their permissions.</p>

        <asp:Label ID="lblRoleMessage" runat="server" CssClass="text-success mb-3"></asp:Label>
    
        <asp:GridView ID="gvUserRoles" runat="server" AutoGenerateColumns="False" 
            CssClass="table table-bordered table-striped" DataKeyNames="UserId"
            OnRowCommand="gvUserRoles_RowCommand" 
            OnRowDataBound="gvUserRoles_RowDataBound"
            EmptyDataText="No registered users found."
            >
        <Columns>
            <asp:BoundField DataField="UserId" HeaderText="ID" />
            <asp:BoundField DataField="FullName" HeaderText="Name" />
            <asp:BoundField DataField="Email" HeaderText="Email" />
            
            <asp:TemplateField HeaderText="Current Role">
                <ItemTemplate>
                    <asp:Label ID="lblCurrentRole" runat="server" Text='<%# Eval("RoleName") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="New Role">
                <ItemTemplate>
                    <asp:DropDownList ID="ddlNewRole" runat="server" 
                        DataTextField="RoleName" 
                        DataValueField="RoleId" 
                        CssClass="form-control form-control-sm">
                    </asp:DropDownList>
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Action">
                <ItemTemplate>
                    <asp:LinkButton ID="btnUpdateRole" runat="server" CommandName="UpdateRole" 
                        CommandArgument='<%# Eval("UserId") %>' 
                        CssClass="btn btn-sm btn-primary">Update Role</asp:LinkButton>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
    <asp:Label ID="lblNoRolePermission" runat="server" Visible="false" CssClass="alert alert-danger" Text="You do not have permission to manage voters (ManageVoters)."></asp:Label>

</div>

    </div>
    
    <script src="https://code.jquery.com/jquery-3.5.1.slim.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/popper.js@1.16.1/dist/umd/popper.min.js"></script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.min.js"></script>

</asp:Content>