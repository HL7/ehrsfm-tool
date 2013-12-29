using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MAX_EA.MAXSchema;

namespace HL7_FM_EA_Extension
{
    public class MAXTreeNodeUtils
    {
        /*
         * Converts a flat MAX model to a tree.
         * Will also fill a id to treeNode Dictionary.
         */
        public static TreeNode ToTree(ModelType model, Dictionary<string, TreeNode> treeNodes)
        {
            foreach (ObjectType maxObj in model.objects)
            {
                treeNodes[maxObj.id] = new TreeNode();
                treeNodes[maxObj.id].metadata = maxObj;
            }

            // Now create tree
            TreeNode root = null;
            foreach (ObjectType maxObj in model.objects)
            {
                // This is a root element when it has no parent
                if (string.IsNullOrEmpty(maxObj.parentId))
                {
                    // We expect only 1 root which is the HL7-FM or HL7-FM-Profile
                    root = treeNodes[maxObj.id];
                }
                else if (treeNodes.ContainsKey(maxObj.parentId))
                {
                    TreeNode node = treeNodes[maxObj.id];
                    TreeNode parent = treeNodes[maxObj.parentId];
                    parent.children.Add(node);
                    node.parent = parent;
                }
                else
                {
                    Console.WriteLine("Warning: parent not found");
                }
            }
            return root;
        }
    }

    public class TreeNode
    {
        public ObjectType metadata;
        public TreeNode parent;
        public ObjectType instruction;
        public RelationshipType consequenceLink;
        public bool import = false;
        public List<TreeNode> children = new List<TreeNode>();

        /*
         * Convert a tree back to a list of MAX Objects starting with some node
         * and following its children recursively.
         */
        private void ToObjectList(TreeNode node, List<ObjectType> objects)
        {
            if (node.metadata != null)
            {
                objects.Add(node.metadata);
            }
            foreach (TreeNode child in node.children)
            {
                ToObjectList(child, objects);
            }
        }

        public List<ObjectType> ToObjectList()
        {
            List<ObjectType> objects = new List<ObjectType>();
            ToObjectList(this, objects);
            return objects;
        }
    }
}
