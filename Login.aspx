<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="AS_Assignment.Login" %>

<!DOCTYPE html>

<head runat="server">
    <title>Login</title>
    <script src="https://www.google.com/recaptcha/api.js"></script>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
        <asp:HiddenField ID="forgeryToken" runat="server"/>
        <div>
            <h2>Login</h2>
            <asp:Table runat="server">
                <asp:TableRow>
                    <asp:TableCell>Email : </asp:TableCell>
                    <asp:TableCell><asp:TextBox ID="tb_userid" runat="server" Height="25px" Width="137px"/></asp:TableCell></asp:TableRow><asp:TableRow>
                    <asp:TableCell>Password :</asp:TableCell><asp:TableCell><asp:TextBox ID="tb_pwd" runat="server" Height="24px" Width="137px" TextMode="Password"/></asp:TableCell></asp:TableRow><asp:TableRow>
                     <asp:TableCell ColumnSpan="2"><asp:Button ID="btnSubmit" runat="server" Text="Login" OnClick="LoginMe" Height="27px" Width="220px"/></asp:TableCell>
                </asp:TableRow>
            </asp:Table>
            <asp:Label ID="lblMessage" runat="server" EnableViewState="false"/>
            <div class="g-recaptcha" data-sitekey="6Ldi6T0dAAAAAIN8AW6RxgFg3Lf4tLafNgEjl7bL"></div>
            <p>New user?<asp:HyperLink runat="server" NavigateUrl="~/Register.aspx"> Sign up here</asp:HyperLink></p></div></form></body></html>