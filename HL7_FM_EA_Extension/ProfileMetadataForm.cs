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
            dateTimePicker1.Value = DateTime.Parse(profDef.LastModified);
            comboBox1.SelectedItem = _profDef.Type;
            comboBox2.SelectedItem = _profDef.LanguageTag;
            rationaleTextBox.Text = _profDef.Rationale;
            scopeTextBox.Text = _profDef.Scope;
            prioDefTextBox.Text = _profDef.PrioDef;
            confClauseTextBox.Text = _profDef.ConfClause;
            baseModelTextBox.Text = _profDef.BaseModel;
            ShowDialog();
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

        private void okButton_Click(object sender, EventArgs e)
        {
            Close();
            _profDef.SaveToSource();
        }
    }
}
