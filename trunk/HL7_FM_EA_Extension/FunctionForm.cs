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

        private EA.Element element;

        public void Show(EA.Element el, R2Config config)
        {
            element = el;
            Text = "EHR-S FM " + el.Stereotype;

            BackColor = config.getSectionColor(el.Name, DefaultBackColor);
            sectionLabel.Text = config.getSectionTitle(el.Name);

            // Other properties
            nameTextBox.Text = el.Name;
            idTextBox.Text = el.Alias;

            string notes = el.Notes;
            int stmtIdx = notes.IndexOf("$ST$");
            int descIdx = notes.IndexOf("$DE$", stmtIdx + 4);
            int examIdx = notes.IndexOf("$EX$", descIdx + 4);
            statementTextBox.Text = notes.Substring(stmtIdx + 4, descIdx - stmtIdx - 4);
            descriptionTextBox.Text = notes.Substring(descIdx + 4, examIdx - descIdx - 4);
            exampleTextBox.Text = notes.Substring(examIdx + 4);

            // TODO: Add "depends on" and "needed by" compartments

            ShowDialog();
        }

        private void updateTaggedValue(string name, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                EA.TaggedValue tv = (EA.TaggedValue)element.TaggedValues.GetByName(name);
                if (tv == null)
                {
                    tv = (EA.TaggedValue)element.TaggedValues.AddNew(name, "TaggedValue");
                }
                tv.Value = value;
                tv.Update();
            }
        }

        private string getTaggedValue(string name)
        {
            EA.TaggedValue tv = (EA.TaggedValue)element.TaggedValues.GetByName(name);
            if (tv != null)
            {
                return tv.Value;
            }
            else
            {
                return "";
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            Close();
            element.Name = nameTextBox.Text;
            element.Alias = idTextBox.Text;
            string notes = string.Format("$ST${0}$DE${1}$EX${2}", statementTextBox.Text, descriptionTextBox.Text, exampleTextBox.Text);
            element.Notes = notes;
            element.Update();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
