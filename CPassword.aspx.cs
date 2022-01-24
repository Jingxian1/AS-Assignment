using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using System.IO;
using System.Web.Script.Serialization;
using System.Drawing;
using System.Security.Cryptography;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace AS_Assignment
{
    public partial class CPassword: System.Web.UI.Page
    {
        static string finalHash;
        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["AssignemntConnection"].ConnectionString;
        public class MyObject
        {
            public string success { get; set; }
            public List<String> ErrorMessge { get; set; }
        }
        public class CsrfHandler
        {
            public static void Validate(Page page, HiddenField forgeryToken)
            {
                if (!page.IsPostBack)
                {
                    Guid antiforgeryToken = Guid.NewGuid();
                    page.Session["AntiforgeryToken"] = antiforgeryToken;
                    forgeryToken.Value = antiforgeryToken.ToString();
                }
                else
                {
                    Guid stored = (Guid)page.Session["AntiforgeryToken"];
                    Guid sent = new Guid(forgeryToken.Value);
                    if (sent != stored)
                    {
                        // you can throw an exception, in my case I'm just logging the user out
                        page.Session.Abandon();
                        page.Response.Redirect("Login.aspx", false);
                    }
                }
            }
        }
        protected void ChangePwdMe(object sender, EventArgs e)
        {
            string userid = tb_userid.Text;
            string pwd = tb_pwd.Text.ToString().Trim();
            string npwd = tb_npwd.Text.ToString().Trim();
            string cpwd = tb_cpwd.Text.ToString().Trim();
            SHA512Managed hashing = new SHA512Managed();
            string dbHash = getDBHash(userid);
            string dbSalt = getDBSalt(userid);
            if (pwd != "" && cpwd != "" && npwd != "")
            {
                try
                {
                    lblMessage.Text = "no email";
                    if (checkEmail(userid))
                    {
                        lblMessage.Text = "yes email";
                        if (dbSalt != null && dbSalt.Length > 0 && dbHash != null && dbHash.Length > 0)
                        {
                            lblMessage.Text += " FUCK";
                            string pwdWithSalt = pwd + dbSalt;
                            byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));
                            string userHash = Convert.ToBase64String(hashWithSalt);
                            if (userHash.Equals(dbHash))
                            {
                                if (pwd == npwd)
                                {
                                    lbl_pwdchecker.Text = "New Password is the same as current Password!";
                                }
                                if (npwd == cpwd)
                                {
                                    string npwdWithsalt = npwd + dbSalt;
                                    byte[] fhashWsalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(npwdWithsalt));
                                    finalHash = Convert.ToBase64String(fhashWsalt);
                                    changePwd(userid, finalHash);
                                    Response.Redirect("Login.aspx", false);
                                }
                                else
                                {
                                    lbl_pwdchecker.Text = "Password does not match!";
                                }
                            }
                        }
                    }
                    else
                    {
                        lbl_email.Text = "Email does not exist in the database";
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.ToString());
                }
                finally { }
            }
            else
            {
                lblMessage.ForeColor = Color.Red;
                lblMessage.Text = "Fields are empty";
            }
        }

        protected string getDBHash(string userid)
        {
            string h = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select PasswordHash FROM Assignment WHERE email=@USERID";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@USERID", userid);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        if (reader["PasswordHash"] != null)
                        {
                            if (reader["PasswordHash"] != DBNull.Value)
                            {
                                h = reader["PasswordHash"].ToString();
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }
            return h;
        }

        protected string getDBSalt(string userid)
        {
            string s = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select PASSWORDSALT FROM Assignment WHERE email=@USERID";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@USERID", userid);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["PASSWORDSALT"] != null)
                        {
                            if (reader["PASSWORDSALT"] != DBNull.Value)
                            {
                                s = reader["PASSWORDSALT"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }
            return s;
        }
        protected void changePwd(string email, string finalHash)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(MYDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("UPDATE Assignment SET passwordHash = @npwd WHERE email= @email"))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.CommandType = CommandType.Text;

                            cmd.Parameters.AddWithValue("@npwd", finalHash);
                            cmd.Parameters.AddWithValue("@email", email);
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
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }
            return false;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            CsrfHandler.Validate(this.Page, forgeryToken);
        }
    }
}