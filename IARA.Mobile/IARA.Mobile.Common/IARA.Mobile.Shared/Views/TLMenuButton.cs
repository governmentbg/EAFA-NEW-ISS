using IARA.Mobile.Shared.Popups;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Extensions;
using Xamarin.Forms;

namespace IARA.Mobile.Shared.Views
{
    public class TLMenuButton : ImageButton
    {
        public static readonly BindableProperty ChoicesProperty =
            BindableProperty.Create(nameof(Choices), typeof(List<MenuOption>), typeof(TLMenuButton));

        public static new readonly BindableProperty CommandProperty =
            BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(TLMenuButton));

        public List<MenuOption> Choices
        {
            get => (List<MenuOption>)GetValue(ChoicesProperty);
            set => SetValue(ChoicesProperty, value);
        }

        public new ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        /// <summary>
        /// For internal use only.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public async void OnViewTouch(double x, double y)
        {
            List<MenuOption> choices = Choices;

            if (choices == null || choices.Count == 0)
            {
                throw new InvalidOperationException($"Cannot show menu, property {nameof(Choices)} is null or has no items.");
            }

            MenuDimension dimension = new MenuDimension(x, y, Width, Height);
            TaskCompletionSource<MenuOption> completionSource = new TaskCompletionSource<MenuOption>();

            MenuListPopup popup = new MenuListPopup(choices,
                CommandBuilder.CreateFrom<(MenuOption Option, MenuListPopup Popup)>((option) =>
                {
                    completionSource.TrySetResult(option.Option);
                    return PopupNavigation.Instance.RemovePageAsync(option.Popup);
                }),
                dimension
            );

            popup.Disappearing += (_, __) => completionSource.TrySetResult(null);

            await PopupNavigation.Instance.PushAsync(popup);

            MenuOption result = await completionSource.Task;

            if (result != null)
            {
                Command?.ExecuteCommand(new MenuResult
                {
                    Option = result.Option,
                    Parameter = CommandParameter
                });
            }
        }
    }
}
