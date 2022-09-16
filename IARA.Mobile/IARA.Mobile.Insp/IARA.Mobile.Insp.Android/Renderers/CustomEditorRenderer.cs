using Android.Content;
using IARA.Mobile.Insp.Behaviors;
using IARA.Mobile.Insp.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(Editor), typeof(CustomEditorRenderer))]

namespace IARA.Mobile.Insp.Droid.Renderers
{
    public class CustomEditorRenderer : EditorRenderer
    {
        public CustomEditorRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Editor> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                bool has2Lines = (bool)e.NewElement.GetValue(EditorNot2LineBehavior.Has2LinesProperty);

                if (has2Lines)
                {
                    Control.SetMinLines(2);
                }
                else
                {
                    Control.SetMinLines(1);
                }
            }
        }
    }
}
