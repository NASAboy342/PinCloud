
using APinI.Helppers;
using System.Text;

namespace APinI.Schedular.Jobs
{
    public class LogMoving : JobBase
    {
        public override TimeSpan Interval { get; set; } = TimeSpan.FromSeconds(5);

        public override void Execute(Object o)
        {
            var yesterday = DateTime.Now.AddDays(-1).ToString("dd-MM-yyyy");
            if (!IsTheYesterdayFileWasMoved(yesterday))
            {
                LoggerHelper.Log($"Log file was tried to end here");
                MoveLogFileToNewFile(yesterday);
                LoggerHelper.Log($"Yesterday log file has been moved to a different file: [{yesterday}.txt]");
            }
        }

        private void MoveLogFileToNewFile(string yesterday)
        {
            var newTargetFile = CreateNewFile(yesterday);
            var logDataTobeMove = GetEntierCurrentLogData();
            WriteCurrentLogDataIntoCreatedFile(newTargetFile, logDataTobeMove);
            CleanAllLogDataFromCurrentLogFile();
        }

        private void CleanAllLogDataFromCurrentLogFile()
        {
            var currentLogFile = File.Open($"{LoggerHelper.LogBasePath}{LoggerHelper.LogFileName}",FileMode.Create);
            currentLogFile.SetLength(0);
            currentLogFile.Close();
        }

        private void WriteCurrentLogDataIntoCreatedFile(FileStream newTargetFile, string logDataTobeMoved)
        {
            newTargetFile.Write(Encoding.UTF8.GetBytes(logDataTobeMoved));
            newTargetFile.Close();
        }

        private string GetEntierCurrentLogData()
        {
            var logFile = File.OpenRead($"{LoggerHelper.LogBasePath}{LoggerHelper.LogFileName}");
            var entierCurrentLogData = new StreamReader(logFile).ReadToEnd();
            logFile.Close();
            return entierCurrentLogData;
        }

        private FileStream CreateNewFile(string yesterday)
        {
            var targetFileName = $"{yesterday}.txt";
            return File.Create($"{LoggerHelper.LogBasePath}{targetFileName}");
        }

        private bool IsTheYesterdayFileWasMoved(string yesterday)
        {
            var targetFileName = $"{yesterday}.txt";
            return File.Exists($"{LoggerHelper.LogBasePath}{targetFileName}");
        }
    }
}
