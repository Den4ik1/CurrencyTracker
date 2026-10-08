using System;

namespace CurrencyTracker.Models
{
    internal class FileJsonModel
    {
        public DateTime? Date { get; set; }
        public string Abbreviation { get; set; }
        public string Name { get; set; }
        public double OfficialRate { get; set; }
    }
}
