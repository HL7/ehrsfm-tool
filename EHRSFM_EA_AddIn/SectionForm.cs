using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace R4C_EHRSFM_EA_AddIn
{
    public partial class SectionForm : Form
    {
        public SectionForm()
        {
            InitializeComponent();
        }

        private EA.Element element;

        public void Show(EA.Element el, R2Config config)
        {
            element = el;
            Text = "EHR-S FM " + el.Stereotype;

            BackColor = config.getSectionColor(el.Alias, DefaultBackColor);

            // Other properties
            nameTextBox.Text = el.Name;
            idTextBox.Text = el.Alias;

            string notes = el.Notes;
            int overIdx = notes.IndexOf("$OV$");
            int examIdx = notes.IndexOf("$EX$", overIdx + 4);
            int actoIdx = notes.IndexOf("$AC$", examIdx + 4);
            overviewTextBox.Text = notes.Substring(overIdx + 4, examIdx - overIdx - 4);
            exampleTextBox.Text = notes.Substring(examIdx + 4, actoIdx - examIdx - 4);
            actorsTextBox.Text = notes.Substring(actoIdx + 4);

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
            string notes = string.Format("$OV${0}$EX${1}$AC${2}", overviewTextBox.Text, exampleTextBox.Text, actorsTextBox.Text);
            element.Notes = notes;
            element.Update();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
