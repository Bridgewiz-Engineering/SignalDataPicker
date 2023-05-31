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

        public bool IsWorking { get => m_IsWorking; private set => SetProperty(ref m_IsWorking, value);     }
        public FileType SelectedFileType { get => m_SelectedFileType; set => SetProperty(ref m_SelectedFileType, value); }
        public static List<FileType> FileTypes { get => Enum.GetValues(typeof(FileType)).Cast<FileType>().ToList(); }
        public FileData? FileData { get => m_FileData; private set => SetProperty(ref m_FileData, value); }
        public int StartIndex { get => m_StartIndex; set => SetProperty(ref m_StartIndex, value); }
        public int EndIndex { get => m_EndIndex; set => SetProperty(ref m_EndIndex, value); }
        public int StartIndexMaximum { get => m_StartIndexMaximum; private set => SetProperty(ref m_StartIndexMaximum, value); }    
        public int EndIndexMaximum { get => m_EndIndexMaximum; private set => SetProperty(ref m_EndIndexMaximum, value); }

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
            FileData = await m_FileService.LoadFile(m_SelectedFileType);

            if (m_FileData == null)
            {
                m_WindowService.ShowErrorDialog("Dosya yüklenemedi.");
            }
            else
            {
                EndIndex = m_FileData.Data.Count;
                StartIndexMaximum = m_FileData.Data.Count;
                EndIndexMaximum = m_FileData.Data.Count;
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
        private FileData? m_FileData = null;
        private FileType m_SelectedFileType = FileType.LordAccelerometer;

        
        private int m_StartIndex = 1;
        private int m_EndIndex = 1;
        private int m_StartIndexMaximum = 1;
        private int m_EndIndexMaximum = 1;
        #endregion
    }
}
