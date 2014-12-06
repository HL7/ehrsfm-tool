using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Saxon.Api;

namespace MAX_EA_Extension
{
    public partial class TransformParamsForm : Form
    {
        public TransformParamsForm()
        {
            InitializeComponent();
        }

        private string defaultFolder = Path.GetTempPath();
        // TODO: outputFormat = html or xml or txt, get from xslt
        private string outputFormat = "html";
        private string defaultXsltFile = @"C:\Eclipse Workspace\NieuwEPD\9.01\901-report.xslt";
        private bool issues = false;

        private EA.Repository repository;
        public bool Show(EA.Repository repository)
        {
            this.repository = repository;
            string maxFile = "";
            string xsltFile = GetXsltTaggedValue(repository).Value;
            string outputFile = "";
            EA.ObjectType type = repository.GetTreeSelectedItemType();
            if (type == EA.ObjectType.otPackage)
            {
                EA.Package package = repository.GetTreeSelectedPackage();
                maxFile = Path.Combine(defaultFolder, string.Format("{0}.max.xml", package.Name));
                outputFile = Path.Combine(defaultFolder, string.Format("{0}.{1}", package.Name, outputFormat));
            }
            else if (type == EA.ObjectType.otDiagram)
            {
                EA.Diagram diagram = (EA.Diagram)repository.GetTreeSelectedObject();
                maxFile = Path.Combine(defaultFolder, string.Format("{0}.max.xml", diagram.Name));
                outputFile = Path.Combine(defaultFolder, string.Format("{0}.{1}", diagram.Name, outputFormat));
            }

            textBox1.Text = maxFile;
            textBox2.Text = xsltFile;
            textBox3.Text = outputFile;

            ShowDialog();
            return issues;
        }

        private bool transform(EA.Repository Repository, string maxFile, string xsltFile, string outputFile)
        {
            // Create a Processor instance.
            Processor processor = new Processor();
            XsltCompiler compiler = processor.NewXsltCompiler();
            compiler.ErrorList = new List<StaticError>();
            DocumentBuilder builder = processor.NewDocumentBuilder();

            try
            {
                // Create a transformer for the stylesheet.
                XsltTransformer transformer = compiler.Compile(new Uri(xsltFile)).Load();

                // Load the source document
                XdmNode input = builder.Build(new Uri(maxFile));

                // Set the root node of the source document to be the initial context node
                transformer.InitialContextNode = input;

                // Create a serializer, with output to the standard output stream
                Serializer serializer = new Serializer();
                serializer.SetOutputFile(outputFile);

                // Transform the source XML and serialize the result document
                transformer.Run(serializer);
                return true;
            }
            catch (Exception ex)
            {
                Main.LogMessage(Repository, ex.Message, 0);
                StringBuilder sb = new StringBuilder();
                foreach (var error in compiler.ErrorList)
                {
                    Main.LogMessage(Repository, error.ToString(), 0);
                }
                return false;
            }
        }

        private const string TV_MAX_XSLTFile = "MAX::XSLTFile";
        private EA.TaggedValue GetXsltTaggedValue(EA.Repository Repository)
        {
            // N.B. Model doesnot support tagged values!
            EA.Package modelPackage = (EA.Package)Repository.Models.GetAt(0);
            EA.Package viewPackage = (EA.Package)modelPackage.Packages.GetAt(0);
            EA.TaggedValue tvXsltFile = (EA.TaggedValue)viewPackage.Element.TaggedValues.GetByName(TV_MAX_XSLTFile);
            if (tvXsltFile == null)
            {
                tvXsltFile = (EA.TaggedValue)viewPackage.Element.TaggedValues.AddNew(TV_MAX_XSLTFile, "");
                tvXsltFile.Value = defaultXsltFile;
                tvXsltFile.Update();
            }
            return tvXsltFile;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Enabled = false;
            Close();

            string maxFile = textBox1.Text;
            string xsltFile = textBox2.Text;
            string outputFile = textBox3.Text;

            // Update project XSLTFile
            EA.TaggedValue tvXsltFile = GetXsltTaggedValue(repository);
            tvXsltFile.Value = xsltFile;
            tvXsltFile.Update();

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
            // TODO: Show feedback about the transform steps
            if (transform(repository, maxFile, xsltFile, outputFile))
            {
                if (checkBox1.Checked)
                {
                    string outputURL = new Uri(outputFile).ToString();
                    System.Diagnostics.Process.Start(outputURL);
                }
            }
            else
            {
                issues = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "XSLT files|*.xslt";
            openFileDialog1.Title = "Select Transformation file";
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
            openFileDialog1.Filter = string.Format("{0} files|*.{1}", outputFormat.ToUpper(), outputFormat);
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
