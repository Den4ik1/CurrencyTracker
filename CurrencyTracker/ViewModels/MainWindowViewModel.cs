using CurrencyTracker.Abstractions;
using CurrencyTracker.Infrastructure;
using CurrencyTracker.Models;
using CurrencyTracker.ViewModels.Base;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace CurrencyTracker.ViewModels
{
    internal class MainWindowViewModel : ViewModelBase
    {
        private readonly ICurrencyApiService _apiService;
        private readonly IFileStorageService _fileStorage;
        private readonly IDialogService _dialogService;

        public ICommand LoadCommand { get; }
        public MainWindowViewModel(ICurrencyApiService apiService, IFileStorageService fileStorage, IDialogService dialogService)
        {
            _apiService = apiService;
            _fileStorage = fileStorage;
            _dialogService = dialogService;

            //RelayCommand не работает с async. При появлении сключения мы не отследим ошибку. В рамках задания, я решил этим принеюречь
            SaveRatesCommand = new RelayCommand(async () => await SaveRatesAsync());
            LoadCommand = new RelayCommand(async () => await LoadCurrencyAsync());

            _ = LoadCurrencyAsync();
        }


        private string _title = "MegaTracker";

        public string Title
        {
            get => _title;

            set => Set(ref _title, value);
        }

        private DateTime _startDate = DateTime.Today.AddDays(-10);
        
        public DateTime StartDate
        {
            get => _startDate;
            set
            {
                if (!IsValidStartDate(value)) { OnPropertyChanged(nameof(StartDate)); return; }
                if (Set(ref _startDate, value))
                {
                    _ = LoadCurrencyAsync();
                }

            }
        }

        private DateTime _endDate = DateTime.Now;

        public DateTime EndDate
        {
            get => _endDate;
            set
            {
                if (!IsValidEndDate(value)) { OnPropertyChanged(nameof(EndDate)); return; }
                if (Set(ref _endDate, value))
                {
                    _ = LoadCurrencyAsync();
                }
            }
        }

        private bool IsValidStartDate(DateTime? value)
        {
            if ((_endDate - value.Value).Days > 365)
            {
                ShowWarning();
                return false;
            }
            return true;
        }
        
        private bool IsValidEndDate(DateTime? value)
        {
            if ((value.Value - _startDate).Days > 365)
            {
                ShowWarning();
                return false;
            }
            return true;
        }
        
        private void ShowWarning() =>

            _dialogService.ShowWarning("Превышен диапазон 365 дней. Изменение не применено.",
                     "Некорректное значение");

        private CancellationTokenSource _dynamicsCts;

        private CancellationTokenSource _currenciesCts;


        private ObservableCollection<CurrencyModel> _currency = new ObservableCollection<CurrencyModel>();
        public ObservableCollection<CurrencyModel> Currency
        {
            get => _currency;
            set => Set(ref _currency, value);
        }

        private CurrencyModel _selectedCurrency;
        public CurrencyModel SelectedCurrency
        {
            get => _selectedCurrency;
            set
            {
                if (!Set(ref _selectedCurrency, value)) return;
                if (value == null) return;
                _ = LoadDynamicsAsync(value.CurId);
            }
        }


        private ObservableCollection<RateShortModel> _rates = new ObservableCollection<RateShortModel>();
        public ObservableCollection<RateShortModel> Rates
        {
            get => _rates;
            set => Set(ref _rates, value);
        }

        public async Task LoadCurrencyAsync()
        {
            _currenciesCts?.Cancel();
            _currenciesCts = new CancellationTokenSource();
            var token = _currenciesCts.Token;

            try
            {
                var data = await _apiService.GetCurrenciesAsync(EndDate, token);
                token.ThrowIfCancellationRequested();
                Currency = new ObservableCollection<CurrencyModel>(data);

                var previousId = SelectedCurrency?.CurId;
                SelectedCurrency = Currency.FirstOrDefault(c => c.CurId == previousId)
                                   ?? Currency.FirstOrDefault();
            }
            catch (OperationCanceledException) when (_currenciesCts.IsCancellationRequested)
            {
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Ошибка загрузки: {ex.Message}");
            }
        }

        public async Task LoadDynamicsAsync(int CurId)
        {
            _dynamicsCts?.Cancel();
            _dynamicsCts = new CancellationTokenSource();
            var token = _dynamicsCts.Token;

            try
            {
                var data = await _apiService.GetDynamicsAsync(CurId, StartDate, EndDate, token);
                token.ThrowIfCancellationRequested();
                Rates = new ObservableCollection<RateShortModel>(data);
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Ошибка загрузки: {ex.Message}");
            }
        }

        public ICommand SaveRatesCommand { get; }

        private async Task SaveRatesAsync()
        {
            try
            {
                string path = await _fileStorage.SaveRatesAsync(SelectedCurrency, Rates);
                _dialogService.ShowInfo($"Сохранено: {path}", "Успех");
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Ошибка сохранения: {ex.Message}");
            }
        }
    }
}
