namespace MAX_EA_Extension
{
    partial class BatchForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BatchForm));
            this.button5 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.availableTreeView = new System.Windows.Forms.TreeView();
            this.SuspendLayout();
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(352, 294);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(79, 23);
            this.button5.TabIndex = 30;
            this.button5.Text = "Export";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.exportButton_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(104, 13);
            this.label2.TabIndex = 28;
            this.label2.Text = "Available Packages:";
            // 
            // button1
            // 
            this.button1.Enabled = false;
            this.button1.Location = new System.Drawing.Point(267, 294);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(79, 23);
            this.button1.TabIndex = 30;
            this.button1.Text = "Transform";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.exportButton_Click);
            // 
            // button2
            // 
            this.button2.Enabled = false;
            this.button2.Location = new System.Drawing.Point(182, 294);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(79, 23);
            this.button2.TabIndex = 30;
            this.button2.Text = "Validate";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.exportButton_Click);
            // 
            // button3
            // 
            this.button3.Enabled = false;
            this.button3.Location = new System.Drawing.Point(97, 294);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(79, 23);
            this.button3.TabIndex = 30;
            this.button3.Text = "Lock";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.exportButton_Click);
            // 
            // button4
            // 
            this.button4.Enabled = false;
            this.button4.Location = new System.Drawing.Point(12, 294);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(79, 23);
            this.button4.TabIndex = 30;
            this.button4.Text = "Unlock";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.exportButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 278);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(157, 13);
            this.label1.TabIndex = 31;
            this.label1.Text = "Select action for selected items:";
            // 
            // availableTreeView
            // 
            this.availableTreeView.CheckBoxes = true;
            this.availableTreeView.Location = new System.Drawing.Point(12, 26);
            this.availableTreeView.Name = "availableTreeView";
            this.availableTreeView.Size = new System.Drawing.Size(419, 238);
            this.availableTreeView.TabIndex = 32;
            this.availableTreeView.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.availableTreeView_AfterCheck);
            // 
            // BatchForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(447, 326);
            this.Controls.Add(this.availableTreeView);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.label2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "BatchForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Batch Process";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TreeView availableTreeView;

    }
}