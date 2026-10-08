using CurrencyTracker.Abstractions;
using CurrencyTracker.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace CurrencyTracker.Services
{
    public class FileStorageService : IFileStorageService
    {
        private static readonly JsonSerializerOptions WriteOptions = new JsonSerializerOptions()
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };


        private static string GetProjectDataDirectory()
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string projectDir = Path.GetFullPath(Path.Combine(baseDir, @"..\..\"));
            return Path.Combine(projectDir, "Data");
        }

        public async Task<string> SaveRatesAsync(CurrencyModel currency, IEnumerable<RateShortModel> rates, string fileName = "rates.json")
        {
            if (rates is null)
                throw new ArgumentNullException(nameof(rates));

            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("Имя файла не может быть пустым.", nameof(fileName));

            if (fileName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
                throw new ArgumentException("Имя файла содержит недопустимые символы.", nameof(fileName));

            List<FileJsonModel> data = new List<FileJsonModel>();
            foreach (var item in rates)
            {
                data.Add(
                  new FileJsonModel
                  {
                      Date = item.Date,
                      OfficialRate = item.CurOfficialRate,
                      Name = currency.CurName,
                      Abbreviation = currency.CurAbbreviation
                  }
                );
            }

            string dataDir = GetProjectDataDirectory();
            Directory.CreateDirectory(dataDir);

            string filePath = Path.Combine(dataDir, fileName);

            using (var stream = new FileStream(
                filePath,
                FileMode.Create,
                FileAccess.Write,
                FileShare.None,
                bufferSize: 4096,
                useAsync: true))
            {
                await JsonSerializer.SerializeAsync(stream, data, WriteOptions).ConfigureAwait(false); ;
            }

            return filePath;
        }
    }
}
