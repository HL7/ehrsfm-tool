namespace MAX_EA_Extension
{
    partial class QuickAccessControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.packageLinkLabel = new System.Windows.Forms.LinkLabel();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.importButton = new System.Windows.Forms.Button();
            this.validateButton = new System.Windows.Forms.Button();
            this.transformButton = new System.Windows.Forms.Button();
            this.exportButton = new System.Windows.Forms.Button();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // packageLinkLabel
            // 
            this.packageLinkLabel.AutoSize = true;
            this.packageLinkLabel.Enabled = false;
            this.packageLinkLabel.Location = new System.Drawing.Point(10, 39);
            this.packageLinkLabel.Name = "packageLinkLabel";
            this.packageLinkLabel.Size = new System.Drawing.Size(31, 13);
            this.packageLinkLabel.TabIndex = 10;
            this.packageLinkLabel.TabStop = true;
            this.packageLinkLabel.Text = "none";
            this.packageLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.packageLinkLabel_LinkClicked);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 26);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(98, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Selected Package:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Roboto", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(3, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(142, 19);
            this.label1.TabIndex = 8;
            this.label1.Text = "MAX Quick Access";
            // 
            // groupBox2
            // 
            this.groupBox2.AutoSize = true;
            this.groupBox2.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox2.Controls.Add(this.validateButton);
            this.groupBox2.Controls.Add(this.transformButton);
            this.groupBox2.Controls.Add(this.exportButton);
            this.groupBox2.Controls.Add(this.importButton);
            this.groupBox2.Location = new System.Drawing.Point(7, 55);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(221, 117);
            this.groupBox2.TabIndex = 11;
            this.groupBox2.TabStop = false;
            // 
            // importButton
            // 
            this.importButton.Enabled = false;
            this.importButton.Location = new System.Drawing.Point(6, 19);
            this.importButton.Name = "importButton";
            this.importButton.Size = new System.Drawing.Size(64, 23);
            this.importButton.TabIndex = 1;
            this.importButton.Text = "Import";
            this.importButton.UseVisualStyleBackColor = true;
            this.importButton.Click += new System.EventHandler(this.importButton_Click);
            // 
            // validateButton
            // 
            this.validateButton.Enabled = false;
            this.validateButton.Location = new System.Drawing.Point(76, 48);
            this.validateButton.Name = "validateButton";
            this.validateButton.Size = new System.Drawing.Size(64, 23);
            this.validateButton.TabIndex = 2;
            this.validateButton.Text = "Validate";
            this.validateButton.UseVisualStyleBackColor = true;
            this.validateButton.Click += new System.EventHandler(this.validateButton_Click);
            // 
            // transformButton
            // 
            this.transformButton.Enabled = false;
            this.transformButton.Location = new System.Drawing.Point(6, 48);
            this.transformButton.Name = "transformButton";
            this.transformButton.Size = new System.Drawing.Size(64, 23);
            this.transformButton.TabIndex = 2;
            this.transformButton.Text = "Transform";
            this.transformButton.UseVisualStyleBackColor = true;
            this.transformButton.Click += new System.EventHandler(this.transformButton_Click);
            // 
            // exportButton
            // 
            this.exportButton.Enabled = false;
            this.exportButton.Location = new System.Drawing.Point(76, 19);
            this.exportButton.Name = "exportButton";
            this.exportButton.Size = new System.Drawing.Size(64, 23);
            this.exportButton.TabIndex = 2;
            this.exportButton.Text = "Export";
            this.exportButton.UseVisualStyleBackColor = true;
            this.exportButton.Click += new System.EventHandler(this.exportButton_Click);
            // 
            // QuickAccessControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.packageLinkLabel);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.label1);
            this.Name = "QuickAccessControl";
            this.Size = new System.Drawing.Size(231, 175);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.LinkLabel packageLinkLabel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button importButton;
        private System.Windows.Forms.Button exportButton;
        private System.Windows.Forms.Button transformButton;
        private System.Windows.Forms.Button validateButton;
    }
}
