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
    public partial class FunctionForm : Form
    {
        public FunctionForm()
        {
            InitializeComponent();
        }

        private R2Function _function;

        public void Show(R2Function function)
        {
            _function = function;

            if (function is CompilerInstruction)
            {
                Text = string.Format("EHR-S FM {0}: {1} (Profile Definition)", _function.Stereotype, _function.Name);
            }
            else
            {
                Text = string.Format("EHR-S FM {0}: {1}", _function.Stereotype, _function.Name);
            }

            BackColor = R2Config.config.getSectionColor(_function.Name, DefaultBackColor);
            pathLabel.Text = function.Path;

            // Other properties
            nameTextBox.Text = _function.Name;
            idTextBox.Text = _function.FunctionID;
            statementTextBox.Text = _function.Statement;
            descriptionTextBox.Text = _function.Description;
            exampleTextBox.Text = _function.Example;

            // TODO: Add ConsequenceLinks (& See Also?) "depends on" and "needed by" compartments

            bool enable = !(function is CompilerInstruction);
            nameTextBox.Enabled = enable;
            idTextBox.Enabled = enable;
            statementTextBox.Enabled = enable;
            descriptionTextBox.Enabled = enable;
            exampleTextBox.Enabled = enable;
            ShowDialog();
        }

        private void applyChanges()
        {
            _function.Name = nameTextBox.Text;
            _function.FunctionID = idTextBox.Text;
            _function.Statement = statementTextBox.Text;
            _function.Description = descriptionTextBox.Text;
            _function.Example = exampleTextBox.Text;
            _function.Update();
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
                this.Close();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
