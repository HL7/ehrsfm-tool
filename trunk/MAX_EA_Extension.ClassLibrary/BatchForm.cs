using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MAX_EA_Extension
{
    public partial class BatchForm : Form
    {
        public BatchForm()
        {
            InitializeComponent();
        }

        public void setContent(Dictionary<string, EA.Package> content)
        {
            availableListBox.Items.Clear();
            selectedListBox.Items.Clear();
            foreach (string name in content.Keys)
            {
                availableListBox.Items.Add(name);
            }
            availableListBox.Refresh();
            selectedListBox.Refresh();
        }

        private void selectButton_Click(object sender, EventArgs e)
        {
            List<string> toRemove = new List<String>();
            foreach (string item in availableListBox.SelectedItems)
            {
                toRemove.Add(item);
                selectedListBox.Items.Add(item);
            }
            foreach (string item in toRemove)
            {
                availableListBox.Items.Remove(item);
            }
            availableListBox.Refresh();
            selectedListBox.Refresh();
        }

        private void deselectButton_Click(object sender, EventArgs e)
        {
            List<string> toRemove = new List<String>();
            foreach (string item in selectedListBox.SelectedItems)
            {
                toRemove.Add(item);
                availableListBox.Items.Add(item);
            }
            foreach (string item in toRemove)
            {
                selectedListBox.Items.Remove(item);
            }
            availableListBox.Refresh();
            selectedListBox.Refresh();
        }

        private void selectAllButton_Click(object sender, EventArgs e)
        {
            List<string> toRemove = new List<String>();
            foreach (string item in availableListBox.Items)
            {
                toRemove.Add(item);
                selectedListBox.Items.Add(item);
            }
            foreach (string item in toRemove)
            {
                availableListBox.Items.Remove(item);
            }
            availableListBox.Refresh();
            selectedListBox.Refresh();
        }

        private void deselectAllButton_Click(object sender, EventArgs e)
        {
            List<string> toRemove = new List<String>();
            foreach (string item in selectedListBox.Items)
            {
                toRemove.Add(item);
                availableListBox.Items.Add(item);
            }
            foreach (string item in toRemove)
            {
                selectedListBox.Items.Remove(item);
            }
            availableListBox.Refresh();
            selectedListBox.Refresh();
        }

        bool exportButtonPressed = false;
        public bool isExportButtonPressed()
        {
            return exportButtonPressed;
        }

        public List<String> getSelectedItems()
        {
            List<string> selected = new List<string>();
            foreach (string item in selectedListBox.Items)
            {
                selected.Add(item);
            }
            return selected;
        }

        private void exportButton_Click(object sender, EventArgs e)
        {
            exportButtonPressed = true;
            Close();
        }
    }
}
