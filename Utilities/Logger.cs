using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

namespace boilerplate_desktop_dotnet.Utilities
{
    // internal class Logger
    // {
    // }

    public static class Logger
    {
        private static readonly string LogPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
        
        static Logger()
        {
            if (!Directory.Exists(LogPath))
            {
                Directory.CreateDirectory(LogPath);
            }
        }
        
        public static void LogInfo(string message)
        {
            WriteLog("INFO", message);
        }
        
        public static void LogError(string message, Exception ex = null)
        {
            string fullMessage = ex != null ? $"{message} - {ex.Message}" : message;
            WriteLog("ERROR", fullMessage);
        }
        
        private static void WriteLog(string level, string message)
        {
            try
            {
                string fileName = $"log_{DateTime.Now:yyyyMMdd}.txt";
                string filePath = Path.Combine(LogPath, fileName);
                string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{level}] {message}";
                
                File.AppendAllText(filePath, logEntry + Environment.NewLine);
            }
            catch
            {
                // Ignore logging errors
            }
        }
    }
}
