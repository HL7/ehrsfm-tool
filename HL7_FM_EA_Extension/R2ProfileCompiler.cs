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

        private const string CHANGEINDICATOR_NOTCHANGED = "NC";
        private const string CHANGEINDICATOR_MODIFIED = "M";
        private const string CHANGEINDICATOR_DELETED = "D";
        private const string CHANGEINDICATOR_DEPRECATED = "DEP";

        private const string QUALIFIER_DELETE = "D";
        private const string QUALIFIER_DEPRECATE = "DEP";

        private const string PRIORITY_ESSENTIALNOW = "EN";

        public void Compile(string fileNameBase, string fileNameDefinition, string fileNameOutput)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ModelType));
            StreamReader streamBase = new StreamReader(fileNameBase);
            StreamReader streamDef = new StreamReader(fileNameDefinition);
            using (streamBase)
            {
                var modelBM = (ModelType)serializer.Deserialize(streamBase);
                TreeNode root = MAXTreeNodeUtils.ToTree(modelBM, treeNodes);
                baseName = root.metadata.name;

                // Add compiler instructions to the nodes
                using (streamDef)
                {
                    ModelType modelCI = (ModelType) serializer.Deserialize(streamDef);
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

                    ObjectType profileDefinition = objectsCI.Values.Single(o => R2Const.ST_FM_PROFILEDEFINITION.Equals(o.stereotype));
                    // TODO: Copy Profile Metadata from the Definition in the empty root
                    string rootid = root.metadata.id;
                    root.metadata = new ObjectType()
                    {
                        id = rootid,
                        name = profileDefinition.name,
                        stereotype = R2Const.ST_FM_PROFILE,
                        type = ObjectTypeEnum.Package,
                        tag = profileDefinition.tag,
                        typeSpecified = true
                    };
                }

                // Create imports from consequenceLinks
                // This will mark all linked nodes to import=true if a compiler instruction references a node with a consequenceLink
                // Only do this for ProfileType != Companion
                bool doFollowConsequenceLinks = true;
                if (root.metadata.tag.Any(t => "Type".Equals(t.name)))
                {
                    TagType typeTag = root.metadata.tag.Single(t => "Type".Equals(t.name));
                    doFollowConsequenceLinks = !"Companion".Equals(typeTag.value);
                }
                if (doFollowConsequenceLinks)
                {
                    followConsequenceLinks(root, false);
                }

                // Compile aka execute compiler instructions
                executeInstructions(root, false, PRIORITY_ESSENTIALNOW);

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
                List<RelationshipType> relationships = root.ToRelationshipList();
                foreach (RelationshipType maxRel in relationships)
                {
                    if (idOrg2idNew.ContainsKey(maxRel.sourceId))
                    {
                        maxRel.sourceId = idOrg2idNew[maxRel.sourceId];
                    }
                    if (idOrg2idNew.ContainsKey(maxRel.destId))
                    {
                        maxRel.destId = idOrg2idNew[maxRel.destId];
                    }
                }

                // save compiled profile as MAX XML
                ModelType model = new ModelType();
                model.objects = objects.ToArray();
                model.relationships = relationships.ToArray();
                foreach (RelationshipType maxRel in model.relationships)
                {
                    if (!objects.Any(t => t.id == maxRel.sourceId))
                    {
                        string sourceId = idOrg2idNew.Single(t => t.Value == maxRel.sourceId).Key;
                        Console.WriteLine("relationship from not included object orgSourceId={0} destId={1} stereotype={2}", sourceId, maxRel.destId, maxRel.stereotype);
                    }
                    if (!objects.Any(t => t.id == maxRel.destId))
                    {
                        string sourceId = idOrg2idNew.Single(t => t.Value == maxRel.sourceId).Key;
                        Console.WriteLine("relationship to not included object orgSourceId={0} destId={1} stereotype={2}", sourceId, maxRel.destId, maxRel.stereotype);
                    }
                }
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
            TagType tagChangeIndicator = new TagType() { name = "Reference.ChangeIndicator", value = CHANGEINDICATOR_NOTCHANGED};
            tags.Add(tagChangeIndicator);
            node.metadata.tag = tags.ToArray();

            bool hasInstruction = node.hasInstruction;
            if (hasInstruction)
            {
                // update Optionality for Criterion
                if ("Criteria".Equals(node.metadata.stereotype))
                {
                    TagType tagOptionality = node.metadata.tag.Single(t => t.name == "Optionality");
                    TagType tagOptionalityNew = node.instruction.tag.SingleOrDefault(t => t.name == "Optionality");
                    if (tagOptionalityNew != null && !tagOptionalityNew.value.Equals(tagOptionality.value))
                    {
                        tagChangeIndicator.value = CHANGEINDICATOR_MODIFIED;
                        tagOptionality.value = tagOptionalityNew.value;
                    }
                }

                // update name if changed
                if (!node.metadata.name.Equals(node.instruction.name))
                {
                    tagChangeIndicator.value = CHANGEINDICATOR_MODIFIED;
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
                    if (QUALIFIER_DELETE.Equals(tagQualifier.value))
                    {
                        tagChangeIndicator.value = CHANGEINDICATOR_DELETED;
                        // Make sure children are not included in profile and stop processing
                        node.children.Clear();
                        return true;
                    }
                    else if (QUALIFIER_DEPRECATE.Equals(tagQualifier.value))
                    {
                        tagChangeIndicator.value = CHANGEINDICATOR_DEPRECATED;
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

        /** 
         * This method will follow all ConsequenceLinks and set import to true
         */
        private void followConsequenceLinks(TreeNode node, bool overrideImport)
        {
            if (overrideImport)
            {
                node.import = true;
            }
            foreach(TreeNode child in node.children)
            {
                if (child.hasConsequenceLinks)
                {
                    foreach (RelationshipType consequenceLink in child.consequenceLinks)
                    {
                        TreeNode linkedNode = treeNodes[consequenceLink.destId];
                        followConsequenceLinks(linkedNode, true);
                    }
                }
                else
                {
                    followConsequenceLinks(child, overrideImport);
                }
            }
        }
    }
}
