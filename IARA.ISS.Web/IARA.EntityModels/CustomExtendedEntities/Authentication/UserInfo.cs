using System;
using System.ComponentModel.DataAnnotations.Schema;
using IARA.Common.Constants;
using TL.AspNet.Security.Abstractions.User;

namespace IARA.EntityModels.Entities
{
    public partial class UserInfo : IEmailConfirmationUser<int>, ILockoutUser<int>, IBlockUser<int>
    {
        [NotMapped]
        public DateTime? LockedUntil { get => this.LockEndDateTime; set => this.LockEndDateTime = value; }

        [NotMapped]
        public string EmailConfirmationToken { get => this.ConfirmEmailKey; set => this.ConfirmEmailKey = value; }

        [NotMapped]
        public string Email => this.User.Email;

        [NotMapped]
        public bool IsBlocked { get => this.User.ValidTo < DateTime.Now; set => this.User.ValidTo = DefaultConstants.MAX_VALID_DATE; }

        [NotMapped]
        public int Id => this.UserId;

        [NotMapped]
        public string Username => this.User.Username;


        [NotMapped]
        int? ILockoutUser<int>.FailedLoginCount { get => this.FailedLoginCount; set => this.FailedLoginCount = (short)value; }
    }
}
