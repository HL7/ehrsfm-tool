using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml.XPath;

namespace HL7_FM_EA_Extension
{
    public partial class ProfilingForm : Form
    {
        public ProfilingForm()
        {
            InitializeComponent();
        }

        private readonly Color BACKCOLOR_EXCLUDED = Color.White;
        private readonly Color BACKCOLOR_INCLUDED = Color.DarkGreen;
        private readonly Color BACKCOLOR_DEPRECATED = Color.Orange;
        private readonly Color BACKCOLOR_DELETED = Color.Red;

        private bool ignoreCheckChanged = false;
        private EA.Repository Repository;
        private EA.Package SectionPackage;
        private EA.Package ProfileDefinition;
        private ListViewGroup mainGroup;

        public void Show(EA.Repository Repository)
        {
            this.Repository = Repository;
            SectionPackage = Repository.GetTreeSelectedPackage();
            if (SectionPackage != null && R2Const.ST_SECTION.Equals(SectionPackage.StereotypeEx))
            {
                if (findAssociatedProfileDefinition())
                {
                    mainListView.Items.Clear();
                    mainListView.Groups.Clear();
                    mainGroup = new ListViewGroup("");
                    mainListView.Groups.Add(mainGroup);
                    mainListView.Columns.Add("stereotype");

                    ListViewItem item = createListViewItem(SectionPackage);
                    item.Group = mainGroup;
                    mainListView.Items.Add(item);
                    visitPackage(SectionPackage);
                    Show();
                }
            }
            else
            {
                MessageBox.Show("Please select a Base FM Section to Profile.");
            }
        }

        private bool findAssociatedProfileDefinition()
        {
            EA.Package BaseModel = Repository.GetPackageByID(SectionPackage.ParentID);
            EA.Connector con = BaseModel.Connectors.Cast<EA.Connector>().SingleOrDefault(t => R2Const.ST_BASEMODEL.Equals(t.Stereotype));
            if (con != null)
            {
                EA.Element packageElement = Repository.GetElementByID(con.ClientID);
                if (R2Const.ST_FM_PROFILEDEFINITION.Equals(packageElement.Stereotype))
                {
                    // con.ClientID is the ElementID of the PackageElement
                    // Find the Package with the PackageElement by selecting the child Package in the parent Package where
                    // the ElementID is con.ClientID
                    ProfileDefinition = Repository.GetPackageByID(packageElement.PackageID).Packages.Cast<EA.Package>().Single(p => p.Element.ElementID == con.ClientID);
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

        private void visitPackage(EA.Package package)
        {
            foreach (EA.Package childPackage in package.Packages)
            {
                ListViewItem subItem = createListViewItem(childPackage);
                mainListView.Items.Add(subItem);
                visitPackage(childPackage);
            }
            createElementGroup(string.Format("«{0}» {1}", package.StereotypeEx, package.Name), package.Elements);
        }

        private void createElementGroup(string groupName, EA.Collection elements)
        {
            ListViewGroup group = new ListViewGroup(groupName);
            group.HeaderAlignment = HorizontalAlignment.Center;
            mainListView.Groups.Add(group);
            foreach (EA.Element element in elements)
            {
                string stereotype = element.Stereotype;
                switch (stereotype)
                {
                    case "Section":
                    case "Header":
                    case "Function":
                        ListViewItem item = createListViewItem(element);
                        item.Group = group;
                        mainListView.Items.Add(item);
                        break;
                }

                // if (element.Elements.Count > 0)
                // Only create group if there is anything else than Criteria in here
                if (element.Elements.Cast<EA.Element>().Any(t => !t.Stereotype.Equals("Criteria")))
                {
                    createElementGroup(string.Format("«{0}» {1}", element.Stereotype, element.Name), element.Elements);
                }
            }
        }

        private ListViewItem createListViewItem(EA.Package package)
        {
            ListViewItem item = new ListViewItem(package.Name);
            DefinitionLink dl = new DefinitionLink();
            dl.baseModelElement = package.Element;
            item.Tag = dl;
            if (!string.IsNullOrEmpty(package.Element.Stereotype))
            {
                ListViewItem.ListViewSubItem subItem = new ListViewItem.ListViewSubItem();
                subItem.Name = "stereotype";
                subItem.Text = package.Element.Stereotype;
                item.SubItems.Add(subItem);
            }
            return item;
        }

        private ListViewItem createListViewItem(EA.Element element)
        {
            ListViewItem item = new ListViewItem(element.Name);
            DefinitionLink dl = new DefinitionLink();
            item.Tag = dl;
            dl.baseModelElement = element;
            // Find compilerinstruction by looking for the generalization connector that points
            // to an Element in the ProfileDefinition Package
            foreach (EA.Connector con in dl.baseModelElement.Connectors.Cast<EA.Connector>().Where(c => "Generalization".Equals(c.Type)))
            {
                EA.Element _element = (EA.Element)Repository.GetElementByID(con.ClientID);
                if (_element.PackageID == ProfileDefinition.PackageID)
                {
                    dl.compilerInstructionElement = _element;
                    updateListViewItem(item);
                    break;
                }
            }
            if (!string.IsNullOrEmpty(element.Stereotype))
            {
                ListViewItem.ListViewSubItem subItem = new ListViewItem.ListViewSubItem();
                subItem.Name = "stereotype";
                subItem.Text = element.Stereotype;
                item.SubItems.Add(subItem);
            }
            return item;
        }

        private ListViewItem createCriteriaListViewItem(EA.Element element)
        {
            ListViewItem item = new ListViewItem(string.Format("{0} {1}", element.Name, element.Notes));
            DefinitionLink dl = new DefinitionLink();
            item.Tag = dl;
            dl.baseModelElement = element;
            // Find compilerinstruction by looking for the generalization connector that points
            // to an Element in the ProfileDefinition Package
            foreach (EA.Connector con in dl.baseModelElement.Connectors.Cast<EA.Connector>().Where(c => "Generalization".Equals(c.Type)))
            {
                EA.Element _element = (EA.Element)Repository.GetElementByID(con.ClientID);
                if (_element.PackageID == ProfileDefinition.PackageID)
                {
                    dl.compilerInstructionElement = _element;
                    updateListViewItem(item);
                    break;
                }
            }
            return item;
        }

        private void mainListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            groupBox2.Hide();
            groupBox3.Hide();

            if (mainListView.SelectedItems.Count > 0)
            {
                ListViewItem selected = mainListView.SelectedItems[0];
                EA.Element element = ((DefinitionLink) selected.Tag).baseModelElement;

                // Also select in Project Browser
                Repository.ShowInProjectView(element);

                // Update checkbox states
                ignoreCheckChanged = true;
                if (selected.BackColor == BACKCOLOR_INCLUDED)
                {
                    radioButton1.Checked = true;
                }
                else if (selected.BackColor == BACKCOLOR_DEPRECATED)
                {
                    radioButton2.Checked = true;
                }
                else if (selected.BackColor == BACKCOLOR_DELETED)
                {
                    radioButton3.Checked = true;
                }
                else
                {
                    radioButton4.Checked = true;
                }
                ignoreCheckChanged = false;

                // Update listBox with Criteria of selected
                criteriaListView.Items.Clear();
                foreach (EA.Element child in element.Elements)
                {
                    if ("Criteria".Equals(child.Stereotype))
                    {
                        criteriaListView.Items.Add(createCriteriaListViewItem(child));
                    }
                }
                groupBox2.Show();
            }
        }

        private void updateSelectedListViewItem()
        {
            if (!ignoreCheckChanged && mainListView.SelectedItems.Count > 0)
            {
                ListViewItem selected = mainListView.SelectedItems[0];
                updateCompilerInstruction(selected);
                updateListViewItem(selected);
            }
        }

        private void updateCompilerInstruction(ListViewItem item)
        {
            DefinitionLink dl = (DefinitionLink)item.Tag;
            // Include
            if (radioButton1.Checked)
            {
                setCompilerInstruction(dl, "");
            }
            // Deprecate
            else if (radioButton2.Checked)
            {
                setCompilerInstruction(dl, "DEP");
            }
            // Delete
            else if (radioButton3.Checked)
            {
                setCompilerInstruction(dl, "D");
            }
            // Exclude
            else if (radioButton4.Checked)
            {
                deleteCompilerInstruction(dl);
            }
        }

        private void updateListViewItem(ListViewItem item)
        {
            DefinitionLink ci = (DefinitionLink)item.Tag;
            if (ci.compilerInstructionElement != null)
            {
                EA.TaggedValue tvQualifier = (EA.TaggedValue) ci.compilerInstructionElement.TaggedValues.GetByName("Qualifier");
                switch(tvQualifier.Value)
                {
                    case "":
                        item.ForeColor = Color.White;
                        item.BackColor = BACKCOLOR_INCLUDED;
                        break;
                    case "DEP":
                        item.ForeColor = Color.White;
                        item.BackColor = BACKCOLOR_DEPRECATED;
                        break;
                    case "D":
                        item.ForeColor = Color.White;
                        item.BackColor = BACKCOLOR_DELETED;
                        break;
                }
            }
            else
            {
                item.ForeColor = Color.Black;
                item.BackColor = BACKCOLOR_EXCLUDED;
            }
        }

        private void setCompilerInstruction(DefinitionLink dl, string qualifier, string optionality = null, string change_note = null)
        {
            if (dl.compilerInstructionElement == null)
            {
                dl.compilerInstructionElement = (EA.Element)ProfileDefinition.Elements.AddNew(dl.baseModelElement.Name, "Class");
                dl.compilerInstructionElement.Stereotype = R2Const.ST_COMPILERINSTRUCTION;
                dl.compilerInstructionElement.Update();
                EA.Connector con = (EA.Connector)dl.compilerInstructionElement.Connectors.AddNew("", "Generalization");
                con.SupplierID = dl.baseModelElement.ElementID;
                con.Update();
            }

            // TODO: remove qualifier if null or empty
            EA.TaggedValue tvQualifier = (EA.TaggedValue)dl.compilerInstructionElement.TaggedValues.GetByName("Qualifier");
            if (tvQualifier == null)
            {
                tvQualifier = (EA.TaggedValue)dl.compilerInstructionElement.TaggedValues.AddNew("Qualifier", "");
            }
            tvQualifier.Value = qualifier;
            tvQualifier.Update();

            // TODO: remove optionality if null or empty
            if (!string.IsNullOrEmpty(optionality))
            {
                EA.TaggedValue tvOptionality = (EA.TaggedValue)dl.compilerInstructionElement.TaggedValues.GetByName("Optionality");
                if (tvOptionality == null)
                {
                    tvOptionality = (EA.TaggedValue)dl.compilerInstructionElement.TaggedValues.AddNew("Optionality", "");
                }
                tvOptionality.Value = optionality;
                tvOptionality.Update();
            }

            if (!string.IsNullOrEmpty(change_note))
            {
                EA.TaggedValue tvChangeNote = (EA.TaggedValue)dl.compilerInstructionElement.TaggedValues.GetByName("ChangeNote");
                if (tvChangeNote == null)
                {
                    tvChangeNote = (EA.TaggedValue)dl.compilerInstructionElement.TaggedValues.AddNew("ChangeNote", "");
                    tvChangeNote.Value = "<memo>";
                }
                tvChangeNote.Notes = change_note;
                tvChangeNote.Update();
            }

            dl.compilerInstructionElement.TaggedValues.Refresh();
            dl.compilerInstructionElement.Refresh();
        }

        private void deleteCompilerInstruction(DefinitionLink ci)
        {
            for (short index = 0; index < ProfileDefinition.Elements.Count; index++)
            {
                EA.Element _element = (EA.Element)ProfileDefinition.Elements.GetAt(index);
                if (_element.ElementID == ci.compilerInstructionElement.ElementID)
                {
                    ProfileDefinition.Elements.Delete(index);
                    break;
                }
            }
            ProfileDefinition.Elements.Refresh();
            ci.compilerInstructionElement = null;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            updateSelectedListViewItem();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            updateSelectedListViewItem();
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            updateSelectedListViewItem();
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            updateSelectedListViewItem();
        }

        private void criteriaListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (criteriaListView.SelectedItems.Count > 0)
            {
                ListViewItem selected = criteriaListView.SelectedItems[0];
                EA.Element ciElement = ((DefinitionLink)selected.Tag).compilerInstructionElement;

                EA.TaggedValue tvOptionality = null;
                if (ciElement != null)
                {
                    tvOptionality = (EA.TaggedValue)ciElement.TaggedValues.GetByName("Optionality");
                }
                if (tvOptionality == null)
                {
                    EA.Element element = ((DefinitionLink)selected.Tag).baseModelElement;
                    tvOptionality = (EA.TaggedValue)element.TaggedValues.GetByName("Optionality");
                }
                if (tvOptionality != null)
                {
                    ignoreCheckChanged = true;
                    optionalityComboBox.SelectedItem = tvOptionality.Value;
                    ignoreCheckChanged = false;
                }
                else
                {
                    ignoreCheckChanged = true;
                    optionalityComboBox.SelectedItem = "";
                    ignoreCheckChanged = false;
                }

                EA.TaggedValue tvChangeNote = null;
                if (ciElement != null)
                {
                    tvChangeNote = (EA.TaggedValue)ciElement.TaggedValues.GetByName("ChangeNote");
                }
                if (tvChangeNote != null)
                {
                    ignoreCheckChanged = true;
                    textBox1.Text = tvChangeNote.Notes;
                    ignoreCheckChanged = false;
                }
                else
                {
                    ignoreCheckChanged = true;
                    textBox1.Text = "";
                    ignoreCheckChanged = false;
                }

                groupBox3.Show();
            }
            else
            {
                groupBox3.Hide();
            }
        }

        private void optionalityComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!ignoreCheckChanged && criteriaListView.SelectedItems.Count > 0)
            {
                ListViewItem selected = criteriaListView.SelectedItems[0];
                DefinitionLink dl = (DefinitionLink) selected.Tag;
                string newOptionality = (string)optionalityComboBox.SelectedItem;
                setCompilerInstruction(dl, "", newOptionality);
                updateListViewItem(selected);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (!ignoreCheckChanged)
            {
                ListViewItem selected = criteriaListView.SelectedItems[0];
                DefinitionLink dl = (DefinitionLink) selected.Tag;
                setCompilerInstruction(dl, "", null, textBox1.Text);
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            string id = textBox2.Text;
            foreach (ListViewItem item in mainListView.Items)
            {
                DefinitionLink dl = (DefinitionLink)item.Tag;
                if (dl.baseModelElement.Name.StartsWith(id))
                {
                    mainListView.SelectedItems.Clear();
                    item.Selected = true;
                    mainListView.EnsureVisible(item.Index);
                    return;
                }
            }
        }
    }

    public class DefinitionLink
    {
        public EA.Element baseModelElement;
        public EA.Element compilerInstructionElement;
    }
}
