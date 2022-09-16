using IARA.Mobile.Application.Interfaces.Transactions;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Insp.Controls;
using IARA.Mobile.Insp.FlyoutPages.LoadingPage;
using IARA.Mobile.Insp.Helpers;
using IARA.Mobile.Shared.ResourceTranslator;
using IARA.Mobile.Shared.Views;
using System.Collections.Generic;
using System.ComponentModel;
using TechnoLogica.Xamarin.Controls;
using TechnoLogica.Xamarin.ResourceTranslator;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;

namespace IARA.Mobile.Insp
{
    public partial class App : Xamarin.Forms.Application
    {
        public static new App Current { get; private set; }

        public App()
        {
            Current = this;
            InitializeComponent();
            UserAppTheme = OSAppTheme.Light;
            MainPage = new LoadingPage();

            On<Android>().UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Resize);
        }

        public void SetMainPage(Page page)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                while (true)
                {
                    try
                    {
                        MainPage = page;
                        return;
                    }
                    catch
                    {
                        // There is a very slight chance this will fail (problem comes from Xamarin itself)
                    }
                }
            });
        }

        public static T GetResource<T>(string index)
        {
            return (T)Current.Resources[index];
        }

        public void SetFontSize(double fontSize)
        {
            tlLocationView.Setters.SetFontSize(TLLocationView.TitleFontSizeProperty, fontSize);
            extendedLocationView.Setters.SetFontSize(ExtendedLocationView.TitleFontSizeProperty, fontSize);
            label.Setters.SetFontSize(Label.FontSizeProperty, fontSize);
            tlRichLabel.Setters.SetFontSize(TLRichLabel.FontSizeProperty, fontSize);

            SetSectionsFontSize(fontSize);
            SetTablesFontSize(fontSize);
            SetTextFieldsFontSize(fontSize);
            SetPickersFontSize(fontSize);
            SetButtonsFontSize(fontSize);
        }

        public void LoadInitialResources()
        {
            GroupResourceEnum[] safeResources = new[] { GroupResourceEnum.Common, GroupResourceEnum.Menu };

            IReadOnlyDictionary<GroupResourceEnum, IReadOnlyDictionary<string, string>> resources =
                DependencyService.Resolve<ITranslationTransaction>()
                    .GetPagesTranslations(safeResources);

            Translator.Current.Remove(safeResources);
            Translator.Current.SafeResources(safeResources);
            Translator.Current.Add(resources);

            // Set resources after load
            SetStyles();

            Translator.Current.PropertyChanged += OnTranslatorPropertyChanged;
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }

        private void OnTranslatorPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            SetStyles();
        }

        private void SetStyles()
        {
            string cancelText = TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/Cancel"];

            PickerStyle.Setters.Add(new Setter
            {
                Property = TLPicker.DialogCancelButtonTextProperty,
                Value = cancelText,
            });
            tlNativePicker.Setters.Add(new Setter
            {
                Property = TLNativePicker.DialogCancelButtonTextProperty,
                Value = cancelText,
            });
            tlCheckBoxWithPicker.Setters.Add(new Setter
            {
                Property = TLPicker.DialogCancelButtonTextProperty,
                Value = cancelText,
            });
            MultiPickerStyle.Setters.Add(new Setter
            {
                Property = TLMultiPicker.DialogCancelButtonTextProperty,
                Value = cancelText,
            });
            InfinitePickerStyle.Setters.Add(new Setter
            {
                Property = TLInfinitePicker.DialogCancelButtonTextProperty,
                Value = cancelText,
            });
            CustomInfinitePickerStyle.Setters.Add(new Setter
            {
                Property = CustomInfinitePicker.DialogCancelButtonTextProperty,
                Value = cancelText,
            });
            CustomCheckBoxInfinitePickerStyle.Setters.Add(new Setter
            {
                Property = CustomCheckBoxWithPicker.DialogCancelButtonTextProperty,
                Value = cancelText,
            });
            CustomInfinitePickerStyle.Setters.Add(new Setter
            {
                Property = CustomInfinitePicker.DialogAddButtonTextProperty,
                Value = TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/Add"]
            });

            DateTimePickerStyle.Setters.Add(new Setter
            {
                Property = TLDateTimePicker.DialogTitleProperty,
                Value = TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/DateTimePickerTitle"]
            });
            DateTimePickerStyle.Setters.Add(new Setter
            {
                Property = TLDateTimePicker.DialogCancelTextProperty,
                Value = cancelText,
            });
            DateTimePickerStyle.Setters.Add(new Setter
            {
                Property = TLDateTimePicker.DialogOkayTextProperty,
                Value = TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/Select"]
            });
            DateTimePickerStyle.Setters.Add(new Setter
            {
                Property = TLDateTimePicker.DialogYearTextProperty,
                Value = TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/DateTimeYear"]
            });
            DateTimePickerStyle.Setters.Add(new Setter
            {
                Property = TLDateTimePicker.DialogMonthTextProperty,
                Value = TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/DateTimeMonth"]
            });
            DateTimePickerStyle.Setters.Add(new Setter
            {
                Property = TLDateTimePicker.DialogDayTextProperty,
                Value = TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/DateTimeDay"]
            });
            DateTimePickerStyle.Setters.Add(new Setter
            {
                Property = TLDateTimePicker.DialogHoursTextProperty,
                Value = TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/DateTimeHour"]
            });
            DateTimePickerStyle.Setters.Add(new Setter
            {
                Property = TLDateTimePicker.DialogMinutesTextProperty,
                Value = TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/DateTimeMinute"]
            });

            tlEntryWithType.Setters.Add(new Setter
            {
                Property = TLEntryWithType.EGNLabelProperty,
                Value = TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/EGN"]
            });
            tlEntryWithType.Setters.Add(new Setter
            {
                Property = TLEntryWithType.LNCHLabelProperty,
                Value = TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/LNCH"]
            });
            tlEntryWithType.Setters.Add(new Setter
            {
                Property = TLEntryWithType.FORIDLabelProperty,
                Value = TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/FORID"]
            });

            string totalText = TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/Total"];

            tlTable.Setters.Add(new Setter
            {
                Property = TLTable.TotalLabelProperty,
                Value = totalText
            });
            tlResponsiveTable.Setters.Add(new Setter
            {
                Property = TLResponsiveTable.TotalLabelProperty,
                Value = totalText
            });
            patrolVehiclesView.Setters.Add(new Setter
            {
                Property = TLResponsiveTable.TotalLabelProperty,
                Value = totalText
            });
            catchInspectionsView.Setters.Add(new Setter
            {
                Property = TLResponsiveTable.TotalLabelProperty,
                Value = totalText
            });
            fishingGearsView.Setters.Add(new Setter
            {
                Property = TLResponsiveTable.TotalLabelProperty,
                Value = totalText
            });
            inspectorsView.Setters.Add(new Setter
            {
                Property = TLResponsiveTable.TotalLabelProperty,
                Value = totalText
            });
            logBooksView.Setters.Add(new Setter
            {
                Property = TLResponsiveTable.TotalLabelProperty,
                Value = totalText
            });
            permitLicensesView.Setters.Add(new Setter
            {
                Property = TLResponsiveTable.TotalLabelProperty,
                Value = totalText
            });
            waterFishingGearsView.Setters.Add(new Setter
            {
                Property = TLResponsiveTable.TotalLabelProperty,
                Value = totalText
            });
            waterVesselsView.Setters.Add(new Setter
            {
                Property = TLResponsiveTable.TotalLabelProperty,
                Value = totalText
            });
            waterCatchesView.Setters.Add(new Setter
            {
                Property = TLResponsiveTable.TotalLabelProperty,
                Value = totalText
            });
            enginesView.Setters.Add(new Setter
            {
                Property = TLResponsiveTable.TotalLabelProperty,
                Value = totalText
            });
        }

        private void SetSectionsFontSize(double fontSize)
        {
            sectionView.Setters.SetFontSize(SectionView.FontSizeProperty, fontSize + 2);
            fishingShipView.Setters.SetFontSize(SectionView.FontSizeProperty, fontSize + 2);
            shipChecksView.Setters.SetFontSize(SectionView.FontSizeProperty, fontSize + 2);
            shipCatchesView.Setters.SetFontSize(SectionView.FontSizeProperty, fontSize + 2);
            shipFishingGearsView.Setters.SetFontSize(SectionView.FontSizeProperty, fontSize + 2);
            signaturesView.Setters.SetFontSize(SectionView.FontSizeProperty, fontSize + 2);
            additionalInfoView.Setters.SetFontSize(SectionView.FontSizeProperty, fontSize + 2);
            inspectionFiles.Setters.SetFontSize(SectionView.FontSizeProperty, fontSize + 2);
        }

        private void SetTablesFontSize(double fontSize)
        {
            tlTable.Setters.SetFontSize(TLTable.TitleFontSizeProperty, fontSize + 6);
            tlTable.Setters.SetFontSize(TLTable.FooterFontSizeProperty, fontSize);
            tlResponsiveTable.Setters.SetFontSize(TLResponsiveTable.TitleFontSizeProperty, fontSize + 6);
            tlResponsiveTable.Setters.SetFontSize(TLResponsiveTable.FooterFontSizeProperty, fontSize);
            patrolVehiclesView.Setters.SetFontSize(TLResponsiveTable.TitleFontSizeProperty, fontSize + 6);
            patrolVehiclesView.Setters.SetFontSize(TLResponsiveTable.FooterFontSizeProperty, fontSize);
            catchInspectionsView.Setters.SetFontSize(TLResponsiveTable.TitleFontSizeProperty, fontSize + 6);
            catchInspectionsView.Setters.SetFontSize(TLResponsiveTable.FooterFontSizeProperty, fontSize);
            fishingGearsView.Setters.SetFontSize(TLResponsiveTable.TitleFontSizeProperty, fontSize + 6);
            fishingGearsView.Setters.SetFontSize(TLResponsiveTable.FooterFontSizeProperty, fontSize);
            inspectorsView.Setters.SetFontSize(TLResponsiveTable.TitleFontSizeProperty, fontSize + 6);
            inspectorsView.Setters.SetFontSize(TLResponsiveTable.FooterFontSizeProperty, fontSize);
            permitLicensesView.Setters.SetFontSize(TLResponsiveTable.TitleFontSizeProperty, fontSize + 6);
            permitLicensesView.Setters.SetFontSize(TLResponsiveTable.FooterFontSizeProperty, fontSize);
            logBooksView.Setters.SetFontSize(TLResponsiveTable.TitleFontSizeProperty, fontSize + 6);
            logBooksView.Setters.SetFontSize(TLResponsiveTable.FooterFontSizeProperty, fontSize);
            waterFishingGearsView.Setters.SetFontSize(TLResponsiveTable.TitleFontSizeProperty, fontSize + 6);
            waterFishingGearsView.Setters.SetFontSize(TLResponsiveTable.FooterFontSizeProperty, fontSize);
            waterVesselsView.Setters.SetFontSize(TLResponsiveTable.TitleFontSizeProperty, fontSize + 6);
            waterVesselsView.Setters.SetFontSize(TLResponsiveTable.FooterFontSizeProperty, fontSize);
            waterCatchesView.Setters.SetFontSize(TLResponsiveTable.TitleFontSizeProperty, fontSize + 6);
            waterCatchesView.Setters.SetFontSize(TLResponsiveTable.FooterFontSizeProperty, fontSize);
            enginesView.Setters.SetFontSize(TLResponsiveTable.TitleFontSizeProperty, fontSize + 6);
            enginesView.Setters.SetFontSize(TLResponsiveTable.FooterFontSizeProperty, fontSize);
        }

        private void SetTextFieldsFontSize(double fontSize)
        {
            tlEntry.Setters.SetFontSize(TLEntry.FontSizeProperty, fontSize);
            tlEntry.Setters.SetFontSize(TLEntry.TitleFontSizeProperty, fontSize);
            tlEditor.Setters.SetFontSize(TLEditor.FontSizeProperty, fontSize);
            tlEditor.Setters.SetFontSize(TLEditor.TitleFontSizeProperty, fontSize);
            tlCheckBoxWithEntry.Setters.SetFontSize(TLEntry.FontSizeProperty, fontSize);
            tlCheckBoxWithEntry.Setters.SetFontSize(TLEntry.TitleFontSizeProperty, fontSize);
            tlMultiToggleWithEntry.Setters.SetFontSize(TLEntry.FontSizeProperty, fontSize);
            tlMultiToggleWithEntry.Setters.SetFontSize(TLEntry.TitleFontSizeProperty, fontSize);
            tlEntryWithType.Setters.SetFontSize(TLEntry.FontSizeProperty, fontSize);
            tlEntryWithType.Setters.SetFontSize(TLEntry.TitleFontSizeProperty, fontSize);
        }

        private void SetPickersFontSize(double fontSize)
        {
            BlankPickerStyle.Setters.SetFontSize(TLBlankPicker.FontSizeProperty, fontSize);
            BlankPickerStyle.Setters.SetFontSize(TLBlankPicker.TitleFontSizeProperty, fontSize);
            PickerStyle.Setters.SetFontSize(TLPicker.FontSizeProperty, fontSize);
            PickerStyle.Setters.SetFontSize(TLPicker.TitleFontSizeProperty, fontSize);
            MultiPickerStyle.Setters.SetFontSize(TLPicker.FontSizeProperty, fontSize);
            MultiPickerStyle.Setters.SetFontSize(TLPicker.TitleFontSizeProperty, fontSize);
            InfinitePickerStyle.Setters.SetFontSize(TLInfinitePicker.FontSizeProperty, fontSize);
            InfinitePickerStyle.Setters.SetFontSize(TLInfinitePicker.TitleFontSizeProperty, fontSize);
            tlNativePicker.Setters.SetFontSize(TLNativePicker.FontSizeProperty, fontSize);
            tlNativePicker.Setters.SetFontSize(TLNativePicker.TitleFontSizeProperty, fontSize);
            tlDatePicker.Setters.SetFontSize(TLDatePicker.FontSizeProperty, fontSize);
            tlDatePicker.Setters.SetFontSize(TLDatePicker.TitleFontSizeProperty, fontSize);
            tlTimePicker.Setters.SetFontSize(TLTimePicker.FontSizeProperty, fontSize);
            tlTimePicker.Setters.SetFontSize(TLTimePicker.TitleFontSizeProperty, fontSize);
            DateTimePickerStyle.Setters.SetFontSize(TLDateTimePicker.FontSizeProperty, fontSize);
            DateTimePickerStyle.Setters.SetFontSize(TLDateTimePicker.TitleFontSizeProperty, fontSize);
            CustomInfinitePickerStyle.Setters.SetFontSize(TLInfinitePicker.FontSizeProperty, fontSize);
            CustomInfinitePickerStyle.Setters.SetFontSize(TLInfinitePicker.TitleFontSizeProperty, fontSize);
            tlCheckBoxWithPicker.Setters.SetFontSize(TLPicker.FontSizeProperty, fontSize);
            tlCheckBoxWithPicker.Setters.SetFontSize(TLPicker.TitleFontSizeProperty, fontSize);
            CustomCheckBoxInfinitePickerStyle.Setters.SetFontSize(TLInfinitePicker.FontSizeProperty, fontSize);
            CustomCheckBoxInfinitePickerStyle.Setters.SetFontSize(TLInfinitePicker.TitleFontSizeProperty, fontSize);
        }

        private void SetButtonsFontSize(double fontSize)
        {
            radioButton.Setters.SetFontSize(RadioButton.FontSizeProperty, fontSize);
            button.Setters.SetFontSize(Xamarin.Forms.Button.FontSizeProperty, fontSize);
            tlMenuTextButton.Setters.SetFontSize(TLMenuTextButton.FontSizeProperty, fontSize);
            tlButton.Setters.SetFontSize(TLButton.FontSizeProperty, fontSize);
            tlCheckView.Setters.SetFontSize(TLCheckView.FontSizeProperty, fontSize);
            tlSwitchView.Setters.SetFontSize(TLSwitchView.FontSizeProperty, fontSize);
            tlCheckListView.Setters.SetFontSize(TLCheckListView.FontSizeProperty, fontSize);
            tlMultiToggleView.Setters.SetFontSize(TLMultiToggleView.FontSizeProperty, fontSize);
        }
    }
}
