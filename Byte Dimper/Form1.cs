using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Byte_Dimper
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private Memory mem = new Memory();
        public string TargetProcessName = "ModernWarfare";

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        [Flags]
        public enum ThreadAccess : int
        {
            TERMINATE = (0x0001),
            SUSPEND_RESUME = (0x0002),
            GET_CONTEXT = (0x0008),
            SET_CONTEXT = (0x0010),
            SET_INFORMATION = (0x0020),
            QUERY_INFORMATION = (0x0040),
            SET_THREAD_TOKEN = (0x0080),
            IMPERSONATE = (0x0100),
            DIRECT_IMPERSONATION = (0x0200)
        }

        [DllImport("kernel32.dll")]
        private static extern IntPtr OpenThread(ThreadAccess dwDesiredAccess, bool bInheritHandle, uint dwThreadId);

        [DllImport("kernel32.dll")]
        private static extern uint SuspendThread(IntPtr hThread);

        [DllImport("kernel32.dll")]
        private static extern int ResumeThread(IntPtr hThread);

        [DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool CloseHandle(IntPtr handle);

        private static void SuspendProcess(int pid)
        {
            var process = Process.GetProcessById(pid); // throws exception if process does not exist

            foreach (ProcessThread pT in process.Threads)
            {
                IntPtr pOpenThread = OpenThread(ThreadAccess.SUSPEND_RESUME, false, (uint)pT.Id);

                if (pOpenThread == IntPtr.Zero)
                {
                    continue;
                }

                SuspendThread(pOpenThread);

                CloseHandle(pOpenThread);
            }
        }

        public static void ResumeProcess(int pid)
        {
            var process = Process.GetProcessById(pid);

            if (process.ProcessName == string.Empty)
                return;

            foreach (ProcessThread pT in process.Threads)
            {
                IntPtr pOpenThread = OpenThread(ThreadAccess.SUSPEND_RESUME, false, (uint)pT.Id);

                if (pOpenThread == IntPtr.Zero)
                {
                    continue;
                }

                var suspendCount = 0;
                do
                {
                    suspendCount = ResumeThread(pOpenThread);
                } while (suspendCount > 0);

                CloseHandle(pOpenThread);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.mem.IsProcessRunning(this.TargetProcessName))
            {
                if (checkBox1.Checked)
                {
                    long decValue = Convert.ToInt64(mem.BaseAddress.ToString());
                    string hexValue = decValue.ToString("X");
                    textBox1.Text = hexValue;
                }
                else
                {
                    textBox1.Text = mem.BaseAddress.ToString();
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string[] hexx = textBox2.Text.Split('x');
            int decValue = Convert.ToInt32(hexx[1], 16);
            textBox4.Text = decValue.ToString();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            long decValue = Convert.ToInt64(textBox3.Text);
            string hexValue = decValue.ToString("X");
            textBox5.Text = "0x" + hexValue;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.mem.IsProcessRunning(this.TargetProcessName))
                {
                    string[] hexx = textBox7.Text.Split('x');
                    long decValue = Convert.ToInt64(hexx[1], 16);
                    string[] hexx2 = textBox6.Text.Split('x');
                    long decValue2 = Convert.ToInt32(hexx2[1], 16);
                    File.WriteAllBytes(textBox8.Text + ".bin", mem.ReadBytes(mem.BaseAddress + decValue, (int)decValue2));
                    string HEXX = decValue.ToString("X");
                    label13.Text = "Dumped at: 0x" + HEXX.ToString();
                }
            }
            catch (Exception error)
            {
                MessageBox.Show(error.ToString());
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                long decValue2 = 0;
                if (this.mem.IsProcessRunning(this.TargetProcessName))
                {
                    string[] hexx = textBox11.Text.Split('x');
                    long decValue = Convert.ToInt64(hexx[1], 16);

                    if (textBox12.Text.Contains('x'))
                    {
                        string[] hexx2 = textBox12.Text.Split('x');
                        decValue2 = Convert.ToInt32(hexx2[1], 16);
                    }
                    else
                    {
                        decValue2 = Convert.ToInt32(textBox12.Text);
                    }
                    if (radioButton4.Checked)
                    {
                        string[] hexx3 = textBox10.Text.Split('x');
                        long decValue4 = Convert.ToInt32(hexx3[1], 16);
                        File.WriteAllBytes(textBox9.Text + ".bin", mem.ReadBytes(mem.ReadInt64(mem.BaseAddress + decValue) + decValue2, (int)decValue4));
                        long Decamil = mem.ReadInt64(mem.BaseAddress + decValue) + decValue2;
                        string HEXX = Decamil.ToString("X");
                        linkLabel1.Text = "Dumped at: " + "0x" + HEXX;
                    }
                    else if (radioButton5.Checked)
                    {
                        string[] hexx3 = textBox10.Text.Split('x');
                        long decValue4 = Convert.ToInt32(hexx3[1], 16);
                        File.WriteAllBytes(textBox9.Text + ".bin", mem.ReadBytes(mem.ReadInt64(mem.BaseAddress + decValue) - decValue2, (int)decValue4));
                        long Decamil = mem.ReadInt64(mem.BaseAddress + decValue) - decValue2;
                        string HEXX = Decamil.ToString("X");
                        linkLabel1.Text = "Dumped at: " + "0x" + HEXX;
                    }
                }
            }
            catch (Exception error)
            {
                MessageBox.Show(error.ToString());
                MessageBox.Show("Make sure everything in textboxes is hex (0xStuff) and not decimal");
            }
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
        }

        private const int WS_MINIMIZEBOX = 0x20000;
        private const int CS_DBLCLKS = 0x8;

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.Style |= WS_MINIMIZEBOX;
                cp.ClassStyle |= CS_DBLCLKS;
                return cp;
            }
        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {
        }

        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                string[] hexx = textBox13.Text.Split('x');
                long offset = Convert.ToInt64(hexx[1], 16);

                string[] hexx2 = textBox13.Text.Split('x');
                long bytee = Convert.ToInt64(hexx2[1], 16);
                if (checkBox2.Checked)
                {
                    if (this.mem.IsProcessRunning(this.TargetProcessName))
                    {
                        if (radioButton1.Checked)
                        {
                            mem.WriteInt(mem.ReadInt64(mem.BaseAddress + offset), Convert.ToInt32(textBox14.Text));
                        }
                        else if (radioButton2.Checked)
                        {
                            mem.WriteByte(mem.ReadInt64(mem.BaseAddress + offset), (byte)bytee);
                        }
                        else if (radioButton3.Checked)
                        {
                            try
                            {
                                string[] content = textBox14.Text.Split(',');

                                for (int i = 0; i < content.Length; i++)
                                {
                                    string[] theshit = content[i].Split('x');
                                    long byeyeyeye = Convert.ToInt64(theshit[1], 16);
                                    mem.WriteByte(mem.ReadInt64(mem.BaseAddress + offset) + i, (byte)byeyeyeye);
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.ToString());
                            }
                        }
                    }
                }
                else if (checkBox3.Checked)
                {
                    if (this.mem.IsProcessRunning(this.TargetProcessName))
                    {
                        if (radioButton1.Checked)
                        {
                            mem.WriteInt(offset, Convert.ToInt32(textBox14.Text));
                        }
                        else if (radioButton2.Checked)
                        {
                            mem.WriteByte(offset, (byte)bytee);
                        }
                        else if (radioButton3.Checked)
                        {
                            try
                            {
                                string[] content = textBox14.Text.Split(',');

                                for (int i = 0; i < content.Length; i++)
                                {
                                    string[] theshit = content[i].Split('x');
                                    long byeyeyeye = Convert.ToInt64(theshit[1], 16);
                                    mem.WriteByte(offset + i, (byte)byeyeyeye);
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.ToString());
                            }
                        }
                    }
                }
                else
                {
                    if (this.mem.IsProcessRunning(this.TargetProcessName))
                    {
                        if (radioButton1.Checked)
                        {
                            mem.WriteInt(mem.BaseAddress + offset, Convert.ToInt32(textBox14.Text));
                        }
                        else if (radioButton2.Checked)
                        {
                            mem.WriteByte(mem.BaseAddress + offset, (byte)bytee);
                        }
                        else if (radioButton3.Checked)
                        {
                            try
                            {
                                string[] content = textBox14.Text.Split(',');

                                for (int i = 0; i < content.Length; i++)
                                {
                                    string[] theshit = content[i].Split('x');
                                    long byeyeyeye = Convert.ToInt64(theshit[1], 16);
                                    mem.WriteByte(mem.BaseAddress + (offset + i), (byte)byeyeyeye);
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.ToString());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HTCAPTION = 0x2;

        [DllImport("User32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("User32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
            }
        }

        private void label17_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void button9_Click(object sender, EventArgs e)
        {
            try
            {
                Process process = Process.GetProcessesByName(processBox.Text)[0];
                if (process.Handle.ToInt64() != 0L)
                {
                    if (comboBox1.SelectedIndex == 0)
                    {
                        SuspendProcess(process.Id);
                    }
                    else if (comboBox1.SelectedIndex == 1)
                    {
                        ResumeProcess(process.Id);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void label17_Click(object sender, EventArgs e)
        {
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string[] nia = linkLabel1.Text.Split('x');
            Clipboard.SetText(nia[1]);
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                checkBox2.Checked = false;
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked)
            {
                checkBox3.Checked = false;
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            new Parser().Show();
        }
        ToolTip t1 = new ToolTip();
        private void checkBox3_MouseHover(object sender, EventArgs e)
        {
            t1.Show("Check this if you DONT want to use Base Address", checkBox3);
        }

        private void checkBox2_MouseHover(object sender, EventArgs e)
        {
            t1.Show("Check this if you are writing to a pointer", checkBox2);
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioButton2_MouseHover(object sender, EventArgs e)
        {
            t1.Show("This writes as a byte format: 0xAA as an example. Requires the 0x", radioButton2);
        }

        private void radioButton4_MouseHover(object sender, EventArgs e)
        {
            t1.Show("Adds Length after pointer after reading the location", radioButton4);
        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            t1.Show("Negates Length after pointer after reading the location", radioButton5);
        }

        private void linkLabel1_MouseHover(object sender, EventArgs e)
        {
            t1.Show("Click to copy address", linkLabel1);
        }
    }
}