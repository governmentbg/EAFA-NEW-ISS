using TechnoLogica.Xamarin.Behaviors.Base;
using TechnoLogica.Xamarin.Controls;
using Xamarin.Forms;

namespace IARA.Mobile.Insp.Behaviors
{
    public class EditorNot2LineBehavior : BaseBindableBehavior<TLEditor>
    {
        public static readonly BindableProperty Has2LinesProperty =
            BindableProperty.Create("Has2Lines", typeof(bool), typeof(Editor), true);

        protected override void OnAttachedTo(TLEditor visualElement)
        {
            base.OnAttachedTo(visualElement);

            visualElement.InnerEditor.SetValue(Has2LinesProperty, false);
        }
    }
}
