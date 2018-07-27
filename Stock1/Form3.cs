using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Stock1
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }

        private void productBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.productBindingSource.EndEdit();
            this.tableAdapterManager.UpdateAll(this.database1DataSet1);

        }

        private void Form3_Load(object sender, EventArgs e)
        {
            this.productTableAdapter.Fill(this.database1DataSet1.Product);

        }

        private void productBindingNavigator_RefreshItems(object sender, EventArgs e)
        {

        }

        private void productDataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                string productName = (string)productDataGridView.Rows[e.RowIndex].Cells[0].Value;
                Form5 form5 = new Form5(productName);
                form5.MdiParent = this.MdiParent;
                form5.Show();
            }
        }

        private void productDataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
