using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Pub.Application.DTObjects.ScientificFishing.LocalDb;
using IARA.Mobile.Pub.ViewModels.Base;
using IARA.Mobile.Pub.Views.FlyoutPages.ScientificFishing;
using IARA.Mobile.Shared.Menu;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ResourceTranslator;

namespace IARA.Mobile.Pub.ViewModels.FlyoutPages.ScientificFishing
{
    public class OutingsViewModel : PageViewModel
    {
        public OutingsViewModel()
        {
            Outings = new TLObservableCollection<SFOutingDto>();
            Edit = CommandBuilder.CreateFrom<SFOutingDto>(OnEdit);
            Review = CommandBuilder.CreateFrom<SFOutingDto>(OnReview);
            Delete = CommandBuilder.CreateFrom<SFOutingDto>(OnDelete);
        }

        public int Id { get; set; }
        public bool IsActive { get; set; }
        public SFPermitDto Permit { get; set; }

        public TLObservableCollection<SFOutingDto> Outings { get; }

        public ICommand Edit { get; }
        public ICommand Review { get; }
        public ICommand Delete { get; }

        public override GroupResourceEnum[] GetPageIndexes()
        {
            return new[] { GroupResourceEnum.Outings };
        }

        public override Task Initialize(object sender)
        {
            List<SFOutingDto> outings = ScientificFishingTransaction.GetOutings(Id);
            Outings.AddRange(outings);

            return Task.CompletedTask;
        }

        private Task OnEdit(SFOutingDto dto)
        {
            return MainNavigator.Current.GoToPageAsync(new AddOutingPage(dto.PermitId, true, Permit, dto));
        }

        private Task OnReview(SFOutingDto dto)
        {
            return MainNavigator.Current.GoToPageAsync(new AddOutingPage(dto.PermitId, false, Permit, dto));
        }

        private async Task OnDelete(SFOutingDto dto)
        {
            bool result = await App.Current.MainPage.DisplayAlert(null,
                TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/DeleteConfirm"],
                TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/Delete"],
                TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/Cancel"]
            );
            if (result)
            {
                await ScientificFishingTransaction.DeleteOuting(dto);
                Outings.Remove(dto);
            }
        }
    }
}
