using CurrencyTracker.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CurrencyTracker.Abstractions
{
    public interface ICurrencyApiService
    {
        Task<List<CurrencyModel>> GetCurrenciesAsync(DateTime endDate, CancellationToken ct = default);
        Task<List<RateShortModel>> GetDynamicsAsync(int curId, DateTime startDate, DateTime endDate, CancellationToken ct = default);
    }
}
