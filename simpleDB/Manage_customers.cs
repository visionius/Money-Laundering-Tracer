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
    public partial class Manage_customers : Form
    {
        public Manage_customers()
        {
            InitializeComponent();
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
        private void Update_grid()
        {
            string queryString2 = "with temps as(select (cast(substring(NatCod,1,1) as int) * 10 +cast(substring(NatCod,2,1) as int) * 9 +cast(substring(NatCod,3,1) as int) * 8 +cast(substring(NatCod,4,1) as int) * 7 +cast(substring(NatCod,5,1) as int) * 6 +cast(substring(NatCod,6,1) as int) * 5 +cast(substring(NatCod,7,1) as int) * 4 +cast(substring(NatCod,8,1) as int) * 3 +cast(substring(NatCod,9,1) as int) * 2) % 11 as remind, NatCod, CID from dbo.Customer)" +
                                 "select Customer.CID, Customer.Name, Customer.NatCod, Customer.Birthdate, Customer.Address, Customer.Tel, case when currects.CID = Customer.CID then 1 else 0 end as NatCodeStatus from (select * from temps where temps.remind <=2  and cast(substring(temps.NatCod, 10, 1) as int) = temps.remind or temps.remind > 2 and cast(substring(temps.NatCod, 10, 1) as int) = 11 - temps.remind) as currects right join Customer on currects.CID = Customer.CID;";

            SqlCommand command2 = new SqlCommand(queryString2, Form1.con);
            command2.CommandType = CommandType.Text;
            SqlDataAdapter da = new SqlDataAdapter(command2);
            DataSet ds = new DataSet();
            da.Fill(ds, "ss");
            dataGridView1.DataSource = ds.Tables["ss"]; ;
        }
        private void Manage_customers_Load(object sender, EventArgs e)
        {
            Update_grid();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int new_CID =Convert.ToInt32(textBox2.Text);
            string new_Name = textBox3.Text;
            string new_NatCode = textBox4.Text;
            string new_Birthdate = textBox6.Text;
            string new_Address = textBox5.Text;
            string new_Tel = textBox7.Text;
            string queryString = "insert into dbo.Customer values (@new_CID, @name, @natCode, @birthdate, @address, @tel);";
            SqlCommand command = new SqlCommand(queryString, Form1.con);
            command.Parameters.Add("@new_CID", SqlDbType.Int);
            command.Parameters.Add("@name", SqlDbType.Text);
            command.Parameters.Add("@natCode", SqlDbType.Text);
            command.Parameters.Add("@birthdate", SqlDbType.Date);
            command.Parameters.Add("@address", SqlDbType.Text);
            command.Parameters.Add("@tel", SqlDbType.Text);
            command.Parameters["@new_CID"].Value = new_CID;
            command.Parameters["@name"].Value = new_Name;
            command.Parameters["@natCode"].Value = new_NatCode;
            command.Parameters["@birthdate"].Value = new_Birthdate;
            command.Parameters["@address"].Value = new_Address;
            command.Parameters["@tel"].Value = new_Tel;
            command.CommandType = CommandType.Text;
            try
            {
                command.ExecuteNonQuery();
                MessageBox.Show("new Customer added successfully!");
            }
            catch (SqlException ex)
            {
                MessageBox.Show("[-] Error" + ex.Message);
            }
            Update_grid();
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int old_CID = Convert.ToInt32(textBox1.Text);
            string queryString = "delete from dbo.Customer where CID = "+ old_CID.ToString() + ";";
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
    }
}
