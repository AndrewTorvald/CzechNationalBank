using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CzechNationalBank.Clients;
using CzechNationalBank.Entities;
using CzechNationalBank.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CzechNationalBank.Console.UI.Commands
{
    public class UpdateExchangeRatesCommand : ICommand
    {
        private readonly NationalBankClient _client;
        private readonly DatabaseContext _context;

        public string Description => "Заполнение базы данными за 2018 и 2019 год";

        public UpdateExchangeRatesCommand(NationalBankClient client, DatabaseContext context)
        {
            _client = client;
            _context = context;
        }

        public async Task Execute(string[] args) //TODO args - set year range
        {
            var data = new List<ExchangeRate>();
            data.AddRange(await _client.GetAnnualExchangeRates(2018));
            data.AddRange(await _client.GetAnnualExchangeRates(2019));
            
            var existingData = await _context.ExchangeRates
                .Where(a => a.Date.Year == 2018 || a.Date.Year == 2019)
                .ToListAsync();

            data = data.Where(a => !existingData.Any(existing => 
                existing.Code == a.Code && existing.Date == a.Date)).ToList();
            
            _context.ExchangeRates.AddRange(data);
            await _context.SaveChangesAsync();
        }
    }
}