using Microsoft.Win32;
using System.Windows.Forms;

namespace VpnDialer.Data
{
    public class Autorun
    {
        private static RegistryKey GetAppRegistryKey() 
        {
            RegistryKey rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            return rkApp;
        }

        public static void SetAutorunOn(string appName) 
        {
            if (!IsStartupItem(appName))
            {
                GetAppRegistryKey().SetValue(appName, Application.ExecutablePath.ToString());
            }
        }

        public static void SetAutorunOff(string appName)
        {
            if (IsStartupItem(appName))
            {
                GetAppRegistryKey().DeleteValue(appName, false); 
            }
        }

        private static bool IsStartupItem(string appName)
        {
            if (GetAppRegistryKey().GetValue(appName) == null) { return false; }
            else { return true; }
        }

    }
}
