using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using MAX_EA.MAXSchema;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HL7_FM_EA_Extension.Tests
{
    [TestClass]
    public class MAXTreeNodeTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            TreeNode node = new TreeNode();
            List<ObjectType> objects = node.ToObjectList();
            Assert.AreEqual(0, objects.Count);
        }

        [TestMethod]
        public void TestMethod2()
        {
            TreeNode node = new TreeNode();
            node.baseModelObject = new ObjectType();
            List<ObjectType> objects = node.ToObjectList();
            Assert.AreEqual(1, objects.Count);
        }

        [TestMethod]
        public void TestMethod3Parent()
        {
            TreeNode node1 = new TreeNode();
            node1.baseModelObject = new ObjectType();

            TreeNode node2 = new TreeNode();
            node2.baseModelObject = new ObjectType();
            node2.parent = node1;

            List<ObjectType> objects = node1.ToObjectList();
            Assert.AreEqual(1, objects.Count);
        }

        [TestMethod]
        public void TestMethod4Parent()
        {
            TreeNode node1 = new TreeNode();

            TreeNode node2 = new TreeNode();
            node2.baseModelObject = new ObjectType();
            node2.parent = node1;

            List<ObjectType> objects = node1.ToObjectList();
            Assert.AreEqual(0, objects.Count);
        }

        [TestMethod]
        public void TestMethod5Parent()
        {
            TreeNode node1 = new TreeNode();
            node1.baseModelObject = new ObjectType();

            TreeNode node2 = new TreeNode();
            node2.parent = node1;

            List<ObjectType> objects = node1.ToObjectList();
            Assert.AreEqual(1, objects.Count);
        }

        [TestMethod]
        public void TestMethod6Parent()
        {
            TreeNode node1 = new TreeNode();
            node1.baseModelObject = new ObjectType();

            TreeNode node2 = new TreeNode();
            node2.baseModelObject = new ObjectType();
            node2.parent = node1;

            TreeNode node3 = new TreeNode();
            node3.baseModelObject = new ObjectType();
            node3.parent = node2;

            List<ObjectType> objects = node1.ToObjectList();
            Assert.AreEqual(1, objects.Count);
        }

        [TestMethod]
        public void TestMethod3Children()
        {
            TreeNode node1 = new TreeNode();
            node1.baseModelObject = new ObjectType();

            TreeNode node2 = new TreeNode();
            node2.baseModelObject = new ObjectType();
            node1.children.Add(node2);

            List<ObjectType> objects = node1.ToObjectList();
            Assert.AreEqual(2, objects.Count);
        }

        [TestMethod]
        public void TestMethod4Children()
        {
            TreeNode node1 = new TreeNode();

            TreeNode node2 = new TreeNode();
            node2.baseModelObject = new ObjectType();
            node1.children.Add(node2);

            List<ObjectType> objects = node1.ToObjectList();
            Assert.AreEqual(1, objects.Count);
        }

        [TestMethod]
        public void TestMethod5Children()
        {
            TreeNode node1 = new TreeNode();
            node1.baseModelObject = new ObjectType();

            TreeNode node2 = new TreeNode();
            node1.children.Add(node2);

            List<ObjectType> objects = node1.ToObjectList();
            Assert.AreEqual(1, objects.Count);
        }

        [TestMethod]
        public void TestMethod6Children()
        {
            TreeNode node1 = new TreeNode();
            node1.baseModelObject = new ObjectType();

            TreeNode node2 = new TreeNode();
            node2.baseModelObject = new ObjectType();
            node1.children.Add(node2);

            TreeNode node3 = new TreeNode();
            node3.baseModelObject = new ObjectType();
            node2.children.Add(node3);

            List<ObjectType> objects = node1.ToObjectList();
            Assert.AreEqual(3, objects.Count);
        }

        [TestMethod]
        public void TestMoveNodeUsingParent()
        {
            TreeNode node1 = new TreeNode();
            node1.baseModelObject = new ObjectType();

            TreeNode node2 = new TreeNode();
            node2.baseModelObject = new ObjectType();

            TreeNode node3 = new TreeNode();
            node3.baseModelObject = new ObjectType();
            node3.parent = node1;
            node3.parent = node2;

            List<ObjectType> objects1 = node1.ToObjectList();
            Assert.AreEqual(1, objects1.Count);
            List<ObjectType> objects2 = node2.ToObjectList();
            Assert.AreEqual(1, objects2.Count);
        }

        [TestMethod]
        public void TestMultipleParent()
        {
            TreeNode node1 = new TreeNode();
            node1.baseModelObject = new ObjectType();

            TreeNode node2 = new TreeNode();
            node2.baseModelObject = new ObjectType();
            node1.children.Add(node2);
            node2.parent = node1;
            node2.parent = node1;
            node2.parent = node1;
            node2.parent = node1;

            List<ObjectType> objects = node1.ToObjectList();
            Assert.AreEqual(2, objects.Count);
        }

        [TestMethod]
        public void TestConsequenceLink0()
        {
            TreeNode node1 = new TreeNode();
            node1.baseModelObject = new ObjectType();

            Assert.AreEqual(0, node1.consequenceLinks.Count());
        }


        [TestMethod]
        public void TestConsequenceLink1()
        {
            TreeNode node1 = new TreeNode();
            node1.baseModelObject = new ObjectType();

            RelationshipType maxRel = new RelationshipType() {
                stereotype = "ConsequenceLink" };

            node1.relationships.Add(new RelationshipType());
            node1.relationships.Add(maxRel);
            node1.relationships.Add(new RelationshipType());
            node1.relationships.Add(new RelationshipType());

            Assert.AreEqual(1, node1.consequenceLinks.Count());
            Assert.AreEqual(maxRel, node1.consequenceLinks.First());
        }
    }
}
