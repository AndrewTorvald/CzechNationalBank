using System;

namespace CzechNationalBank.Entities
{
    /// <summary>
    /// Валютный курс
    /// </summary>
    public class ExchangeRate
    {
        /// <summary>
        /// Идентификатор валютного курса
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Дата
        /// </summary>
        public DateTimeOffset Date { get; set; }

        /// <summary>
        /// Код валюты
        /// </summary>
        public string Code { get; set; }
        
        /// <summary>
        /// Курс валюты
        /// </summary>
        public decimal Rate { get; set; }

        /// <summary />
        public ExchangeRate()
        {
            Id = Guid.NewGuid();
        }
    }
}