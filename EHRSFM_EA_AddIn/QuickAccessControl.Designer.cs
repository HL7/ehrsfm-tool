namespace R4C_EHRSFM_EA_AddIn
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
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonImportR11 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.buttonImportR2 = new System.Windows.Forms.Button();
            this.buttonImportMAX = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.buttonValidate = new System.Windows.Forms.Button();
            this.buttonGenHTML = new System.Windows.Forms.Button();
            this.buttonExportMAX = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Enabled = false;
            this.linkLabel1.Location = new System.Drawing.Point(108, 28);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(31, 13);
            this.linkLabel1.TabIndex = 8;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "none";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 28);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(98, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Selected Package:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Roboto", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(3, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(169, 19);
            this.label1.TabIndex = 6;
            this.label1.Text = "HL7 EHR-S FM Tool";
            // 
            // buttonImportR11
            // 
            this.buttonImportR11.Enabled = false;
            this.buttonImportR11.Location = new System.Drawing.Point(6, 19);
            this.buttonImportR11.Name = "buttonImportR11";
            this.buttonImportR11.Size = new System.Drawing.Size(97, 23);
            this.buttonImportR11.TabIndex = 1;
            this.buttonImportR11.Text = "Import R1.1";
            this.buttonImportR11.UseVisualStyleBackColor = true;
            this.buttonImportR11.Click += new System.EventHandler(this.buttonImportR11_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox1.Controls.Add(this.buttonImportR11);
            this.groupBox1.Controls.Add(this.buttonImportR2);
            this.groupBox1.Location = new System.Drawing.Point(7, 65);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(214, 55);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "FM Release Import from XML";
            // 
            // buttonImportR2
            // 
            this.buttonImportR2.Enabled = false;
            this.buttonImportR2.Location = new System.Drawing.Point(110, 19);
            this.buttonImportR2.Name = "buttonImportR2";
            this.buttonImportR2.Size = new System.Drawing.Size(97, 23);
            this.buttonImportR2.TabIndex = 2;
            this.buttonImportR2.Text = "Import R2";
            this.buttonImportR2.UseVisualStyleBackColor = true;
            this.buttonImportR2.Click += new System.EventHandler(this.buttonImportR2_Click);
            // 
            // buttonImportMAX
            // 
            this.buttonImportMAX.Enabled = false;
            this.buttonImportMAX.Location = new System.Drawing.Point(6, 19);
            this.buttonImportMAX.Name = "buttonImportMAX";
            this.buttonImportMAX.Size = new System.Drawing.Size(97, 23);
            this.buttonImportMAX.TabIndex = 3;
            this.buttonImportMAX.Text = "Import MAX";
            this.buttonImportMAX.UseVisualStyleBackColor = true;
            this.buttonImportMAX.Click += new System.EventHandler(this.buttonImportMAX_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox2.Controls.Add(this.buttonValidate);
            this.groupBox2.Controls.Add(this.buttonGenHTML);
            this.groupBox2.Location = new System.Drawing.Point(7, 126);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(214, 55);
            this.groupBox2.TabIndex = 10;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Other FM Tools";
            // 
            // buttonValidate
            // 
            this.buttonValidate.Enabled = false;
            this.buttonValidate.Location = new System.Drawing.Point(6, 19);
            this.buttonValidate.Name = "buttonValidate";
            this.buttonValidate.Size = new System.Drawing.Size(97, 23);
            this.buttonValidate.TabIndex = 1;
            this.buttonValidate.Text = "Validate";
            this.buttonValidate.UseVisualStyleBackColor = true;
            this.buttonValidate.Click += new System.EventHandler(this.buttonValidateR2_Click);
            // 
            // buttonGenHTML
            // 
            this.buttonGenHTML.Enabled = false;
            this.buttonGenHTML.Location = new System.Drawing.Point(109, 19);
            this.buttonGenHTML.Name = "buttonGenHTML";
            this.buttonGenHTML.Size = new System.Drawing.Size(97, 23);
            this.buttonGenHTML.TabIndex = 2;
            this.buttonGenHTML.Text = "Generate HTML";
            this.buttonGenHTML.UseVisualStyleBackColor = true;
            // 
            // buttonExportMAX
            // 
            this.buttonExportMAX.Enabled = false;
            this.buttonExportMAX.Location = new System.Drawing.Point(109, 19);
            this.buttonExportMAX.Name = "buttonExportMAX";
            this.buttonExportMAX.Size = new System.Drawing.Size(97, 23);
            this.buttonExportMAX.TabIndex = 3;
            this.buttonExportMAX.Text = "Export MAX";
            this.buttonExportMAX.UseVisualStyleBackColor = true;
            this.buttonExportMAX.Click += new System.EventHandler(this.buttonExportMAX_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox3.Controls.Add(this.buttonImportMAX);
            this.groupBox3.Controls.Add(this.buttonExportMAX);
            this.groupBox3.Location = new System.Drawing.Point(7, 187);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(214, 55);
            this.groupBox3.TabIndex = 11;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Model Automation eXchange";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.label3.Location = new System.Drawing.Point(153, 52);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(73, 13);
            this.label3.TabIndex = 12;
            this.label3.Text = "v0000-aaa-00";
            // 
            // QuickAccessControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label3);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "QuickAccessControl";
            this.Size = new System.Drawing.Size(243, 280);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonImportR11;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button buttonImportR2;
        private System.Windows.Forms.Button buttonImportMAX;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button buttonValidate;
        private System.Windows.Forms.Button buttonGenHTML;
        private System.Windows.Forms.Button buttonExportMAX;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label3;

    }
}
