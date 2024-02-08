using System;
using System.Threading;
using System.Windows.Forms;
using VpnDialer.Data;

namespace VpnDialer
{
    public partial class Form1 : Form
    {
        private static bool isConnectionDisable = true;
        private static String statusDisconnected = "VPN - Отключено";

        public Form1()
        {
            InitializeComponent();
            new MyLog();
            Console.WriteLine($"{AppDomain.CurrentDomain.FriendlyName} запущен!");
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(appExit);
            MyDialer.CloseAllConnections();
            notifyIcon1.Visible = false;
            notifyIcon1.Text = statusDisconnected;
            comboBox1.Items.AddRange(MyDialer.GetVpnConnections());
            comboBox1.SelectedIndex = 0;
        }

        static void appExit(object sender, EventArgs e)
        {
            MyDialer.CloseAllConnections();
            Console.WriteLine($"{AppDomain.CurrentDomain.FriendlyName} завершён!");
            MyLog.WriteDelimiter();
            Thread.Sleep(500);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var conName = comboBox1.SelectedItem as string;
            if (isConnectionDisable)
            {
                if (MyDialer.ConOpen(conName))
                {
                    var conTxt = $"{conName} - Подключено";
                    notifyIcon1.Text = conTxt;
                    button1.Text = "Отключиться";
                    isConnectionDisable = false;
                    comboBox1.Enabled = false;
                }
            }
            else
            {
                    MyDialer.ConClose();
                    notifyIcon1.Text = statusDisconnected;
                    isConnectionDisable = true;
                    button1.Text = "Подключиться";
                    comboBox1.Enabled = true;
            }
        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
            this.ShowInTaskbar = true;
            notifyIcon1.Visible = false;
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                if (this.Visible)
                {
                    notifyIcon1.Visible = true;
                    this.ShowInTaskbar = false;
                }
            }
        }

        private void Form1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            LogForm lg = LogForm.getInstance();
            lg.Text = $"{MyLog.GetLogFilePath()}";
            if (e.Button == MouseButtons.Left)
            {
                lg.richTextBox1.Text = MyLog.GetLogLastRun();
            }
            else
            {
                lg.richTextBox1.Text = MyLog.GetAllLog();
            }
            lg.Show();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
    }

}