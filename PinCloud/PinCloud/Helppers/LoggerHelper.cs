using System.Text;

namespace APinI.Helppers
{
    public class LoggerHelper
    {
        /// <summary> The base path to the log files. End with slash "/" </summary>
        public static string LogBasePath => "D:\\log\\LocalLog\\";

        /// <summary>
        /// Name of the log file
        /// </summary>
        public static object LogFileName => "ApinI.txt";

        public static void Log(string message)
        {
            var LogFile = File.OpenWrite($"{LogBasePath}{LogFileName}");

            LogFile.Seek(0, SeekOrigin.End);
            LogFile.Write(Encoding.UTF8.GetBytes($"[{DateTime.Now}] ====== {message}\n"));
            LogFile.Close();
        }
    }
}
