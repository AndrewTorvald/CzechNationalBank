using System;
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
        private readonly INationalBankClient _client;
        private readonly DatabaseContext _context;

        public string Description => "Заполнение базы данными. Аргументы : --year <год>";

        public UpdateExchangeRatesCommand(INationalBankClient client, DatabaseContext context)
        {
            _client = client;
            _context = context;
        }

        public async Task Execute(string[] args)
        {
            var data = new List<ExchangeRate>();
            var years = new List<int>();

            for (var i = 0; i < args.Length; i++)
            {
                var arg = args[i];
                switch (arg)
                {
                    case "--year":
                    case "-y":
                    {
                        var value = args[++i];

                        if (!int.TryParse(value, out var year))
                        {
                            throw new Exception($"Невалидное значение для аргумента {arg}");
                        }
                        years.Add(year);
                        break;
                    }
                }
            }

            foreach (var year in years)
            {
                data.AddRange(await _client.GetAnnualExchangeRates(year));
            }
            
            var existingData = await _context.ExchangeRates
                .Where(a => years.Contains(a.Date.Year))
                .ToListAsync();

            data = data.Where(a => !existingData.Any(existing => 
                existing.Code == a.Code && existing.Date == a.Date)).ToList();
            
            _context.ExchangeRates.AddRange(data);
            await _context.SaveChangesAsync();
        }
    }
}