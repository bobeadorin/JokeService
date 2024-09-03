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
        public static Guid? RetriveDataFromTokenWithPossibleNull(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return null; // Token is null or empty, return null
            }

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token);
                var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "id");

                if (userIdClaim != null)
                {
                    return new Guid(userIdClaim.Value);
                }

                return null; // If no user ID claim is found, return null
            }
            catch (Exception)
            {
                return null; // If token is invalid, return null
            }
        }

        public static Guid RetriveDataFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "id");

            if (userIdClaim != null)
            {
                return new Guid(userIdClaim.Value);
            }

            throw new Exception("User ID claim not found in the token.");
        }
    }
}
