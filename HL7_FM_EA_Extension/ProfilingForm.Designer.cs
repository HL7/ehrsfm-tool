﻿namespace HL7_FM_EA_Extension
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProfilingForm));
            this.mainListView = new System.Windows.Forms.ListView();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.button2 = new System.Windows.Forms.Button();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.excludeRadioButton = new System.Windows.Forms.RadioButton();
            this.includeRadioButton = new System.Windows.Forms.RadioButton();
            this.deprecateRadioButton = new System.Windows.Forms.RadioButton();
            this.deleteRadioButton = new System.Windows.Forms.RadioButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.findTextBox = new System.Windows.Forms.TextBox();
            this.findButton = new System.Windows.Forms.Button();
            this.groupCheckBox = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.criteriaListView = new System.Windows.Forms.ListView();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainListView
            // 
            this.mainListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainListView.HideSelection = false;
            this.mainListView.Location = new System.Drawing.Point(0, 0);
            this.mainListView.MultiSelect = false;
            this.mainListView.Name = "mainListView";
            this.mainListView.Size = new System.Drawing.Size(335, 394);
            this.mainListView.TabIndex = 0;
            this.mainListView.TileSize = new System.Drawing.Size(168, 45);
            this.mainListView.UseCompatibleStateImageBehavior = false;
            this.mainListView.View = System.Windows.Forms.View.Tile;
            this.mainListView.SelectedIndexChanged += new System.EventHandler(this.mainListView_SelectedIndexChanged);
            this.mainListView.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.mainListView_MouseDoubleClick);
            // 
            // splitContainer1
            // 
            this.splitContainer1.BackColor = System.Drawing.Color.Black;
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(8);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.BackColor = System.Drawing.SystemColors.Control;
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
            this.splitContainer1.Panel1.Padding = new System.Windows.Forms.Padding(8);
            this.splitContainer1.Panel1MinSize = 300;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.BackColor = System.Drawing.SystemColors.Control;
            this.splitContainer1.Panel2.Controls.Add(this.groupBox3);
            this.splitContainer1.Panel2.Controls.Add(this.groupBox2);
            this.splitContainer1.Panel2.Padding = new System.Windows.Forms.Padding(8);
            this.splitContainer1.Size = new System.Drawing.Size(852, 483);
            this.splitContainer1.SplitterDistance = 355;
            this.splitContainer1.SplitterWidth = 8;
            this.splitContainer1.TabIndex = 1;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer2.IsSplitterFixed = true;
            this.splitContainer2.Location = new System.Drawing.Point(8, 8);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.mainListView);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.flowLayoutPanel1);
            this.splitContainer2.Size = new System.Drawing.Size(335, 463);
            this.splitContainer2.SplitterDistance = 394;
            this.splitContainer2.TabIndex = 1;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(257, 3);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(33, 23);
            this.button2.TabIndex = 14;
            this.button2.Text = "<->";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.excludeRadioButton);
            this.flowLayoutPanel1.Controls.Add(this.includeRadioButton);
            this.flowLayoutPanel1.Controls.Add(this.deprecateRadioButton);
            this.flowLayoutPanel1.Controls.Add(this.deleteRadioButton);
            this.flowLayoutPanel1.Controls.Add(this.button2);
            this.flowLayoutPanel1.Controls.Add(this.panel1);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(335, 65);
            this.flowLayoutPanel1.TabIndex = 6;
            // 
            // excludeRadioButton
            // 
            this.excludeRadioButton.Appearance = System.Windows.Forms.Appearance.Button;
            this.excludeRadioButton.AutoSize = true;
            this.excludeRadioButton.BackColor = System.Drawing.SystemColors.Control;
            this.excludeRadioButton.Checked = true;
            this.excludeRadioButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.excludeRadioButton.ForeColor = System.Drawing.Color.Black;
            this.excludeRadioButton.Location = new System.Drawing.Point(3, 3);
            this.excludeRadioButton.Name = "excludeRadioButton";
            this.excludeRadioButton.Size = new System.Drawing.Size(57, 25);
            this.excludeRadioButton.TabIndex = 5;
            this.excludeRadioButton.TabStop = true;
            this.excludeRadioButton.Text = "Exclude";
            this.excludeRadioButton.UseVisualStyleBackColor = false;
            this.excludeRadioButton.CheckedChanged += new System.EventHandler(this.excludeRadioButton_CheckedChanged);
            // 
            // includeRadioButton
            // 
            this.includeRadioButton.Appearance = System.Windows.Forms.Appearance.Button;
            this.includeRadioButton.AutoSize = true;
            this.includeRadioButton.BackColor = System.Drawing.Color.DarkGreen;
            this.includeRadioButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.includeRadioButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.includeRadioButton.ForeColor = System.Drawing.Color.White;
            this.includeRadioButton.Location = new System.Drawing.Point(66, 3);
            this.includeRadioButton.Name = "includeRadioButton";
            this.includeRadioButton.Size = new System.Drawing.Size(54, 25);
            this.includeRadioButton.TabIndex = 5;
            this.includeRadioButton.Text = "Include";
            this.includeRadioButton.UseVisualStyleBackColor = false;
            this.includeRadioButton.CheckedChanged += new System.EventHandler(this.includeRadioButton_CheckedChanged);
            // 
            // deprecateRadioButton
            // 
            this.deprecateRadioButton.Appearance = System.Windows.Forms.Appearance.Button;
            this.deprecateRadioButton.AutoSize = true;
            this.deprecateRadioButton.BackColor = System.Drawing.Color.Orange;
            this.deprecateRadioButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.deprecateRadioButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.deprecateRadioButton.ForeColor = System.Drawing.Color.White;
            this.deprecateRadioButton.Location = new System.Drawing.Point(126, 3);
            this.deprecateRadioButton.Name = "deprecateRadioButton";
            this.deprecateRadioButton.Size = new System.Drawing.Size(69, 25);
            this.deprecateRadioButton.TabIndex = 5;
            this.deprecateRadioButton.Text = "Deprecate";
            this.deprecateRadioButton.UseVisualStyleBackColor = false;
            this.deprecateRadioButton.CheckedChanged += new System.EventHandler(this.deprecateRadioButton_CheckedChanged);
            // 
            // deleteRadioButton
            // 
            this.deleteRadioButton.Appearance = System.Windows.Forms.Appearance.Button;
            this.deleteRadioButton.AutoSize = true;
            this.deleteRadioButton.BackColor = System.Drawing.Color.Red;
            this.deleteRadioButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.deleteRadioButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.deleteRadioButton.ForeColor = System.Drawing.Color.White;
            this.deleteRadioButton.Location = new System.Drawing.Point(201, 3);
            this.deleteRadioButton.Name = "deleteRadioButton";
            this.deleteRadioButton.Size = new System.Drawing.Size(50, 25);
            this.deleteRadioButton.TabIndex = 5;
            this.deleteRadioButton.Text = "Delete";
            this.deleteRadioButton.UseVisualStyleBackColor = false;
            this.deleteRadioButton.CheckedChanged += new System.EventHandler(this.deleteRadioButton_CheckedChanged);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.findTextBox);
            this.panel1.Controls.Add(this.findButton);
            this.panel1.Controls.Add(this.groupCheckBox);
            this.panel1.Location = new System.Drawing.Point(3, 34);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(296, 29);
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
            // findTextBox
            // 
            this.findTextBox.Location = new System.Drawing.Point(74, 4);
            this.findTextBox.Name = "findTextBox";
            this.findTextBox.Size = new System.Drawing.Size(100, 20);
            this.findTextBox.TabIndex = 1;
            this.findTextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.findTextBox_KeyUp);
            // 
            // findButton
            // 
            this.findButton.Location = new System.Drawing.Point(180, 2);
            this.findButton.Name = "findButton";
            this.findButton.Size = new System.Drawing.Size(49, 23);
            this.findButton.TabIndex = 17;
            this.findButton.Text = "Find";
            this.findButton.UseVisualStyleBackColor = true;
            this.findButton.Click += new System.EventHandler(this.findButton_Click);
            // 
            // groupCheckBox
            // 
            this.groupCheckBox.AutoSize = true;
            this.groupCheckBox.Checked = true;
            this.groupCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.groupCheckBox.Location = new System.Drawing.Point(235, 6);
            this.groupCheckBox.Name = "groupCheckBox";
            this.groupCheckBox.Size = new System.Drawing.Size(55, 17);
            this.groupCheckBox.TabIndex = 6;
            this.groupCheckBox.Text = "Group";
            this.groupCheckBox.UseVisualStyleBackColor = true;
            this.groupCheckBox.CheckedChanged += new System.EventHandler(this.groupCheckBox_CheckedChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.button1);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.textBox1);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.groupBox3.Location = new System.Drawing.Point(8, 367);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(469, 104);
            this.groupBox3.TabIndex = 11;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "For selected Criterion";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "Change note";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(80, 19);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(376, 74);
            this.textBox1.TabIndex = 11;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.criteriaListView);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.groupBox2.Location = new System.Drawing.Point(8, 8);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(469, 353);
            this.groupBox2.TabIndex = 10;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Criteria for selected Header or Function";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(9, 70);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(33, 23);
            this.button1.TabIndex = 13;
            this.button1.Text = "<->";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // criteriaListView
            // 
            this.criteriaListView.CheckBoxes = true;
            this.criteriaListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.criteriaListView.HideSelection = false;
            this.criteriaListView.Location = new System.Drawing.Point(3, 16);
            this.criteriaListView.MultiSelect = false;
            this.criteriaListView.Name = "criteriaListView";
            this.criteriaListView.ShowGroups = false;
            this.criteriaListView.Size = new System.Drawing.Size(463, 334);
            this.criteriaListView.TabIndex = 0;
            this.criteriaListView.TileSize = new System.Drawing.Size(168, 45);
            this.criteriaListView.UseCompatibleStateImageBehavior = false;
            this.criteriaListView.View = System.Windows.Forms.View.Details;
            this.criteriaListView.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.criteriaListView_ItemChecked);
            this.criteriaListView.SelectedIndexChanged += new System.EventHandler(this.criteriaListBox_SelectedIndexChanged);
            this.criteriaListView.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.criteriaListView_MouseDoubleClick);
            // 
            // ProfilingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(852, 483);
            this.Controls.Add(this.splitContainer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ProfilingForm";
            this.Text = "Profiling Form";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView mainListView;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.RadioButton deleteRadioButton;
        private System.Windows.Forms.RadioButton deprecateRadioButton;
        private System.Windows.Forms.RadioButton includeRadioButton;
        private System.Windows.Forms.RadioButton excludeRadioButton;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ListView criteriaListView;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox findTextBox;
        private System.Windows.Forms.Button findButton;
        private System.Windows.Forms.CheckBox groupCheckBox;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.ToolTip toolTip1;

    }
}