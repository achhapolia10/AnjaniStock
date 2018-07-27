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
    public partial class ViewLabourPayment : Form,Form1.PrintListener
    {
        List<String> labourNames;
        DataTable data;
        List<Products> products;
        private double TotalAmmount=0;
        private class LabourPaymentEntry
        {
            
            public string name="";
        }
        DateTime fromDate;
        DateTime toDate;
        public ViewLabourPayment()
        {
            InitializeComponent();
            fromDate = FromdateTimePicker1.Value.Date;
            toDate = TodateTimePicker2.Value.Date;
        }

        private void FromdateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            fromDate = FromdateTimePicker1.Value.Date;
            TodateTimePicker2.Value = FromdateTimePicker1.Value.AddDays(6);
        }

        private void TodateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            toDate = TodateTimePicker2.Value.Date;
            FromdateTimePicker1.Value = TodateTimePicker2.Value.AddDays(-6);
        }

        private void ViewLabourPayment_Load(object sender, EventArgs e)
        {
            this.FromdateTimePicker1.Value = DateTime.Today.AddDays(-7);
            this.TodateTimePicker2.Value = DateTime.Today.AddDays(-1);
            this.getProducts();
        }

        private void loadButton_Click(object sender, EventArgs e)
        {
            getLabourNames();
            TotalAmmount = 0;
            data = new DataTable();
            addColumnsToTable();
            getAllEntries();
            dataGridView1.DataSource = data;
            bindTotal();
        }
        void bindTotal()
        {
            totalLabel.Text = TotalAmmount+"";
        }

        public void addColumnsToTable()
        {
            data.Columns.Add("Name");
            DateTime date = fromDate;
            while(date<=toDate)
            {
                DataColumn column = new DataColumn(date.ToString("dd/MM/yy") + " , " + date.DayOfWeek.ToString(), typeof(int));
                
                data.Columns.Add(column);
                date=date.AddDays(1);

            }
            data.Columns.Add("Total",typeof(int));
            data.Columns.Add("Ammount",typeof(double));
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }
        public void getLabourNames()
        {
            string query;
            labourNames = new List<string>();
            DateTime date = fromDate;
            using (OleDbConnection connection = new OleDbConnection(Properties.Settings.Default.Database1ConnectionString1))
            {
                connection.Open();
                foreach (Products product in products)
                {
                    while (date <= toDate)
                    {
                        query = "SELECT NAME1 FROM ["+product.Id+"JOURNAL] WHERE DATE1='" + date.ToString("dd/MM/yyyy") +
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
                        date = date.AddDays(1);
                    }
                    date = fromDate;
                }
                connection.Close();
                labourNames.Sort();
            }
        }


        public void getProducts()
        {
            products = new List<Products>();
            string query = "SELECT NAME1,WEIGHT,PACKET,BOX,PRICE,ID FROM Product";
            using (OleDbConnection connection = new OleDbConnection(Properties.Settings.Default.Database1ConnectionString1))
            {
                connection.Open();
                using (OleDbCommand command = new OleDbCommand(query, connection))
                {
                    command.CommandType = CommandType.Text;
                    using (OleDbDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            products.Add(new Products(reader.GetString(0), reader.GetDouble(1), reader.GetInt32(3), reader.GetInt32(2), 0, 0,reader.GetDouble(4),reader.GetInt32(5)));

                        }
                    }
                }
                connection.Close();

            }
        }

        public int getProductForEachLabour(string name, DateTime date,OleDbConnection connection)
        {
            int glasses = 0;
            
            
                foreach (Products product in products)
                {
                    string query = "SELECT [BOXES PACKED],PACKETS FROM ["+product.Id+"JOURNAL] WHERE NAME1='" + name + "' AND DATE1='" +
                date.ToString("dd/MM/yyyy") + "'";
                    using (OleDbCommand command = new OleDbCommand(query, connection))
                    {
                        command.CommandType = CommandType.Text;
                        using (OleDbDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                glasses += ((reader.GetInt32(0) * product.BoxCapacity) + reader.GetInt32(1)) * product.PacketCapacity;
                            }
                        }
                    }
                
            }

            return glasses;
        }

        public double getAmmountForEachLabour(string name, DateTime date,OleDbConnection connection)
        {
            double ammount = 0;
            
                foreach (Products product in products)
                {
                    
                    string query = "SELECT [BOXES PACKED],PACKETS FROM ["+product.Id+"JOURNAL] WHERE NAME1='" + name + "' AND DATE1='" +
                    date.ToString("dd/MM/yyyy") + "'";
                    using (OleDbCommand command = new OleDbCommand(query, connection))
                    {
                        command.CommandType = CommandType.Text;
                        using (OleDbDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ammount += (((reader.GetInt32(0) * product.BoxCapacity) + reader.GetInt32(1)) * product.PacketCapacity) * product.Price / 1000.0;
                            }
                        }
                    }
                    
                }
            
            return ammount;
        }


        public void getAllEntries()
        {
            OleDbConnection connection = new OleDbConnection(Properties.Settings.Default.Database1ConnectionString1);
            connection.Open();
            DateTime date;
            foreach (string name in labourNames)
            {
                int noOfDays = 0;
                date = fromDate;
                DataRow row = data.NewRow();
                int index = 0;
                int total = 0;
                double ammount = 0;
                row[index++] = name;
                while (date <= toDate)
                {
                    
                    int glasses = getProductForEachLabour(name, date,connection);
                    ammount+= getAmmountForEachLabour(name, date,connection);
                    total += glasses;
                    row[index++] = glasses;
                    date = date.AddDays(1);
                    if(glasses!=0)
                    {
                        noOfDays++;
                    }
                    
                }
                row[0] = (string)row[0] + " - " + noOfDays;
                row[index++] = total;
                row[index++] =  ammount;
                TotalAmmount += ammount;
                data.Rows.Add(row);
            }
            connection.Close();
            connection.Dispose();
            
        }

        private void printButton_Click(object sender, EventArgs e)
        {
            if (DialogResult.OK == printDialog1.ShowDialog())
            {
                LabourPaymentDocument.Print();
            }

        }

        private bool titlePrinted = false;
        private bool headersPrinted = false;
        private bool HasMorePages = false;
        private Font font;
        private Font TitleFont= new Font("Tahoma", 24, FontStyle.Bold, GraphicsUnit.Point);
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
        private void LabourPaymentDocument_BeginPrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            
            titlePrinted = false;
            headersPrinted = false;
            HasMorePages = false;
            PaperSize paper = new PaperSize("A4",827,1169);
            LabourPaymentDocument.DefaultPageSettings.PaperSize = paper;
            LabourPaymentDocument.DefaultPageSettings.Margins = new Margins(0, 0, 0, 0);
            LabourPaymentDocument.DefaultPageSettings.Landscape = true;
            font = new Font("Ariel",11, FontStyle.Regular);
            contentFont = new Font("Ariel", 11, FontStyle.Regular);
            cursorX = 0;
            cursorY = 0;
            currentRow = 0;
     
            pen = new Pen(brush,0.8F);
            
        }
        private void LabourPaymentDocument_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            
            if(!titlePrinted)
            {
                e.Graphics.PageUnit = GraphicsUnit.Display;
                TitleSize =e.Graphics.MeasureString("Labour Payment", TitleFont);
                PointF points = new PointF(
                    (e.PageSettings.PaperSize.Height - TitleSize.Width)/2,10);
                e.Graphics.DrawString("Labour Payment", TitleFont, new SolidBrush(color), points);
                cursorY = 10 + TitleSize.Height + 5;
                TitleSize = e.Graphics.MeasureString("Date:" + fromDate.ToString("dd/MM/yyyy") + "-" + toDate.ToString("dd/MM/yyyy"), SubtitleFont);
                points = new PointF(
                    (e.PageSettings.PaperSize.Height- TitleSize.Width) / 2, cursorY);
                e.Graphics.DrawString("Date:" + fromDate.ToString("dd/MM/yyyy") + "-" + toDate.ToString("dd/MM/yyyy"), SubtitleFont, new SolidBrush(color), points);
                cursorY += TitleSize.Height;
                PaperHeight = e.MarginBounds.Height;
                titlePrinted = true;
            }
            if(!headersPrinted)
            {
                headersPrinted=drawColumnHeaders(e.Graphics);
            }

            e.HasMorePages = DrawDataRows(e.Graphics);
            return;
        }
        private void LabourPaymentDocument_EndPrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            
        }


        /** Used to Draw Column Header
         ** Passed graphic
         * DrawHeader*/
         public bool drawColumnHeaders(Graphics g)
        {
            g.PageUnit=GraphicsUnit.Display;
            cursorY += 4;
            cursorX = 10;
            SizeF textSize=g.MeasureString("Name",font);
            wordHeight = textSize.Height;
            g.DrawRectangle(pen,cursorX,cursorY,170,textSize.Height+ 1F);
            g.DrawString("Name", font, brush, new PointF(cursorX + 0.1F, cursorY + 0.25F));
            cursorX += 170;

            DateTime date = fromDate;
            while(date<=toDate)
            {
                g.DrawRectangle(pen, cursorX, cursorY,90, textSize.Height + 0.5F);
                g.DrawString(date.DayOfWeek.ToString(), font, brush, new PointF(cursorX + 0.1F, cursorY + 0.25F));
                cursorX += 90;
                date = date.AddDays(1);
            }
            g.DrawRectangle(pen, cursorX, cursorY, 90, textSize.Height + 0.5F);
            g.DrawString("Total", font, brush, new PointF(cursorX + 0.1F, cursorY + 0.25F));
            cursorX += 90;
            g.DrawRectangle(pen, cursorX, cursorY, 90, textSize.Height + 0.5F);
            g.DrawString("Ammount", font, brush, new PointF(cursorX + 0.1F, cursorY + 0.25F));
            cursorX += 90;
            g.DrawRectangle(pen, cursorX, cursorY,150, textSize.Height + 0.5F);
            g.DrawString("Signature", font, brush, new PointF(cursorX + 0.1F, cursorY + 0.25F));
            cursorX = 10;
            cursorY += textSize.Height + 0.5F;
            return true;
        }



        public bool DrawDataRows(Graphics g)
        {
            g.PageUnit = GraphicsUnit.Display;
            DataRow row;
            int i;
            while(currentRow<data.Rows.Count)
            {
                if (cursorY + wordHeight + 24 > LabourPaymentDocument.DefaultPageSettings.PaperSize.Width)
                {
                    cursorY = 10;
                    return true;
                }
                row = data.Rows[currentRow];
                
                g.DrawRectangle(pen, cursorX, cursorY, 170, wordHeight + 12F);
                g.DrawString(((string)row.ItemArray[0]).ToUpper(), contentFont, brush, new PointF(cursorX + 0.5F, cursorY + 5F));
                cursorX += 170;
                for(i=1;i<=7;i++)
                {
                    g.DrawRectangle(pen, cursorX, cursorY, 90, wordHeight + 12F);
                    if((int)row.ItemArray[i]!=0)
                    g.DrawString(((int)row.ItemArray[i])+"", font, brush, new PointF(cursorX + 0.3F, cursorY + 5F));
                    cursorX +=90;
                }
                g.DrawRectangle(pen, cursorX, cursorY, 90, wordHeight + 12F);
                g.DrawString(((int)row.ItemArray[8]) + "", font, brush, new PointF(cursorX + 0.3F, cursorY + 8F));
                cursorX += 90;
                g.DrawRectangle(pen, cursorX, cursorY, 90, wordHeight + 12);
                g.DrawString(((Double)row.ItemArray[9]) + "", font, brush, new PointF(cursorX + 0.3F, cursorY + 8F));
                cursorX += 90;
                g.DrawRectangle(pen, cursorX, cursorY, 150, wordHeight + 12F);
                cursorX = 10;
                cursorY += wordHeight+ 12F;
                currentRow++;
            }

            if(currentRow==data.Rows.Count)
            {
                g.DrawRectangle(pen, cursorX, cursorY, 980, wordHeight + 10F);
                g.DrawString("Total", contentFont, brush, new PointF(cursorX + 0.5F, cursorY + 4F));
                cursorX += 980;
                g.DrawRectangle(pen, cursorX, cursorY, 150, wordHeight + 10F);
                g.DrawString(TotalAmmount+"", contentFont, brush, new PointF(cursorX + 0.5F, cursorY + 4F));
                cursorX = 10;
            }
            return false;
        }
        public void onPrintClick()
        {
            if (printDialog1.ShowDialog() == DialogResult.OK)
            {

                printDialog1.Document.Print();
            }
        }
        public bool getStatus()
        {
            return IsDisposed;
        }
    }
}
