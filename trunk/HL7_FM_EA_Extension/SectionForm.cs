﻿using System;
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

        private R2Section _section;

        public void Show(R2Section section)
        {
            _section = section;

            Text = string.Format("EHR-S FM Section: {0} @{1}", _section.Name, _section.LastModified);
            BackColor = R2Config.config.getSectionColor(_section.SectionId, DefaultBackColor);

            // Other properties
            nameTextBox.Text = _section.Name;
            idTextBox.Text = _section.SectionId;
            overviewTextBox.Text = _section.Overview;
            exampleTextBox.Text = _section.Example;
            actorsTextBox.Text = _section.Actors;

            // TODO: Add "depends on" and "needed by" compartments

            bool enable = !section.IsCompilerInstruction;
            nameTextBox.Enabled = enable;
            idTextBox.Enabled = enable;
            overviewTextBox.Enabled = enable;
            exampleTextBox.Enabled = enable;
            actorsTextBox.Enabled = enable;
            ShowDialog();
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

        private void nameTextBox_TextChanged(object sender, EventArgs e)
        {
            string id = "";
            int spidx = nameTextBox.Text.IndexOf(' ');
            if (spidx != -1)
            {
                id = nameTextBox.Text.Substring(0, spidx);
            }
            idTextBox.Text = id;
        }
    }
}