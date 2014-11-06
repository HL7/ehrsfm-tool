using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using HL7_FM_EA_Extension.R2ModelV2.Base;
using MAX_EA.MAXSchema;

namespace HL7_FM_EA_Extension
{
    public partial class MergeProfilesForm : Form
    {
        public MergeProfilesForm()
        {
            InitializeComponent();
        }

        private string fileNameBaseModel = @"C:\Temp\EHRS_FM_R2_N2.max.xml";
        private string fileNameMerged = @"C:\Temp\merged-profile-definition.max";
        private string fileNameProfile1 = "";
        private string fileNameProfile2 = "";
        private string fileNameProfile3 = "";
        private int _currentRow = -1;
        private int _currentCompareRow = -1;
        const int COLUMN_BASE_MODEL = 0;
        const int COLUMN_MERGED_PROFILE = 1;
        private List<string> modelNames = new List<string>();
        private R2Model baseModel;

        public void PopulateAndShow()
        {
            modelNames.Clear();
            modelNames.Add("Base Model");
            modelNames.Add("Merged Profile");
            modelNames.Add("");
            modelNames.Add("");
            modelNames.Add("");

            dataGridView2.Rows.Clear();
            dataGridView2.Columns.Clear();
            dataGridView2.Columns.Add("bm", "");
            dataGridView2.Columns.Add("mp", "Merged Profile");
            dataGridView2.Columns.Add("p1", "");
            dataGridView2.Columns.Add("p2", "");
            dataGridView2.Columns.Add("p3", "");

            DataGridViewCellStyle c2CellStyle = new DataGridViewCellStyle();
            c2CellStyle.BackColor = Color.LightYellow;
            DataGridViewCellStyle cellStyle = new DataGridViewCellStyle();
            cellStyle.BackColor = Color.White;
            DataGridViewCellStyle emptyCellStyle = new DataGridViewCellStyle();
            emptyCellStyle.BackColor = Color.LightGray;

            //dataGridView2.Columns[0].DefaultCellStyle = emptyCellStyle;
            dataGridView2.Columns[1].DefaultCellStyle = c2CellStyle;
            dataGridView2.Columns[2].DefaultCellStyle = emptyCellStyle;
            dataGridView2.Columns[3].DefaultCellStyle = emptyCellStyle;
            dataGridView2.Columns[4].DefaultCellStyle = emptyCellStyle;

            baseModel = R2ModelV2.MAX.Factory.LoadModel(fileNameBaseModel, true);
            dataGridView2.Rows.Add(baseModel.children.Count);

            PopulateBaseModelColumn(baseModel);
            LoadDummyProfile(4, cellStyle);
            UpdateStatistics();

            ShowDialog();
        }

        private int getBaseModelRowNumber(string id)
        {
            for(int rowNumber=0; rowNumber<dataGridView2.Rows.Count; rowNumber++)
            {
                DataGridViewCell cell = dataGridView2.Rows[rowNumber].Cells[COLUMN_BASE_MODEL];
                if (cell.Tag != null && id.Equals(((R2ModelElement)cell.Tag).GetExtId()))
                {
                    return rowNumber;
                }
            }
            return -1;
        }

        private void PopulateBaseModelColumn(R2Model baseModel)
        {
            modelNames[COLUMN_BASE_MODEL] = baseModel.Name;
            dataGridView2.Columns[COLUMN_BASE_MODEL].HeaderText = baseModel.Name;
            int rowNumber = 0;
            foreach (R2ModelElement element in baseModel.children)
            {
                DataGridViewCell cell = dataGridView2.Rows[rowNumber].Cells[COLUMN_BASE_MODEL];
                cell.Tag = element;
                cell.Value = element.GetExtId();
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
                rowNumber++;
            }
        }

        private void LoadProfile(int columnNumber, string maxFileName, DataGridViewCellStyle cellStyle)
        {
            R2Model model = R2ModelV2.MAX.Factory.LoadModel(maxFileName, true);
            modelNames[columnNumber] = model.Name;
            dataGridView2.Columns[columnNumber].HeaderText = model.Name;
            foreach (R2ModelElement element in model.children)
            {
                int rowNumber = getBaseModelRowNumber(element.GetAlignId());
                if (rowNumber == -1)
                {
                    // Base Model doesnot have this row, add Row at end
                    rowNumber = dataGridView2.Rows.Add();
                }

                DataGridViewCell cell = dataGridView2.Rows[rowNumber].Cells[columnNumber];
                cell.Style = cellStyle;
                cell.Tag = element;
                cell.Value = element.GetExtId();
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

        private void LoadDummyProfile(int columnNumber, DataGridViewCellStyle cellStyle)
        {
            modelNames[columnNumber] = "Dummy";
            dataGridView2.Columns[columnNumber].HeaderText = "Dummy";

            R2Section section = new R2Section { SectionId = "CP", IsReadOnly = true };
            int rowNumber = getBaseModelRowNumber(section.GetAlignId());

            DataGridViewCell sectionCell = dataGridView2.Rows[rowNumber].Cells[columnNumber];
            sectionCell.Tag = section;
            sectionCell.Value = string.Format("{0}", section.SectionId);
            sectionCell.Style = cellStyle;
            for (int h = 1; h < 3; h++)
            {
                R2Function header = new R2Function { FunctionId = string.Format("CP.{0}", h), IsReadOnly = true };
                rowNumber = getBaseModelRowNumber(header.GetAlignId());
                if (rowNumber == -1)
                {
                    // Base Model doesnot have this row, add Row at end
                    rowNumber = dataGridView2.Rows.Add();
                }

                DataGridViewCell headerCell = dataGridView2.Rows[rowNumber].Cells[columnNumber];
                headerCell.Tag = header;
                headerCell.Value = header.FunctionId;
                headerCell.Style = cellStyle;
                for (int f = 1; f < 3; f++)
                {
                    R2Function function = new R2Function { FunctionId = string.Format("CP.{0}.{1}", h, f), IsReadOnly = true };
                    rowNumber = getBaseModelRowNumber(function.GetAlignId());
                    if (rowNumber == -1)
                    {
                        // Base Model doesnot have this row, add Row at end
                        rowNumber = dataGridView2.Rows.Add();
                    }

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
                                                        Optionality = "SHALL",
                                                        IsReadOnly = true
                                                    };
                        criterion.SetRefId(null, string.Format("CP.{0}.{1}", h, f), string.Format("{0}", c+1));
                        rowNumber = getBaseModelRowNumber(criterion.GetAlignId());
                        if (rowNumber == -1)
                        {
                            // Base Model doesnot have this row, add Row at end
                            rowNumber = dataGridView2.Rows.Add();
                        }

                        DataGridViewCell criterionCell = dataGridView2.Rows[rowNumber].Cells[columnNumber];
                        criterionCell.Tag = criterion;
                        criterionCell.Value = string.Format("! {0}", criterion.Name);
                        criterionCell.ToolTipText = criterion.Text;
                        criterionCell.Style = cellStyle;
                    }
                }
            }
        }

        private void ClearColumn(int colNum)
        {
            // Clear Column
            for (int rowNumber = dataGridView2.Rows.Count - 1; rowNumber >= 0; rowNumber--)
            {
                dataGridView2.Rows[rowNumber].Cells[colNum].Style = null;
                dataGridView2.Rows[rowNumber].Cells[colNum].Value = "";
                dataGridView2.Rows[rowNumber].Cells[colNum].Tag = null;
            }
        }

        private void LoadProfileDefinition(string maxFileName)
        {
            R2ProfileDefinition model = R2ModelV2.MAX.Factory.LoadProfileDefinition(baseModel, maxFileName);
            modelNames[COLUMN_MERGED_PROFILE] = model.Name;
            dataGridView2.Columns[COLUMN_MERGED_PROFILE].HeaderText = model.Name;
            foreach (R2ModelElement element in model.children)
            {
                int rowNumber = getBaseModelRowNumber(element.GetAlignId());
                if (rowNumber == -1)
                {
                    // Base Model doesnot have this row, add Row at end
                    rowNumber = dataGridView2.Rows.Add();
                }

                DataGridViewCell cell = dataGridView2.Rows[rowNumber].Cells[COLUMN_MERGED_PROFILE];
                cell.Tag = element;
                cell.Value = element.GetExtId();
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

        private void populateSelectedSection(R2Section section, int rowNumber, DataGridViewCellStyle emptyCellStyle)
        {
            if (section != null)
            {
                dataGridView1.Rows[rowNumber].DefaultCellStyle = dataGridView1.DefaultCellStyle;
                dataGridView1.Rows[rowNumber].Cells["Priority"].Value = section.Priority;
                dataGridView1.Rows[rowNumber].Cells["SectionId"].Value = section.SectionId;
                dataGridView1.Rows[rowNumber].Cells["Name"].Value = section.Name;
                dataGridView1.Rows[rowNumber].Cells["LastModified"].Value = section.LastModified;
            }
            else
            {
                dataGridView1.Rows[rowNumber].DefaultCellStyle = emptyCellStyle;
            }
        }

        private void populateSelectedFunction(R2Function function, int rowNumber, DataGridViewCellStyle emptyCellStyle, R2Function compareFunction, DataGridViewCellStyle diffCellStyle)
        {
            if (compareFunction == null) compareFunction = function;
            if (function != null)
            {
                dataGridView1.Rows[rowNumber].DefaultCellStyle = dataGridView1.DefaultCellStyle;
                populateSelectedCell(dataGridView1.Rows[rowNumber].Cells["Priority"], function.Priority, compareFunction.Priority, diffCellStyle);
                populateSelectedCell(dataGridView1.Rows[rowNumber].Cells["FunctionId"], function.FunctionId, compareFunction.FunctionId, diffCellStyle);
                populateSelectedCell(dataGridView1.Rows[rowNumber].Cells["Name"], function.Name, compareFunction.Name, diffCellStyle);
                populateSelectedCell(dataGridView1.Rows[rowNumber].Cells["LastModified"], function.LastModified, compareFunction.LastModified, diffCellStyle);
            }
            else
            {
                dataGridView1.Rows[rowNumber].DefaultCellStyle = emptyCellStyle;
            }
        }

        private void populateSelectedCriterion(R2Criterion criterion, int rowNumber, DataGridViewCellStyle emptyCellStyle, R2Criterion compareCriterion, DataGridViewCellStyle diffCellStyle)
        {
            if (compareCriterion == null) compareCriterion = criterion;
            if (criterion != null)
            {
                dataGridView1.Rows[rowNumber].DefaultCellStyle = dataGridView1.DefaultCellStyle;
                populateSelectedCell(dataGridView1.Rows[rowNumber].Cells["Priority"], criterion.Priority, compareCriterion.Priority, diffCellStyle);
                populateSelectedCell(dataGridView1.Rows[rowNumber].Cells["Name"], criterion.Name, compareCriterion.Name, diffCellStyle);
                populateSelectedCell(dataGridView1.Rows[rowNumber].Cells["Text"], criterion.Text, compareCriterion.Text, diffCellStyle);
                populateSelectedCell(dataGridView1.Rows[rowNumber].Cells["Row#"], criterion.Row, compareCriterion.Row, diffCellStyle);
                populateSelectedCell(dataGridView1.Rows[rowNumber].Cells["Dependent"], criterion.Dependent, compareCriterion.Dependent, diffCellStyle);
                populateSelectedCell(dataGridView1.Rows[rowNumber].Cells["Conditional"], criterion.Conditional, compareCriterion.Conditional, diffCellStyle);
                populateSelectedCell(dataGridView1.Rows[rowNumber].Cells["Optionality"], criterion.Optionality, compareCriterion.Optionality, diffCellStyle);
                populateSelectedCell(dataGridView1.Rows[rowNumber].Cells["LastModified"], criterion.LastModified, compareCriterion.LastModified, diffCellStyle);
            }
            else
            {
                dataGridView1.Rows[rowNumber].DefaultCellStyle = emptyCellStyle;
            }
        }

        private void populateSelectedCell(DataGridViewCell cell, object value, object compareValue, DataGridViewCellStyle diffCellStyle)
        {
            cell.Value = value;
            if (value.Equals(compareValue))
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
            if (e.RowIndex == -1)
            {
                // Only popup file selection for Base Model and Profiles
                // There are separate buttons for MergedProfile Definition (== column 1)
                string defaultFileName = null;
                switch (e.ColumnIndex)
                {
                    case COLUMN_BASE_MODEL:
                        defaultFileName = fileNameBaseModel;
                        break;
                    case COLUMN_MERGED_PROFILE:
                        defaultFileName = fileNameMerged;
                        break;
                    case 2:
                        defaultFileName = fileNameProfile1;
                        break;
                    case 3:
                        defaultFileName = fileNameProfile2;
                        break;
                    case 4:
                        defaultFileName = fileNameProfile3;
                        break;
                }
                if (defaultFileName != null)
                {
                    string fileName = FileUtil.showFileDialog("Select input MAX XML file",
                                                              "max files (*.xml, *.max)|*.xml;*.max", defaultFileName,
                                                              true);
                    if (!string.IsNullOrEmpty(fileName))
                    {
                        Cursor = Cursors.WaitCursor;
                        DataGridViewCellStyle cellStyle = new DataGridViewCellStyle();
                        cellStyle.BackColor = Color.White;
                        switch (e.ColumnIndex)
                        {
                            case COLUMN_BASE_MODEL:
                                fileNameBaseModel = fileName;
                                MessageBox.Show("Not yet implemented");
                                break;
                            case COLUMN_MERGED_PROFILE:
                                fileNameMerged = fileName;
                                ClearColumn(COLUMN_MERGED_PROFILE);
                                LoadProfileDefinition(fileNameMerged);
                                break;
                            case 2:
                                fileNameProfile1 = fileName;
                                ClearColumn(e.ColumnIndex);
                                LoadProfile(e.ColumnIndex, fileName, cellStyle);
                                break;
                            case 3:
                                fileNameProfile2 = fileName;
                                ClearColumn(e.ColumnIndex);
                                LoadProfile(e.ColumnIndex, fileName, cellStyle);
                                break;
                            case 4:
                                fileNameProfile3 = fileName;
                                ClearColumn(e.ColumnIndex);
                                LoadProfile(e.ColumnIndex, fileName, cellStyle);
                                break;
                        }
                        UpdateStatistics();
                        Cursor = Cursors.Default;
                    }
                }
            }
            else
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
                // Update content of compare grid, content might be changed in the form
                updateDataGridView1(false, _currentRow, _currentCompareRow);
            }
        }

        private void updateDataGridView1(bool reset, int selectedRow, int compareRow)
        {
            DataGridViewCell cell0 = dataGridView2.Rows[selectedRow].Cells[0];
            DataGridViewCell cell1 = dataGridView2.Rows[selectedRow].Cells[1];
            DataGridViewCell cell2 = dataGridView2.Rows[selectedRow].Cells[2];
            DataGridViewCell cell3 = dataGridView2.Rows[selectedRow].Cells[3];
            DataGridViewCell cell4 = dataGridView2.Rows[selectedRow].Cells[4];

            // Get type of rows by the cell type. Some rows have no base, so then we use the profile.
            DataGridViewCell typeCell = cell0;
            if (typeCell.Tag == null) typeCell = cell1;
            if (typeCell.Tag == null) typeCell = cell2;
            if (typeCell.Tag == null) typeCell = cell3;
            if (typeCell.Tag == null) typeCell = cell4;
            if (typeCell.Tag == null)
            {
                // Happens when a row is empty, e.g. by clearing
                return;
            }

            if (reset)
            {
                // setup datagrid to selected Element type
                dataGridView1.Columns.Clear();
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

                // Create columns based on element type of the cell
                if (typeCell.Tag is R2Section)
                {
                    dataGridView1.Columns.Add("Priority", "Priority");
                    dataGridView1.Columns.Add("SectionId", "SectionId");
                    dataGridView1.Columns.Add("Name", "Name");
                    dataGridView1.Columns.Add("LastModified", "LastModified");
                }
                else if (typeCell.Tag is R2Function)
                {
                    dataGridView1.Columns.Add("Priority", "Priority");
                    dataGridView1.Columns.Add("FunctionId", "FunctionId");
                    dataGridView1.Columns.Add("Name", "Name");
                    dataGridView1.Columns.Add("LastModified", "LastModified");
                }
                else if (typeCell.Tag is R2Criterion)
                {
                    dataGridView1.Columns.Add("Priority", "Priority");
                    dataGridView1.Columns.Add("Name", "Name");
                    dataGridView1.Columns.Add("Optionality", "Optionality");
                    dataGridView1.Columns.Add("Text", "Text");
                    dataGridView1.Columns.Add("Dependent", "Dependent");
                    dataGridView1.Columns.Add("Conditional", "Conditional");
                    dataGridView1.Columns.Add("Row#", "Row#");
                    dataGridView1.Columns.Add("LastModified", "LastModified");
                }

                dataGridView1.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
                dataGridView1.Rows.Add(5);
                for (int rowNumber = 0; rowNumber < modelNames.Count; rowNumber++)
                {
                    dataGridView1.Rows[rowNumber].HeaderCell.Value = modelNames[rowNumber];
                }
            }

            // populate cells
            DataGridViewCellStyle emptyCellStyle = new DataGridViewCellStyle();
            emptyCellStyle.BackColor = Color.LightGray;
            DataGridViewCellStyle c2CellStyle = new DataGridViewCellStyle();
            c2CellStyle.BackColor = Color.LightYellow;
            DataGridViewCellStyle diffCellStyle = new DataGridViewCellStyle();
            diffCellStyle.BackColor = Color.LightGreen;

            R2ModelElement compareElement;
            switch (compareRow)
            {
                case 1:
                    compareElement = (R2ModelElement)cell1.Tag;
                    break;
                case 2:
                    compareElement = (R2ModelElement)cell2.Tag;
                    break;
                case 3:
                    compareElement = (R2ModelElement)cell3.Tag;
                    break;
                case 4:
                    compareElement = (R2ModelElement)cell4.Tag;
                    break;
                default:
                    compareElement = (R2ModelElement)cell0.Tag;
                    break;
            }

            if (typeCell.Tag is R2Section)
            {
                populateSelectedSection((R2Section)cell0.Tag, 0, emptyCellStyle);
                populateSelectedSection((R2Section)cell1.Tag, 1, c2CellStyle);
                populateSelectedSection((R2Section)cell2.Tag, 2, emptyCellStyle);
                populateSelectedSection((R2Section)cell3.Tag, 3, emptyCellStyle);
                populateSelectedSection((R2Section)cell4.Tag, 4, emptyCellStyle);
            }
            else if (typeCell.Tag is R2Function)
            {
                R2Function compareFunction = (R2Function)compareElement;
                populateSelectedFunction((R2Function)cell0.Tag, 0, emptyCellStyle, compareFunction, diffCellStyle);
                populateSelectedFunction((R2Function)cell1.Tag, 1, c2CellStyle, compareFunction, diffCellStyle);
                populateSelectedFunction((R2Function)cell2.Tag, 2, emptyCellStyle, compareFunction, diffCellStyle);
                populateSelectedFunction((R2Function)cell3.Tag, 3, emptyCellStyle, compareFunction, diffCellStyle);
                populateSelectedFunction((R2Function)cell4.Tag, 4, emptyCellStyle, compareFunction, diffCellStyle);
            }
            else if (typeCell.Tag is R2Criterion)
            {
                R2Criterion compareCriterion = (R2Criterion)compareElement;
                populateSelectedCriterion((R2Criterion)cell0.Tag, 0, emptyCellStyle, compareCriterion, diffCellStyle);
                populateSelectedCriterion((R2Criterion)cell1.Tag, 1, c2CellStyle, compareCriterion, diffCellStyle);
                populateSelectedCriterion((R2Criterion)cell2.Tag, 2, emptyCellStyle, compareCriterion, diffCellStyle);
                populateSelectedCriterion((R2Criterion)cell3.Tag, 3, emptyCellStyle, compareCriterion, diffCellStyle);
                populateSelectedCriterion((R2Criterion)cell4.Tag, 4, emptyCellStyle, compareCriterion, diffCellStyle);
            }
        }

        private void dataGridView2_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (_currentRow != e.RowIndex)
            {
                _currentRow = e.RowIndex;
                if (e.RowIndex == -1) return;
                _currentCompareRow = dataGridView2.CurrentCell.ColumnIndex;
                updateDataGridView1(true, _currentRow, _currentCompareRow);
            }
        }

        private void dataGridView1_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (_currentCompareRow != e.RowIndex)
            {
                _currentCompareRow = e.RowIndex;
                if (e.RowIndex == -1) return;
                updateDataGridView1(false, _currentRow, _currentCompareRow);
            }
        }

        // Hide rows that are profiled
        private void button1_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            for (int rowNumber = dataGridView2.Rows.Count - 1; rowNumber >= 0; rowNumber--)
            {
                if (dataGridView2.Rows[rowNumber].Cells[1].Tag != null)
                {
                    dataGridView2.Rows[rowNumber].Visible = false;
                }
            }
            UpdateStatistics();
            Cursor = Cursors.Default;
        }

        // Show rows that are profiled
        private void button2_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            for (int rowNumber = dataGridView2.Rows.Count - 1; rowNumber >= 0; rowNumber--)
            {
                if (dataGridView2.Rows[rowNumber].Cells[1].Tag != null)
                {
                    dataGridView2.Rows[rowNumber].Visible = true;
                }
            }
            UpdateStatistics();
            Cursor = Cursors.Default;
        }

        // Hide rows that have no profiled element
        private void collapseButton_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            for (int rowNumber = dataGridView2.Rows.Count - 1; rowNumber >= 0; rowNumber--)
            {
                int cellsWithTag = 0;
                // Ignore BaseModel and Merged
                for (int colNum=2; colNum<dataGridView2.Rows[rowNumber].Cells.Count;colNum++)
                {
                    if (dataGridView2.Rows[rowNumber].Cells[colNum].Tag != null)
                    {
                        cellsWithTag++;
                    }
                }
                if (cellsWithTag < 1)
                {
                    dataGridView2.Rows[rowNumber].Visible = false;
                }
            }
            UpdateStatistics();
            Cursor = Cursors.Default;
        }

        // Show all rows
        private void button3_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            for (int rowNumber = dataGridView2.Rows.Count - 1; rowNumber >= 0; rowNumber--)
            {
                dataGridView2.Rows[rowNumber].Visible = true;
            }
            UpdateStatistics();
            Cursor = Cursors.Default;
        }

        private void UpdateStatistics()
        {
            int rowCount = dataGridView2.Rows.Cast<DataGridViewRow>().Count();
            int showingCount = dataGridView2.Rows.Cast<DataGridViewRow>().Count(r => r.Visible);
            int hiddenCount = dataGridView2.Rows.Cast<DataGridViewRow>().Count(r => !r.Visible);
            int mergedCount = dataGridView2.Rows.Cast<DataGridViewRow>().Count(r => r.Cells[1].Tag != null);

            StringBuilder sb = new StringBuilder();
            sb.Append("Rows: ").Append(rowCount).AppendLine();
            sb.Append("Showing: ").Append(showingCount).AppendLine();
            sb.Append("Hidden: ").Append(hiddenCount).AppendLine();
            sb.Append("Merged: ").Append(mergedCount).AppendLine();
            statsTextBox.Text = sb.ToString();
        }

        private void selectButton_Click(object sender, EventArgs e)
        {
            if (dataGridView2.CurrentCell.ColumnIndex < 2)
            {
                // Ignore, cannot select FM or Merged Profile itself
                return;
            }
            DataGridViewCell currentCell = dataGridView2.CurrentCell;
            if (currentCell.Tag == null)
            {
                // Nothing to do, select an empty cell.
                return;
            }
            dataGridView2.CurrentRow.Cells[1].Value = currentCell.Value;

            R2ModelElement compilerInstruction = R2ModelV2.MAX.Factory.CreateModelElement((R2ModelElement)currentCell.Tag, (R2ModelElement)dataGridView2.CurrentRow.Cells[0].Tag);
            DataGridViewCell cell = dataGridView2.CurrentRow.Cells[1];
            cell.Tag = compilerInstruction;
            if (compilerInstruction is R2Criterion)
            {
                R2Criterion criterion = (R2Criterion)compilerInstruction;
                cell.ToolTipText = criterion.Text;
            }
            else if (compilerInstruction is R2Function)
            {
                R2Function function = (R2Function)compilerInstruction;
                cell.ToolTipText = function.Name + "\n" + function.Description;
            }
            else if (compilerInstruction is R2Section)
            {
                R2Section section = (R2Section)compilerInstruction;
                cell.ToolTipText = section.Name;
            }
            UpdateStatistics();
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            DataGridViewCell cell = dataGridView2.CurrentRow.Cells[1];
            cell.Value = "";
            cell.Tag = null;
            cell.ToolTipText = null;
            UpdateStatistics();
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            string fileNameOutput = FileUtil.showFileDialog("Select Profile Definition MAX XML file", "max files (*.xml, *.max)|*.xml;*.max", fileNameMerged, false);
            if (string.IsNullOrEmpty(fileNameOutput))
            {
                // Save canceled
                return;
            }

            List<ObjectType> objects = new List<ObjectType>();
            List<RelationshipType> relationships = new List<RelationshipType>();

            // Create Profile Definition Object
            R2ProfileDefinition profileDef = new R2ModelV2.MAX.R2ProfileDefinition
            {
                Name = "Merged Profile",
                Type = "Merged",
                Version = "1.0",
                LanguageTag = "en-EN",
                Rationale = " ",
                Scope = " ",
                PrioDef = " ",
                ConfClause = " ",
                LastModified = Util.FormatLastModified(DateTime.Now)
            };
            profileDef.SaveToSource();
            string defId = profileDef.Id;
            ObjectType maxDefObj = (ObjectType)profileDef.SourceObject;
            maxDefObj.SetTagValue("MAX::ExportDate", Util.FormatLastModified(DateTime.Now));
            maxDefObj.SetTagValue("MAX::ExportFile", fileNameOutput);
            objects.Add(maxDefObj);

            foreach(DataGridViewRow row in dataGridView2.Rows)
            {
                R2ModelElement element = (R2ModelElement)row.Cells[1].Tag;
                if (element != null)
                {
                    element.SaveToSource();
                    ObjectType maxObj = (ObjectType) element.SourceObject;
                    maxObj.id = Guid.NewGuid().ToString();
                    maxObj.parentId = defId;
                    objects.Add(maxObj);

                    // Only create Generalization Relationship if this is a Compiler Instruction
                    if (element.BaseElement != null)
                    {
                        RelationshipType maxRel = new RelationshipType();
                        maxRel.sourceId = maxObj.id;
                        maxRel.destId = ((ObjectType) element.BaseElement.SourceObject).id;
                        maxRel.type = RelationshipTypeEnum.Generalization;
                        maxRel.typeSpecified = true;
                        relationships.Add(maxRel);
                    }
                }
            }

            // Convert to MAX model
            ModelType model = new ModelType();
            model.exportDate = Util.FormatLastModified(DateTime.Now);
            model.objects = objects.ToArray();
            model.relationships = relationships.ToArray();

            // Save Merged profile definition as MAX XML
            XmlSerializer serializer = new XmlSerializer(typeof(ModelType));
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.NewLineChars = "\n";
            using (XmlWriter writer = XmlWriter.Create(fileNameOutput, settings))
            {
                serializer.Serialize(writer, model);
            }
            MessageBox.Show("Created Profile Definition MAX file.");
        }

        private void loadButton_Click(object sender, EventArgs e)
        {
            string fileNameInput = FileUtil.showFileDialog("Select Profile Definition MAX XML file", "max files (*.xml, *.max)|*.xml;*.max", fileNameMerged, true);
            if (string.IsNullOrEmpty(fileNameInput))
            {
                // Load canceled
                return;
            }
            Cursor = Cursors.WaitCursor;
            ClearColumn(COLUMN_MERGED_PROFILE);
            LoadProfileDefinition(fileNameInput);
            UpdateStatistics();
            Cursor = Cursors.Default;
        }
    }
}
