using System;
using System.IO;
using System.Text;

namespace PodHead
{

    internal class ErrorLogger
    {
        private static readonly object _lock = new object();

        private static ErrorLogger _instance;

        private readonly IConfig _config;

        public event ErrorFoundEventHandler ErrorFound;

        public string ErrorLogPath
        {
            get
            {
                return Path.Combine(_config.AppDataFolder, "ErrorLog.txt");
            }
        }

        private ErrorLogger(IConfig config)
        {
            _config = config;
        }

        public static ErrorLogger Get(IConfig config)
        {
            lock(_lock)
            {
                if(_instance == null)
                {
                    _instance = new ErrorLogger(config);
                }
            }
            return _instance;
        }

        public void Log(Exception ex)
        {
            try
            {
                if (ex != null)
                {
                    Log(ex.ToString());
                }
            }
            catch { }
        }

        public void Log(string message)
        {
            using (var writer = File.Open(ErrorLogPath, FileMode.Append | FileMode.Create))
            {
                message += "\n" + new string('-', 100) + "\n";
                var dateBytes = Encoding.UTF8.GetBytes(DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString() + "\n");
                var messageBytes = Encoding.UTF8.GetBytes(message);
                writer.Write(dateBytes, 0, dateBytes.Length);
                writer.Write(messageBytes, 0, messageBytes.Length);
                OnErrorFound(message);
            }
        }

        private void OnErrorFound(string message)
        {
            var copy = ErrorFound;
            if(copy != null)
            {
                copy.Invoke(message);
            }
        }
    }
}
