using Api.Models;
using Common;
using Domain;
using Microsoft.Extensions.Options;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Api.Auth
{
    public interface IJwtFactory
    {
        Task<string> GetToken(string userName, ClaimsIdentity identity);
        ClaimsIdentity GenerateClaimsIdentity(User user);
    }

    public class JwtFactory : IJwtFactory
    {
        private readonly JwtIssuerOptions _jwtOptions;

        public JwtFactory(IOptions<JwtIssuerOptions> jwtOptions)
        {
            _jwtOptions = jwtOptions.Value;
            ThrowIfInvalidOptions(_jwtOptions);
        }


        public async Task<string> GetToken(string userName, ClaimsIdentity identity)
        {
            var claims = new[]
             {
                 new Claim(JwtRegisteredClaimNames.Sub, userName),
                 new Claim(JwtRegisteredClaimNames.Jti, await _jwtOptions.JtiGenerator()),
                 new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(_jwtOptions.IssuedAt).ToString(), ClaimValueTypes.Integer64),
                 identity.FindFirst(Constants.JwtClaimIdentifiers.UserName),
                 identity.FindFirst(Constants.JwtClaimIdentifiers.Id),
                 identity.FindFirst(Constants.JwtClaimIdentifiers.ApiAccess),
             };

            // Create the JWT security token and encode it.
            var jwt = new JwtSecurityToken(
                _jwtOptions.Issuer,
                _jwtOptions.Audience,
                claims,
                _jwtOptions.NotBefore,
                _jwtOptions.Expiration,
                _jwtOptions.SigningCredentials);

            return new JwtSecurityTokenHandler().WriteToken(jwt);

        }

        public ClaimsIdentity GenerateClaimsIdentity(User user)
        {
            return new ClaimsIdentity(new GenericIdentity(user.UserName, "Token"), new[]
            {
                new Claim(Constants.JwtClaimIdentifiers.Id, user.Id),
                new Claim(Constants.JwtClaimIdentifiers.UserName, user.UserName),
                new Claim(Constants.JwtClaimIdentifiers.ApiAccess, $"{ApiAccess.Reader}|{ApiAccess.Contributor}"),
            });
        }

        /// <returns>Date converted to seconds since Unix epoch (Jan 1, 1970, midnight UTC).</returns>
        private static long ToUnixEpochDate(DateTime date)
          => (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);

        private static void ThrowIfInvalidOptions(JwtIssuerOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            if (options.ValidFor <= TimeSpan.Zero)
            {
                throw new ArgumentException("ValidFor must be a non-zero TimeSpan.", nameof(JwtIssuerOptions.ValidFor));
            }

            if (options.SigningCredentials == null)
            {
                throw new ArgumentNullException(nameof(JwtIssuerOptions.SigningCredentials));
            }

            if (options.JtiGenerator == null)
            {
                throw new ArgumentNullException(nameof(JwtIssuerOptions.JtiGenerator));
            }
        }
    }
}
