using Microsoft.VisualStudio.TestTools.UnitTesting;
using TaskManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Tests
{
    [TestClass()]
    public class CPUtempTests
    {
        [TestMethod()]
        public void open_CPUform_true()
        {
            CPUtemp cpu = new CPUtemp();
            cpu.ShowDialog();

            Assert.IsNotNull(cpu);
        }
    }
}