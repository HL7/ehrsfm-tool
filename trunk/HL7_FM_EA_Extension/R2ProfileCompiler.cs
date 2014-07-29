using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using HL7_FM_EA_Extension.R2ModelV2.Base;
using MAX_EA.MAXSchema;

namespace HL7_FM_EA_Extension
{
    public class R2ProfileCompiler
    {
        // This contains the citation id of the Base Model where this Profile is based on
        private string baseName = "";
        // This will contain a lookup map
        private Dictionary<string, TreeNode> treeNodes = new Dictionary<string, TreeNode>();

        private const string CHANGEINDICATOR_NOCHANGE = "NC";
        private const string CHANGEINDICATOR_CHANGED = "C";
        private const string CHANGEINDICATOR_DELETED = "D";
        private const string CHANGEINDICATOR_DEPRECATED = "DEP";
        private const string CHANGEINDICATOR_NEW = "N";

        private const string QUALIFIER_DELETE = "D";
        private const string QUALIFIER_DEPRECATE = "DEP";
        private const string QUALIFIER_EXCLUDE = "EXCLUDE";

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
                baseName = root.baseModelObject.name;

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
                        if (treeNodes.ContainsKey(rel.destId))
                        {
                            TreeNode node = treeNodes[rel.destId];
                            // Changed element
                            if (rel.type == RelationshipTypeEnum.Generalization)
                            {
                                node.instructionObject = objectsCI[rel.sourceId];
                                node.includeInProfile = true;
                            }
                            // New child element
                            else if (rel.type == RelationshipTypeEnum.Aggregation)
                            {
                                // Set new parentId for the new element
                                TreeNode newNode = new TreeNode();
                                newNode.baseModelObject = objectsCI[rel.sourceId];
                                newNode.baseModelObject.parentId = node.baseModelObject.id;
                                newNode.parent = node;
                                newNode.isNew = true;
                                newNode.includeInProfile = true;
                                node.children.Add(newNode);
                                node.includeInProfile = true;
                            }
                            else
                            {
                                Console.WriteLine("Ignored relationship type: {0}", rel.type);
                            }
                        }
                        else
                        {
                            // E.g. for ExternalReferences
                            Console.WriteLine("Destination not in model: {0}", rel.destId);
                        }
                    }

                    ObjectType profileDefinition = objectsCI.Values.Single(o => R2Const.ST_FM_PROFILEDEFINITION.Equals(o.stereotype));
                    string rootid = root.baseModelObject.id;
                    root.baseModelObject = new ObjectType()
                    {
                        id = rootid,
                        name = profileDefinition.name,
                        stereotype = R2Const.ST_FM_PROFILE,
                        type = ObjectTypeEnum.Package,
                        typeSpecified = true,
                        tag = profileDefinition.tag
                    };
                }

                // Only follow ConsequenceLinks for ProfileType != Companion
                bool doFollowConsequenceLinks = true;
                if (root.baseModelObject.tag.Any(t => "Type".Equals(t.name)))
                {
                    TagType typeTag = root.baseModelObject.tag.Single(t => "Type".Equals(t.name));
                    doFollowConsequenceLinks = !"Companion".Equals(typeTag.value);
                }

                // Now auto include parents and criterions and included consequenceLinks
                autoInclude(root, doFollowConsequenceLinks);

                // Compile aka execute compiler instructions
                executeInstructions(root, PRIORITY_ESSENTIALNOW);

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
                    // Also remove modified date
                    maxObj.modifiedSpecified = false;
                }

                // assign new id's in objects parentId
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

                // assign new id's for relationships
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
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.NewLineChars = "\n";
                using (XmlWriter writer = XmlWriter.Create(fileNameOutput, settings))
                {
                    serializer.Serialize(writer, model);
                }

                // Check if all objects are included
                foreach (RelationshipType maxRel in model.relationships)
                {
                    if (!objects.Any(t => t.id == maxRel.sourceId))
                    {
                        string sourceId = idOrg2idNew.Single(t => t.Value == maxRel.sourceId).Key;
                        string destName = model.objects.Single(t => t.id == maxRel.destId).name;
                        Console.WriteLine("relationship from not included object sourceId={0} destId={1} stereotype={2} destName={3}", sourceId, maxRel.destId, maxRel.stereotype, destName);
                    }
                    if (!objects.Any(t => t.id == maxRel.destId))
                    {
                        string sourceId = idOrg2idNew.Single(t => t.Value == maxRel.sourceId).Key;
                        string sourceName = model.objects.Single(t => t.id == maxRel.sourceId).name;
                        Console.WriteLine("relationship to not included object sourceId={0} destId={1} stereotype={2} sourceName={3}", sourceId, maxRel.destId, maxRel.stereotype, sourceName);
                    }
                }
            }
        }

        private void displayNode(TreeNode node, int depth)
        {
            List<String> tagsAsString = new List<string>();
            List<TagType> tags = new List<TagType>(node.baseModelObject.tag);
            tags.ForEach(t => tagsAsString.Add(string.Format("{0}={1}", t.name, t.value)));
            if (node.instructionObject == null)
            {
                Console.WriteLine("[{0}] {1} {2}", depth, node.baseModelObject.name, string.Join(", ", tagsAsString.ToArray()));
            }
            else
            {
                Console.WriteLine("[{0}] <profiled> {1} {2}", depth, node.baseModelObject.name, string.Join(", ", tagsAsString.ToArray()));
            }
            foreach (TreeNode child in node.children)
            {
                displayNode(child, depth+1);
            }
        }

        /**
         * This will include the path to elements that have a Compiler Instruction or that are autoIncluded because of a ConsequenceLink.
         * E.g. when a Criterion has a Compiler Instruction than its parent Function/Headers get included
         * Also include Criterion when function is included, except for Criterion with QUALIFIER_EXCLUDE.
         * 
         * returns bool includeParent
         */
        private bool autoInclude(TreeNode node, bool doFollowConsequenceLinks)
        {
            node.includeInProfile |= node.hasInstruction;
            if (node.includeInProfile)
            {
                if (doFollowConsequenceLinks)
                {
                    followConsequenceLinks(node);
                }
                List<TreeNode> newChildren = new List<TreeNode>();
                foreach (TreeNode child in node.children)
                {
                    if (R2Const.ST_CRITERION.Equals(child.baseModelObject.stereotype))
                    {
                        if (child.instructionObject != null && child.instructionObject.tag != null)
                        {
                            TagType tagQualifier = child.instructionObject.tag.FirstOrDefault(t => t.name == R2Const.TV_QUALIFIER);
                            if (tagQualifier != null && QUALIFIER_EXCLUDE.Equals(tagQualifier.value))
                            {
                                continue;
                            }
                        }
                        else
                        {
                            child.includeInProfile = true;
                            node.includeInProfile = true;
                        }
                    }
                    else
                    {
                        node.includeInProfile |= autoInclude(child, doFollowConsequenceLinks);
                    }
                    newChildren.Add(child);
                }
                node.children = newChildren;
            }
            else
            {
                foreach (TreeNode child in node.children)
                {
                    node.includeInProfile |= autoInclude(child, doFollowConsequenceLinks);
                }
            }
            return node.includeInProfile;
        }

        /**
         * Just follow consequenceLinks and detect circular references.
         */
        private void followConsequenceLinks(TreeNode node)
        {
            if (node.hasConsequenceLinks)
            {
                foreach (RelationshipType consequenceLink in node.consequenceLinks)
                {
                    TreeNode linkedNode = treeNodes[consequenceLink.destId];
                    if (!linkedNode.includeInProfile)
                    {
                        linkedNode.includeInProfile = true;
                        followConsequenceLinks(linkedNode);
                    }
                    else
                    {
                        Console.WriteLine("consequenceLink from {0} to {1} object already included", node.baseModelObject.alias, linkedNode.baseModelObject.alias);
                    }
                }
            }
        }

        private void executeInstructions(TreeNode node, string priority)
        {
            List<TagType> tags = new List<TagType>();
            if (node.baseModelObject.tag != null)
            {
                tags.AddRange(node.baseModelObject.tag);
            }
            // remove deprecated row tag
            tags.RemoveAll(t => t.name.Equals(R2Const.TV_ROW));
            // remove empty value tags (cleans up)
            tags.RemoveAll(t => string.IsNullOrEmpty(t.value));
            
            if (!node.isNew)
            {
                // remove old Reference and set new Reference to base
                tags.RemoveAll(t => t.name.StartsWith("Reference."));
                switch (node.baseModelObject.stereotype)
                {
                    case R2Const.ST_CRITERION:
                        tags.Add(new TagType() {name = "Reference.Alias", value = baseName});
                        string functionId = node.baseModelObject.name.Substring(0, node.baseModelObject.name.IndexOf('#'));
                        tags.Add(new TagType() {name = "Reference.FunctionID", value = functionId});
                        int criterionId = int.Parse(node.baseModelObject.name.Substring(node.baseModelObject.name.IndexOf('#') + 1));
                        tags.Add(new TagType() {name = "Reference.CriterionID", value = criterionId.ToString()});
                        break;
                    case R2Const.ST_HEADER:
                    case R2Const.ST_FUNCTION:
                        tags.Add(new TagType() {name = "Reference.Alias", value = baseName});
                        tags.Add(new TagType() {name = "Reference.FunctionID", value = node.baseModelObject.alias});
                        break;
                    case R2Const.ST_SECTION:
                        tags.Add(new TagType() {name = "Reference.Alias", value = baseName});
                        tags.Add(new TagType() {name = "Reference.SectionID", value = node.baseModelObject.alias});
                        break;
                }
            }
            // set Priority & set ChangeIndicator to default "NC"; changed when executing instruction
            switch (node.baseModelObject.stereotype)
            {
                case R2Const.ST_SECTION:
                case R2Const.ST_HEADER:
                case R2Const.ST_FUNCTION:
                case R2Const.ST_CRITERION:
                    tags.RemoveAll(t => "Reference.ChangeIndicator".Equals(t.name));
                    tags.Add(new TagType() { name = "Reference.ChangeIndicator", 
                        value = node.isNew?CHANGEINDICATOR_NEW:CHANGEINDICATOR_NOCHANGE });
                    tags.Add(new TagType() { name = R2Const.TV_PRIORITY, value = priority });
                    break;
            }
            node.baseModelObject.tag = tags.ToArray();

            bool hasInstruction = node.hasInstruction;
            if (hasInstruction)
            {
                TagType tagChangeIndicator = node.baseModelObject.tag.Single(t => t.name.Equals("Reference.ChangeIndicator"));

                // update Optionality for Criterion
                if (R2Const.ST_CRITERION.Equals(node.baseModelObject.stereotype))
                {
                    int optionalityCount = node.baseModelObject.tag.Count(t => t.name == R2Const.TV_OPTIONALITY);
                    if (optionalityCount != 1)
                    {
                        Console.WriteLine("{0} expected 1 Optionality tag but got {1}", node.baseModelObject.name, optionalityCount);
                    }
                    TagType tagOptionality = node.baseModelObject.tag.Single(t => t.name == R2Const.TV_OPTIONALITY);

                    int newOptionalityCount = node.instructionObject.tag.Count(t => t.name == R2Const.TV_OPTIONALITY);
                    if (newOptionalityCount > 1)
                    {
                        Console.WriteLine("{0} expected 0..1 Optionality tag but got {1}", node.baseModelObject.name, newOptionalityCount);
                    }
                    TagType tagOptionalityNew = node.instructionObject.tag.FirstOrDefault(t => t.name == R2Const.TV_OPTIONALITY);
                    if (tagOptionalityNew != null && !tagOptionalityNew.value.Equals(tagOptionality.value))
                    {
                        tagChangeIndicator.value = CHANGEINDICATOR_CHANGED;
                        tagOptionality.value = tagOptionalityNew.value;
                    }
                }

                // update name if changed
                if (!node.baseModelObject.name.Equals(node.instructionObject.name))
                {
                    tagChangeIndicator.value = CHANGEINDICATOR_CHANGED;
                    node.baseModelObject.name = node.instructionObject.name;
                }

                // update notes if instruction has notes
                if (node.instructionObject.notes != null && node.instructionObject.notes.Text.Length > 0)
                {
                    tagChangeIndicator.value = CHANGEINDICATOR_CHANGED;
                    node.baseModelObject.notes = node.instructionObject.notes;
                }

                // override Priority
                TagType tagPriorityNew = node.instructionObject.tag.FirstOrDefault(t => t.name == R2Const.TV_PRIORITY);
                if (tagPriorityNew != null)
                {
                    TagType tagPriority = node.baseModelObject.tag.Single(t => t.name == R2Const.TV_PRIORITY);
                    tagPriority.value = tagPriorityNew.value;
                }

                // do Qualifier stuff. Delete or Deprecate
                int newQualifierCount = node.instructionObject.tag.Count(t => t.name == R2Const.TV_QUALIFIER);
                if (newQualifierCount > 1)
                {
                    Console.WriteLine("{0} expected 0..1 Qualifier tag but got {1}", node.baseModelObject.name, newQualifierCount);
                }
                TagType tagQualifier = node.instructionObject.tag.FirstOrDefault(t => t.name == R2Const.TV_QUALIFIER);
                if (tagQualifier != null)
                {
                    if (QUALIFIER_DELETE.Equals(tagQualifier.value))
                    {
                        tagChangeIndicator.value = CHANGEINDICATOR_DELETED;
                        // Make sure children are not included in profile and stop processing
                        node.children.Clear();
                    }
                    else if (QUALIFIER_DEPRECATE.Equals(tagQualifier.value))
                    {
                        tagChangeIndicator.value = CHANGEINDICATOR_DEPRECATED;
                    }
                }
            }

            List<TreeNode> newChildren = new List<TreeNode>();
            foreach(TreeNode child in node.children)
            {
                executeInstructions(child, priority);
                if (child.includeInProfile)
                {
                    newChildren.Add(child);
                }
            }
            switch (node.baseModelObject.stereotype)
            {
                case R2Const.ST_SECTION:
                case R2Const.ST_HEADER:
                case R2Const.ST_FUNCTION:
                case R2Const.ST_CRITERION:
                    // sort the items based on their name
                    node.children = newChildren.OrderBy(o => o.baseModelObject.name).ToList();
                    break;
                default:
                    // sort root childs == sections on ID tagged value
                    node.children = newChildren.OrderBy(o => o.baseModelObject.tag.FirstOrDefault(t=>"ID".Equals(t.name)).value).ToList();
                    break;
            }
        }
    }
}
