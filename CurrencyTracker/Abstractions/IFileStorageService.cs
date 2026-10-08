using CurrencyTracker.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CurrencyTracker.Abstractions
{
    public interface IFileStorageService
    {
        Task<string> SaveRatesAsync(CurrencyModel currency, IEnumerable<RateShortModel> rates, string fileName = "rates.json");
    }
}
