using IARA.Mobile.Insp.ViewModels.Models;
using TechnoLogica.Xamarin.Controls;
using TechnoLogica.Xamarin.ViewModels.Interfaces;
using Xamarin.CommunityToolkit.Markup;

namespace IARA.Mobile.Insp.Controls
{
    public class CustomInfinitePicker : TLInfinitePicker
    {
        protected override void OnValidStateValueBindingPropertyChanged(IValidState<object> validState)
        {
            base.OnValidStateValueBindingPropertyChanged(validState);

            this.Bind(CustomInfinitePicker.ItemsSourceProperty, nameof(ValidStateInfiniteSelect<TLInfinitePicker>.ItemsSource), source: validState);
            this.Bind(CustomInfinitePicker.DialogClosedCommandProperty, nameof(ValidStateInfiniteSelect<TLInfinitePicker>.Closed), source: validState);
            this.Bind(CustomInfinitePicker.DialogEndReachedCommandProperty, nameof(ValidStateInfiniteSelect<TLInfinitePicker>.EndReached), source: validState);
            this.Bind(CustomInfinitePicker.DialogUserStoppedTypingCommandProperty, nameof(ValidStateInfiniteSelect<TLInfinitePicker>.StoppedTyping), source: validState);
        }
    }
}
