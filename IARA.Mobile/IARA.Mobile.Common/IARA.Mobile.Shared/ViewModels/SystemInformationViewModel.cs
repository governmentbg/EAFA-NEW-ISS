using IARA.Mobile.Application.Interfaces.Utilities;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Shared.ResourceTranslator;
using IARA.Mobile.Shared.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ViewModels.Base;

namespace IARA.Mobile.Shared.ViewModels
{
    public class SystemInformationViewModel : TLBasePageViewModel, IPageViewModel
    {
        private readonly ISystemInformationProvider _systemInformationProvider;
        private readonly IExceptionHandler _exceptionHandler;

        public SystemInformationViewModel(ISystemInformationProvider systemInformationProvider, IExceptionHandler exceptionHandler)
        {
            _systemInformationProvider = systemInformationProvider;
            _exceptionHandler = exceptionHandler;
            SystemParamters = new TLObservableCollection<string>();
            SendData = CommandBuilder.CreateFrom(OnSendData);
        }

        public TLObservableCollection<string> SystemParamters { get; }

        public ICommand SendData { get; }

        public GroupResourceEnum[] GetPageIndexes()
        {
            return Array.Empty<GroupResourceEnum>();
        }

        public IReadOnlyDictionary<GroupResourceEnum, IReadOnlyDictionary<string, string>> GetPageResources(out GroupResourceEnum[] filtered)
        {
            filtered = Translator.Current.Filter(GetPageIndexes()).ToArray();
            return new Dictionary<GroupResourceEnum, IReadOnlyDictionary<string, string>>();
        }

        public override async Task Initialize(object sender)
        {
            List<string> systemInformation = await _systemInformationProvider.Get();
            SystemParamters.AddRange(systemInformation);
        }

        private async Task OnSendData()
        {
            string message = string.Join("\n", SystemParamters);
            await _exceptionHandler.DebugLog(message);
        }
    }
}
