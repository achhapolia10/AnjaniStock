using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;

namespace Stock1
{
    public partial class DispatchForm : Form
    {
        Database1DataSet1TableAdapters.DISPATCHTABLETableAdapter dispatchAdapter = new Database1DataSet1TableAdapters.DISPATCHTABLETableAdapter();
        public DispatchForm()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {
           
        }
        List<Products> products;
        public void bindProductList()
        {
            string query = "SELECT * from Product ORDER BY NAME1 ASC;";
            products = new List<Products>();
            using (OleDbConnection connection = new OleDbConnection(Properties.Settings.Default.Database1ConnectionString1))
            {
                connection.Open();
                using (OleDbCommand cmd = new OleDbCommand(query, connection))
                {
                    cmd.CommandType = CommandType.Text;

                    using (OleDbDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            products.Add(new Products(reader.GetString(1), reader.GetDouble(2)
                                , reader.GetInt32(4), reader.GetInt32(3), 0, 0, reader.GetDouble(7),reader.GetInt32(0)));
                        }
                        
                        BindingSource bindingSource = new BindingSource
                        {
                            DataSource = products
                        };
                        ProductComboBox.DataSource = bindingSource;
                        ProductComboBox.DisplayMember = "Name";
                        ProductComboBox.ValueMember = "Name";
                    }
                }
                connection.Close();
            }
        }

        private void DispatchForm_Load(object sender, EventArgs e)
        {
            bindProductList();
            bindDataGridView();
        }

        private void BoxTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                PacketTextBox.Focus();

                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void PacketTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SaveButton.Focus();

                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private int getBoxes()
        {
            if (BoxTextBox.Text == "")
                return 0;
            else
                return int.Parse(BoxTextBox.Text);
        }
        private int getPackets()
        {
            if (PacketTextBox.Text == "")
                return 0;
            else
                return int.Parse(PacketTextBox.Text);
        }


        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (products.FindIndex(p => p.Name == ProductComboBox.Text) >= 0)
            {
                dispatchAdapter.Insert((string)ProductComboBox.SelectedValue, getBoxes(), getPackets(), dateTimePicker1.Value.ToString("dd/MM/yyyy"));
                updateStock(getBoxes(), getPackets(), (string)ProductComboBox.SelectedValue);

                ProductComboBox.Focus();
                BoxTextBox.Text = 0 + "";
                PacketTextBox.Text = 0 + "";
                bindDataGridView();
            }
            else
            {
                MessageBox.Show("No product of such name", "Error", MessageBoxButtons.OK);
                ProductComboBox.Focus();
            }
        }

        private void bindDataGridView()
        {
            DataTable dataTable = new DataTable();

            dataTable.Columns.Add("Date", typeof(string));
            dataTable.Columns.Add("Product", typeof(string));
            dataTable.Columns.Add("Boxes", typeof(int));
            dataTable.Columns.Add("Packets", typeof(int));

            DataRow row;
            string query = @"SELECT  [NAME1], [BOX], [PACKETS] FROM 
                DISPATCHTABLE WHERE Date1='" + dateTimePicker1.Value.ToString("dd/MM/yyyy")+"';";
            using(OleDbConnection connection=new OleDbConnection(Properties.Settings.Default.Database1ConnectionString1))
            {
                connection.Open();
                using (OleDbCommand command = new OleDbCommand(query, connection))
                {
                    command.CommandType = CommandType.Text;
                    using(OleDbDataReader reader= command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            row = dataTable.NewRow();
                            row[0] = dateTimePicker1.Value.ToString("dd/MM/yyyy");
                            row[1] = reader.GetString(0);
                            row[2] = reader.GetInt32(1);
                            row[3] = reader.GetInt32(2);
                            
                            dataTable.Rows.Add(row);

                        }
                    }
                }
                connection.Close();
            }
            dataGridView1.DataSource = dataTable;
            
        }

        private void Delete_Click(object sender, EventArgs e)
        {
            onDeletePress();
        }

        public void onDeletePress()
        {
            DataGridViewSelectedCellCollection selectedCellCollection = dataGridView1.SelectedCells;
            List<int> selectedRows = new List<int>();

            foreach (DataGridViewCell cell in selectedCellCollection)
            {
                if (!selectedRows.Contains(cell.RowIndex))
                {
                    selectedRows.Add(cell.RowIndex);
                }
            }
            foreach(int i in selectedRows)
            {
                string product =(string) dataGridView1.Rows[i].Cells[1].Value;
                string date = (string)dataGridView1.Rows[i].Cells[0].Value;
                int box = (int)dataGridView1.Rows[i].Cells[2].Value;
                int packet = (int)dataGridView1.Rows[i].Cells[3].Value;
                updateStock(-box, -packet,product);
            }
            using (OleDbConnection connection = new OleDbConnection(Properties.Settings.Default.Database1ConnectionString1 ))
            {
                connection.Open();
                string query;
                foreach (int i in selectedRows)
                {
                    string product = (string)dataGridView1.Rows[i].Cells[1].Value;
                    string date = (string)dataGridView1.Rows[i].Cells[0].Value;
                    int box = (int)dataGridView1.Rows[i].Cells[2].Value;
                    int packet = (int)dataGridView1.Rows[i].Cells[3].Value;
                    query = "DELETE FROM DISPATCHTABLE WHERE DATE1='" + date + "' AND NAME1='" + product +
                        "' AND BOX=" + box + " AND PACKETS=" + packet + ";";
                    using(OleDbCommand command=new OleDbCommand(query,connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.ExecuteNonQuery();
                    }
                }
                connection.Close();
            }
            bindDataGridView();
        }
        private void updateStock(int boxes, int packets,string name)
        {
            int  STboxes = 0, STpacket = 0;
            int index = products.FindIndex(product => product.Name == name);
            int id = products[index].Id;
            using (OleDbConnection connection = new OleDbConnection(Properties.Settings.Default.Database1ConnectionString1))
            {
                string query = "SELECT [BOXES DISPATCHED],[PACKETS DISPATCHED] FROM [" + id + "STOCK] WHERE DATE1='" +
                    dateTimePicker1.Value.ToString("dd/MM/yyyy") + "';";
                connection.Open();
                using (OleDbCommand command = new OleDbCommand(query, connection))
                {
                    command.CommandType = CommandType.Text;
                    using (OleDbDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            STboxes = reader.GetInt32(0);
                            STpacket = reader.GetInt32(1);
                        }
                        else
                        {
                            query = "INSERT INTO [" + id + "STOCK]" +
                                "VALUES('" + dateTimePicker1.Value.ToString("dd/MM/yyyy") + "',0,0,0,0);";
                            using (OleDbCommand command1 = new OleDbCommand(query, connection))
                            {
                                command1.CommandType = CommandType.Text;
                                command1.ExecuteNonQuery();

                            }
                        }
                    }
                }

                STboxes += boxes;
                STpacket += packets;
                int[] a = resloveBoxes(new int[] { STboxes, STpacket });
                STboxes = a[0];
                STpacket = a[1];
                string query1 = "UPDATE [" + id + "STOCK] SET" +
                    " [BOXES DISPATCHED]='" + STboxes + "' , [PACKETS DISPATCHED]='"
                    + STpacket + "' WHERE DATE1='" + dateTimePicker1.Value.ToString("dd/MM/yyyy") + "'";
                using (OleDbCommand command = new OleDbCommand(query1, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();
                }
            }

        }
        private int[] resloveBoxes(int[] a)
        {
            int index = products.FindIndex(product => product.Name == (string)ProductComboBox.SelectedValue);
            int capacity = products[index].BoxCapacity;
            int boxes = a[0] + a[1] / capacity;
            int packets = a[1] % capacity;
            return new int[] { boxes, packets };
        }
        private int[] equateBoxes(int boxes, int packets, int capacity)
        {
           
            boxes += packets / capacity;
            packets = packets % capacity;
            while (packets < -capacity)
            {
                boxes--;
                packets = capacity + packets;
            }

            return new int[] { boxes, packets };
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            bindDataGridView();
        }
        public void setValues(DateTime date)
        {
            this.dateTimePicker1.Value = date;
            this.bindDataGridView();
        }

        private void ProductComboBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (products.FindIndex(p => p.Name == ProductComboBox.Text) >= 0)
                {
                    BoxTextBox.Focus();

                    e.Handled = true;
                    e.SuppressKeyPress = true;
                }
                else
                {
                    MessageBox.Show("No product of such name","Error",MessageBoxButtons.OK);
                    
                }
            }
        }
    }
}
