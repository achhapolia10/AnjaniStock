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
    public partial class DailyNewEntry : Form
    {
        Database1DataSet1TableAdapters.CFTABLETableAdapter CFTABLETableAdapter = new Database1DataSet1TableAdapters.CFTABLETableAdapter();
        
        List<Products> products = new List<Products>();
        int packets = 0, boxes = 0;
        List<string> labourName = new List<string>();
        Database1DataSet1TableAdapters.DAILYENTRYTABLETableAdapter dailyEntryTableAdapter = new Database1DataSet1TableAdapters.DAILYENTRYTABLETableAdapter();
        Database1DataSet1TableAdapters.LabourNameTableAdapter LabourNameTableAdapter = new Database1DataSet1TableAdapters.LabourNameTableAdapter();
        public DailyNewEntry()
        {
            InitializeComponent();
        }

        private void DailyNewEntry_Load(object sender, EventArgs e)
        {
            bindProductList();
            bindDataGridView(DateTime.Today.ToString("dd/MM/yyyy"),(string)productNameList.SelectedValue);
            labourNameLoader();
            bindToNameTextBox();
            getCarryForward();
            bindTotal(DateTime.Today.ToString("dd/MM/yyyy"), (string)productNameList.SelectedValue);
           
              

        }

#pragma warning disable IDE1006 // Naming Styles
        public void bindDataGridView(string date, string name)
#pragma warning restore IDE1006 // Naming Styles
        {
            int index = products.FindIndex(product => product.Name == (string)productNameList.SelectedValue);
            int id = products[index].Id;
            string query = "SELECT NAME1,[BOXES PACKED],[PACKETS] from ["+id+"JOURNAL] WHERE DATE1='" + date+"';";
            using (OleDbConnection connection = new OleDbConnection(Properties.Settings.Default.Database1ConnectionString1))
            {
                connection.Open();
                using (OleDbCommand command = new OleDbCommand("DELETE FROM [" + id + "JOURNAL] WHERE PACKETS=0 AND [BOXES PACKED]=0;", connection))
                {
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();
                }
                using (OleDbCommand cmd = new OleDbCommand(query, connection))
                {
                    cmd.CommandType = CommandType.Text;

                    using (OleDbDataAdapter adapter = new OleDbDataAdapter(cmd))
                    {
                        using (DataTable dt = new DataTable())
                        {

                            adapter.Fill(dt);
                            dataGridView1.ClearSelection();
                            dt.Columns[0].ColumnName = "Name";
                            this.dataGridView1.DataSource = dt;
                            
                            
                        }
                    }
                }
                    connection.Close();
            }
            dataGridView1.ShowCellToolTips = true;
        }

        public void bindTotal(string date, string name)
        {
            int index = products.FindIndex(product => product.Name == (string)productNameList.SelectedValue);
            int id = products[index].Id;
            try
            {
                if (carrForwardText.Text != "")
                    packets = int.Parse(carrForwardText.Text);
                else
                {
                    packets = 0;
                }
            }
            catch(Exception)
            {
                packets = 0;
            }
            boxes = 0;

            string query = "SELECT NAME1,[BOXES PACKED],[PACKETS] from [" + id + "JOURNAL] WHERE DATE1='" + date + "';";
            using (OleDbConnection connection = new OleDbConnection(Properties.Settings.Default.Database1ConnectionString1))
            {
                connection.Open();
                using (OleDbCommand cmd = new OleDbCommand(query, connection))
                {
                    cmd.CommandType = CommandType.Text;

                    using (OleDbDataReader reader = cmd.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            boxes += reader.GetInt32(1);
                            packets += reader.GetInt32(2);
                            
                        }
                    
                    }
                }
                connection.Close();
            }
            resloveBoxes();
            label7.Text ="Total: "+boxes + " Boxes "+
             packets + " Packets";

        }
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
                        while(reader.Read())
                        {
                            products.Add(new Products(reader.GetString(1), reader.GetDouble(2)
                                , reader.GetInt32(4), reader.GetInt32(3), 0, 0,reader.GetDouble(7),
                                reader.GetInt32(0)));
                        }
                        BindingSource bindingSource = new BindingSource
                        {
                            DataSource = products
                        };
                        productNameList.DataSource = bindingSource;
                        productNameList.DisplayMember = "name";
                        productNameList.ValueMember = "name";
                    }
                }
                connection.Close();
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            try
            {
                if (checkDoubleEntry(nameTextBox.Text))
                {
                    EntryInsert(nameTextBox.Text.ToUpper(),
                        int.Parse(boxesPackedTextBox.Text),
                        int.Parse(packetsTextBox.Text),
                        dateTimePicker1.Value.ToString("dd/MM/yyyy"), (string)productNameList.SelectedValue);

                    bindDataGridView(
                        dateTimePicker1.Value.ToString("dd/MM/yyyy"), (string)productNameList.SelectedValue);
                    packets += int.Parse(packetsTextBox.Text);
                    boxes += int.Parse(boxesPackedTextBox.Text);
                    updateStock(int.Parse(boxesPackedTextBox.Text), int.Parse(packetsTextBox.Text));
                    this.resloveBoxes();
                    label7.Text = "Total: " + boxes + " Boxes " +
                    packets + " Packets";
                    checkList(nameTextBox.Text);
                    dataGridView1.ScrollBars = ScrollBars.Vertical;
                    if (dataGridView1.Rows.Count > 0)
                    {

                        dataGridView1.FirstDisplayedScrollingRowIndex = dataGridView1.Rows.Count - 1 < 0 ? 0 : dataGridView1.Rows.Count - 1;
                    }
                        nameTextBox.Text = "";
                    boxesPackedTextBox.Text = "0";
                    packetsTextBox.Text = "0";
                }
                else
                {
                    MessageBox.Show("Name Already Entered!!", "Error", MessageBoxButtons.OK);
                }
                  

            }
            catch (FormatException)
            {
                MessageBox.Show("Wrong entry", "Error", MessageBoxButtons.OK);
            }
            finally
            {
                nameTextBox.Focus();
            }

        }

        private void EntryInsert(string v1, int v2, int v3, string v4, string selectedValue)
        {
            int index = products.FindIndex(product => product.Name == (string)productNameList.SelectedValue);
            int id = products[index].Id;
            string query = "INSERT INTO [" + id + "JOURNAL] VALUES('" + v1 + "'," + v2 + "," + v3 + ",'" + v4 + "');";
            using (OleDbConnection connection = new OleDbConnection(Properties.Settings.Default.Database1ConnectionString1))
            {

                connection.Open();
                using (OleDbCommand command = new OleDbCommand(query, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();
                }
            }
        }

       

        private void nameTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (nameTextBox.Text != "")
                if (e.KeyCode == Keys.Enter)
                {
                    boxesPackedTextBox.Focus();

                    e.Handled = true;
                    e.SuppressKeyPress = true;
                }
        }

        private void boxesPackedTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                packetsTextBox.Focus();

                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void packetsTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button1_Click(this, new EventArgs());

                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            bindDataGridView(dateTimePicker1.Value.ToString("dd/MM/yyyy"), (string)productNameList.SelectedValue);
            getCarryForward();
              
            bindTotal(dateTimePicker1.Value.ToString("dd/MM/yyyy"), (string)productNameList.SelectedValue);
        }

        public void labourNameLoader()
        {
            string query = "SELECT [Labour Name] from LabourName;";
            using (OleDbConnection connection = new OleDbConnection(Properties.Settings.Default.Database1ConnectionString1))
            {
                connection.Open();
                using (OleDbCommand cmd = new OleDbCommand(query, connection))
                {
                    cmd.CommandType = CommandType.Text;

                    using (OleDbDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            labourName.Add(sdr.GetString(0));
                        }
                    }
                }
                connection.Close();
            }

        }

        public void checkList(string name)
        {
            if(!labourName.Contains(name))
            {
                labourName.Add(name);
                LabourNameTableAdapter.Insert(name);
                bindToNameTextBox();
            }
            
        }

        public void bindToNameTextBox()
        {
            nameTextBox.AutoCompleteCustomSource.Clear() ;
            nameTextBox.AutoCompleteCustomSource.AddRange(labourName.ToArray());

        }

        private void productNameList_SelectionChangeCommitted(object sender, EventArgs e)
        {
            bindDataGridView(dateTimePicker1.Value.ToString("dd/MM/yyyy"), (string)productNameList.SelectedValue);
            getCarryForward();
            bindTotal(dateTimePicker1.Value.ToString("dd/MM/yyyy"), (string)productNameList.SelectedValue);
              
        }


        string nameBeforeEdit = "";
        int beforeEdit = 0;


        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if(e.ColumnIndex==0)
            {
                if(nameBeforeEdit!= (string)(((DataGridView)sender).Rows[e.RowIndex].Cells[0].Value))
                if (checkDoubleEntry((string)(((DataGridView)sender).Rows[e.RowIndex].Cells[0].Value)))
                {
                    updateName(((string)(((DataGridView)sender).Rows[e.RowIndex].Cells[0].Value)).ToUpper());
                    ((DataGridView)sender).Rows[e.RowIndex].Cells[0].Value = ((string)((DataGridView)sender).Rows[e.RowIndex].Cells[0].Value).ToUpper();
                }
                else
                {
                    MessageBox.Show("Name Already Entered!!", "Error", MessageBoxButtons.OK);
                    ((DataGridView)sender).Rows[e.RowIndex].Cells[0].Value = nameBeforeEdit;
                }
            }
            if(e.ColumnIndex==1)
            {
                updateBox((int)(((DataGridView)sender).Rows[e.RowIndex].Cells[1].Value),
                    (string)(((DataGridView)sender).Rows[e.RowIndex].Cells[0].Value));
                updateStock(-beforeEdit, 0);
                updateStock((int)(((DataGridView)sender).Rows[e.RowIndex].Cells[1].Value),0);
            }
            if(e.ColumnIndex==2)
            {
                updatePackets((int)(((DataGridView)sender).Rows[e.RowIndex].Cells[2].Value),
                    (string)(((DataGridView)sender).Rows[e.RowIndex].Cells[0].Value));
                updateStock(0,-beforeEdit);
                updateStock(0,(int)(((DataGridView)sender).Rows[e.RowIndex].Cells[2].Value));
            }
            bindTotal(dateTimePicker1.Value.ToString("dd/MM/yyyy"), (string)productNameList.SelectedValue);
              
        }

        private void productNameList_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void resloveBoxes()
        {
            int index = products.FindIndex(product => product.Name == (string)productNameList.SelectedValue);
            int capacity = products[index].BoxCapacity;
            int remainder = 0;
            remainder = packets % capacity;
            boxes += packets / capacity;
            packets = remainder;
        }

        private int[] resloveBoxes(int[] a)
        {
            int index = products.FindIndex(product => product.Name == (string)productNameList.SelectedValue);
            int capacity = products[index].BoxCapacity;
            int boxes=a[0]+ a[1] / capacity;
            int packets = a[1] % capacity;
            return new int[] { boxes, packets };
        }

        private void carrForwardText_TextChanged(object sender, EventArgs e)
        {
            changeCFTable();
            bindTotal(dateTimePicker1.Value.ToString("dd/MM/yyyy"), (string)productNameList.SelectedValue);
        }

        private void getCarryForward()
        {
            string date= dateTimePicker1.Value.ToString("dd/MM/yyyy");
            string name = (string)productNameList.SelectedValue;
            string query="SELECT [Carry Forward] FROM CFTABLE WHERE DATE1='"+date+"' AND PRODUCT='"+name+"';";
            using (OleDbConnection connection = new OleDbConnection(Properties.Settings.Default.Database1ConnectionString1))
            {
                connection.Open();
                using (OleDbCommand cmd = new OleDbCommand(query, connection))
                {
                    cmd.CommandType = CommandType.Text;
                     
                    using (OleDbDataReader sdr = cmd.ExecuteReader())
                    {
                        if (sdr.Read())
                        {
                            carrForwardText.Text = ""+sdr.GetInt32(0);
                        }
                        else
                        {
                            CFTABLETableAdapter.Insert(0, date, name);
                            carrForwardText.Text = "0";
                        }
                        
                    }
                }
                connection.Close();
            }
        }

        

        private void carrForwardText_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void dataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.ColumnIndex == 0)
                nameBeforeEdit = (string)(((DataGridView)sender).Rows[e.RowIndex].Cells[0].Value);

            if (e.ColumnIndex == 1)
                beforeEdit = (int)(((DataGridView)sender).Rows[e.RowIndex].Cells[1].Value);

            if (e.ColumnIndex == 2)
                beforeEdit = (int)(((DataGridView)sender).Rows[e.RowIndex].Cells[2].Value);
        }

        void updateName(string name)
        {
            string date = dateTimePicker1.Value.ToString("dd/MM/yyyy");
            string productname = (string)productNameList.SelectedValue;
            int index = products.FindIndex(product => product.Name == (string)productNameList.SelectedValue);
            int id = products[index].Id;
            string query = "UPDATE [" + id + "JOURNAL] SET NAME1='" + name + "' WHERE NAME1='" + nameBeforeEdit +
                "' AND DATE1='" + date + "' ;";
            using (OleDbConnection connection = new OleDbConnection(Properties.Settings.Default.Database1ConnectionString1))
            {
                connection.Open();
                using (OleDbCommand cmd = new OleDbCommand(query, connection))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                }
                connection.Close();
            }
        }
        void updateBox(int value,string name)
        {
            int index = products.FindIndex(product => product.Name == (string)productNameList.SelectedValue);
            int id = products[index].Id;
            string date = dateTimePicker1.Value.ToString("dd/MM/yyyy");
            string query = "UPDATE [" +id+ "JOURNAL] SET [BOXES PACKED]='" + value + "' WHERE NAME1='" + name +
                "' AND DATE1='" + date + "';";
            using (OleDbConnection connection = new OleDbConnection(Properties.Settings.Default.Database1ConnectionString1))
            {
                connection.Open();
                using (OleDbCommand cmd = new OleDbCommand(query, connection))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                }
                connection.Close();
            }
        }
        void updatePackets(int value,string name)
        {
            int index = products.FindIndex(product => product.Name == (string)productNameList.SelectedValue);
            int id = products[index].Id;
            string date = dateTimePicker1.Value.ToString("dd/MM/yyyy");
            string query = "UPDATE  [" + id + "JOURNAL] SET [PACKETS]='" + value + "' WHERE NAME1='" + name+
                "' AND DATE1='" + date + "';";
            using (OleDbConnection connection = new OleDbConnection(Properties.Settings.Default.Database1ConnectionString1))
            {
                connection.Open();
                using (OleDbCommand cmd = new OleDbCommand(query, connection))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                }
                connection.Close();
            }
        }

        private void totalBoxes_Click(object sender, EventArgs e)
        {

        }

        void changeCFTable()
        {
           string date = dateTimePicker1.Value.ToString("dd/MM/yyyy");
            string name = (string)productNameList.SelectedValue;
            string query1 = "DELETE FROM CFTABLE WHERE DATE1='" + date + "' AND PRODUCT='" + name + "';";
            using (OleDbConnection connection = new OleDbConnection(Properties.Settings.Default.Database1ConnectionString1)) 
            {
                connection.Open();
                using (OleDbCommand cmd = new OleDbCommand(query1, connection))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                }
                connection.Close();
            }
            try
            {
                if (carrForwardText.Text != "")
                    CFTABLETableAdapter.Insert(int.Parse(carrForwardText.Text), date, name);
            }
            catch(Exception)
            { }
            
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        public bool checkDoubleEntry(string name)
        {
            int index = products.FindIndex(product => product.Name == (string)productNameList.SelectedValue);
            int id = products[index].Id;
            string date = dateTimePicker1.Value.ToString("dd/MM/yyyy");
            string productName = (string)productNameList.SelectedValue;
            string query1 = "SELECT NAME1 FROM [" + id+ "JOURNAL] WHERE NAME1='" + name+
                "' AND DATE1='" + date + "';";
            bool hasEntry = false;
            using (OleDbConnection connection = new OleDbConnection(Properties.Settings.Default.Database1ConnectionString1))
            {
                connection.Open();
                using (OleDbCommand cmd = new OleDbCommand(query1, connection))
                {
                    cmd.CommandType = CommandType.Text;
                    using (OleDbDataReader reader = cmd.ExecuteReader())
                    {
                        if(!reader.Read())
                        {
                            hasEntry = true;
                        }
                    }
                }
            }
            return hasEntry;
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            onDeletePress();
              
        }

        public void onDeletePress()
        {
            DataGridViewSelectedCellCollection selectedCellCollection = dataGridView1.SelectedCells;
            List<int> selectedRows = new List<int>();
            int index = products.FindIndex(product => product.Name == (string)productNameList.SelectedValue);
            int id = products[index].Id;
            foreach (DataGridViewCell cell in selectedCellCollection)
            {
                if (!selectedRows.Contains(cell.RowIndex))
                {
                    selectedRows.Add(cell.RowIndex);
                }
            }
            string name;

            string date = dateTimePicker1.Value.ToString("dd/MM/yyyy");
            using (OleDbConnection connection = new OleDbConnection(Properties.Settings.Default.Database1ConnectionString1))
            {
                connection.Open();
                foreach (int i in selectedRows)
                {
                    name = (string)dataGridView1.Rows[i].Cells[0].Value;
                    string query= " SELECT [BOXES PACKED],PACKETS FROM [" + id + "JOURNAL] WHERE DATE1='" + date
                        + "' AND NAME1='" + name + "';";
                    using (OleDbCommand cmd = new OleDbCommand(query, connection))
                    {
                        cmd.CommandType = CommandType.Text;
                        using (OleDbDataReader reader = cmd.ExecuteReader())
                        {
                            while(reader.Read())
                            {
                                updateStock(-reader.GetInt32(0), -reader.GetInt32(1));
                            }
                        }
                    }
                }
                foreach (int i in selectedRows)
                {
                    name = (string)dataGridView1.Rows[i].Cells[0].Value;
                    string query = @" DELETE FROM  [" + id + "JOURNAL] WHERE DATE1='" + date
                        + "' AND  NAME1='" + name + "';";
                    using(OleDbCommand cmd=new OleDbCommand(query, connection))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.ExecuteNonQuery();
                    }
                }
                connection.Close();

            }
            bindDataGridView(date,productNameList.SelectedText);
            
        }

        private void updateStock(int boxes, int packets)
        {
            int  STboxes=0, STpacket=0;
            int index = products.FindIndex(product => product.Name == (string)productNameList.SelectedValue);
            int id = products[index].Id;
            using (OleDbConnection connection = new OleDbConnection(Properties.Settings.Default.Database1ConnectionString1))
            {
                string query = "SELECT [BOXES RECIEVED], [PACKETS RECIEVED] FROM ["+
                    id+"STOCK] WHERE DATE1='"+
                    dateTimePicker1.Value.ToString("dd/MM/yyyy")+"';";
                connection.Open();
                using (OleDbCommand command=new OleDbCommand(query,connection))
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
                            query = "INSERT INTO [" +id + "STOCK]" +
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
                    " [BOXES RECIEVED]='" + STboxes + "' , [PACKETS RECIEVED]='"
                    + STpacket + "' WHERE DATE1='"+dateTimePicker1.Value.ToString("dd/MM/yyyy") + "'";
                using(OleDbCommand command=new OleDbCommand(query1,connection))
                {
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();
                }
            }

        }

        private void DailyNewEntry_Activated(object sender, EventArgs e)
        {
              
        }

        private void nameTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
                DeleteButton.PerformClick();
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
            if(boxes>0&& packets<0)
            {
                while (packets < 0)
                {
                    boxes--;
                    packets = capacity + packets;
                }
            }
            return new int[] { boxes, packets };
        }


        public void setValues(DateTime date, string name)
        {
            dateTimePicker1.Value = date;
            productNameList.SelectedIndex = products.FindIndex(p => p.Name == name);
            bindDataGridView(date.ToString("dd/MM/yyyy"),name);
            bindTotal(date.ToString("dd/MM/yyyy"), name);
            getCarryForward();
        }
    }
    
}
