using IARA.Mobile.Domain.Interfaces;
using IARA.Mobile.Pub.Application.DTObjects.DocumentTypes.LocalDb;
using IARA.Mobile.Pub.ViewModels.Base;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TechnoLogica.Xamarin.Attributes;
using TechnoLogica.Xamarin.ViewModels.Models;

namespace IARA.Mobile.Pub.ViewModels.Controls.PersonView
{
    public class ForeignerViewModel : ViewModel
    {
        private ValidStateSelect<DocumentTypeSelectDto> _documentType;
        private ValidStateSelect<ISelectProperty> _citizenship;
        private ValidState _idCard;

        [Required]
        [ValidGroup(Group.FOREIGNER)]
        [StringLength(15)]
        public ValidState Idcard
        {
            get => _idCard;
            set => SetProperty(ref _idCard, value);
        }

        [Required]
        [ValidGroup(Group.FOREIGNER)]
        public ValidStateSelect<DocumentTypeSelectDto> DocumentType
        {
            get => _documentType;
            set => SetProperty(ref _documentType, value);
        }

        [Required]
        [ValidGroup(Group.FOREIGNER)]
        public ValidStateSelect<ISelectProperty> Citizenship
        {
            get => _citizenship;
            set => SetProperty(ref _citizenship, value);
        }

        public List<DocumentTypeSelectDto> DocumentTypes { get; set; }
    }
}
