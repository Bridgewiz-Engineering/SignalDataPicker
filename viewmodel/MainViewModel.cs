using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SignalDataPicker.model;
using SignalDataPicker.service;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace SignalDataPicker.viewmodel
{
    internal class MainViewModel : ObservableObject
    {
        #region Properties
        public IAsyncRelayCommand LoadFileCommand { get => m_LoadFileCommand; }

        public bool IsWorking { get => m_IsWorking; }
        public string FileName { get => m_FileName; }
        public FileType SelectedFileType { get => m_SelectedFileType; set => SetProperty(ref m_SelectedFileType, value); }
        public List<FileType> FileTypes { get => Enum.GetValues(typeof(FileType)).Cast<FileType>().ToList(); }
        #endregion

        public MainViewModel(IAnalysisService analysisService, IFileService fileService, ILogService logService, IWindowService windowService)
        {
            m_AnalysisService = analysisService;
            m_FileService = fileService;
            m_LogService = logService;
            m_WindowService = windowService;

            m_LoadFileCommand = new AsyncRelayCommand(LoadFileAsync, LoadFileAsyncCanExecute);
        }


        #region Command Handlers
        async private Task LoadFileAsync()
        {
            m_FileData = await m_FileService.LoadFile(m_SelectedFileType);

            if (m_FileData == null)
            {
                m_WindowService.ShowErrorDialog("Dosya yüklenemedi.");
            }

        }
        #endregion


        #region Command States
        private bool LoadFileAsyncCanExecute()
        {
            return !m_IsWorking;
        }
        #endregion




        #region Fields
        private readonly IAnalysisService m_AnalysisService;
        private readonly IFileService m_FileService;
        private readonly ILogService m_LogService;
        private readonly IWindowService m_WindowService;


        private IAsyncRelayCommand m_LoadFileCommand;

        private bool m_IsWorking = false;
        private string m_FileName = string.Empty;
        private FileData? m_FileData = null;
        private FileType m_SelectedFileType = FileType.LordAccelerometer;

        private const int m_MaxDecimals = 5;
        #endregion
    }
}
