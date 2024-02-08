using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace VpnDialer.Data
{
    public class MyLog : TextWriter
    {
        #region Constructor
        static MyLog() 
        { 
            Console.SetOut(new MyLog());
            DeleteOldLogs();
        }
        #endregion

        #region Vars
        private static object sync = new object();
        public const string txtDelimiter = "-------------------------------------------------------------------------------------------------------------------------------------------------------------------";
        string textLog = "";
        char previousChar;
        private static Encoding encodingWin1251 = Encoding.GetEncoding("Windows-1251");
        const int dayToDelete = 3; //Логи удаляются старше 3 дней
        #endregion

        #region DateTime
        private static String GetTimeNowForLine()
        {
            var dtn = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            return dtn;
        }

        private static String GetTimeNowForFolder()
        {
            var dtn = DateTime.Now.ToString("yyyy_MM_dd");
            return dtn;
        }

        public static DateTime parseDateString(String inputText, String pattern)
        {
            DateTime outData;
            DateTime.TryParseExact(inputText, pattern, CultureInfo.InvariantCulture, DateTimeStyles.None, out outData);
            return outData;
        }
        #endregion

        #region Overrides
        public override Encoding Encoding
        {
            get
            {
                return encodingWin1251;
            }
        }

        public override void Write(char value)
        {
            textLog += value;
            if (value.ToString().Equals("\n") && previousChar.ToString().Equals("\r")) 
            {
                WriteLine(textLog);
                textLog = String.Empty;
            }
            previousChar = value;
            return;
        }
        #endregion

        #region Methods
        public static void WriteLine(String textLog, Exception ex = null)
        {
            try
            {
                if (String.IsNullOrEmpty(textLog) && ex == null) { return; }
                string exception = "";
                string filename = GetLogFilePath();
                if (ex != null)
                { exception = $" - ErrMsg: {ex.Message}"; }
                string fullText = $"[{GetTimeNowForLine()}] - {textLog}{exception}";
                lock (sync)
                {
                    File.AppendAllText(filename, fullText, encodingWin1251);
                }
            }
            catch { }
        }

        public static void WriteDelimiter()
        {
            try
            {
                string filename = GetLogFilePath();
                lock (sync)
                {
                    File.AppendAllText(filename, txtDelimiter+"\r\n", encodingWin1251);
                }
            }
            catch { }
        }

        public static String GetLogFilePath()
        {
            string logDir = GetLogFolder();
            string fullPathToFile = Path.Combine(logDir, $"{GetTimeNowForFolder()}.log");
            if (!File.Exists(fullPathToFile)) 
            {
                try
                {
                    lock (sync)
                    {
                        File.AppendAllText(fullPathToFile, String.Empty, encodingWin1251);
                    }
                } 
                catch { } 
            } 
            return fullPathToFile;
        }

        public static String GetLogFolder()
        {
            string logDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log");
            if (!Directory.Exists(logDir)) { Directory.CreateDirectory(logDir); }
            return logDir;
        }

        public static String GetLogLastRun()
        {
            string filename = GetLogFilePath();
            var txt = File.ReadAllText(filename, encodingWin1251);
            var spl = txt.Split(new string[] { txtDelimiter }, StringSplitOptions.None);
            var lastSessionBlock = spl[spl.Length - 1].Trim();
            var lines = lastSessionBlock.Split(new string[] { "\r\n" }, StringSplitOptions.None).Reverse();
            return String.Join("\r\n", lines);
        }

        public static String GetAllLog()
        {
            string filename = GetLogFilePath();
            var lines = File.ReadAllLines(filename, encodingWin1251).Reverse();
            return String.Join("\r\n", lines);
        }

        public static void DeleteOldLogs() 
        {
            var logFolder = GetLogFolder();
            var logFiles = Directory.GetFiles(logFolder);
            foreach (var logPath in logFiles)
            {
                var fileNameWe = Path.GetFileNameWithoutExtension(logPath).Trim();
                var DtNow = DateTime.Now;
                var logDate = parseDateString(fileNameWe, "yyyy_MM_dd");
                var td = (DtNow - logDate).TotalDays;
                if (td > dayToDelete) 
                {
                    try 
                    { 
                      File.Delete(logPath); 
                    } 
                    catch { } 
                }
            }
        }

        #endregion

    }
}
