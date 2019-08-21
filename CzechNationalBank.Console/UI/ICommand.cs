using System.Threading.Tasks;

namespace CzechNationalBank.Console.UI
{
    /// <summary>
    /// Интерфейс команды консольного интерфейса
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Справка по команде
        /// </summary>
        string Description { get; }
        
        /// <summary>
        /// Выполнить команду
        /// </summary>
        /// <param name="args">Аргументы</param>
        Task Execute(string[] args);
    }
}