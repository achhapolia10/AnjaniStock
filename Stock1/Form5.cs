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
    public partial class Form5 : Form
    {
        private string productName;
        private DateTime fromDate;
        private DateTime toDate;
        DataTable data;
        Products product;
        cellClickListener clickListener;
        public Form5(string productName)
        {
            InitializeComponent();
            this.productName = productName;
            this.fromDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            this.toDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month));
        }
        public Form5(string productName, DateTime fromDate, DateTime toDate)
        {

            InitializeComponent();
            this.productName = productName;
            this.fromDate = fromDate;
            this.toDate = toDate;
        }
        private void Form5_Load(object sender, EventArgs e)
        {
            clickListener = (cellClickListener)MdiParent;
            this.Text = productName;
            FromDateTimePicker.Value = fromDate;
            TODateTimePicker.Value = toDate;

            GetProductDetails();
            onLoad();
        }
        private void GetProductDetails()
        {
            string query;
            using (OleDbConnection connection = new OleDbConnection(Properties.Settings.Default.Database1ConnectionString1))
            {
                connection.Open();
                query = "SELECT BOX,[OPENING BOXES],[OPENING PACKETS],[DATE ADDED],ID FROM Product WHERE NAME1='"
                    + productName + "';";
                using (OleDbCommand command = new OleDbCommand(query, connection))
                {
                    using (OleDbDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            product = new Products(productName, reader.GetInt32(0),
                                reader.GetInt32(1), reader.GetInt32(2), reader.GetString(3), reader.GetInt32(4));
                        }
                    }
                }
                connection.Close();
            }
        }
        private void LoadButton_Click(object sender, EventArgs e)
        {
            if (fromDate.Date <= toDate.Date)
            {
                fromDate = FromDateTimePicker.Value.Date;
                toDate = TODateTimePicker.Value.Date;
                onLoad();
            }
            else
                MessageBox.Show("From Date smaller than TO date");
        }
        private void FromDateTimePicker_ValueChanged(object sender, EventArgs e)
        {
            if (FromDateTimePicker.Value.Date > TODateTimePicker.Value.Date)
            {
                MessageBox.Show("From Date greater than To Date!!", "Error", MessageBoxButtons.OK);
            }
        }

        private void TODateTimePicker_ValueChanged(object sender, EventArgs e)
        {
            if (FromDateTimePicker.Value.Date > TODateTimePicker.Value.Date)
            {
                MessageBox.Show("From Date greater than To Date!!", "Error", MessageBoxButtons.OK);
            }
        }

        private void onLoad()
        {
            data = new DataTable();
            data.Columns.Add("Date", typeof(string));
            data.Columns.Add("Opening Boxes", typeof(int));
            data.Columns.Add("Opening Packets", typeof(int));
            data.Columns.Add("Recieved Boxes", typeof(int));
            data.Columns.Add("Recieved Packets", typeof(int));
            data.Columns.Add("Dispatched Boxes", typeof(int));
            data.Columns.Add("Dispatched Packets", typeof(int));
            data.Columns.Add("Closing BOXES", typeof(int));
            data.Columns.Add("Closing Packets", typeof(int));
            FillDataTable();
            dataGridView1.DataSource = data;

        }
        private void FillDataTable()
        {
            OleDbConnection connection = new OleDbConnection(Properties.Settings.Default.Database1ConnectionString1);
            connection.Open();
            DataRow row;
            DateTime date = fromDate;
            int[] stock = GetOpeningStock();
            int[] recived;
            int[] dispatched;
            int[] closing = new int[2];
            stock = ResolveBoxes(product.BoxCapacity, stock);
            while (date <= toDate)
            {
                row = data.NewRow();
                recived = GetRecievedStock(date, connection);
                recived = ResolveBoxes(product.BoxCapacity, recived);
                dispatched = GetDispatchedStock(date, connection);
                dispatched = ResolveBoxes(product.BoxCapacity, dispatched);
                closing[0] = stock[0] + recived[0] - dispatched[0];
                closing[1] = stock[1] + recived[1] - dispatched[1];
                closing = ResolveBoxes(product.BoxCapacity, closing);
                if (date == fromDate || date == toDate || (recived[0] != 0 || recived[1] != 0) || (dispatched[0] != 0 || dispatched[1] != 0))
                {
                    row[0] = date.ToString("dd/MM/yyyy");
                    row[1] = stock[0];
                    row[2] = stock[1];
                    row[3] = recived[0];
                    row[4] = recived[1];
                    row[5] = dispatched[0];
                    row[6] = dispatched[1];
                    row[7] = closing[0];
                    row[8] = closing[1];
                    data.Rows.Add(row);
                }
                stock[1] = closing[1];
                stock[0] = closing[0];
                date = date.AddDays(1);
            }
            connection.Close();
            connection.Dispose();
        }
        private int[] GetOpeningStock()
        {
            string query;
            int boxes = product.StockedBoxes1, packets = product.StockedPackets;
            using (OleDbConnection connection = new OleDbConnection(Properties.Settings.Default.Database1ConnectionString1))
            {
                connection.Open();
                DateTime date = product.DateCreated;
                while (date < FromDateTimePicker.Value.Date)
                {
                    query = "SELECT * FROM[" + product.Id + "STOCK] WHERE DATE1='" + date.ToString("dd/MM/yyyy") + "';";
                    using (OleDbCommand command = new OleDbCommand(query, connection))
                    {
                        using (OleDbDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                boxes += reader.GetInt32(1);
                                packets += reader.GetInt32(2);
                                boxes -= reader.GetInt32(3);
                                packets -= reader.GetInt32(4);

                            }
                        }

                    }

                    date = date.AddDays(1);
                }
            }

            return new int[] { boxes, packets };
        }

        public int[] GetRecievedStock(DateTime date, OleDbConnection connection)
        {
            string query;
            int boxes = 0, packets = 0;

            query = "SELECT * FROM[" + product.Id + "STOCK] WHERE DATE1='" + date.ToString("dd/MM/yyyy") + "';";
            using (OleDbCommand command = new OleDbCommand(query, connection))
            {
                using (OleDbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        boxes += reader.GetInt32(1);
                        packets += reader.GetInt32(2);
                    }
                }

            }


            return new int[] { boxes, packets };
        }
        private int[] GetDispatchedStock(DateTime date, OleDbConnection connection)
        {
            string query;
            int boxes = 0, packets = 0;
            query = "SELECT * FROM[" + product.Id + "STOCK] WHERE DATE1='" + date.ToString("dd/MM/yyyy") + "';";
            using (OleDbCommand command = new OleDbCommand(query, connection))
            {
                using (OleDbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        boxes += reader.GetInt32(3);
                        packets += reader.GetInt32(4);

                    }
                }



            }
            return new int[] { boxes, packets };
        }
        public int[] ResolveBoxes(int BoxCapacity, int[] a)
        {
            int boxes = a[0], packets = a[1];
            boxes += packets / BoxCapacity;
            packets = packets % BoxCapacity;
            if (packets > 0 && boxes < 0)
            {
                boxes++;
                packets -= BoxCapacity;
            }
            if (packets < 0 && boxes > 0)
            {
                boxes--;
                packets += BoxCapacity;
            }
            return new int[] { boxes, packets };
        }

        public interface cellClickListener
        {
            void OnCellClick(DateTime date,string name);
            void OnDispatchClick(DateTime date);
            
        }
        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            string dateString = (string)dataGridView1.Rows[e.RowIndex].Cells[0].Value;
            string[] dateSplit = dateString.Split(new char[] { '/' });
            DateTime date = new DateTime(Convert.ToInt32(dateSplit[2]), Convert.ToInt32(dateSplit[1]),
                Convert.ToInt32(dateSplit[0]));
            if (e.ColumnIndex == 3 || e.ColumnIndex == 4)
            {
                clickListener.OnCellClick(date, this.Text);
            }
            if (e.ColumnIndex == 5 || e.ColumnIndex == 6)
            {
                clickListener.OnDispatchClick(date);
            }
        }
    }
}
