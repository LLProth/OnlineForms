using gov.nd.itd.util.logging;
using System;
using System.Configuration;
using OnlineForms.Email;

namespace OnlineForms.Logging
{
    public class LoggingService
    {
        public static string AgencyAbbreviation = "WSI";
        public static string LoggerApplicationName = "myWSI Submit Webservice";
        public static string LoggerCodeModuleName = "App";
        public static readonly Logger logItd = new Logger(LoggerApplicationName, LoggerCodeModuleName, AgencyAbbreviation);
        private static readonly log4net.ILog logWsi = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public void Debug(string message)
        {
            LogMessage(message);// logWsi.Debug(message);
            //logItd.LogDebug(message);
        }

        public void Debug(string message, Exception ex)
        {
            LogError(message, ex);//logWsi.Debug(message, ex);
            //logItd.LogError(ex);
        }

        public void Info(string message)
        {
            LogMessage(message);//logWsi.Info(message);
            //logItd.LogDebug(message);
        }

        public void Info(string message, Exception ex)
        {
            LogError(message, ex);//logWsi.Info(message, ex);
            //logItd.LogError(ex);
        }

        public void Warn(string message)
        {
            LogMessage(message);//logWsi.Warn(message);
            //logItd.LogDebug(message);
        }

        public void Warn(string message, Exception ex)
        {
            LogError(message, ex);//.Warn(message, ex);
            //logItd.LogError(ex);
        }

        public void Error(string message)
        {
            LogMessage(message);//logWsi.Error(message);
            //logItd.LogDebug(message);
        }

        public void Error(string message, Exception ex)
        {
            LogError(message, ex);//logWsi.Error(message, ex);
            //logItd.LogError(ex);
        }

        public void Fatal(string message)
        {
            LogMessage(message);//logWsi.Fatal(message);
            //logItd.LogDebug(message);
        }

        public void Fatal(string message, Exception ex)
        {
            LogError(message, ex);//logWsi.Fatal(message, ex);
            //logItd.LogError(ex);
        }

        public void LogMessage(string message, int logLevel = 0)
        {
            LogFile(message, "log", logLevel);
            LogItdService(message, null);
        }

        public void LogError(string message, Exception ex, int logLevel = 0)
        {
            string s = ex.Message + "\n" + ex.StackTrace;
            LogFile(message + "\n" + s, "log", logLevel);
            LogFile(message + "\n" + s, "error", logLevel);
            LogItdService(message, ex);
            LogEmail("[" + ConfigurationManager.AppSettings["Environment"].ToUpper() + "] " + message, ex);
        }

        private void LogEmail(string message, Exception ex)
        {
            string[] toAddresses = ConfigurationManager.AppSettings["AdminEmail"].Split(';');
            string subject = message;
            string content = ex.Message + "\n" + ex.StackTrace;
            bool html = false;

            try
            {
                EmailService.SendEmail("", toAddresses, null, subject, content, html);
            }
            catch { }
        }

        private void LogFile(string message, string logType, int logLevel)
        {
            for (int i = 0; i < 5; i++)
            {
                try
                {
                    FileLogging.WriteLog(ConfigurationManager.AppSettings["Environment"] + " " + message, logType, logLevel);
                    break;
                }
                catch { }
            }
        }

        private void LogItdService(string message, Exception ex)
        {
            if (ex == null)
            {
                //logItd.LogDebug(message);
            }
            else
            {
                //logItd.LogError(ex);
            }
        }
    }
}