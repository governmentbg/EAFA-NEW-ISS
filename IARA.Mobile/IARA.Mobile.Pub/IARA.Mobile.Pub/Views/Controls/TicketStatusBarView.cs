using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Pub.Domain.Enums;
using IARA.Mobile.Shared.Views;
using System;
using System.Windows.Input;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ResourceTranslator;
using Xamarin.CommunityToolkit.Markup;
using Xamarin.Forms;

namespace IARA.Mobile.Pub.Views.Controls
{
    public class TicketStatusBarView : Frame
    {
        public static readonly BindableProperty StatusCodeProperty =
           BindableProperty.Create(nameof(StatusCode), typeof(string), typeof(TicketStatusBarView));

        public static readonly BindableProperty ApplicationStatusCodeProperty =
          BindableProperty.Create(nameof(ApplicationStatusCode), typeof(string), typeof(TicketStatusBarView));

        public static readonly BindableProperty ValidFromProperty =
           BindableProperty.Create(nameof(ValidFrom), typeof(DateTime), typeof(TicketStatusBarView));

        public static readonly BindableProperty ValidToProperty =
           BindableProperty.Create(nameof(ValidTo), typeof(DateTime), typeof(TicketStatusBarView));

        public static readonly BindableProperty PaymentStatusProperty =
          BindableProperty.Create(nameof(PaymentStatus), typeof(PaymentStatusEnum), typeof(TicketStatusBarView));

        public static readonly BindableProperty PaymentCommandProperty =
           BindableProperty.Create(nameof(PaymentCommand), typeof(ICommand), typeof(TicketStatusBarView));

        public static readonly BindableProperty PaymentCommandParameterProperty =
            BindableProperty.Create(nameof(PaymentCommandParameter), typeof(object), typeof(TicketStatusBarView));

        public static readonly BindableProperty TicketRenewCommandProperty =
          BindableProperty.Create(nameof(TicketRenewCommand), typeof(ICommand), typeof(TicketStatusBarView));

        public static readonly BindableProperty TicketRenewCommandParameterProperty =
            BindableProperty.Create(nameof(TicketRenewCommandParameter), typeof(object), typeof(TicketStatusBarView));

        public static readonly BindableProperty TicketUpdateCommandProperty =
  BindableProperty.Create(nameof(TicketUpdateCommand), typeof(ICommand), typeof(TicketStatusBarView));

        public static readonly BindableProperty TicketUpdateCommandParameterProperty =
            BindableProperty.Create(nameof(TicketUpdateCommandParameter), typeof(object), typeof(TicketStatusBarView));


        public TicketStatusBarView()
        {
            BackgroundColor = Color.Transparent;
            Padding = 0;
            FuncConverter<string, View> statusChangedConverter = new FuncConverter<string, View>(status =>
            {
                DateTime now = DateTime.Now;
                if (now > ValidTo || status == nameof(TicketStatusEnum.EXPIRED))
                {
                    if (ApplicationStatusCode == ApplicationStatuses.CONFIRMED_ISSUED_TICKET ||
                        ApplicationStatusCode == ApplicationStatuses.TICKET_ISSUED ||
                        ApplicationStatusCode == ApplicationStatuses.INSP_CORR_FROM_EMP)
                    {
                        return ExpiredTicket();
                    }
                    else
                    {
                        return IconTextView(TranslateExtension.Translator[nameof(GroupResourceEnum.FishingTicket) + "/InvalidTicket"], IconFont.Xmark, Color.Red);
                    }
                }
                else if (ApplicationStatusCode == ApplicationStatuses.PAYMENT_PROCESSING)
                {
                    return IconTextView(TranslateExtension.Translator[nameof(GroupResourceEnum.FishingTicket) + "/PaymentProcessing"], IconFont.Clock, Color.Black);
                }
                else if (ApplicationStatusCode == ApplicationStatuses.CORR_BY_USR_NEEDED ||
                    ApplicationStatusCode == ApplicationStatuses.FILL_BY_APPL)
                {
                    return IconButton(TranslateExtension.Translator[nameof(GroupResourceEnum.FishingTicket) + "/TicketDataUpdate"], IconFont.TriangleExclamation, Color.FromHex("#F15151"))//Корекция на данни, #F15151 = Red
                                 .Bind(TLButton.CommandProperty, TicketUpdateCommandProperty.PropertyName, source: this)
                                .Bind(TLButton.CommandParameterProperty, TicketUpdateCommandParameterProperty.PropertyName, source: this);
                }
                else if (PaymentStatus == PaymentStatusEnum.PaidOK || PaymentStatus == PaymentStatusEnum.NotNeeded)
                {
                    //СТАТУС ИЗЧАКВА ПРОВЕРКИ ОТПАДА!!!!!!!
                    //ПОТРЕБИТЕЛЯ ВИЖДА БИЛЕТА КАТО АКТИВЕН ПО ВРЕМЕ НА РЕДЖИКС ПРОВЕРКИТЕ И ПРИ НУЖДА ОТ ПРОВЕРКА НА СЛУЖИТЕЛ!!!!!
                    //if (status == nameof(TicketStatusEnum.REQUESTED))
                    //{
                    //    return IconTextView(TranslateExtension.Translator[nameof(GroupResourceEnum.FishingTicket) + "/AwaitingInspections"], IconFont.Clock, Color.Black);//"Изчаква проверки"
                    //}
                    //else
                    if (status == nameof(TicketStatusEnum.APPROVED) || status == nameof(TicketStatusEnum.REQUESTED) || status == nameof(TicketStatusEnum.ISSUED))
                    {
                        if (now < ValidFrom)
                        {
                            return IconTextView(TranslateExtension.Translator[nameof(GroupResourceEnum.FishingTicket) + "/ActiveTicket"], IconFont.Check, Color.Green);//"Активен билет"
                        }
                        else if (ValidFrom <= now && now < ValidTo)
                        {
                            return new Grid()
                            {
                                RowSpacing = 0,
                                RowDefinitions =
                                {
                                    new RowDefinition { Height = GridLength.Auto },
                                    new RowDefinition { Height = GridLength.Auto }
                                },
                                Children =
                                {
                                    new Label
                                    {
                                        HorizontalTextAlignment = TextAlignment.Center,
                                        VerticalTextAlignment = TextAlignment.Center,
                                        FontSize = 30,
                                        FontFamily = "FA",
                                        TextColor = Color.Green,
                                        Text = IconFont.Check,
                                    }.Row(0),
                                    new StackLayout()
                                    {
                                    HorizontalOptions = LayoutOptions.Center,
                                    VerticalOptions = LayoutOptions.Center,
                                    Children =
                                        {
                                            new Label()
                                            {
                                                HorizontalTextAlignment = TextAlignment.Center,
                                                FontSize = 15,
                                                Text = TranslateExtension.Translator[nameof(GroupResourceEnum.FishingTicket) + "/Valid"],//"Валиден още:"
                                                LineBreakMode = LineBreakMode.WordWrap,
                                                IsVisible = ValidTo.Year != 9999,
                                            },
                                            new Label()
                                            {
                                                HorizontalTextAlignment = TextAlignment.Center,
                                                FontSize = 15,
                                                LineBreakMode = LineBreakMode.WordWrap,
                                                Text = HumanizeTimeLeft(),
                                            }
                                        }
                                    }.Row(1),
                                }
                            };
                        }
                        else
                        {
                            return ExpiredTicket();
                        }
                    }
                    else if (status == nameof(TicketStatusEnum.CANCELED))
                    {
                        return new Grid()
                        {
                            RowSpacing = 0,
                            RowDefinitions =
                            {
                                new RowDefinition { Height = GridLength.Auto },
                                new RowDefinition { Height = GridLength.Auto }
                            },
                            Children =
                            {
                                new Label
                                {
                                    HorizontalTextAlignment = TextAlignment.Center,
                                    VerticalTextAlignment = TextAlignment.Center,
                                    FontSize = 30,
                                    FontFamily = "FA",
                                    TextColor = Color.Red,
                                    Text = IconFont.Xmark,
                                }.Row(0),
                                new Label
                                {
                                    HorizontalTextAlignment = TextAlignment.Center,
                                    VerticalTextAlignment = TextAlignment.Center,
                                    FontSize = 15,
                                    Text = TranslateExtension.Translator[nameof(GroupResourceEnum.FishingTicket) + "/CanceledTicket"],//"Анулиран билет"
                                    LineBreakMode = LineBreakMode.WordWrap,
                                }.Row(1),
                            }
                        };
                    }
                }
                else if (PaymentStatus == PaymentStatusEnum.PaymentFail || PaymentStatus == PaymentStatusEnum.Unpaid || ApplicationStatusCode == ApplicationStatuses.PAYMENT_ANNUL)
                {
                    return new StackLayout()
                    {
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center,
                        Children =
                        {
                            new Label
                            {
                                HorizontalTextAlignment = TextAlignment.Center,
                                VerticalTextAlignment = TextAlignment.Center,
                                LineBreakMode = LineBreakMode.WordWrap,
                                FontSize = 15,
                                Text = PaymentStatus == PaymentStatusEnum.PaymentFail || ApplicationStatusCode == ApplicationStatuses.PAYMENT_ANNUL ?
                                TranslateExtension.Translator[nameof(GroupResourceEnum.FishingTicket) + "/InvalidPayment"]//"Неуспешно плащане"
                                : TranslateExtension.Translator[nameof(GroupResourceEnum.FishingTicket) + "/AwaitingPayment"]//"Очаква плащане"
                            },
                             IconButton(TranslateExtension.Translator[nameof(GroupResourceEnum.FishingTicket) + "/ProceedToPayment"], IconFont.MoneyCheck, Color.FromHex("#58A1E1"))//Към плащане, #58A1E1 = Blue
                                 .Bind(TLButton.CommandProperty, PaymentCommandProperty.PropertyName, source: this)
                                    .Bind(TLButton.CommandParameterProperty, PaymentCommandParameterProperty.PropertyName, source: this)
                        }
                    };

                }

                return null;
            });

            this.Bind(TicketStatusBarView.ContentProperty, StatusCodeProperty.PropertyName, source: this, converter: statusChangedConverter);
        }

        public string StatusCode
        {
            get => (string)GetValue(StatusCodeProperty);
            set => SetValue(StatusCodeProperty, value);
        }

        public string ApplicationStatusCode
        {
            get => (string)GetValue(ApplicationStatusCodeProperty);
            set => SetValue(ApplicationStatusCodeProperty, value);
        }

        public DateTime ValidFrom
        {
            get => (DateTime)GetValue(ValidFromProperty);
            set => SetValue(ValidFromProperty, value);
        }
        public DateTime ValidTo
        {
            get => (DateTime)GetValue(ValidToProperty);
            set => SetValue(ValidToProperty, value);
        }

        public PaymentStatusEnum PaymentStatus
        {
            get => (PaymentStatusEnum)GetValue(PaymentStatusProperty);
            set => SetValue(PaymentStatusProperty, value);
        }

        public ICommand PaymentCommand
        {
            get => (ICommand)GetValue(PaymentCommandProperty);
            set => SetValue(PaymentCommandProperty, value);
        }

        public object PaymentCommandParameter
        {
            get => GetValue(PaymentCommandParameterProperty);
            set => SetValue(PaymentCommandParameterProperty, value);
        }

        public ICommand TicketRenewCommand
        {
            get => (ICommand)GetValue(TicketRenewCommandProperty);
            set => SetValue(TicketRenewCommandProperty, value);
        }

        public object TicketRenewCommandParameter
        {
            get => GetValue(TicketRenewCommandParameterProperty);
            set => SetValue(TicketRenewCommandParameterProperty, value);
        }

        public ICommand TicketUpdateCommand
        {
            get => (ICommand)GetValue(TicketUpdateCommandProperty);
            set => SetValue(TicketUpdateCommandProperty, value);
        }

        public object TicketUpdateCommandParameter
        {
            get => GetValue(TicketUpdateCommandParameterProperty);
            set => SetValue(TicketUpdateCommandParameterProperty, value);
        }

        private StackLayout ExpiredTicket()
        {
            return new StackLayout()
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                Children =
                            {
                                new Label
                                {
                                    HorizontalTextAlignment = TextAlignment.Center,
                                    VerticalTextAlignment = TextAlignment.Center,
                                    LineBreakMode = LineBreakMode.WordWrap,
                                    FontSize = 15,
                                    Text = TranslateExtension.Translator[nameof(GroupResourceEnum.FishingTicket) + "/ExpiredTicket"], //Изтекъл билет
                                },
                                IconButton(TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/TicketRenew"], IconFont.Ticket, Color.FromHex("#F15151"))//Поднови, #F15151 = Red
                                    .Bind(TLButton.CommandProperty, TicketRenewCommandProperty.PropertyName, source: this)
                                    .Bind(TLButton.CommandParameterProperty, TicketRenewCommandParameterProperty.PropertyName, source: this)
                            }
            };
        }

        private TLButton IconButton(string text, string icon, Color color)
        {
            return new TLButton
            {
                ImageSource = new FontImageSource
                {
                    FontFamily = "FA",
                    Glyph = icon,
                    Color = Color.White
                },
                Text = text,
                BackgroundColor = color,
                TextColor = Color.White,
                FontSize = 12
            };
        }

        private Grid IconTextView(string label, string icon, Color iconColor)
        {
            return new Grid()
            {
                RowSpacing = 0,
                RowDefinitions =
                                {
                                    new RowDefinition { Height = GridLength.Auto },
                                    new RowDefinition { Height = GridLength.Auto }
                                },
                Children =
                                {
                                    new Label
                                    {
                                        HorizontalTextAlignment = TextAlignment.Center,
                                        VerticalTextAlignment = TextAlignment.Center,
                                        FontSize = 30,
                                        FontFamily = "FA",
                                        TextColor =iconColor,
                                        Text = icon,
                                    }.Row(0),
                                    new Label
                                    {
                                        HorizontalTextAlignment = TextAlignment.Center,
                                        VerticalTextAlignment = TextAlignment.Center,
                                        FontSize = 15,
                                        Text =label,
                                        LineBreakMode = LineBreakMode.WordWrap,
                                    }.Row(1),
                                }
            };
        }

        private string HumanizeTimeLeft()
        {
            TimeSpan value = ValidTo - DateTime.Now;

            string duration = string.Empty;

            if (ValidTo.Year == 9999)
            {
                return "Безсрочен";
            }

            if (value.Seconds < 0)
            {
                return duration;
            }

            if (value.TotalMinutes < 1)
            {
                duration += $"{value.Seconds} " + (value.Seconds == 1 ? TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/Second"] : TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/Seconds"]);
            }
            else if (value.TotalHours < 1)
            {
                int minutesLeft = (int)value.TotalMinutes;
                duration += $"{minutesLeft} " + (minutesLeft == 1 ? TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/Minute"] : TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/Minutes"]);
            }
            else if (value.TotalDays < 1)
            {
                int hoursLeft = (int)value.TotalHours;
                duration += $"{hoursLeft} " + (hoursLeft == 1 ? TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/Hour"] : TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/Hours"]);

                int minutes = (int)value.TotalHours - hoursLeft * 60;
                if (minutes > 0)
                {
                    duration += $" и {minutes} " + (minutes == 1 ? TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/Minute"] : TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/Minutes"]);
                }
            }
            else if (value.TotalDays < 365)
            {
                int daysLeft = (int)value.TotalDays;
                duration += $"{daysLeft} " + (daysLeft == 1 ? TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/Day"] : TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/Days"]);

                if (daysLeft < 30)
                {
                    int hoursLeft = (int)value.TotalHours - daysLeft * 24;
                    if (hoursLeft > 0)
                    {
                        duration += $" и {hoursLeft} " + (hoursLeft == 1 ? TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/Hour"] : TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/Hours"]);
                    }
                }
            }
            else if (value.TotalDays >= 365)
            {
                int daysLeft = (int)value.TotalDays;
                int years = daysLeft / 365;
                duration += $"{years} " + (years == 1 ? TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/Year"] : TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/Years"]);

                daysLeft -= years * 365;
                if (daysLeft > 0)
                {
                    duration += $" и {daysLeft} " + (daysLeft == 1 ? TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/Day"] : TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/Days"]);
                }
            }
            return duration;
        }

    }
}
