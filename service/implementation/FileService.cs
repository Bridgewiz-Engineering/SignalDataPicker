using Microsoft.Win32;
using SignalDataPicker.model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SignalDataPicker.service.implementation
{
    internal class FileService : IFileService
    {
        public FileService(ILogService logService)
        {
            m_LogService = logService;
        }

        public async Task<bool> SaveFile(FileData fileData, OutputType outputType, DataAxis dataAxis, int startIndex, int endIndex)
        {
            var success = false;
            if (fileData.FilteredData.Count == 0) return success;

            var currentFolder = Path.GetDirectoryName(fileData.FileName);
            var currentFileName = Path.GetFileNameWithoutExtension(fileData.FileName);
            var targetFileName = $"{currentFileName}_{dataAxis}_{startIndex}_{endIndex}.csv";

            SaveFileDialog sfd = new()
            {
                AddExtension = true,
                Filter = "CSV|*.csv",
                InitialDirectory = currentFolder,
                FileName = targetFileName,
                OverwritePrompt = true,
                Title = "Kaydet",
            };
            if (sfd.ShowDialog() == true)
            {
                targetFileName = sfd.FileName;
                switch (outputType)
                {
                    case OutputType.SeismoSignal:
                        success = await SaveSeismoSignalData(targetFileName, fileData.FilteredData);
                        break;
                    default:
                        break;
                }
            }
            return success;
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
                        AllData = data ?? new List<Record>()
                    };
                }
                catch (Exception ex)
                {
                    m_LogService.LogError($"Error opening file {fileName}: {ex.Message}");
                }
            }
            return fileData;
        }
        #region Private Methods
        private static async Task<List<Record>> ParseLordAccelerometerData(string[] data) =>

            await Task.Run(() => data.Skip(29).ToList().ConvertAll(x =>
            {
                var splitted = x.Split(';');
                return new Record
                {
                    TimeStamp = splitted[0],
                    Index = int.Parse(splitted[1],CultureInfo.InvariantCulture),
                    X = double.Parse(splitted[2],CultureInfo.InvariantCulture),
                    Y = double.Parse(splitted[3], CultureInfo.InvariantCulture),
                    Z = double.Parse(splitted[4], CultureInfo.InvariantCulture)
                };
            }
            ));

        private async Task<bool> SaveSeismoSignalData(string fileName, List<double> data)
        {
            bool success = false;
            try
            {
                await File.WriteAllLinesAsync(fileName, data.ConvertAll(q => q.ToString(CultureInfo.InvariantCulture)));
                success = true;
            }
            catch (Exception ex)
            {
                m_LogService.LogError($"Error saving file {fileName}: {ex.Message}");
            }
            return success;
        }

        #endregion

        #region Fields
        private readonly ILogService m_LogService;
        #endregion
    }
}
