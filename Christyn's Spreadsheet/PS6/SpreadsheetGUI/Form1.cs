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
using System.Net.Sockets;
using System.Threading;
using System.Net;
using SSModelNS;


// Christyn Phillippi u0636074
// 10/30/2014
//CS3500 Assignment 6

namespace SpreadsheetGUI
{
    public partial class Form1 : Form
    {
        private Spreadsheet sp;
        private String IPaddress, name, ssname;
        private SSModel model;
        public int count;

        /// <summary>
        /// Creates a new empty spreadsheet
        /// </summary>
        public Form1()
        {
            InitializeComponent();

            model = new SSModel();
            sp = new Spreadsheet(isNameValid, t => t.ToUpper(), "ps6");

            model.updateCellEvent += updateCell;
            model.cellErrorEvent += cellError;
            model.genericErrorEvent += genericError;
            model.commandErrorEvent += commandError;
            model.ConnectionNotFoundEvent += reconnect;
            model.usedNameEvent += nameInUse;
            model.noSpreadSheetEvent += noSS;
        }

        private void noSS(string obj)
        {
            Text = "Spreadsheet Form";
            MessageBox.Show("Unable to perform command in current state: "+obj);
        }

        private void nameInUse(string obj)
        {
            MessageBox.Show(obj+" is already registered or invalid.");
        }

        private void reconnect()
        {
            MessageBox.Show("Connection Failed");
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

            if (this.IsHandleCreated)
            {

                this.Invoke((MethodInvoker)delegate
                {
                    textBox3.Focus();    // puts cursor in editable contents box when the cell is selected so you can began entering formula

                    cellNameBox.Text = temp;

                    //display cellValue box
                    object value = sp.GetCellValue(temp);
                    if (value is String || value is double)
                    {
                        textBox2.Text = value.ToString();
                    }
                    else
                    {
                        FormulaError fe = (FormulaError)value;
                        textBox2.Text = "FormulaError";
                    }
                    //display EDITABLE cell contents
                    object contents = sp.GetCellContents(temp);
                    if (contents is Formula)
                    {
                        textBox3.Text = "=" + contents.ToString();
                    }
                    else
                    {
                        textBox3.Text = contents.ToString();
                    }
                });
            }
        }


        /// <summary>
        /// Checks that the cell name is valid for the spreadsheet
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private bool isNameValid(String s)
        {
            return ((Regex.IsMatch(s, "^[A-za-z][0-9]{1,2}$")));
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
            string temp;

            name = getSelectionName();

            Boolean isformula = false;
            temp = textBox3.Text;

            if(textBox3.Text.StartsWith("="))
            {
                isformula = true;
                temp = temp.Substring(1);
            }
            try
            {
                if (isformula)
                {
                    Formula send = new Formula(temp.ToUpper());
                    temp = send.ToString().Insert(0, "=");
                    model.sendCell(name, temp);
                }
                else
                {
                    model.sendCell(name, temp);
                }


            }
            catch (FormulaFormatException)
            {
                MessageBox.Show("Your Formula is in an invalid format");
                return;
            }

        }

        //updates the cell with the given name and contents from the server
        private void updateCell(string name, string contents)
        {
            HashSet<String> evaluateFormula;
            try
            {
                evaluateFormula = (HashSet<String>)sp.SetContentsOfCell(name, contents);
            }
            catch (FormulaFormatException)
            {
                MessageBox.Show("Your Formula is in an invalid format");
                return;
            }
            catch (CircularException)
            {
                //MessageBox.Show("Your Formula is introducing a Circular Exception and has not been entered. Please check your Formula and try again.");
                return;
            }
            updateTextDisplays(spreadsheetPanel1);

            if (sp.GetCellValue(name) is FormulaError)
            {
                SSValueDisplay(name);
                return;
            }

            foreach(String s in evaluateFormula)
            {
                SSValueDisplay(s);
            }
        }

        //if error returned is was from attempting to add an invalid cell
        private void cellError(string error_message)
        {
            MessageBox.Show("Invalid Cell Request: " + error_message);
        }

        private void genericError(string error_message)
        {
            MessageBox.Show("Generic Error: " + error_message);
        }

        private void commandError(string error_message)
        {
            MessageBox.Show("Invalid Command Error: Client may be dammaged or server no longer accepts the request. Request: " + error_message);
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
        /// Helper method to convert  the  cell name to it's corresponding row and column 
        /// </summary>
        /// <returns></returns>
        private void getRowandCol(String s, out int col, out int row)
        {
            int getRow;
            int.TryParse(s.Substring(1), out getRow);

            col = s.First<char>() - 'A';
            row = getRow - 1;
        }

        private void newForm()
        {

            Application.Run(new Form1());
        }

        /// <summary>
        /// Opens a new, blank spreadsheet in a new window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //SSApplicationContext.getAppContext().RunForm(new Form1());

             System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(newForm));
             t.Start();
        }


        /// <summary>
        /// Closes the spreadsheet. If the spreadsheet has been changed since it was opened or created it will ask
        /// if you would like to save. If yes, it will invoke saveToolStripMenuItem_Click. Otherwise it will close.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            model.closeSocket();
            Close();
        }

        /// <summary>
        /// Overrides the red "x" button to ask if you want to save before closing.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            model.closeSocket();

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

        //Open connection window for user to input connection info
        private void connectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 connect_form = new Form2(this);
            connect_form.Show();
        }

        //set connection variables and then call 
        public void connection_settings(string IP, string client_name, string ss_name, string _port)
        {
            if (IP == "" || client_name == "" || ss_name == "")
            {
                // values not filled
            }
            else
            {
                IPaddress = IP;
                name = client_name;
                ssname = ss_name;
                try
                {
                    int port = Convert.ToInt32(_port);
                    model.Connect(IPaddress, port, name, ssname);
                    Text = ssname + " : " + client_name;
                }
                catch(Exception)
                {
                    MessageBox.Show("Failed to connect.");
                    return;
                }
            }
        }

        //sends command to model when CTRL+Z or the undo button is activated
        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            model.sendUndo();
        }

        public void registerUser(string username)
        {
           model.registerUser(username);
        }
    }
}
