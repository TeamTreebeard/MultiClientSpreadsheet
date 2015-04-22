namespace SpreadsheetGUI
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.spreadsheetPanel1 = new SS.SpreadsheetPanel();
            this.cellNameBox = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.valueBox = new System.Windows.Forms.Label();
            this.EditContentsBox = new System.Windows.Forms.Label();
            this.evaluate = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.connectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.undoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.registerUserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.connectToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.disconnectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // spreadsheetPanel1
            // 
            this.spreadsheetPanel1.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
            this.spreadsheetPanel1.BackColor = System.Drawing.SystemColors.Control;
            this.spreadsheetPanel1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.spreadsheetPanel1.Location = new System.Drawing.Point(12, 30);
            this.spreadsheetPanel1.Name = "spreadsheetPanel1";
            this.spreadsheetPanel1.Size = new System.Drawing.Size(984, 599);
            this.spreadsheetPanel1.TabIndex = 2;
            this.spreadsheetPanel1.SelectionChanged += new SS.SelectionChangedHandler(this.updateTextDisplays);
            // 
            // cellNameBox
            // 
            this.cellNameBox.Location = new System.Drawing.Point(336, 4);
            this.cellNameBox.Name = "cellNameBox";
            this.cellNameBox.ReadOnly = true;
            this.cellNameBox.Size = new System.Drawing.Size(100, 20);
            this.cellNameBox.TabIndex = 4;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(516, 4);
            this.textBox2.Name = "textBox2";
            this.textBox2.ReadOnly = true;
            this.textBox2.Size = new System.Drawing.Size(100, 20);
            this.textBox2.TabIndex = 5;
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(677, 3);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(100, 20);
            this.textBox3.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(275, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Cell Name";
            // 
            // valueBox
            // 
            this.valueBox.AutoSize = true;
            this.valueBox.Location = new System.Drawing.Point(442, 7);
            this.valueBox.Name = "valueBox";
            this.valueBox.Size = new System.Drawing.Size(54, 13);
            this.valueBox.TabIndex = 8;
            this.valueBox.Text = "Cell Value";
            // 
            // EditContentsBox
            // 
            this.EditContentsBox.AutoSize = true;
            this.EditContentsBox.Location = new System.Drawing.Point(622, 7);
            this.EditContentsBox.Name = "EditContentsBox";
            this.EditContentsBox.Size = new System.Drawing.Size(49, 13);
            this.EditContentsBox.TabIndex = 9;
            this.EditContentsBox.Text = "Contents";
            // 
            // evaluate
            // 
            this.evaluate.Location = new System.Drawing.Point(783, 3);
            this.evaluate.Name = "evaluate";
            this.evaluate.Size = new System.Drawing.Size(42, 21);
            this.evaluate.TabIndex = 10;
            this.evaluate.Text = "Enter";
            this.evaluate.UseVisualStyleBackColor = true;
            this.evaluate.Click += new System.EventHandler(this.evaluate_Click);
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.closeToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.newToolStripMenuItem.Text = "New";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.closeToolStripMenuItem.Text = "Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.connectToolStripMenuItem,
            this.undoToolStripMenuItem,
            this.registerUserToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1034, 24);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // connectToolStripMenuItem
            // 
            this.connectToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.connectToolStripMenuItem1,
            this.disconnectToolStripMenuItem});
            this.connectToolStripMenuItem.Name = "connectToolStripMenuItem";
            this.connectToolStripMenuItem.Size = new System.Drawing.Size(81, 20);
            this.connectToolStripMenuItem.Text = "Connection";
            this.connectToolStripMenuItem.Click += new System.EventHandler(this.connectToolStripMenuItem_Click);
            // 
            // undoToolStripMenuItem
            // 
            this.undoToolStripMenuItem.Name = "undoToolStripMenuItem";
            this.undoToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
            this.undoToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.undoToolStripMenuItem.Text = "Undo";
            this.undoToolStripMenuItem.Click += new System.EventHandler(this.undoToolStripMenuItem_Click);
            // 
            // registerUserToolStripMenuItem
            // 
            this.registerUserToolStripMenuItem.Name = "registerUserToolStripMenuItem";
            this.registerUserToolStripMenuItem.Size = new System.Drawing.Size(87, 20);
            this.registerUserToolStripMenuItem.Text = "Register User";
            this.registerUserToolStripMenuItem.Click += new System.EventHandler(this.registerUserToolStripMenuItem_Click);
            // 
            // connectToolStripMenuItem1
            // 
            this.connectToolStripMenuItem1.Name = "connectToolStripMenuItem1";
            this.connectToolStripMenuItem1.Size = new System.Drawing.Size(152, 22);
            this.connectToolStripMenuItem1.Text = "Connect";
            this.connectToolStripMenuItem1.Click += new System.EventHandler(this.connectToolStripMenuItem1_Click);
            // 
            // disconnectToolStripMenuItem
            // 
            this.disconnectToolStripMenuItem.Name = "disconnectToolStripMenuItem";
            this.disconnectToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.disconnectToolStripMenuItem.Text = "Disconnect";
            this.disconnectToolStripMenuItem.Click += new System.EventHandler(this.disconnectToolStripMenuItem_Click);
            // 
            // Form1
            // 
            this.AcceptButton = this.evaluate;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1034, 603);
            this.Controls.Add(this.evaluate);
            this.Controls.Add(this.EditContentsBox);
            this.Controls.Add(this.valueBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.cellNameBox);
            this.Controls.Add(this.spreadsheetPanel1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Spreadsheet Form";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SS.SpreadsheetPanel spreadsheetPanel1;
        private System.Windows.Forms.TextBox cellNameBox;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label valueBox;
        private System.Windows.Forms.Label EditContentsBox;
        private System.Windows.Forms.Button evaluate;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.ToolStripMenuItem connectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem undoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem registerUserToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem connectToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem disconnectToolStripMenuItem;
    }
}

