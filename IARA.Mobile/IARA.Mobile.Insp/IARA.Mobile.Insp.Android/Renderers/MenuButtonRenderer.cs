using Android.Content;
using IARA.Mobile.Insp.Droid.Renderers;
using IARA.Mobile.Shared.Views;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(TLMenuButton), typeof(MenuButtonRenderer))]

namespace IARA.Mobile.Insp.Droid.Renderers
{
    public class MenuButtonRenderer : ImageButtonRenderer
    {
        public MenuButtonRenderer(Context context)
            : base(context) { }

        protected override void OnElementChanged(ElementChangedEventArgs<ImageButton> e)
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

            (Element as TLMenuButton).OnViewTouch(
                position.ElementAtOrDefault(0) / displayDensity,
                position.ElementAtOrDefault(1) / displayDensity
            );
        }
    }
}
