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
        private string baseName;
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

        public OutputListener _OutputListener { get; set; }

        public void Compile(string fileNameBase, string fileNameDefinition, string fileNameOutput)
        {
            if (_OutputListener == null)
            {
                _OutputListener = new ConsoleOutputListener();
            }

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
                                setDebugInclusionReason(newNode, "New in ProfileDefinition");

                                node.children.Add(newNode);
                                if (!node.includeInProfile)
                                {
                                    node.includeInProfile = true;
                                    setDebugInclusionReason(node, "Because of new child");
                                }

                                // Make sure to add new Node!
                                treeNodes[rel.sourceId] = newNode;
                            }
                            else
                            {
                                _OutputListener.writeOutput("[WARN] Ignored relationship type: {0}", rel.type);
                            }
                        }
                        else
                        {
                            if (objectsCI.ContainsKey(rel.destId))
                            {
                                // E.g. for links in Profile Definition
                                string srcName = rel.sourceId;
                                if (objectsCI[rel.sourceId].name != null)
                                {
                                    srcName = objectsCI[rel.sourceId].name.Split(new[] { ' ' })[0];
                                }
                                string dstName = rel.sourceId;
                                if (objectsCI[rel.destId].name != null)
                                {
                                    dstName = objectsCI[rel.destId].name.Split(new[] { ' ' })[0];
                                }
                                _OutputListener.writeOutput("[WARN] Ignored {0}. Not expected {1} to {2} in profile definition", srcName, rel.type, dstName, int.Parse(rel.sourceId));
                            }
                            else
                            {
                                // E.g. for ExternalReferences
                                _OutputListener.writeOutput("[WARN] Ignored {0}. Not expected {1} to id={2} outside base model", objectsCI[rel.sourceId].name, rel.type, rel.destId, int.Parse(rel.sourceId));
                            }
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

                // Exclude and exclude consequenceLinks when Criterion is Excluded
                exclude(root);

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
                            // object already has new id assigned, which is also not an integer!
                            _OutputListener.writeOutput("[WARN] Already has new id: {0}", maxObj.name, -1);
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

                // Convert back to MAX model
                ModelType model = new ModelType();
                model.exportDate = Util.FormatLastModified(DateTime.Now);
                model.objects = objects.ToArray();
                model.relationships = relationships.ToArray();
                // Check if all objects are included and remove dangling relationships
                foreach (RelationshipType maxRel in model.relationships)
                {
                    if (!objects.Any(t => t.id == maxRel.sourceId))
                    {
                        string sourceId = idOrg2idNew.Single(t => t.Value == maxRel.sourceId).Key;
                        string destName = model.objects.Single(t => t.id == maxRel.destId).name;
                        _OutputListener.writeOutput("[WARN] Relationship from not included object removed (sourceId={0} destId={1} stereotype={2} destName={3})", sourceId, maxRel.destId, maxRel.stereotype, destName);
                        relationships.Remove(maxRel);
                    }
                    if (!objects.Any(t => t.id == maxRel.destId))
                    {
                        string sourceId = idOrg2idNew.Single(t => t.Value == maxRel.sourceId).Key;
                        string sourceName = model.objects.Single(t => t.id == maxRel.sourceId).name;
                        _OutputListener.writeOutput("[WARN] Relationship to not included object removed (sourceId={0} destId={1} stereotype={2} sourceName={3})", sourceId, maxRel.destId, maxRel.stereotype, sourceName);
                        relationships.Remove(maxRel);
                    }
                }
                // update relationships
                model.relationships = relationships.ToArray();

                // Save compiled profile as MAX XML
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
            List<TagType> tags = new List<TagType>(node.baseModelObject.tag);
            tags.ForEach(t => tagsAsString.Add(string.Format("{0}={1}", t.name, t.value)));
            if (node.instructionObject == null)
            {
                _OutputListener.writeOutput("[DEBUG] #{0} {1} {2}", depth, node.baseModelObject.name, string.Join(", ", tagsAsString.ToArray()));
            }
            else
            {
                _OutputListener.writeOutput("[DEBUG] #{0} <profiled> {1} {2}", depth, node.baseModelObject.name, string.Join(", ", tagsAsString.ToArray()));
            }
            foreach (TreeNode child in node.children)
            {
                displayNode(child, depth+1);
            }
        }

        /**
         * This will remove excluded functions and also consequenceLinks when originating criterion is excluded
         */
        private void exclude(TreeNode node)
        {
            List<TreeNode> newChildren = new List<TreeNode>();
            foreach (TreeNode childNode in node.children)
            {
                if (R2Const.ST_CRITERION.Equals(childNode.baseModelObject.stereotype))
                {
                    if (childNode.instructionObject != null && childNode.instructionObject.tag != null)
                    {
                        TagType tagQualifier = childNode.instructionObject.tag.FirstOrDefault(t => R2Const.TV_QUALIFIER.Equals(t.name));
                        if (tagQualifier != null && QUALIFIER_EXCLUDE.Equals(tagQualifier.value))
                        {
                            // If this criterion results in a consequencelink, make sure to also remove that function
                            // ... if it was not explicitly included
                            // Check for "conform to function <FunctionID> (...)"
                            if (childNode.baseModelObject.notes != null && childNode.baseModelObject.notes.Text.Any())
                            {
                                string criterionText = childNode.baseModelObject.notes.Text[0];
                                int ref_idx = criterionText.IndexOf("conform to function");
                                if (ref_idx > 0)
                                {
                                    string ref_id = criterionText.Substring(ref_idx + 20, criterionText.IndexOf(' ', ref_idx + 20) - ref_idx - 20);
                                    TreeNode referencedNode = treeNodes.Values.SingleOrDefault(t => ref_id.Equals(t.baseModelObject.alias));
                                    if (referencedNode != null)
                                    {
                                        if (referencedNode.instructionObject == null)
                                        {
                                            childNode.relationships.RemoveAll(t => t.destId.Equals(referencedNode.baseModelObject.id));
                                            treeNodes.Remove(referencedNode.baseModelObject.id);

                                            _OutputListener.writeOutput("[INFO] Reference from {0} to {1} removed because of EXCLUDE", childNode.baseModelObject.name, ref_id);
                                        }
                                        else
                                        {
                                            _OutputListener.writeOutput("[INFO] Reference from {0} to {1} NOT removed because there is an instruction", childNode.baseModelObject.name, ref_id);
                                        }
                                    }
                                }
                            }
                            continue;
                        }
                    }
                }
                else
                {
                    exclude(childNode);
                }
                newChildren.Add(childNode);
            }
            // Smart sort objects, mixed string and numbers
            node.children = newChildren.OrderBy(n => ObjectTypeToSortKey(n.baseModelObject)).ToList();
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
                        child.includeInProfile = true;
                        node.includeInProfile = true;
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
                    if (treeNodes.ContainsKey(consequenceLink.destId))
                    {
                        TreeNode linkedNode = treeNodes[consequenceLink.destId];
                        if (!linkedNode.includeInProfile)
                        {
                            linkedNode.includeInProfile = true;
                            followConsequenceLinks(linkedNode);
                            setDebugInclusionReason(linkedNode, string.Format("ConsequenceLink from function {0}", node.baseModelObject.name));
                        }
                        else
                        {
                            _OutputListener.writeOutput("[WARN] ConsequenceLink from {0} to {1} object already included", node.baseModelObject.alias, linkedNode.baseModelObject.alias);
                        }
                    }
                    else
                    {
                        _OutputListener.writeOutput("[WARN] Already removed {0}, probably because of an EXCLUDE", consequenceLink.destId);
                    }
                }
            }
        }

        private void setDebugInclusionReason(TreeNode node, string reason)
        {
            node.addBaseModelTag("Debug.InclusionReason", reason);
        }

        private void executeInstructions(TreeNode node, string priority)
        {
            List<TagType> tags = new List<TagType>();
            if (node.baseModelObject.tag != null)
            {
                tags.AddRange(node.baseModelObject.tag);
            }
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
            // copy changenote.value if any
            if (node.hasInstruction)
            {
                int changeNoteCount = node.instructionObject.tag.Count(t => R2Const.TV_CHANGENOTE.Equals(t.name));
                if (changeNoteCount > 1)
                {
                    _OutputListener.writeOutput("[WARN] {0} expected 0..1 ChangeNote tag but got {1}", node.baseModelObject.name, changeNoteCount, node.instrID);
                }
                TagType tagChangeNote = node.instructionObject.tag.FirstOrDefault(t => R2Const.TV_CHANGENOTE.Equals(t.name));
                if (tagChangeNote != null)
                {
                    tags.Add(tagChangeNote);
                }
            }
            node.baseModelObject.tag = tags.ToArray();

            if (node.hasInstruction)
            {
                TagType tagChangeIndicator = node.baseModelObject.tag.Single(t => "Reference.ChangeIndicator".Equals(t.name));

                // update Optionality for Criterion
                if (R2Const.ST_CRITERION.Equals(node.baseModelObject.stereotype))
                {
                    int optionalityCount = node.baseModelObject.tag.Count(t => R2Const.TV_OPTIONALITY.Equals(t.name));
                    if (optionalityCount == 0)
                    {
                        _OutputListener.writeOutput("[ERROR] {0} expected 1 Optionality tag but got none", node.baseModelObject.name, node.instrID);
                        return;
                    }
                    else if (optionalityCount != 1)
                    {
                        _OutputListener.writeOutput("[WARN] {0} expected 1 Optionality tag but got {1}", node.baseModelObject.name, optionalityCount, node.instrID);
                    }
                    TagType tagOptionality = node.baseModelObject.tag.First(t => R2Const.TV_OPTIONALITY.Equals(t.name));

                    int newOptionalityCount = node.instructionObject.tag.Count(t => R2Const.TV_OPTIONALITY.Equals(t.name));
                    if (newOptionalityCount > 1)
                    {
                        _OutputListener.writeOutput("[WARN] {0} expected 0..1 Optionality tag but got {1}", node.baseModelObject.name, newOptionalityCount, node.instrID);
                    }
                    TagType tagOptionalityNew = node.instructionObject.tag.FirstOrDefault(t => R2Const.TV_OPTIONALITY.Equals(t.name));
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

                // is a new Priority in the instruction?
                TagType tagPriorityNew = node.instructionObject.tag.FirstOrDefault(t => R2Const.TV_PRIORITY.Equals(t.name));
                if (tagPriorityNew != null)
                {
                    int newPriorityCount = node.instructionObject.tag.Count(t => R2Const.TV_PRIORITY.Equals(t.name));
                    if (newPriorityCount > 1)
                    {
                        _OutputListener.writeOutput("[WARN] {0} expected 1 Priority tag but got {1}", node.baseModelObject.name, newPriorityCount, node.instrID);
                    }
                    if (!string.IsNullOrEmpty(tagPriorityNew.value))
                    {
                        priority = tagPriorityNew.value;
                    }
                }
                // update priority in the compiled (base) profile
                TagType tagPriority = node.baseModelObject.tag.FirstOrDefault(t => R2Const.TV_PRIORITY.Equals(t.name));
                if (tagPriority != null)
                {
                    tagPriority.value = priority;
                }
                else
                {
                    node.addBaseModelTag(R2Const.TV_PRIORITY, priority);
                }

                // do Qualifier stuff. Delete or Deprecate
                int newQualifierCount = node.instructionObject.tag.Count(t => R2Const.TV_QUALIFIER.Equals(t.name));
                if (newQualifierCount > 1)
                {
                    _OutputListener.writeOutput("[WARN] {0} expected 0..1 Qualifier tag but got {1}", node.baseModelObject.name, newQualifierCount, node.instrID);
                }
                TagType tagQualifier = node.instructionObject.tag.FirstOrDefault(t => R2Const.TV_QUALIFIER.Equals(t.name));
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
            node.children = newChildren;
        }

        public string ObjectTypeToSortKey(ObjectType element)
        {
            switch (element.stereotype)
            {
                case R2Const.ST_SECTION:
                    return ElementNameToSortKey(element.tag.Single(t => "ID".Equals(t.name)).value);
                case R2Const.ST_HEADER:
                case R2Const.ST_FUNCTION:
                case R2Const.ST_CRITERION:
                    return ElementNameToSortKey(element.name);
                default:
                    return "";
            }
        }

        public string ElementNameToSortKey(string sortKey)
        {
            // Only take first part == ID
            // Make sure # shows before .1 by replacing it with .0
            string[] parts = sortKey.Split(' ')[0].Replace("#", ".0.").Split('.');
            StringBuilder sb = new StringBuilder(parts[0]);
            for(int i=1; i<parts.Length;i++)
            {
                string part = parts[i];
                try
                {
                    part = string.Format("{0:D4}", int.Parse(part));
                }
                catch { }
                sb.Append(part);
            }
            return sb.ToString();
        }
    }

    public interface OutputListener
    {
        void writeOutput(string format, params object[] arg);
    }

    public class ConsoleOutputListener : OutputListener
    {
        public void writeOutput(string format, params object[] arg)
        {
            Console.WriteLine(format, arg);
        }
    }
}
