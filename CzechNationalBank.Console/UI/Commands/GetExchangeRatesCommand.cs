using System.Linq;
using System.Threading.Tasks;
using CzechNationalBank.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CzechNationalBank.Console.UI.Commands
{
    public class GetExchangeRatesCommand : ICommand
    {
        private readonly DatabaseContext _context;

        public string Description => "Получение данных из базы";

        public GetExchangeRatesCommand(DatabaseContext context) 
        {
            _context = context;
        }

        public async Task Execute(string[] args) //TODO count/offset/search args
        {
            var data = await _context.ExchangeRates.AsNoTracking().ToListAsync();

            if (!data.Any())
            {
                System.Console.WriteLine("Данные отсутствуют");
            }
            data.ForEach(item => System.Console.WriteLine($"{item.Date:d}-{item.Code}-{item.Rate}"));
        }
    }
}