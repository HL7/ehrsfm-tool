using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using MAX_EA.MAXSchema;

namespace HL7_FM_EA_Extension
{
    public class R2ProfileCompiler
    {
        // This contains the citation id of the Base Model where this Profile is based on
        private string baseName = "";
        // This will contain a lookup map
        private Dictionary<string, TreeNode> treeNodes = new Dictionary<string, TreeNode>();

        public void Compile(string fileNameBase, string fileNameInstructions, string fileNameOutput)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ModelType));
            StreamReader streamBase = new StreamReader(fileNameBase);
            StreamReader streamInstr = new StreamReader(fileNameInstructions);
            using (streamBase)
            {
                var modelBM = (ModelType)serializer.Deserialize(streamBase);
                TreeNode root = MAXTreeNodeUtils.ToTree(modelBM, treeNodes);
                baseName = root.metadata.name;

                // Add ConsequenceLinks
                foreach (RelationshipType maxRel in modelBM.relationships)
                {
                    if ("ConsequenceLink".Equals(maxRel.stereotype))
                    {
                        treeNodes[maxRel.sourceId].consequenceLink = maxRel;
                    }
                }

                // Add compiler instructions to the nodes
                using (streamInstr)
                {
                    ModelType modelCI = (ModelType) serializer.Deserialize(streamInstr);
                    var objectsCI = new Dictionary<string, ObjectType>();
                    foreach (ObjectType maxObj in modelCI.objects)
                    {
                        objectsCI[maxObj.id] = maxObj;
                    }
                    
                    // Bind CI to related elements in the base tree
                    foreach (RelationshipType rel in modelCI.relationships)
                    {
                        TreeNode node = treeNodes[rel.destId];
                        node.instruction = objectsCI[rel.sourceId];
                        if ("import".Equals(rel.stereotype))
                        {
                            node.import = true;
                        }
                    }
                }

                // TODO: Put Profile Metadata in the empty root
                string rootid = root.metadata.id;
                root.metadata = new ObjectType() {
                                      id = rootid, 
                                      name = "Compiled Profile", 
                                      stereotype = "HL7-FM-Profile", 
                                      type = ObjectTypeEnum.Package,
                                      typeSpecified = true };

                // Convert consequenceLinks to imports
                // This will mark all linked nodes to import=true if a compiler instruction references a node with a consequenceLink
                processConsequenceLinks(root, false);

                // Compile aka execute compiler instructions
                executeInstructions(root, false, "EN");

                // Print tree to Console
                //displayTree(root);

                // Give profile elements new unique ids
                // Only what is still in the Tree!
                List<ObjectType> objects = root.ToObjectList();
                Dictionary<string,string> idOrg2idNew = new Dictionary<string, string>();
                foreach(ObjectType maxObj in objects)
                {
                    string idOrg = maxObj.id;
                    string idNew = Guid.NewGuid().ToString();
                    idOrg2idNew[idOrg] = idNew;
                    maxObj.id = idNew;
                    // Also remove modified
                    maxObj.modifiedSpecified = false;
                }
                foreach (ObjectType maxObj in objects)
                {
                    if (!string.IsNullOrEmpty(maxObj.parentId))
                    {
                        if (idOrg2idNew.ContainsKey(maxObj.parentId))
                        {
                            maxObj.parentId = idOrg2idNew[maxObj.parentId];
                        }
                        else
                        {
                            Console.WriteLine("already new: {0}", maxObj.name);
                        }
                    }
                }

                // save compiled profile as MAX XML
                ModelType model = new ModelType();
                model.objects = objects.ToArray();
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.NewLineChars = "\n";
                using (XmlWriter writer = XmlWriter.Create(fileNameOutput, settings))
                {
                    serializer.Serialize(writer, model);
                }
            }
        }

        private void displayNode(TreeNode node, int depth)
        {
            List<String> tagsAsString = new List<string>();
            List<TagType> tags = new List<TagType>(node.metadata.tag);
            tags.ForEach(t => tagsAsString.Add(string.Format("{0}={1}", t.name, t.value)));
            if (node.instruction == null)
            {
                Console.WriteLine("[{0}] {1} {2}", depth, node.metadata.name, string.Join(", ", tagsAsString.ToArray()));
            }
            else
            {
                Console.WriteLine("[{0}] <profiled> {1} {2}", depth, node.metadata.name, string.Join(", ", tagsAsString.ToArray()));
            }
            foreach (TreeNode child in node.children)
            {
                displayNode(child, depth+1);
            }
        }

        private bool executeInstructions(TreeNode node, bool overrideImport, string priority)
        {
            overrideImport |= node.import;
            bool hasInstruction = (node.instruction != null);
            List<TagType> tags = new List<TagType>();
            if (node.metadata.tag != null)
            {
                tags.AddRange(node.metadata.tag);
            }
            // remove deprecated row tag
            tags.RemoveAll(t => t.name.Equals("Row"));

            // set Priority
            TagType tagPriority = new TagType() { name = "Priority", value = priority };
            tags.Add(tagPriority);

            // remove old Reference
            tags.RemoveAll(t => t.name.StartsWith("Reference."));
            // update Reference
            if ("Criteria".Equals(node.metadata.stereotype))
            {
                tags.Add(new TagType() { name = "Reference.Alias", value = baseName });
                string functionId = node.metadata.name.Substring(0, node.metadata.name.IndexOf('#'));
                tags.Add(new TagType() { name = "Reference.FunctionID", value = functionId });
                int criterionId = int.Parse(node.metadata.name.Substring(node.metadata.name.IndexOf('#') + 1));
                tags.Add(new TagType() { name = "Reference.CriterionID", value = criterionId.ToString() });
            }
            else if ("Header".Equals(node.metadata.stereotype) || "Function".Equals(node.metadata.stereotype))
            {
                tags.Add(new TagType() { name = "Reference.Alias", value = baseName });
                tags.Add(new TagType() { name = "Reference.FunctionID", value = node.metadata.alias });
            }
            else if ("Section".Equals(node.metadata.stereotype))
            {
                tags.Add(new TagType() { name = "Reference.Alias", value = baseName });
                tags.Add(new TagType() { name = "Reference.SectionID", value = node.metadata.alias });
            }

            // default set ChangeIndicator to "NC"; changed when executing instruction
            TagType tagChangeIndicator = new TagType() {name = "Reference.ChangeIndicator", value = "NC"};
            tags.Add(tagChangeIndicator);
            node.metadata.tag = tags.ToArray();

            if (hasInstruction)
            {
                // update Optionality for Criterion
                if ("Criteria".Equals(node.metadata.stereotype))
                {
                    TagType tagOptionality = node.metadata.tag.Single(t => t.name == "Optionality");
                    TagType tagOptionalityNew = node.instruction.tag.SingleOrDefault(t => t.name == "Optionality");
                    if (tagOptionalityNew != null && !tagOptionalityNew.value.Equals(tagOptionality.value))
                    {
                        tagChangeIndicator.value = "M";
                        tagOptionality.value = tagOptionalityNew.value;
                    }
                }

                // update name if changed
                if (!node.metadata.name.Equals(node.instruction.name))
                {
                    tagChangeIndicator.value = "M";
                    node.metadata.name = node.instruction.name;
                }

                // override Priority
                TagType tagPriorityNew = node.instruction.tag.SingleOrDefault(t => t.name == "Priority");
                if (tagPriorityNew != null)
                {
                    priority = tagPriorityNew.value;
                }
                tagPriority.value = priority;

                // do Qualifier stuff. Delete or Deprecate
                TagType tagQualifier = node.instruction.tag.SingleOrDefault(t => t.name == "Qualifier");
                if (tagQualifier != null)
                {
                    if ("D".Equals(tagQualifier.value))
                    {
                        tagChangeIndicator.value = "D";
                        // Make sure children are not included in profile and stop processing
                        node.children.Clear();
                        return true;
                    }
                    else if ("DEP".Equals(tagQualifier.value))
                    {
                        tagChangeIndicator.value = "DEP";
                    }
                }

                // Also import children when this CompilerInstruction pertains a Header or Function
                if ("Header".Equals(node.metadata.stereotype) ||
                    "Function".Equals(node.metadata.stereotype))
                {
                    overrideImport = true;
                    // TODO: overrideChangeIndicator to DEP or D
                }
            }

            List<TreeNode> newChildren = new List<TreeNode>();
            foreach(TreeNode child in node.children)
            {
                bool childHasInstruction = executeInstructions(child, overrideImport, priority);
                if (overrideImport || childHasInstruction)
                {
                    newChildren.Add(child);
                    hasInstruction = true;
                }
            }
            node.children = newChildren;
            return hasInstruction;
        }

        private void processConsequenceLinks(TreeNode node, bool overrideImport)
        {
            if (overrideImport)
            {
                node.import = true;
            }
            foreach(TreeNode child in node.children)
            {
                if (child.consequenceLink != null)
                {
                    TreeNode linkedNode = treeNodes[child.consequenceLink.destId];
                    processConsequenceLinks(linkedNode, true);
                }
                else
                {
                    processConsequenceLinks(child, overrideImport);
                }
            }
        }
    }
}
