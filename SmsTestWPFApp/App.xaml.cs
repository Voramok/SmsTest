using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using SmsTestWPFApp.Services;
using SmsTestWPFApp.ViewModels;
using SmsTestWPFApp.Views;
using System;
using System.IO;
using System.Windows;

namespace SmsTestWPFApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        public IConfiguration Configuration { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            InitializeLoggingDirectory("Logs");
            InitializeFile("variables.json");

            Configuration = BuildConfiguration();

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            var serviceProvider = serviceCollection.BuildServiceProvider();

            var mainWindow = serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        private void InitializeLoggingDirectory(string logDirectory)
        {
            try
            {
                if (!Directory.Exists(logDirectory))
                {
                    Directory.CreateDirectory(logDirectory);
                    logger.Info($"Директория '{logDirectory}' была создана.");
                }
                else
                {
                    logger.Info($"Директория '{logDirectory}' уже существует.");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Ошибка при создании директории логов.");
            }
        }

        private void InitializeFile(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    using (File.Create(filePath)) { } // Используем using для автоматического закрытия потока
                    logger.Info($"Файл '{filePath}' был создан.");
                }
                else
                {
                    logger.Info($"Файл '{filePath}' уже существует.");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"Ошибка при работе с файлом '{filePath}'.");
            }
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.AddNLog("Nlog.config");
            });

            var configuration = BuildConfiguration();
            services.AddSingleton<IConfiguration>(configuration);

            services.AddSingleton<IVariablesService, VariablesService>();

            services.AddTransient<MainViewModel>();
            services.AddTransient<MainWindow>();
        }

        private IConfiguration BuildConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            return builder.Build();
        }
    }
}
