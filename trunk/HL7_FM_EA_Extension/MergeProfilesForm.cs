﻿using System;
using System.Collections;
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

        // Limit rows for each model
        private int _currentRow = -1;
        private List<string> modelNames = new List<string>();

        public void PopulateAndShow()
        {
//            Show();
//            Refresh();

            modelNames.Clear();
            modelNames.Add("Base Model");
            modelNames.Add("Merged Profile");
            modelNames.Add("");
            modelNames.Add("");
            modelNames.Add("");

            dataGridView2.Rows.Clear();
            dataGridView2.Columns.Clear();
            int c1 = dataGridView2.Columns.Add("bm", modelNames[0]);
            int c2 = dataGridView2.Columns.Add("mp", modelNames[1]);
            int c3 = dataGridView2.Columns.Add("p1", modelNames[2]);
            int c4 = dataGridView2.Columns.Add("p2", modelNames[3]);
            int c5 = dataGridView2.Columns.Add("p3", modelNames[4]);

            DataGridViewCellStyle c2CellStyle = new DataGridViewCellStyle();
            c2CellStyle.BackColor = Color.LightYellow;
            DataGridViewCellStyle cellStyle = new DataGridViewCellStyle();
            cellStyle.BackColor = Color.White;
            DataGridViewCellStyle emptyCellStyle = new DataGridViewCellStyle();
            emptyCellStyle.BackColor = Color.LightGray;

            //dataGridView2.Columns[c1].DefaultCellStyle = emptyCellStyle;
            dataGridView2.Columns[c2].DefaultCellStyle = c2CellStyle;
            dataGridView2.Columns[c3].DefaultCellStyle = emptyCellStyle;
            dataGridView2.Columns[c4].DefaultCellStyle = emptyCellStyle;
            dataGridView2.Columns[c5].DefaultCellStyle = emptyCellStyle;

            R2ModelV2.MAX.R2Model baseModel = R2ModelV2.MAX.Factory.LoadModel(@"C:\Eclipse Workspace\ehrsfm_profile\HL7_FM_EA_Extension.Tests\InputFiles\EHRS_FM_R2_N2.max.xml");
            dataGridView2.Rows.Add(baseModel.elements.Count);

            ArrayList rowIds = loadBaseModel(c1, baseModel);
            loadData(c3, @"C:\My Documents\__R4C 2014\EHR-S FM FHFP Merge Project (2014-jun)\CDC Surveillance Compiled Profile.max", rowIds, cellStyle);
            loadData(c4, @"C:\My Documents\__R4C 2014\EHR-S FM FHFP Merge Project (2014-jun)\PHFP Vital Records compiled-profile.max.xml", rowIds, cellStyle);
            dummyData(c5, rowIds, cellStyle);

            updateStatistics();

            ShowDialog();
        }

        private ArrayList loadBaseModel(int columnNumber, R2ModelV2.MAX.R2Model baseModel)
        {
            modelNames[columnNumber] = baseModel.Name;
            dataGridView2.Columns[columnNumber].HeaderText = baseModel.Name;
            ArrayList rowIds = new ArrayList(baseModel.elements.Count);
            int rowNumber = 0;
            foreach (R2ModelElement element in baseModel.elements)
            {
                DataGridViewCell cell = dataGridView2.Rows[rowNumber].Cells[columnNumber];
                cell.Tag = element;
                cell.Value = element.GetId();
                if (element is R2Criterion)
                {
                    R2Criterion criterion = (R2Criterion) element;
                    cell.ToolTipText = criterion.Text;
                }
                else if (element is R2Function)
                {
                    R2Function function = (R2Function) element;
                    cell.ToolTipText = function.Name + "\n" + function.Description;
                }
                else if (element is R2Section)
                {
                    R2Section section = (R2Section) element;
                    cell.ToolTipText = section.Name;
                }
                rowIds.Add(element.GetId());
                rowNumber++;
            }
            return rowIds;
        }

        private void loadData(int columnNumber, string maxFileName, ArrayList rowIds, DataGridViewCellStyle cellStyle)
        {
            R2ModelV2.MAX.R2Model model = R2ModelV2.MAX.Factory.LoadModel(maxFileName);
            modelNames[columnNumber] = model.Name;
            dataGridView2.Columns[columnNumber].HeaderText = model.Name;
            foreach (R2ModelElement element in model.elements)
            {
                int rowNumber = rowIds.IndexOf(element.GetId());
                if (rowNumber == -1) continue;

                DataGridViewCell cell = dataGridView2.Rows[rowNumber].Cells[columnNumber];
                cell.Style = cellStyle;
                cell.Tag = element;
                cell.Value = element.GetId();
                if (element is R2Criterion)
                {
                    R2Criterion criterion = (R2Criterion)element;
                    cell.ToolTipText = criterion.Text;
                }
                else if (element is R2Function)
                {
                    R2Function function = (R2Function)element;
                    cell.ToolTipText = function.Name + "\n" + function.Description;
                }
                else if (element is R2Section)
                {
                    R2Section section = (R2Section)element;
                    cell.ToolTipText = section.Name;
                }
            }
        }

        private void dummyData(int columnNumber, ArrayList rowIds, DataGridViewCellStyle cellStyle)
        {
            modelNames[columnNumber] = "Dummy";
            dataGridView2.Columns[columnNumber].HeaderText = "Dummy";

            R2Section section = new R2Section { SectionId = "CP" };
            int rowNumber = rowIds.IndexOf(section.GetId());

            DataGridViewCell sectionCell = dataGridView2.Rows[rowNumber].Cells[columnNumber];
            sectionCell.Tag = section;
            sectionCell.Value = string.Format("{0}", section.SectionId);
            sectionCell.Style = cellStyle;
            for (int h = 1; h < 3; h++)
            {
                R2Function header = new R2Function { FunctionId = string.Format("CP.{0}", h) };
                rowNumber = rowIds.IndexOf(header.GetId());
                if (rowNumber == -1) continue;

                DataGridViewCell headerCell = dataGridView2.Rows[rowNumber].Cells[columnNumber];
                headerCell.Tag = header;
                headerCell.Value = header.FunctionId;
                headerCell.Style = cellStyle;
                for (int f = 1; f < 3; f++)
                {
                    R2Function function = new R2Function { FunctionId = string.Format("CP.{0}.{1}", h, f) };
                    rowNumber = rowIds.IndexOf(function.GetId());
                    if (rowNumber == -1) continue;

                    DataGridViewCell functionCell = dataGridView2.Rows[rowNumber].Cells[columnNumber];
                    functionCell.Tag = function;
                    functionCell.Value = function.FunctionId;
                    functionCell.Style = cellStyle;
                    for (int c = 1; c < 4; c++)
                    {
                        R2Criterion criterion = new R2Criterion
                                                    {
                                                        FunctionId = string.Format("CP.{0}.{1}", h, f), 
                                                        CriterionSeqNo = c,
                                                        Text = "The system SHALL xyz",
                                                        Optionality = "SHALL"
                                                    };
                        rowNumber = rowIds.IndexOf(criterion.GetId());
                        if (rowNumber == -1) continue;

                        DataGridViewCell criterionCell = dataGridView2.Rows[rowNumber].Cells[columnNumber];
                        criterionCell.Tag = criterion;
                        criterionCell.Value = criterion.Name;
                        criterionCell.ToolTipText = string.Format("{0} {1}", criterion.Name, criterion.Text);
                        criterionCell.Style = cellStyle;
                    }
                }
            }
        }

        private void populateSelectedSection(R2Section section, int rowNumber, DataGridViewCellStyle emptyCellStyle)
        {
            if (section != null)
            {
                dataGridView1.Rows[rowNumber].DefaultCellStyle = dataGridView1.DefaultCellStyle;
                dataGridView1.Rows[rowNumber].Cells["SectionId"].Value = section.SectionId;
                dataGridView1.Rows[rowNumber].Cells["Name"].Value = section.Name;
                dataGridView1.Rows[rowNumber].Cells["LastModified"].Value = section.LastModified;
            }
            else
            {
                dataGridView1.Rows[rowNumber].DefaultCellStyle = emptyCellStyle;
            }
        }

        private void populateSelectedFunction(R2Function function, int rowNumber, DataGridViewCellStyle emptyCellStyle, R2Function function0, DataGridViewCellStyle diffCellStyle)
        {
            if (function != null)
            {
                dataGridView1.Rows[rowNumber].DefaultCellStyle = dataGridView1.DefaultCellStyle;

                populateSelectedCell(dataGridView1.Rows[rowNumber].Cells["FunctionId"], function.FunctionId, function0.FunctionId, diffCellStyle);
                populateSelectedCell(dataGridView1.Rows[rowNumber].Cells["Name"], function.Name, function0.Name, diffCellStyle);
                populateSelectedCell(dataGridView1.Rows[rowNumber].Cells["LastModified"], function.LastModified, function0.LastModified, diffCellStyle);
            }
            else
            {
                dataGridView1.Rows[rowNumber].DefaultCellStyle = emptyCellStyle;
            }
        }

        private void populateSelectedCriterion(R2Criterion criterion, int rowNumber, DataGridViewCellStyle emptyCellStyle, R2Criterion criterion0, DataGridViewCellStyle diffCellStyle)
        {
            if (criterion != null)
            {
                dataGridView1.Rows[rowNumber].DefaultCellStyle = dataGridView1.DefaultCellStyle;
                populateSelectedCell(dataGridView1.Rows[rowNumber].Cells["Name"], criterion.Name, criterion0.Name, diffCellStyle);
                populateSelectedCell(dataGridView1.Rows[rowNumber].Cells["Text"], criterion.Text, criterion0.Text, diffCellStyle);
                populateSelectedCell(dataGridView1.Rows[rowNumber].Cells["Row#"], criterion.Row, criterion0.Row, diffCellStyle);
                populateSelectedCell(dataGridView1.Rows[rowNumber].Cells["Dependent"], criterion.Dependent, criterion0.Dependent, diffCellStyle);
                populateSelectedCell(dataGridView1.Rows[rowNumber].Cells["Conditional"], criterion.Conditional, criterion0.Conditional, diffCellStyle);
                populateSelectedCell(dataGridView1.Rows[rowNumber].Cells["Optionality"], criterion.Optionality, criterion0.Optionality, diffCellStyle);
                populateSelectedCell(dataGridView1.Rows[rowNumber].Cells["LastModified"], criterion.LastModified, criterion0.LastModified, diffCellStyle);
            }
            else
            {
                dataGridView1.Rows[rowNumber].DefaultCellStyle = emptyCellStyle;
            }
        }

        private void populateSelectedCell(DataGridViewCell cell, object value, object value0, DataGridViewCellStyle diffCellStyle)
        {
            cell.Value = value;
            if (value.Equals(value0))
            {
                cell.Style = null;
            }
            else
            {
                cell.Style = diffCellStyle;
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

        private void dataGridView2_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (_currentRow != e.RowIndex)
            {
                _currentRow = e.RowIndex;
                if (e.RowIndex == -1) return;

                // setup datagrid to selected Element type
                dataGridView1.Columns.Clear();
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

                DataGridViewCell cell0 = dataGridView2.Rows[e.RowIndex].Cells[0];
                if (cell0.Tag is R2Section)
                {
                    dataGridView1.Columns.Add("SectionId", "SectionId");
                    dataGridView1.Columns.Add("Name", "Name");
                    dataGridView1.Columns.Add("LastModified", "LastModified");
                }
                else if (cell0.Tag is R2Function)
                {
                    dataGridView1.Columns.Add("FunctionId", "FunctionId");
                    dataGridView1.Columns.Add("Name", "Name");
                    dataGridView1.Columns.Add("LastModified", "LastModified");
                }
                else if (cell0.Tag is R2Criterion)
                {
                    dataGridView1.Columns.Add("Name", "Name");
                    dataGridView1.Columns.Add("Row#", "Row#");
                    dataGridView1.Columns.Add("Dependent", "Dependent");
                    dataGridView1.Columns.Add("Conditional", "Conditional");
                    dataGridView1.Columns.Add("Optionality", "Optionality");
                    dataGridView1.Columns.Add("Text", "Text");
                    dataGridView1.Columns.Add("LastModified", "LastModified");
                }

                dataGridView1.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
                dataGridView1.Rows.Add(5);
                for (int rowNumber=0; rowNumber<modelNames.Count; rowNumber++)
                {
                    dataGridView1.Rows[rowNumber].HeaderCell.Value = modelNames[rowNumber];
                }

                // populate cells
                DataGridViewCell cell1 = dataGridView2.Rows[e.RowIndex].Cells[1];
                DataGridViewCell cell2 = dataGridView2.Rows[e.RowIndex].Cells[2];
                DataGridViewCell cell3 = dataGridView2.Rows[e.RowIndex].Cells[3];
                DataGridViewCell cell4 = dataGridView2.Rows[e.RowIndex].Cells[4];

                DataGridViewCellStyle emptyCellStyle = new DataGridViewCellStyle();
                emptyCellStyle.BackColor = Color.LightGray;
                DataGridViewCellStyle c2CellStyle = new DataGridViewCellStyle();
                c2CellStyle.BackColor = Color.LightYellow;
                DataGridViewCellStyle diffCellStyle = new DataGridViewCellStyle();
                diffCellStyle.BackColor = Color.LightGreen;

                if (cell0.Tag is R2Section)
                {
                    populateSelectedSection((R2Section)cell0.Tag, 0, emptyCellStyle);
                    populateSelectedSection((R2Section)cell1.Tag, 1, c2CellStyle);
                    populateSelectedSection((R2Section)cell2.Tag, 2, emptyCellStyle);
                    populateSelectedSection((R2Section)cell3.Tag, 3, emptyCellStyle);
                    populateSelectedSection((R2Section)cell4.Tag, 4, emptyCellStyle);
                }
                else if (cell0.Tag is R2Function)
                {
                    R2Function function = (R2Function)cell0.Tag;
                    populateSelectedFunction(function, 0, emptyCellStyle, function, diffCellStyle);
                    populateSelectedFunction((R2Function)cell1.Tag, 1, c2CellStyle, function, diffCellStyle);
                    populateSelectedFunction((R2Function)cell2.Tag, 2, emptyCellStyle, function, diffCellStyle);
                    populateSelectedFunction((R2Function)cell3.Tag, 3, emptyCellStyle, function, diffCellStyle);
                    populateSelectedFunction((R2Function)cell4.Tag, 4, emptyCellStyle, function, diffCellStyle);
                }
                else if (cell0.Tag is R2Criterion)
                {
                    R2Criterion criterion = (R2Criterion)cell0.Tag;
                    populateSelectedCriterion(criterion, 0, emptyCellStyle, criterion, diffCellStyle);
                    populateSelectedCriterion((R2Criterion)cell1.Tag, 1, c2CellStyle, criterion, diffCellStyle);
                    populateSelectedCriterion((R2Criterion)cell2.Tag, 2, emptyCellStyle, criterion, diffCellStyle);
                    populateSelectedCriterion((R2Criterion)cell3.Tag, 3, emptyCellStyle, criterion, diffCellStyle);
                    populateSelectedCriterion((R2Criterion)cell4.Tag, 4, emptyCellStyle, criterion, diffCellStyle);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            for (int rowNumber=dataGridView2.Rows.Count-1; rowNumber>=0; rowNumber--)
            {
                int cellsWithTag = 0;
                for (int colNum=0; colNum<dataGridView2.Rows[rowNumber].Cells.Count;colNum++)
                {
                    if (dataGridView2.Rows[rowNumber].Cells[colNum].Tag != null)
                    {
                        cellsWithTag++;
                    }
                }
                if (cellsWithTag < 2)
                {
                    dataGridView2.Rows.RemoveAt(rowNumber);
                }
            }
            updateStatistics();
        }

        private void updateStatistics()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Rows: ").Append(dataGridView2.Rows.Count);
            statsTextBox.Text = sb.ToString();
        }
    }
}