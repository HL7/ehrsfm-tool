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
        private R2Model baseModel;

        public void PopulateAndShow()
        {
            toolTip1.SetToolTip(button1, "Will hide rows with an\nelement in the Merged Profile.");
            toolTip1.SetToolTip(button2, "Will show all rows with an\nelement in the Merged Profile.");
            toolTip1.SetToolTip(button3, "Show all rows.");

            modelsDataGridView.Rows.Clear();
            modelsDataGridView.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            modelsDataGridView.RowHeadersWidth = 24;
            modelsDataGridView.Columns.Clear();
            modelsDataGridView.Columns.Add("bm", "");
            modelsDataGridView.Columns.Add("mp", "");
            modelsDataGridView.Columns.Add("p1", "");
            modelsDataGridView.Columns.Add("p2", "");
            modelsDataGridView.Columns.Add("p3", "");
            // Disable sorting on header for now
            modelsDataGridView.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            modelsDataGridView.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            modelsDataGridView.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
            modelsDataGridView.Columns[3].SortMode = DataGridViewColumnSortMode.NotSortable;
            modelsDataGridView.Columns[4].SortMode = DataGridViewColumnSortMode.NotSortable;

            DataGridViewCellStyle c2CellStyle = new DataGridViewCellStyle {BackColor = Color.LightYellow};
            DataGridViewCellStyle cellStyle = new DataGridViewCellStyle {BackColor = Color.White};
            DataGridViewCellStyle emptyCellStyle = new DataGridViewCellStyle {BackColor = Color.LightGray};

            modelsDataGridView.Columns[1].DefaultCellStyle = c2CellStyle;
            modelsDataGridView.Columns[2].DefaultCellStyle = emptyCellStyle;
            modelsDataGridView.Columns[3].DefaultCellStyle = emptyCellStyle;
            modelsDataGridView.Columns[4].DefaultCellStyle = emptyCellStyle;
            ShowDialog();
        }

        private int getBaseModelRowNumber(string id)
        {
            for(int rowNumber=0; rowNumber<modelsDataGridView.Rows.Count; rowNumber++)
            {
                DataGridViewCell cell = modelsDataGridView.Rows[rowNumber].Cells[COLUMN_BASE_MODEL];
                if (cell.Tag != null && id.Equals(((R2ModelElement)cell.Tag).GetExtId()))
                {
                    return rowNumber;
                }
            }
            return -1;
        }

        private void PopulateBaseModelColumn(R2Model baseModel)
        {
            modelsDataGridView.Columns[COLUMN_BASE_MODEL].HeaderText = baseModel.Name;
            modelsDataGridView.Columns[COLUMN_BASE_MODEL].Tag = baseModel;
            int rowNumber = 0;
            foreach (R2ModelElement element in baseModel.children)
            {
                DataGridViewCell cell = modelsDataGridView.Rows[rowNumber].Cells[COLUMN_BASE_MODEL];
                cell.Style = new DataGridViewCellStyle { BackColor = R2Config.config.getSectionColor(element.GetExtId(), Color.White) };
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
            modelsDataGridView.Columns[columnNumber].HeaderText = model.Name;
            modelsDataGridView.Columns[columnNumber].Tag = model;
            foreach (R2ModelElement element in model.children)
            {
                int rowNumber = getBaseModelRowNumber(element.GetAlignId());
                if (rowNumber == -1)
                {
                    // Base Model doesnot have this row, add Row at end
                    rowNumber = modelsDataGridView.Rows.Add();
                }

                DataGridViewCell cell = modelsDataGridView.Rows[rowNumber].Cells[columnNumber];
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

        private void LoadBaseModelAndNewMergedProfile()
        {
            baseModel = R2ModelV2.MAX.Factory.LoadModel(fileNameBaseModel, true);
            modelsDataGridView.Rows.Clear();
            modelsDataGridView.Rows.Add(baseModel.children.Count);

            // Create initial Profile Definition Object
            R2ProfileDefinition profileDef = new R2ModelV2.MAX.R2ProfileDefinition
            {
                Name = "Merged Profile",
                Type = "Merged",
                Version = "1.0",
                LanguageTag = "en-EN",
                Rationale = "",
                Scope = "",
                PrioDef = "",
                ConfClause = "",
                LastModified = Util.FormatLastModified(DateTime.Now),
                BaseModelName = baseModel.Name
            };
            profileDef.SaveToSource();
            modelsDataGridView.Columns[COLUMN_MERGED_PROFILE].HeaderText = profileDef.Name;
            modelsDataGridView.Columns[COLUMN_MERGED_PROFILE].Tag = profileDef;

            PopulateBaseModelColumn(baseModel);
        }

        private void LoadDummyProfile(int columnNumber, DataGridViewCellStyle cellStyle)
        {
            R2Model model = new R2Model { Name = "Dummy" };
            modelsDataGridView.Columns[columnNumber].HeaderText = model.Name;
            modelsDataGridView.Columns[columnNumber].Tag = model;

            R2Section section = new R2Section { SectionId = "CP", IsReadOnly = true };
            model.children.Add(section);
            int rowNumber = getBaseModelRowNumber(section.GetAlignId());

            DataGridViewCell sectionCell = modelsDataGridView.Rows[rowNumber].Cells[columnNumber];
            sectionCell.Tag = section;
            sectionCell.Value = string.Format("{0}", section.SectionId);
            sectionCell.Style = cellStyle;
            for (int h = 1; h < 3; h++)
            {
                R2Function header = new R2Function { FunctionId = string.Format("CP.{0}", h), IsReadOnly = true };
                model.children.Add(header);
                rowNumber = getBaseModelRowNumber(header.GetAlignId());
                if (rowNumber == -1)
                {
                    // Base Model doesnot have this row, add Row at end
                    rowNumber = modelsDataGridView.Rows.Add();
                }

                DataGridViewCell headerCell = modelsDataGridView.Rows[rowNumber].Cells[columnNumber];
                headerCell.Tag = header;
                headerCell.Value = header.FunctionId;
                headerCell.Style = cellStyle;
                for (int f = 1; f < 3; f++)
                {
                    R2Function function = new R2Function { FunctionId = string.Format("CP.{0}.{1}", h, f), IsReadOnly = true };
                    model.children.Add(function);
                    rowNumber = getBaseModelRowNumber(function.GetAlignId());
                    if (rowNumber == -1)
                    {
                        // Base Model doesnot have this row, add Row at end
                        rowNumber = modelsDataGridView.Rows.Add();
                    }

                    DataGridViewCell functionCell = modelsDataGridView.Rows[rowNumber].Cells[columnNumber];
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
                        model.children.Add(criterion);
                        criterion.SetRefId(null, string.Format("CP.{0}.{1}", h, f), string.Format("{0}", c+1));
                        rowNumber = getBaseModelRowNumber(criterion.GetAlignId());
                        if (rowNumber == -1)
                        {
                            // Base Model doesnot have this row, add Row at end
                            rowNumber = modelsDataGridView.Rows.Add();
                        }

                        DataGridViewCell criterionCell = modelsDataGridView.Rows[rowNumber].Cells[columnNumber];
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
            for (int rowNumber = modelsDataGridView.Rows.Count - 1; rowNumber >= 0; rowNumber--)
            {
                modelsDataGridView.Rows[rowNumber].Cells[colNum].Style = null;
                modelsDataGridView.Rows[rowNumber].Cells[colNum].Value = "";
                modelsDataGridView.Rows[rowNumber].Cells[colNum].Tag = null;
            }
        }

        private void LoadProfileDefinition(string maxFileName)
        {
            R2ProfileDefinition profDef = R2ModelV2.MAX.Factory.LoadProfileDefinition(baseModel, maxFileName);
            profDef.BaseModelName = baseModel.Name;
            modelsDataGridView.Columns[COLUMN_MERGED_PROFILE].HeaderText = profDef.Name;
            modelsDataGridView.Columns[COLUMN_MERGED_PROFILE].Tag = profDef;
            foreach (R2ModelElement element in profDef.children)
            {
                int rowNumber = getBaseModelRowNumber(element.GetAlignId());
                if (rowNumber == -1)
                {
                    // Base Model doesnot have this row, add Row at end
                    rowNumber = modelsDataGridView.Rows.Add();
                }

                DataGridViewCell cell = modelsDataGridView.Rows[rowNumber].Cells[COLUMN_MERGED_PROFILE];
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
                compareDataGridView.Rows[rowNumber].DefaultCellStyle = compareDataGridView.DefaultCellStyle;
                compareDataGridView.Rows[rowNumber].Cells["Priority"].Value = section.Priority;
                compareDataGridView.Rows[rowNumber].Cells["SectionId"].Value = section.SectionId;
                compareDataGridView.Rows[rowNumber].Cells["Name"].Value = section.Name;
                compareDataGridView.Rows[rowNumber].Cells["LastModified"].Value = section.LastModified;
            }
            else
            {
                compareDataGridView.Rows[rowNumber].DefaultCellStyle = emptyCellStyle;
            }
        }

        private void populateSelectedFunction(R2Function function, int rowNumber, DataGridViewCellStyle emptyCellStyle, R2Function compareFunction, DataGridViewCellStyle diffCellStyle)
        {
            if (compareFunction == null) compareFunction = function;
            if (function != null)
            {
                compareDataGridView.Rows[rowNumber].DefaultCellStyle = compareDataGridView.DefaultCellStyle;
                populateSelectedCell(compareDataGridView.Rows[rowNumber].Cells["Priority"], function.Priority, compareFunction.Priority, diffCellStyle);
                populateSelectedCell(compareDataGridView.Rows[rowNumber].Cells["FunctionId"], function.FunctionId, compareFunction.FunctionId, diffCellStyle);
                populateSelectedCell(compareDataGridView.Rows[rowNumber].Cells["Name"], function.Name, compareFunction.Name, diffCellStyle);
                populateSelectedCell(compareDataGridView.Rows[rowNumber].Cells["LastModified"], function.LastModified, compareFunction.LastModified, diffCellStyle);
            }
            else
            {
                compareDataGridView.Rows[rowNumber].DefaultCellStyle = emptyCellStyle;
            }
        }

        private void populateSelectedCriterion(R2Criterion criterion, int rowNumber, DataGridViewCellStyle emptyCellStyle, R2Criterion compareCriterion, DataGridViewCellStyle diffCellStyle)
        {
            if (compareCriterion == null) compareCriterion = criterion;
            if (criterion != null)
            {
                compareDataGridView.Rows[rowNumber].DefaultCellStyle = compareDataGridView.DefaultCellStyle;
                populateSelectedCell(compareDataGridView.Rows[rowNumber].Cells["Priority"], criterion.Priority, compareCriterion.Priority, diffCellStyle);
                populateSelectedCell(compareDataGridView.Rows[rowNumber].Cells["Name"], criterion.Name, compareCriterion.Name, diffCellStyle);
                populateSelectedCell(compareDataGridView.Rows[rowNumber].Cells["Text"], criterion.Text, compareCriterion.Text, diffCellStyle);
                populateSelectedCell(compareDataGridView.Rows[rowNumber].Cells["Row#"], criterion.Row, compareCriterion.Row, diffCellStyle);
                populateSelectedCell(compareDataGridView.Rows[rowNumber].Cells["Dependent"], criterion.Dependent, compareCriterion.Dependent, diffCellStyle);
                populateSelectedCell(compareDataGridView.Rows[rowNumber].Cells["Conditional"], criterion.Conditional, compareCriterion.Conditional, diffCellStyle);
                populateSelectedCell(compareDataGridView.Rows[rowNumber].Cells["Optionality"], criterion.Optionality, compareCriterion.Optionality, diffCellStyle);
                populateSelectedCell(compareDataGridView.Rows[rowNumber].Cells["LastModified"], criterion.LastModified, compareCriterion.LastModified, diffCellStyle);
            }
            else
            {
                compareDataGridView.Rows[rowNumber].DefaultCellStyle = emptyCellStyle;
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

        private void modelsDataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
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
                        ProfileMetadataForm form = new ProfileMetadataForm();
                        R2ProfileDefinition profDef = (R2ProfileDefinition)modelsDataGridView.Columns[COLUMN_MERGED_PROFILE].Tag;
                        form.Show(profDef);
                        modelsDataGridView.Columns[COLUMN_MERGED_PROFILE].HeaderText = profDef.Name;
                        return;
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
                        DataGridViewCellStyle cellStyle = new DataGridViewCellStyle {BackColor = Color.White};
                        switch (e.ColumnIndex)
                        {
                            case COLUMN_BASE_MODEL:
                                fileNameBaseModel = fileName;
                                if (MessageBox.Show("This action will lose any changes to the Merged Profile that are not saved. Are you sure?", "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                                    == DialogResult.Yes)
                                {
                                    ClearColumn(0);
                                    ClearColumn(1);
                                    ClearColumn(2);
                                    ClearColumn(3);
                                    ClearColumn(4);
                                    LoadBaseModelAndNewMergedProfile();
                                    UpdateStatistics();
                                }
                                break;
/*                            case COLUMN_MERGED_PROFILE:
                                fileNameMerged = fileName;
                                ClearColumn(COLUMN_MERGED_PROFILE);
                                LoadProfileDefinition(fileNameMerged);
                                break;*/
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
            else if (e.ColumnIndex != -1)
            {
                DataGridViewCell cell = modelsDataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];
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
                updateCompareDataGridView(false, _currentRow, _currentCompareRow);
            }
        }

        private void updateCompareDataGridView(bool reset, int selectedRow, int compareRow)
        {
            DataGridViewCell cell0 = modelsDataGridView.Rows[selectedRow].Cells[0];
            DataGridViewCell cell1 = modelsDataGridView.Rows[selectedRow].Cells[1];
            DataGridViewCell cell2 = modelsDataGridView.Rows[selectedRow].Cells[2];
            DataGridViewCell cell3 = modelsDataGridView.Rows[selectedRow].Cells[3];
            DataGridViewCell cell4 = modelsDataGridView.Rows[selectedRow].Cells[4];

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
                compareDataGridView.Columns.Clear();
                compareDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

                // Create columns based on element type of the cell
                if (typeCell.Tag is R2Section)
                {
                    compareDataGridView.Columns.Add("Priority", "Priority");
                    compareDataGridView.Columns.Add("SectionId", "SectionId");
                    compareDataGridView.Columns.Add("Name", "Name");
                    compareDataGridView.Columns.Add("LastModified", "LastModified");
                }
                else if (typeCell.Tag is R2Function)
                {
                    compareDataGridView.Columns.Add("Priority", "Priority");
                    compareDataGridView.Columns.Add("FunctionId", "FunctionId");
                    compareDataGridView.Columns.Add("Name", "Name");
                    compareDataGridView.Columns.Add("LastModified", "LastModified");
                }
                else if (typeCell.Tag is R2Criterion)
                {
                    compareDataGridView.Columns.Add("Priority", "Priority");
                    compareDataGridView.Columns.Add("Name", "Name");
                    compareDataGridView.Columns.Add("Optionality", "Optionality");
                    compareDataGridView.Columns.Add("Text", "Text");
                    compareDataGridView.Columns.Add("Dependent", "Dependent");
                    compareDataGridView.Columns.Add("Conditional", "Conditional");
                    compareDataGridView.Columns.Add("Row#", "Row#");
                    compareDataGridView.Columns.Add("LastModified", "LastModified");
                }

                compareDataGridView.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
                compareDataGridView.Rows.Add(5);
                for (int rowNumber = 0; rowNumber < modelsDataGridView.Columns.Count; rowNumber++)
                {
                    DataGridViewColumn col = modelsDataGridView.Columns[rowNumber];
                    string modelName = "";
                    if (col.Tag is R2Model)
                    {
                        modelName = ((R2Model)col.Tag).Name;
                    }
                    else if (col.Tag is R2ProfileDefinition)
                    {
                        modelName = ((R2ProfileDefinition)col.Tag).Name;
                    }
                    compareDataGridView.Rows[rowNumber].HeaderCell.Value = modelName;
                }
            }

            // populate cells
            DataGridViewCellStyle emptyCellStyle = new DataGridViewCellStyle {BackColor = Color.LightGray};
            DataGridViewCellStyle c2CellStyle = new DataGridViewCellStyle {BackColor = Color.LightYellow};
            DataGridViewCellStyle diffCellStyle = new DataGridViewCellStyle {BackColor = Color.LightGreen};

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

        private void modelsDataGridView_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (_currentRow != e.RowIndex)
            {
                _currentRow = e.RowIndex;
                if (e.RowIndex == -1) return;
                _currentCompareRow = modelsDataGridView.CurrentCell.ColumnIndex;
                updateCompareDataGridView(true, _currentRow, _currentCompareRow);
            }
        }

        private void compareDataGridView_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (_currentCompareRow != e.RowIndex)
            {
                _currentCompareRow = e.RowIndex;
                if (e.RowIndex == -1) return;
                updateCompareDataGridView(false, _currentRow, _currentCompareRow);
            }
        }

        // Hide rows that are profiled
        private void button1_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            for (int rowNumber = modelsDataGridView.Rows.Count - 1; rowNumber >= 0; rowNumber--)
            {
                if (modelsDataGridView.Rows[rowNumber].Cells[1].Tag != null)
                {
                    modelsDataGridView.Rows[rowNumber].Visible = false;
                }
            }
            UpdateStatistics();
            Cursor = Cursors.Default;
        }

        // Show rows that are profiled
        private void button2_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            for (int rowNumber = modelsDataGridView.Rows.Count - 1; rowNumber >= 0; rowNumber--)
            {
                if (modelsDataGridView.Rows[rowNumber].Cells[1].Tag != null)
                {
                    modelsDataGridView.Rows[rowNumber].Visible = true;
                }
            }
            UpdateStatistics();
            Cursor = Cursors.Default;
        }

        // Hide rows that have no profiled element
        private void collapseButton_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            for (int rowNumber = modelsDataGridView.Rows.Count - 1; rowNumber >= 0; rowNumber--)
            {
                int cellsWithTag = 0;
                // Ignore BaseModel and Merged
                for (int colNum=2; colNum<modelsDataGridView.Rows[rowNumber].Cells.Count;colNum++)
                {
                    if (modelsDataGridView.Rows[rowNumber].Cells[colNum].Tag != null)
                    {
                        cellsWithTag++;
                    }
                }
                if (cellsWithTag < 1)
                {
                    modelsDataGridView.Rows[rowNumber].Visible = false;
                }
            }
            UpdateStatistics();
            Cursor = Cursors.Default;
        }

        // Show all rows
        private void button3_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            for (int rowNumber = modelsDataGridView.Rows.Count - 1; rowNumber >= 0; rowNumber--)
            {
                modelsDataGridView.Rows[rowNumber].Visible = true;
            }
            UpdateStatistics();
            Cursor = Cursors.Default;
        }

        private void UpdateStatistics()
        {
            int rowCount = modelsDataGridView.Rows.Cast<DataGridViewRow>().Count();
            int showingCount = modelsDataGridView.Rows.Cast<DataGridViewRow>().Count(r => r.Visible);
            int hiddenCount = modelsDataGridView.Rows.Cast<DataGridViewRow>().Count(r => !r.Visible);
            int mergedCount = modelsDataGridView.Rows.Cast<DataGridViewRow>().Count(r => r.Cells[1].Tag != null);

            StringBuilder sb = new StringBuilder();
            sb.Append("Rows: ").Append(rowCount).AppendLine();
            sb.Append("Showing: ").Append(showingCount).AppendLine();
            sb.Append("Hidden: ").Append(hiddenCount).AppendLine();
            sb.Append("Merged: ").Append(mergedCount).AppendLine();
            statsTextBox.Text = sb.ToString();
        }

        private void selectButton_Click(object sender, EventArgs e)
        {
            if (modelsDataGridView.CurrentCell.ColumnIndex < 2)
            {
                // Ignore, cannot select FM or Merged Profile itself
                return;
            }
            DataGridViewCell currentCell = modelsDataGridView.CurrentCell;
            if (currentCell.Tag == null)
            {
                // Nothing to do, select an empty cell.
                return;
            }
            modelsDataGridView.CurrentRow.Cells[1].Value = currentCell.Value;

            R2ModelElement compilerInstruction = R2ModelV2.MAX.Factory.CreateModelElement((R2ModelElement)currentCell.Tag, (R2ModelElement)modelsDataGridView.CurrentRow.Cells[0].Tag);
            DataGridViewCell cell = modelsDataGridView.CurrentRow.Cells[1];
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
            DataGridViewCell cell = modelsDataGridView.CurrentRow.Cells[1];
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

            R2ProfileDefinition profileDef = (R2ProfileDefinition)modelsDataGridView.Columns[COLUMN_MERGED_PROFILE].Tag;
            string defId = profileDef.Id;
            ObjectType maxDefObj = (ObjectType)profileDef.SourceObject;
            maxDefObj.SetTagValue("MAX::ExportDate", Util.FormatLastModified(DateTime.Now));
            maxDefObj.SetTagValue("MAX::ExportFile", fileNameOutput);
            objects.Add(maxDefObj);

            foreach(DataGridViewRow row in modelsDataGridView.Rows)
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
            MessageBox.Show("Created Profile Definition MAX file.", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
