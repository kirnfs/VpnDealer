using DotRas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Timer = System.Threading.Timer;

namespace VpnDialer.Data
{
    public static class MyDialer
    {
        #region Vars
        private static Timer timer = null;
        private static RasDialer dialer = null;
        private static RasHandle connection = null;
        private static String connectionName = null;
        private static int seconds = 30; // 30 секунд интервал проверки
        #endregion

        public static String[] GetVpnConnections()
        {
            List<String> list = new List<String>();
            var phonePath = RasPhoneBook.GetPhoneBookPath(RasPhoneBookType.AllUsers);
            var rasBook = new RasPhoneBook();
            rasBook.Open(phonePath);
            if (rasBook.Entries.Count == 0) { MessageBox.Show("Не удалось найти ни одного VPN соединения в ОС!", Program.appName, MessageBoxButtons.OK, MessageBoxIcon.Warning);}
            foreach (var item in rasBook.Entries)
            {
                list.Add(item.Name);
            }
            return list.ToArray();
        }

        public static bool ConOpen(String connectionNameVpn)
        {
            dialer = new RasDialer();
            dialer.Timeout = 1000;
            dialer.AllowUseStoredCredentials = true;
            dialer.PhoneBookPath = RasPhoneBook.GetPhoneBookPath(RasPhoneBookType.AllUsers);
            dialer.EntryName = connectionNameVpn;
            connectionName = connectionNameVpn;
            //dialer.Credentials = new NetworkCredential(userName, userPassword);
            try 
            { 
                connection = dialer.Dial();
            } 
            catch (Exception exc) 
            {
                var errMsg = $"Ошибка при попытке подключения {connectionNameVpn}! ErrMsg: " + exc.Message;
                Console.WriteLine(errMsg);
                return false; 
            }

            var connList = RasConnection.GetActiveConnections();
            if (connList.Count != 0)
            {
                Console.WriteLine($"Соединение {connectionNameVpn} установлено!");
                RunTimer();
                return true;
            }
            else 
            {
                var errMsg = $"Соединение {connectionNameVpn} НЕ установлено!";
                Console.WriteLine(errMsg);
                MessageBox.Show($"Не удалось подключиться к {connectionNameVpn}.\r\nВозможна проблема с портом или основным интернет соединением!");
                return false;
            }
        }

        public static void CloseAllConnections()
        {
            RasConnection.GetActiveConnections().ToList().ForEach(x => 
            { 
              x.HangUp(); 
              Console.WriteLine($"Соединение {x.EntryName} разорвано принудительно при старте/завершении программы!");
            });
        }

        public static void TimerStop() 
        {
            timer.Change(Timeout.Infinite, Timeout.Infinite);
            timer.Dispose();
            timer = null;
            Console.WriteLine($"Остановлена проверка соединения {connectionName} каждые {seconds} секунд");
        }

        public static void ConClose()
        {
            var connList = RasConnection.GetActiveConnections();
            TimerStop();

            foreach (var con in connList)
             {
                    if (con.EntryName.Equals(connectionName)) 
                    {
                    con.HangUp();
                    dialer = null;
                    connection = null;
                    Console.WriteLine($"Соединение {connectionName} разорвано!");
                    connectionName = null;
                    return;
                    }  
             }
            Console.WriteLine("Не удалось найти активное соединение!");

                //Реализация закрытия соединения через CMD
                /*
                timer.Change(Timeout.Infinite, Timeout.Infinite);
                timer.Dispose();
                timer = null;
                var command = $"{connectionName} /DISCONNECT";
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = "rasdial.exe";
                startInfo.Arguments = command;
                startInfo.CreateNoWindow = true;
                startInfo.UseShellExecute = false;
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                startInfo.Verb = "runas";
                Process.Start(startInfo);
                return true;
                */

        }

        static void RunTimer()
        {
            var timePeriod = seconds * 1000; 
            TimerCallback tm = new TimerCallback(HandleConnection);
            timer = new Timer(tm, 0, 0, timePeriod);
            Console.WriteLine($"Запущена проверка соединения {connectionName} каждые {seconds} секунд");
        }

        public static void HandleConnection(object obj)
        {
            try
            {
                var connList = RasConnection.GetActiveConnections();
                foreach (var connection in connList)
                {
                    if (connection.EntryName.Equals(connectionName)) { return; }
                }
                    connection.Close();
                    connection.Dispose();
                    connection = dialer.Dial();
                    Console.WriteLine($"VPN соединение {connectionName} переподключено!");
            }
            catch (Exception ex) 
            {
                var errMsg = $"Не удалось переподключиться! ErrMsg: {ex.Message}";
                Console.WriteLine(errMsg);
            }
        }

    }
}
