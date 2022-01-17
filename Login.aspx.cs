using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using System.IO;
using System.Web.Script.Serialization;
using System.Web.Services;
using System.Drawing;
using System.Security.Cryptography;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Timers;

namespace AS_Assignment
{
    public partial class Login : System.Web.UI.Page
    {
        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["AssignemntConnection"].ConnectionString;
        public class MyObject
        {
            public string success { get; set; }
            public List<String> ErrorMessge { get; set; }
        }
        public bool ValidateCaptcha()
        {
            bool result = true;

            //When user submits the recaptcha form, the user gets a response POST parameter. 
            //captchaResponse consist of the user click pattern. Behaviour analytics! AI :) 
            string captchaResponse = Request.Form["g-recaptcha-response"];

            //To send a GET request to Google along with the response and Secret key.
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create
           (" https://www.google.com/recaptcha/api/siteverify?secret=6Ldi6T0dAAAAAIxVmwCCWe8T17wvGaDNdRMN2260 &response=" + captchaResponse);


            try
            {

                //Codes to receive the Response in JSON format from Google Server
                using (WebResponse wResponse = req.GetResponse())
                {
                    using (StreamReader readStream = new StreamReader(wResponse.GetResponseStream()))
                    {
                        //The response in JSON format
                        string jsonResponse = readStream.ReadToEnd();

                        //To show the JSON response string for learning purpose

                        JavaScriptSerializer js = new JavaScriptSerializer();

                        //Create jsonObject to handle the response e.g success or Error
                        //Deserialize Json
                        MyObject jsonObject = js.Deserialize<MyObject>(jsonResponse);

                        //Convert the string "False" to bool false or "True" to bool true
                        result = Convert.ToBoolean(jsonObject.success);//

                    }
                }

                return result;
            }
            catch (WebException ex)
            {
                throw ex;
            }
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
        protected void LoginMe(object sender, EventArgs e)
        {
            if (ValidateCaptcha())
            {
                string pwd = tb_pwd.Text.ToString().Trim();
                string userid = tb_userid.Text.ToString().Trim();
                SHA512Managed hashing = new SHA512Managed();
                string dbHash = getDBHash(userid);
                string dbSalt = getDBSalt(userid);
                if (pwd != "" || userid != "") { 
                try
                {
                    if (dbSalt != null && dbSalt.Length > 0 && dbHash != null && dbHash.Length > 0)
                    {
                        string pwdWithSalt = pwd + dbSalt;
                        byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));
                        string userHash = Convert.ToBase64String(hashWithSalt);
                        if (userHash.Equals(dbHash))
                        {
                            Session["LoggedIn"] = tb_userid.Text.Trim();

                            string guid = Guid.NewGuid().ToString();
                            Session["AuthToken"] = guid;

                            Response.Cookies.Add(new HttpCookie("AuthToken", guid));

                            Response.Redirect("HomePage.aspx", false);
                        }


                        else
                        {
                                var email = tb_userid.Text;
                                if (retrieveFLA(email) < 3)
                                {
                                    lblMessage.ForeColor = Color.Red;
                                    lblMessage.Text = "Userid or password not valid. Please try again.";
                                    addFLA(email);
                                }
                                else
                                {
                                    lblMessage.ForeColor = Color.Red;
                                    lblMessage.Text = ("Account locked out");
                                    var aTimer = new System.Timers.Timer(2000);
                                    aTimer.Elapsed += resetFLA;
                                    aTimer.Interval = 2000;
                                    aTimer.Enabled = true;
                                }
                            }
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
            else
            {
                lblMessage.ForeColor = Color.Red;
                lblMessage.Text = "Validate captcha to prove you are human";
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

        protected int retrieveFLA(string email)
        {
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "SELECT FLA As count FROM Assignment WHERE email='" + email + "'";
            SqlCommand command = new SqlCommand(sql, connection);
            try
            {
                connection.Open();
                int count = (int)command.ExecuteScalar();
                connection.Close();
                return count;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }
        }

        protected void resetFLA(object source, ElapsedEventArgs e)
        {
            try
            {
                var email = tb_userid.Text;
                using (SqlConnection con = new SqlConnection(MYDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("UPDATE Assignment SET FLA = @FLA WHERE email='" + email + "'"))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.CommandType = CommandType.Text;

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

        protected void addFLA(string email)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(MYDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("UPDATE Assignment SET FLA = @FLA WHERE email='" + email + "'"))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.CommandType = CommandType.Text;

                            cmd.Parameters.AddWithValue("@FLA", retrieveFLA(email) + 1);
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

        protected void Page_Load(object sender, EventArgs e)
        {
            CsrfHandler.Validate(this.Page, forgeryToken);
        }
    }
}