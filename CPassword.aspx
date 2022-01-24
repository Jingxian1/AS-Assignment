<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CPassword.aspx.cs" Inherits="AS_Assignment.CPassword" %>

<!DOCTYPE html>

<head runat="server">
    <title>Change Password</title>
    <script type="text/javascript">
            function validatePass() {
            var str = document.getElementById('<%=tb_npwd.ClientID %>').value;

            if (str.length < 12) {
                document.getElementById("lbl_pwdchecker").innerHTML = "New Password length must be at least 12 characters"
                document.getElementById("lbl_pwdchecker").style.color = "red";
                return ("too_short");
            }
            else if (str.search(/[0-9]/) == -1) {
                document.getElementById("lbl_pwdchecker").innerHTML = "New Password requires at least 1 number"
                document.getElementById("lbl_pwdchecker").style.color = "red";
                return ("no_number");
            }
            else if (str.search(/[a-z]/) == -1) {
                document.getElementById("lbl_pwdchecker").innerHTML = "New Password requires at least 1 lowercase letter"
                document.getElementById("lbl_pwdchecker").style.color = "red";
                return ("no_LC");
            }
            else if (str.search(/[A-Z]/) == -1) {
                document.getElementById("lbl_pwdchecker").innerHTML = "New Password requires at least 1 uppercase letter"
                document.getElementById("lbl_pwdchecker").style.color = "red";
                return ("no_UC");
            }
            else if (str.search(/[!@#$%^&*]/) == -1) {
                document.getElementById("lbl_pwdchecker").innerHTML = "New Password requires at least 1 special character(! @ # $ % ^ & *)"
                document.getElementById("lbl_pwdchecker").style.color = "red";
                return ("no_SC");
            }



            document.getElementById("lbl_pwdchecker").innerHTML = "Excellent"
            document.getElementById("lbl_pwdchecker").style.color = "green";
        }
        </script>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
        <asp:HiddenField ID="forgeryToken" runat="server"/>
        <div>
            <h2>Change Password</h2>
            <asp:Table runat="server">
                <asp:TableRow>
                   <asp:TableCell>Email : </asp:TableCell>
                    <asp:TableCell><asp:TextBox ID="tb_userid" runat="server" Height="25px" Width="137px"/><asp:Label ID="lbl_email" runat="server"/></asp:TableCell></asp:TableRow><asp:TableRow>
                    <asp:TableCell>Current Password : </asp:TableCell>
                    <asp:TableCell><asp:TextBox ID="tb_pwd" runat="server" Height="25px" Width="137px" TextMode="Password"/></asp:TableCell></asp:TableRow><asp:TableRow>
                    <asp:TableCell>New Password :</asp:TableCell><asp:TableCell><asp:TextBox onkeyup="javascript:validatePass()" ID="tb_npwd" runat="server" Height="24px" Width="137px" TextMode="Password"/><asp:Label ID="lbl_pwdchecker" runat="server"/></asp:TableCell></asp:TableRow><asp:TableRow>
                        <asp:TableCell>Confirm New Password :</asp:TableCell><asp:TableCell><asp:TextBox ID="tb_cpwd" runat="server" Height="24px" Width="137px" TextMode="Password"/></asp:TableCell></asp:TableRow><asp:TableRow>
                     <asp:TableCell ColumnSpan="2"><asp:Button ID="btnSubmit" runat="server" Text="Change Password" OnClick="ChangePwdMe" Height="27px" Width="220px"/></asp:TableCell>
                </asp:TableRow>
            </asp:Table>
            <asp:Label ID="lblMessage" runat="server" EnableViewState="false"/>
            <p>Back to Login?<asp:HyperLink runat="server" NavigateUrl="~/Login.aspx"> Back</asp:HyperLink></p></div></form></body>
