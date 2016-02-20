using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using MAX_EA;
using Saxon.Api;

namespace MAX_EA_Extension
{
    class Filters
    {
        public static string CurrentOutputPath { get; set; }
        private const string FILEDIALOG_TITLE = "Select file and type";
        private const string FILEDIALOG_FILTERS = "MAX xml files (*.xml,*.max)|*.xml;*.max|DataElement xml files (*.xml)|*.xml";
        private const int FILTER_MAX = 1;
        private const int FILTER_EXCEL_DATASET = 2;
        private int SelectedFilter { get; set; }

        public bool import(EA.Repository Repository, EA.Package selectedPackage)
        {
            string defaultFileName = @"C:\Temp\default.max";
            EA.TaggedValue tvImportFile = (EA.TaggedValue)selectedPackage.Element.TaggedValues.GetByName("MAX::ImportFile");
            if (tvImportFile != null)
            {
                defaultFileName = tvImportFile.Value;
            }
            string fileName = showFileDialog(defaultFileName, true);
            if (fileName != String.Empty)
            {
                if (SelectedFilter == FILTER_MAX)
                {
                    return new MAXImporter3().import(Repository, selectedPackage, fileName);
                }
                else if (SelectedFilter == FILTER_EXCEL_DATASET)
                {
                    string xsltFile = Main.getAppDataFullPath(@"XML Transforms\excel-de-to-max.xslt");
                    string tmpFile = fileName + ".tmp";
                    if (!filter(Repository, fileName, xsltFile, tmpFile))
                    {
                        return new MAXImporter3().import(Repository, selectedPackage, tmpFile);
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public bool export(EA.Repository Repository)
        {
            EA.ObjectType type = Repository.GetContextItemType();
            if (type == EA.ObjectType.otPackage)
            {
                EA.Package package = (EA.Package)Repository.GetContextObject();
                return exportPackage(Repository, package);
            }
            else if (type == EA.ObjectType.otDiagram)
            {
                EA.Diagram diagram = (EA.Diagram)Repository.GetContextObject();
                string defaultFileName = Path.Combine(CurrentOutputPath, string.Format(@"{0}.max", diagram.Name));
                string fileName = showFileDialog(defaultFileName, false);
                if (fileName != String.Empty)
                {
                    if (SelectedFilter == FILTER_MAX)
                    {
                        return new MAXExporter3().exportDiagram(Repository, diagram, fileName);
                    }
                    else if (SelectedFilter == FILTER_EXCEL_DATASET)
                    {
                        string xsltFile = Main.getAppDataFullPath(@"XML Transforms\max-to-excel-de.xslt");
                        string tmpFile = fileName + ".tmp";
                        if (!new MAXExporter3().exportDiagram(Repository, diagram, tmpFile))
                        {
                            return filter(Repository, tmpFile, xsltFile, fileName);
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                MessageBox.Show("Select a Package or Diagram");
                return false;
            }
        }

        public bool exportPackage(EA.Repository Repository, EA.Package package)
        {
            string defaultFileName = Path.Combine(CurrentOutputPath, string.Format(@"{0}.max", package.Name));
            EA.TaggedValue tvExportFile = (EA.TaggedValue)package.Element.TaggedValues.GetByName("MAX::ExportFile");
            if (tvExportFile != null)
            {
                defaultFileName = tvExportFile.Value;
            }
            string fileName = showFileDialog(defaultFileName, false);
            if (fileName != String.Empty)
            {
                if (SelectedFilter == FILTER_MAX)
                {
                    return new MAXExporter3().exportPackage(Repository, package, fileName);
                }
                else if (SelectedFilter == FILTER_EXCEL_DATASET)
                {
                    string xsltFile = Main.getAppDataFullPath(@"XML Transforms\max-to-excel-de.xslt");
                    string tmpFile = fileName + ".tmp";
                    if (!new MAXExporter3().exportPackage(Repository, package, tmpFile))
                    {
                        return filter(Repository, tmpFile, xsltFile, fileName);
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }                
            }
            else
            {
                return false;
            }
        }

        private bool filter(EA.Repository Repository, string maxFile, string xsltFile, string outputFile)
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
                return false; // no issues
            }
            catch (Exception ex)
            {
                Main.LogMessage(Repository, ex.Message, 0);
                StringBuilder sb = new StringBuilder();
                foreach (var error in compiler.ErrorList)
                {
                    Main.LogMessage(Repository, error.ToString(), 0);
                }
                return true; // issues!
            }
        }

        public string showFileDialog(string fileName, bool open)
        {
            FileDialog dialog;
            if (open)
            {
                dialog = new OpenFileDialog();
            }
            else
            {
                dialog = new SaveFileDialog();
            }
            dialog.Title = FILEDIALOG_TITLE;
            dialog.Filter = FILEDIALOG_FILTERS;
            dialog.InitialDirectory = Path.GetDirectoryName(fileName);
            dialog.FileName = Path.GetFileName(fileName);

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                SelectedFilter = dialog.FilterIndex;
                CurrentOutputPath = Path.GetDirectoryName(dialog.FileName);
                return dialog.FileName;
            }
            else
            {
                return String.Empty;
            }
        }
    }
}
