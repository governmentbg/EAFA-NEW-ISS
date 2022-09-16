using TechnoLogica.Xamarin.ViewModels.Base.Models;

namespace IARA.Mobile.Pub.ViewModels.Models
{
    public class CheckNomenclatureModel : CheckNomenclatureModel<int>
    {
    }

    public class CheckNomenclatureModel<T> : BaseModel
        where T : struct
    {
        private bool _isChecked;

        public T Value { get; set; }
        public string DisplayName { get; set; }

        public bool IsChecked
        {
            get => _isChecked;
            set => SetProperty(ref _isChecked, value);
        }
    }
}
