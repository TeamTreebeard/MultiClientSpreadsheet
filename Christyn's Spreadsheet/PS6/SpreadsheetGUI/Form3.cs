using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpreadsheetGUI
{
    public partial class Form3 : Form
    {
        private Form1 last_form;
        private string username;
       
        public Form3(Form1 _form)
        {
            last_form = _form;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            username = textBox1.Text;
            last_form.registerUser(username);
            this.Close();
        }

    }
}
