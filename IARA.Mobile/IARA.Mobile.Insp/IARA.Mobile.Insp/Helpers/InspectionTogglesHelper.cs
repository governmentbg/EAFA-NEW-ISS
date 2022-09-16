using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Insp.Domain.Enums;
using System.Collections.Generic;
using TechnoLogica.Xamarin.ResourceTranslator;
using TechnoLogica.Xamarin.ViewModels.Models;
using Xamarin.Forms;

namespace IARA.Mobile.Insp.Helpers
{
    public static class InspectionTogglesHelper
    {
        public static List<ToggleOption> YesNoMultiToggles { get; } = new List<ToggleOption>
        {
            new ToggleOption
            {
                Value = nameof(CheckTypeEnum.Y),
                Text = TranslateExtension.Translator["Common/Yes"],
                SelectedColor = Color.Green
            },
            new ToggleOption
            {
                Value = nameof(CheckTypeEnum.N),
                Text = TranslateExtension.Translator["Common/No"],
                SelectedColor = Color.Red
            }
        };

        public static List<ToggleOption> YesNoNotApplicableMultiToggles { get; } = new List<ToggleOption>
        {
            new ToggleOption
            {
                Value = nameof(CheckTypeEnum.Y),
                Text = TranslateExtension.Translator["Common/Yes"],
                SelectedColor = Color.Green
            },
            new ToggleOption
            {
                Value = nameof(CheckTypeEnum.N),
                Text = TranslateExtension.Translator["Common/No"],
                SelectedColor = Color.Red
            },
            new ToggleOption
            {
                Value = nameof(CheckTypeEnum.X),
                Text = TranslateExtension.Translator["Common/NotApplicable"],
                SelectedColor = Color.Gray
            }
        };

        public static List<ToggleOption> YesNoUnavailableMultiToggles { get; } = new List<ToggleOption>
        {
            new ToggleOption
            {
                Value = nameof(InspectedFishingGearEnum.Y),
                Text = TranslateExtension.Translator["Common/Yes"],
                SelectedColor = Color.Green
            },
            new ToggleOption
            {
                Value = nameof(InspectedFishingGearEnum.N),
                Text = TranslateExtension.Translator["Common/No"],
                SelectedColor = Color.Red
            },
            new ToggleOption
            {
                Value = nameof(InspectedFishingGearEnum.R),
                Text = TranslateExtension.Translator["Common/Unavailable"],
                SelectedColor = Color.Gray
            }
        };

        public static List<ToggleOption> YesNoUnavailableAsDefaultMultiToggles { get; } = new List<ToggleOption>
        {
            new ToggleOption
            {
                Value = nameof(CheckTypeEnum.Y),
                Text = TranslateExtension.Translator["Common/Yes"],
                SelectedColor = Color.Green
            },
            new ToggleOption
            {
                Value = nameof(CheckTypeEnum.N),
                Text = TranslateExtension.Translator["Common/No"],
                SelectedColor = Color.Red
            },
            new ToggleOption
            {
                Value = nameof(CheckTypeEnum.X),
                Text = TranslateExtension.Translator["Common/Unavailable"],
                SelectedColor = Color.Gray
            }
        };

        public static List<ToggleOption> CoincidesMultiToggles { get; } = new List<ToggleOption>
        {
            new ToggleOption
            {
                Value = nameof(CheckTypeEnum.Y),
                Text = TranslateExtension.Translator["Common/Coincides"],
                SelectedColor = Color.Green
            },
            new ToggleOption
            {
                Value = nameof(CheckTypeEnum.N),
                Text = TranslateExtension.Translator["Common/DoesNotCoincide"],
                SelectedColor = Color.Red
            },
            new ToggleOption
            {
                Value = nameof(CheckTypeEnum.X),
                Text = TranslateExtension.Translator["Common/Unavailable"],
                SelectedColor = Color.Gray
            }
        };
    }
}
