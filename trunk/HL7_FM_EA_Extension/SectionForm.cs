using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HL7_FM_EA_Extension.R2ModelV2.Base;

namespace HL7_FM_EA_Extension
{
    public partial class SectionForm : Form
    {
        public SectionForm()
        {
            InitializeComponent();
        }

        private bool ignoreEvents = false;
        private R2Section _section;

        public void Show(R2Section section)
        {
            _section = section;
            BackColor = R2Config.config.getSectionColor(_section.Name, DefaultBackColor);
            setShowingSection(section, true);
            switchLinkLabel.Visible = section.HasBaseElement;
            ShowDialog();
        }

        public void setShowingSection(R2Section section, bool enableEdit)
        {
            ignoreEvents = true;

            lockIcon.Visible = section.IsReadOnly;
            if (section.IsReadOnly)
            {
                enableEdit = false;
            }
            okButton.Enabled = enableEdit;
            applyButton.Enabled = enableEdit;
            idTextBox.Enabled = enableEdit;
            nameTextBox.Enabled = enableEdit;
            overviewTextBox.Enabled = enableEdit;
            exampleTextBox.Enabled = enableEdit;
            actorsTextBox.Enabled = enableEdit;

            // Other properties
            idTextBox.Text = section.SectionId;
            nameTextBox.Text = section.Name;
            overviewTextBox.Text = section.Overview;
            exampleTextBox.Text = section.Example;
            actorsTextBox.Text = section.Actors;

            // TODO: Add "depends on" and "needed by" compartments

            if (section.HasBaseElement)
            {
                //textLinkLabel.Enabled = false;
                //textTextBox.Visible = true;

                Text = string.Format("EHR-S FM Section: {0} (Profile Definition) @{1}", section.Name, section.LastModified);
                switchLinkLabel.Text = "Switch to base Element";

                updateLabels();
            }
            else
            {
                //textTextBox.Visible = true;
                //textLinkLabel.Enabled = false;

                Text = string.Format("EHR-S FM Section: {0} @{1}", section.Name, section.LastModified);
                switchLinkLabel.Text = "Back to Profile Definition Element";

                updateLabels(false);
            }

            ignoreEvents = false;
        }

        /**
         * Append star(*) after label of Label is value is different from Default
         */
        private void updateLabel(string key, string labelText, Label label, bool? star)
        {
            if (star == null)
            {
                if (!_section.isDefault(key)) labelText = string.Format("{0}*", labelText);
            }
            else
            {
                if (star == true) labelText = string.Format("{0}*", labelText);
            }
            label.Text = labelText;
        }

        /**
         * Update labels for all editable elements
         */
        private void updateLabels(bool? star = null)
        {
            updateLabel(R2Const.AT_NAME, "Name", nameLabel, star);
            updateLabel(R2Const.AT_OVERVIEW, "Overview", overviewLabel, star);
            updateLabel(R2Const.AT_EXAMPLE, "Example", exampleLabel, star);
            updateLabel(R2Const.AT_ACTORS, "Actors", actorsLabel, star);
        }

        private void applyChanges()
        {
            _section.Name = nameTextBox.Text;
            _section.SectionId = idTextBox.Text;
            _section.Overview = overviewTextBox.Text;
            _section.Example = exampleTextBox.Text;
            _section.Actors = actorsTextBox.Text;
            _section.SaveToSource();
        }

        private void applyButton_Click(object sender, EventArgs e)
        {
            applyChanges();
            updateLabels();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            Close();
            applyChanges();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                Close();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void idTextBox_TextChanged(object sender, EventArgs e)
        {
            BackColor = R2Config.config.getSectionColor(idTextBox.Text, DefaultBackColor);
        }

        private bool switched = false;
        private void switchLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (!switched)
            {
                switched = true;
                R2Section baseSection = (R2Section)_section.BaseElement;
                // Disable edit of base to prevent accidental changes
                setShowingSection(baseSection, false);
            }
            else
            {
                switched = false;
                setShowingSection(_section, true);
            }
        }
    }
}
