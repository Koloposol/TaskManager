using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenHardwareMonitor.Hardware;

namespace TaskManager
{
    public partial class CPUtemp : Form
    {
        private string tmpInfo = string.Empty;

        private void GetCPUTemperature()
        {
            tmpInfo = string.Empty;

            Visitor visitor = new Visitor();

            Computer computer = new Computer();
            computer.Open();
            computer.CPUEnabled = true;
            computer.Accept(visitor);

            for (int i = 0; i < computer.Hardware.Length; i++)
            {
                if (computer.Hardware[i].HardwareType == HardwareType.CPU)
                {
                    for (int j = 0; j < computer.Hardware[i].Sensors.Length; j++)
                    {
                        if (computer.Hardware[i].Sensors[j].SensorType == SensorType.Temperature)
                        {
                            tmpInfo += computer.Hardware[i].Sensors[j].Name + ": " + computer.Hardware[i].Sensors[j].Value.ToString() + "\n";
                        }
                    }
                }
            }

            richTextBox1.Text = tmpInfo;

            computer.Close();
        }

        public CPUtemp()
        {
            InitializeComponent();
        }

        private void CPUtemp_Load(object sender, EventArgs e)
        {
            backgroundWorker1.RunWorkerAsync();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                GetCPUTemperature();

                Thread.Sleep(3000);
            }
        }
    }
}
