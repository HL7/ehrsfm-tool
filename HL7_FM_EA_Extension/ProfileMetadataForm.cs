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
    public partial class ProfileMetadataForm : Form
    {
        public ProfileMetadataForm()
        {
            InitializeComponent();
        }

        private R2ProfileDefinition _profDef;

        public void Show(R2ProfileDefinition profDef)
        {
            _profDef = profDef;
            nameTextBox.Text = _profDef.Name;
            versionTextBox.Text = _profDef.Version;
            if (string.IsNullOrEmpty(_profDef.LastModified))
            {
                dateTimePicker1.Value = DateTime.Now;
            }
            else
            {
                dateTimePicker1.Value = DateTime.Parse(profDef.LastModified);
            }
            comboBox1.Text = _profDef.Type;
            comboBox2.Text = _profDef.LanguageTag;
            rationaleTextBox.Text = _profDef.Rationale;
            scopeTextBox.Text = _profDef.Scope;
            prioDefTextBox.Text = _profDef.PrioDef;
            confClauseTextBox.Text = _profDef.ConfClause;
            baseModelTextBox.Text = _profDef.BaseModelName;
            ShowDialog();
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

        private void applyChanges()
        {
            _profDef.LastModified = Util.FormatLastModified(dateTimePicker1.Value);
            _profDef.Name = nameTextBox.Text;
            _profDef.Type = comboBox1.Text;
            _profDef.LanguageTag = comboBox2.Text;
            _profDef.Rationale = rationaleTextBox.Text;
            _profDef.Scope = scopeTextBox.Text;
            _profDef.PrioDef = prioDefTextBox.Text;
            _profDef.ConfClause = confClauseTextBox.Text;
            _profDef.SaveToSource();
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

        private void applyButton_Click(object sender, EventArgs e)
        {
            applyChanges();
        }
    }
}
