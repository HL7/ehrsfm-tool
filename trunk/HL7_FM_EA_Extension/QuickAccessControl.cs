using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace HL7_FM_EA_Extension
{
    public partial class QuickAccessControl : UserControl
    {
        public QuickAccessControl()
        {
            InitializeComponent();
            label3.Text = string.Format("v{0}", Assembly.GetExecutingAssembly().GetName().Version.ToString());
        }

        EA.Repository Repository;
        EA.Package SelectedPackage;
        public void SetRepository(EA.Repository Repository)
        {
            this.Repository = Repository;
            SetSelectedPackage(Repository.GetTreeSelectedPackage());
        }

        public void SetSelectedPackage(EA.Package SelectedPackage)
        {
            this.SelectedPackage = SelectedPackage;
            if (SelectedPackage == null)
            {
                linkLabel1.Text = "none";
                linkLabel1.Enabled = false;
                buttonImportR11.Enabled = false;
                buttonImportR2.Enabled = false;
                buttonImportMAX.Enabled = false;
                buttonValidate.Enabled = false;
                buttonGenHTML.Enabled = false;
                buttonExportMAX.Enabled = false;
            }
            else
            {
                linkLabel1.Text = SelectedPackage.Name;
                linkLabel1.Enabled = true;
                buttonImportR11.Enabled = true;
                buttonImportR2.Enabled = true;
                buttonImportMAX.Enabled = true;
                buttonValidate.Enabled = true;
                buttonGenHTML.Enabled = false;
                buttonExportMAX.Enabled = true;
            }
        }

        private void buttonExportMAX_Click(object sender, EventArgs e)
        {
            new MAX_EA.MAXExporter3().export(Repository);
        }

        private void buttonImportMAX_Click(object sender, EventArgs e)
        {
            new MAX_EA.MAXImporter3().import(Repository, SelectedPackage);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Repository.ShowInProjectView(SelectedPackage);
        }

        private void buttonImportR11_Click(object sender, EventArgs e)
        {
            new R11Importer().import(Repository, SelectedPackage);
        }

        private void buttonImportR2_Click(object sender, EventArgs e)
        {
            new R2Importer().import(Repository, SelectedPackage);
        }

        private void buttonValidateR2_Click(object sender, EventArgs e)
        {
            new R2Validator().validate(Repository, SelectedPackage);
        }
    }
}
