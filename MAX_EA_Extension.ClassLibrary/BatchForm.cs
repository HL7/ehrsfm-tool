using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
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
            Dictionary<int,TreeNode> nodes = new Dictionary<int, TreeNode>();

            availableTreeView.Nodes.Clear();
            foreach (string name in content.Keys)
            {
                EA.Package package = content[name];
                if (!nodes.ContainsKey(package.ParentID))
                {
                    nodes[package.PackageID] = availableTreeView.Nodes.Add(name);
                }
                else
                {
                    TreeNode parentNode = nodes[package.ParentID];
                    nodes[package.PackageID] = parentNode.Nodes.Add(name);
                }
            }
            availableTreeView.Refresh();
        }

        bool exportButtonPressed = false;
        public bool isExportButtonPressed()
        {
            return exportButtonPressed;
        }

        public List<string> getSelectedItems()
        {
            List<string> selected = new List<string>();
            foreach (TreeNode node in availableTreeView.Nodes)
            {
                if (node.Checked)
                {
                    selected.Add(node.Text);
                }
            }
            return selected;
        }

        private void exportButton_Click(object sender, EventArgs e)
        {
            exportButtonPressed = true;
            Close();
        }

        private void availableTreeView_AfterCheck(object sender, TreeViewEventArgs e)
        {
        }
    }
}
