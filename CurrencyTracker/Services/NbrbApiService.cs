using CurrencyTracker.Abstractions;
using CurrencyTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace CurrencyTracker.Services
{
    public class NbrbApiService : ICurrencyApiService
    {
        private readonly HttpClient _client;

        public NbrbApiService(HttpClient client)
        {
            _client = client;
        }

        public async Task<List<CurrencyModel>> GetCurrenciesAsync(DateTime endDate, CancellationToken ct = default)
        {
            var response = await _client.GetAsync("exrates/currencies", ct).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            var currency = JsonSerializer.Deserialize<List<CurrencyModel>>(json)
                ?? new List<CurrencyModel>();

            var end = endDate.Date;
            return currency
                .Where(x => x.CurDateEnd == null || x.CurDateEnd.Value.Date >= end)
                .ToList();
        }

        public async Task<List<RateShortModel>> GetDynamicsAsync(int curId, DateTime startDate, DateTime endDate, CancellationToken ct = default)
        {
            string url = $"exrates/rates/dynamics/{curId}" +
                         $"?startdate={startDate:yyyy-MM-dd}" +
                         $"&enddate={endDate:yyyy-MM-dd}";

            var response = await _client.GetAsync(url, ct).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return JsonSerializer.Deserialize<List<RateShortModel>>(json)
                   ?? new List<RateShortModel>();
        }
    }
}
