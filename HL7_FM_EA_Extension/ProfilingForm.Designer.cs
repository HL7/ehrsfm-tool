namespace HL7_FM_EA_Extension
{
    partial class ProfilingForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProfilingForm));
            this.mainListView = new System.Windows.Forms.ListView();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.optionalityComboBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.criteriaListView = new System.Windows.Forms.ListView();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.radioButton4 = new System.Windows.Forms.RadioButton();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.radioButton3 = new System.Windows.Forms.RadioButton();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainListView
            // 
            this.mainListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainListView.Location = new System.Drawing.Point(0, 0);
            this.mainListView.MultiSelect = false;
            this.mainListView.Name = "mainListView";
            this.mainListView.Size = new System.Drawing.Size(375, 483);
            this.mainListView.TabIndex = 0;
            this.mainListView.TileSize = new System.Drawing.Size(168, 45);
            this.mainListView.UseCompatibleStateImageBehavior = false;
            this.mainListView.View = System.Windows.Forms.View.Tile;
            this.mainListView.SelectedIndexChanged += new System.EventHandler(this.mainListView_SelectedIndexChanged);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.panel1);
            this.splitContainer1.Panel1.Controls.Add(this.mainListView);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.groupBox3);
            this.splitContainer1.Panel2.Controls.Add(this.groupBox2);
            this.splitContainer1.Panel2.Controls.Add(this.groupBox1);
            this.splitContainer1.Size = new System.Drawing.Size(852, 483);
            this.splitContainer1.SplitterDistance = 375;
            this.splitContainer1.TabIndex = 1;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.textBox2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(375, 30);
            this.panel1.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 7);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Select by ID";
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(74, 4);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(100, 20);
            this.textBox2.TabIndex = 1;
            this.textBox2.TextChanged += new System.EventHandler(this.textBox2_TextChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.textBox1);
            this.groupBox3.Controls.Add(this.optionalityComboBox);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.groupBox3.Location = new System.Drawing.Point(0, 357);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(473, 126);
            this.groupBox3.TabIndex = 11;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Options for selected Criterion";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "Change note";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(80, 40);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(376, 74);
            this.textBox1.TabIndex = 11;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // optionalityComboBox
            // 
            this.optionalityComboBox.FormattingEnabled = true;
            this.optionalityComboBox.Items.AddRange(new object[] {
            "",
            "SHALL",
            "SHOULD",
            "MAY"});
            this.optionalityComboBox.Location = new System.Drawing.Point(80, 19);
            this.optionalityComboBox.Name = "optionalityComboBox";
            this.optionalityComboBox.Size = new System.Drawing.Size(89, 21);
            this.optionalityComboBox.TabIndex = 10;
            this.optionalityComboBox.SelectedIndexChanged += new System.EventHandler(this.optionalityComboBox_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "Optionality";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.criteriaListView);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox2.Location = new System.Drawing.Point(0, 68);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(473, 286);
            this.groupBox2.TabIndex = 10;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Criteria for selected item";
            // 
            // criteriaListView
            // 
            this.criteriaListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.criteriaListView.Location = new System.Drawing.Point(3, 16);
            this.criteriaListView.MultiSelect = false;
            this.criteriaListView.Name = "criteriaListView";
            this.criteriaListView.Size = new System.Drawing.Size(467, 267);
            this.criteriaListView.TabIndex = 0;
            this.criteriaListView.TileSize = new System.Drawing.Size(168, 45);
            this.criteriaListView.UseCompatibleStateImageBehavior = false;
            this.criteriaListView.View = System.Windows.Forms.View.List;
            this.criteriaListView.SelectedIndexChanged += new System.EventHandler(this.criteriaListBox_SelectedIndexChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.flowLayoutPanel1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(473, 68);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Actions for selected item";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.radioButton4);
            this.flowLayoutPanel1.Controls.Add(this.radioButton1);
            this.flowLayoutPanel1.Controls.Add(this.radioButton2);
            this.flowLayoutPanel1.Controls.Add(this.radioButton3);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 16);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(467, 49);
            this.flowLayoutPanel1.TabIndex = 6;
            // 
            // radioButton4
            // 
            this.radioButton4.Appearance = System.Windows.Forms.Appearance.Button;
            this.radioButton4.AutoSize = true;
            this.radioButton4.BackColor = System.Drawing.SystemColors.Control;
            this.radioButton4.Checked = true;
            this.radioButton4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.radioButton4.ForeColor = System.Drawing.Color.Black;
            this.radioButton4.Location = new System.Drawing.Point(3, 3);
            this.radioButton4.Name = "radioButton4";
            this.radioButton4.Size = new System.Drawing.Size(57, 25);
            this.radioButton4.TabIndex = 5;
            this.radioButton4.TabStop = true;
            this.radioButton4.Text = "Exclude";
            this.radioButton4.UseVisualStyleBackColor = false;
            this.radioButton4.CheckedChanged += new System.EventHandler(this.radioButton4_CheckedChanged);
            // 
            // radioButton1
            // 
            this.radioButton1.Appearance = System.Windows.Forms.Appearance.Button;
            this.radioButton1.AutoSize = true;
            this.radioButton1.BackColor = System.Drawing.Color.DarkGreen;
            this.radioButton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.radioButton1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioButton1.ForeColor = System.Drawing.Color.White;
            this.radioButton1.Location = new System.Drawing.Point(66, 3);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(54, 25);
            this.radioButton1.TabIndex = 5;
            this.radioButton1.Text = "Include";
            this.radioButton1.UseVisualStyleBackColor = false;
            this.radioButton1.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            // 
            // radioButton2
            // 
            this.radioButton2.Appearance = System.Windows.Forms.Appearance.Button;
            this.radioButton2.AutoSize = true;
            this.radioButton2.BackColor = System.Drawing.Color.Orange;
            this.radioButton2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.radioButton2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioButton2.ForeColor = System.Drawing.Color.White;
            this.radioButton2.Location = new System.Drawing.Point(126, 3);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(69, 25);
            this.radioButton2.TabIndex = 5;
            this.radioButton2.Text = "Deprecate";
            this.radioButton2.UseVisualStyleBackColor = false;
            this.radioButton2.CheckedChanged += new System.EventHandler(this.radioButton2_CheckedChanged);
            // 
            // radioButton3
            // 
            this.radioButton3.Appearance = System.Windows.Forms.Appearance.Button;
            this.radioButton3.AutoSize = true;
            this.radioButton3.BackColor = System.Drawing.Color.Red;
            this.radioButton3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.radioButton3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioButton3.ForeColor = System.Drawing.Color.White;
            this.radioButton3.Location = new System.Drawing.Point(201, 3);
            this.radioButton3.Name = "radioButton3";
            this.radioButton3.Size = new System.Drawing.Size(50, 25);
            this.radioButton3.TabIndex = 5;
            this.radioButton3.Text = "Delete";
            this.radioButton3.UseVisualStyleBackColor = false;
            this.radioButton3.CheckedChanged += new System.EventHandler(this.radioButton3_CheckedChanged);
            // 
            // ProfilingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(852, 483);
            this.Controls.Add(this.splitContainer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ProfilingForm";
            this.Text = "ProfilingForm";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView mainListView;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioButton3;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.RadioButton radioButton4;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox optionalityComboBox;
        private System.Windows.Forms.ListView criteriaListView;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox2;

    }
}