﻿using IARA.Mobile.Insp.UWP.Renderers;
using Windows.UI.Xaml;
using Xamarin.Forms.Platform.UWP;

[assembly: ExportRenderer(typeof(Xamarin.Forms.Button), typeof(CustomButtonRenderer))]

namespace IARA.Mobile.Insp.UWP.Renderers
{
    public class CustomButtonRenderer : ButtonRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Button> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                Control.Style = Windows.UI.Xaml.Application.Current.Resources["ButtonStyle"] as Style;
            }
        }
    }
}
