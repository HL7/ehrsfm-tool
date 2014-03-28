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

        private bool codeChangedValue = false;
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
                    Text = string.Format("Profile Definition for Section: {0}", SectionPackage.Name);

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
                EA.Element _element = Repository.GetElementByID(con.ClientID);
                if (_element.PackageID == ProfileDefinition.PackageID)
                {
                    dl.compilerInstructionElement = _element;
                    item.Text = string.Format("{0} {1}", _element.Name, _element.Notes);
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
                codeChangedValue = true;
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
                codeChangedValue = false;

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
            if (!codeChangedValue && mainListView.SelectedItems.Count > 0)
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
                setCompilerInstruction(dl, R2Const.Qualifier.None);
            }
            // Deprecate
            else if (radioButton2.Checked)
            {
                setCompilerInstruction(dl, R2Const.Qualifier.Deprecate);
            }
            // Delete
            else if (radioButton3.Checked)
            {
                setCompilerInstruction(dl, R2Const.Qualifier.Delete);
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
                    case "EXCLUDE": // special for excluded criteria
                        item.ForeColor = Color.LightGray;
                        item.BackColor = BACKCOLOR_EXCLUDED;
                        break;
                }
            }
            else
            {
                item.ForeColor = Color.Black;
                item.BackColor = BACKCOLOR_EXCLUDED;
            }
        }

        private void setCompilerInstruction(DefinitionLink dl, R2Const.Qualifier qualifier, string optionality = null, string change_note = null)
        {
            if (dl.compilerInstructionElement == null)
            {
                dl.compilerInstructionElement = (EA.Element)ProfileDefinition.Elements.AddNew(dl.baseModelElement.Name, "Class");
                dl.compilerInstructionElement.Stereotype = R2Const.ST_COMPILERINSTRUCTION;
                dl.compilerInstructionElement.Update();
                EA.Connector con = (EA.Connector)dl.compilerInstructionElement.Connectors.AddNew("", "Generalization");
                con.SupplierID = dl.baseModelElement.ElementID;
                con.Update();
                dl.compilerInstructionElement.Connectors.Refresh();
            }

            // TODO: remove qualifier if null or empty
            EA.TaggedValue tvQualifier = (EA.TaggedValue)dl.compilerInstructionElement.TaggedValues.GetByName("Qualifier");
            if (tvQualifier == null)
            {
                tvQualifier = (EA.TaggedValue)dl.compilerInstructionElement.TaggedValues.AddNew("Qualifier", "");
            }
            switch(qualifier)
            {
                case R2Const.Qualifier.Deprecate:
                    tvQualifier.Value = "DEP";
                    break;
                case R2Const.Qualifier.Delete:
                    tvQualifier.Value = "D";
                    break;
                case R2Const.Qualifier.Exclude:
                    tvQualifier.Value = "EXCLUDE";
                    break;
                default:
                    tvQualifier.Value = "";
                    break;
            }
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
                EA.TaggedValue tvChangeNote = null;
                if (ciElement != null)
                {
                    tvChangeNote = (EA.TaggedValue)ciElement.TaggedValues.GetByName("ChangeNote");
                }
                if (tvChangeNote != null)
                {
                    codeChangedValue = true;
                    textBox1.Text = tvChangeNote.Notes;
                    codeChangedValue = false;
                }
                else
                {
                    codeChangedValue = true;
                    textBox1.Text = "";
                    codeChangedValue = false;
                }
                EA.TaggedValue tvQualifier = null;
                if (ciElement != null)
                {
                    tvQualifier = (EA.TaggedValue)ciElement.TaggedValues.GetByName("Qualifier");
                }
                if (tvQualifier != null)
                {
                    codeChangedValue = true;
                    checkBox1.Checked = "EXCLUDE".Equals(tvQualifier.Value);
                    codeChangedValue = false;
                }
                else
                {
                    codeChangedValue = true;
                    checkBox1.Checked = false;
                    codeChangedValue = false;
                }

                groupBox3.Show();
            }
            else
            {
                groupBox3.Hide();
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (!codeChangedValue)
            {
                ListViewItem selected = criteriaListView.SelectedItems[0];
                DefinitionLink dl = (DefinitionLink) selected.Tag;
                setCompilerInstruction(dl, R2Const.Qualifier.None, null, textBox1.Text);
            }
        }

        private void criteriaListView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListViewItem selected = criteriaListView.SelectedItems[0];
            DefinitionLink dl = (DefinitionLink)selected.Tag;
            // make sure CI is created first
            setCompilerInstruction(dl, R2Const.Qualifier.None, null, textBox1.Text);

            R2Criterion criterion = (R2Criterion)R2Model.Create(Repository, dl.compilerInstructionElement);
            new CriterionForm().Show(criterion);

            // Update Criterion Text in Critaria List
            selected.Text = string.Format("{0} {1}", criterion.Name, criterion.Text);
        }

        private void mainListView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListViewItem selected = mainListView.SelectedItems[0];
            EA.Element element = ((DefinitionLink)selected.Tag).baseModelElement;
            R2Function function = (R2Function)R2Model.Create(Repository, element);
            new FunctionForm().Show(function);
        }

        private void findButton_Click(object sender, EventArgs e)
        {
            string id = textBox2.Text.ToUpper();
            foreach (ListViewItem item in mainListView.Items)
            {
                DefinitionLink dl = (DefinitionLink)item.Tag;
                if (dl.baseModelElement.Name.ToUpper().StartsWith(id))
                {
                    mainListView.SelectedItems.Clear();
                    item.Selected = true;
                    mainListView.EnsureVisible(item.Index);
                    mainListView.Select();
                    return;
                }
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (!codeChangedValue)
            {
                ListViewItem selected = criteriaListView.SelectedItems[0];
                DefinitionLink dl = (DefinitionLink) selected.Tag;
                if (checkBox1.Checked)
                {
                    setCompilerInstruction(dl, R2Const.Qualifier.Exclude);
                }
                else
                {
                    setCompilerInstruction(dl, R2Const.Qualifier.None);
                }
                updateListViewItem(selected);
            }
        }
    }

    public class DefinitionLink
    {
        public EA.Element baseModelElement;
        public EA.Element compilerInstructionElement;
    }
}
