using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Byte_Dimper
{
    public partial class Parser : Form
    {
        public Parser()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MessageBox.Show("So this for example, say you have bytes from CE or HxD. The bytes will be like this: 0E 1F BA 0E 00 B4 09\n\nThe Parser will turn it into this: 0x0E, 0x1F, 0xBA, 0x0E, 0x00, 0xB4, 0x09\n\nand if you select the new byte option, it will turn it to this: new byte[] { 0x0E, 0x1F, 0xBA, 0x0E, 0x00, 0xB4, 0x09 }");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                string text = richTextBox1.Text;
                string text2 = "0x" + text.Replace(" ", ", 0x");
                richTextBox1.Text = "new byte[] { " + text2 + " }";
            }
            else
            {
                string text = richTextBox1.Text;
                richTextBox1.Text = "0x" + text.Replace(" ", ", 0x");
            }
           
        }
    }
}
