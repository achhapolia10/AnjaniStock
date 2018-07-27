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
    public partial class Form1 : Form ,Form2.IListener,ProductRawMaterialView.CellClickListener
        ,Form5.cellClickListener
    {
        Form2 fm2;
        DailyNewEntry dne;
        ViewLabourPayment viewLabourPayment;
        ProductRawMaterialView productRawMaterialView;
        DispatchForm dispatchForm;
        Form4 form4;
        Form3 f3;
        PrintListener printListener;
        public Form1()
        {
            InitializeComponent();

            dateLable.Text = DateTime.Today.ToString("dd/MM/yyyy")+" , "+DateTime.Today.DayOfWeek;
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            fm2 = new Form2();
            fm2.listener = this;
            fm2.ShowDialog();
            
        }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void editProductToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (f3 == null)
            {
                f3 = new Form3();
                f3.MdiParent = this;
                f3.Show();
            }
            if (f3.IsDisposed)
            {
                f3 = new Form3();
                f3.MdiParent = this;
                f3.Show();
            }
            else
            {
                f3.BringToFront();
            }
            
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void dayEntryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(dne==null)
            {
                dne = new DailyNewEntry();
                dne.MdiParent = this;
                dne.Show();
            }
            if (dne.IsDisposed)
            {
                dne = new DailyNewEntry();
                dne.MdiParent = this;
                dne.Show();
            }
            else
            {
                dne.BringToFront();
            }
           
        }

        private void labourPaymentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (viewLabourPayment == null)
            {
                viewLabourPayment = new ViewLabourPayment();
                viewLabourPayment.MdiParent = this;
                viewLabourPayment.Show();
            }
            if(viewLabourPayment.IsDisposed)
            {
                viewLabourPayment = new ViewLabourPayment();
                viewLabourPayment.MdiParent = this;
                viewLabourPayment.Show();
            }
            else
            {
                viewLabourPayment.BringToFront();
            }
        }

        private void dailyReportToolStripMenu_Click(object sender, EventArgs e)
        {
            if (productRawMaterialView == null)
            {
                productRawMaterialView = new ProductRawMaterialView
                {
                    MdiParent = this
                };
                productRawMaterialView.Show();
            }
            if(productRawMaterialView.IsDisposed)
            {
                productRawMaterialView = new ProductRawMaterialView
                {
                    MdiParent = this
                };
                productRawMaterialView.Show();
            }
            else
            {
                productRawMaterialView.BringToFront();
            }
        }

        

        public void onClickSave()
        {
            if (dne != null)
            {
                if (!dne.IsDisposed)
                {
                    dne.bindProductList();
                }
            }
        }

        private void dispatchEntryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dispatchForm == null)
            {
                dispatchForm = new DispatchForm
                {
                    MdiParent = this
                };
                dispatchForm.Show();
            }
            if (dispatchForm.IsDisposed)
            {
                dispatchForm = new DispatchForm
                {
                    MdiParent = this
                };
                dispatchForm.Show();
            }
            else
            {
                dispatchForm.BringToFront();
            }
        }

        private void stockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (form4== null)
            {
                form4 = new Form4
                {
                    MdiParent = this
                };
                form4.Show();
            }
            if (form4.IsDisposed)
            {
                form4 = new Form4
                {
                    MdiParent = this
                };
                form4.Show();
            }
            else
            {
                form4.BringToFront();
            }
        }

        public interface PrintListener
        {
            void onPrintClick();
            bool getStatus();

        }

        private void printToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(this.ActiveMdiChild is PrintListener)
            {
                printListener =(PrintListener)this.ActiveMdiChild;
                printListener.onPrintClick();
            }
        }

        public void OnCellClick(DateTime date, string name)
        {
            if (dne == null)
            {
                dne = new DailyNewEntry();
                dne.MdiParent = this;
                dne.Show();
            }
            if (dne.IsDisposed)
            {
                dne = new DailyNewEntry();
                dne.MdiParent = this;
                dne.Show();
                
            }
            else
            {
                dne.BringToFront();
            }
            dne.setValues(date, name);
        }

        public void OnDispatchClick(DateTime date)
        {
            if (dispatchForm == null)
            {
                dispatchForm = new DispatchForm
                {
                    MdiParent = this
                };
                dispatchForm.Show();
            }
            if (dispatchForm.IsDisposed)
            {
                dispatchForm = new DispatchForm
                {
                    MdiParent = this
                };
                dispatchForm.Show();
            }
            else
            {
                dispatchForm.BringToFront();
            }
            dispatchForm.setValues(date);
        }

        private void toolStripStatusLabel1_Click(object sender, EventArgs e)
        {

        }
    }
   
}
