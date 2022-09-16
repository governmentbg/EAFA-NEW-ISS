using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Xml;
using Microsoft.IdentityModel.Tokens;

namespace IARA.WebHelpers
{
    public sealed class CustomJwtSecurityTokenHandler : JwtSecurityTokenHandler
    {
        private readonly string[] validAuthorities;
        private readonly string[] validAudiences;
        public CustomJwtSecurityTokenHandler(string[] validAuthorities, string[] validAudiences)
        {
            this.validAuthorities = validAuthorities;
            this.validAudiences = validAudiences;
        }


        public override ClaimsPrincipal ValidateToken(string token, TokenValidationParameters validationParameters, out SecurityToken validatedToken)
        {
            return base.ValidateToken(token, validationParameters, out validatedToken);
        }

        public override ClaimsPrincipal ValidateToken(XmlReader reader, TokenValidationParameters validationParameters, out SecurityToken validatedToken)
        {
            return base.ValidateToken(reader, validationParameters, out validatedToken);
        }

        protected override string ValidateIssuer(string issuer, JwtSecurityToken jwtToken, TokenValidationParameters validationParameters)
        {
            if (validAuthorities.Contains(jwtToken.Issuer))
            {
                return jwtToken.Issuer;
            }

            return string.Empty;
        }

        protected override void ValidateIssuerSecurityKey(SecurityKey key, JwtSecurityToken securityToken, TokenValidationParameters validationParameters)
        {
            base.ValidateIssuerSecurityKey(key, securityToken, validationParameters);
        }

        protected override void ValidateAudience(IEnumerable<string> audiences, JwtSecurityToken jwtToken, TokenValidationParameters validationParameters)
        {
            base.ValidateAudience(audiences, jwtToken, validationParameters);
        }

    }
}
