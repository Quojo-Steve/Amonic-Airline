using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

namespace Amonic_Airlines
{
    public partial class Form2 : Form
    {
        public string connection = "Data Source=MONSTER\\SQLEXPRESS;Initial Catalog=Session_1;Integrated Security=True";
        public static DataTable dt = new DataTable();
        public static DataTable reason = new DataTable();
        System.Timers.Timer timer;


        int count = 0;
        private int secondsRemaining = 600; // 10 minutes

        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string username = textBox1.Text;
            string password = textBox2.Text;
            string newpassword = GetMd5Hash(password);
            if (username == "" || password == "")
            {
                this.Hide();
                MessageBox.Show("Please enter your username and password");
                this.Show();
            }
            else
            {
                try
                {
                    SqlConnection logconnection = new SqlConnection(connection);
                    SqlCommand sqlCommand = new SqlCommand("select * from [dbo].[Users] where @username = [dbo].[Users].[Email]" +
                        " and @password = [dbo].[Users].[Password]", logconnection);
                    sqlCommand.Parameters.AddWithValue("@username", username);
                    sqlCommand.Parameters.AddWithValue("@password", newpassword);
                    SqlDataAdapter adapter = new SqlDataAdapter(sqlCommand);

                    adapter.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        int id = Convert.ToInt32(dt.Rows[0]["ID"]);
                        DateTime currenttime = DateTime.Now;
                        //DateTime utctime = DateTime.UtcNow;
                        try
                        {
                            SqlConnection conn = new SqlConnection(connection);
                            SqlCommand cmd = new SqlCommand("insert into [dbo].[reasons]([dbo].[reasons].[UserID],[dbo].[reasons].[logon])" +
                                "values(@userid,@logon)", logconnection);
                            cmd.Parameters.AddWithValue("@userid", id);
                            cmd.Parameters.AddWithValue("@logon", currenttime);
                            SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                            dataAdapter.Fill(reason);
                            Console.WriteLine(currenttime);
                            
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("" + ex);
                        }

                        Form1 f = new Form1();
                        this.Hide();
                        f.Show();
                    }
                    else
                    {
                        count++;
                        if (count == 3)
                        {
                            countdown();
                            count = 1;
                        }
                        else
                        {
                            this.Hide();
                            MessageBox.Show("Either your password or username is incorrect");
                            this.Show();
                        }
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show("q" + ex);
                }
            }


        }
        private void countdown()
        {
            timer = new System.Timers.Timer();
            timer.Interval = 1000;
            timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);

            timer.Enabled = true;
            button1.Enabled = false;
            textBox1.Enabled = false;
            textBox2.Enabled = false;

        }


        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            Invoke(new Action(() =>
            {

                secondsRemaining--;

                if (secondsRemaining <= 0)
                {
                    var timer = (System.Timers.Timer)source;
                    timer.Stop();
                    timer.Enabled = false;
                    button1.Enabled = true;
                    button1.Text = "Login";
                    secondsRemaining = 60;
                    label6.Text = "";
                    return;
                }


                var minutes = secondsRemaining / 60;
                var seconds = secondsRemaining % 60;
                String countText = string.Format("{0}:{1}", minutes.ToString().PadLeft(2, '0'), seconds.ToString().PadLeft(2, '0'));
                button1.Text = countText;
                label6.Text = string.Format("Invalid login credentials, try again in {0}:{1}", minutes.ToString().PadLeft(2, '0'), seconds.ToString().PadLeft(2, '0'));
                label6.Visible = true;
            }));
        }

        public static string GetMd5Hash(string input)
        {
            // Create a new instance of the MD5CryptoServiceProvider class
            using (MD5 md5Hash = MD5.Create())
            {
                // Convert the input string to a byte array
                byte[] data = md5Hash.ComputeHash(System.Text.Encoding.UTF8.GetBytes(input));

                // Create a new StringBuilder to collect the bytes
                System.Text.StringBuilder sBuilder = new System.Text.StringBuilder();

                // Loop through each byte of the hashed data and format each one as a hexadecimal string
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }

                // Return the hexadecimal string
                return sBuilder.ToString();
            }
        }
    }
}
