using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IARA.Common.Constants;
using IARA.Common.Enums;
using IARA.Common.Utils;
using IARA.Security;
using IARA.Security.Enum;
using IARA.Security.Enums;
using IARA.Security.SecurityModels;
using IARA.Security.Services;
using IdentityModel;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using TechnoLogica.Authentication.Common;

namespace IARA.IdentityProfile
{
    public abstract class BaseProfileClientService : IProfileClientService
    {
        protected readonly ISecurityService securityService;
        private SecurityEmailSender emailSenderQueue;
        private PasswordHasher passwordHasher;
        private IHttpContextAccessor httpContext;
        protected BaseProfileClientService(PasswordHasher passwordHasher,
                                           ISecurityService securityService,
                                           SecurityEmailSender emailSenderQueue,
                                           IHttpContextAccessor httpContext)
        {
            this.emailSenderQueue = emailSenderQueue;
            this.securityService = securityService;
            this.passwordHasher = passwordHasher;
            this.httpContext = httpContext;
        }

        public abstract string ClientId { get; }

        protected abstract SecurityUser GetUser(string userName, bool searchByPersonId = false);

        public Task<bool> ChangePassword(string scheme, string username, string password, string newPassword)
        {
            throw new NotImplementedException();
        }

        public Task<UserInfo> FindByUsername(string scheme, string username)
        {
            bool searchByPersonId = false;

            if (scheme != LoginSchemes.ID_SRV && EAuthUtils.TryParseIdentifier(username, out IdentificationModel identification))
            {
                searchByPersonId = true;
                username = identification.Identifier;

                SecurityUser user = GetUser(username, searchByPersonId);

                if (user != null)
                {
                    if (!user.HasEauthLogin && !user.EmailConfirmed)
                    {
                        user.EmailConfirmed = true;
                        securityService.UpdateUser(user);
                    }

                    securityService.UpdateUserRoles(user.Id, LoginTypesEnum.EAUTH);

                    return Task.FromResult(new UserInfo
                    {
                        Username = user.UserName,
                        Active = securityService.IsActive(user),
                        SubjectId = user.Id.ToString(),
                        Name = user.UserName
                    });
                }
                else
                {
                    return Task.FromResult(new UserInfo
                    {
                        Username = identification.Identifier,
                        Active = true,
                        SubjectId = "-1",
                        Name = null
                    });
                }
            }
            else
            {
                SecurityUser user = GetUser(username, searchByPersonId);

                if (user != null)
                {
                    securityService.UpdateUserRoles(user.Id, LoginTypesEnum.PASSWORD);

                    return Task.FromResult(new UserInfo
                    {
                        Username = user.UserName,
                        Active = securityService.IsActive(user),
                        SubjectId = user.Id.ToString(),
                        Name = user.UserName
                    });
                }
            }

            return Task.FromResult<UserInfo>(null);
        }


        public Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            int userId = int.Parse(context.Subject.GetSubjectId());

            if (userId != -1)
            {
                LoginTypesEnum loginType = context.Subject.GetAuthenticationMethod() != LoginSchemes.PWD ? LoginTypesEnum.EAUTH : LoginTypesEnum.PASSWORD;
                FillKnownUserClaims(context, userId, loginType);
            }
            else
            {
                string identifier = context.Subject.GetName();

                if (EAuthUtils.TryParseIdentifier(identifier, out IdentificationModel identification))
                {
                    SecurityUser user = null;

                    if (identification.IdentifierType == UserIdentifierTypes.UNKNOWN
                        || identification.IdentifierType == UserIdentifierTypes.EGN
                        || identification.IdentifierType == UserIdentifierTypes.LNCH)
                    {
                        user = securityService.GetUserByPersonIdentifier(identification.Identifier);
                    }
                    else if (identification.IdentifierType == UserIdentifierTypes.VAT_LEGAL_ID)
                    {
                        user = securityService.GetUserByVATNumber(identification.Identifier);
                    }

                    if (user != null)
                    {
                        if (identification.IdentifierType == UserIdentifierTypes.VAT_LEGAL_ID)
                        {
                            securityService.UpdateUserRoles(userId, LoginTypesEnum.EAUTH, true);
                        }
                        else
                        {
                            securityService.UpdateUserRoles(userId, LoginTypesEnum.EAUTH, false);
                        }

                        FillKnownUserClaims(context, user.Id, LoginTypesEnum.EAUTH, securityService.IsActive(user));
                    }
                    else
                    {
                        context.IssuedClaims.Add(new Claim(JwtClaimTypes.Id, userId.ToString()));
                        context.IssuedClaims.Add(new Claim(JwtClaimTypes.Name, identifier));
                        context.IssuedClaims.Add(new Claim(CustomClaims.LoginType, ((byte)LoginTypesEnum.EAUTH).ToString()));
                    }
                }
            }

            return Task.CompletedTask;
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            string subjectId = context.Subject.GetSubjectId();
            if (subjectId != "-1")
            {
                context.IsActive = securityService.IsActive(int.Parse(subjectId));
            }
            else
            {
                context.IsActive = true;
            }

            return Task.CompletedTask;
        }

        public Task<UserRegistrationResult> RegisterUser(string scheme, string name, string userName, string email, string password, Dictionary<string, string> additionalAttributes)
        {
            return Task.FromResult(new UserRegistrationResult
            {
                Succeeded = true,
            });
        }

        public Task<ResetPasswordResult> ResetPasswordAsync(string scheme, string email, string token, string newPassword)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SendPasswordResetTokenAsync(string scheme, string baseAddress, string email)
        {
            SecurityUser user = GetUser(email);
            if (user != null)
            {
                var uri = new Uri(baseAddress);

                emailSenderQueue.EnqueuePasswordEmail(user.Email, SecurityEmailTypes.FORGOTTEN_PASSWORD_MAIL, $"{uri.Scheme}://{uri.Authority}");

                return Task.FromResult(true);
            }
            else
            {
                return Task.FromResult(false);
            }
        }

        public Task SignOutAsync()
        {
            try
            {
                return securityService.LogUserLoginAction(httpContext?.HttpContext, false);
            }
            catch (Exception)
            {
                return Task.CompletedTask;
            }
        }

        public async Task<bool> ValidateCredentials(string scheme, string username, string password)
        {
            SecurityUser user = GetUser(username);
            if (user != null && user.HasUserPassLogin && securityService.IsActive(user))
            {
                bool isPasswordValid = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password) == PasswordVerificationResult.Success;
                if (!isPasswordValid)
                {
                    user.AccessFailedCount++;
                    this.securityService.UpdateUser(user);
                }
                else if (user.AccessFailedCount > 0)
                {
                    user.AccessFailedCount = 0;
                    this.securityService.UpdateUser(user);
                }

                await securityService.LogUserLoginAction(httpContext?.HttpContext, true, isPasswordValid, user.Id.ToString(), user.UserName);

                return isPasswordValid;
            }

            return false;
        }


        private void FillKnownUserClaims(ProfileDataRequestContext context, int userId, LoginTypesEnum loginType, bool addPermissions = true)
        {
            SecurityUser user = securityService.GetUser(userId);
            List<int> userRoles = securityService.GetUserRoles(userId).Concat(securityService.GetUserLegalRoles(userId)).Distinct().ToList();
            List<int> userPermissions = securityService.GetPermissions(userRoles);

            List<Claim> issuedClaims = new List<Claim>
                                       {
                                          new Claim(JwtClaimTypes.Id, user.Id.ToString()),
                                          new Claim(JwtClaimTypes.Name, user.UserName),
                                          new Claim(CustomClaims.LoginType,((byte)loginType).ToString())
                                       };

            if (addPermissions)
            {
                issuedClaims.Add(new Claim(CustomClaims.Permissions, string.Join(',', userPermissions)));
            }

            context.IssuedClaims.AddRange(issuedClaims);
        }

        public Task<UserInfo> FindByUsername(string scheme, string username, List<Claim> externalClaims)
        {
            return FindByUsername(scheme, username);
        }
    }
}
