using IARA.Mobile.Pub.ViewModels.Base;
using System.ComponentModel.DataAnnotations;
using TechnoLogica.Xamarin.Attributes;
using TechnoLogica.Xamarin.ViewModels.Models;

namespace IARA.Mobile.Pub.ViewModels.Controls.PersonView
{
    public class BulgarianCitizenViewModel : ViewModel
    {
        private ValidState _idCard;
        private ValidStateDate _idCardDate;
        private ValidState _idCardPublisher;

        [Required]
        [ValidGroup(Group.BULGARIAN_CITIZEN)]
        [StringLength(15)]
        public ValidState Idcard
        {
            get => _idCard;
            set => SetProperty(ref _idCard, value);
        }

        [Required]
        [ValidGroup(Group.BULGARIAN_CITIZEN)]
        public ValidStateDate IdcardDate
        {
            get => _idCardDate;
            set => SetProperty(ref _idCardDate, value);
        }

        [Required]
        [ValidGroup(Group.BULGARIAN_CITIZEN)]
        [StringLength(50)]
        public ValidState IdcardPublisher
        {
            get => _idCardPublisher;
            set => SetProperty(ref _idCardPublisher, value);
        }
    }
}
