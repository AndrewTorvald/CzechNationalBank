using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using static System.Console;

namespace CzechNationalBank.Console.UI
{
    /// <summary>
    /// Интерфейс командной строки
    /// </summary>
    public class CommandLineInterface
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<CommandLineInterface> _logger;
        private readonly Dictionary<string, Type> _commands;
        
        /// <summary/>
        public CommandLineInterface(IServiceProvider serviceProvider, ILogger<CommandLineInterface> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _commands = new Dictionary<string, Type>();
        }

        
        /// <summary>
        /// Регистрация команды 
        /// </summary>
        public void AddCommand<T>(string name)
            where T : ICommand
        {
            _commands.Add(name, typeof(T));
        }
        
        /// <summary>
        /// Запуск обработчика команд консоли
        /// </summary>
        public async Task Run(string[] args)
        {
            if (args.Length == 0 || args[0] == "--help")
            {
                ShowHelp();
                return;
            }
            
            var commandName = args[0];
            var commandArgs = args.Skip(1).ToArray();
            await ExecuteCommand(commandName, commandArgs);
        }

        private async Task ExecuteCommand(string commandName, string[] args)
        {
            if (!_commands.TryGetValue(commandName, out var commandType))
            {
                throw new ArgumentException($"Команда с именем {commandName} не найдена");
            }
            var command = (ICommand) _serviceProvider.GetService(commandType);
            
            _logger.LogInformation($"Выполнение команды {commandName}");
            await command.Execute(args);
            _logger.LogInformation($"Команда {commandName} успешно завершена");
        }
        
        private void ShowHelp()
        {
            WriteLine("Консольное приложение для работы с Чешским национальным банком");
            WriteLine("==============================================================");
            
            WriteLine("Доступные команды:");
            foreach (var (commandName, commandType) in _commands)
            {
                var command = (ICommand) _serviceProvider.GetService(commandType);

                WriteLine($"\t{commandName} - {command.Description}");
            }
        }
    }
}
