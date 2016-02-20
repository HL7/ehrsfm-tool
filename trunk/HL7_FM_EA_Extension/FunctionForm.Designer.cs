namespace HL7_FM_EA_Extension
{
    partial class FunctionForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FunctionForm));
            this.idTextBox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.statementLabel = new System.Windows.Forms.Label();
            this.descriptionTextBox = new System.Windows.Forms.TextBox();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.pathLabel = new System.Windows.Forms.Label();
            this.nameTextBox = new System.Windows.Forms.TextBox();
            this.nameLabel = new System.Windows.Forms.Label();
            this.statementTextBox = new System.Windows.Forms.TextBox();
            this.helpButton = new System.Windows.Forms.Button();
            this.applyButton = new System.Windows.Forms.Button();
            this.priorityComboBox = new System.Windows.Forms.ComboBox();
            this.priorityLabel = new System.Windows.Forms.Label();
            this.switchLinkLabel = new System.Windows.Forms.LinkLabel();
            this.label1 = new System.Windows.Forms.Label();
            this.changeNoteLinkLabel = new System.Windows.Forms.LinkLabel();
            this.descriptionLinkLabel = new System.Windows.Forms.LinkLabel();
            this.changeNoteTextBox = new System.Windows.Forms.TextBox();
            this.lockIcon = new System.Windows.Forms.PictureBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.lockIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // idTextBox
            // 
            this.idTextBox.Location = new System.Drawing.Point(99, 43);
            this.idTextBox.Name = "idTextBox";
            this.idTextBox.ReadOnly = true;
            this.idTextBox.Size = new System.Drawing.Size(112, 20);
            this.idTextBox.TabIndex = 16;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 46);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(18, 13);
            this.label6.TabIndex = 15;
            this.label6.Text = "ID";
            // 
            // statementLabel
            // 
            this.statementLabel.AutoSize = true;
            this.statementLabel.Location = new System.Drawing.Point(12, 97);
            this.statementLabel.Name = "statementLabel";
            this.statementLabel.Size = new System.Drawing.Size(55, 13);
            this.statementLabel.TabIndex = 13;
            this.statementLabel.Text = "Statement";
            // 
            // descriptionTextBox
            // 
            this.descriptionTextBox.Location = new System.Drawing.Point(99, 169);
            this.descriptionTextBox.Multiline = true;
            this.descriptionTextBox.Name = "descriptionTextBox";
            this.descriptionTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.descriptionTextBox.Size = new System.Drawing.Size(661, 197);
            this.descriptionTextBox.TabIndex = 21;
            this.descriptionTextBox.TextChanged += new System.EventHandler(this.descriptionTextBox_TextChanged);
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(502, 376);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(60, 23);
            this.okButton.TabIndex = 25;
            this.okButton.Text = "Ok";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(568, 376);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(60, 23);
            this.cancelButton.TabIndex = 24;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // pathLabel
            // 
            this.pathLabel.AutoEllipsis = true;
            this.pathLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pathLabel.ForeColor = System.Drawing.SystemColors.InfoText;
            this.pathLabel.Location = new System.Drawing.Point(14, 9);
            this.pathLabel.Name = "pathLabel";
            this.pathLabel.Size = new System.Drawing.Size(746, 24);
            this.pathLabel.TabIndex = 26;
            this.pathLabel.Text = "Path";
            // 
            // nameTextBox
            // 
            this.nameTextBox.Location = new System.Drawing.Point(99, 69);
            this.nameTextBox.Name = "nameTextBox";
            this.nameTextBox.Size = new System.Drawing.Size(485, 20);
            this.nameTextBox.TabIndex = 27;
            this.nameTextBox.TextChanged += new System.EventHandler(this.nameTextBox_TextChanged);
            // 
            // nameLabel
            // 
            this.nameLabel.AutoSize = true;
            this.nameLabel.Location = new System.Drawing.Point(12, 72);
            this.nameLabel.Name = "nameLabel";
            this.nameLabel.Size = new System.Drawing.Size(35, 13);
            this.nameLabel.TabIndex = 28;
            this.nameLabel.Text = "Name";
            // 
            // statementTextBox
            // 
            this.statementTextBox.Location = new System.Drawing.Point(99, 97);
            this.statementTextBox.Multiline = true;
            this.statementTextBox.Name = "statementTextBox";
            this.statementTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.statementTextBox.Size = new System.Drawing.Size(661, 66);
            this.statementTextBox.TabIndex = 29;
            this.statementTextBox.TextChanged += new System.EventHandler(this.statementTextBox_TextChanged);
            // 
            // helpButton
            // 
            this.helpButton.Enabled = false;
            this.helpButton.Location = new System.Drawing.Point(700, 376);
            this.helpButton.Name = "helpButton";
            this.helpButton.Size = new System.Drawing.Size(60, 23);
            this.helpButton.TabIndex = 31;
            this.helpButton.Text = "Help";
            this.helpButton.UseVisualStyleBackColor = true;
            // 
            // applyButton
            // 
            this.applyButton.Location = new System.Drawing.Point(634, 376);
            this.applyButton.Name = "applyButton";
            this.applyButton.Size = new System.Drawing.Size(60, 23);
            this.applyButton.TabIndex = 30;
            this.applyButton.Text = "Apply";
            this.applyButton.UseVisualStyleBackColor = true;
            this.applyButton.Click += new System.EventHandler(this.applyButton_Click);
            // 
            // priorityComboBox
            // 
            this.priorityComboBox.FormattingEnabled = true;
            this.priorityComboBox.Items.AddRange(new object[] {
            "",
            "EN",
            "EF",
            "OPT"});
            this.priorityComboBox.Location = new System.Drawing.Point(302, 42);
            this.priorityComboBox.Name = "priorityComboBox";
            this.priorityComboBox.Size = new System.Drawing.Size(89, 21);
            this.priorityComboBox.TabIndex = 33;
            this.priorityComboBox.TextChanged += new System.EventHandler(this.priorityComboBox_TextChanged);
            // 
            // priorityLabel
            // 
            this.priorityLabel.AutoSize = true;
            this.priorityLabel.Location = new System.Drawing.Point(249, 45);
            this.priorityLabel.Name = "priorityLabel";
            this.priorityLabel.Size = new System.Drawing.Size(38, 13);
            this.priorityLabel.TabIndex = 32;
            this.priorityLabel.Text = "Priority";
            // 
            // switchLinkLabel
            // 
            this.switchLinkLabel.AutoSize = true;
            this.switchLinkLabel.Location = new System.Drawing.Point(15, 381);
            this.switchLinkLabel.Name = "switchLinkLabel";
            this.switchLinkLabel.Size = new System.Drawing.Size(80, 13);
            this.switchLinkLabel.TabIndex = 34;
            this.switchLinkLabel.TabStop = true;
            this.switchLinkLabel.Text = "Switch Element";
            this.switchLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.switchLinkLabel_LinkClicked);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(438, 381);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 13);
            this.label1.TabIndex = 35;
            this.label1.Text = "* = Profiled";
            // 
            // changeNoteLinkLabel
            // 
            this.changeNoteLinkLabel.AutoSize = true;
            this.changeNoteLinkLabel.Location = new System.Drawing.Point(12, 194);
            this.changeNoteLinkLabel.Name = "changeNoteLinkLabel";
            this.changeNoteLinkLabel.Size = new System.Drawing.Size(70, 13);
            this.changeNoteLinkLabel.TabIndex = 36;
            this.changeNoteLinkLabel.TabStop = true;
            this.changeNoteLinkLabel.Text = "Change Note";
            this.changeNoteLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.changeNoteLinkLabel_LinkClicked);
            // 
            // descriptionLinkLabel
            // 
            this.descriptionLinkLabel.AutoSize = true;
            this.descriptionLinkLabel.Location = new System.Drawing.Point(12, 172);
            this.descriptionLinkLabel.Name = "descriptionLinkLabel";
            this.descriptionLinkLabel.Size = new System.Drawing.Size(60, 13);
            this.descriptionLinkLabel.TabIndex = 36;
            this.descriptionLinkLabel.TabStop = true;
            this.descriptionLinkLabel.Text = "Description";
            this.descriptionLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.descriptionLinkLabel_LinkClicked);
            // 
            // changeNoteTextBox
            // 
            this.changeNoteTextBox.Location = new System.Drawing.Point(99, 169);
            this.changeNoteTextBox.Multiline = true;
            this.changeNoteTextBox.Name = "changeNoteTextBox";
            this.changeNoteTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.changeNoteTextBox.Size = new System.Drawing.Size(661, 197);
            this.changeNoteTextBox.TabIndex = 21;
            this.changeNoteTextBox.TextChanged += new System.EventHandler(this.descriptionTextBox_TextChanged);
            // 
            // lockIcon
            // 
            this.lockIcon.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lockIcon.Image = ((System.Drawing.Image)(resources.GetObject("lockIcon.Image")));
            this.lockIcon.InitialImage = null;
            this.lockIcon.Location = new System.Drawing.Point(744, 9);
            this.lockIcon.Name = "lockIcon";
            this.lockIcon.Size = new System.Drawing.Size(16, 16);
            this.lockIcon.TabIndex = 38;
            this.lockIcon.TabStop = false;
            // 
            // FunctionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(772, 411);
            this.Controls.Add(this.lockIcon);
            this.Controls.Add(this.descriptionLinkLabel);
            this.Controls.Add(this.changeNoteLinkLabel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.switchLinkLabel);
            this.Controls.Add(this.priorityComboBox);
            this.Controls.Add(this.priorityLabel);
            this.Controls.Add(this.helpButton);
            this.Controls.Add(this.applyButton);
            this.Controls.Add(this.statementTextBox);
            this.Controls.Add(this.nameLabel);
            this.Controls.Add(this.nameTextBox);
            this.Controls.Add(this.pathLabel);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.changeNoteTextBox);
            this.Controls.Add(this.descriptionTextBox);
            this.Controls.Add(this.idTextBox);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.statementLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FunctionForm";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "HL7 FM Function";
            ((System.ComponentModel.ISupportInitialize)(this.lockIcon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox idTextBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label statementLabel;
        private System.Windows.Forms.TextBox descriptionTextBox;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Label pathLabel;
        private System.Windows.Forms.TextBox nameTextBox;
        private System.Windows.Forms.Label nameLabel;
        private System.Windows.Forms.TextBox statementTextBox;
        private System.Windows.Forms.Button helpButton;
        private System.Windows.Forms.Button applyButton;
        private System.Windows.Forms.ComboBox priorityComboBox;
        private System.Windows.Forms.Label priorityLabel;
        private System.Windows.Forms.LinkLabel switchLinkLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.LinkLabel changeNoteLinkLabel;
        private System.Windows.Forms.LinkLabel descriptionLinkLabel;
        private System.Windows.Forms.TextBox changeNoteTextBox;
        private System.Windows.Forms.PictureBox lockIcon;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}