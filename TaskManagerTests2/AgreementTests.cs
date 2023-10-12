﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using TaskManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Tests
{
    [TestClass()]
    public class AgreementTests
    {
        [TestMethod()]
        public void open_agreement_true()
        {
            Agreement ag = new Agreement();
            ag.ShowDialog();

            bool isOpened = ag.IsDisposed != true;

            Assert.IsTrue(isOpened);
        }

    }
}