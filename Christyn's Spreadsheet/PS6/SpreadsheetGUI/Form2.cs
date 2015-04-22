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
        private string IPaddress, name, ssname, port;

        public Form2()
        {
            InitializeComponent();
        }

        public Form2(Form1 _form)
        {
            last_form = _form;
            InitializeComponent();
            textBox4.Text = "2000";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            IPaddress = textBox1.Text;
            name = textBox2.Text;
            ssname = textBox3.Text;
            port = textBox4.Text;
            last_form.connection_settings(IPaddress, name, ssname, port);
            this.Close();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox1.Checked == true)
            {
                textBox1.Text = "lab1-??.eng.utah.edu";
                textBox2.Text = "sysadmin";
                textBox3.Text = "default";
                textBox4.Text = "2120";
            }
            else
            {
                textBox1.Text = "";
                textBox2.Text = "";
                textBox3.Text = "";
                textBox4.Text = "2000";
            }
        }
    }
}
