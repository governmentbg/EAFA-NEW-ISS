using System.Linq;
using Android.Content;
using IARA.Mobile.Insp.Droid.Renderers;
using IARA.Mobile.Shared.Views;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(TLMenuTextButton), typeof(MenuTextButtonRenderer))]

namespace IARA.Mobile.Insp.Droid.Renderers
{
    public class MenuTextButtonRenderer : ButtonRenderer
    {
        public MenuTextButtonRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
        {
            base.OnElementChanged(e);

            if (e?.NewElement != null)
            {
                Element.Command = new Command(OnClick);
            }
        }

        private void OnClick()
        {
            float displayDensity = Context.Resources.DisplayMetrics.Density;
            int[] position = new int[2];
            GetLocationInWindow(position);

            (Element as TLMenuTextButton).OnViewTouch(
                position.ElementAtOrDefault(0) / displayDensity,
                position.ElementAtOrDefault(1) / displayDensity
            );
        }
    }
}
