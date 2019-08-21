using System.Collections.Generic;
using System.Threading.Tasks;
using CzechNationalBank.Entities;

namespace CzechNationalBank.Clients
{
    /// <summary>
    /// Клиент Чешского национального банка
    /// </summary>
    public interface INationalBankClient
    {
        /// <summary>
        /// Получение валютных курсов за год
        /// </summary>
        /// <param name="year">Год</param>
        /// <returns>Список валютных курсов за год</returns>
        Task<List<ExchangeRate>> GetAnnualExchangeRates(int year);

        /// <summary>
        /// Получение валютных курсов за текущий день
        /// </summary>
        /// <returns>Список валютных курсов за день</returns>
        Task<List<ExchangeRate>> GetDailyExchangeRates();
    }
}