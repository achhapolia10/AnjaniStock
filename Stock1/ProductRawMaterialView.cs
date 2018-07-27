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
    public partial class ProductRawMaterialView : Form, Form1.PrintListener
    {

        private CellClickListener clickListener;
        class RawMaterialEntryReport
        {
            int boxes;
            int packets;
            int boxCapacity;
            int packetCapacity;
            int carryForward;
            double rawMaterialWeight;
            double packetWeight;
            Products product;
            private bool equatedCarryForward=false;
            public RawMaterialEntryReport(Products product, int boxes, int packets,int carryForward, int boxCapacity, int packetCapacity, double rawMaterialWeight, double packetWeight)
            {
                this.Product = product;
                this.boxes = boxes;
                this.packets = packets;
                this.boxCapacity = boxCapacity;
                this.carryForward = carryForward;
                this.packetCapacity = packetCapacity;
                this.rawMaterialWeight = rawMaterialWeight;
                this.packetWeight = packetWeight;
            }
            
            public int Boxes { get => boxes; set => boxes = value; }
            public int Packets { get => packets; set => packets = value; }
            public int BoxCapacity { get => boxCapacity; set => boxCapacity = value; }
            public int PacketCapacity { get => packetCapacity; set => packetCapacity = value; }
            public int CarryForward { get => carryForward; set => carryForward = value; }
            public double RawMaterialWeight { get => rawMaterialWeight; set => rawMaterialWeight = value; }
            public double PacketWeight { get => packetWeight; set => packetWeight = value; }
            internal Products Product { get => product; set => product = value; }

            public void equateBoxes()
            {
                if(!equatedCarryForward)
                {
                    packets += carryForward;
                    boxes += packets / boxCapacity;
                    packets %= boxCapacity;
                    equatedCarryForward = true;
                }
            }
        }
        
        private class LabourEntry
        {
            string labourName;
            Products product;
            int boxes;
            int packets;
            string date;

            public LabourEntry(string labourName,Products product, int boxes, int packets, string date)
            {
                this.labourName = labourName;
                this.product = product;
                this.boxes = boxes;
                this.packets =packets;
                this.date = date;
            }

            public string LabourName { get => labourName; set => labourName = value; }
            public int Boxes { get => boxes; set => boxes = value; }
            public Products Product { get => product; set => Product1 = product; }
            public string Date { get => date; set => date = value; }
            public int Packets { get => packets; set => packets = value; }
            internal Products Product1 { get => product; set => product = value; }
        }
        

        private List<RawMaterialEntryReport> rawMaterialEntryReports = new List<RawMaterialEntryReport>();
        private List<LabourEntry>[] labourEntryforProducts;
        private List<Products> productNames = new List<Products>();
        int totalBoxes = 0;
        private List<Products> allProducts = new List<Products>();
        double rawWeight = 0;
        double plasticWeight = 0;
        DataTable productDetails;
        public ProductRawMaterialView()
        {
            InitializeComponent();
        }

        private void ProductRawMaterialView_Load(object sender, EventArgs e)
        {
            getAllProducts();
            GetDetails();

            bindDataGridView();
            clickListener =(CellClickListener)this.MdiParent;

        }
        private void getAllProducts()
        {

            string date = dateTimePicker1.Value.ToString("dd/MM/yyyy");
            string query = "SELECT * FROM Product";
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
                                allProducts.Add(new Products(reader.GetString(1),reader.GetDouble(2),
                                    reader.GetInt32(4),reader.GetInt32(3),reader.GetInt32(0)));
                        }


                    }

                }
                connection.Close();
            }
        }
        private void printDailyCompleteReport_Click(object sender, EventArgs e)
        {
            if(printDialog1.ShowDialog()==DialogResult.OK)
            {
                ProductDetailsDocument.Print();
            }
        }


        private void GetProductNames()
        {
            using (OleDbConnection connection = new OleDbConnection(Properties.Settings.Default.Database1ConnectionString1))
            {

                connection.Open();
                foreach(Products product in allProducts)
                { 
                string date = dateTimePicker1.Value.ToString("dd/MM/yyyy");
                 string query = "Select * FROM ["+product.Id+"JOURNAL] WHERE DATE1='" +
                date + "';";

                    using (OleDbCommand cmd = new OleDbCommand(query, connection))
                    {
                        cmd.CommandType = CommandType.Text;
                        using (OleDbDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                
                                    productNames.Add(product);
                            }


                        }
                    }
               }
                connection.Close();
            }
            labourEntryforProducts = new List<LabourEntry>[productNames.Count];
        }
        private void GetDetails()
        {
            GetProductNames();
            LabourEntry entry;
            List<LabourEntry> entriesForAProduct;
            string date = dateTimePicker1.Value.ToString("dd/MM/yyyy");
            using (OleDbConnection connection = new OleDbConnection(Properties.Settings.Default.Database1ConnectionString1))
            {
                connection.Open();
                int i = 0;
                foreach (Products product in productNames)
                {
                    int packets = 0;
                    int boxes = 0;
                    int boxCapacity = 0;
                    int packetCapacity = 0;
                    double weight = 0;
                    int carryForward = 0;
                    entriesForAProduct = new List<LabourEntry>();
                    string query1 = "SELECT NAME1,[BOXES PACKED],[PACKETS] FROM ["+product.Id
                        +"JOURNAL] WHERE DATE1='" +
                        date + "' ORDER BY NAME1 ASC;";
                    using (OleDbCommand command1 = new OleDbCommand(query1, connection))
                    {
                        command1.CommandType = CommandType.Text;
                        using (OleDbDataReader reader = command1.ExecuteReader())
                        {
                            
                            while(reader.Read())
                            {
                                entry = new LabourEntry(reader.GetString(0), product, reader.GetInt32(1),
                                    reader.GetInt32(2), date);
                                entriesForAProduct.Add(entry);
                                boxes += reader.GetInt32(1);
                                packets += reader.GetInt32(2);
                            }
                            labourEntryforProducts[i] = entriesForAProduct;
                            
                        }
                    }
                

                    string query2 = "SELECT WEIGHT, PACKET,BOX FROM Product where NAME1='" + product.Name + "';";
                    using (OleDbCommand command1 = new OleDbCommand(query2, connection))
                    {
                        command1.CommandType = CommandType.Text;
                        using (OleDbDataReader reader = command1.ExecuteReader())
                        {
                            if(reader.Read())
                            {
                                boxCapacity = reader.GetInt32(2);
                                packetCapacity = reader.GetInt32(1);
                                weight = reader.GetDouble(0);

                            }
                        }
                    }

                    string query3= "SELECT [Carry Forward] FROM CFTABLE WHERE DATE1='"+date+"' AND PRODUCT='"+
                        product.Name+"';";
                    using (OleDbCommand command1 = new OleDbCommand(query3, connection))
                    {
                        command1.CommandType = CommandType.Text;
                        using (OleDbDataReader reader = command1.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                carryForward = reader.GetInt32(0);

                            }
                            else
                            {
                                carryForward = 0;
                            }
                        }
                    }
                    rawMaterialEntryReports.Add(new RawMaterialEntryReport(
                                    product, boxes, packets,carryForward,boxCapacity,packetCapacity,weight,
                                    Properties.Settings.Default.PacketWeight));
                    rawMaterialEntryReports[i].equateBoxes();
                    i++;
                }
                connection.Close();
            }
        }


        private void bindDataGridView()
        {
            productDetails = new DataTable();
            productDetails.Columns.Add("Name", typeof(string));
            productDetails.Columns.Add("Boxes", typeof(int));
            productDetails.Columns.Add("Packets", typeof(int));
            productDetails.Columns.Add("Plastic Weight(in KG)", typeof(double));
            productDetails.Columns.Add("Raw Material Weight(in KG)", typeof(double));
            DataRow row;
            foreach(RawMaterialEntryReport materialView in rawMaterialEntryReports)
            {
                row = productDetails.NewRow();
                row[0] = materialView.Product.Name;
                row[1] = materialView.Boxes;
                row[2] = materialView.Packets;
                row[3] = Math.Round((materialView.Boxes * materialView.BoxCapacity) / Properties.Settings.Default.PacketWeight,3);
                row[4] = Math.Round((materialView.Boxes * materialView.BoxCapacity * materialView.PacketCapacity * materialView.RawMaterialWeight) / 1000.0,2);

                totalBoxes += materialView.Boxes;
                plasticWeight += Math.Round((materialView.Boxes * materialView.BoxCapacity) / Properties.Settings.Default.PacketWeight, 3);
                rawWeight += (materialView.Boxes * materialView.BoxCapacity * materialView.PacketCapacity * materialView.RawMaterialWeight) / 1000.0;
                productDetails.Rows.Add(row);
            }
            dataGridView1.DataSource = productDetails;
            rawWeight = Math.Round(rawWeight, 3);
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            rawMaterialEntryReports = new List<RawMaterialEntryReport>();
            productNames = new List<Products>();
            productDetails = new DataTable();
            GetDetails();
            bindDataGridView();
        }

        
        List<String> labourNames;
        public void getLabourNames()
        {
            DateTime date = dateTimePicker1.Value;
            string query;
            labourNames = new List<string>();
            using (OleDbConnection connection = new OleDbConnection(Properties.Settings.Default.Database1ConnectionString1))
            {
                connection.Open();
                foreach (Products product in productNames)
                {
                        query = "SELECT NAME1 FROM [" + product.Id + "JOURNAL] WHERE DATE1='" + date.ToString("dd/MM/yyyy") +
                            "';";
                        using (OleDbCommand cmd = new OleDbCommand(query, connection))
                        {
                            cmd.CommandType = CommandType.Text;
                            using (OleDbDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    string name = reader.GetString(0);
                                    if (!labourNames.Contains(name))
                                        labourNames.Add(name);
                                }
                            }
                        }
                }
                connection.Close();
                labourNames.Sort();
            }
        }







        
        private Font font;
        private Font TitleFont = new Font("Tahoma", 18, FontStyle.Bold);
        private Font SubtitleFont = new Font("Ariel", 18, FontStyle.Underline);
        private Color color = Color.Black;
        private float cursorX = 0;
        private float cursorY = 0;
        private Brush brush = new SolidBrush(Color.Black);
        private Pen pen;
        private Font contentFont;
        private Font totalFont = new Font("Ariel", 11, FontStyle.Bold);



        bool headersNeed = true;
        int ColumnsPrinted = 0;
        int startingColumn;
        int currentColumn;

        int currentName = 0;
        

        private void ProductDetailsDocument_BeginPrint(object sender, PrintEventArgs e)
        {
            PaperSize paper = new PaperSize("A4", 827, 1169);
            ProductDetailsDocument.DefaultPageSettings.PaperSize = paper;
            ProductDetailsDocument.DefaultPageSettings.Margins = new Margins(0, 0, 0, 0);
            font = new Font("Ariel", 12, FontStyle.Regular);
            contentFont = new Font("Ariel", 10, FontStyle.Regular);
            cursorX = 0;
            cursorY = 0;
            pen = new Pen(brush, 0.8F);
            headersNeed = true;
            currentColumn = 0;
            ColumnsPrinted = 0;
            currentName = 0;
            getLabourNames();
        }
        
        private void ProductDetailsDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            e.Graphics.PageUnit = GraphicsUnit.Display;
            cursorY = 10;
            cursorX += 30;
            e.HasMorePages=PrintProductDetails(e.Graphics);
            
        }



        private bool PrintProductDetails(Graphics g)
        {
            string date = dateTimePicker1.Value.ToString("dd/MM/yyyy");
            string measureString = "Date:" +  date;
            SizeF dateSize=g.MeasureString(measureString, SubtitleFont);
            g.DrawString(measureString,SubtitleFont, brush,
                30, cursorY);
            cursorX += dateSize.Width + 70;
            measureString = "Boxes:" + totalBoxes;
            dateSize = g.MeasureString(measureString, contentFont);
            g.DrawString(measureString, contentFont, brush, cursorX, cursorY);
            cursorY += dateSize.Height + 2;

            measureString = "Plastic:" + plasticWeight+" KG";
            dateSize = g.MeasureString(measureString, contentFont);
            g.DrawString(measureString, contentFont, brush, cursorX, cursorY);
            cursorY += dateSize.Height + 2;

            measureString = "Raw Material:" + rawWeight + " KG";
            dateSize = g.MeasureString(measureString, contentFont);
            g.DrawString(measureString, contentFont, brush, cursorX, cursorY);
            cursorY += dateSize.Height + 2;

            cursorY += 12;
            cursorX = 5;




            if (headersNeed)
            {
                g.DrawRectangle(pen, cursorX, cursorY, 150, 40);
                g.DrawString("Name", font, brush, cursorX + 30, cursorY + 10);
                cursorX += 150;
                SizeF fontSize = g.MeasureString(productNames[0].Name, new Font("Ariel", 10, FontStyle.Regular));
                startingColumn = currentColumn;
                for (; currentColumn < productNames.Count; currentColumn++)
                {
                    string productName = productNames[currentColumn].Name;
                    g.DrawRectangle(pen, cursorX, cursorY, 133.4F, 40);

                    g.DrawString(productName.Substring(0, productName.LastIndexOf(' ')), new Font("Ariel", 10, FontStyle.Regular), brush, cursorX + 3, cursorY + 4);
                    g.DrawString(productName.Substring(productName.LastIndexOf(' ') + 1), new Font("Ariel", 10, FontStyle.Regular), brush, cursorX + 3, cursorY + fontSize.Height + 2);
                    cursorX += 133.4F;
                    if (cursorX + 120 > ProductDetailsDocument.DefaultPageSettings.PaperSize.Width - 5)
                    {
                        currentColumn++;
                        break;
                    }
                }
                headersNeed = false;
            }


            cursorY += 40;
            cursorX = 5;



            SizeF wordHeight = g.MeasureString("Anshu", contentFont);
            using (OleDbConnection connection = new OleDbConnection(Properties.Settings.Default.Database1ConnectionString1))
            {
                connection.Open();
                for (;currentName < labourNames.Count;)
                {
                    string name = labourNames[currentName];
                    g.DrawRectangle(pen, cursorX, cursorY, 150, wordHeight.Height + 5);
                    g.DrawString(name, contentFont, brush, cursorX + 2, cursorY + 3.5F);
                    cursorX += 150;
                    for (int i = startingColumn; i <currentColumn; i++)
                    {
                        string query = "SELECT [BOXES PACKED], PACKETS FROM ["+ productNames[i].Id+"JOURNAL] WHERE DATE1='" + date + "' AND" +
                            " NAME1='" + name + "';";
                        using (OleDbCommand command = new OleDbCommand(query, connection))
                        {
                            command.CommandType = CommandType.Text;
                            using (OleDbDataReader reader = command.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    g.DrawRectangle(pen, cursorX, cursorY, 66.7F, wordHeight.Height + 5);
                                    g.DrawString(reader.GetInt32(0) + "", contentFont, brush, cursorX + 2, cursorY + 3.5F);
                                    g.DrawRectangle(pen, cursorX + 66.7F, cursorY, 66.7F, wordHeight.Height + 5);
                                    g.DrawString(reader.GetInt32(1) + "", contentFont, brush, cursorX + 67, cursorY + 3.5F);
                                    cursorX += 133.4F;
                                }
                                else
                                {
                                    g.DrawRectangle(pen, cursorX, cursorY, 66.7F, wordHeight.Height + 5);
                                    g.DrawRectangle(pen, cursorX + 66.7F, cursorY, 66.7F, wordHeight.Height + 5);
                                    cursorX += 133.4F;
                                }
                            }
                        }
                    }
                    currentName++;
                    cursorY += wordHeight.Height + 5;
                    cursorX = 5;
                    if(currentName==labourNames.Count)
                    {
                        if(cursorY + wordHeight.Height + 10>ProductDetailsDocument.DefaultPageSettings.PaperSize.Height-5)
                        {
                            headersNeed = true;
                            connection.Close();
                            return true;
                        }
                        else
                        {
                            g.DrawRectangle(pen, cursorX, cursorY, 150, wordHeight.Height + 5);
                            g.DrawString("Total", totalFont, brush, cursorX + 2, cursorY + 3.5F);
                            cursorX += 150;
                            for (int k = startingColumn; k < currentColumn; k++)
                            {
                                g.DrawRectangle(pen, cursorX, cursorY, 66.7F, wordHeight.Height + 5);
                                g.DrawString(rawMaterialEntryReports[k].Boxes+"",totalFont, brush, cursorX + 2, cursorY + 3.5F);
                                g.DrawRectangle(pen, cursorX + 66.7F, cursorY, 66.7F, wordHeight.Height + 5);
                                g.DrawString(rawMaterialEntryReports[k].Packets + "", totalFont, brush, cursorX + 67, cursorY + 3.5F);
                                cursorX += 133.4F;
                            }
                            cursorY += wordHeight.Height + 5;
                            cursorX = 5;
                        }

                    }
                    if (currentName == labourNames.Count && currentColumn< productNames.Count)
                    {
                        currentName = 0;
                        connection.Close();
                        headersNeed = true;
                        return true;
                    }
                    if (cursorY + wordHeight.Height + 5> ProductDetailsDocument.DefaultPageSettings.PaperSize.Height - 15)
                    {
                        headersNeed = true;
                        currentColumn = 0;
                        connection.Close();
                        return true;
                    }
            
                }
                connection.Close();
            }
            return false;
        }

        public void onPrintClick()
        {
            if (printDialog1.ShowDialog() == DialogResult.OK)
            {
                ProductDetailsDocument.Print();
            }
        }
        public bool getStatus()
        {
            return IsDisposed;
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.ColumnIndex==0)
            {
                clickListener.OnCellClick(dateTimePicker1.Value.Date,
                    (string)dataGridView1.Rows[e.RowIndex].Cells[0].Value);
            }
        }

        public interface CellClickListener
        {
            void OnCellClick(DateTime date, string name);
        }
    }
}
