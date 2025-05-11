using Microsoft.Extensions.Configuration;
using NLog;
using System;
using System.IO;
using System.Windows;

namespace SmsTestDesktopApp
{
    public class Program
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        [STAThread]
        public static void Main(string[] args)
        {
            string logDirectory = "Logs";

            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
                logger.Info($"Директория '{logDirectory}' была создана.");
            }
            else
            {
                logger.Info($"Директория '{logDirectory}' уже существует.");
            }

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            try
            {
                logger.Info("Запуск приложения");
                
                var app = new App();
                app.InitializeComponent();
                app.Run(new MainWindow());
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Произошла ошибка при запуске приложения");
                MessageBox.Show("Произошла ошибка: " + ex.Message);
            }
        }
    }
}
