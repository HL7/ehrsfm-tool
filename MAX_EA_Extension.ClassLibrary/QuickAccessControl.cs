using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MAX_EA_Extension
{
    public partial class QuickAccessControl : UserControl
    {
        public QuickAccessControl()
        {
            InitializeComponent();
        }

        EA.Repository Repository;
        EA.Package SelectedPackage;
        internal void SetRepository(EA.Repository Repository)
        {
            this.Repository = Repository;
            SetSelectedPackage(Repository.GetTreeSelectedPackage());
        }

        internal void SetSelectedPackage(EA.Package SelectedPackage)
        {
            this.SelectedPackage = SelectedPackage;
            if (SelectedPackage == null)
            {
                linkLabel1.Text = "none";
                linkLabel1.Enabled = false;
                button1.Enabled = false;
                button2.Enabled = false;
            }
            else
            {
                linkLabel1.Text = SelectedPackage.Name;
                linkLabel1.Enabled = true;
                button1.Enabled = true;
                button2.Enabled = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            new MAX_EA.MAXImporter3().import(Repository, SelectedPackage);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            new MAX_EA.MAXExporter3().export(Repository);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Repository.ShowInProjectView(SelectedPackage);
        }
    }
}
