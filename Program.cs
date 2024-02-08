using System;
using System.Windows.Forms;

namespace VpnDialer
{
    internal static class Program
    {
        public static string appName = "VpnDealer";
        private static bool createdNew = false;

        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using (var mutex = new System.Threading.Mutex(true, appName, out createdNew))
            {
                if (createdNew)
                {
                  Application.EnableVisualStyles();
                  Application.SetCompatibleTextRenderingDefault(false);
                  Application.Run(new Form1());
                }
                else
                { MessageBox.Show("Эта программа уже запущена!", appName, MessageBoxButtons.OK, MessageBoxIcon.Warning); }
            }
        }
    }
}
