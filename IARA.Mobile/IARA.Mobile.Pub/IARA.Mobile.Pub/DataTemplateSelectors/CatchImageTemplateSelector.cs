using System;
using IARA.Mobile.Application;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Pub.ViewModels.FlyoutPages.CatchRecords;
using IARA.Mobile.Pub.ViewModels.Models;
using TechnoLogica.Xamarin.DataTemplates.Base;
using TechnoLogica.Xamarin.Extensions;
using TechnoLogica.Xamarin.Helpers;
using Xamarin.CommunityToolkit.Effects;
using Xamarin.Forms;

namespace IARA.Mobile.Pub.DataTemplateSelectors
{
    public class CatchImageTemplateSelector : BaseDataTemplateSelector<CatchImageModel, AddCatchRecordViewModel>
    {
        public override Func<object> FromTemplate(CatchImageModel item, BindableObject container, AddCatchRecordViewModel bindingContext)
        {
            return () =>
            {
                if (CommonGlobalVariables.InternetStatus == InternetStatus.Disconnected)
                {
                    if (item.IsFileSavedLocally || item.WasAddedNow)
                    {
                        return GetImage(item, bindingContext);
                    }

                    ContentView view = new Frame
                    {
                        HasShadow = false,
                        CornerRadius = 0,
                        BorderColor = App.GetResource<Color>("GrayColor"),
                        HeightRequest = 100,
                        WidthRequest = 100,
                        Content = new StackLayout
                        {
                            VerticalOptions = LayoutOptions.Center,
                            Children =
                            {
                                new Label
                                {
                                    FontSize = 35,
                                    FontFamily = "FA",
                                    Text = IconFont.Image,
                                    HorizontalOptions = LayoutOptions.Center,
                                },
                                new Label
                                {
                                    LineBreakMode = LineBreakMode.WordWrap,
                                }.BindTranslation(Label.TextProperty, "NoInternet", nameof(GroupResourceEnum.Common))
                            }
                        }
                    };

                    ApplyCommands(view, item, bindingContext);

                    return view;
                }
                else
                {
                    return GetImage(item, bindingContext);
                }
            };
        }

        private static Image GetImage(CatchImageModel item, AddCatchRecordViewModel bindingContext)
        {
            Image image = new Image
            {
                HeightRequest = 100,
                WidthRequest = 100,
                Aspect = Aspect.AspectFill,
                Source = item.Image,
                Margin = new Thickness(1, 0),
            };

            ApplyCommands(image, item, bindingContext);

            return image;
        }

        private static void ApplyCommands(View view, CatchImageModel item, AddCatchRecordViewModel bindingContext)
        {
            TouchEffect.SetCommand(view, bindingContext.OpenPicture);
            TouchEffect.SetCommandParameter(view, item);

            TouchEffect.SetLongPressCommand(view, bindingContext.RemovePicture);
            TouchEffect.SetLongPressCommandParameter(view, item);

            TouchEffect.SetNativeAnimation(view, true);
        }
    }
}
