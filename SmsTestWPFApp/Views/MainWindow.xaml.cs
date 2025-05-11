using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SmsTestWPFApp.ViewModels;
using System.Windows;

namespace SmsTestWPFApp.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(MainViewModel mainViewModel)
        {
            InitializeComponent();
            DataContext = mainViewModel;
        }
    }
}
