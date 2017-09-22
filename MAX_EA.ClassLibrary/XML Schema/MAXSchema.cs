﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by xsd, Version=4.6.1055.0.
// 
namespace MAX_EA.MAXSchema {
    using System.Xml.Serialization;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.umcg.nl/MAX")]
    [System.Xml.Serialization.XmlRootAttribute("model", Namespace="http://www.umcg.nl/MAX", IsNullable=false)]
    public partial class ModelType {
        
        private ObjectType[] objectsField;
        
        private RelationshipType[] relationshipsField;
        
        private string exportDateField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Xml.Serialization.XmlArrayItemAttribute("object", Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=false)]
        public ObjectType[] objects {
            get {
                return this.objectsField;
            }
            set {
                this.objectsField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Xml.Serialization.XmlArrayItemAttribute("relationship", Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=false)]
        public RelationshipType[] relationships {
            get {
                return this.relationshipsField;
            }
            set {
                this.relationshipsField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string exportDate {
            get {
                return this.exportDateField;
            }
            set {
                this.exportDateField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.umcg.nl/MAX")]
    public partial class ObjectType {
        
        private string idField;
        
        private string nameField;
        
        private string aliasField;
        
        private MarkupType notesField;
        
        private string stereotypeField;
        
        private ObjectTypeEnum typeField;
        
        private bool typeFieldSpecified;
        
        private string parentIdField;
        
        private System.DateTime modifiedField;
        
        private bool modifiedFieldSpecified;
        
        private TagType[] tagField;
        
        private AttributeType[] attributeField;
        
        private bool isAbstractField;
        
        private bool isAbstractFieldSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, DataType="token")]
        public string id {
            get {
                return this.idField;
            }
            set {
                this.idField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, DataType="token")]
        public string name {
            get {
                return this.nameField;
            }
            set {
                this.nameField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string alias {
            get {
                return this.aliasField;
            }
            set {
                this.aliasField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public MarkupType notes {
            get {
                return this.notesField;
            }
            set {
                this.notesField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string stereotype {
            get {
                return this.stereotypeField;
            }
            set {
                this.stereotypeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public ObjectTypeEnum type {
            get {
                return this.typeField;
            }
            set {
                this.typeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool typeSpecified {
            get {
                return this.typeFieldSpecified;
            }
            set {
                this.typeFieldSpecified = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string parentId {
            get {
                return this.parentIdField;
            }
            set {
                this.parentIdField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public System.DateTime modified {
            get {
                return this.modifiedField;
            }
            set {
                this.modifiedField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool modifiedSpecified {
            get {
                return this.modifiedFieldSpecified;
            }
            set {
                this.modifiedFieldSpecified = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("tag", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public TagType[] tag {
            get {
                return this.tagField;
            }
            set {
                this.tagField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("attribute", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public AttributeType[] attribute {
            get {
                return this.attributeField;
            }
            set {
                this.attributeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool isAbstract {
            get {
                return this.isAbstractField;
            }
            set {
                this.isAbstractField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool isAbstractSpecified {
            get {
                return this.isAbstractFieldSpecified;
            }
            set {
                this.isAbstractFieldSpecified = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.umcg.nl/MAX")]
    public partial class MarkupType {
        
        private string[] textField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string[] Text {
            get {
                return this.textField;
            }
            set {
                this.textField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.umcg.nl/MAX")]
    public partial class RelationshipType {
        
        private string idField;
        
        private string labelField;
        
        private string sourceIdField;
        
        private string sourceLabelField;
        
        private string sourceCardField;
        
        private string destIdField;
        
        private string destLabelField;
        
        private string destCardField;
        
        private MarkupType notesField;
        
        private string stereotypeField;
        
        private RelationshipTypeEnum typeField;
        
        private bool typeFieldSpecified;
        
        private TagType[] tagField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, DataType="token")]
        public string id {
            get {
                return this.idField;
            }
            set {
                this.idField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, DataType="token")]
        public string label {
            get {
                return this.labelField;
            }
            set {
                this.labelField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string sourceId {
            get {
                return this.sourceIdField;
            }
            set {
                this.sourceIdField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, DataType="token")]
        public string sourceLabel {
            get {
                return this.sourceLabelField;
            }
            set {
                this.sourceLabelField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, DataType="token")]
        public string sourceCard {
            get {
                return this.sourceCardField;
            }
            set {
                this.sourceCardField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, DataType="token")]
        public string destId {
            get {
                return this.destIdField;
            }
            set {
                this.destIdField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, DataType="token")]
        public string destLabel {
            get {
                return this.destLabelField;
            }
            set {
                this.destLabelField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, DataType="token")]
        public string destCard {
            get {
                return this.destCardField;
            }
            set {
                this.destCardField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public MarkupType notes {
            get {
                return this.notesField;
            }
            set {
                this.notesField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, DataType="token")]
        public string stereotype {
            get {
                return this.stereotypeField;
            }
            set {
                this.stereotypeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public RelationshipTypeEnum type {
            get {
                return this.typeField;
            }
            set {
                this.typeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool typeSpecified {
            get {
                return this.typeFieldSpecified;
            }
            set {
                this.typeFieldSpecified = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("tag", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public TagType[] tag {
            get {
                return this.tagField;
            }
            set {
                this.tagField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.umcg.nl/MAX")]
    public enum RelationshipTypeEnum {
        
        /// <remarks/>
        Association,
        
        /// <remarks/>
        DirectedAssociation,
        
        /// <remarks/>
        Dependency,
        
        /// <remarks/>
        Aggregation,
        
        /// <remarks/>
        Composition,
        
        /// <remarks/>
        Generalization,
        
        /// <remarks/>
        Realisation,
        
        /// <remarks/>
        InformationFlow,
        
        /// <remarks/>
        Abstraction,
        
        /// <remarks/>
        Usage,
        
        /// <remarks/>
        Package,
        
        /// <remarks/>
        NoteLink,
        
        /// <remarks/>
        UseCase,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.umcg.nl/MAX")]
    public partial class TagType {
        
        private string nameField;
        
        private string valueField;
        
        private string[] textField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType="token")]
        public string name {
            get {
                return this.nameField;
            }
            set {
                this.nameField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string value {
            get {
                return this.valueField;
            }
            set {
                this.valueField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string[] Text {
            get {
                return this.textField;
            }
            set {
                this.textField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.umcg.nl/MAX")]
    public partial class AttributeType {
        
        private TagType[] tagField;
        
        private string[] textField;
        
        private string idField;
        
        private string nameField;
        
        private string aliasField;
        
        private string minCardField;
        
        private string maxCardField;
        
        private string valueField;
        
        private string typeField;
        
        private string stereotypeField;
        
        private bool isReadOnlyField;
        
        private bool isReadOnlyFieldSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("tag", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public TagType[] tag {
            get {
                return this.tagField;
            }
            set {
                this.tagField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string[] Text {
            get {
                return this.textField;
            }
            set {
                this.textField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType="token")]
        public string id {
            get {
                return this.idField;
            }
            set {
                this.idField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType="token")]
        public string name {
            get {
                return this.nameField;
            }
            set {
                this.nameField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string alias {
            get {
                return this.aliasField;
            }
            set {
                this.aliasField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType="token")]
        public string minCard {
            get {
                return this.minCardField;
            }
            set {
                this.minCardField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType="token")]
        public string maxCard {
            get {
                return this.maxCardField;
            }
            set {
                this.maxCardField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string value {
            get {
                return this.valueField;
            }
            set {
                this.valueField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType="token")]
        public string type {
            get {
                return this.typeField;
            }
            set {
                this.typeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType="token")]
        public string stereotype {
            get {
                return this.stereotypeField;
            }
            set {
                this.stereotypeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool isReadOnly {
            get {
                return this.isReadOnlyField;
            }
            set {
                this.isReadOnlyField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool isReadOnlySpecified {
            get {
                return this.isReadOnlyFieldSpecified;
            }
            set {
                this.isReadOnlyFieldSpecified = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.umcg.nl/MAX")]
    public enum ObjectTypeEnum {
        
        /// <remarks/>
        Interface,
        
        /// <remarks/>
        Class,
        
        /// <remarks/>
        Object,
        
        /// <remarks/>
        Feature,
        
        /// <remarks/>
        Requirement,
        
        /// <remarks/>
        Component,
        
        /// <remarks/>
        Package,
        
        /// <remarks/>
        Activity,
        
        /// <remarks/>
        Action,
        
        /// <remarks/>
        Issue,
        
        /// <remarks/>
        Change,
        
        /// <remarks/>
        Artifact,
        
        /// <remarks/>
        Note,
        
        /// <remarks/>
        Text,
        
        /// <remarks/>
        Constraint,
        
        /// <remarks/>
        Enumeration,
        
        /// <remarks/>
        Boundary,
        
        /// <remarks/>
        UseCase,
        
        /// <remarks/>
        Actor,
    }
}
