using SmsTestWPFApp.Models;
using System.Collections.ObjectModel;

namespace SmsTestWPFApp.Services
{
    public interface IVariablesService
    {
        ObservableCollection<Variable> LoadVariables();
        void SaveVariables(ObservableCollection<Variable> variables);
    }
}
