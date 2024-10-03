using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Application.Interfaces.Utilities;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Insp.Application.Interfaces.Transactions;
using IARA.Mobile.Insp.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Helpers;
using ISettings = IARA.Mobile.Insp.Application.Interfaces.Utilities.ISettings;

namespace IARA.Mobile.Insp.FlyoutPages.SettingsPage
{
    public class SettingsViewModel : PageViewModel
    {
        private readonly IAppDbMigration _appDbMigration;
        private readonly IDbSettings _dbSettings;
        private readonly ISettings _settings;
        private readonly IStartupTransaction _startupTransaction;
        private readonly INomenclatureDatesClear _nomenclatureDates;
        private readonly IInspectionsTransaction _inspectionsTransaction;

        private List<SelectNomenclatureDto> _fleets;
        private bool _hasFleetFilter;
        private double _fontSize;

        public SettingsViewModel(ISettings settings, IAppDbMigration appDbMigration, IDbSettings dbSettings, INomenclatureDatesClear nomenclatureDates, IInspectionsTransaction inspectionsTransaction, IStartupTransaction startupTransaction)
        {
            _settings = settings;
            _startupTransaction = startupTransaction;
            _appDbMigration = appDbMigration;
            _dbSettings = dbSettings;
            _nomenclatureDates = nomenclatureDates;
            _inspectionsTransaction = inspectionsTransaction;

            FontSizeChanged = CommandBuilder.CreateFrom(OnFontSizeChanged);
            FleetFilterChanged = CommandBuilder.CreateFrom(OnFleetFilterChanged);
            ResetDatabase = CommandBuilder.CreateFrom(OnResetDatabase);

            SelectedFleets = new TLObservableCollection<SelectNomenclatureDto>();
        }

        public DateTime Now { get; set; } = DateTime.Now;

        public double FontSize
        {
            get => _fontSize;
            set => SetProperty(ref _fontSize, value);
        }

        public bool HasFleetFilter
        {
            get => _hasFleetFilter;
            set => SetProperty(ref _hasFleetFilter, value);
        }

        public TLObservableCollection<SelectNomenclatureDto> SelectedFleets { get; }

        public List<SelectNomenclatureDto> Fleets
        {
            get => _fleets;
            set => SetProperty(ref _fleets, value);
        }

        public ICommand FontSizeChanged { get; }
        public ICommand FleetFilterChanged { get; }
        public ICommand ResetDatabase { get; }

        public override GroupResourceEnum[] GetPageIndexes()
        {
            return new[] { GroupResourceEnum.Settings };
        }

        public override Task Initialize(object sender)
        {
            FontSize = _settings.FontSize;

            Fleets = NomenclaturesTransaction.GetFleetTypes();

            int[] selectedFleetIds = _settings.Fleets;

            SelectedFleets.AddRange(Fleets.FindAll(f => selectedFleetIds.Contains(f.Id)));

            return Task.CompletedTask;
        }

        private void OnFontSizeChanged()
        {
            App.Current.SetFontSize(FontSize);
            _settings.FontSize = FontSize;
        }

        private void OnFleetFilterChanged()
        {
            if (HasFleetFilter)
            {
                _settings.Fleets = SelectedFleets.Select(f => f.Id).ToArray();
            }
            else
            {
                _settings.Fleets = Array.Empty<int>();
            }
        }

        private async Task OnResetDatabase()
        {
            await TLLoadingHelper.ShowFullLoadingScreen();

            _appDbMigration.DropDatabase();
            _nomenclatureDates.Clear();
            _dbSettings.Clear();
            _appDbMigration.CheckForMigrations();
            await _startupTransaction.GetInitialData(true, null, null);
            _ = await _inspectionsTransaction.GetAll(1, reset: true);

            await TLLoadingHelper.HideFullLoadingScreen();
        }

    }
}
