using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Management;
using Microsoft.VisualBasic;

namespace TaskManager
{
    public partial class Form1 : Form
    {

        private List<Process> processes = null;

        public Form1()
        {
            InitializeComponent();
        }

        private void GetProcesses()
        {
            //processes.Clear();

            processes = Process.GetProcesses().ToList<Process>();
        }

        private void RefreshProcessesList()
        {
            listView1.Items.Clear();

            double memSize = 0;

            foreach (Process p in processes)
            {
                memSize = 0;

                PerformanceCounter pc = new PerformanceCounter();
                pc.CategoryName = "Process";
                pc.CounterName = "Working Set - Private";
                pc.InstanceName = p.ProcessName;

                memSize = (double)pc.NextValue() / (1000 * 1000);

                string[] row = new string[]
                {
                    p.ProcessName.ToString(), Math.Round(memSize, 1).ToString() + " MB"
                };

                listView1.Items.Add(new ListViewItem(row));

                pc.Close();
                pc.Dispose();
            }

            Text = "Processes running: " + processes.Count.ToString();

        }

        private void RefreshProcessesList(List<Process> processes, string keyword)
        {
            listView1.Items.Clear();

            double memSize = 0;

            foreach (Process p in processes)
            {
                memSize = 0;

                PerformanceCounter pc = new PerformanceCounter();
                pc.CategoryName = "Process";
                pc.CounterName = "Working Set - Private";
                pc.InstanceName = p.ProcessName;

                memSize = (double)pc.NextValue() / (1000 * 1000);

                string[] row = new string[]
                {
                    p.ProcessName.ToString(), Math.Round(memSize, 1).ToString() + " MB"
                };

                listView1.Items.Add(new ListViewItem(row));

                pc.Close();
                pc.Dispose();
            }

            Text = $"Processes running {keyword} : " + processes.Count.ToString();

        }
    }
}
