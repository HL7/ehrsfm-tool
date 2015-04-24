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
                packageLinkLabel.Text = "none";
                packageLinkLabel.Enabled = false;
                importButton.Enabled = false;
                exportButton.Enabled = false;
                transformButton.Enabled = false;
                validateButton.Enabled = false;
            }
            else
            {
                packageLinkLabel.Text = SelectedPackage.Name;
                packageLinkLabel.Enabled = true;
                importButton.Enabled = true;
                exportButton.Enabled = true;
                transformButton.Enabled = true;
                validateButton.Enabled = true;
            }
        }

        private void importButton_Click(object sender, EventArgs e)
        {
            new MAX_EA.MAXImporter3().import(Repository, SelectedPackage);
        }

        private void exportButton_Click(object sender, EventArgs e)
        {
            new MAX_EA.MAXExporter3().export(Repository);
        }

        private void packageLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Repository.ShowInProjectView(SelectedPackage);
        }

        private void validateButton_Click(object sender, EventArgs e)
        {
            new ValidateParamsForm().Show(Repository);
        }

        private void transformButton_Click(object sender, EventArgs e)
        {
            new TransformParamsForm().Show(Repository);
        }
    }
}
