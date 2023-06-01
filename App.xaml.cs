using Microsoft.Extensions.DependencyInjection;
using SignalDataPicker.service;
using SignalDataPicker.service.implementation;
using SignalDataPicker.viewmodel;
using System;
using System.Windows;

namespace SignalDataPicker
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            ServiceProvider = ConfigureServices();
            this.InitializeComponent();
        }

        public new static App Current => (App)Application.Current;
        public IServiceProvider ServiceProvider { get; }

        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            services.AddSingleton<IAnalysisService, AnalysisService>();
            services.AddSingleton<IFileService, FileService>();
            services.AddSingleton<ILogService, LogService>();
            services.AddSingleton<IWindowService, WindowService>();
            services.AddTransient<MainViewModel>();
            services.AddTransient<ProcessingViewModel>();

            return services.BuildServiceProvider();
        }

    }
}
