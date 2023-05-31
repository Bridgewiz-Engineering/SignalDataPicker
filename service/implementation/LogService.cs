using System;
using System.IO;
using System.Threading;

namespace SignalDataPicker.service.implementation
{
    internal class LogService : ILogService
    {
        public void LogError(string message)
        {
            semaphoreSlim.Wait();
            File.AppendAllText(LOG_FILE_NAME, $"{DateTime.Now} - [ERROR] - {message}{Environment.NewLine}");
            semaphoreSlim.Release();
        }

        #region Fields
        private const string LOG_FILE_NAME = "log.txt";
        private static readonly SemaphoreSlim semaphoreSlim = new(1, 1);
        #endregion
    }
}
