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
            setShowingFunction(function, true);
            switchLinkLabel.Visible = (function is CompilerInstruction);
            ShowDialog();
        }

        private void setShowingFunction(R2Function function, bool enableEdit)
        {
            if (function is CompilerInstruction)
            {
                Text = string.Format("EHR-S FM {0}: {1} (Profile Definition) @{2}", function.Stereotype, function.Name, function.LastModified);
                switchLinkLabel.Text = "Switch to base Element";
                label3.Visible = true;
                priorityComboBox.SelectedItem = ((R2FunctionCI)function).Priority;
                priorityComboBox.Visible = true;
            }
            else
            {
                Text = string.Format("EHR-S FM {0}: {1} @{2}", _function.Stereotype, function.Name, function.LastModified);
                switchLinkLabel.Text = "Back to Profile Definition Element";
                label3.Visible = false;
                priorityComboBox.SelectedItem = R2FunctionCI.EmptyPriority;
                priorityComboBox.Visible = false;
            }

            BackColor = R2Config.config.getSectionColor(_function.Name, DefaultBackColor);
            pathLabel.Text = function.Path;

            // Other properties
            nameTextBox.Text = _function.Name;
            idTextBox.Text = _function.FunctionID;
            statementTextBox.Text = _function.Statement;
            descriptionTextBox.Text = _function.Description;
            exampleTextBox.Text = _function.Example;

            if (enableEdit)
            {
                if (function is CompilerInstruction)
                {
                    idTextBox.Enabled = false;
                    bool isRealm = "Realm".Equals(((R2FunctionCI) function).ProfileType);
                    nameTextBox.Enabled = isRealm;
                    statementTextBox.Enabled = isRealm;
                    descriptionTextBox.Enabled = true;
                    exampleTextBox.Enabled = true;
                }
                else
                {
                    nameTextBox.Enabled = true;
                    idTextBox.Enabled = true;
                    statementTextBox.Enabled = true;
                    descriptionTextBox.Enabled = true;
                    exampleTextBox.Enabled = true;
                }
            }
            else
            {
                nameTextBox.Enabled = false;
                idTextBox.Enabled = false;
                statementTextBox.Enabled = false;
                descriptionTextBox.Enabled = false;
                exampleTextBox.Enabled = false;
            }
        }

        private void applyChanges()
        {
            _function.FunctionID = idTextBox.Text;
            _function.Name = nameTextBox.Text;
            _function.Statement = statementTextBox.Text;
            _function.Description = descriptionTextBox.Text;
            _function.Example = exampleTextBox.Text;
            _function.Update();

            if (_function is CompilerInstruction)
            {
                ((R2FunctionCI)_function).Priority = priorityComboBox.Text;
            }
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

        private bool switched = false;
        private void switchLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (!switched)
            {
                switched = true;
                R2Function baseFunction = (R2Function)R2Model.GetBase(_function);
                // Disable edit of base to prevent accidental changes
                setShowingFunction(baseFunction, false);
            }
            else
            {
                switched = false;
                setShowingFunction(_function, true);
            }
        }
    }
}
