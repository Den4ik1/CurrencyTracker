using System;
using System.Text.Json.Serialization;

namespace CurrencyTracker.Models
{
    public class RateShortModel
    {
        [JsonPropertyName("Cur_ID")]
        public int CurId { get; set; }

        [JsonPropertyName("Date")]
        public DateTime Date { get; set; }

        [JsonPropertyName("Cur_OfficialRate")]
        public double CurOfficialRate { get; set; }
    }
}
