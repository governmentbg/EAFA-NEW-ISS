using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace IARA.Mobile.Insp.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PersonView : StackLayout
    {
        public static readonly BindableProperty TitleProperty =
            BindableProperty.Create(nameof(Title), typeof(string), typeof(PersonView));
        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public static readonly BindableProperty IsTitleVisibleProperty =
           BindableProperty.Create(nameof(IsTitleVisible), typeof(bool), typeof(PersonView), defaultValue: true);
        public bool IsTitleVisible
        {
            get => (bool)GetValue(IsTitleVisibleProperty);
            set => SetValue(IsTitleVisibleProperty, value);
        }
        public PersonView()
        {
            InitializeComponent();
        }
    }
}