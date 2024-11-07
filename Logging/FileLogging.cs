using System;
using System.Configuration;
using System.IO;

namespace OnlineForms.Logging
{
    public static class FileLogging
    {
        public static bool WriteLog(string LogText, string LogType = "log", int LogLevel = 0)
        {
            string logFolder = ConfigurationManager.AppSettings["LogFilePath"];
            string fileName = DateTime.Today.ToString("yyyyMMdd") + "_" + LogType + ".txt";
            string filePath = logFolder + fileName;

            try
            {
                if (File.Exists(filePath))
                {
                    File.AppendAllText(filePath, DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "\t" + LogText + "\r\n");
                }
                else
                {
                    File.Create(filePath).Dispose();
                    File.AppendAllText(filePath, DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "\t" + LogText + "\r\n");
                }
            }
            catch { }
            
            return true;
        }
    }
}