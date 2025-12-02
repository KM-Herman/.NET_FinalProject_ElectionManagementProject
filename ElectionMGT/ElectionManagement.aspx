<%@ Page Title="Election Management" Language="C#" MasterPageFile="~/AdminMaster.Master" AutoEventWireup="true" CodeBehind="ElectionManagement.aspx.cs" Inherits="ElectionMGT.ElectionManagement" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    </asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <h2 class="mb-4">🗳️ Election Management</h2>
    
    <div class="row">
        <div class="col-lg-4 mb-4">
            <div class="card shadow">
                <div class="card-header bg-primary text-white">
                    <h5 class="mb-0">Create New Election</h5>
                </div>
                <div class="card-body">
                    <asp:Label ID="lblCreateMessage" runat="server" CssClass="mb-3"></asp:Label>
                    
                    <div class="form-group">
                        <asp:Label ID="Label1" runat="server" Text="Election Name"></asp:Label>
                        <asp:TextBox ID="txtElectionName" runat="server" CssClass="form-control"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvElectionName" runat="server" ControlToValidate="txtElectionName" ErrorMessage="Election name is required." ForeColor="Red"></asp:RequiredFieldValidator>
                    </div>

                    <div class="form-group">
                        <asp:Label ID="Label2" runat="server" Text="Start Date (Optional)"></asp:Label>
                        <asp:TextBox ID="txtStartDate" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                    </div>
                    
                    <div class="form-group">
                        <asp:Label ID="Label3" runat="server" Text="End Date (Optional)"></asp:Label>
                        <asp:TextBox ID="txtEndDate" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                    </div>

                    <asp:Button ID="btnCreateElection" runat="server" Text="Create Election" 
                        CssClass="btn btn-primary btn-block mt-3" OnClick="btnCreateElection_Click" />
                </div>
            </div>
        </div>

        <div class="col-lg-8 mb-4">
            <div class="card shadow">
                <div class="card-header bg-secondary text-white">
                    <h5 class="mb-0">Current and Past Elections</h5>
                </div>
                <div class="card-body">
                    <asp:Label ID="lblListMessage" runat="server" CssClass="mb-3"></asp:Label>
                    
                    <asp:GridView ID="gvElections" runat="server" AutoGenerateColumns="False" 
                        CssClass="table table-bordered table-striped" DataKeyNames="ElectionId"
                        OnRowCommand="gvElections_RowCommand" EmptyDataText="No elections created yet."
                        >
                        <Columns>
                            <asp:BoundField DataField="ElectionId" HeaderText="ID" />
                            <asp:BoundField DataField="ElectionName" HeaderText="Name" />
                            <asp:TemplateField HeaderText="Status">
                                <ItemTemplate>
                                    <asp:Label ID="lblStatus" runat="server" 
                                        Text='<%# Eval("Status") %>' 
                                        CssClass='<%# GetStatusCss(Eval("Status")) %>'
                                        />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Actions">
                                <ItemTemplate>
                                    <asp:LinkButton ID="btnOpen" runat="server" CommandName="SetOngoing" 
                                        CommandArgument='<%# Eval("ElectionId") %>' 
                                        Visible='<%# Eval("Status").ToString() != "Ongoing" && Eval("Status").ToString() != "Finished" %>'
                                        CssClass="btn btn-sm btn-success mr-2">Open</asp:LinkButton>
            
                                    <asp:LinkButton ID="btnPending" runat="server" CommandName="SetUpcoming" 
                                        CommandArgument='<%# Eval("ElectionId") %>' 
                                         Visible='<%# Eval("Status").ToString() == "Ongoing" || Eval("Status").ToString() == "Finished" %>'
                                         OnClientClick="return confirm('WARNING: Setting to Pending will stop voting. Proceed?');"
                                         CssClass="btn btn-sm btn-info mr-2">Pending</asp:LinkButton>
            
                                    <asp:LinkButton ID="btnClose" runat="server" CommandName="SetFinished" 
                                        CommandArgument='<%# Eval("ElectionId") %>' 
                                         Visible='<%# Eval("Status").ToString() == "Ongoing" %>'
                                          CssClass="btn btn-sm btn-warning mr-2">Close</asp:LinkButton>
                                        
                                    <asp:LinkButton ID="btnDelete" runat="server" CommandName="DeleteElection" 
                                        CommandArgument='<%# Eval("ElectionId") %>' 
                                        OnClientClick="return confirm('WARNING: Deleting an election is irreversible. Proceed?');" 
                                        CssClass="btn btn-sm btn-danger">Delete</asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </div>
        </div>
    </div>

</asp:Content>