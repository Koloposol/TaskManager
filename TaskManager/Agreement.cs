using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TaskManager
{
    public partial class Agreement : Form
    {
        public Agreement()
        {
            InitializeComponent();
        }

        public void button1_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked == true)
            {
                Form1 main = new Form1();
                main.Show();

                this.Hide();
            }
            else if (radioButton2.Checked == true)
            {
                Environment.Exit(0);
            }
            else
            {
                
            }
        }
    }
}
