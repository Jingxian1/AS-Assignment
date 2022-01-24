<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="AS_Assignment.Register" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Registeration Form</title>

    <script type="text/javascript">

        function validateDate() {
            var str_date = document.getElementById('<%=tb_date.ClientID %>').value;
            var tdyDate = new Date();
            if (str_date > tdyDate) {
                document.getElementById("lbl_date").innerHTML = "Invalid date of birth"
                document.getElementById("lbl_date").style.color = "red";
                return ("date_invalid");
            }
        }

        function validatePass() {
            var str = document.getElementById('<%=tb_password.ClientID %>').value;

            if (str.length < 12) {
                document.getElementById("lbl_pwdchecker").innerHTML = "Password length must be at least 12 characters"
                document.getElementById("lbl_pwdchecker").style.color = "red";
                return ("too_short");
            }
            else if (str.search(/[0-9]/) == -1) {
                document.getElementById("lbl_pwdchecker").innerHTML = "Password requires at least 1 number"
                document.getElementById("lbl_pwdchecker").style.color = "red";
                return ("no_number");
            }
            else if (str.search(/[a-z]/) == -1) {
                document.getElementById("lbl_pwdchecker").innerHTML = "Password requires at least 1 lowercase letter"
                document.getElementById("lbl_pwdchecker").style.color = "red";
                return ("no_LC");
            }
            else if (str.search(/[A-Z]/) == -1) {
                document.getElementById("lbl_pwdchecker").innerHTML = "Password requires at least 1 uppercase letter"
                document.getElementById("lbl_pwdchecker").style.color = "red";
                return ("no_UC");
            }
            else if (str.search(/[!@#$%^&*]/) == -1) {
                document.getElementById("lbl_pwdchecker").innerHTML = "Password requires at least 1 special character(! @ # $ % ^ & *)"
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
        <h2>Account Registration</h2>
            <asp:Table runat="server">
                <asp:TableRow>
                    <asp:TableCell><div>First Name*:</div></asp:TableCell>
                    <asp:TableCell><asp:TextBox  runat="server" ID="tb_firstName"/><asp:Label ID="lbl_firstName" runat="server"/></asp:TableCell>
                </asp:TableRow>

                <asp:TableRow>
                    <asp:TableCell><div>Last Name*:</div></asp:TableCell>
                    <asp:TableCell><asp:TextBox  runat="server" ID="tb_lastName"/><asp:Label ID="lbl_lastName" runat="server" /></asp:TableCell>
                </asp:TableRow>

                <asp:TableRow>
                    <asp:TableCell><div>Email*:</div></asp:TableCell>
                    <asp:TableCell><asp:TextBox ID="tb_email" runat="server" TextMode="Email"/><asp:Label ID="lbl_email" runat="server"/></asp:TableCell>
                </asp:TableRow>

                <asp:TableRow>
                    <asp:TableCell><div>Password*:</div></asp:TableCell>
                    <asp:TableCell><asp:TextBox ID="tb_password" runat="server" onkeyup="javascript:validatePass()" TextMode="Password"/><asp:Label ID="lbl_pwdchecker" runat="server"/></asp:TableCell>
                </asp:TableRow>

                <asp:TableRow>
                    <asp:TableCell><div>Date of Birth*:</div></asp:TableCell>
                    <asp:TableCell><asp:TextBox ID="tb_date" runat="server" TextMode="Date"/><asp:Label runat="server" ID="lbl_date"/></asp:TableCell>
                </asp:TableRow>

                <asp:TableRow>
                    <asp:TableCell><div>Card Number*:</div></asp:TableCell>
                </asp:TableRow>

                <asp:TableRow>
                    <asp:TableCell ColumnSpan="3"><asp:TextBox  runat="server" ID="tb_cardnum" TextMode="Number" Width="270"/></asp:TableCell>
                    <asp:TableCell><asp:Label runat="server" ID="lbl_cardnum"/></asp:TableCell>
                </asp:TableRow>

                <asp:TableRow>
                    <asp:TableCell>CVV*:</asp:TableCell>
                    <asp:TableCell>Expiration Date*:</asp:TableCell>
                </asp:TableRow>

                <asp:TableRow>
                    <asp:TableCell><asp:TextBox  runat="server" ID="tb_cvv" TextMode="Number" width="130px"/><asp:Label runat="server" ID="lbl_cvv"/></asp:TableCell>
                    <asp:TableCell><asp:TextBox  runat="server" ID="tb_expiry" width="130px"/></asp:TableCell>
                </asp:TableRow>

                <asp:TableRow>
                    <asp:TableCell><div>Photo*: </div></asp:TableCell>
                    <asp:TableCell><asp:FileUpload ID="tb_image" runat="server" /><asp:Label ID="lbl_image" runat="server"/></asp:TableCell>
                </asp:TableRow>

                <asp:TableRow>
                    <asp:TableCell><asp:Button style="margin:10px 0px 0px 0px;" Text="Submit" ID="reg_button" runat="server" onClick="btn_checkPassword_Click"/></asp:TableCell>
                    <asp:TableCell></asp:TableCell>
                </asp:TableRow>
            </asp:Table>
        <p>Already have an account? <asp:HyperLink runat="server" NavigateUrl="~/Login.aspx">Log in</asp:HyperLink></p>
    </form>
</body>
</html>
