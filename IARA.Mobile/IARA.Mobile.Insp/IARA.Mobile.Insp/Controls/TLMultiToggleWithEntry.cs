using System.Collections.Generic;
using IARA.Mobile.Domain.Enums;
using TechnoLogica.Xamarin.Controls;
using TechnoLogica.Xamarin.Extensions;
using TechnoLogica.Xamarin.ViewModels.Interfaces;
using TechnoLogica.Xamarin.ViewModels.Models;
using Xamarin.CommunityToolkit.Markup;
using Xamarin.Forms;

namespace IARA.Mobile.Insp.Controls
{
    public class TLMultiToggleWithEntry : TLEntry
    {
        public static readonly BindableProperty EqualToProperty =
            BindableProperty.Create(nameof(EqualTo), typeof(string), typeof(TLMultiToggleWithEntry), nameof(CheckTypeEnum.Y));

        public static readonly BindableProperty ButtonsProperty =
            BindableProperty.Create(nameof(Buttons), typeof(IList<ToggleOption>), typeof(TLMultiToggleWithEntry));

        public static readonly BindableProperty SelectedValueProperty =
            BindableProperty.Create(nameof(SelectedValue), typeof(string), typeof(TLMultiToggleWithEntry), null, BindingMode.TwoWay);

        public static readonly BindableProperty ToggleValidStateProperty =
            BindableProperty.Create(nameof(ToggleValidState), typeof(IValidState<string>), typeof(TLMultiToggleWithEntry));

        public TLMultiToggleWithEntry()
        {
            TitleView = new TLMultiToggleView
            {
                BindingContext = this
            }.Bind(TLMultiToggleView.ButtonsProperty, ButtonsProperty.PropertyName)
                .Bind(TLMultiToggleView.TextProperty, TitleProperty.PropertyName)
                .Bind(TLMultiToggleView.SelectedValueProperty, SelectedValueProperty.PropertyName)
                .Bind(TLMultiToggleView.ValidStateProperty, ToggleValidStateProperty.PropertyName)
                .Bind(TLMultiToggleView.IsEnabledProperty, IsEnabledProperty.PropertyName, source: this);

            FrameWrapper.Bind(Frame.IsVisibleProperty,
                ToggleValidStateProperty.PropertyName + ".Value",
                convert: (string selected) => selected == EqualTo
            );
        }

        public string SelectedValue
        {
            get => (string)GetValue(SelectedValueProperty);
            set => SetValue(SelectedValueProperty, value);
        }

        public string EqualTo
        {
            get => (string)GetValue(EqualToProperty);
            set => SetValue(EqualToProperty, value);
        }

        public IList<ToggleOption> Buttons
        {
            get => (IList<ToggleOption>)GetValue(ButtonsProperty);
            set => SetValue(ButtonsProperty, value);
        }

        public IValidState<string> ToggleValidState
        {
            get => (IValidState<string>)GetValue(ToggleValidStateProperty);
            set => SetValue(ToggleValidStateProperty, value);
        }
    }
}
