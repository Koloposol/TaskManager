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
using System.Runtime.InteropServices;
using Microsoft.VisualBasic;

namespace TaskManager
{
    public partial class Form1 : Form
    {

        private List<Process> processes = null;

        private ListViewItemComparer comparer = null;

        private float cpu;

        private float ram;

        private ulong installedMemory;

        public Form1()
        {
            InitializeComponent();
        }
        CPUtemp temp = new CPUtemp();

        private void GetProcesses()
        {
            processes.Clear();

            processes = Process.GetProcesses().ToList<Process>();
        }

        private void RefreshProcessesList()
        {
            try
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
                        p.ProcessName.ToString(), Math.Round(memSize, 1).ToString()// + " MB"
                    };

                    listView1.Items.Add(new ListViewItem(row));

                    pc.Close();
                    pc.Dispose();
                }

                toolStripTextBox1.Text = "";
                Text = "Processes running: " + processes.Count.ToString();

            }
            catch (Exception)
            {

            }
        }

        private void RefreshProcessesList(List<Process> processes, string keyword)
        {
            try
            {
                listView1.Items.Clear();

                double memSize = 0;

                foreach (Process p in processes)
                {
                    if (p != null)
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
                }

                Text = $"Processes running '{keyword}' : " + processes.Count.ToString();

            }
            catch (Exception)
            {

            }
        }
        //Завершить процесс
        private void KillProcess(Process process)
        {
            process.Kill();

            process.WaitForExit();
        }

        //Завершить дерево процессов
        private void KillProcessAndChildren(int pId)
        {
            if (pId == 0) 
            {
                return;            
            }

            ManagementObjectSearcher searcher = new ManagementObjectSearcher("Select * From Win32_Process Where ParentProcessID=" + pId);

            ManagementObjectCollection objectsCollection = searcher.Get();

            foreach (ManagementObject obj in objectsCollection)
            {
                KillProcessAndChildren(Convert.ToInt32(obj["ProcessID"]));
            }

            try
            {
                Process p = Process.GetProcessById(pId);

                p.Kill();

                p.WaitForExit();
            }
            catch (ArgumentException)
            {

            }

        }

        private int GetParentProcessId(Process p)
        {
            int parentID = 0;

            try
            {
                ManagementObject managementObject = new ManagementObject("win32_process.handle='" + p.Id + "'");

                managementObject.Get();

                parentID = Convert.ToInt32(managementObject["ParentProcessId"]);
            }
            catch (Exception)
            {

            }

            return parentID;
        }

        private void GetHardWareInfo(string key, ListView list)
        {
            list.Items.Clear();

            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM " + key);

            try
            {
                foreach (ManagementObject obj in searcher.Get())
                {
                    ListViewGroup listViewGroup;

                    try
                    {
                        listViewGroup = list.Groups.Add(obj["Name"].ToString(), obj["Name"].ToString());
                    }
                    catch (Exception ex)
                    {
                        listViewGroup = list.Groups.Add(obj.ToString(), obj.ToString());
                    }


                    if(obj.Properties.Count == 0)
                    {
                        MessageBox.Show("Информация не найдена", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        return;
                    }

                    foreach (PropertyData data in obj.Properties)
                    {
                        ListViewItem item = new ListViewItem(listViewGroup);

                        if (list.Items.Count % 2 != 0)
                        {
                            item.BackColor = Color.Azure;
                        }
                        else
                        {
                            item.BackColor = Color.LightSteelBlue;
                        }

                        item.Text = data.Name;

                        if (data.Value != null && !string.IsNullOrEmpty(data.Value.ToString()))
                        {
                            switch (data.Value.GetType().ToString())
                            {
                                case "System.String[]":

                                    string[] stringData = data.Value as string[];

                                    string resStr1 = string.Empty;

                                    foreach (string s in stringData)
                                    {
                                        resStr1 += $"{s} ";
                                    }

                                    item.SubItems.Add(resStr1);

                                    break;

                                case "System.UInt16[]":

                                    ushort[] ushortData = data.Value as ushort[];

                                    string resStr2 = string.Empty;

                                    foreach(ushort u in ushortData)
                                    {
                                        resStr2 += $"{Convert.ToString(u)} ";
                                    }

                                    item.SubItems.Add(resStr2);

                                    break;

                                default:

                                    item.SubItems.Add(data.Value.ToString());

                                    break;
                            }

                            list.Items.Add(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //Процессы
            processes = new List<Process>();

            processes = Process.GetProcesses().ToList<Process>();

            RefreshProcessesList();

            comparer = new ListViewItemComparer();
            comparer.ColumnIndex = 0;

            //Характеристики
            toolStripComboBox1.SelectedIndex = 0;

            //Системный монитор
            MEMORYSTATUSEX mEMORYSTATUSEX = new MEMORYSTATUSEX();

            if (GlobalMemoryStatusEx(mEMORYSTATUSEX))
            {
                installedMemory = mEMORYSTATUSEX.ullTotalPhys;
            }

            label10.Text = Convert.ToString(installedMemory / 1000000000) + " Гб";

            timer1.Interval = 1000;

            timer1.Start();
        }

        //Реализация класса для получения общего объема памяти (Системный монитор) 
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private class MEMORYSTATUSEX
        {
            public uint dwLength;
            public uint dwMemoryLength;
            public ulong ullTotalPhys;
            public ulong ullAvailPhys;
            public ulong ullTotalPageFile;
            public ulong ullAvailPageFile;
            public ulong ullTotalVirtual;
            public ulong ullAvailVirtual;
            public ulong ullAvailExtandadVirtual;

            public MEMORYSTATUSEX()
            {
                this.dwLength = (uint)Marshal.SizeOf(typeof(MEMORYSTATUSEX));
            }
        }

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern bool GlobalMemoryStatusEx([In, Out] MEMORYSTATUSEX lpBuffer);

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            GetProcesses();

            RefreshProcessesList();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            try
            {
                if (listView1.SelectedItems[0] != null) 
                {
                    Process processToKill = processes.Where((x) => x.ProcessName == listView1.SelectedItems[0].SubItems[0].Text).ToList()[0];

                    KillProcess(processToKill);

                    GetProcesses();

                    RefreshProcessesList();
                }
            }
            catch(Exception) 
            {
            
            }
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            try
            {
                if (listView1.SelectedItems[0] != null)
                {
                    Process processToKill = processes.Where((x) => x.ProcessName == listView1.SelectedItems[0].SubItems[0].Text).ToList()[0];

                    KillProcessAndChildren(GetParentProcessId(processToKill));

                    GetProcesses();

                    RefreshProcessesList();
                }
            }
            catch (Exception)
            {

            }
        }

        private void завершитьДеревоПроцессовToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (listView1.SelectedItems[0] != null)
                {
                    Process processToKill = processes.Where((x) => x.ProcessName == listView1.SelectedItems[0].SubItems[0].Text).ToList()[0];

                    KillProcessAndChildren(GetParentProcessId(processToKill));

                    GetProcesses();

                    RefreshProcessesList();
                }
            }
            catch (Exception)
            {

            }
        }

        private void завершитьПроцессToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (listView1.SelectedItems[0] != null)
                {
                    Process processToKill = processes.Where((x) => x.ProcessName == listView1.SelectedItems[0].SubItems[0].Text).ToList()[0];

                    KillProcess(processToKill);

                    GetProcesses();

                    RefreshProcessesList();
                }
            }
            catch (Exception)
            {

            }
        }

        private void запуститьНовуюЗадачуToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string path = Interaction.InputBox("Введите имя программы", "Запуск новой задачи");

            try
            {
                Process.Start(path);

                RefreshProcessesList();
            }
            catch(Exception) 
            { 
            
            }
        }

        private void toolStripTextBox1_TextChanged(object sender, EventArgs e)
        {
            GetProcesses();

            List<Process> filteredProcesses = processes.Where((x) => x.ProcessName.ToLower().Contains(toolStripTextBox1.Text.ToLower())).ToList<Process>();

            RefreshProcessesList(filteredProcesses, toolStripTextBox1.Text);
        }

        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            comparer.ColumnIndex = e.Column;

            comparer.SortDirection = comparer.SortDirection == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending;

            listView1.ListViewItemSorter = comparer;

            listView1.Sort();
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string key = string.Empty;

            switch (toolStripComboBox1.SelectedItem.ToString())
            {
                case "Процессор":

                    key = "Win32_Processor";

                    break;

                case "Видеокарта":

                    key = "Win32_VideoController";

                    break;

                case "Чипсет":

                    key = "Win32_IDEController";

                    break;

                case "Батарея":

                    key = "Win32_Battery";

                    break;

                case "Биос":

                    key = "Win32_BIOS";

                    break;

                case "Оперативная память":

                    key = "Win32_PhysicalMemory";

                    break;

                case "Кэш":

                    key = "Win32_CacheMemory";

                    break;

                case "USB":

                    key = "Win32_USBController";

                    break;

                case "Диск":

                    key = "Win32_DiskDrive";

                    break;

                case "Логические диски":

                    key = "Win32_LogicalDisk";

                    break;

                case "Клавиатура":

                    key = "Win32_Keyboard";

                    break;

                case "Сеть":

                    key = "Win32_NetworkAdapter";

                    break;

                case "Пользователи":

                    key = "Win32_Account";

                    break;

                default:
                    key = "Win32_Processor";
                    break;
            }

            GetHardWareInfo(key, listView2);
        }

        private void toolStrip3_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {

            if (temp == null || temp.IsDisposed)
            {
                temp = new CPUtemp();
                temp.Show();
            }
            else
            {
                temp.Show();
                temp.Focus();
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            cpu = performanceCPU.NextValue();
            ram = performanceRAM.NextValue();

            //Настройка шкал
            progressBar1.Value = (int)cpu;
            progressBar2.Value = (int)ram;

            //Настройка %-ов шкал
            label3.Text = Convert.ToString(Math.Round(cpu, 1)) + " %";
            label4.Text = Convert.ToString(Math.Round(ram, 1)) + " %";

            //Настройка нижних значений
            label6.Text = Convert.ToString(Math.Round((ram / 100 * installedMemory) / 1000000000, 1)) + " Гб";
            label8.Text = Convert.ToString(Math.Round((installedMemory - ram / 100 * installedMemory) / 1000000000, 1)) + " Гб";

            //Настройка диаграммы
            chart1.Series["ЦП"].Points.AddY(cpu);
            chart1.Series["ОЗУ"].Points.AddY(ram);
        }
    }
}
