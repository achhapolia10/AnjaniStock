using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Stock1
{
    public partial class Form2 : Form
    {
        public IListener listener;
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            getProductNames();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            this.Close();
        }
        private List<string> name;
        public void getProductNames()
        {
            if(name==null)
            {
                name = new List<string>();
                string query = "SELECT NAME1 FROM Product;";
                using(OleDbConnection connection=new OleDbConnection(Properties.Settings.Default.Database1ConnectionString1))
                {
                    connection.Open();
                    using (OleDbCommand command = new OleDbCommand(query, connection))
                    {
                        using (OleDbDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                                name.Add(reader.GetString(0));

                        }
                    }
                    connection.Close();
                }
            }
        }
        private void saveButton_Click(object sender, EventArgs e)
        {
            if (addProduct.Text == "" || addPacket.Text == "" || addBox.Text == "" || addWeight.Text == "")
            {
                addProduct.Focus();
                MessageBox.Show("Enter all boxes", "Error");
            }
            /*else if (addProduct.Text.Contains("."))
            {
                addProduct.Focus();
                MessageBox.Show("Can't contain Period (.)", "Error");
                
            }*/
            else if(name.Contains(addProduct.Text))
            {

                MessageBox.Show("Duplicate Product", "Error");
            }
            else {

                Database1DataSet1TableAdapters.ProductTableAdapter productTableAdapter =
                    new Database1DataSet1TableAdapters.ProductTableAdapter();
                productTableAdapter.Insert(addProduct.Text.ToUpper(), double.Parse(addWeight.Text),
                    int.Parse(addPacket.Text), int.Parse(addBox.Text), Convert.ToInt32(OpeningStockBoxes.Text), Convert.ToInt32(OpeningStockPackets.Text), float.Parse(priceTextBox1.Text), Convert.ToInt32(OpeningStockBoxes.Text), Convert.ToInt32(OpeningStockPackets.Text), "16/06/2018");
                name.Add(addProduct.Text);
                using (OleDbConnection connection = new OleDbConnection(Properties.Settings.Default.Database1ConnectionString1))
                {
                    int id;
                    connection.Open();
                    string query = "SELECT ID FROM Product WHERE NAME1='"+addProduct.Text+"';";
                    using (OleDbCommand command = new OleDbCommand(query, connection))
                    {
                        using (OleDbDataReader reader = command.ExecuteReader())
                        {
                            reader.Read();
                            id = reader.GetInt32(0);
                            
                        }
                    }

                    query = "CREATE TABLE [" + id + "JOURNAL](NAME1 varchar(50), [BOXES PACKED] int, PACKETS int,DATE1 varchar(50));";
                    using (OleDbCommand command = new OleDbCommand(query, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                    query = "CREATE TABLE [" + id + "STOCK](DATE1 varchar(50), [BOXES RECIEVED] int," +
                        " [PACKETS RECIEVED] int,[BOXES DISPATCHED] int ,[PACKETS DISPATCHED] int);";
                    using (OleDbCommand command = new OleDbCommand(query, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                }
                this.addProduct.Text = "";
                this.addWeight.Text = "";
                this.addPacket.Text = "0";
                this.addBox.Text = "0";
                this.OpeningStockBoxes.Text = "0";
                this.OpeningStockPackets.Text = "0";
                this.priceTextBox1.Text = "6.5";
                this.addProduct.Focus();
                if (listener != null)
                {
                    listener.onClickSave();
                }
            }
        }

        private void addProduct_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                addWeight.Focus();

                e.Handled = true;
                e.SuppressKeyPress = true;
            }

        }

        

        private void addPacket_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                addBox.Focus();

                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void addBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                OpeningStockBoxes.Focus();

                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void addWeight_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                addPacket.Focus();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void priceTextBox1_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys.Enter)
            {
                saveButton.Focus();

                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }
        public interface IListener
        {
            void onClickSave();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void OpeningStockBoxes_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                OpeningStockPackets.Focus();

                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void OpeningStockPackets_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                priceTextBox1.Focus();

                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }
    }
}
