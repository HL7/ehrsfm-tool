using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HL7_FM_EA_Extension.R2ModelV2.Base;

namespace HL7_FM_EA_Extension
{
    public partial class MergeProfilesForm : Form
    {
        public MergeProfilesForm()
        {
            InitializeComponent();
        }

        private bool ignoreEvent = false;
        private EA.Repository repository;

        public void Show(EA.Repository repository, EA.Package selectedPackage)
        {
            this.repository = repository;
            Show();
            Refresh();

            dataGridView2.Rows.Clear();
            dataGridView2.Columns.Clear();
            int c1 = dataGridView2.Columns.Add("bm", "Base Model");
            int c2 = dataGridView2.Columns.Add("mp", "Merged Profile");
            int c3 = dataGridView2.Columns.Add("p1", "Chronic Diseases Cancer Surveillance");
            int c4 = dataGridView2.Columns.Add("p2", "Early Hearing Detection & Intervention");
            int c5 = dataGridView2.Columns.Add("p3", "Vital Records");

            DataGridViewCellStyle c2CellStyle = new DataGridViewCellStyle();
            c2CellStyle.BackColor = Color.Yellow;
            dataGridView2.Columns[c2].DefaultCellStyle = c2CellStyle;
            dataGridView2.Rows.Add(100);

            dummyData(c1);
            dummyData(c3);
            dummyData(c4);
            dummyData(c5);
        }

        private void dummyData(int columnNumber)
        {
            int rowNumber = 0;
            DataGridViewCell sectionCell = dataGridView2.Rows[rowNumber].Cells[columnNumber];
            R2Section section = new R2Section {SectionId = "CP"};
            sectionCell.Tag = section;
            sectionCell.Value = string.Format("Section {0}", section.SectionId);
            for (int h = 0; h < 3; h++)
            {
                rowNumber++;
                DataGridViewCell headerCell = dataGridView2.Rows[rowNumber].Cells[columnNumber];
                R2Function header = new R2Function {FunctionId = string.Format("CP.{0}", h)};
                headerCell.Tag = header;
                headerCell.Value = header.FunctionId;
                headerCell.ToolTipText = string.Format("{0} Header", header.FunctionId);
                for (int f = 0; f < 3; f++)
                {
                    rowNumber++;
                    DataGridViewCell functionCell = dataGridView2.Rows[rowNumber].Cells[columnNumber];
                    R2Function function = new R2Function { FunctionId = string.Format("CP.{0}.{1}", h, f) };
                    functionCell.Tag = function;
                    functionCell.Value = function.FunctionId;
                    functionCell.ToolTipText = string.Format("{0} Function", function.FunctionId);
                    for (int c = 0; c < 4; c++)
                    {
                        rowNumber++;
                        DataGridViewCell criterionCell = dataGridView2.Rows[rowNumber].Cells[columnNumber];
                        R2Criterion criterion = new R2Criterion
                                                    {
                                                        FunctionId = string.Format("CP.{0}.{1}", h, f), 
                                                        CriterionId = c,
                                                        Text = "The system SHALL xyz",
                                                        Optionality = "SHALL"
                                                    };
                        criterionCell.Tag = criterion;
                        criterionCell.Value = criterion.Name;
                        criterionCell.ToolTipText = string.Format("{0} {1}", criterion.Name, criterion.Text);
                    }
                }
            }
        }

        private int _currentRow = -1;
        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (_currentRow != e.RowIndex)
            {
                _currentRow = e.RowIndex;

                dataGridView1.Rows.Clear();
                dataGridView1.Columns.Clear();

                DataGridViewCell cell = dataGridView2.Rows[e.RowIndex].Cells[e.ColumnIndex];
                if (cell.Tag is R2Section)
                {
                    dataGridView1.Columns.Add("Stereotype", "Stereotype");
                    dataGridView1.Columns.Add("SectionId", "SectionId");
                    dataGridView1.Columns.Add("Name", "Name");
                    dataGridView1.Columns.Add("LastModified", "LastModified");
                }
                else if (cell.Tag is R2Function)
                {
                    dataGridView1.Columns.Add("Stereotype", "Stereotype");
                    dataGridView1.Columns.Add("FunctionId", "FunctionId");
                    dataGridView1.Columns.Add("Name", "Name");
                    dataGridView1.Columns.Add("LastModified", "LastModified");
                }
                else if (cell.Tag is R2Criterion)
                {
                    dataGridView1.Columns.Add("Stereotype", "Stereotype");
                    dataGridView1.Columns.Add("Name", "Name");
                    dataGridView1.Columns.Add("Text", "Text");
                    dataGridView1.Columns.Add("Row", "Row");
                    dataGridView1.Columns.Add("Dependent", "Dependent");
                    dataGridView1.Columns.Add("Conditional", "Conditional");
                    dataGridView1.Columns.Add("Optionality", "Optionality");
                    dataGridView1.Columns.Add("LastModified", "LastModified");
                }

                dataGridView1.Rows.Add(5);
                dataGridView1.Rows[0].HeaderCell.Value = "Base Model";
                dataGridView1.Rows[1].HeaderCell.Value = "Merged Profile";
                dataGridView1.Rows[2].HeaderCell.Value = "Chronic Diseases Cancer Surveillance";
                dataGridView1.Rows[3].HeaderCell.Value = "Early Hearing Detection & Intervention";
                dataGridView1.Rows[4].HeaderCell.Value = "Vital Records";
            }
        }

        private void dataGridView2_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewCell cell = dataGridView2.Rows[e.RowIndex].Cells[e.ColumnIndex];
            if (cell.Tag is R2Section)
            {
                new SectionForm().Show((R2Section)cell.Tag);
            }
            else if (cell.Tag is R2Function)
            {
                new FunctionForm().Show((R2Function)cell.Tag);
            }
            else if (cell.Tag is R2Criterion)
            {
                new CriterionForm().Show((R2Criterion)cell.Tag);
            }
        }
    }
}
