using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace Anymate.UiPath
{
    public static class TokenValidator
    {
        public static string CustomerKey => "auth.anymate.app/CustomerKey";
        public static string UserType => "auth.anymate.app/UserType";

      
        private static long GetExpiryEpochFromToken(string access_token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(access_token) as JwtSecurityToken;
            var expire_epoch = Convert.ToInt64(jsonToken.Claims.FirstOrDefault(claim => claim.Type == "exp").Value);
            if (expire_epoch == 0)
                throw new Exception("Token invalid");

            return expire_epoch;
        }

        private static bool RefreshNotNeeded(long expiry_epoch)
        {
            var expiryOffset = expiry_epoch.ToDateTimeOffsetFromEpoch();
            return expiryOffset > DateTimeOffset.UtcNow.AddMinutes(15);
        }

        public static bool RefreshNotNeeded(string access_token)
        {
            var exp = GetExpiryEpochFromToken(access_token);
            return RefreshNotNeeded(exp);
        }


    }
}
