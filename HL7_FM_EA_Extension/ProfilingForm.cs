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
using HL7_FM_EA_Extension.R2ModelV2.Base;

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

        private bool ignoreEvent = false;
        private EA.Repository repository;
        private EA.Package profileDefinitionPackage;
        private ListViewGroup mainGroup;

        public void Show(EA.Repository repository, EA.Package selectedPackage)
        {
            this.repository = repository;
            if (repository.GetTreeSelectedItemType() == EA.ObjectType.otElement)
            {
                EA.Element selectedElement = (EA.Element) repository.GetTreeSelectedObject();
                if (R2Const.ST_HEADER.Equals(selectedElement.StereotypeEx) || R2Const.ST_FUNCTION.Equals(selectedElement.StereotypeEx))
                {
                    profileDefinitionPackage = EAHelper.getAssociatedProfileDefinition(repository, selectedPackage);
                    if (profileDefinitionPackage != null)
                    {
                        Text = string.Format("Profile Definition for {0}: {1}", selectedElement.StereotypeEx, selectedElement.Name);
                        Show();
                        Refresh();

                        mainListView.Items.Clear();
                        mainListView.Groups.Clear();
                        mainGroup = new ListViewGroup("");
                        mainListView.Groups.Add(mainGroup);

                        ListViewItem item = createListViewItem(selectedElement);
                        item.Group = mainGroup;
                        mainListView.Items.Add(item);

                        createElementGroup(string.Format("«{0}» {1}", selectedElement.StereotypeEx, selectedElement.Name), selectedElement.Elements);
                    }
                    else
                    {
                        MessageBox.Show(Main.MESSAGE_PROFILE_DEFINITION);
                    }
                }
            }
            else if (selectedPackage != null && R2Const.ST_SECTION.Equals(selectedPackage.StereotypeEx))
            {
                profileDefinitionPackage = EAHelper.getAssociatedProfileDefinition(repository, selectedPackage);
                if (profileDefinitionPackage != null)
                {
                    Text = string.Format("Profile Definition for Section: {0}", selectedPackage.Name);
                    Show();
                    Refresh();

                    mainListView.Items.Clear();
                    mainListView.Groups.Clear();
                    mainGroup = new ListViewGroup("");
                    mainListView.Groups.Add(mainGroup);

                    ListViewItem item = createListViewItem(selectedPackage);
                    item.Group = mainGroup;
                    mainListView.Items.Add(item);

                    Cursor = Cursors.WaitCursor;
                    visitPackage(selectedPackage);
                    Cursor = Cursors.Default;
                }
                else
                {
                    MessageBox.Show(Main.MESSAGE_PROFILE_DEFINITION);
                }
            }
            else
            {
                MessageBox.Show("Select a Base FM Section to Profile.");
            }
        }

        private void visitPackage(EA.Package package)
        {
            foreach (EA.Package childPackage in package.Packages)
            {
                ListViewItem item = createListViewItem(childPackage);
                mainListView.Items.Add(item);
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
                    case R2Const.ST_SECTION:
                    case R2Const.ST_HEADER:
                    case R2Const.ST_FUNCTION:
                        ListViewItem item = createListViewItem(element);
                        item.Group = group;
                        mainListView.Items.Add(item);
                        break;
                }

                // Only create group if there is anything else than Criteria in here
                if (element.Elements.Cast<EA.Element>().Any(t => !t.Stereotype.Equals(R2Const.ST_CRITERION)))
                {
                    createElementGroup(string.Format("«{0}» {1}", element.Stereotype, element.Name), element.Elements);
                }
            }
        }

        private ListViewItem createListViewItem(EA.Package package)
        {
            ListViewItem item = new ListViewItem(package.Name);
            item.Tag = new DefinitionLink(repository, package.Element);
            return item;
        }

        private ListViewItem createListViewItem(EA.Element element)
        {
            ListViewItem item = new ListViewItem(element.Name);
            item.Tag = new DefinitionLink(repository, element);
            updateListViewItem(item);
            return item;
        }

        private ListViewItem createCriteriaListViewItem(EA.Element element)
        {
            ListViewItem item = new ListViewItem();
            DefinitionLink dl = new DefinitionLink(repository, element);
            item.Tag = dl;
            R2Criterion criterion = (R2Criterion)dl.modelElement;
            item.Text = string.Format("{0} {1}", criterion.Name, criterion.Text);
            updateListViewItem(item);
            return item;
        }

        private void mainListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            groupBox2.Hide();
            groupBox3.Hide();

            if (mainListView.SelectedItems.Count > 0)
            {
                ListViewItem selected = mainListView.SelectedItems[0];
                EA.Element element = ((DefinitionLink) selected.Tag).baseModelElement;

                // Update checkbox states
                ignoreEvent = true;
                if (selected.BackColor == BACKCOLOR_INCLUDED)
                {
                    includeRadioButton.Checked = true;
                }
                else if (selected.BackColor == BACKCOLOR_DEPRECATED)
                {
                    deprecateRadioButton.Checked = true;
                }
                else if (selected.BackColor == BACKCOLOR_DELETED)
                {
                    deleteRadioButton.Checked = true;
                }
                else
                {
                    excludeRadioButton.Checked = true;
                }

                // Update listBox with Criteria of selected
                criteriaListView.Items.Clear();
                criteriaListView.Columns.Add("Name");
                criteriaListView.Columns[0].Width = criteriaListView.Width - 4;
                criteriaListView.HeaderStyle = ColumnHeaderStyle.None;

                foreach (EA.Element child in element.Elements.Cast<EA.Element>().Where(t => R2Const.ST_CRITERION.Equals(t.Stereotype)))
                {
                    criteriaListView.Items.Add(createCriteriaListViewItem(child));
                }
                groupBox2.Show();
                ignoreEvent = false;
            }
            Cursor = Cursors.Default;
        }

        private void updateSelectedListViewItem()
        {
            if (!ignoreEvent && mainListView.SelectedItems.Count > 0)
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
            if (includeRadioButton.Checked)
            {
                setCompilerInstruction(dl, R2Const.Qualifier.None);
            }
            // Deprecate
            else if (deprecateRadioButton.Checked)
            {
                setCompilerInstruction(dl, R2Const.Qualifier.Deprecate);
            }
            // Delete
            else if (deleteRadioButton.Checked)
            {
                setCompilerInstruction(dl, R2Const.Qualifier.Delete);
            }
            // Exclude
            else if (excludeRadioButton.Checked)
            {
                deleteCompilerInstruction(dl);
            }
        }

        private void updateListViewItem(ListViewItem item)
        {
            bool _ignoreEvent = ignoreEvent;
            ignoreEvent = true;
            DefinitionLink dl = (DefinitionLink)item.Tag;
            if (dl.compilerInstructionElement != null)
            {
                switch (EAHelper.GetTaggedValue(dl.compilerInstructionElement, R2Const.TV_QUALIFIER, ""))
                {
                    case "DEP":
                        item.ForeColor = Color.White;
                        item.BackColor = BACKCOLOR_DEPRECATED;
                        item.Checked = true;
                        break;
                    case "D":
                        item.ForeColor = Color.White;
                        item.BackColor = BACKCOLOR_DELETED;
                        item.Checked = false;
                        break;
                    case "EXCLUDE": // special for excluded criteria; excluded is kind of deleted
                        item.ForeColor = Color.LightGray;
                        item.BackColor = BACKCOLOR_EXCLUDED;
                        item.Checked = false;
                        break;
                    default: // ""
                        item.ForeColor = Color.White;
                        item.BackColor = BACKCOLOR_INCLUDED;
                        item.Checked = true;
                        break;
                }
            }
            else
            {
                item.ForeColor = Color.Black;
                item.BackColor = BACKCOLOR_EXCLUDED;
                item.Checked = false;
            }
            ignoreEvent = _ignoreEvent;
        }

        private void setCompilerInstruction(DefinitionLink dl, R2Const.Qualifier qualifier, string change_note = null)
        {
            // If there is no Compiler Instruction, create one
            if (dl.compilerInstructionElement == null)
            {
                dl.compilerInstructionElement = (EA.Element)profileDefinitionPackage.Elements.AddNew(dl.baseModelElement.Name, "Class");
                dl.compilerInstructionElement.Stereotype = R2Const.ST_COMPILERINSTRUCTION;
                dl.compilerInstructionElement.Update();
                EA.Connector con = (EA.Connector)dl.compilerInstructionElement.Connectors.AddNew("", "Generalization");
                con.SupplierID = dl.baseModelElement.ElementID;
                con.Update();
                dl.compilerInstructionElement.Connectors.Refresh();
                dl.modelElement = R2ModelV2.EA_API.Factory.Create(repository, dl.compilerInstructionElement);
                profileDefinitionPackage.Elements.Refresh();
            }

            switch (qualifier)
            {
                case R2Const.Qualifier.Deprecate:
                    EAHelper.SetTaggedValue(dl.compilerInstructionElement, R2Const.TV_QUALIFIER, "DEP");
                    break;
                case R2Const.Qualifier.Delete:
                    EAHelper.SetTaggedValue(dl.compilerInstructionElement, R2Const.TV_QUALIFIER, "D");
                    break;
                case R2Const.Qualifier.Exclude:
                    EAHelper.SetTaggedValue(dl.compilerInstructionElement, R2Const.TV_QUALIFIER, "EXCLUDE");
                    break;
                case R2Const.Qualifier.None:
                default:
                    EAHelper.SetTaggedValue(dl.compilerInstructionElement, R2Const.TV_QUALIFIER, "");
                    break;
            }

            if (!string.IsNullOrEmpty(change_note))
            {
                EAHelper.SetTaggedValue(dl.compilerInstructionElement, R2Const.TV_CHANGENOTE, "<memo>", change_note);
            }
            else
            {
                EAHelper.DeleteTaggedValue(dl.compilerInstructionElement, R2Const.TV_CHANGENOTE);
            }

            dl.compilerInstructionElement.TaggedValues.Refresh();
            dl.compilerInstructionElement.Refresh();
        }

        private void deleteCompilerInstruction(DefinitionLink dl)
        {
            // Only delete if there is a Compiler Instruction
            if (dl.compilerInstructionElement != null)
            {
                Cursor = Cursors.WaitCursor;
                deleteElementRecurse(profileDefinitionPackage.Elements, dl.compilerInstructionElement.ElementID);
                dl.compilerInstructionElement = null;
                dl.modelElement = R2ModelV2.EA_API.Factory.Create(repository, dl.baseModelElement);
                Cursor = Cursors.Default;
            }
        }

        private void deleteElementRecurse(EA.Collection Elements, int ElementID)
        {
            for (short index = 0; index < Elements.Count; index++)
            {
                EA.Element _element = (EA.Element)Elements.GetAt(index);
                if (_element.ElementID == ElementID)
                {
                    Elements.Delete(index);
                    Elements.Refresh();
                    break;
                }
                deleteElementRecurse(_element.Elements, ElementID);
            }
        }

        private void includeRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            updateSelectedListViewItem();
        }

        private void deprecateRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            updateSelectedListViewItem();
        }

        private void deleteRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            updateSelectedListViewItem();
        }

        private void excludeRadioButton_CheckedChanged(object sender, EventArgs e)
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
                    tvChangeNote = (EA.TaggedValue)ciElement.TaggedValues.GetByName(R2Const.TV_CHANGENOTE);
                }
                ignoreEvent = true;
                if (tvChangeNote != null)
                {
                    textBox1.Text = tvChangeNote.Notes;
                }
                else
                {
                    textBox1.Text = "";
                }
                ignoreEvent = false;
                groupBox3.Show();
            }
            else
            {
                groupBox3.Hide();
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (!ignoreEvent)
            {
                ListViewItem selected = criteriaListView.SelectedItems[0];
                DefinitionLink dl = (DefinitionLink) selected.Tag;
                setCompilerInstruction(dl, R2Const.Qualifier.None, textBox1.Text);
            }
        }

        private void criteriaListView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
/*            ListViewItem selected = criteriaListView.SelectedItems[0];
            DefinitionLink dl = (DefinitionLink)selected.Tag;

            // make sure CI is created first
            //setCompilerInstruction(dl, R2Const.Qualifier.None, textBox1.Text);

            R2Criterion criterion = (R2Criterion)dl.modelElement;
            new CriterionForm().Show(criterion);

            // Update Criterion Text in Critaria List
            selected.Text = string.Format("{0} {1}", criterion.Name, criterion.Text);

            updateListViewItem(selected);*/
        }

        private void mainListView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListViewItem selected = mainListView.SelectedItems[0];
            DefinitionLink dl = (DefinitionLink) selected.Tag;
            if (dl.modelElement is R2Section)
            {
                new SectionForm().Show((R2Section)dl.modelElement);
            }
            else
            {
                new FunctionForm().Show((R2Function)dl.modelElement);
            }
        }

        private void findButton_Click(object sender, EventArgs e)
        {
            string id = findTextBox.Text.ToUpper();
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
            MessageBox.Show(string.Format("There is no element with ID '{0}'", id), "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void findTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                findButton_Click(sender, e);
            }
        }

        private void groupCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (groupCheckBox.Checked)
            {
                mainListView.ShowGroups = true;
                mainListView.View = View.Tile;
            }
            else
            {
                mainListView.ShowGroups = false;
                mainListView.View = View.List;
            }
        }

        private void criteriaListView_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (!ignoreEvent)
            {
                ListViewItem selected = e.Item;
                DefinitionLink dl = (DefinitionLink)selected.Tag;
                if (!selected.Checked)
                {
                    setCompilerInstruction(dl, R2Const.Qualifier.Exclude);
                }
                else
                {
                    // Tri-state; back to normal
                    if (dl.compilerInstructionElement != null)
                    {
                        ignoreEvent = true;
                        selected.Checked = false;
                        deleteCompilerInstruction(dl);
                        ignoreEvent = false;
                    }
                    else
                    {
                        setCompilerInstruction(dl, R2Const.Qualifier.None);
                    }
                }
                updateListViewItem(selected);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (criteriaListView.SelectedItems.Count > 0)
            {
                DefinitionLink dl = (DefinitionLink) criteriaListView.SelectedItems[0].Tag;
                if (dl.compilerInstructionElement != null)
                {
                    repository.ShowInProjectView(dl.compilerInstructionElement);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (mainListView.SelectedItems.Count > 0)
            {
                DefinitionLink dl = (DefinitionLink) mainListView.SelectedItems[0].Tag;
                if (dl.compilerInstructionElement != null)
                {
                    repository.ShowInProjectView(dl.compilerInstructionElement);
                }
            }
        }
    }

    public class DefinitionLink
    {
        public DefinitionLink(EA.Repository repository, EA.Element element)
        {
            baseModelElement = element;
            compilerInstructionElement = findCompilerInstruction(repository, element);
            if (compilerInstructionElement == null)
            {
                modelElement = R2ModelV2.EA_API.Factory.Create(repository, element);
            }
            else
            {
                modelElement = R2ModelV2.EA_API.Factory.Create(repository, compilerInstructionElement);
            }
        }

        EA.Element findCompilerInstruction(EA.Repository repository, EA.Element element)
        {
            // Find compilerinstruction by looking for the generalization connector that points
            // to an Element with stereotype Compiler Instruction
            foreach (EA.Connector con in element.Connectors.Cast<EA.Connector>().Where(c => "Generalization".Equals(c.Type)))
            {
                EA.Element _element = repository.GetElementByID(con.ClientID);
                if (R2Const.ST_COMPILERINSTRUCTION.Equals(_element.Stereotype))
                {
                    return _element;
                }
            }
            return null;
        }

        public EA.Element baseModelElement;
        public EA.Element compilerInstructionElement;
        public R2ModelElement modelElement;
    }
}
