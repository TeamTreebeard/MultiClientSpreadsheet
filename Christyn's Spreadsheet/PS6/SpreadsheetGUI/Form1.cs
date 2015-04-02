using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using SS;
using SpreadsheetUtilities;


// Christyn Phillippi u0636074
// 10/30/2014
//CS3500 Assignment 6

namespace SpreadsheetGUI
{
    public partial class Form1 : Form
    {
        private Spreadsheet sp;
        private String fileName;

        /// <summary>
        /// Creates a new empty spreadsheet
        /// </summary>
        public Form1()
        {
            InitializeComponent();

            sp = new Spreadsheet(isNameValid, t => t.ToUpper(), "ps6");
            fileName = null;
            updateTextDisplays(spreadsheetPanel1);
        }



        /// <summary>
        /// Creates a spreadsheet form and populates using a saved spreadsheet.
        /// </summary>
        /// <param name="path"></param>
        public Form1(String path)
        {
            InitializeComponent();

            sp = new Spreadsheet(path, isNameValid, t => t.ToUpper(), "ps6");
            fileName = path;
            updateTextDisplays(spreadsheetPanel1);
        }

        /// <summary>
        /// Updates the 3 textboxes to display the cell name, cell value and cell contents any new cell
        /// is selected via mouseclick or arrow keys.
        /// </summary>
        /// <param name="s"></param>
        private void updateTextDisplays(SpreadsheetPanel s)
        {
            //display cellName box
            String temp = getSelectionName();

            textBox3.Focus();    // puts cursor in editable contents box when the cell is selected so you can began entering formula

            cellNameBox.Text = temp;

            //display cellValue box
            object value = sp.GetCellValue(temp);
            if(value is String || value is double)
            {
                textBox2.Text = value.ToString();
            }
            else
            {
                FormulaError fe = (FormulaError)value;
                MessageBox.Show(fe.Reason);
            }
            //display EDITABLE cell contents
            object contents = sp.GetCellContents(temp);
            if (contents is Formula)
            {
                textBox3.Text = "="+contents.ToString();
            }
            else
            {
                textBox3.Text = contents.ToString();
            }
        }


        /// <summary>
        /// Checks that the cell name is valid for the spreadsheet
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private bool isNameValid(String s)
        {
            return ((Regex.IsMatch(s, "^[A-za-z]+[0-9]+$")));
        }

        /// <summary>
        /// When "go" is clicked by the editable textbox the formula will be evaluated and added to 
        /// the spreadsheet, or the string/double will be added to the spreadsheet. If any exceptions or errors occurs while
        /// adding the Formula to the spreadsheet no changes will be made and the user will be informed of the error.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void evaluate_Click(object sender, EventArgs e)
        {
            String name;
            HashSet<String> evaluateFormula;

            name = getSelectionName();

            try
            {
                evaluateFormula = (HashSet<String>)sp.SetContentsOfCell(name, textBox3.Text.ToUpper());
            }
            catch (FormulaFormatException)
            {
                MessageBox.Show("Your Formula is in an invalid format");
                return;
            }
            catch (CircularException)
            {
                MessageBox.Show("Your Formula is introducing a Circular Exception and has not been entered. Please check your Formula and try again.");
                return;
            }

            updateTextDisplays(spreadsheetPanel1);

            if (sp.GetCellValue(name) is FormulaError)
                return;

            foreach(String s in evaluateFormula)
            {
                SSValueDisplay(s);
            }
        }

        /// <summary>
        /// updates the spreadsheet panel display to show recalcuated cell values.
        /// </summary>
        /// <param name="s"></param>
        private void SSValueDisplay(string s)
        {
            int col;
            int row;
            getRowandCol(s, out col, out row);
            object c = sp.GetCellValue(s);
            if (c is FormulaError)
            {
                spreadsheetPanel1.SetValue(col, row, "Formula Error");
                return;

            }
            else
            {
                spreadsheetPanel1.SetValue(col, row, c.ToString());
            }

        }

        /// <summary>
        /// Helper method to convert the row and column of the selected cell to its corresponding cell name
        /// </summary>
        /// <returns></returns>
        private string getSelectionName()
        {
            int row, col, cellNum;
            char cellChar;
            String name;

            spreadsheetPanel1.GetSelection(out col, out row);       //grab col and row to convert to name
            cellChar = (char)(col + 'A');                           // convert column # to char
            cellNum = row + 1;
            name = cellChar + cellNum.ToString();

            return name;
        }

        /// <summary>
        /// Helper method to convert the row and column of the selected cell to its corresponding cell name
        /// </summary>
        /// <returns></returns>
        private void getRowandCol(String s, out int col, out int row)
        {
            int getRow;
            int.TryParse(s.Substring(1), out getRow);

            col = s.First<char>() - 'A';
            row = getRow - 1;
        }

        /// <summary>
        /// Gives a summary of how the spreadsheet GUI is used
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void help_Click(object sender, EventArgs e)
        {
            MessageBox.Show("After starting the program, click the spreadsheet to begin navigating the cells or entering data." + "\n" +
                "You can navigate the Spreadsheet using your mouse to select a cell or by using the arrow keys." + "\n" + 
                "Once you are on your selected cell you can start typing to enter in the data." + "\n" + 
                "To move the data into the spreadsheet you can hit the 'Enter' button, or hit the Enter key on your keyboard" + "\n" +
                "Use the file menu to Save your spreadsheet with the 'Save' button, Open a spreadsheet from file with 'Open', " + 
                "ppen a new, blank spreadsheet with 'New', or Close out the current spreadsheet with 'Close' or by hitting the red 'x'." + 
                "\n" + "If the spreadsheet has been altered since the last save or since it was opened you will be prompted to save." + "\n" 
                + "Any errors opening, saving, or entering data will cause an error message to display and no changes will be made.");
        }

        /// <summary>
        /// Saves spreadsheet. If the member variable fileName is null, a dialog will open allowing the user to Save.
        /// If a filename is set, it will overwrite the existing copy.
        /// 
        /// If no changes have been made since the file was last saved or opened, no save will occur.
        /// 
        /// If an error occurs while saving the user will be informed that no save has occurred.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (fileName == null)
            {
                saveFileDialog1.Filter = "Spreadsheet files (*.sprd)|*.sprd|All files (*.*)|*.*";
                saveFileDialog1.ShowDialog();

                try
                {
                    if (saveFileDialog1.FileName != "")
                    {
                        fileName = saveFileDialog1.FileName;
                        sp.Save(fileName);
                    }
                }
                catch(Exception)
                {
                    MessageBox.Show("File not saved");
                }
            }

            else
            {
                if (sp.Changed)
                {
                    sp.Save(fileName);
                }
            }
        }

        /// <summary>
        /// Opens a new, blank spreadsheet in a new window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SSApplicationContext.getAppContext().RunForm(new Form1());
        }

        /// <summary>
        /// Opens a spreadsheet from file. If an error occurs while populating the spreadsheet a popup will inform you and no spreadsheet will
        /// be opened.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Spreadsheet files (*.sprd)|*.sprd|All files (*.*)|*.*";
            openFileDialog1.ShowDialog();

            if(openFileDialog1.FileName != "")
            {
                try
                {
                    Form1 toOpen = (new Form1(openFileDialog1.FileName));
                    SSApplicationContext.getAppContext().RunForm(toOpen);

                    foreach (string s in toOpen.sp.GetNamesOfAllNonemptyCells())
                    {
                        toOpen.SSValueDisplay(s);
                    }
                }
                catch (SpreadsheetReadWriteException)
                {
                    MessageBox.Show("Error reading file.");
                }
            }
        }

        /// <summary>
        /// Closes the spreadsheet. If the spreadsheet has been changed since it was opened or created it will ask
        /// if you would like to save. If yes, it will invoke saveToolStripMenuItem_Click. Otherwise it will close.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sp.Changed)
            {
                DialogResult result = MessageBox.Show("The Spreadsheet has been altered, do you want to save?", "Warning", 
                                                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                if(result == DialogResult.Yes)
                 {
                    saveToolStripMenuItem.PerformClick();
                 }      
                else if(result == DialogResult.No)
                 {
                    Close();
                 }
                else if (result == DialogResult.Cancel)
                 {
                     // do nothing
                 }
            }
            else
            {
                Close();
            }
        }

        /// <summary>
        /// Overrides the red "x" button to ask if you want to save before closing.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (sp.Changed)
            {
                DialogResult result = MessageBox.Show("The Spreadsheet has been altered, do you want to save?", "Warning",
                                                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    saveToolStripMenuItem.PerformClick();
                }
                else if (result == DialogResult.No)
                {
                   // just close
                }
            }
            else
            {
               //just close
            }
        }
        /// <summary>
        /// Override to allow arrowkeys to be used in Spreadsheet panel to navigate the cells. Updates the textboxes as it goes.
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="keyData"></param>
        /// <returns></returns>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {

            int row, col;
            spreadsheetPanel1.GetSelection(out col, out row);

            //capture up arrow key
            if (keyData == Keys.Up)
            {
                if (row == 0)
                {
                    if(spreadsheetPanel1.SetSelection(col, (row + 98)))
                    {
                        updateTextDisplays(spreadsheetPanel1);
                        return true;
                    }
                    return false;
                }
                else
                {
                    if(spreadsheetPanel1.SetSelection(col, (row - 1)))
                    {
                        updateTextDisplays(spreadsheetPanel1);
                        return true;
                    }
                    return false;
                }
            }
            //capture down arrow key
            if (keyData == Keys.Down)
            {
                if (row == 98)
                {
                    if(spreadsheetPanel1.SetSelection(col, (row - 98)))
                    {
                        updateTextDisplays(spreadsheetPanel1);
                        return true;
                    }
                    return false;
                }
                else
                {
                    if(spreadsheetPanel1.SetSelection(col, (row + 1)))
                    {
                        updateTextDisplays(spreadsheetPanel1);
                        return true;
                    }
                    return false;
                }
                  
            }
            //capture left arrow key
            if (keyData == Keys.Left)
            {
                if (col == 0)
                {
                    if(spreadsheetPanel1.SetSelection((col+25), row))
                    {
                        updateTextDisplays(spreadsheetPanel1);
                        return true;
                    }
                    return false;
                }
                else
                {
                    if(spreadsheetPanel1.SetSelection((col-1), row))
                    {
                        updateTextDisplays(spreadsheetPanel1);
                        return true;
                    }
                    return false;
                }
            }
            //capture right arrow key
            if (keyData == Keys.Right)
            {
                if (col == 25)
                {
                    if(spreadsheetPanel1.SetSelection((col - 25), row))
                    {
                        updateTextDisplays(spreadsheetPanel1);
                        return true;
                    }
                    return false;
                }
                else
                {
                    if(spreadsheetPanel1.SetSelection((col + 1), row))
                    {
                        updateTextDisplays(spreadsheetPanel1);
                        return true;
                    }
                    return false;
                }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
