using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Pub.Application.DTObjects.ScientificFishing.LocalDb;
using IARA.Mobile.Pub.ViewModels.Base;
using IARA.Mobile.Pub.Views.FlyoutPages.ScientificFishing;
using IARA.Mobile.Shared.Menu;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.ResourceTranslator;
using Xamarin.CommunityToolkit.ObjectModel;

namespace IARA.Mobile.Pub.ViewModels.FlyoutPages
{
    public class ScientificFishingViewModel : MainPageViewModel
    {
        public ScientificFishingViewModel()
        {
            ScientificFishings = new ObservableRangeCollection<SFPermitDto>();
            Review = CommandBuilder.CreateFrom<SFPermitDto>(OnReview);
            Refresh = CommandBuilder.CreateFrom(OnRefresh);
            AddOuting = CommandBuilder.CreateFrom<SFPermitDto>(OnAddOuting);
            Outings = CommandBuilder.CreateFrom<SFPermitDto>(OnOutings);
        }

        public ObservableRangeCollection<SFPermitDto> ScientificFishings { get; }

        public ICommand Review { get; }
        public ICommand Refresh { get; }
        public ICommand AddOuting { get; }
        public ICommand Outings { get; }

        public override GroupResourceEnum[] GetPageIndexes()
        {
            return new[] { GroupResourceEnum.ScientificFishing };
        }

        public override Task Initialize(object sender)
        {
            return OnRefresh();
        }

        private async Task OnRefresh()
        {
            List<SFPermitDto> list = await ScientificFishingTransaction.GetAll(null);

            if (list == null)
            {
                return;
            }

            ScientificFishings.ReplaceRange(list);
        }

        private Task OnReview(SFPermitDto permit)
        {
            return MainNavigator.Current.GoToPageAsync(new ReviewPage(permit.Id));
        }

        private Task OnAddOuting(SFPermitDto permit)
        {
            DateTime now = DateTime.Now;
            if (permit.ValidFrom > now)
            {
                return App.Current.MainPage.DisplayAlert(
                    null,
                    TranslateExtension.Translator["ScientificFishing/PermitNotYetValid"],
                    TranslateExtension.Translator["Common/Ok"]
                );
            }
            else if (permit.ValidTo < now)
            {
                return App.Current.MainPage.DisplayAlert(
                    null,
                    TranslateExtension.Translator["ScientificFishing/PermitHasExpired"],
                    TranslateExtension.Translator["Common/Ok"]
                );
            }
            else
            {
                return MainNavigator.Current.GoToPageAsync(new AddOutingPage(permit.Id, true, permit));
            }
        }

        private Task OnOutings(SFPermitDto permit)
        {
            if (!permit.HasOutings)
            {
                return App.Current.MainPage.DisplayAlert(
                    null,
                    TranslateExtension.Translator["ScientificFishing/NoOutings"],
                    TranslateExtension.Translator["Common/Ok"]
                );
            }
            else
            {
                return MainNavigator.Current.GoToPageAsync(new OutingsPage(permit.Id, IsPermitActive(permit), permit));
            }
        }

        private bool IsPermitActive(SFPermitDto permit)
        {
            DateTime now = DateTime.Now;
            return permit.ValidFrom < now && now < permit.ValidTo;
        }
    }
}
