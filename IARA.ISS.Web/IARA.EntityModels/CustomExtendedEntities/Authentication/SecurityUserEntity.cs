using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using IARA.Common.Constants;
using TL.AspNet.Security.Abstractions.User;

namespace IARA.EntityModels.Entities
{
    public class SecurityUserEntity : IPasswordUser<int>,
                                      ILockoutUser<int>,
                                      IBlockUser<int>,
                                      IEmailConfirmationUser<int>,
                                      IEmailUser<int>,
                                      IEGovExpressionUser<int, SecurityUserEntity>
    {
        [Required]
        [StringLength(100)]
        [Column("Username")]
        public string Username { get; set; }


        [StringLength(200)]
        [Column("Password")]
        public string Password { get; set; }

        [Column("PersonID")]
        public int PersonId { get; set; }

        [Column("ID")]
        public int Id { get; set; }

        [Column("LastFailedLoginAttempt")]
        public DateTime? LastFailedLoginAttempt { get; set; }

        [Column("FailedLoginCount")]
        public int? FailedLoginCount { get; set; }

        [Column("IsLocked")]
        public bool IsLocked { get; set; }

        [Column("LockEndDateTime")]
        public DateTime? LockedUntil { get; set; }

        [Column("LastLoginDate")]
        public DateTime? LastLoginDate { get; set; }

        [NotMapped]
        public bool IsBlocked { get => this.ValidTo < DateTime.Now; set => this.ValidTo = DefaultConstants.MAX_VALID_DATE; }

        [StringLength(100)]
        [Column("ConfirmEmailKey")]
        public string EmailConfirmationToken { get; set; }

        public bool IsEmailConfirmed { get; set; }

        [StringLength(200)]
        public string Email { get; set; }

        public string Egn => this.Person.EgnLnc;

        [Column("ValidFrom", TypeName = "timestamp without time zone")]
        public DateTime ValidFrom { get; set; }
        /// <summary>
        /// Крайна дата на валидност на записа
        /// </summary>
        [Column("ValidTo", TypeName = "timestamp without time zone")]
        public DateTime ValidTo { get; set; }

        [Column(TypeName = "timestamp without time zone")]
        public DateTime? EmailKeyValidTo { get; set; }

        [ForeignKey("PersonId")]
        public virtual Person Person { get; set; }

        public Expression<Func<SecurityUserEntity, bool>> GetEgnExpression(string egn)
        {
            return x => x.Person.EgnLnc == egn;
        }
    }
}
