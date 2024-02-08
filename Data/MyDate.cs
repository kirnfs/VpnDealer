using System;
using System.Globalization;

namespace LogsAnalyzer.Data
{
    public class MyDate
    {
        public static String getTimeNowForDb()
        {
            var dtn = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            return dtn;
        }

        public static String getTimeNowForFolder()
        {
            var dtn = DateTime.Now.ToString("yyyy_MM_dd");
            return dtn;
        }

        public static String getTimeNowForFile()
        {
            var dtn = DateTime.Now.ToString("yyyy_MM_dd HH_mm_ss");
            return dtn;
        }
        public static Boolean tryParseDateString(String inputText, String pattern)
        {
            return DateTime.TryParseExact(inputText, pattern, CultureInfo.InvariantCulture, DateTimeStyles.None, out _);
        }
    }
}
