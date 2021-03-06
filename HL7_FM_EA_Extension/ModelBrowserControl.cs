﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml.XPath;

namespace HL7_FM_EA_Extension
{
    public partial class ModelBrowserControl : UserControl
    {
        public ModelBrowserControl()
        {
            InitializeComponent();
        }

        EA.Repository Repository;
        public void SetRepository(EA.Repository Repository)
        {
            this.Repository = Repository;
            SetSelectedPackage(Repository.GetTreeSelectedPackage());
        }

        public void SetSelectedPackage(EA.Package SelectedPackage)
        {
            if (SelectedPackage != null)
            {
                // update contents treeView1
                treeView1.Nodes.Clear();

                System.Windows.Forms.TreeNode topNode = createTreeNode(SelectedPackage.Element.ElementID, SelectedPackage.Element.Stereotype, SelectedPackage.Name, SelectedPackage.Alias);
                treeView1.Nodes.Add(topNode);
                visitPackage(SelectedPackage, topNode);
                treeView1.ExpandAll();
            }
            else
            {
                // clear contents of treeView1
                treeView1.Nodes.Clear();
            }
        }

        private void visitPackage(EA.Package package, System.Windows.Forms.TreeNode treeNode)
        {
            foreach (EA.Package childPackage in package.Packages)
            {
                System.Windows.Forms.TreeNode childTreeNode = createTreeNode(package.Element.ElementID, childPackage.Element.Stereotype, childPackage.Name, childPackage.Alias);
                treeNode.Nodes.Add(childTreeNode);
                visitPackage(childPackage, childTreeNode);
            }
            visitObjects(package.PackageID, treeNode);
        }

        private void visitObjects(int Selected_Package_ID, System.Windows.Forms.TreeNode treeNode)
        {
            // get objects in selected package
            string sql = string.Format("SELECT Object_ID, Object_Type, Stereotype, Name, Alias, ParentID FROM t_object WHERE Package_ID={0} AND Object_Type<>'Package' ORDER BY Object_ID", Selected_Package_ID);
            string xml = Repository.SQLQuery(sql);
            XElement xEADATA = XElement.Parse(xml, LoadOptions.None);

            Dictionary<int, System.Windows.Forms.TreeNode> nodes = new Dictionary<int, System.Windows.Forms.TreeNode>();
            IEnumerable<XElement> xRows = xEADATA.XPathSelectElements("//Data/Row");
            foreach (XElement xRow in xRows)
            {
                string EA_Object_Type = xRow.Element("Object_Type").Value;
                // Ignore Text and Note Diagram type objects
                if (!"Text".Equals(EA_Object_Type) && !"Boundary".Equals(EA_Object_Type) && !"Note".Equals(EA_Object_Type))
                {
                    int EA_Object_ID = int.Parse(xRow.Element("Object_ID").Value);
                    XElement xStereotype = xRow.Element("Stereotype");
                    string stereotype = null;
                    if (xStereotype != null)
                    {
                        stereotype = xStereotype.Value;
                    }
                    XElement xName = xRow.Element("Name"); // Even name can be empty
                    string name = "";
                    if (xName != null)
                    {
                        name = xName.Value;
                    }
                    XElement xAlias = xRow.Element("Alias");
                    string alias = "";
                    if (xAlias != null)
                    {
                        alias = xAlias.Value;
                    }
                    int EA_ParentID = int.Parse(xRow.Element("ParentID").Value);
                    System.Windows.Forms.TreeNode childTreeNode = createTreeNode(EA_Object_ID, stereotype, name, alias);
                    nodes[EA_Object_ID] = childTreeNode;
                    if (nodes.ContainsKey(EA_ParentID))
                    {
                        nodes[EA_ParentID].Nodes.Add(childTreeNode);
                    }
                    else
                    {
                        treeNode.Nodes.Add(childTreeNode);
                    }
                }
            }
        }

        private System.Windows.Forms.TreeNode createTreeNode(int ID, string stereotype, string name, string alias)
        {
            System.Windows.Forms.TreeNode treeNode;
            if (string.IsNullOrEmpty(stereotype))
            {
                treeNode = new System.Windows.Forms.TreeNode(name);
            }
            else
            {
                treeNode = new System.Windows.Forms.TreeNode(string.Format("«{0}» {1}", stereotype, name));
            }
            string extID = alias;
            if (string.IsNullOrEmpty(alias))
            {
                extID = name;
            }
            treeNode.BackColor = R2Config.config.getSectionColor(extID, DefaultBackColor);
            treeNode.Tag = ID;
            return treeNode;
        }

        private void treeView1_DoubleClick(object sender, EventArgs e)
        {
            System.Windows.Forms.TreeNode selectedNode = treeView1.SelectedNode;
            Repository.ShowInProjectView(Repository.GetElementByID((int)selectedNode.Tag));
        }
    }
}
