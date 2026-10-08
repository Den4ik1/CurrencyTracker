using System;
using System.Text.Json.Serialization;

namespace CurrencyTracker.Models
{
    public class CurrencyModel
    {
        [JsonPropertyName("Cur_ID")]
        public int CurId { get; set; }

        [JsonPropertyName("Cur_Code")]
        public string CurCode { get; set; }

        [JsonPropertyName("Cur_Abbreviation")]
        public string CurAbbreviation { get; set; }

        [JsonPropertyName("Cur_Name")]
        public string CurName { get; set; }

        [JsonPropertyName("Cur_DateStart")]
        public DateTime? CurDateStart { get; set; }

        [JsonPropertyName("Cur_DateEnd")]
        public DateTime? CurDateEnd { get; set; }
    }
}
