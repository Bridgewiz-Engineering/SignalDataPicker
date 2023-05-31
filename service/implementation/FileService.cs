using Microsoft.Win32;
using SignalDataPicker.model;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace SignalDataPicker.service.implementation
{
    internal class FileService : IFileService
    {
        public FileService(ILogService logService)
        {
            m_LogService = logService;
        }
        async public Task<FileData?> LoadFile(FileType fileType)
        {
            FileData? fileData = null;

            OpenFileDialog ofd = new()
            {
                Filter = "CSV|*.csv|TXT|*.txt|Hepsi|*.*"
            };
            if (ofd.ShowDialog() == true)
            {
                var fileName = ofd.FileName;
                try
                {
                    var allData = await File.ReadAllLinesAsync(fileName);
                    var data = fileType switch
                    {
                        FileType.LordAccelerometer => await ParseLordAccelerometerData(allData),
                        _ => null
                    };
                    fileData = new FileData
                    {
                        FileName = fileName,
                        Data = data ?? new List<Record>()
                    };
                }
                catch (Exception ex)
                {
                    m_LogService.LogError(ex.Message);
                }
            }
            return fileData;
        }
        #region Private Methods
        async private Task<List<Record>> ParseLordAccelerometerData(string[] data) =>

            await Task.Run(() => data.Skip(29).ToList().ConvertAll(x =>
            {
                var splitted = x.Split(',');
                return new Record
                {
                    TimeStamp = splitted[0],
                    Index = int.Parse(splitted[1]),
                    X = double.Parse(splitted[2]),
                    Y = double.Parse(splitted[3]),
                    Z = double.Parse(splitted[4])
                };
            }
            ));
        #endregion

        #region Fields
        private readonly ILogService m_LogService;
        #endregion
    }
}
