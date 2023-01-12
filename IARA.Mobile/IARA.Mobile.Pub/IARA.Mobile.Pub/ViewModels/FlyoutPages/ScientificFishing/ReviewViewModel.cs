using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Pub.Application.DTObjects.ScientificFishing.LocalDb;
using IARA.Mobile.Pub.ViewModels.Base;
using IARA.Mobile.Pub.ViewModels.Models;
using Xamarin.CommunityToolkit.ObjectModel;

namespace IARA.Mobile.Pub.ViewModels.FlyoutPages.ScientificFishing
{
    public class ReviewViewModel : PageViewModel
    {
        private SFPermitReviewDto _edit;

        public ReviewViewModel()
        {
            PermitReasons = new ObservableRangeCollection<CheckNomenclatureModel>();
        }

        public int Id { get; set; }

        public SFPermitReviewDto Edit
        {
            get => _edit;
            set => SetProperty(ref _edit, value);
        }

        public ObservableRangeCollection<CheckNomenclatureModel> PermitReasons { get; }

        public override GroupResourceEnum[] GetPageIndexes()
        {
            return new[] { GroupResourceEnum.ScientificFishingReview };
        }

        public override Task Initialize(object sender)
        {
            Edit = ScientificFishingTransaction.Get(Id);

            if (Edit == null)
            {
                return Task.CompletedTask;
            }

            List<NomenclatureDto> permitReasons = NomenclaturesTransaction.GetPermitReasons();

            if (permitReasons != null)
            {
                PermitReasons.AddRange(
                    permitReasons
                        .Select(f => new CheckNomenclatureModel
                        {
                            Value = f.Value,
                            DisplayName = f.DisplayName,
                            IsChecked = Edit.PermitReasonsIds.Contains(f.Value)
                        })
                );
            }

            return Task.CompletedTask;
        }
    }
}
