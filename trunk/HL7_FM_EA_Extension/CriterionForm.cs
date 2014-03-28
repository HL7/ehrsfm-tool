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
    public partial class CriterionForm : Form
    {
        public CriterionForm()
        {
            InitializeComponent();
        }

        private R2Criterion _criterion;

        public void Show(R2Criterion criterion)
        {
            _criterion = criterion;

            BackColor = R2Config.config.getSectionColor(_criterion.Name, DefaultBackColor);
            Text = string.Format("EHR-S FM Criterion: {0}", _criterion.Name);
            pathLabel.Text = criterion.Path;

            idNumericUpDown.Minimum = 1;
            idNumericUpDown.Maximum = 99;
            idNumericUpDown.Value = _criterion.CriterionID;
            textTextBox.Text = _criterion.Text;

            rowNumericUpDown.Minimum = 1;
            rowNumericUpDown.Maximum = 10000;
            rowNumericUpDown.Value = _criterion.Row;

            conditionalCheckBox.Checked = _criterion.Conditional;
            dependentCheckBox.Checked = _criterion.Dependent;
            optionalityComboBox.SelectedItem = _criterion.Optionality;

            ShowDialog();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            Close();

            _criterion.CriterionID = idNumericUpDown.Value;
            _criterion.Text = textTextBox.Text;
            _criterion.Update();

            _criterion.Row = rowNumericUpDown.Value;
            _criterion.Conditional = conditionalCheckBox.Checked;
            _criterion.Dependent = dependentCheckBox.Checked;
            _criterion.Optionality = optionalityComboBox.SelectedItem.ToString();
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

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void optionalityComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string newOptionality = optionalityComboBox.SelectedItem.ToString();
            if (!string.IsNullOrEmpty(newOptionality))
            {
                string newText = textTextBox.Text.Replace("SHALL", newOptionality);
                newText = newText.Replace("SHOULD", newOptionality);
                newText = newText.Replace("MAY", newOptionality);
                textTextBox.Text = newText;
            }
        }
    }
}
