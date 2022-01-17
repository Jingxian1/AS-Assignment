using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Security.Cryptography;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace AS_Assignment
{
    public partial class Register : System.Web.UI.Page
    {
        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["AssignemntConnection"].ConnectionString;
        static string finalHash;
        static string salt;
        byte[] Key;
        byte[] IV;
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btn_checkPassword_Click(object sender, EventArgs e)
        {

            int scores = checkPassword(tb_password.Text);
            string check = checkDate(tb_date.Text);
            string checkFName = checkFirstName(tb_firstName.Text);
            string checkLName = checkLastName(tb_lastName.Text);
            string checkEEmail = checkEmailfield(tb_email.Text);

            string status = "";
            if (check != null)
            {
                lbl_date.ForeColor = Color.Red;
                lbl_date.Text = check;
            }
            if (checkFName != null)
            {
                lbl_firstName.ForeColor = Color.Red;
                lbl_firstName.Text = checkFName;
            }
            if(checkLName != null)
            {
                lbl_lastName.ForeColor = Color.Red;
                lbl_lastName.Text = checkLName;
            }
            if (checkEEmail != null)
            {
                lbl_email.ForeColor = Color.Red;
                lbl_email.Text = checkEEmail;
            }

            switch (scores)
            {
                case 1:
                    status = "Very Weak";
                    break;
                case 2:
                    status = "Weak";
                    break;
                case 3:
                    status = "Medium";
                    break;
                case 4:
                    status = "Strong";
                    break;
                case 5:
                    status = "Very Strong";
                    break;
                default:
                    break;
            }
            lbl_pwdchecker.Text = "Status : " + status;
            if (scores < 4)
            {
                lbl_pwdchecker.ForeColor = Color.Red;
                return;
            }
            lbl_pwdchecker.ForeColor = Color.Green;

            lbl_date.Text = check;
            if (lbl_date.Text != "")
            {
                lbl_date.ForeColor = Color.Red;
            }

            string email = tb_email.Text.ToString().Trim();
            string pwd = tb_password.Text.ToString().Trim(); 
            //Generate random "salt"
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] saltByte = new byte[8];
            //Fills array of bytes with a cryptographically strong sequence of random values.
            rng.GetBytes(saltByte);
            salt = Convert.ToBase64String(saltByte);
            SHA512Managed hashing = new SHA512Managed();
            string pwdWithSalt = pwd + salt;
            byte[] plainHash = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwd));
            byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));
            finalHash = Convert.ToBase64String(hashWithSalt);
            RijndaelManaged cipher = new RijndaelManaged();
            cipher.GenerateKey();
            Key = cipher.Key;
            IV = cipher.IV;
            if (checkEmail(email))
            {
                createAccount();
                Response.Redirect("Login.aspx");
            }
            else
            {
                lbl_email.Text = "Email exists in database";
                lbl_email.ForeColor = Color.Red;
            }

        }

        protected bool checkEmail(string email)
        {
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "SELECT COUNT(email) As EmailCount FROM Assignment WHERE email= @email";
            SqlCommand command = new SqlCommand(sql, connection);
            try
            {
                command.Parameters.AddWithValue("@email", email);
                connection.Open();
                int EmailCount = (int)command.ExecuteScalar();
                if (EmailCount > 0)
                {
                    connection.Close();
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }
            return true;
        }

        protected void createAccount()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(MYDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("INSERT INTO Assignment VALUES(@firstName, @lastName, @email, @passwordHash, @passwordSalt, @DOB, @cardNum, @CVV, @cardExp, @IV, @Key, @FLA )"))
                {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@firstName", tb_firstName.Text.Trim());
                            cmd.Parameters.AddWithValue("@lastName", tb_lastName.Text.Trim());
                            cmd.Parameters.AddWithValue("@email", tb_email.Text.Trim());
                            cmd.Parameters.AddWithValue("@passwordHash", finalHash);
                            cmd.Parameters.AddWithValue("@passwordSalt", salt);
                            cmd.Parameters.AddWithValue("@DOB", tb_date.Text);
                            cmd.Parameters.AddWithValue("@cardNum", encryptData(tb_cardnum.Text.Trim()));
                            cmd.Parameters.AddWithValue("@CVV", encryptData(tb_cvv.Text.Trim()));
                            cmd.Parameters.AddWithValue("@cardExp", encryptData(tb_expiry.Text.Trim()));
                            cmd.Parameters.AddWithValue("@IV", Convert.ToBase64String(IV));
                            cmd.Parameters.AddWithValue("@Key", Convert.ToBase64String(Key));
                            cmd.Parameters.AddWithValue("@FLA", 0);
                            cmd.Connection = con;
                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        protected byte[] encryptData(string data)
        {
            byte[] cipherText = null;
            try
            {
                RijndaelManaged cipher = new RijndaelManaged();
                cipher.IV = IV;
                cipher.Key = Key;
                ICryptoTransform encryptTransform = cipher.CreateEncryptor();
                byte[] plainText = Encoding.UTF8.GetBytes(data);
                cipherText = encryptTransform.TransformFinalBlock(plainText, 0, plainText.Length);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { }
            return cipherText;
        }


        private int checkPassword(string password)
        {
            int score = 0;
            if (password.Length < 12)
            {
                return 1;
            }
            else
            {
                score = 1;
            }
            if (Regex.IsMatch(password, "[a-z]"))
            {
                score++;
            }
            if (Regex.IsMatch(password, "[A-Z]"))
            {
                score++;
            }
            if (Regex.IsMatch(password, "[0-9]"))
            {
                score++;
            }
            if (Regex.IsMatch(password, "[!@#$%^&*]"))
            {
                score++;
            }

            return score;
        }

        private string checkFirstName(string name)
        {
            string check = "";
            if(name == "")
            {
                check = "First Name field must be filled";
            }
            return check;
        }

        private string checkEmailfield(string name)
        {
            string check = "";
            if (name == "")
            {
                check = "Email field must be filled";
            }
            return check;
        }

        private string checkLastName(string name)
        {
            string check = "";
            if (name == "")
            {
                check = "Last Name field must be filled";
            }
            return check;
        }

        private string checkDate(string date)
        {
            string check = "";
            if (date != "")
            {
                if (Convert.ToDateTime(date) > DateTime.Now.Date) ;
            }
            else
            {
                check = "Please enter Date of Birth";
            }
            return check;
        }
    }
}