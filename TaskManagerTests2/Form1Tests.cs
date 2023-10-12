using Microsoft.VisualStudio.TestTools.UnitTesting;
using TaskManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TaskManager.Tests
{
    [TestClass()]
    public class Form1Tests
    {
        [TestMethod()]
        public void showProcesse_mainTextNotNull_true()
        {
            Form1 main = new Form1();
            main.ShowDialog();

            //bool listNotEmpty = main.listView1.Items.Count != 0;

            
            Assert.IsTrue(main.Text != null);
        }
    }
}