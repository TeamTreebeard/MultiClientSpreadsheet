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
    public partial class Form2 : Form
    {
        private Form1 last_form;
        private string IPaddress, name, ssname;

        public Form2()
        {
            InitializeComponent();
        }

        public Form2(Form1 _form)
        {
            last_form = _form;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            IPaddress = textBox1.Text;
            name = textBox2.Text;
            ssname = textBox3.Text;
            last_form.connection_settings(IPaddress, name, ssname);
            this.Close();
        }
    }
}
