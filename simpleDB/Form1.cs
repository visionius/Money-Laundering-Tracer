using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using Microsoft.Glee.Drawing;

namespace simpleDB
{
    public partial class Form1 : Form
    {
        //create new forms for add and delete any row in tables and show the results in their forms
        public static SqlConnection con;
        public static int[,] points;
        public static long[] amounts;
        public static string[] times;
        public static DateTime[] dateTimes;
        public static string[] description;
        public static int size = 0;
        public static bool dbConnectionEstablished = false;
        public Form1()
        {
            InitializeComponent();

        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            /*connection:
             * username: hossein
             * passwrd:  1111
            */

            string connectionString;
            string system_name = textBox2.Text;
            string database_name = textBox3.Text;
            string user_name = textBox4.Text;
            string passwrd = textBox5.Text;
            connectionString = @"Data Source=" + system_name + ";Initial Catalog=" + database_name + ";User ID=" + user_name + ";Password=" + passwrd + ";";
            con = new SqlConnection(connectionString);
            try
            {
                con.Open();
                MessageBox.Show("Connection Opend!");
                dbConnectionEstablished = true;
            }
            catch(SqlException ex)
            {
                MessageBox.Show("[-] Error" + ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dbConnectionEstablished)
            {
                string voucherID = textBox1.Text;
                /// need to chech all null values
                string queryString = "select Amount, SourceDep, DesDep, TrnTime, TrnDate from Trn_Src_Des where cast(VoucherId as int) = cast(" + voucherID + " as int);";
                SqlCommand command1 = new SqlCommand(queryString, con);
                SqlDataReader sqlread = command1.ExecuteReader();
                sqlread.Read();
                Int64 amount = sqlread.GetInt64(0);
                int src = sqlread.GetInt32(1);
                int des = sqlread.GetInt32(2);
                string trnTime = sqlread.GetString(3);
                DateTime trnDate = sqlread.GetDateTime(4);
                sqlread.Close();
                queryString = "select * from re_forward(@amnt, @des, @date, @time) union select * from re_backward(@amnt, @src, @date, @time);";
                SqlCommand command2 = new SqlCommand(queryString, con);
                command2.Parameters.Add("@time", SqlDbType.Text);
                command2.Parameters.Add("@date", SqlDbType.Date);
                command2.Parameters.Add("@src", SqlDbType.Int);
                command2.Parameters.Add("@amnt", SqlDbType.BigInt);
                command2.Parameters.Add("@des", SqlDbType.Int);
                command2.Parameters["@time"].Value = trnTime;
                command2.Parameters["@date"].Value = trnDate;
                command2.Parameters["@src"].Value = src;
                command2.Parameters["@amnt"].Value = amount;
                command2.Parameters["@des"].Value = des;
                command2.CommandType = CommandType.Text;
                //store source and destination in array
                //first get transaction numbers to be array size
                queryString = "select count(*) from re_forward(@amnt, @des, @date, @time);";
                SqlCommand command4 = new SqlCommand(queryString, con);
                command4.Parameters.Add("@time", SqlDbType.Text);
                command4.Parameters.Add("@date", SqlDbType.Date);
                command4.Parameters.Add("@des", SqlDbType.Int);
                command4.Parameters.Add("@amnt", SqlDbType.BigInt);
                command4.Parameters["@time"].Value = trnTime;
                command4.Parameters["@date"].Value = trnDate;
                command4.Parameters["@des"].Value = des;
                command4.Parameters["@amnt"].Value = amount;
                command4.CommandType = CommandType.Text;
                SqlDataReader sql_read = command4.ExecuteReader();
                sql_read.Read();
                Int32 size_trnsaction = sql_read.GetInt32(0);
                size = size_trnsaction;
                sql_read.Close();
                ///backward
                queryString = "select count(*) from re_backward(@amnt, @src, @date, @time);";
                SqlCommand command5 = new SqlCommand(queryString, con);
                command5.Parameters.Add("@time", SqlDbType.Text);
                command5.Parameters.Add("@date", SqlDbType.Date);
                command5.Parameters.Add("@amnt", SqlDbType.BigInt);
                command5.Parameters.Add("@src", SqlDbType.Int);
                command5.Parameters["@time"].Value = trnTime;
                command5.Parameters["@date"].Value = trnDate;
                command5.Parameters["@amnt"].Value = amount;
                command5.Parameters["@src"].Value = src;
                command5.CommandType = CommandType.Text;
                SqlDataReader sql_read2 = command5.ExecuteReader();
                sql_read2.Read();
                size += sql_read2.GetInt32(0);
                sql_read2.Close();
                size += 1;                      //selected transaction
                points = new int[size, 2];
                amounts = new Int64[size];
                times = new string[size];
                dateTimes = new DateTime[size];
                description = new string[size];
                SqlDataReader sqlreader = command2.ExecuteReader();
                int counter = 0;
                int index = 0;
                while (sqlreader.Read() && counter < size)
                {
                    if (!sqlreader.IsDBNull(4))
                    {
                        points[counter, 0] = sqlreader.GetInt32(4);
                    }
                    else
                    {
                        points[counter, 0] = index;
                        description[counter] = sqlreader.GetString(7);
                        index--;
                    }
                    if (!sqlreader.IsDBNull(5))
                    {
                        points[counter, 1] = sqlreader.GetInt32(5);

                    }
                    else
                    {
                        points[counter, 1] = index;
                        description[counter] = sqlreader.GetString(7);
                        index--;
                    }
                    amounts[counter] = sqlreader.GetInt64(3);
                    times[counter] = sqlreader.GetString(2);
                    dateTimes[counter] = sqlreader.GetDateTime(1);
                    counter++;

                }
                points[size - 1, 0] = src;
                points[size - 1, 1] = des;
                amounts[size - 1] = amount;
                times[size - 1] = trnTime;
                dateTimes[size - 1] = trnDate;
                sqlreader.Close();
                //////////////backward

                //draw
                Form2 frm2 = new Form2();
                frm2.Show();

                //grid show
                SqlDataAdapter da = new SqlDataAdapter(command2);
                DataSet ds = new DataSet();
                da.Fill(ds, "ss");
                dataGridView1.DataSource = ds.Tables["ss"]; ;
            }
            else
            {
                MessageBox.Show("Check DB connection", "Error", MessageBoxButtons.OK);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (dbConnectionEstablished)
            {
                string queryString = "select dp.Dep_ID, cu.CID, cu.Name, cu.NatCod, dpt.Dep_Typ_Desc, dps.Status_Desc, cu.BirthDate, cu.Address, cu.Tel from [Deposit] dp, [Customer] cu, [Deposit_Type] dpt, [Deposit_Status] dps where cu.CID = dp.CID and dp.Dep_Type = dpt.Dep_Type and dp.Status = dps.Status;";
                SqlCommand command = new SqlCommand(queryString, con);
                command.CommandType = CommandType.Text;
                SqlDataAdapter da = new SqlDataAdapter(command);
                DataSet ds = new DataSet();
                da.Fill(ds, "ss");
                dataGridView1.DataSource = ds.Tables["ss"]; ;
            }
            else
            {
                MessageBox.Show("Check DB connection", "Error", MessageBoxButtons.OK);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (dbConnectionEstablished)
            {
                string queryString = "select * from Trn_Src_Des;";
                SqlCommand command = new SqlCommand(queryString, con);
                command.CommandType = CommandType.Text;
                SqlDataAdapter da = new SqlDataAdapter(command);
                DataSet ds = new DataSet();
                da.Fill(ds, "ss");
                dataGridView1.DataSource = ds.Tables["ss"]; ;
            }
            else
            {
                MessageBox.Show("Check DB connection", "Error", MessageBoxButtons.OK);
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            
            
        }

        private void gViewer_Load(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (dbConnectionEstablished)
            {
                Manage_customers frmMgCu = new Manage_customers();
                frmMgCu.Show();
            }
            else
            {
                MessageBox.Show("Check DB connection", "Error", MessageBoxButtons.OK);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (dbConnectionEstablished)
            {
                Manage_Deposits frmMgDe = new Manage_Deposits();
                frmMgDe.Show();
            }
            else
            {
                MessageBox.Show("Check DB connection", "Error", MessageBoxButtons.OK);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (dbConnectionEstablished)
            {
                Manage_Transactions frmMgTr = new Manage_Transactions();
                frmMgTr.Show();
            }
            else
            {
                MessageBox.Show("Check DB connection", "Error", MessageBoxButtons.OK);
            }
        }
    }
}
