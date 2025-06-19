using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace simpleDB
{
    public partial class Manage_Transactions : Form
    {
        public Manage_Transactions()
        {
            InitializeComponent();
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
        private void Update_grid()
        {
            string queryString = "select * from Trn_Src_Des;";
            SqlCommand command = new SqlCommand(queryString, Form1.con);
            command.CommandType = CommandType.Text;
            SqlDataAdapter da = new SqlDataAdapter(command);
            DataSet ds = new DataSet();
            da.Fill(ds, "ss");
            dataGridView1.DataSource = ds.Tables["ss"]; ;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            int old_voucherId = Convert.ToInt32(textBox1.Text);
            string queryString = "delete from dbo.Trn_Src_Des where cast(VoucherId as int) = " + old_voucherId + ";";
            SqlCommand command = new SqlCommand(queryString, Form1.con);
            try
            {
                int effected_row = command.ExecuteNonQuery();
                MessageBox.Show("Command successfully executed! effected row(s) number: " + effected_row.ToString());
            }
            catch (SqlException ex)
            {
                MessageBox.Show("[-] Error" + ex.Message);
            }
            Update_grid();
        }

        private void Manage_Transactions_Load(object sender, EventArgs e)
        {
            Update_grid();
        }
        private string set_voucher(int input)
        {
            string res = "";
            int cond = 10 - input.ToString().Length;
            for (int i = 0; i < cond; i++)
            {
                res += '0';
            }
            res += input.ToString();
            return res;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            int new_voucher = Convert.ToInt32(textBox2.Text);
            string new_set_voucher = set_voucher(new_voucher);
            string new_TrnDate = textBox3.Text;
            string new_TrnTime = textBox4.Text;
            long new_Amount = Convert.ToInt64(textBox5.Text);
            int new_src = Convert.ToInt32(textBox6.Text);
            int new_des = Convert.ToInt32(textBox7.Text);
            int new_branch_id = Convert.ToInt32(textBox8.Text);
            string new_desc = textBox9.Text;
            string queryString = "insert into dbo.Trn_Src_Des values (@vou, @date, @time, @amnt, @src, @des, @branch, @desc);";
            SqlCommand command = new SqlCommand(queryString, Form1.con);
            command.Parameters.Add("@vou", SqlDbType.Text);
            command.Parameters.Add("@date", SqlDbType.Date);
            command.Parameters.Add("@time", SqlDbType.Text);
            command.Parameters.Add("@amnt", SqlDbType.BigInt);
            command.Parameters.Add("@src", SqlDbType.Int);
            command.Parameters.Add("@des", SqlDbType.Int);
            command.Parameters.Add("@branch", SqlDbType.Int);
            command.Parameters.Add("@desc", SqlDbType.Text);
            command.Parameters["@vou"].Value = new_set_voucher;
            command.Parameters["@date"].Value = new_TrnDate;
            command.Parameters["@time"].Value = new_TrnTime;
            command.Parameters["@amnt"].Value = new_Amount;
            command.Parameters["@src"].Value = new_src;
            command.Parameters["@des"].Value = new_des;
            command.Parameters["@branch"].Value = new_branch_id;
            command.Parameters["@desc"].Value = new_desc;
            command.CommandType = CommandType.Text;
            try
            {
                command.ExecuteNonQuery();
                MessageBox.Show("new Transaction added successfully!");
            }
            catch (SqlException ex)
            {
                MessageBox.Show("[-] Error" + ex.Message);
            }
            Update_grid();
        }
    }
}
