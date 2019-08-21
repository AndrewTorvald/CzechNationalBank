using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CzechNationalBank.Entities;

namespace CzechNationalBank.Clients
{
    /// <inheritdoc />
    public class NationalBankClient : INationalBankClient
    {
        private readonly HttpClient _client;

        /// <inheritdoc />
        public NationalBankClient(HttpClient client)
        {
            _client = client;
        }

        /// <inheritdoc />
        public async Task<List<ExchangeRate>> GetAnnualExchangeRates(int year)
        {
            var response = await _client.GetAsync(
                $"en/financial_markets/foreign_exchange_market/exchange_rate_fixing/year.txt?year={year}");
            
            if (!response.IsSuccessStatusCode)
                throw new Exception($"Ошибка запроса на {_client.BaseAddress}");
            
            var fileStream = await response.Content.ReadAsStreamAsync();
            var streamReader = new StreamReader(fileStream, System.Text.Encoding.UTF8);

            var result = new List<ExchangeRate>();

            var header = streamReader.ReadLine()
                .Split('|')
                .Skip(1)
                .Select(substring => substring.Split())
                .Select(currency => new
                {
                    Amount = currency[0],
                    Code = currency[1]
                }).ToList();

            string line;
            while ((line = streamReader.ReadLine()) != null)
            {
                var substrings = line.Split('|');
                var date =
                    DateTimeOffset.ParseExact(substrings.First(), "dd.MM.yyyy", 
                        CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
                
                var data = substrings.Skip(1).Select((rate, index) => new ExchangeRate
                {
                    Rate = decimal.Parse(rate) * int.Parse(header[index].Amount),
                    Date = date,
                    Code = header[index].Code
                });
                result.AddRange(data);
            }

            return result;
        }
        
        public async Task<List<ExchangeRate>> GetDailyExchangeRates()
        {
            var response = await _client.GetAsync(
                $"en/financial_markets/foreign_exchange_market/exchange_rate_fixing/daily.txt");
            
            if (!response.IsSuccessStatusCode)
                throw new Exception($"Ошибка запроса на {_client.BaseAddress}");
            
            var fileStream = await response.Content.ReadAsStreamAsync();
            var streamReader = new StreamReader(fileStream, System.Text.Encoding.UTF8);

            var result = new List<ExchangeRate>();

            var header = streamReader.ReadLine().Split('#').First();
            header = header.Remove(header.Length - 1);

            var rateDate = 
                DateTimeOffset.ParseExact(header, "dd MMM yyyy", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
            
            streamReader.ReadLine();

            string line;
            while ((line = streamReader.ReadLine()) != null)
            {
                var substrings = line.Split('|');
                var data = new ExchangeRate
                {
                    Date = rateDate,
                    Code = substrings[3],
                    Rate = decimal.Parse(substrings[4]) * int.Parse(substrings[2])
                };
                result.Add(data);
            }

            return result;
        }
    }
}