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
            toolTip1.SetToolTip(button1, "Click to select the current Criterion in the Project Browser.\nNote that if there is a Compiler Instruction that will be selected instead.");
            toolTip1.SetToolTip(button2, "Click to select the current Element in the Project Browser.\nNote that if there is a Compiler Instruction that will be selected instead.");
            toolTip1.SetToolTip(excludeRadioButton, "The current Element (and children) will be excluded.");
            toolTip1.SetToolTip(includeRadioButton, "The current Element (and children) will be included.");
            toolTip1.SetToolTip(deprecateRadioButton, "The current Element (and children) will be included and marked as deprecated.");
            toolTip1.SetToolTip(deleteRadioButton, "The current Element (and children) will be included and marked as deleted.");
            toolTip1.SetToolTip(groupCheckBox, "If checked the Elements will be grouped. If unchecked the Elements will be presented in a list.");

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
                        MessageBox.Show(Main.MESSAGE_PROFILE_DEFINITION, "Complete setup", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
                    MessageBox.Show(Main.MESSAGE_PROFILE_DEFINITION, "Complete setup", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            else
            {
                MessageBox.Show("Select a Base FM Section to Profile.", "INFO", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            item.Tag = new R2ModelElementHolder(repository, package.Element);
            return item;
        }

        private ListViewItem createListViewItem(EA.Element element)
        {
            int criteriaCount = element.Elements.Cast<EA.Element>().Count(t => t.Stereotype.Equals(R2Const.ST_CRITERION));
            ListViewItem item = new ListViewItem(string.Format("{0} ({1})", element.Name, criteriaCount));
            item.Tag = new R2ModelElementHolder(repository, element);
            updateListViewItem(item);
            return item;
        }

        private ListViewItem createCriteriaListViewItem(EA.Element element)
        {
            ListViewItem item = new ListViewItem();
            R2ModelElementHolder dl = new R2ModelElementHolder(repository, element);
            item.Tag = dl;
            if (dl.modelElement != null)
            {
                R2Criterion criterion = (R2Criterion)dl.modelElement;
                item.Text = string.Format("{0} {1}", criterion.Name, criterion.Text);
            }
            else
            {
                item.Text = "Invalid Compiler Instruction";
            }
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
                EA.Element element = ((R2ModelElementHolder) selected.Tag).baseModelEAElement;

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
            R2ModelElementHolder dl = (R2ModelElementHolder)item.Tag;
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
            R2ModelElementHolder dl = (R2ModelElementHolder)item.Tag;
            if (dl.compilerInstructionEAElement != null)
            {
                switch (EAHelper.GetTaggedValue(dl.compilerInstructionEAElement, R2Const.TV_QUALIFIER, ""))
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

        private void setCompilerInstruction(R2ModelElementHolder dl, R2Const.Qualifier qualifier, string change_note = null)
        {
            // If there is no Compiler Instruction, create one
            if (dl.compilerInstructionEAElement == null)
            {
                dl.compilerInstructionEAElement = (EA.Element)profileDefinitionPackage.Elements.AddNew(dl.baseModelEAElement.Name, "Class");
                dl.compilerInstructionEAElement.Stereotype = R2Const.ST_COMPILERINSTRUCTION;
                dl.compilerInstructionEAElement.Update();
                if (dl.baseModelEAElement.Type == "Package")
                {
                    EA.Connector con = (EA.Connector)dl.compilerInstructionEAElement.Connectors.AddNew("", "Dependency");
                    con.Stereotype = "Generalization";
                    con.SupplierID = dl.baseModelEAElement.ElementID;
                    con.Update();
                }
                else
                {
                    EA.Connector con = (EA.Connector)dl.compilerInstructionEAElement.Connectors.AddNew("", "Generalization");
                    con.SupplierID = dl.baseModelEAElement.ElementID;
                    con.Update();
                }
                dl.compilerInstructionEAElement.Connectors.Refresh();
                dl.modelElement = R2ModelV2.EA_API.Factory.Create(repository, dl.compilerInstructionEAElement);
                profileDefinitionPackage.Elements.Refresh();
            }

            switch (qualifier)
            {
                case R2Const.Qualifier.Deprecate:
                    EAHelper.SetTaggedValue(dl.compilerInstructionEAElement, R2Const.TV_QUALIFIER, "DEP");
                    break;
                case R2Const.Qualifier.Delete:
                    EAHelper.SetTaggedValue(dl.compilerInstructionEAElement, R2Const.TV_QUALIFIER, "D");
                    break;
                case R2Const.Qualifier.Exclude:
                    EAHelper.SetTaggedValue(dl.compilerInstructionEAElement, R2Const.TV_QUALIFIER, "EXCLUDE");
                    break;
                case R2Const.Qualifier.None:
                default:
                    EAHelper.SetTaggedValue(dl.compilerInstructionEAElement, R2Const.TV_QUALIFIER, "");
                    break;
            }

            if (!string.IsNullOrEmpty(change_note))
            {
                EAHelper.SetTaggedValue(dl.compilerInstructionEAElement, R2Const.TV_CHANGENOTE, "<memo>", change_note);
            }
            else
            {
                EAHelper.DeleteTaggedValue(dl.compilerInstructionEAElement, R2Const.TV_CHANGENOTE);
            }

            dl.compilerInstructionEAElement.TaggedValues.Refresh();
            dl.compilerInstructionEAElement.Refresh();
        }

        private void deleteCompilerInstruction(R2ModelElementHolder dl)
        {
            // Only delete if there is a Compiler Instruction
            if (dl.compilerInstructionEAElement != null)
            {
                Cursor = Cursors.WaitCursor;
                deleteElementRecurse(profileDefinitionPackage.Elements, dl.compilerInstructionEAElement.ElementID);
                dl.compilerInstructionEAElement = null;
                dl.modelElement = R2ModelV2.EA_API.Factory.Create(repository, dl.baseModelEAElement);
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
                EA.Element ciElement = ((R2ModelElementHolder)selected.Tag).compilerInstructionEAElement;
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
                R2ModelElementHolder dl = (R2ModelElementHolder) selected.Tag;
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
            R2ModelElementHolder dl = (R2ModelElementHolder) selected.Tag;
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
                R2ModelElementHolder dl = (R2ModelElementHolder)item.Tag;
                if (dl.baseModelEAElement.Name.ToUpper().StartsWith(id))
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
                R2ModelElementHolder dl = (R2ModelElementHolder)selected.Tag;
                if (!selected.Checked)
                {
                    setCompilerInstruction(dl, R2Const.Qualifier.Exclude);
                }
                else
                {
                    // Tri-state; back to normal
                    if (dl.compilerInstructionEAElement != null)
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
                R2ModelElementHolder dl = (R2ModelElementHolder) criteriaListView.SelectedItems[0].Tag;
                if (dl.compilerInstructionEAElement != null)
                {
                    repository.ShowInProjectView(dl.compilerInstructionEAElement);
                }
                else if (dl.baseModelEAElement != null)
                {
                    repository.ShowInProjectView(dl.baseModelEAElement);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (mainListView.SelectedItems.Count > 0)
            {
                R2ModelElementHolder dl = (R2ModelElementHolder) mainListView.SelectedItems[0].Tag;
                if (dl.compilerInstructionEAElement != null)
                {
                    repository.ShowInProjectView(dl.compilerInstructionEAElement);
                }
                else if (dl.baseModelEAElement != null)
                {
                    repository.ShowInProjectView(dl.baseModelEAElement);
                }
            }
        }
    }

    public class R2ModelElementHolder
    {
        public EA.Element baseModelEAElement;
        public EA.Element compilerInstructionEAElement;
        public R2ModelElement modelElement;

        public R2ModelElementHolder(EA.Repository repository, EA.Element element)
        {
            baseModelEAElement = element;
            compilerInstructionEAElement = findCompilerInstruction(repository, element);
            if (compilerInstructionEAElement == null)
            {
                modelElement = R2ModelV2.EA_API.Factory.Create(repository, element);
            }
            else
            {
                modelElement = R2ModelV2.EA_API.Factory.Create(repository, compilerInstructionEAElement);
            }
        }

        EA.Element findCompilerInstruction(EA.Repository repository, EA.Element element)
        {
            // Find compilerinstruction by looking for the generalization connector that points
            // to an Element with stereotype "CI" (Compiler Instruction)
            int genCount = element.Connectors.Cast<EA.Connector>().Count(c => "Generalization".Equals(c.Type)
                && R2Const.ST_COMPILERINSTRUCTION.Equals(repository.GetElementByID(c.ClientID).Stereotype));
            if (genCount > 1)
            {
                MessageBox.Show(string.Format("{0} has {1} Compiler Instructions.\nExpected zero or one(0..1).\nFix this manually.", element.Name, genCount), "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            else if (genCount == 0)
            {
                return null;
            }
            else
            {
                EA.Connector con = element.Connectors.Cast<EA.Connector>().SingleOrDefault(c => "Generalization".Equals(c.Type)
                    && R2Const.ST_COMPILERINSTRUCTION.Equals(repository.GetElementByID(c.ClientID).Stereotype));
                return repository.GetElementByID(con.ClientID);
            }
        }
    }
}
