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
    public partial class SectionForm : Form
    {
        public SectionForm()
        {
            InitializeComponent();
        }

        private R2Section _section;

        public void Show(R2Section section, R2Config config)
        {
            _section = section;

            Text = string.Format("EHR-S FM Section: {0}", _section.Name);
            BackColor = config.getSectionColor(_section.SectionID, DefaultBackColor);

            // Other properties
            nameTextBox.Text = _section.Name;
            idTextBox.Text = _section.SectionID;
            overviewTextBox.Text = _section.Overview;
            exampleTextBox.Text = _section.Example;
            actorsTextBox.Text = _section.Actors;

            // TODO: Add "depends on" and "needed by" compartments

            bool enable = !(section is CompilerInstruction);
            nameTextBox.Enabled = enable;
            idTextBox.Enabled = enable;
            overviewTextBox.Enabled = enable;
            exampleTextBox.Enabled = enable;
            actorsTextBox.Enabled = enable;
            ShowDialog();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            Close();
            _section.Name = nameTextBox.Text;
            _section.SectionID = idTextBox.Text;
            _section.Overview = overviewTextBox.Text;
            _section.Example = exampleTextBox.Text;
            _section.Actors = actorsTextBox.Text;
            _section.Update();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
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
    }
}
