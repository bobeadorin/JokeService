using Azure.Core;
using System;
using System.IdentityModel.Tokens.Jwt;

namespace JokeService.Utility
{
    public class JwtUtility
    {
        public static DateTime ExtractExpirationDate(string token)
        {
            JwtSecurityToken parsedToken = new JwtSecurityToken(token);
            DateTime expirationDate = parsedToken.ValidTo;

            return expirationDate;
        }
    }
}
