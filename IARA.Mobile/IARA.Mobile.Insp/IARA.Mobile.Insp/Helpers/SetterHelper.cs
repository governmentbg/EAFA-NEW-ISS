using System.Collections.Generic;
using Xamarin.Forms;

namespace IARA.Mobile.Insp.Helpers
{
    public static class SetterHelper
    {
        public static void SetFontSize(this IList<Setter> setters, BindableProperty fontSizeProperty, double fontSize)
        {
            for (int i = 0; i < setters.Count; i++)
            {
                if (setters[i].Property == fontSizeProperty)
                {
                    setters.Remove(setters[i]);
                    break;
                }
            }

            setters.Add(new Setter { Property = fontSizeProperty, Value = fontSize });
        }
    }
}
