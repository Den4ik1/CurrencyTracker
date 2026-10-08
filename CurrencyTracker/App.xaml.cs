using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using CurrencyTracker.Services;
using CurrencyTracker.ViewModels;
using CurrencyTracker.Abstractions;

namespace CurrencyTracker
{
    public partial class App : Application
    {
        private readonly IServiceProvider _services;

        public App()
        {
            var services = new ServiceCollection();

            services.AddHttpClient<ICurrencyApiService, NbrbApiService>(c =>
            {
                c.BaseAddress = new Uri("https://api.nbrb.by/");
                c.Timeout = TimeSpan.FromSeconds(30);
            });

            services.AddSingleton<MainWindowViewModel>();
            services.AddSingleton<IFileStorageService, FileStorageService>();
            services.AddSingleton<IDialogService, DialogService>();

            _services = services.BuildServiceProvider();
        }

        public static T GetService<T>()
      => ((App)Application.Current)._services.GetRequiredService<T>();
    }
}
