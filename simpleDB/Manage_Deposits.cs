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
    public partial class Manage_Deposits : Form
    {
        public Manage_Deposits()
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
            string queryString = "select dp.Dep_ID, dp.OpenDate, cu.CID, cu.Name, cu.NatCod, dpt.Dep_Typ_Desc, dps.Status_Desc, cu.BirthDate, cu.Address, cu.Tel from [Deposit] dp, [Customer] cu, [Deposit_Type] dpt, [Deposit_Status] dps where cu.CID = dp.CID and dp.Dep_Type = dpt.Dep_Type and dp.Status = dps.Status;";
            SqlCommand command = new SqlCommand(queryString, Form1.con);
            command.CommandType = CommandType.Text;
            SqlDataAdapter da = new SqlDataAdapter(command);
            DataSet ds = new DataSet();
            da.Fill(ds, "ss");
            dataGridView1.DataSource = ds.Tables["ss"]; ;
        }
        private void Manage_Deposits_Load(object sender, EventArgs e)
        {
            Update_grid();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int old_Dep_ID = Convert.ToInt32(textBox1.Text);
            string queryString = "delete from dbo.Deposit where Dep_ID = " + old_Dep_ID.ToString() + ";";
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

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            int new_Dep_ID = Convert.ToInt32(textBox2.Text);
            int new_Dep_Type = Convert.ToInt32(textBox3.Text);
            int new_CID = Convert.ToInt32(textBox4.Text);
            string new_date = textBox5.Text;
            int new_status = Convert.ToInt32(textBox6.Text);
            string queryString = "insert into dbo.Deposit values (@new_Dep, @type, @CID, @date, @status);";
            SqlCommand command = new SqlCommand(queryString, Form1.con);
            command.Parameters.Add("@new_Dep", SqlDbType.Int);
            command.Parameters.Add("@type", SqlDbType.Int);
            command.Parameters.Add("@CID", SqlDbType.Int);
            command.Parameters.Add("@date", SqlDbType.Date);
            command.Parameters.Add("@status", SqlDbType.Int);
            command.Parameters["@new_Dep"].Value = new_Dep_ID;
            command.Parameters["@type"].Value = new_Dep_Type;
            command.Parameters["@CID"].Value = new_CID;
            command.Parameters["@date"].Value = new_date;
            command.Parameters["@status"].Value = new_status;
            command.CommandType = CommandType.Text;
            try
            {
                command.ExecuteNonQuery();
                MessageBox.Show("new Deposit added successfully!");
            }
            catch (SqlException ex)
            {
                MessageBox.Show("[-] Error" + ex.Message);
            }
            Update_grid();
        }
    }
}
