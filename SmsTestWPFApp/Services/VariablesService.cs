using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SmsTestWPFApp.Models;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;

namespace SmsTestWPFApp.Services
{
    public class VariablesService : IVariablesService
    {
        private const string FilePath = "variables.json";
        private readonly IConfiguration _configuration;
        private readonly ILogger<VariablesService> _logger;

        public VariablesService(IConfiguration configuration, ILogger<VariablesService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public ObservableCollection<Variable> LoadVariables()
        {
            _logger.LogInformation($"Загрузка переменных из {FilePath}");

            var variablesNames = _configuration.GetSection("VariablesNames").Get<string[]>() ?? Array.Empty<string>();
            var variables = new ObservableCollection<Variable>();

            if (File.Exists(FilePath))
            {
                try
                {
                    var json = File.ReadAllText(FilePath);
                    var savedVariables = JsonSerializer.Deserialize<List<Variable>>(json) ?? new List<Variable>();

                    foreach (var variableName in variablesNames)
                    {
                        var tempVariable = savedVariables.FirstOrDefault(v => v.Field == variableName);
                        if (tempVariable != null)
                        {
                            variables.Add(new Variable
                            {
                                Field = tempVariable.Field,
                                Value = tempVariable.Value,
                                Comment = tempVariable.Comment
                            });

                            _logger.LogTrace($"Переменная {variableName} загружена");
                        }
                        else
                        {
                            variables.Add(new Variable
                            {
                                Field = variableName,
                                Value = string.Empty,
                                Comment = string.Empty
                            });

                            _logger.LogTrace($"Переменная {variableName} создана");
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ошибка при загрузке переменных из файла");
                }
            }
            else
            {
                _logger.LogWarning($"Файл {FilePath} не найден. Будут созданы новые переменные.");
                foreach (var variableName in variablesNames)
                {
                    variables.Add(new Variable
                    {
                        Field = variableName,
                        Value = string.Empty,
                        Comment = string.Empty
                    });

                    _logger.LogTrace($"Переменная {variableName} создана");
                }
            }

            return variables;
        }

        public void SaveVariables(ObservableCollection<Variable> variables)
        {
            _logger.LogInformation($"Сохраняем переменные в {FilePath}");

            try
            {
                var json = JsonSerializer.Serialize(variables);
                File.WriteAllText(FilePath, json);

                _logger.LogInformation($"Переменные сохранены в {FilePath}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при сохранении переменных в файл");
            }
        }
    }
}
