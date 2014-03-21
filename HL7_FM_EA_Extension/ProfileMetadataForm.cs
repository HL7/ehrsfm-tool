using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HL7_FM_EA_Extension
{
    public partial class ProfileMetadataForm : Form
    {
        public ProfileMetadataForm()
        {
            InitializeComponent();
        }

        private EA.Package ProfileDefinitionPackage;
        private EA.Package BaseModelPackage;

        public void Show(EA.Repository Repository, EA.Package ProfileDefinitionPackage)
        {
            this.ProfileDefinitionPackage = ProfileDefinitionPackage;
            EA.Element element = ProfileDefinitionPackage.Element;

            nameTextBox.Text = element.Name;
            versionTextBox.Text = element.Version;
            dateTimePicker1.Value = element.Modified;
            comboBox1.SelectedItem = EAHelper.getTaggedValue(element, "Type");
            rationaleTextBox.Text = EAHelper.getTaggedValueNotes(element, "Rationale");
            scopeTextBox.Text = EAHelper.getTaggedValueNotes(element, "Scope");
            prioDescTextBox.Text = EAHelper.getTaggedValueNotes(element, "PrioritiesDescription");

            if (findAssociatedBaseModel(Repository, ProfileDefinitionPackage))
            {
                // TODO: If basemodel is profile, then use tagged values to construct name
                baseModelTextBox.Text = BaseModelPackage.Name;
                ShowDialog();
            }
        }

        private bool findAssociatedBaseModel(EA.Repository Repository, EA.Package ProfileDefinitionPackage)
        {
            EA.Connector con = ProfileDefinitionPackage.Connectors.Cast<EA.Connector>().SingleOrDefault(t => R2Const.ST_BASEMODEL.Equals(t.Stereotype));
            if (con != null)
            {
                EA.Element packageElement = Repository.GetElementByID(con.SupplierID);
                if (R2Const.ST_FM.Equals(packageElement.Stereotype) || R2Const.ST_FM_PROFILE.Equals(packageElement.Stereotype))
                {
                    // con.SupplierID is the ElementID of the PackageElement
                    // Find the Package with the PackageElement by selecting the child Package in the parent Package where
                    // the ElementID is con.SupplierID
                    BaseModelPackage = Repository.GetPackageByID(packageElement.PackageID).Packages.Cast<EA.Package>().Single(p => p.Element.ElementID == con.SupplierID);
                    return true;
                }
                else
                {
                    MessageBox.Show("First setup Profile Definition Package.");
                    return false;
                }
            }
            else
            {
                MessageBox.Show("First setup Profile Definition Package.");
                return false;
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                this.Close();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            Close();

            EA.Element element = ProfileDefinitionPackage.Element;
            element.Name = nameTextBox.Text;
            element.Version = versionTextBox.Text;
            element.Modified = dateTimePicker1.Value;
            element.Update();

            EAHelper.updateTaggedValue(element, "Type", comboBox1.SelectedItem.ToString());
            EAHelper.updateTaggedValue(element, "Rationale", "<memo>", rationaleTextBox.Text);
            EAHelper.updateTaggedValue(element, "Scope", "<memo>", scopeTextBox.Text);
            EAHelper.updateTaggedValue(element, "PrioritiesDefinition", "<memo>", prioDescTextBox.Text);
        }
    }
}
