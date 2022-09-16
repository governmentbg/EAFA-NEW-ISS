using IARA.Mobile.Application.DTObjects.Users;
using IARA.Mobile.Application.Interfaces.Transactions;
using IARA.Mobile.Application.Interfaces.Utilities;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Shared.Attributes.PasswordAttributes;
using IARA.Mobile.Shared.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Windows.Input;
using TechnoLogica.Xamarin.Attributes;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ViewModels.Models;
using Xamarin.Forms;

namespace IARA.Mobile.Shared.ViewModels
{
    public class ChangePasswordViewModel : BasePageViewModel
    {
        public ChangePasswordViewModel()
        {
            ChangePassword = CommandBuilder.CreateFrom(OnChangePassword);

            this.AddValidation();
        }

        protected IPasswordTransaction PasswordTransaction =>
            DependencyService.Resolve<IPasswordTransaction>();

        public ICommand ChangePassword { get; }

        [Required]
        [MaxLength(200)]
        public ValidState OldPassword { get; set; }

        [Required]
        [MaxLength(200)]
        [RequireLowerCase]
        [RequireUpperCase]
        [RequireMinLength(8)]
        [RequireSpecialSymbolOrDigit]
        public ValidState NewPassword { get; set; }

        [Required]
        [MaxLength(200)]
        [EqualTo(nameof(NewPassword))]
        [UpdateFrom(nameof(NewPassword), MustBeDirty = false)]
        public ValidState VerifyPassword { get; set; }

        public override GroupResourceEnum[] GetPageIndexes()
        {
            return Array.Empty<GroupResourceEnum>();
        }

        public override IReadOnlyDictionary<GroupResourceEnum, IReadOnlyDictionary<string, string>> GetPageResources(out GroupResourceEnum[] filtered)
        {
            filtered = Array.Empty<GroupResourceEnum>();
            return new Dictionary<GroupResourceEnum, IReadOnlyDictionary<string, string>>();
        }

        public override Task Initialize(object sender)
        {
            return Task.CompletedTask;
        }

        private async Task OnChangePassword()
        {
            Validation.Force();

            if (!Validation.IsValid)
            {
                return;
            }

            bool result = await PasswordTransaction.ChangePassword(new UserPasswordDto
            {
                NewPassword = NewPassword,
                OldPassword = OldPassword,
            });

            if (result)
            {
                DependencyService.Resolve<ICommonLogout>()
                    .DeleteLocalInfo();
            }
        }
    }
}
