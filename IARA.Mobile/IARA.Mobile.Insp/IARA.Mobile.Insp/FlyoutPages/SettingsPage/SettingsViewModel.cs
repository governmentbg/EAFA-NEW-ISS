using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Insp.Application.Interfaces.Utilities;
using IARA.Mobile.Insp.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Helpers;

namespace IARA.Mobile.Insp.FlyoutPages.SettingsPage
{
    public class SettingsViewModel : PageViewModel
    {
        private readonly ISettings _settings;
        private List<SelectNomenclatureDto> _fleets;
        private bool _hasFleetFilter;
        private double _fontSize;

        public SettingsViewModel(ISettings settings)
        {
            _settings = settings;
            FontSizeChanged = CommandBuilder.CreateFrom(OnFontSizeChanged);
            FleetFilterChanged = CommandBuilder.CreateFrom(OnFleetFilterChanged);

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
    }
}
