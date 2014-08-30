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
            this.textTextBox = new System.Windows.Forms.TextBox();
            this.conditionalCheckBox = new System.Windows.Forms.CheckBox();
            this.dependentCheckBox = new System.Windows.Forms.CheckBox();
            this.optionalityComboBox = new System.Windows.Forms.ComboBox();
            this.optionalityLabel = new System.Windows.Forms.Label();
            this.idLabel = new System.Windows.Forms.Label();
            this.rowLabel = new System.Windows.Forms.Label();
            this.idNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.rowNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.switchLinkLabel = new System.Windows.Forms.LinkLabel();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.applyButton = new System.Windows.Forms.Button();
            this.helpButton = new System.Windows.Forms.Button();
            this.priorityLabel = new System.Windows.Forms.Label();
            this.priorityComboBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.changeNoteLinkLabel = new System.Windows.Forms.LinkLabel();
            this.textLinkLabel = new System.Windows.Forms.LinkLabel();
            this.changeNoteTextBox = new System.Windows.Forms.TextBox();
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
            // textTextBox
            // 
            this.textTextBox.Location = new System.Drawing.Point(85, 72);
            this.textTextBox.Multiline = true;
            this.textTextBox.Name = "textTextBox";
            this.textTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textTextBox.Size = new System.Drawing.Size(579, 159);
            this.textTextBox.TabIndex = 2;
            this.textTextBox.TextChanged += new System.EventHandler(this.textTextBox_TextChanged);
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
            this.conditionalCheckBox.CheckedChanged += new System.EventHandler(this.conditionalCheckBox_CheckedChanged);
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
            this.dependentCheckBox.CheckedChanged += new System.EventHandler(this.dependentCheckBox_CheckedChanged);
            // 
            // optionalityComboBox
            // 
            this.optionalityComboBox.FormattingEnabled = true;
            this.optionalityComboBox.Items.AddRange(new object[] {
            "",
            "SHALL",
            "SHOULD",
            "MAY"});
            this.optionalityComboBox.Location = new System.Drawing.Point(335, 237);
            this.optionalityComboBox.Name = "optionalityComboBox";
            this.optionalityComboBox.Size = new System.Drawing.Size(89, 21);
            this.optionalityComboBox.TabIndex = 5;
            this.toolTip1.SetToolTip(this.optionalityComboBox, "The selected Optionality is expected in the Text in \r\nthe format \"The system <Opt" +
        "ionality> ...\"");
            this.optionalityComboBox.SelectedIndexChanged += new System.EventHandler(this.optionalityComboBox_SelectedIndexChanged);
            // 
            // optionalityLabel
            // 
            this.optionalityLabel.AutoSize = true;
            this.optionalityLabel.Location = new System.Drawing.Point(263, 241);
            this.optionalityLabel.Name = "optionalityLabel";
            this.optionalityLabel.Size = new System.Drawing.Size(56, 13);
            this.optionalityLabel.TabIndex = 6;
            this.optionalityLabel.Text = "Optionality";
            // 
            // idLabel
            // 
            this.idLabel.AutoSize = true;
            this.idLabel.Location = new System.Drawing.Point(10, 49);
            this.idLabel.Name = "idLabel";
            this.idLabel.Size = new System.Drawing.Size(63, 13);
            this.idLabel.TabIndex = 7;
            this.idLabel.Text = "Sequence#";
            // 
            // rowLabel
            // 
            this.rowLabel.AutoSize = true;
            this.rowLabel.Location = new System.Drawing.Point(151, 49);
            this.rowLabel.Name = "rowLabel";
            this.rowLabel.Size = new System.Drawing.Size(36, 13);
            this.rowLabel.TabIndex = 8;
            this.rowLabel.Text = "Row#";
            // 
            // idNumericUpDown
            // 
            this.idNumericUpDown.Location = new System.Drawing.Point(85, 46);
            this.idNumericUpDown.Name = "idNumericUpDown";
            this.idNumericUpDown.Size = new System.Drawing.Size(42, 20);
            this.idNumericUpDown.TabIndex = 13;
            this.idNumericUpDown.ValueChanged += new System.EventHandler(this.idNumericUpDown_ValueChanged);
            // 
            // rowNumericUpDown
            // 
            this.rowNumericUpDown.Location = new System.Drawing.Point(193, 46);
            this.rowNumericUpDown.Name = "rowNumericUpDown";
            this.rowNumericUpDown.Size = new System.Drawing.Size(67, 20);
            this.rowNumericUpDown.TabIndex = 14;
            this.rowNumericUpDown.ValueChanged += new System.EventHandler(this.rowNumericUpDown_ValueChanged);
            // 
            // switchLinkLabel
            // 
            this.switchLinkLabel.AutoSize = true;
            this.switchLinkLabel.Location = new System.Drawing.Point(10, 268);
            this.switchLinkLabel.Name = "switchLinkLabel";
            this.switchLinkLabel.Size = new System.Drawing.Size(80, 13);
            this.switchLinkLabel.TabIndex = 18;
            this.switchLinkLabel.TabStop = true;
            this.switchLinkLabel.Text = "Switch Element";
            this.toolTip1.SetToolTip(this.switchLinkLabel, "If you switch edits will be lost. Press Apply to keep changes.");
            this.switchLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.switchLinkLabel_LinkClicked);
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(472, 264);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(60, 23);
            this.cancelButton.TabIndex = 15;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(406, 264);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(60, 23);
            this.okButton.TabIndex = 16;
            this.okButton.Text = "Ok";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // applyButton
            // 
            this.applyButton.Location = new System.Drawing.Point(538, 264);
            this.applyButton.Name = "applyButton";
            this.applyButton.Size = new System.Drawing.Size(60, 23);
            this.applyButton.TabIndex = 19;
            this.applyButton.Text = "Apply";
            this.applyButton.UseVisualStyleBackColor = true;
            this.applyButton.Click += new System.EventHandler(this.applyButton_Click);
            // 
            // helpButton
            // 
            this.helpButton.Enabled = false;
            this.helpButton.Location = new System.Drawing.Point(604, 264);
            this.helpButton.Name = "helpButton";
            this.helpButton.Size = new System.Drawing.Size(60, 23);
            this.helpButton.TabIndex = 20;
            this.helpButton.Text = "Help";
            this.helpButton.UseVisualStyleBackColor = true;
            // 
            // priorityLabel
            // 
            this.priorityLabel.AutoSize = true;
            this.priorityLabel.Location = new System.Drawing.Point(294, 49);
            this.priorityLabel.Name = "priorityLabel";
            this.priorityLabel.Size = new System.Drawing.Size(38, 13);
            this.priorityLabel.TabIndex = 6;
            this.priorityLabel.Text = "Priority";
            // 
            // priorityComboBox
            // 
            this.priorityComboBox.FormattingEnabled = true;
            this.priorityComboBox.Items.AddRange(new object[] {
            "",
            "EN",
            "EF",
            "OPT"});
            this.priorityComboBox.Location = new System.Drawing.Point(354, 45);
            this.priorityComboBox.Name = "priorityComboBox";
            this.priorityComboBox.Size = new System.Drawing.Size(89, 21);
            this.priorityComboBox.TabIndex = 21;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(342, 269);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 13);
            this.label1.TabIndex = 36;
            this.label1.Text = "* = Profiled";
            // 
            // changeNoteLinkLabel
            // 
            this.changeNoteLinkLabel.AutoSize = true;
            this.changeNoteLinkLabel.Location = new System.Drawing.Point(10, 97);
            this.changeNoteLinkLabel.Name = "changeNoteLinkLabel";
            this.changeNoteLinkLabel.Size = new System.Drawing.Size(70, 13);
            this.changeNoteLinkLabel.TabIndex = 18;
            this.changeNoteLinkLabel.TabStop = true;
            this.changeNoteLinkLabel.Text = "Change Note";
            this.changeNoteLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.changeNoteLinkLabel_LinkClicked);
            // 
            // textLinkLabel
            // 
            this.textLinkLabel.AutoSize = true;
            this.textLinkLabel.Location = new System.Drawing.Point(10, 75);
            this.textLinkLabel.Name = "textLinkLabel";
            this.textLinkLabel.Size = new System.Drawing.Size(28, 13);
            this.textLinkLabel.TabIndex = 18;
            this.textLinkLabel.TabStop = true;
            this.textLinkLabel.Text = "Text";
            this.textLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.textLinkLabel_LinkClicked);
            // 
            // changeNoteTextBox
            // 
            this.changeNoteTextBox.Location = new System.Drawing.Point(86, 72);
            this.changeNoteTextBox.Multiline = true;
            this.changeNoteTextBox.Name = "changeNoteTextBox";
            this.changeNoteTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.changeNoteTextBox.Size = new System.Drawing.Size(579, 159);
            this.changeNoteTextBox.TabIndex = 2;
            // 
            // CriterionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(676, 295);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.priorityComboBox);
            this.Controls.Add(this.helpButton);
            this.Controls.Add(this.applyButton);
            this.Controls.Add(this.textLinkLabel);
            this.Controls.Add(this.changeNoteLinkLabel);
            this.Controls.Add(this.switchLinkLabel);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.rowNumericUpDown);
            this.Controls.Add(this.idNumericUpDown);
            this.Controls.Add(this.rowLabel);
            this.Controls.Add(this.idLabel);
            this.Controls.Add(this.priorityLabel);
            this.Controls.Add(this.optionalityLabel);
            this.Controls.Add(this.optionalityComboBox);
            this.Controls.Add(this.dependentCheckBox);
            this.Controls.Add(this.conditionalCheckBox);
            this.Controls.Add(this.changeNoteTextBox);
            this.Controls.Add(this.textTextBox);
            this.Controls.Add(this.pathLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CriterionForm";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "EHR-S FM Criterion";
            ((System.ComponentModel.ISupportInitialize)(this.idNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rowNumericUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label pathLabel;
        private System.Windows.Forms.TextBox textTextBox;
        private System.Windows.Forms.CheckBox conditionalCheckBox;
        private System.Windows.Forms.CheckBox dependentCheckBox;
        private System.Windows.Forms.ComboBox optionalityComboBox;
        private System.Windows.Forms.Label optionalityLabel;
        private System.Windows.Forms.Label idLabel;
        private System.Windows.Forms.Label rowLabel;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.NumericUpDown idNumericUpDown;
        private System.Windows.Forms.NumericUpDown rowNumericUpDown;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.LinkLabel switchLinkLabel;
        private System.Windows.Forms.Button applyButton;
        private System.Windows.Forms.Button helpButton;
        private System.Windows.Forms.Label priorityLabel;
        private System.Windows.Forms.ComboBox priorityComboBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.LinkLabel changeNoteLinkLabel;
        private System.Windows.Forms.LinkLabel textLinkLabel;
        private System.Windows.Forms.TextBox changeNoteTextBox;
    }
}