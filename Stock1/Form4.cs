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
using System.Drawing.Printing;

namespace Stock1
{
    public partial class Form4 : Form,Form1.PrintListener
    {

        private DataTable data;
        public class Product
        {
            private string name;
            private string dateAdded;
            private int boxCapacity;
            private int packetCapacity;
            private int openingPackets;
            private int openingBoxes;
            private int id;
            public Product(int id,string name, int boxCapacity, int packetCapacity, int openingPackets, int openingBoxes,string dateAdded)
            {
                this.id = id;
                this.name = name;
                this.boxCapacity = boxCapacity;
                this.packetCapacity = packetCapacity;
                this.openingPackets = openingPackets;
                this.openingBoxes = openingBoxes;
                this.dateAdded = dateAdded;
            }

            public string Name { get => name; set => name = value; }
            public string DateAdded { get => dateAdded; set => dateAdded = value; }
            public int BoxCapacity { get => boxCapacity; set => boxCapacity = value; }
            public int PacketCapacity { get => packetCapacity; set => packetCapacity = value; }
            public int OpeningPackets { get => openingPackets; set => openingPackets = value; }
            public int OpeningBoxes { get => openingBoxes; set => openingBoxes = value; }
            public int Id { get => id; set => id = value; }
        }
        public Form4()
        {
            InitializeComponent();
        }
        public List<Form4.Product> products = new List<Form4.Product>();

        private void Form4_Load(object sender, EventArgs e)
        {
            if (products.Count == 0)
            {
                using (OleDbConnection connection = new OleDbConnection(Properties.Settings.Default.Database1ConnectionString1))
                {
                    string query = @"SELECT NAME1, PACKET,BOX,[OPENING BOXES],[OPENING PACKETS],[DATE ADDED],ID
                                    FROM Product;";
                    connection.Open();
                    using (OleDbCommand command = new OleDbCommand(query, connection))
                    {
                        using (OleDbDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                products.Add(new Product(reader.GetInt32(6),reader.GetString(0),
                                    reader.GetInt32(2), reader.GetInt32(1), reader.GetInt32(4), reader.GetInt32(3),reader.GetString(5)));
                                
                            }
                        }
                    }
                    

                }
            }
        }

        private void LoadButton_Click(object sender, EventArgs e)
        {
            if (FromDateTimePicker.Value.Date <= TODateTimePicker.Value.Date)
                getDetails();
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



        public void getDetails()
        {
            data = new DataTable();
            OleDbConnection connection = new OleDbConnection(Properties.Settings.Default.Database1ConnectionString1);
            connection.Open();
            DataColumn ProductNameColumn = new DataColumn("Product Name", typeof(string));
            data.Columns.Add(ProductNameColumn);
            data.Columns.Add(new DataColumn("Opening Boxes", typeof(int)));
            data.Columns.Add(new DataColumn("Opening Packets", typeof(int)));
            data.Columns.Add(new DataColumn("Boxes Recieved", typeof(int)));
            data.Columns.Add(new DataColumn("Packets Recieved", typeof(int)));
            data.Columns.Add(new DataColumn("Boxes Dispatched", typeof(int)));
            data.Columns.Add(new DataColumn("Packets Dispatched", typeof(int)));
            data.Columns.Add(new DataColumn("Closing Boxes", typeof(int)));
            data.Columns.Add(new DataColumn("Closing Packets", typeof(int)));
            foreach(Product product in products)
            {

                int[] openingStock = GetOpeningStock(product,connection);
                openingStock=ResolveBoxes(openingStock,product);
                int[] recievedStock = GetRecievedStock(product, connection);
                recievedStock = ResolveBoxes(recievedStock, product);
                int[] dispatchedStock = GetDispatchedStock(product, connection);
                dispatchedStock = ResolveBoxes(dispatchedStock, product);
                int[] closingStock = new int[2];
                closingStock[0] = openingStock[0] + recievedStock[0] - dispatchedStock[0];
                closingStock[1] = openingStock[1] + recievedStock[1] - dispatchedStock[1];
                closingStock = ResolveBoxes(closingStock, product);
                DataRow row = data.NewRow();
                row[0] = product.Name;
                row[1] = openingStock[0];
                row[2] = openingStock[1];
                row[3] = recievedStock[0];
                row[4] = recievedStock[1];
                row[5] = dispatchedStock[0];
                row[6] = dispatchedStock[1];
                row[7] = closingStock[0];
                row[8] = closingStock[1];
                if(!( openingStock[0]==0 && openingStock[1]==0 && 
                    closingStock[0]==0 && closingStock[1]==0 ))
                {
                    data.Rows.Add(row);
                }
            }
            
            dataGridView1.DataSource = data;
            connection.Close();
            connection.Dispose();

        }
        private int[] GetOpeningStock(Product product,OleDbConnection connection)
        {
            string query;
            int boxes = product.OpeningBoxes, packets = product.OpeningPackets;
            
                string[] dateSplit = product.DateAdded.Split(new char[] { '/' });
                DateTime date = new DateTime(Convert.ToInt32(dateSplit[2]), Convert.ToInt32(dateSplit[1]), Convert.ToInt32(dateSplit[0]));
                while(date<FromDateTimePicker.Value.Date)
                {
                    query="SELECT * FROM  ["+ product.Id + "STOCK] WHERE DATE1='"+date.ToString("dd/MM/yyyy") + "';";
                    using (OleDbCommand command = new OleDbCommand(query, connection))
                    {
                        using(OleDbDataReader reader=command.ExecuteReader())
                        {
                            while(reader.Read())
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
            

            return new int[] { boxes, packets };
        }
        public int[] GetRecievedStock(Product product, OleDbConnection connection)
        {
            string query;
            int boxes = 0, packets = 0;
           
                DateTime date = FromDateTimePicker.Value.Date;
                while (date <= TODateTimePicker.Value.Date)
                {
                    query = "SELECT * FROM [" + product.Id + "STOCK] WHERE DATE1='" + date.ToString("dd/MM/yyyy") + "';";
                    using (OleDbCommand command = new OleDbCommand(query, connection))
                    {
                        using (OleDbDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                boxes += reader.GetInt32(1);
                                packets += reader.GetInt32(2);
                            }
                        }

                    }

                    date = date.AddDays(1);
                }
            

            return new int[] { boxes, packets };
        }
        private int[] GetDispatchedStock(Product product,OleDbConnection connection)
        {
            string query;
            int boxes = 0, packets = 0;
                DateTime date = FromDateTimePicker.Value.Date;
                while (date <= TODateTimePicker.Value.Date)
                {
                    query = "SELECT * FROM [" + product.Id + "STOCK] WHERE DATE1='" + date.ToString("dd/MM/yyyy") + "';";
                    using (OleDbCommand command = new OleDbCommand(query, connection))
                    {
                        using (OleDbDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                boxes += reader.GetInt32(3);
                                packets += reader.GetInt32(4);
                            }
                        }

                    }

                    date = date.AddDays(1);
                }
            

            return new int[] { boxes, packets };
        }


        public int[] ResolveBoxes(int[] a,Product product)
        {
            int boxes = a[0], packets = a[1];
            boxes += packets / product.BoxCapacity;
            packets = packets % product.BoxCapacity;
            if(packets>0 && boxes<0)
            {
                boxes++;
                packets -= product.BoxCapacity;
            }
            if(packets<0 && boxes>0)
            {
                boxes--;
                packets += product.BoxCapacity;
            }
            return new int[] { boxes, packets };
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                string productName = (string)dataGridView1.Rows[e.RowIndex].Cells[0].Value;
                Form5 form5 = new Form5(productName,FromDateTimePicker.Value.Date,TODateTimePicker.Value.Date);
                form5.MdiParent = this.MdiParent;
                form5.Show();
            }
        }

        public void onPrintClick()
        {
            
            if(printDialog1.ShowDialog()==DialogResult.OK)
            {
                StockDocument.Print();
            }
            
        }

        public bool getStatus()
        {
            return IsDisposed;
        }


        private bool titlePrinted = false;
        private bool headersPrinted = false;
        private bool HasMorePages = false;
        private Font font;
        private Font TitleFont = new Font("Tahoma", 24, FontStyle.Bold, GraphicsUnit.Point);
        private Font SubtitleFont = new Font("Ariel", 18, FontStyle.Underline, GraphicsUnit.Point);
        private float PaperHeight;
        private Color color = Color.Black;
        private SizeF TitleSize;
        private float cursorX = 0;
        private float cursorY = 0;
        private Brush brush = new SolidBrush(Color.Black);
        private Pen pen;
        private Font contentFont;
        private int currentRow = 0;
        private float wordHeight;
        private void StockDocument_BeginPrint(object sender, PrintEventArgs e)
        {
            titlePrinted = false;
            headersPrinted = false;
            HasMorePages = false;
            PaperSize paper = new PaperSize("A4", 827, 1169);
            StockDocument.DefaultPageSettings.PaperSize = paper;
            StockDocument.DefaultPageSettings.Margins = new Margins(0, 0, 0, 0);
            font = new Font("Ariel", 11, FontStyle.Regular);
            contentFont = new Font("Ariel", 11, FontStyle.Regular);
            cursorX = 0;
            cursorY = 0;
            currentRow = 0;
            pen = new Pen(brush, 0.8F);
        }

        private void StockDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            e.Graphics.PageUnit = GraphicsUnit.Display;
            if(!titlePrinted)
            {
                titlePrinted = PrintTitle(e.Graphics);
            }
            e.HasMorePages = PrintDetails(e.Graphics);
            
        }
        private bool PrintTitle(Graphics g)
        {
            cursorY += 15;
            SizeF size = g.MeasureString("Stock", TitleFont);
            g.DrawString("Stock", TitleFont, brush,
                (StockDocument.DefaultPageSettings.PaperSize.Width - size.Width) / 2, cursorY);
            cursorY += size.Height + 15;
            if(FromDateTimePicker.Value.Date==TODateTimePicker.Value.Date)
            {
                string subtitle = "Date:" + TODateTimePicker.Value.ToString("dd/MM/yyyy");
                size = g.MeasureString(subtitle, SubtitleFont);
                g.DrawString(subtitle, SubtitleFont, brush,
                (StockDocument.DefaultPageSettings.PaperSize.Width - size.Width) / 2, cursorY);
                cursorY += size.Height + 15;
            }
            else
            {
                string subtitle = "Date: " +FromDateTimePicker.Value.ToString("dd/MM/yyyy")+" - " +
                    TODateTimePicker.Value.ToString("dd/MM/yyyy");
                size = g.MeasureString(subtitle, SubtitleFont);
                g.DrawString(subtitle, SubtitleFont, brush,
                (StockDocument.DefaultPageSettings.PaperSize.Width - size.Width) / 2, cursorY);
                cursorY += size.Height + 15;
            }
            return true;
        }

        private bool PrintDetails(Graphics g)
        {
            cursorX = 10;
            string content = "Anshu";
            DataRow row;
            SizeF size = g.MeasureString(content, contentFont);
            g.DrawRectangle(pen, cursorX, cursorY, 200, size.Height * 2 + 15);
            g.DrawString("Product Name", font, brush, cursorX + 30, cursorY + 10);
            cursorX += 200;
            g.DrawRectangle(pen, cursorX, cursorY, 150, size.Height * 2 + 15);
            g.DrawString("Opening", font, brush, cursorX + 30, cursorY + 5);
            g.DrawRectangle(pen, cursorX, cursorY + size.Height + 7, 75, size.Height + 8);
            g.DrawString("Box", contentFont, brush, cursorX + 4, cursorY + size.Height + 9);
            g.DrawRectangle(pen, cursorX + 75, cursorY + size.Height + 7, 75, size.Height + 8);
            g.DrawString("Packet", contentFont, brush, cursorX + 79, cursorY + size.Height + 9);
            cursorX += 150;
            g.DrawRectangle(pen, cursorX, cursorY, 150, size.Height * 2 + 15);
            g.DrawString("Recived",font, brush, cursorX + 30, cursorY + 5);
            g.DrawRectangle(pen, cursorX, cursorY + size.Height + 7, 75, size.Height + 8);
            g.DrawString("Box", contentFont, brush, cursorX + 4, cursorY + size.Height + 9);
            g.DrawRectangle(pen, cursorX + 75, cursorY + size.Height + 7, 75, size.Height + 8);
            g.DrawString("Packet", contentFont, brush, cursorX + 79, cursorY + size.Height + 9);
            cursorX += 150;
            g.DrawRectangle(pen, cursorX, cursorY, 150, size.Height * 2 + 15);
            g.DrawString("Dispatched", font, brush, cursorX + 30, cursorY + 5);
            g.DrawRectangle(pen, cursorX, cursorY + size.Height + 7, 75, size.Height + 8);
            g.DrawString("Box", contentFont, brush, cursorX + 4, cursorY + size.Height + 9);
            g.DrawRectangle(pen, cursorX + 75, cursorY + size.Height + 7, 75, size.Height + 8);
            g.DrawString("Packet", contentFont, brush, cursorX + 79, cursorY + size.Height + 9);
            g.DrawRectangle(pen, cursorX, cursorY, 150, size.Height * 2 + 15);
            cursorX += 150;
            g.DrawRectangle(pen, cursorX, cursorY, 150, size.Height * 2 + 15);
            g.DrawString("Closing", font, brush, cursorX + 30, cursorY + 5);
            g.DrawRectangle(pen, cursorX, cursorY + size.Height + 7, 75, size.Height + 8);
            g.DrawString("Box", contentFont, brush, cursorX + 4, cursorY + size.Height + 9);
            g.DrawRectangle(pen, cursorX + 75, cursorY + size.Height + 7, 75, size.Height + 8);
            g.DrawString("Packet", contentFont, brush, cursorX + 79, cursorY + size.Height + 9);
            cursorX = 10;
            cursorY += size.Height * 2 + 15;
            for (; currentRow < data.Rows.Count; currentRow++)
            {
                if(currentRow+size.Height+20>StockDocument.DefaultPageSettings.PaperSize.Height)
                {
                    cursorY = 20;
                    return true;
                }
                row = data.Rows[currentRow];
                g.DrawRectangle(pen, cursorX, cursorY, 200, size.Height +10);
                g.DrawString((string)row.ItemArray[0], contentFont, brush, cursorX + 5, cursorY + 10);
                cursorX += 200;
                g.DrawRectangle(pen, cursorX, cursorY, 75, size.Height + 10);
                g.DrawString((int)row.ItemArray[1]+"", contentFont, brush, cursorX + 4, cursorY+5);
                g.DrawRectangle(pen, cursorX + 75, cursorY, 75, size.Height + 10);
                g.DrawString((int)row.ItemArray[2]+"", contentFont, brush, cursorX + 79, cursorY +5);
                cursorX += 150;
                g.DrawRectangle(pen, cursorX, cursorY, 75, size.Height + 10);
                g.DrawString((int)row.ItemArray[3]+"", contentFont, brush, cursorX + 4, cursorY + 5);
                g.DrawRectangle(pen, cursorX + 75, cursorY, 75, size.Height + 10);
                g.DrawString((int)row.ItemArray[4]+"", contentFont, brush, cursorX + 79, cursorY + 5);
                cursorX += 150;
                g.DrawRectangle(pen, cursorX, cursorY, 75, size.Height + 10);
                g.DrawString((int)row.ItemArray[5]+"", contentFont, brush, cursorX + 4, cursorY + 5);
                g.DrawRectangle(pen, cursorX + 75, cursorY, 75, size.Height + 10);
                g.DrawString((int)row.ItemArray[6]+"", contentFont, brush, cursorX + 79, cursorY + 5);
                cursorX += 150;
                g.DrawRectangle(pen, cursorX, cursorY, 75, size.Height + 10);
                g.DrawString((int)row.ItemArray[7]+"", contentFont, brush, cursorX + 4, cursorY + 5);
                g.DrawRectangle(pen, cursorX + 75, cursorY, 75, size.Height + 10);
                g.DrawString((int)row.ItemArray[8]+"", contentFont, brush, cursorX + 79, cursorY + 5);
                cursorX = 10;
                cursorY += size.Height + 10;
            }
            return false;
        }

        private void printPreviewDialog1_Load(object sender, EventArgs e)
        {

        }
    }
}
