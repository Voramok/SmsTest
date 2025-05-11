using SmsTestWPFApp.Commands;
using SmsTestWPFApp.Models;
using SmsTestWPFApp.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace SmsTestWPFApp.ViewModels
{
    public sealed class MainViewModel : BaseViewModel
    {
        private IVariablesService _variablesService;

        public ObservableCollection<Variable> Variables { get; set; }

        public ICommand SaveCommand { get; }
        public ICommand LoadCommand { get; }

        public MainViewModel(IVariablesService variablesService)
        {
            _variablesService = variablesService;

            Variables = new ObservableCollection<Variable>();

            SaveCommand = new RelayCommand(SaveVariables);
            LoadCommand = new RelayCommand(LoadVariables);

            LoadVariables();
        }

        private void LoadVariables()
        {
            Variables.Clear();

            foreach (var variable in _variablesService.LoadVariables())
            {
                Variables.Add(variable);
            }
        }

        private void SaveVariables()
        {
            _variablesService.SaveVariables(Variables);
        }
    }
}
