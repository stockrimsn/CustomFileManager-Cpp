using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SimpleFileManager;

namespace SimpleFileManager
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            label1.BackColor = Color.FromArgb(0, 0, 0, 0);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                label1.Visible = false;
            }
            else { label1.Visible = true; }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Name = textBox1.Text;
            this.Close();
        }
    }
}
