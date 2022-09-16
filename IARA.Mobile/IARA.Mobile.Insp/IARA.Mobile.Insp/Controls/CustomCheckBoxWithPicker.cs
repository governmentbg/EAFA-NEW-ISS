using System.Windows.Input;
using TechnoLogica.Xamarin.Controls;
using TechnoLogica.Xamarin.ViewModels.Interfaces;
using Xamarin.CommunityToolkit.Markup;
using Xamarin.Forms;

namespace IARA.Mobile.Insp.Controls
{
    public class CustomCheckBoxWithPicker : CustomInfinitePicker
    {
        public static readonly BindableProperty IsCheckedProperty =
            BindableProperty.Create(nameof(IsChecked), typeof(bool), typeof(CustomCheckBoxWithPicker), false, BindingMode.TwoWay);

        public static readonly BindableProperty CheckCommandProperty =
            BindableProperty.Create(nameof(CheckCommand), typeof(ICommand), typeof(CustomCheckBoxWithPicker));

        public static readonly BindableProperty CheckValidStateProperty =
            BindableProperty.Create(nameof(CheckValidState), typeof(IValidState<bool>), typeof(CustomCheckBoxWithPicker));

        public CustomCheckBoxWithPicker()
        {
            TitleView = new TLCheckView
            {
                BindingContext = this
            }.Bind(TLCheckView.IsCheckedProperty, IsCheckedProperty.PropertyName)
                .Bind(TLCheckView.IsEnabledProperty, IsEnabledProperty.PropertyName)
                .Bind(TLCheckView.ValidStateProperty, CheckValidStateProperty.PropertyName)
                .Bind(TLCheckView.TextProperty, TitleProperty.PropertyName)
                .Bind(TLCheckView.CommandProperty, CheckCommandProperty.PropertyName);

            FrameWrapper.Bind(Frame.IsVisibleProperty, IsCheckedProperty.PropertyName, convert: (bool value) => !value);
        }

        public bool IsChecked
        {
            get => (bool)GetValue(IsCheckedProperty);
            set => SetValue(IsCheckedProperty, value);
        }

        public ICommand CheckCommand
        {
            get => (ICommand)GetValue(CheckCommandProperty);
            set => SetValue(CheckCommandProperty, value);
        }

        public IValidState<bool> CheckValidState
        {
            get => (IValidState<bool>)GetValue(CheckValidStateProperty);
            set => SetValue(CheckValidStateProperty, value);
        }
    }
}
