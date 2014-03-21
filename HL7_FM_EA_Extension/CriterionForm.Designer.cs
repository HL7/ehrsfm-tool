namespace HL7_FM_EA_Extension
{
    partial class CriterionForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CriterionForm));
            this.pathLabel = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textTextBox = new System.Windows.Forms.TextBox();
            this.conditionalCheckBox = new System.Windows.Forms.CheckBox();
            this.dependentCheckBox = new System.Windows.Forms.CheckBox();
            this.optionalityComboBox = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.idNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.rowNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.idNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rowNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // pathLabel
            // 
            this.pathLabel.AutoEllipsis = true;
            this.pathLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pathLabel.ForeColor = System.Drawing.SystemColors.Info;
            this.pathLabel.Location = new System.Drawing.Point(12, 9);
            this.pathLabel.Name = "pathLabel";
            this.pathLabel.Size = new System.Drawing.Size(652, 24);
            this.pathLabel.TabIndex = 0;
            this.pathLabel.Text = "Path";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 75);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Criteria Text";
            // 
            // textTextBox
            // 
            this.textTextBox.Location = new System.Drawing.Point(85, 72);
            this.textTextBox.Multiline = true;
            this.textTextBox.Name = "textTextBox";
            this.textTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textTextBox.Size = new System.Drawing.Size(579, 159);
            this.textTextBox.TabIndex = 2;
            // 
            // conditionalCheckBox
            // 
            this.conditionalCheckBox.AutoSize = true;
            this.conditionalCheckBox.Location = new System.Drawing.Point(84, 239);
            this.conditionalCheckBox.Name = "conditionalCheckBox";
            this.conditionalCheckBox.Size = new System.Drawing.Size(78, 17);
            this.conditionalCheckBox.TabIndex = 3;
            this.conditionalCheckBox.Text = "Conditional";
            this.toolTip1.SetToolTip(this.conditionalCheckBox, "If Conditional is checked then we expect \r\n\"IF ... THEN ...\" construct in the Cri" +
        "teria Text.");
            this.conditionalCheckBox.UseVisualStyleBackColor = true;
            // 
            // dependentCheckBox
            // 
            this.dependentCheckBox.AutoSize = true;
            this.dependentCheckBox.Location = new System.Drawing.Point(168, 239);
            this.dependentCheckBox.Name = "dependentCheckBox";
            this.dependentCheckBox.Size = new System.Drawing.Size(79, 17);
            this.dependentCheckBox.TabIndex = 4;
            this.dependentCheckBox.Text = "Dependent";
            this.dependentCheckBox.UseVisualStyleBackColor = true;
            // 
            // optionalityComboBox
            // 
            this.optionalityComboBox.FormattingEnabled = true;
            this.optionalityComboBox.Items.AddRange(new object[] {
            "",
            "SHALL",
            "SHOULD",
            "MAY"});
            this.optionalityComboBox.Location = new System.Drawing.Point(325, 237);
            this.optionalityComboBox.Name = "optionalityComboBox";
            this.optionalityComboBox.Size = new System.Drawing.Size(89, 21);
            this.optionalityComboBox.TabIndex = 5;
            this.toolTip1.SetToolTip(this.optionalityComboBox, "The selected Optionality is expected in the Text in \r\nthe format \"The system <Opt" +
        "ionality> ...\"");
            this.optionalityComboBox.SelectedIndexChanged += new System.EventHandler(this.optionalityComboBox_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(263, 241);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Optionality";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(16, 49);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Criteria ID";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(151, 49);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(29, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Row";
            // 
            // idNumericUpDown
            // 
            this.idNumericUpDown.Location = new System.Drawing.Point(85, 46);
            this.idNumericUpDown.Name = "idNumericUpDown";
            this.idNumericUpDown.Size = new System.Drawing.Size(42, 20);
            this.idNumericUpDown.TabIndex = 13;
            // 
            // rowNumericUpDown
            // 
            this.rowNumericUpDown.Location = new System.Drawing.Point(186, 46);
            this.rowNumericUpDown.Name = "rowNumericUpDown";
            this.rowNumericUpDown.Size = new System.Drawing.Size(67, 20);
            this.rowNumericUpDown.TabIndex = 14;
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(508, 263);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 15;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(589, 263);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 16;
            this.okButton.Text = "Ok";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // CriterionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(676, 295);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.rowNumericUpDown);
            this.Controls.Add(this.idNumericUpDown);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.optionalityComboBox);
            this.Controls.Add(this.dependentCheckBox);
            this.Controls.Add(this.conditionalCheckBox);
            this.Controls.Add(this.textTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.pathLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CriterionForm";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "EHR-S FM Criteria";
            ((System.ComponentModel.ISupportInitialize)(this.idNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rowNumericUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label pathLabel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textTextBox;
        private System.Windows.Forms.CheckBox conditionalCheckBox;
        private System.Windows.Forms.CheckBox dependentCheckBox;
        private System.Windows.Forms.ComboBox optionalityComboBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.NumericUpDown idNumericUpDown;
        private System.Windows.Forms.NumericUpDown rowNumericUpDown;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
    }
}