using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MAX_EA;
using Saxon.Api;

namespace MAX_EA_Extension
{
    public partial class ValidateParamsForm : Form
    {
        public ValidateParamsForm()
        {
            InitializeComponent();
        }

        private string defaultSchematronFile = @"C:\Eclipse Workspace\NieuwEPD\9.01 Schematron\901 Validation Rules.sch";
        private bool issues = false;

        private EA.Repository repository;
        public bool Show(EA.Repository repository)
        {
            this.repository = repository;
            string defaultFolder = Util.CurrentOutputPath;
            string maxFile = "";
            string schFile = GetSchematronTaggedValue(repository).Value;
            string svrlFile = "";
            EA.ObjectType type = repository.GetTreeSelectedItemType();
            if (type == EA.ObjectType.otPackage)
            {
                EA.Package package = repository.GetTreeSelectedPackage();
                maxFile = Path.Combine(defaultFolder, string.Format("{0}.max.xml", package.Name));
                svrlFile = Path.Combine(defaultFolder, string.Format("{0}-svrl_output.xml", package.Name));
            }
            else if (type == EA.ObjectType.otDiagram)
            {
                EA.Diagram diagram = (EA.Diagram)repository.GetTreeSelectedObject();
                maxFile = Path.Combine(defaultFolder, string.Format("{0}.max.xml", diagram.Name));
                svrlFile = Path.Combine(defaultFolder, string.Format("{0}-svrl_output.xml", diagram.Name));
            }

            textBox1.Text = maxFile;
            textBox2.Text = schFile;
            textBox3.Text = svrlFile;

            ShowDialog();
            return issues;
        }

        private const string TV_MAX_SchematronFile = "MAX::SchematronFile";
        private EA.TaggedValue GetSchematronTaggedValue(EA.Repository Repository)
        {
            EA.Package selectedPackage = Repository.GetTreeSelectedPackage();
            EA.TaggedValue tvSchematronFile = (EA.TaggedValue)selectedPackage.Element.TaggedValues.GetByName(TV_MAX_SchematronFile);
            if (tvSchematronFile == null)
            {
                tvSchematronFile = (EA.TaggedValue)selectedPackage.Element.TaggedValues.AddNew(TV_MAX_SchematronFile, "");
                tvSchematronFile.Value = defaultSchematronFile;
                tvSchematronFile.Update();
            }
            return tvSchematronFile;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Enabled = false;
            Close();

            string maxFile = textBox1.Text;
            string schFile = textBox2.Text;
            string svrlFile = textBox3.Text;

            // Update Package SchematronFile
            EA.TaggedValue tvSchematronFile = GetSchematronTaggedValue(repository);
            tvSchematronFile.Value = schFile;
            tvSchematronFile.Update();

            // Create MAX file
            MAX_EA.MAXExporter3 exporter = new MAX_EA.MAXExporter3();
            EA.ObjectType type = repository.GetTreeSelectedItemType();
            if (type == EA.ObjectType.otPackage)
            {
                EA.Package package = repository.GetTreeSelectedPackage();
                exporter.exportPackage(repository, package, maxFile);
            }
            else if (type == EA.ObjectType.otDiagram)
            {
                EA.Diagram diagram = (EA.Diagram)repository.GetTreeSelectedObject();
                exporter.exportDiagram(repository, diagram, maxFile);
            }

            // Execute Transform
            issues = new MAXValidator().validate(repository, maxFile, schFile, svrlFile);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Schematron files|*.sch";
            openFileDialog1.Title = "Select Schematron file";
            openFileDialog1.InitialDirectory = Path.GetDirectoryName(textBox2.Text);
            openFileDialog1.FileName = Path.GetFileName(textBox2.Text);
            openFileDialog1.CheckFileExists = true;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox2.Text = openFileDialog1.FileName;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "MAX files|*.max.xml";
            openFileDialog1.Title = "Select Input file";
            openFileDialog1.InitialDirectory = Path.GetDirectoryName(textBox1.Text);
            openFileDialog1.FileName = Path.GetFileName(textBox1.Text);
            openFileDialog1.CheckFileExists = false;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = openFileDialog1.FileName;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "XML files|*.xml";
            openFileDialog1.Title = "Select Output file";
            openFileDialog1.InitialDirectory = Path.GetDirectoryName(textBox3.Text);
            openFileDialog1.FileName = Path.GetFileName(textBox3.Text);
            openFileDialog1.CheckFileExists = false;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox3.Text = openFileDialog1.FileName;
            }
        }
    }
}
