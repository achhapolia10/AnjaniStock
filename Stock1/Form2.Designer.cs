namespace Stock1
{
    partial class Form2
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.addProduct = new System.Windows.Forms.TextBox();
            this.addWeight = new System.Windows.Forms.TextBox();
            this.addBox = new System.Windows.Forms.TextBox();
            this.addPacket = new System.Windows.Forms.TextBox();
            this.labelProduct = new System.Windows.Forms.Label();
            this.labelWeight = new System.Windows.Forms.Label();
            this.labelPacket = new System.Windows.Forms.Label();
            this.labelBox = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.saveButton = new System.Windows.Forms.Button();
            this.priceTextBox1 = new System.Windows.Forms.TextBox();
            this.priceLabel = new System.Windows.Forms.Label();
            this.OpeningStockPackets = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.OpeningStockBoxes = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // addProduct
            // 
            this.addProduct.Location = new System.Drawing.Point(209, 17);
            this.addProduct.Name = "addProduct";
            this.addProduct.Size = new System.Drawing.Size(177, 20);
            this.addProduct.TabIndex = 1;
            this.addProduct.KeyDown += new System.Windows.Forms.KeyEventHandler(this.addProduct_KeyDown);
            // 
            // addWeight
            // 
            this.addWeight.Location = new System.Drawing.Point(270, 42);
            this.addWeight.Name = "addWeight";
            this.addWeight.Size = new System.Drawing.Size(116, 20);
            this.addWeight.TabIndex = 2;
            this.addWeight.KeyDown += new System.Windows.Forms.KeyEventHandler(this.addWeight_KeyDown);
            // 
            // addBox
            // 
            this.addBox.Location = new System.Drawing.Point(270, 98);
            this.addBox.Name = "addBox";
            this.addBox.Size = new System.Drawing.Size(116, 20);
            this.addBox.TabIndex = 4;
            this.addBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.addBox_KeyDown);
            // 
            // addPacket
            // 
            this.addPacket.Location = new System.Drawing.Point(270, 72);
            this.addPacket.Name = "addPacket";
            this.addPacket.Size = new System.Drawing.Size(116, 20);
            this.addPacket.TabIndex = 3;
            this.addPacket.KeyDown += new System.Windows.Forms.KeyEventHandler(this.addPacket_KeyDown);
            // 
            // labelProduct
            // 
            this.labelProduct.AutoSize = true;
            this.labelProduct.Location = new System.Drawing.Point(12, 20);
            this.labelProduct.Name = "labelProduct";
            this.labelProduct.Size = new System.Drawing.Size(75, 13);
            this.labelProduct.TabIndex = 0;
            this.labelProduct.Text = "Product Name";
            // 
            // labelWeight
            // 
            this.labelWeight.AutoSize = true;
            this.labelWeight.Location = new System.Drawing.Point(12, 49);
            this.labelWeight.Name = "labelWeight";
            this.labelWeight.Size = new System.Drawing.Size(41, 13);
            this.labelWeight.TabIndex = 0;
            this.labelWeight.Text = "Weight";
            // 
            // labelPacket
            // 
            this.labelPacket.AutoSize = true;
            this.labelPacket.Location = new System.Drawing.Point(12, 75);
            this.labelPacket.Name = "labelPacket";
            this.labelPacket.Size = new System.Drawing.Size(82, 13);
            this.labelPacket.TabIndex = 0;
            this.labelPacket.Text = "PacketCapacity";
            // 
            // labelBox
            // 
            this.labelBox.AutoSize = true;
            this.labelBox.Location = new System.Drawing.Point(12, 101);
            this.labelBox.Name = "labelBox";
            this.labelBox.Size = new System.Drawing.Size(69, 13);
            this.labelBox.TabIndex = 0;
            this.labelBox.Text = "Box Capacity";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(209, 217);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(61, 23);
            this.button1.TabIndex = 5;
            this.button1.Text = "Cancel";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(311, 217);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(75, 23);
            this.saveButton.TabIndex = 6;
            this.saveButton.Text = "Save";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // priceTextBox1
            // 
            this.priceTextBox1.Location = new System.Drawing.Point(270, 190);
            this.priceTextBox1.Name = "priceTextBox1";
            this.priceTextBox1.Size = new System.Drawing.Size(116, 20);
            this.priceTextBox1.TabIndex = 4;
            this.priceTextBox1.Text = "6.5";
            this.priceTextBox1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.priceTextBox1_KeyDown);
            // 
            // priceLabel
            // 
            this.priceLabel.AutoSize = true;
            this.priceLabel.Location = new System.Drawing.Point(12, 193);
            this.priceLabel.Name = "priceLabel";
            this.priceLabel.Size = new System.Drawing.Size(31, 13);
            this.priceLabel.TabIndex = 0;
            this.priceLabel.Text = "Price";
            // 
            // OpeningStockPackets
            // 
            this.OpeningStockPackets.Location = new System.Drawing.Point(270, 160);
            this.OpeningStockPackets.Name = "OpeningStockPackets";
            this.OpeningStockPackets.Size = new System.Drawing.Size(116, 20);
            this.OpeningStockPackets.TabIndex = 4;
            this.OpeningStockPackets.Text = "0";
            this.OpeningStockPackets.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            this.OpeningStockPackets.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OpeningStockPackets_KeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 163);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Opening Packets";
            // 
            // OpeningStockBoxes
            // 
            this.OpeningStockBoxes.Location = new System.Drawing.Point(270, 130);
            this.OpeningStockBoxes.Name = "OpeningStockBoxes";
            this.OpeningStockBoxes.Size = new System.Drawing.Size(116, 20);
            this.OpeningStockBoxes.TabIndex = 4;
            this.OpeningStockBoxes.Text = "0";
            this.OpeningStockBoxes.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OpeningStockBoxes_KeyDown);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 133);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(79, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Opening Boxes";
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(398, 261);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.priceLabel);
            this.Controls.Add(this.labelBox);
            this.Controls.Add(this.labelPacket);
            this.Controls.Add(this.OpeningStockBoxes);
            this.Controls.Add(this.labelWeight);
            this.Controls.Add(this.OpeningStockPackets);
            this.Controls.Add(this.labelProduct);
            this.Controls.Add(this.priceTextBox1);
            this.Controls.Add(this.addPacket);
            this.Controls.Add(this.addBox);
            this.Controls.Add(this.addWeight);
            this.Controls.Add(this.addProduct);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form2";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ADD";
            this.Load += new System.EventHandler(this.Form2_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox addProduct;
        private System.Windows.Forms.TextBox addWeight;
        private System.Windows.Forms.TextBox addBox;
        private System.Windows.Forms.TextBox addPacket;
        private System.Windows.Forms.Label labelProduct;
        private System.Windows.Forms.Label labelWeight;
        private System.Windows.Forms.Label labelPacket;
        private System.Windows.Forms.Label labelBox;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.TextBox priceTextBox1;
        private System.Windows.Forms.Label priceLabel;
        private System.Windows.Forms.TextBox OpeningStockPackets;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox OpeningStockBoxes;
        private System.Windows.Forms.Label label2;
    }
}